using API.DTOs;
using API.Entities;
using API.Errors;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountService(IUnitOfWork unitOfWork, ITokenService tokenService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<UserDto> Register(RegisterRequest registerDto)
    {
        if (await UserExists(registerDto.Username)) throw new BadRequestException("Username is already taken");

        // using var hmac = new HMACSHA512();
        var user = new AppUser()
        {
            UserName = registerDto.Username,
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

        var result = await _unitOfWork.UserRepository.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) throw new BadRequestException(result.ToString());

        result = await _unitOfWork.UserRepository.AddToRoleAsync(user, "Member");

        if (!result.Succeeded) throw new BadRequestException(result.ToString());

        if (_unitOfWork.HasChanges())
        {
            await _unitOfWork.Complete();
        }

        var token = await _tokenService.CreateToken(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Token = token;

        return userDto;
    }

    public async Task<ActionResult<UserDto>> Login(LoginRequest loginDto)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(loginDto.Username);

        if (user == null) throw new UnauthorizedException("Invalid Username");

        var successful = await _unitOfWork.UserRepository.CheckPasswordAsync(user, loginDto.Password);

        if (!successful) throw new UnauthorizedAccessException("Invalid password");

        var token = await _tokenService.CreateToken(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Token = token;

        return userDto;
    }

    private async Task<bool> UserExists(string username)
    {
        return await _unitOfWork.UserRepository.GetUserByUsernameAsync(username) != null;
    }
}