using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services;

public interface ILikesRepository
{
    public Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
    
    public Task<AppUser> GetUserWithLikes(int userId);

    public Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);

}