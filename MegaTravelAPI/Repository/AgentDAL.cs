using MegaTravelAPI.IRepository;
using MegaTravelAPI.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using System.Text;

namespace MegaTravelAPI.Data
{
    public class AgentDAL : IAgent
    {
        //context for the database connection
        private readonly MegaTravelContext context;

        public AgentDAL(MegaTravelContext Context)
        {
            context = Context;
        }


        #region Login Agent Method
        public async Task<LoginResponse> LoginAgent(LoginModel tokenData)
        {
            LoginResponse res = new LoginResponse();
            try
            {
                if(tokenData != null)
                {
                    //look for the agent in the database
                    var query = context.Agents
                    .Where(x => x.LoginInfo.UserName == tokenData.Username && x.LoginInfo.Password == tokenData.Password)
                    .FirstOrDefault<Agent>();

                    //if query has a result then we have a match
                    if(query != null)
                    {
                        res.Status = true;
                        res.StatusCode = 200;
                        res.Message = "Login Success";
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
                Console.WriteLine("LoginAgent --- " + ex.Message);
                res.Status = false;
                res.StatusCode = 500;
            }

            return res;
        }
        #endregion

        #region Find Agent By Name Method

        public async Task<Agent> FindByName(string username)
        {
            Agent agent = null;

            try
            {
                if (username != null)
                {
                    //query the database to find the agent who has this username
                    var query = context.Agents
                        .Where(x => x.LoginInfo.UserName == username)
                        .FirstOrDefault<Agent>();

                    if (query != null)
                    {
                        //set up the object so we can return it
                        agent = new Agent
                        {
                            AgentId = query.AgentId,
                            FirstName = query.FirstName,
                            LastName = query.LastName,
                            OfficeLocation = query.OfficeLocation,
                            Phone = query.Phone
                            
                        };
                    }

                }

                return agent;


            }
            catch (Exception ex)
            {
                Console.WriteLine("FindByName --- " + ex.Message);
            }

            return agent;
        }
        #endregion

        #region Find Agent By AgentID Method
        public async Task<Agent> FindByID(int agentID)
        {
            Agent agent = null;

            try
            {
                if (agentID != null)
                {
                    //query the database to find the agent who has this username
                    var query = context.Agents
                        .Where(x => x.LoginInfo.Id == agentID)
                        .FirstOrDefault<Agent>();

                    if (query != null)
                    {
                        //set up the object so we can return it
                        agent = new Agent
                        {
                            AgentId = query.AgentId,
                            FirstName = query.FirstName,
                            LastName = query.LastName,
                            OfficeLocation = query.OfficeLocation,
                            Phone = query.Phone

                        };
                    }

                }

                return agent;


            }
            catch (Exception ex)
            {
                Console.WriteLine("FindByID --- " + ex.Message);
            }

            return agent;
        }
        #endregion

        #region Get All Trips Method
        /// <summary>
        /// Method to get all trips in the database
        /// </summary>
        /// <returns></returns>

        public List<TripData> GetAllTrips()
        {
            List<TripData> tripList = new List<TripData>();

            try
            {
                //query the database to get all of the trips
                var trips = context.Trips.ToList();

                foreach(Trip trip in trips)
                {
                    //get all of the object data to send back
                    tripList.Add(new TripData()
                    {
                        UserID = trip.UserId,
                        AgentID = trip.AgentId,
                        TripID = trip.TripId,
                        TripName = trip.TripName,
                        Location = trip.Location,
                        StartDate = trip.StartDate,
                        EndDate = trip.EndDate,
                        NumAdults = trip.NumAdults,
                        NumChildren = trip.NumChildren
                    });


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllTrips --- " + ex.Message);
                throw;
            }

            return tripList;
        }
        #endregion

        #region Get All Trips For Agent
        /// <summary>
        /// Method to get all trips in the database for a particular agent
        /// </summary>
        /// <returns></returns>

        public List<TripData> GetAllTripsForAgent(int agentID)
        {
            List<TripData> tripList = new List<TripData>();
            UserData selectedUser = null;
            try
            {
                //query the database to get all of the trips for this agentID
                var trips = context.Trips.Where(x=> x.AgentId == agentID).ToList();

                foreach (Trip trip in trips)
                {
                    //query the database to get the user data using the userid
                    var query = context.Users.Where(x => x.UserId == trip.UserId)
                        .FirstOrDefault<User>();

                    //check to see if users is found
                    if (query != null)
                    {
                        //we found the user
                        //set up the Userdata ovject so we can return it
                        selectedUser = new UserData
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
                        //same as
                        //Userdata myUser = new Userdata();
                        //myUser.UserID = query.UserId;
                    }


                    //get all of the object data to send back
                    tripList.Add(new TripData()
                    {

                        UserID = trip.UserId,
                        AgentID = trip.AgentId,
                        TripID = trip.TripId,
                        TripName = trip.TripName,
                        Location = trip.Location,
                        StartDate = trip.StartDate,
                        EndDate = trip.EndDate,
                        NumAdults = trip.NumAdults,
                        NumChildren = trip.NumChildren,
                        UserInfo = selectedUser //add the new populated user refrence

                    });


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllTripsForAgent --- " + ex.Message);
                throw;
            }

            return tripList;
        }
        #endregion

    }
}
