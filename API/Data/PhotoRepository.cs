using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository: IPhotoRepository
{
    private readonly DataContext _context;

    public PhotoRepository(DataContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Photo>> GetUnapprovedPhotos()
    {
        return await _context.Photos.Where(t => !t.IsApproved).ToListAsync();
    }

    public async Task<Photo> GetPhotoById(int photoId)
    {
        return await _context.Photos.Where(t => t.Id == photoId).FirstOrDefaultAsync();
    }

    public Task<bool> RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
        return Task.FromResult(true);
    }
}