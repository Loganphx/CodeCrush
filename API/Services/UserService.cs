using API.DTOs;
using API.Entities;
using API.Errors;
using API.Helpers;
using API.Interfaces;

namespace API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPhotoService   _photoService;

    public UserService(IUserRepository userRepository, IPhotoService photoService)
    {
        _userRepository    = userRepository;
        _photoService = photoService;
    }
    public async Task<PagedList<MemberDto>> GetMembersAsync(string username, UserParams userParams)
    {
        var currentUser = await _userRepository.GetUserByUsernameAsync(username);
        userParams.CurrentUsername = currentUser.Username;

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
        }
        
        var users       = await _userRepository.GetUsersAsync(userParams);

        return users;
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if (user == null) throw new BadRequestException("No valid user was found with that username");

        return new MemberDto(user);
    }

    public async Task UpdateUser(string username, MemberUpdateDto memberUpdateDto)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if(user == null) throw new BadRequestException($"User with {username} not found");

        user.KnownAs = memberUpdateDto.KnownAs;
        user.Introduction = memberUpdateDto.Introduction;
        user.LookingFor   = memberUpdateDto.LookingFor;
        user.Interests    = memberUpdateDto.Interests;
        user.City         = memberUpdateDto.City;
        user.Country      = memberUpdateDto.Country;

        if (await _userRepository.SaveAllAsync()) return;

        throw new BadRequestException("Failed to update user");
    }

    public async Task<PhotoDto> AddPhoto(string username, IFormFile file)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if(user == null) throw new BadRequestException($"User with {username} not found");
        
        var result = await _photoService.AddPhotoAsync(file);
        if(result.Error != null) throw new BadRequestException(result.Error.Message);
        
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        
        if(user.Photos.Count == 0) photo.IsMain = true;
        
        user.Photos.Add(photo);

        if (await _userRepository.SaveAllAsync())
        {
            return new PhotoDto(photo);
        }
        
        throw new BadRequestException("Problem adding photo");
    }

    public async Task SetMainPhoto(string username, int photoId)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if(user == null) throw new BadRequestException($"User with {username} not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if(photo == null) throw new BadRequestException($"Photo with {photoId} not found");
        
        if(photo.IsMain) throw new BadRequestException("This is already your main photo");

        var currentMainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);
        if(currentMainPhoto != null) currentMainPhoto.IsMain = false;
        photo.IsMain = true;
        
        if (await _userRepository.SaveAllAsync()) return;
        
        throw new BadRequestException("Failed to set main photo");
    }

    public async Task DeletePhoto(string username, int photoId)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username);
        if(user == null) throw new UnauthorizedException($"User with {username} not found");
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        
        if(photo == null) throw new UnauthorizedException($"Photo with {photoId} not found");
        
        if(photo.IsMain) throw new BadRequestException("You cannot delete your main photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) throw new BadRequestException(result.Error.Message);
        }
        
        user.Photos.Remove(photo);
        
        if(await _userRepository.SaveAllAsync()) return; 
        
        throw new BadRequestException("Failed to delete photo");
    }
}