using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class MemberService : IMemberService
{
    private readonly UserManager<MemberEntity> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public MemberService(
        UserManager<MemberEntity> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Member> GetCurrentUserAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return null;

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        // Use null-coalescing operator (??) to provide defaults for potentially null strings
        return new Member
        {
            Id = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            JobTitle = user.JobTitle ?? string.Empty,
            Phone = user.PhoneNumber ?? string.Empty,
            ImageUrl = user.ImageUrl,
            IsAdmin = isAdmin,
        };
    }

    public async Task<List<Member>> GetAllMembers()
    {
        var users = await _userManager.Users.ToListAsync();
        var members = new List<Member>();

        foreach (var user in users)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            members.Add(
                new Member
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    JobTitle = user.JobTitle ?? string.Empty,
                    Phone = user.PhoneNumber ?? string.Empty,
                    ImageUrl = user.ImageUrl,
                    IsAdmin = isAdmin,
                }
            );
        }

        return members;
    }

    public async Task<List<Member>> GetAdminMembers()
    {
        var allMembers = await GetAllMembers();
        return allMembers.Where(m => m.IsAdmin).ToList();
    }

    public async Task<EditMemberForm?> GetMemberForEditAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return null;

        return new EditMemberForm
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            JobTitle = user.JobTitle,
            Phone = user.PhoneNumber,
            IsActive = user.IsActive,
            ImageUrl = user.ImageUrl,
        };
    }

    public async Task<bool> EditMemberAsync(EditMemberForm form)
    {
        if (form == null || string.IsNullOrEmpty(form.Id))
            return false;

        var user = await _userManager.FindByIdAsync(form.Id);
        if (user == null)
            return false;

        // Update user properties
        user.FirstName = form.FirstName;
        user.LastName = form.LastName;
        user.Email = form.Email;
        user.NormalizedEmail = form.Email.ToUpper();
        user.UserName = form.Email;
        user.NormalizedUserName = form.Email.ToUpper();
        user.JobTitle = form.JobTitle;
        user.PhoneNumber = form.Phone;
        user.IsActive = form.IsActive;

        // Update image URL if provided in the form
        if (!string.IsNullOrEmpty(form.ImageUrl))
        {
            user.ImageUrl = form.ImageUrl;
        }

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteMemberAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
