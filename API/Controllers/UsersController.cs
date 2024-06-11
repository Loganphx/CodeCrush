using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService  _userService;
    public UsersController(
        IUserService userService)
    {
        _userService       = userService;
    }
    
    [HttpGet]
    // GET: api/users/ 
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        var users = await _userService.GetMembersAsync(User.GetUsername(), userParams);
        
        var paginationHeader = new PaginationHeader(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages);
        Response.AddPaginationHeader(paginationHeader);

        return Ok(users);
    }
    
    [HttpGet("{username}")] //api/users/2
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var isPersonal = username.ToLower() == User.GetUsername().ToLower();
        return Ok(await _userService.GetMemberAsync(username, !isPersonal));
    }
    
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        await _userService.UpdateUser(User.GetUsername(), memberUpdateDto);
        return NoContent();
    }
    
    [HttpPost("add-photo")]
    [Authorize]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var photoDto = await _userService.AddPhoto(User.GetUsername(), file);

        return CreatedAtAction(nameof(GetUser), new {username = User.GetUsername()}, photoDto);
    }
    
    [HttpPut("set-main-photo/{photoId}")]
    [Authorize]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        await _userService.SetMainPhoto(User.GetUsername(), photoId);
        return NoContent();
    }
    
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        await _userService.DeletePhoto(User.GetUsername(), photoId);
        return NoContent();
    }
}