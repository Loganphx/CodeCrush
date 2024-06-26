﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserRepository : IUserRepository
{
    private readonly UserManager<AppUser> _context;
    private readonly IMapper _mapper;

    public UserRepository(
        UserManager<AppUser> context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, string password)
    {
        return await _context.CreateAsync(user, password);
    }

    public async Task<IdentityResult> AddToRoleAsync(AppUser user, string roleName)
    {
        return await _context.AddToRoleAsync(user, roleName);
    }

    public async Task<bool> CheckPasswordAsync(AppUser user, string password)
    {
        return await _context.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user)
    {
        return await _context.UpdateAsync(user);
    }

    public async Task<PagedList<MemberDto>> GetUsersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(userParams.CurrentUsername))
            query = query.Where(user => user.UserName != userParams.CurrentUsername);
        if (!string.IsNullOrEmpty(userParams.Gender)) query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "age" => query.OrderByDescending(u => u.DateOfBirth),
            "name" => query.OrderBy(u => u.KnownAs),
            "created" => query.OrderByDescending(u => u.Created),
            "lastActive" => query.OrderByDescending(u => u.LastActive),
            _ => query.OrderByDescending(u => u.LastActive)
        };
        return await PagedList<MemberDto>.CreateAsync(query.Include(p => p.Photos.Where(o => o.IsApproved))
            .Select(user => _mapper.Map<MemberDto>(user))
            .AsNoTracking(), userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser?> GetUserById(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<AppUser> GetUserByPhotoId(Photo photo)
    {
        return await GetUserById(photo.AppUserId);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username, bool filterPhotos = true)
    {
        var query = _context.Users;
        if (filterPhotos)
        {
            query = query.Include(p => p.Photos.Where(o => o.IsApproved));
        }
        else
        {
            query = query.Include(p => p.Photos);

        }
        return await query.FirstOrDefaultAsync(user => user.UserName.ToLower() == username.ToLower());
    }

    public async Task<string> GetUserGender(string username)
    {
        return await _context.Users
            .Where(x => x.UserName == username)
            .Select(x => x.Gender)
            .FirstOrDefaultAsync();
    }
}