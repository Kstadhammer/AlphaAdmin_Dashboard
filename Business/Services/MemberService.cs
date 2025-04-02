using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class MemberService : IMemberService
{
    private readonly UserManager<MemberEntity> _userManager;

    public MemberService(UserManager<MemberEntity> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Member> GetCurrentUserAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return null;

        // Use null-coalescing operator (??) to provide defaults for potentially null strings
        return new Member
        {
            Id = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            JobTitle = user.JobTitle ?? string.Empty,
            Phone = user.PhoneNumber ?? string.Empty,
        };
    }

    public async Task<List<Member>> GetAllMembers()
    {
        var users = await _userManager.Users.ToListAsync();

        return users
            .Select(user => new Member
            {
                // Use null-coalescing operator (??) for potentially null strings
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                JobTitle = user.JobTitle ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
            })
            .ToList();
    }
}
