using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u => u.Username).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            users = likes.Select(likes => likes.TargetUser);
        }

        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        var likedUsers = users.Select(user => new LikeDto()
        {
            Username = user.Username,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}