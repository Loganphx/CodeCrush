using API.Entities;
using API.Extensions;

namespace API.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    
    public string Username { get; set; }

    public int Age { get; set; }

    public string KnownAs { get; set; }

    public string         PhotoUrl     { get; set; }
    public DateTime       Created      { get; set; }
    public DateTime       LastActive   { get; set; }
    public string         Gender       { get; set; }
    public string         Introduction { get; set; } 
    public string         LookingFor   { get; set; } 
    public string         Interests    { get; set; } 
    public string         City         { get; set; } 
    public string         Country      { get; set; } 
    public List<PhotoDto> Photos       { get; set; }

    public MemberDto(AppUser user)
    {
        Id           = user.Id;
        Username     = user.Username;
        Age          = user.DateOfBirth.CalculateAge();
        KnownAs      = user.KnownAs;
        Created      = user.Created;
        LastActive   = user.LastActive;
        Gender       = user.Gender;
        Introduction = user.Introduction;
        LookingFor   = user.LookingFor;
        Interests    = user.Interests;
        City         = user.City;
        Country      = user.Country;
        Photos       = user.Photos.Select(photo => new PhotoDto(photo)).ToList();
        PhotoUrl     = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url;   
    }
}