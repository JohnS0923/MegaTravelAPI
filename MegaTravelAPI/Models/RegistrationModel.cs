using System.ComponentModel.DataAnnotations;
namespace MegaTravelAPI.Models
{
    public class RegistrationModel
    {

        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string Street1 { get; set; } = null!;
        public string? Street2 { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        [Required]
        public string State { get; set; } = null!;
        [Required]
        public int ZipCode { get; set; }
        [Required]
        public string Phone { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string UserType { get; set; } = null!;
    }
}
