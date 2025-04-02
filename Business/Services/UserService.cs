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
    UserManager<MemberEntity> userManager,
    RoleManager<IdentityRole> roleManager
) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<MemberEntity> _userManager = userManager;
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
            Debug.WriteLine("Form data is null");
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Form data cannot be null",
            };
        }

        Debug.WriteLine(
            $"Creating user with email: {formData.Email}, FirstName: {formData.FirstName}, LastName: {formData.LastName}"
        );

        var existsResult = await _userRepository.ExistsAsync(x => x.Email == formData.Email);
        if (existsResult.Result)
        {
            Debug.WriteLine($"User with email {formData.Email} already exists");
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 409,
                Error = "User with this email already exists",
            };
        }
        try
        {
            Debug.WriteLine("Mapping form data to UserEntity");
            var userEntity = formData.MapTo<MemberEntity>();
            Debug.WriteLine(
                $"UserEntity created: {userEntity.Email}, UserName: {userEntity.UserName}"
            );

            Debug.WriteLine("Calling UserManager.CreateAsync");
            var result = await _userManager.CreateAsync(userEntity, formData.Password);
            Debug.WriteLine($"UserManager.CreateAsync result: {result.Succeeded}");

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine($"Error: {error.Code} - {error.Description}");
                }

                return new UserResult
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = string.Join(", ", result.Errors.Select(e => e.Description)),
                };
            }

            if (result.Succeeded)
            {
                // Make sure role name casing is correct - roles are case sensitive
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    Debug.WriteLine($"Role {roleName} does not exist");
                    return new UserResult
                    {
                        Succeeded = false,
                        StatusCode = 500,
                        Error = $"Role {roleName} does not exist",
                    };
                }

                Debug.WriteLine($"Adding user {userEntity.Id} to role {roleName}");
                var addToRoleResult = await _userManager.AddToRoleAsync(userEntity, roleName);
                Debug.WriteLine($"AddToRoleAsync result: {addToRoleResult.Succeeded}");

                return addToRoleResult.Succeeded
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
            Debug.WriteLine($"Exception in CreateUserAsync: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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
