namespace MegaTravelAPI.Models
{
    public class LoginResponse
    {
        public int UserID { get; set; }
        public string AccountType { get; set; } = null;
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = null;
        public string Authtoken { get; set; } = null;

        public UserData Data { get; set; }

        public AgentData AgentData { get; set; }
    }
}
