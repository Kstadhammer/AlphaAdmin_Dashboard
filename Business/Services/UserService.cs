using System.Diagnostics;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class UserService(
    IUserRepository userRepository,
    UserManager<UserEntity> userManager,
    RoleManager<IdentityRole> roleManager
) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<UserResult> GetUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        var result = await _userRepository.GetAllAsync();
        return result.MapTo<UserResult>();
    }

    public async Task<UserResult> AddUserToRole(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "Role does not exist",
            };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "User does not exist",
            };
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = 200 }
            : new UserResult
            {
                Succeeded = false,
                StatusCode = 500,
                Error = "Failed to add user to role",
            };
    }

    public async Task<UserResult> CreateUserAsync(SignUpFormData formData, string roleName)
    {
        if (formData == null)
        {
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Form data cannot be null",
            };
        }

        var existsResult = await _userRepository.ExistsAsync(x => x.Email == formData.Email);
        if (existsResult.Result)
        {
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 409,
                Error = "User with this email already exists",
            };
        }
        try
        {
            var userEntity = formData.MapTo<UserEntity>();
            var result = await _userManager.CreateAsync(userEntity, formData.Password);

            if (result.Succeeded)
            {
                var addToRoleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                return result.Succeeded
                    ? new UserResult { Succeeded = true, StatusCode = 201 }
                    : new UserResult
                    {
                        Succeeded = false,
                        StatusCode = 201,
                        Error = "Failed to add user to role",
                    };
            }
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 500,
                Error = "Failed to create user",
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }

    public Task<UserResult> GetUserByIdAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
