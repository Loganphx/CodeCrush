using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterRequest
{
    [Required]
    [StringLength(32, MinimumLength = 5)]
    public string Username { get; set; }
    
    [Required]
    [StringLength(32, MinimumLength = 8)]
    public string  Password { get; set; }
}