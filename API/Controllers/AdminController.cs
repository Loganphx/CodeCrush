using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class RoleDto
{
    public string Username { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
public class AdminController : BaseApiController
{
    private readonly DataContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AdminController(DataContext _context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        this._context = _context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult<PagedList<RoleDto>>> GetUserWithRoles([FromQuery] RoleParams userParams)
    {
        var query = _context.Users.AsQueryable();

        // if (!string.IsNullOrEmpty(userParams.CurrentUsername)) query = query.Where(user => user.UserName != userParams.CurrentUsername);
        // if (!string.IsNullOrEmpty(userParams.Gender)) query = query.Where(u => u.Gender == userParams.Gender);

        // var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        // var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        // query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "age" => query.OrderByDescending(u => u.DateOfBirth),
            "name" => query.OrderBy(u => u.KnownAs),
            "created" => query.OrderByDescending(u => u.Created),
            "lastActive" => query.OrderByDescending(u => u.LastActive),
            _ => query.OrderBy(u => u.KnownAs)
        };
     
        var users = await PagedList<RoleDto>.CreateAsync(query
            .Select(u => new RoleDto()
            {
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name)
            })
            .AsNoTracking(), userParams.PageNumber, userParams.PageSize);

        var paginationHeader = new PaginationHeader(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages);
        Response.AddPaginationHeader(paginationHeader);

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("available-roles")]
    public async Task<ActionResult> GetAvailableRoles()
    {
        var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return Ok(roles);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {

        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role.");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return NotFound("Invalid username");

        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }
    
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}