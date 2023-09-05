using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services;

public interface IUserRepository
{
    void                       Add(AppUser    user);
    void                       Update(AppUser user);
    Task<bool>                 SaveAllAsync();
    Task<PagedList<MemberDto>> GetUsersAsync(UserParams      userParams);
    Task<AppUser?>             GetUserById(int               id);
    Task<AppUser?>             GetUserByUsernameAsync(string username);  

}