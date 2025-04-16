using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

using Data.Entities; // Add this using
using Microsoft.AspNetCore.Identity; // Add this using

[Route("Auth")] // Add base route for the controller
public class AuthController(
    IAuthService authService,
    UserManager<MemberEntity> userManager // Add UserManager
) : Controller
{
    #region Fields & Constructor

    private readonly IAuthService _authService = authService;
    private readonly UserManager<MemberEntity> _userManager = userManager; // Add field
    #endregion

    #region Login

    [HttpGet("Login")] // Explicit route: /Auth/Login
    public IActionResult Login(string returnUrl = "~/") // Default returnUrl might need adjustment now
    {
        // Check if user is already authenticated
        if (User.Identity?.IsAuthenticated ?? false)
        {
            // User is logged in, redirect based on role
            if (User.IsInRole("Admin"))
            {
                // Redirect Admins to the main dashboard
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // Redirect standard Users to the Projects page (or another default)
                return RedirectToAction("Projects", "Admin");
            }
        }

        // User is not authenticated, show the login page
        ViewBag.ErrorMessage = "";
        // If returnUrl is the root (~/), maybe default it to dashboard after login?
        ViewBag.ReturnUrl = (returnUrl == "~/") ? Url.Action("Index", "Admin") : returnUrl;
        return View();
    }

    [HttpPost("Login")] // Explicit route: /Auth/Login (POST)
    public async Task<IActionResult> Login(MemberLoginForm form, string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";

        if (ModelState.IsValid)
        {
            var result = await _authService.LoginAsync(form);
            if (result)
            {
                return LocalRedirect(returnUrl);
            }
        }

        ViewBag.ErrorMessage = "Invalid email address or password";
        return View(form);
    }

    #endregion

    #region Registration

    [HttpGet("SignUp")] // Explicit route: /Auth/SignUp
    public IActionResult SignUp()
    {
        ViewBag.ErrorMessage = "";
        return View();
    }

    [HttpPost("SignUp")] // Explicit route: /Auth/SignUp (POST)
    public async Task<IActionResult> SignUp(MemberSignUpForm form)
    {
        ViewBag.ErrorMessage = "";

        try
        {
            Console.WriteLine(
                $"SignUp: Processing form with Email={form.Email}, FirstName={form.FirstName}, LastName={form.LastName}"
            );

            if (ModelState.IsValid)
            {
                var result = await _authService.SignUpAsync(form);
                if (result)
                {
                    Console.WriteLine("SignUp: User created successfully, redirecting to Login");
                    return RedirectToAction("Login");
                }
                else
                {
                    // Get the actual error from the service
                    Console.WriteLine("SignUp: User creation failed");
                    ViewBag.ErrorMessage =
                        "Failed to create account. Please try with a different email or a stronger password (8+ characters with numbers and special characters).";
                }
            }
            else
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                Console.WriteLine($"SignUp: Validation errors: {string.Join(", ", errors)}");
                ViewBag.ErrorMessage =
                    ModelState
                        .Values.FirstOrDefault(v => v.Errors.Count > 0)
                        ?.Errors.FirstOrDefault()
                        ?.ErrorMessage ?? "Please fix the validation errors.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignUp: Exception: {ex.Message}");
            ViewBag.ErrorMessage = "An error occurred during registration. Please try again.";
        }

        return View(form);
    }

    #endregion

    [Route("/admin")] // Route for /admin
    public IActionResult AdminRedirect()
    {
        // Simply redirect to the AdminLogin page
        return RedirectToAction("AdminLogin");
    }

    #region Admin Login

    [HttpGet("AdminLogin")] // Explicit route: /Auth/AdminLogin
    public IActionResult AdminLogin(string returnUrl = "/") // Default return to dashboard
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl; // Keep returnUrl in case it's needed later
        return View();
    }

    [HttpPost("AdminLogin")] // Explicit route: /Auth/AdminLogin (POST)
    public async Task<IActionResult> AdminLogin(MemberLoginForm form, string returnUrl = "/")
    {
        ViewBag.ErrorMessage = "";

        if (ModelState.IsValid)
        {
            var loginSucceeded = await _authService.LoginAsync(form);
            if (loginSucceeded)
            {
                // Login succeeded, now check if the user is an Admin
                var user = await _userManager.FindByEmailAsync(form.Email);
                if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // User is an Admin, proceed to the dashboard (or returnUrl)
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    // User logged in successfully but is NOT an Admin.
                    // Log them out immediately and show an error.
                    await _authService.LogoutAsync();
                    ViewBag.ErrorMessage = "Access denied. This login is for administrators only.";
                    // Return the specific AdminLogin view
                    return View("AdminLogin", form);
                }
            }
            else
            {
                // Login failed (invalid credentials)
                ViewBag.ErrorMessage = "Invalid email address or password.";
            }
        }
        else
        {
            // Model validation failed
            ViewBag.ErrorMessage = "Please correct the errors below.";
        }

        // If we reach here, something went wrong (validation fail, login fail, or non-admin user)
        // Return the specific AdminLogin view
        return View("AdminLogin", form);
    }

    #endregion


    #region Session Management

    [HttpGet("Logout")] // Explicit route: /Auth/Logout
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return LocalRedirect("~/");
    }

    [HttpGet("AccessDenied")] // Explicit route: /Auth/AccessDenied
    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion

    #region External Authentication


    [HttpPost("ExternalSignIn")] // Explicit route: /Auth/ExternalSignIn
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, provider);
    }

    #endregion

    #region Password Reset

    [HttpGet("ForgotPassword")]
    public IActionResult ForgotPassword()
    {
        ViewBag.ErrorMessage = "";
        ViewBag.SuccessMessage = "";
        return View();
    }

    [HttpPost("ForgotPassword")]
    public IActionResult ForgotPassword(ForgotPasswordForm form)
    {
        ViewBag.ErrorMessage = "";
        ViewBag.SuccessMessage = "";

        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Please enter a valid email address.";
            return View(form);
        }

        // This is a dummy implementation that just shows a success message
        // In a real implementation, you would:
        // 1. Check if the email exists in the database
        // 2. Generate a password reset token
        // 3. Store the token with an expiration time
        // 4. Send an email with a link to reset the password

        // Simulate success
        ViewBag.SuccessMessage =
            "If an account exists with this email, you will receive password reset instructions.";
        return View();
    }

    #endregion
}
