using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    #region Fields & Constructor

    private readonly IAuthService _authService = authService;

    #endregion

    #region Login

    public IActionResult Login(string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
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

    public IActionResult SignUp()
    {
        ViewBag.ErrorMessage = "";
        return View();
    }

    [HttpPost]
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

    #region Session Management

    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return LocalRedirect("~/");
    }

    #endregion

    #region External Authentication


    [HttpPost]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, provider);
    }

    #endregion
}
