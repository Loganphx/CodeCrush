using API.Entities;

namespace API.DTOs;

public class PhotoDto
{
    public int    Id     { get; set; }
    public string Url    { get; set; }
    public bool   IsMain { get; set; }

    public PhotoDto(Photo photo)
    {
        Id     = photo.Id;
        Url    = photo.Url;
        IsMain = photo.IsMain;
    }
}