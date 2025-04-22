using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly SignInManager<MemberEntity> _signInManager;
    private readonly UserManager<MemberEntity> _userManager;

    public AuthService(
        IUserService userService,
        SignInManager<MemberEntity> signInManager,
        UserManager<MemberEntity> userManager
    )
    {
        _userService = userService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<SignInResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
        {
            return new SignInResult();
        }

        var result = await _signInManager.PasswordSignInAsync(
            formData.Email,
            formData.Password,
            formData.IsPersistent,
            false
        );

        return result;
    }

    public async Task<ServiceResult<object>> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
        {
            return new ServiceResult<object>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Not all fields are filled",
            };
        }

        var result = await _userService.CreateUserAsync(formData, "User");
        return result.Succeeded
            ? new ServiceResult<object> { Succeeded = true, StatusCode = 201 }
            : new ServiceResult<object>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error,
            };
    }

    public async Task<ServiceResult<object>> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new ServiceResult<object> { Succeeded = true, StatusCode = 200 };
    }

    // New methods to satisfy the interface
    public async Task<bool> LoginAsync(MemberLoginForm form)
    {
        if (form == null)
            return false;

        var result = await _signInManager.PasswordSignInAsync(
            form.Email,
            form.Password,
            form.RememberMe,
            false
        );

        return result.Succeeded;
    }

    public async Task<bool> SignUpAsync(MemberSignUpForm form)
    {
        if (form == null)
        {
            Console.WriteLine("SignUpAsync: Form is null");
            return false;
        }

        Console.WriteLine($"SignUpAsync: Creating user with Email={form.Email}");

        // Create a SignUpFormData from MemberSignUpForm
        var signUpData = new SignUpFormData
        {
            Email = form.Email,
            Password = form.Password,
            FirstName = form.FirstName,
            LastName = form.LastName,
        };

        // Fixed role name to match exactly what we created in Program.cs
        var result = await SignUpAsync(signUpData);
        Console.WriteLine(
            $"SignUpAsync: Result Succeeded={result.Succeeded}, StatusCode={result.StatusCode}, Error={result.Error ?? "none"}"
        );
        return result.Succeeded;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> RegisterUserAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return IdentityResult.Failed(
                new IdentityError { Description = "Email and password cannot be empty" }
            );
        }

        // Create a new user with the minimum required properties
        var userEntity = new MemberEntity
        {
            UserName = email,
            Email = email,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = email.ToUpper(),
            EmailConfirmed = true,
        };

        // Use UserManager to create the user with the provided password
        var result = await _userManager.CreateAsync(userEntity, password);

        if (result.Succeeded)
        {
            // Add the user to the "User" role
            await _userManager.AddToRoleAsync(userEntity, "User");
        }

        return result;
    }
}
