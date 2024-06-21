using System.ComponentModel.DataAnnotations;

namespace TodoApi.AuthenticationService.Models
{
    public class LoginRequestModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [MaxLength(254, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(100, ErrorMessage = "Invalid password")]
        public string Password { get; set; }
    }
}