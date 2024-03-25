using System.ComponentModel.DataAnnotations;

namespace TodoApi.AuthenticationService.Models;

public class User
{
    public string Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; }
}