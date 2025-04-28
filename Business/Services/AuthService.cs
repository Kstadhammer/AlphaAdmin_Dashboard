using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

/// <summary>
/// Service responsible for handling user authentication operations like sign-in, sign-up, and sign-out.
/// It orchestrates interactions between user service, sign-in manager, and user manager.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly SignInManager<MemberEntity> _signInManager;
    private readonly UserManager<MemberEntity> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userService">Service for user creation and management.</param>
    /// <param name="signInManager">ASP.NET Core Identity SignInManager.</param>
    /// <param name="userManager">ASP.NET Core Identity UserManager.</param>
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

    /// <summary>
    /// Attempts to sign in a user using their email and password.
    /// </summary>
    /// <param name="formData">The sign-in form data containing email, password, and persistence preference.</param>
    /// <returns>A <see cref="SignInResult"/> indicating the outcome of the sign-in attempt.</returns>
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

    /// <summary>
    /// Registers a new user in the system based on the provided form data.
    /// Uses the <see cref="IUserService"/> to handle the actual user creation.
    /// </summary>
    /// <param name="formData">The sign-up form data containing user details.</param>
    /// <returns>A <see cref="ServiceResult{T}"/> indicating success or failure.</returns>
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

    /// <summary>
    /// Signs out the currently authenticated user.
    /// </summary>
    /// <returns>A <see cref="ServiceResult{T}"/> indicating the sign-out operation was successful.</returns>
    public async Task<ServiceResult<object>> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new ServiceResult<object> { Succeeded = true, StatusCode = 200 };
    }

    /// <summary>
    /// Attempts to log in a user using credentials from the login form model.
    /// </summary>
    /// <param name="form">The login form data.</param>
    /// <returns>True if login is successful, false otherwise.</returns>
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

    /// <summary>
    /// Registers a new user based on the sign-up form model.
    /// This method adapts the form data and calls the internal SignUpAsync.
    /// </summary>
    /// <param name="form">The sign-up form data.</param>
    /// <returns>True if registration is successful, false otherwise.</returns>
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

    /// <summary>
    /// Signs out the currently authenticated user.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    /// <summary>
    /// Registers a new user entity with basic details and assigns the default "User" role.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's chosen password.</param>
    /// <returns>An <see cref="IdentityResult"/> indicating the outcome of the registration process.</returns>
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
