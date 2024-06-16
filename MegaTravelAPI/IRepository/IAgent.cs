using MegaTravelAPI.Models;
using MegaTravelAPI.Data;

namespace MegaTravelAPI.IRepository
{
    public interface IAgent
    {
        /// <summary>
        /// Logs in an agent
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<LoginResponse> LoginAgent(LoginModel tokenData);

        /// <summary>
        /// Finds the user data for an agent based on username
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<MegaTravelAPI.Data.Agent> FindByName(string username);

        /// <summary>
        /// Gets all trips in the database
        /// </summary>
        /// <returns></returns>
        List<TripData> GetAllTrips();

        /// <summary>
        /// Finds the agent based on agentID
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        Task<Agent> FindByID(int agentID);

        /// <summary>
        /// Method that returns all trips for a particular agent
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        List<TripData> GetAllTripsForAgent(int agentID);
    }
}
