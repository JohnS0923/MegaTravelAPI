using MegaTravelAPI.Models;

namespace MegaTravelAPI
{
    public class TripData
    {
        public int UserID { get; set; }
        public int AgentID { get; set; }
        public int TripID { get; set; }
        public string TripName {  get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumAdults { get; set; }
        public int NumChildren { get; set; }

        public UserData UserInfo { get; set; }
    }
}
