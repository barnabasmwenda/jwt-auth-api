using System.ComponentModel.DataAnnotations;

namespace TodoApi.AuthenticationService.Models
{
    public class RegisterRequestModel
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }
    }

}
