using MegaTravelAPI.Data;

namespace MegaTravelAPI.Models
{
    public class SaveUserResponse
    {
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public User Data { get; set; }
        public Error Errors { get; set; }
        public int UserId { get; set; }

    }

    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
