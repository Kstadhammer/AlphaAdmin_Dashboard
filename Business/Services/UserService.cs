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

    public async Task<ServiceResult<object>> GetUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        var result = await _userRepository.GetAllAsync();
        return new ServiceResult<object>
        {
            Succeeded = result.Succeeded,
            StatusCode = result.StatusCode,
            Error = result.Error,
            Result = result.Result,
        };
    }

    public async Task<ServiceResult<object>> AddUserToRole(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return new ServiceResult<object>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "Role does not exist",
            };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ServiceResult<object>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "User does not exist",
            };
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded
            ? new ServiceResult<object> { Succeeded = true, StatusCode = 200 }
            : new ServiceResult<object>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = "Failed to add user to role",
            };
    }

    public async Task<ServiceResult<object>> CreateUserAsync(
        SignUpFormData formData,
        string roleName
    )
    {
        if (formData == null)
        {
            Debug.WriteLine("Form data is null");
            return new ServiceResult<object>
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
            return new ServiceResult<object>
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

                return new ServiceResult<object>
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
                    return new ServiceResult<object>
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
                    ? new ServiceResult<object> { Succeeded = true, StatusCode = 201 }
                    : new ServiceResult<object>
                    {
                        Succeeded = false,
                        StatusCode = 201,
                        Error = "Failed to add user to role",
                    };
            }
            return new ServiceResult<object>
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
            return new ServiceResult<object>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }

    public Task<ServiceResult<object>> GetUserByIdAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
