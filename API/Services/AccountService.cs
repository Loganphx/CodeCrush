using System.Security.Cryptography;
using System.Text;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Errors;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AccountService : IAccountService
{
    private readonly ITokenService   _tokenService;
    private readonly IUserRepository _userRepository;

    public AccountService(ITokenService tokenService, IUserRepository userRepository)
    {
        _tokenService        = tokenService;
        _userRepository = userRepository;
    }
    
    public async Task<UserDto> Register(RegisterRequest request)
    {
        if (await UserExists(request.Username)) throw new BadRequestException("Username is already taken");
        
        using var hmac = new HMACSHA512();

        var user = new AppUser()
        {
            Username     = request.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key,
            Email = request.Email,
            KnownAs = request.KnownAs,
            Gender = request.Gender,
            City = request.City,
            Country = request.Country,
            DateOfBirth = request.DateOfBirth,
        };

        _userRepository.Add(user);
        await _userRepository.SaveAllAsync();

        return new UserDto(user.Username, _tokenService.CreateToken(user), user.Photos.FirstOrDefault(x => x.IsMain)?.Url, user.KnownAs);
    }

    public async Task<ActionResult<UserDto>> Login(LoginRequest request)
    {
        var user = await _userRepository.GetUserByUsernameAsync(request.Username);

        if (user == null) throw new BadRequestException("Invalid Username");

        using var hmac         = new HMACSHA512(user.PasswordSalt);
        var       computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (user.PasswordHash[i] != computedHash[i]) throw new BadRequestException("Invalid password");
        }


        var token = _tokenService.CreateToken(user);
        return new UserDto(user.Username, token, user.Photos.FirstOrDefault(x => x.IsMain)?.Url, user.KnownAs);

    }

    private async Task<bool> UserExists(string username)
    {
        return await _userRepository.GetUserByUsernameAsync(username) != null;
    }
}