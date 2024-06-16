using Microsoft.AspNetCore.Mvc;
using MegaTravelAPI.Data;
using MegaTravelAPI.IRepository;
using MegaTravelAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MegaTravelAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        // private readonly IUser repository;
        private readonly IUser repository;

        //context for the database connection
        private readonly MegaTravelContext context;

        //variable for holding the configuration data for login authentication
        private IConfiguration config;

        public UserController(IConfiguration Config)
        {
            config = Config;
            context = new MegaTravelContext(config);
            repository = new UserDAL(context, config);
            

        }

        [HttpGet("GetUsers", Name = "GetUsers")]
        [AllowAnonymous]
        public async Task<GetUsersResponseModel> GetAllUsers()
        {
            GetUsersResponseModel response = new GetUsersResponseModel();

            //set up a list to hold the incoming users we will get from the db
            List<User> userList = new List<User>();
            try
            {
                userList = repository.GetAllUsers();

                //check the list isn't empty
                if(userList.Count != 0)
                {
                    response.Status = true;
                    response.StatusCode = 200;
                    response.userList = userList;
                }
                else
                {
                    //there has been an error
                    response.Status = false;
                    response.Message = "Get Failed";
                    response.StatusCode = 0;
                }

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Get Failed";
                response.StatusCode = 0;
                //there has been an error
                Console.WriteLine(ex.Message);
            }
            return response;
        }


        

        [HttpPost("RegisterUser", Name = "RegisterUser")]
        [AllowAnonymous]
        public async Task<CreateUserResponseModel> RegisterUser(RegistrationModel userData)
        {
            CreateUserResponseModel response = new CreateUserResponseModel();

            if (userData != null)
            {
                try
                {
                    //call the method that will save the user record
                    var user = await repository.SaveUserRecord(userData).ConfigureAwait(true);

                    if (user.StatusCode == 200) //status success
                    {
                        //user has been added
                        
                        response.Status = true;
                        response.Message = "Registration Successful";
                        response.StatusCode = 200;

                        // send back the user information we just added
                        response.Data = new UserData();

                        response.Data.UserId = user.Data.UserId;
                        response.Data.FirstName = user.Data.FirstName;
                        response.Data.LastName = user.Data.LastName;
                        response.Data.Email = user.Data.Email;
                        response.Data.Phone = user.Data.Phone;
                        response.Data.Street1 = user.Data.Street1;
                        response.Data.Street2 = user.Data.Street2;
                        response.Data.City = user.Data.City;
                        response.Data.State = user.Data.State;
                        response.Data.ZipCode = user.Data.ZipCode;
                        response.Data.Phone = user.Data.Phone;
                        
                    }
                    else
                    {
                        //there has been an error
                        response.Status = false;
                        response.Message = "Registration Failed";
                        response.StatusCode = 0;
                        
                    }
                }
                catch (Exception ex)
                {
                    //there has been an error
                    response.Status = false;
                    response.Message = "Registration Failed";
                    response.StatusCode = 0;
                    Console.WriteLine(ex.Message);
                }
               

            }

            return response;
        }

        [HttpPost("LoginUser", Name = "LoginUser")]
        [AllowAnonymous]
        public async Task<LoginResponse> LoginUser(LoginModel tokenData)
        {
            LoginResponse response = new LoginResponse();
            try
            {
                if(tokenData != null)
                {
                    //call the method that will check the user credentials
                    var loginResult = await repository.LoginUser(tokenData).ConfigureAwait(true);

                    if(loginResult.StatusCode == 200)
                    {
                        //login check has succeeded

                        //query the database to get the information about our logged in user
                        var user = await repository.FindByName(tokenData.Username).ConfigureAwait(true);

                        if(user != null)
                        {
                            //generate the authentication token
                            var tokenString = GenerateJwtToken(tokenData);

                            response.StatusCode = 200;
                            response.Status = true;
                            response.Message = "Login Successful";
                            response.AccountType = tokenData.UserType;
                            response.Authtoken = tokenString;
                            response.Data = new UserData
                            {
                                UserId = user.UserId,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Street1 = user.Street1,
                                Street2 = user.Street2,
                                City = user.City,
                                State = user.State,
                                ZipCode = user.ZipCode,
                                Phone = user.Phone
                            };

                            return response;
                        }                        
                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.Status = false;
                        response.Message = "Login Failed";
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoginUser --- " + ex.Message);
                response.StatusCode = 500;
                response.Status = false;
                response.Message = "Login Failed";
            }

            return response;
        }
        [HttpPost("UpdateUser",Name ="UpdateUser")]
        [AllowAnonymous]

        public async Task<CreateUserResponseModel> UpdateUser(UserData userData)
        {
            //setup response obj
            CreateUserResponseModel res = new CreateUserResponseModel();

            //check client didnt sent a null
            if (userData != null)
            {
                try
                {
                    //call method that will update the record
                    var user = await repository.UpdateUserRecord(userData).ConfigureAwait(true);
                    //check status code
                    if (user.StatusCode == 200)
                    {
                        //user has been update fine
                        res.Status = true;
                        res.Message = "update Successful";
                        res.StatusCode = 200;

                        //send back the user data object we updated
                        res.Data = new UserData();
                        res.Data.UserId = user.Data.UserId;
                        res.Data.FirstName = user.Data.FirstName;
                        res.Data.LastName = user.Data.LastName;
                        res.Data.Phone = user.Data.Phone;
                        res.Data.Email = user.Data.Email;
                        res.Data.Street1 = user.Data.Street1;
                        res.Data.Street2 = user.Data.Street2;
                        res.Data.City = user.Data.City;
                        res.Data.State = user.Data.State;
                        res.Data.ZipCode = user.Data.ZipCode;

                    }
                    else
                    {
                        //there is an error
                        res.Status = false;
                        res.StatusCode = 500;
                        res.Message = "update failed";

                    }


                }
                catch (Exception ex)
                {
                    //there is an error
                    res.Status = false;
                    res.StatusCode = 500;
                    res.Message = "update failed";
                    Console.WriteLine(ex);
                }
            }
            return (res);

        }

        [HttpGet("GetAllTripsByUser", Name = "GetAllTripsByUser")]
        [AllowAnonymous]
        public GetTripForUsersResponseModel GetAllTripsByUser(int userID)
        {
            GetTripForUsersResponseModel res = new GetTripForUsersResponseModel();

            try
            {
                //set up list
                List<Trip> tripList = new List<Trip>();
                //call the method of GetAllTripsByUser written in dal
                tripList = repository.GetAllTripsByUser(userID);

                //check list isn't empty
                if (tripList.Count != 0)
                {
                    res.Status = true;
                    res.Message = "Get successful";
                    res.StatusCode = 200;
                    res.tripList = tripList;
                     
                }
                else
                {
                    res.Status = false;
                    res.Message = "Get failed";
                    res.StatusCode = 0;
                }

            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = "Get failed" + ex.Message;
                res.StatusCode = 0;
                Console.WriteLine("GetAllTripsByUser User Controlller"+ ex.Message);
            }

            return res;
        }

        [HttpPost("SetTripPayment", Name = "SetTripPayment")]
        [AllowAnonymous]

        public async Task<TripPaymentResponseModel> SetTripPayment(int tripId)
        {
            //setup response obj
            TripPaymentResponseModel res = new TripPaymentResponseModel();

            //check client didnt sent a null
            if (tripId != null)
            {
                try
                {
                    //call method that will update the record
                    var trip = await repository.SetTripPaymentStatus(tripId).ConfigureAwait(true);
                    //check status code
                    if (trip.StatusCode == 200)
                    {
                        //user has been update fine
                        res.Status = true;
                        res.Message = "update Successful";
                        res.StatusCode = 200;

                        //send back the user data object we updated
                        res.TripPayment = trip.TripPayment;


                    }
                    else
                    {
                        //there is an error
                        res.Status = false;
                        res.StatusCode = 500;
                        res.Message = "update failed";
                        
                    }


                }
                catch (Exception ex)
                {
                    //there is an error
                    res.Status = false;
                    res.StatusCode = 500;
                    res.Message = "update failed";
                    Console.WriteLine(ex);
                }
            }
            return (res);

        }

        /// <summary>
        /// generate the token for registration
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GenerateJwtToken(LoginModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenAuthentication:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                //new Claim(JwtRegisteredClaimNames., userInfo.Password),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            DateTime expiredDate = DateTime.UtcNow.AddDays(15);
            var token = new JwtSecurityToken(config["TokenAuthentication:Issuer"],
                config["TokenAuthentication:Issuer"],
                claims,
                expires: expiredDate,
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}