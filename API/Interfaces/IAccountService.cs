using API.Controllers;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces;

public interface IAccountService
{
    public Task<UserDto> Register(RegisterRequest request);
    public Task<ActionResult<UserDto>> Login(LoginRequest request);
}