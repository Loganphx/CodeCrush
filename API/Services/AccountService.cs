using API.DTOs;
using API.Entities;
using API.Errors;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService   _tokenService;
    private readonly IMapper _mapper;

    public AccountService(IUserRepository userRepository, ITokenService tokenService, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _tokenService        = tokenService;
        _mapper = mapper;
    }
    
    public async Task<UserDto> Register(RegisterRequest registerDto)
    {
        if (await UserExists(registerDto.Username)) throw new BadRequestException("Username is already taken");
        
        // using var hmac = new HMACSHA512();
        var user = new AppUser()
        {
            UserName     = registerDto.Username,
            // PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            // PasswordSalt = hmac.Key,
            Email = registerDto.Email,
            KnownAs = registerDto.KnownAs,
            Gender = registerDto.Gender,
            City = registerDto.City,
            Country = registerDto.Country,
            DateOfBirth = registerDto.DateOfBirth,
        };
        
        user.UserName = registerDto.Username.ToLower();
        
        var result = await _userRepository.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) throw new BadRequestException(result.ToString());

        result = await _userRepository.AddToRoleAsync(user, "Member");
        
        if (!result.Succeeded) throw new BadRequestException(result.ToString());

        
        var token = await _tokenService.CreateToken(user);
        return new UserDto(user.UserName, token, user.Photos.FirstOrDefault(x => x.IsMain)?.Url, user.KnownAs, user.Gender);
    }

    public async Task<ActionResult<UserDto>> Login(LoginRequest loginDto)
    {
        var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
        
        if (user == null) throw new UnauthorizedException("Invalid Username");

        var successful = await _userRepository.CheckPasswordAsync(user, loginDto.Password);

        if (!successful) throw new UnauthorizedAccessException("Invalid password");
        
        var token = await _tokenService.CreateToken(user);
        return new UserDto(user.UserName, token, user.Photos.FirstOrDefault(x => x.IsMain)?.Url, user.KnownAs, user.Gender);

    }

    private async Task<bool> UserExists(string username)
    {
        return await _userRepository.GetUserByUsernameAsync(username) != null;
    }
}