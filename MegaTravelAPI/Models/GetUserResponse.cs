using MegaTravelAPI.Data;

namespace MegaTravelAPI.Models
{
    public class GetUserResponse
    {
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public List<User> Data { get; set; }
        public Error Errors { get; set; }

    }

}
