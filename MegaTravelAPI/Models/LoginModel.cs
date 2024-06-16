using System.ComponentModel.DataAnnotations;

namespace MegaTravelAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserType { get; set; }
    }
}
