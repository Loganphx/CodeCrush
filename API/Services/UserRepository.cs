using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserRepository : IUserRepository
{
    private readonly DataContext         _context;

    public UserRepository(
        DataContext context)
    {
        _context = context;
    }
    public void Add(AppUser user)
    {
        _context.Users.Add(user);
    }
    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<PagedList<MemberDto>> GetUsersAsync(UserParams userParams)
    {
        var query = _context.Users
            .Include(p => p.Photos)
            .Select(user => new MemberDto(user))
            .AsNoTracking();
        
        return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser?> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
    }
}