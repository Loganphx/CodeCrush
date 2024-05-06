using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class AccountController : BaseApiController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    //public async Task<ActionResult<AppUser>> Register([FromBody] string username, [FromBody] string password)
    
    [AllowAnonymous]
    [HttpPost("register")] // POST: api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
    {
        return await _accountService.Register(request);
    }

    [AllowAnonymous]
    [HttpPost("login")] // POST: api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginRequest request)
    {
        return await _accountService.Login(request);
    }
}