using API.DTOs;
using API.Entities;
using API.Helpers;
using CloudinaryDotNet.Actions;

namespace API.Interfaces;

public interface IUserService
{
    public Task<PagedList<MemberDto>> GetMembersAsync(string username, UserParams userParams);  
    public Task<MemberDto>            GetMemberAsync(string username);
    public Task                       UpdateUser(string      username, MemberUpdateDto memberUpdateDto);
    public Task<PhotoDto>             AddPhoto(string        username, IFormFile       file);
    public Task                       SetMainPhoto(string    username, int             photoId);
    public Task                       DeletePhoto(string     username, int             photoId);

}

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult>    DeletePhotoAsync(string publicId);
}
