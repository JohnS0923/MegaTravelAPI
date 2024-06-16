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
    public class AgentController : ControllerBase
    {
        // private readonly IUser repository;
        private readonly IAgent repository;

        //context for the database connection
        private readonly MegaTravelContext context;

        //variable for holding the configuration data for login authentication
        private IConfiguration config;

        public AgentController(IConfiguration Config)
        {
            config = Config;
            context = new MegaTravelContext(config);
            repository = new AgentDAL(context);


        }

        #region Login Agent
        /// <summary>
        /// Method to log in an agent for authentication
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        [HttpPost("LoginAgent", Name = "LoginAgent")]
        [AllowAnonymous]
        public async Task<LoginResponse> LoginUser(LoginModel tokenData)
        {
            LoginResponse response = new LoginResponse();
            try
            {
                if (tokenData != null)
                {
                    //call the method that will check the user credentials
                    var loginResult = await repository.LoginAgent(tokenData).ConfigureAwait(true);

                    if (loginResult.StatusCode == 200)
                    {
                        //login check has succeeded

                        //query the database to get the information about our logged in user
                        var agent = await repository.FindByName(tokenData.Username).ConfigureAwait(true);

                        if (agent != null)
                        {
                            //generate the authentication token
                            var tokenString = GenerateJwtToken(tokenData);

                            response.StatusCode = 200;
                            response.Status = true;
                            response.Message = "Login Successful";
                            response.AccountType = tokenData.UserType;
                            response.Authtoken = tokenString;
                            response.AgentData = new AgentData
                            {

                                AgentID = agent.AgentId,
                                FirstName = agent.FirstName,
                                LastName = agent.LastName,
                                OfficeLocation = agent.OfficeLocation,
                                Phone = agent.Phone
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
                Console.WriteLine("LoginAgent --- " + ex.Message);
                response.StatusCode = 500;
                response.Status = false;
                response.Message = "Login Failed";
            }

            return response;
        }
        #endregion

        #region Get All Trips
        /// <summary>
        /// Method to return all trips from the database
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllTrips", Name = "GetAllTrips")]
        [AllowAnonymous]
        public async Task<GetTripsResponseModel> GetTrips()
        {
            GetTripsResponseModel response = new GetTripsResponseModel();

            //set up a list to hold the list of trips we will get back from the database
            List<TripData> tripList = new List<TripData>();
            try
            {
                tripList = repository.GetAllTrips();

                //check the list isn't empty
                if (tripList.Count != 0)
                {
                    response.Status = true;
                    response.StatusCode = 200;
                    response.tripList = tripList;
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
        #endregion

        #region Get All Trips For Agent
        /// <summary>
        /// Method that returns all trips for a particular agent
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        [HttpGet("GetAllTripsForAgent", Name = "GetAllTripsForAgent")]
        [AllowAnonymous]
        public async Task<GetTripsResponseModel> GetAllTripsForAgent(int agentID)
        {
            GetTripsResponseModel response = new GetTripsResponseModel();

            //set up a list to hold the list of trips we will get back from the database
            List<TripData> tripList = new List<TripData>();
            try
            {
                tripList = repository.GetAllTripsForAgent(agentID);

                //check the list isn't empty
                if (tripList.Count != 0)
                {
                    response.Status = true;
                    response.StatusCode = 200;
                    response.tripList = tripList;
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
        #endregion

        #region Generate JWT Token
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
        #endregion
    }
}
