using MegaTravelAPI.IRepository;
using MegaTravelAPI.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using System.Text;

namespace MegaTravelAPI.Data
{
    public class UserDAL : IUser
    {
        //context for the database connection
        private readonly MegaTravelContext context;

        private readonly IConfiguration _config;

        public UserDAL(MegaTravelContext Context, IConfiguration config)
        {
            context = Context;
            _config = config;
        }

        #region Get All Users Method
        /// <summary>
        /// Method that retrieves all users in the database
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllUsers()
        {
            List<User> userList = new List<User>();

            try
            {
                //query the database to get all of the users
                var users = context.Users.ToList();

                foreach (var user in users)
                {
                    userList.Add(new User()
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Street1 = user.Street1,
                        Street2 = user.Street2,
                        City = user.City,
                        State = user.State,
                        ZipCode = user.ZipCode,
                        Phone = user.Phone

                    });
                }

                if(userList.Count == 0)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllUsers --- " + ex.Message);
                throw;
            }

            return userList;
        }

        #endregion

        #region Save User Record Method
        /// <summary>
        /// save the user registration record into database
        /// </summary>
        /// <param name="usermodel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<SaveUserResponse> SaveUserRecord(RegistrationModel usermodel)
        {
            //set up a new object to hold our user information
            User objApplicationUser;

            //set up a response status object to hold the response
            SaveUserResponse res = new SaveUserResponse();

            try
            {
                //use the incoming information to populate our new user
                objApplicationUser = new User
                {
                    FirstName = usermodel.FirstName,
                    LastName = usermodel.LastName,
                    Email = usermodel.Email,
                    Street1 = usermodel.Street1,
                    Street2 = usermodel.Street2,
                    City = usermodel.City,
                    State = usermodel.State,
                    ZipCode = usermodel.ZipCode,
                    Phone = usermodel.Phone


                };

                //populate the Login information
                objApplicationUser.LoginInfo = new Login
                {
                    UserName = usermodel.Username,
                    Password = usermodel.Password,
                    UserType = "User"
                };

                //get the ID of the most recent user added to the DB
                var highestID = context.Users.Max(x => x.UserId);

                //set the user ID to be the next ID value
                objApplicationUser.UserId = highestID + 1;

                    //save this user in the database
                    using(var db = new MegaTravelContext(_config))
                {
                    db.Users.Add(objApplicationUser);
                    db.SaveChanges();
                }

                //update the login table with the username and password
                context.Database.ExecuteSqlRaw("[dbo].[AddLogin] @ID, @USERNAME, @PASSWORD, @TYPE",
                            new SqlParameter("@ID", objApplicationUser.UserId),
                            new SqlParameter("@USERNAME", objApplicationUser.LoginInfo.UserName),
                            new SqlParameter("@PASSWORD", objApplicationUser.LoginInfo.Password),
                            new SqlParameter("@TYPE", objApplicationUser.LoginInfo.UserType));


                //set the success to pass the data back
                res.StatusCode = 200;
                res.Message = "Save Successful";
                res.Status = true;
                res.Data = objApplicationUser;
                res.UserId = objApplicationUser.UserId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveUserRecord --- " + ex.Message);
                res.Status = false;
                res.StatusCode = 0;
            }
            return res;
        }
        #endregion

        
        #region Login User Method
        public async Task<LoginResponse> LoginUser(LoginModel tokenData)
        {
            LoginResponse res = new LoginResponse();
            try
            {
                if(tokenData != null)
                {
                    //look for the user in the database
                    var query = context.Logins
                    .Where(x => x.UserName == tokenData.Username && x.Password == tokenData.Password)
                    .FirstOrDefault<Login>();

                    //if query has a result then we have a match
                    if(query != null)
                    {
                        res.Status = true;
                        res.StatusCode = 200;
                        res.Message = "Login Success";

                        //get the user data so we can send it back with
                        UserData user = await FindByName(query.UserName);
                        res.Data = new UserData
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Street1 = user.Street1,
                            Street2 = user.Street2,
                            City = user.City,
                            State = user.State,
                            ZipCode = user.ZipCode,
                            Phone = user.Phone
                        };
                        return res;
                    }
                    else
                    {
                        //the user wasn't found or wasn't a match
                        res.Status = false;
                        res.StatusCode = 500;
                        res.Message = "Login Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoginUser --- " + ex.Message);
                res.Status = false;
                res.StatusCode = 500;
            }

            return res;
        }
        #endregion

        #region Find User By Name Method

        public async Task<UserData> FindByName(string username)
        {
            UserData user = null;

            try
            {
                if (username != null)
                {
                    //query the database to find the user who has this username
                    var query = context.Users
                        .Where(x => x.LoginInfo.UserName == username)
                        .FirstOrDefault<User>();

                    if (query != null)
                    {
                        //set up the object so we can return it
                        user = new UserData
                        {
                            UserId = query.UserId,
                            FirstName = query.FirstName,
                            LastName = query.LastName,
                            Email = query.Email,
                            Street1 = query.Street1,
                            Street2 = query.Street2,
                            City = query.City,
                            State = query.State,
                            ZipCode = query.ZipCode,
                            Phone = query.Phone
                        };
                    }
                    
                }

                return user;


            }
            catch (Exception ex)
            {
                Console.WriteLine("FindByName --- " + ex.Message);
            }

            return user;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usermodel"></param>
        /// <returns></returns>
        #region Update User Record Method
        public async Task<SaveUserResponse> UpdateUserRecord(UserData usermodel)
        {
            

            //set up a response status object to hold the response
            SaveUserResponse res = new SaveUserResponse();
            try{
                //open database connection
                using (var db = new MegaTravelContext(_config))
                {
                    //find the current user in the data that matches the data of the current user im trying to update
                    var result = db.Users.SingleOrDefault(u => u.UserId == usermodel.UserId);

                    //check I actually found them
                    if (result != null)
                    {
                        //update all the field with incoming data
                        result.FirstName = usermodel.FirstName;
                        result.LastName = usermodel.LastName;
                        result.Email = usermodel.Email;
                        result.Street1 = usermodel.Street1;
                        result.Street2 = usermodel.Street2;
                        result.City = usermodel.City;
                        result.State = usermodel.State;
                        result.ZipCode = usermodel.ZipCode;
                        result.Phone = usermodel.Phone;

                        //save obj to database
                        db.SaveChanges();
                        //set status code and pass the data back through response
                        res.Status = true;
                        res.StatusCode = 200;
                        res.Message = "Update Successful";
                        res.Data = result;
                        res.UserId = result.UserId;
                    }

                }
                
            }
            //catch error
            catch(Exception ex)
            {
                //set status 
                Console.WriteLine("Update User Record -------- "+ ex);
                res.StatusCode = 500;
                res.Status = false;
            }

            return (res);
        }



        #endregion

        #region Get Trip by Users Method
        /// <summary>
        /// Method that retrieves all users in the database
        /// </summary>
        /// <returns></returns>
        public List<Trip> GetAllTripsByUser(int userID)
        {
            List<Trip> tripList = new List<Trip>();
            
            try
            {
                //query database to gell trips associated with this userid
                //var trips = context.Trips.ToList();

                //foreach (var trip in trips)
                //{
                //    if (trip.UserId == userID)
                //    {
                //        tripList.Add(trip);
                //    }
                //}

                tripList = context.Trips
                    .Where(x => x.UserId == userID).ToList();

                if (tripList != null)
                {
                    foreach (var trip in tripList)
                    {//get payment status for trip
                        var payment = context.TripPayment
                            .Where(x => x.TripId == trip.TripId)
                            .FirstOrDefault();
                        //if there is no payment record associated with this trips
                        //we can send back an empty object so we dont get null refrence
                        TripPayment paymentStatus = new TripPayment();
                        //if it not null
                        //populate object properties
                        if (paymentStatus != null)
                        {
                            paymentStatus.PaymentId = payment.PaymentId;
                            paymentStatus.TripId = payment.TripId;
                            paymentStatus.PaymentStatus = payment.PaymentStatus;

                        }
                        trip.TripPayment = paymentStatus;
                        trip.UserId = userID;
                    }
                }
                




                if (tripList.Count == 0)
                {
                    return (null);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTripByUser"+ex.Message);
            }

            return (tripList);
        }

        #endregion

        public async Task<TripPaymentResponseModel> SetTripPaymentStatus(int tripId)
        {
            TripPaymentResponseModel res = new TripPaymentResponseModel();
            try
            {
                //open database connection
                using (var db = new MegaTravelContext(_config))
                {
                    //find the current user in the data that matches the data of the current user im trying to update
                    var result = db.TripPayment.SingleOrDefault(u => u.TripId == tripId);
                    //check I actually found them
                    if (result != null)
                    {
                        //update all the field with incoming data
                        result.PaymentStatus = !result.PaymentStatus;
                        
                        //save obj to database
                        db.SaveChanges();
                        //set status code and pass the data back through response
                        res.Status = true;
                        res.StatusCode = 200;
                        res.Message = "Update Successful";
                        res.TripPayment = result;

                    }

                }

            }
            //catch error
            catch (Exception ex)
            {
                //set status 
                Console.WriteLine("Set Trip Payment Status -------- " + ex);
                res.StatusCode = 500;
                res.Status = false;
            }


            return (res);
        }
    }
}
