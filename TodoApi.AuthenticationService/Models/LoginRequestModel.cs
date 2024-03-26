using System.ComponentModel.DataAnnotations;

namespace TodoApi.AuthenticationService.Models
{
    public class LoginRequestModel
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }
    }
}
