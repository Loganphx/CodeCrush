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
        
        var query = _context.Users.AsQueryable();
            
        if(!string.IsNullOrEmpty(userParams.CurrentUsername)) query = query.Where(user => user.Username != userParams.CurrentUsername);
        if(!string.IsNullOrEmpty(userParams.Gender)) query = query.Where(u => u.Gender == userParams.Gender);
        
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
        
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        
        query = userParams.OrderBy switch
        {
            "age"          => query.OrderByDescending(u => u.DateOfBirth),
            "name" => query.OrderBy(u => u.KnownAs),
            "created"      => query.OrderByDescending(u => u.Created),
            "lastActive"   => query.OrderByDescending(u => u.LastActive),
            _              => query.OrderByDescending(u => u.LastActive)
        };
        return await PagedList<MemberDto>.CreateAsync(query.Include(p => p.Photos)
            .Select(user => new MemberDto(user))
            .AsNoTracking(), userParams.PageNumber, userParams.PageSize);
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