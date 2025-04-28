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

/// <summary>
/// Service for managing team members (users) using ASP.NET Core Identity.
/// Handles retrieval, update, and deletion of member information.
/// </summary>
public class MemberService : IMemberService
{
    private readonly UserManager<MemberEntity> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberService"/> class.
    /// </summary>
    /// <param name="userManager">The ASP.NET Core Identity UserManager.</param>
    /// <param name="roleManager">The ASP.NET Core Identity RoleManager.</param>
    public MemberService(
        UserManager<MemberEntity> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Retrieves the details of the currently logged-in user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A <see cref="Member"/> object representing the user, or null if not found.</returns>
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

    /// <summary>
    /// Retrieves a list of all registered members.
    /// </summary>
    /// <returns>A list of <see cref="Member"/> objects.</returns>
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

    /// <summary>
    /// Retrieves a list of all members who have the 'Admin' role.
    /// </summary>
    /// <returns>A list of <see cref="Member"/> objects with admin privileges.</returns>
    public async Task<List<Member>> GetAdminMembers()
    {
        var allMembers = await GetAllMembers();
        return allMembers.Where(m => m.IsAdmin).ToList();
    }

    /// <summary>
    /// Retrieves member details formatted for an edit form.
    /// </summary>
    /// <param name="id">The unique identifier of the member to edit.</param>
    /// <returns>An <see cref="EditMemberForm"/> object populated with member data, or null if not found.</returns>
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

    /// <summary>
    /// Updates an existing member's details based on the submitted form data.
    /// </summary>
    /// <param name="form">The form containing the updated member information.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
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

    /// <summary>
    /// Deletes a member from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the member to delete.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
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
