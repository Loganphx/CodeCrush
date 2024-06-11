using API.Entities;

namespace API.Interfaces;

public interface IPhotoRepository
{
    public Task<IEnumerable<Photo>> GetUnapprovedPhotos();
    public Task<Photo> GetPhotoById(int photoId);
    public Task<bool> RemovePhoto(Photo photo);
}