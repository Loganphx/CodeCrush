using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterRequest
{
    [Required]
    [StringLength(32, MinimumLength = 4)]
    public string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string KnownAs { get; set; }

    [Required]
    public string Gender { get; set; }
    
    [Required]
    public DateOnly DateOfBirth { get; set; }
    
    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }

    [Required]
    [StringLength(64, MinimumLength = 8)]
    public string  Password { get; set; }
}