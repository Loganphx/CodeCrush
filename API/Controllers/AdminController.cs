using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
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

public class PhotoForApprovalDto
{
    public int PhotoId { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public bool IsApproved { get; set; }
}
public class AdminController : BaseApiController
{
    private readonly DataContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;

    public AdminController(
        DataContext context,
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager, 
        RoleManager<AppRole> roleManager,
        IMapper mapper)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
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
    [HttpGet("unapproved-photos")]
    public async Task<ActionResult<IEnumerable<PhotoForApprovalDto>>> GetPhotosForApproval()
    {
        var unapprovedPhotos = await GetUnapprovedPhotos();
        
        return Ok(unapprovedPhotos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if(photo == null) return NotFound("Invalid photo");
        
        photo.IsApproved = true;

        var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photo);

        if (!user.Photos.Any(t => t.IsMain))
        {
            photo.IsMain = true;
        }
        
        if (_unitOfWork.HasChanges())
        {
            if (await _unitOfWork.Complete()) return Ok();
        }

        return BadRequest("Failed to approve photo");
    }
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
        if(photo == null) return NotFound("Invalid photo");

        return Ok();
    }

    private async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        var unapprovedPhotos = photos.Select(p =>
        {
            var t = _mapper.Map<PhotoForApprovalDto>(p);
            t.Username = _unitOfWork.UserRepository.GetUserByPhotoId(p).Result.KnownAs;
            return t;
        });

        return unapprovedPhotos;
    }
}