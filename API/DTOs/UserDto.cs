namespace API.DTOs;

public class UserDto
{
    public string Username { get; set; }

    public string Token    { get; set; }

    public string PhotoUrl { get; set; }
    
    public string KnownAs { get; set; }
    public string Gender { get; set; }

    public UserDto(string username, string token, string photoUrl, string knownAs, string gender)
    {
        Username = username;
        Token    = token;
        PhotoUrl = photoUrl;
        KnownAs  = knownAs;
        Gender   = gender;
    }
}