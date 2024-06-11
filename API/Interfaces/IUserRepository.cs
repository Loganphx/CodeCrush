using API.DTOs;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public interface IUserRepository
{
    Task<IdentityResult>                       CreateAsync(AppUser    user, string password);
    Task<IdentityResult>                       AddToRoleAsync(AppUser    user, string roleName);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
    Task<PagedList<MemberDto>> GetUsersAsync(UserParams      userParams);
    Task<AppUser?>             GetUserById(int               id);
    Task<AppUser?>             GetUserByUsernameAsync(string username);
    Task<string> GetUserGender(string username);

}