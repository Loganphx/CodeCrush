using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterRequest
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string  Password { get; set; }
}