using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context      = context;
        _tokenService = tokenService;
    }

    //public async Task<ActionResult<AppUser>> Register([FromBody] string username, [FromBody] string password)
    [AllowAnonymous]
    [HttpPost("register")] // POST: api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || await UserExists(request.Username)) return BadRequest("Invalid Username");
        if (string.IsNullOrEmpty(request.Password)) return BadRequest("Incorrect Password");
        using var hmac = new HMACSHA512();

        var user = new AppUser()
        {
            Username     = request.Username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.Username,
            Token    = _tokenService.CreateToken(user),
        };
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginRequest request)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Username.ToLower() == request.Username.ToLower());

        if (user == null) return Unauthorized("Invalid Username");

        using var hmac         = new HMACSHA512(user.PasswordSalt);
        var       computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (user.PasswordHash[i] != computedHash[i]) return Unauthorized("Invalid password");
        }


        var token = _tokenService.CreateToken(user);
        return new UserDto
        {
            Username = user.Username,
            Token    = token,
        };
    }  
    
    
    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
    }
}