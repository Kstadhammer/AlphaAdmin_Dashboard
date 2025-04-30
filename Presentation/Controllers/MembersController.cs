using System;
using System.IO;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

/// <summary>
/// Controller responsible for handling actions related to team members (users),
/// including adding, editing, deleting, and assigning roles.
/// Requires authorization for most actions.
/// </summary>
[Route("[controller]")]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly UserManager<MemberEntity> _userManager;
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    /// <summary>
    /// Initializes a new instance of the <see cref="MembersController"/> class.
    /// </summary>
    /// <param name="memberService">Service for member data operations.</param>
    /// <param name="userManager">ASP.NET Core Identity UserManager.</param>
    /// <param name="authService">Service for authentication operations (used for adding members).</param>
    /// <param name="webHostEnvironment">Provides information about the web hosting environment.</param>
    public MembersController(
        IMemberService memberService,
        UserManager<MemberEntity> userManager,
        IAuthService authService,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _memberService = memberService;
        _userManager = userManager;
        _authService = authService;
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Helper method to set the current user information in ViewBag.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SetCurrentUserAsync()
    {
        var userId = _userManager.GetUserId(User);
        if (userId != null)
        {
            var currentUser = await _memberService.GetCurrentUserAsync(userId);
            if (currentUser != null)
            {
                ViewBag.CurrentUser = currentUser;
            }
        }
    }

    /// <summary>
    /// API endpoint to retrieve member details for editing (GET: /Members/GetMember/{id}).
    /// </summary>
    /// <param name="id">The unique identifier of the member.</param>
    /// <returns>JSON response containing member details or an error message.</returns>
    [HttpGet]
    [Route("GetMember/{id}")]
    public async Task<IActionResult> GetMember(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return Json(new { success = false, message = "Invalid member ID" });
        }

        var member = await _memberService.GetMemberForEditAsync(id);
        if (member == null)
        {
            return Json(new { success = false, message = "Member not found" });
        }

        return Json(new { success = true, member });
    }

    /// <summary>
    /// Handles the HTTP POST request to edit an existing member's details (POST: /Members/EditMember).
    /// Includes handling for image uploads.
    /// </summary>
    /// <param name="form">The form containing updated member information.</param>
    /// <returns>Redirects to the members list page with a status message.</returns>
    [HttpPost]
    [Route("EditMember")]
    public async Task<IActionResult> EditMember(EditMemberForm form)
    {
        await SetCurrentUserAsync();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Failed to update member. Please check the form and try again.";
            return RedirectToAction("Members", "Admin");
        }

        try
        {
            // Handle image upload if provided
            if (form.MemberImage != null && form.MemberImage.Length > 0)
            {
                // Ensure the target directory exists
                var uploadsFolderPath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "members"
                );
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                // Generate unique filename and save
                var uniqueFileName =
                    Guid.NewGuid().ToString() + "_" + Path.GetFileName(form.MemberImage.FileName);
                var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.MemberImage.CopyToAsync(stream);
                }

                // Update the form with the image URL
                form.ImageUrl = $"/images/members/{uniqueFileName}";
            }
            // If no image is uploaded but an avatar is selected
            else if (!string.IsNullOrEmpty(form.ImageUrl))
            {
                // ImageUrl is already set correctly from the form, no action needed
            }

            var success = await _memberService.EditMemberAsync(form);
            if (!success)
            {
                TempData["Error"] = "Failed to update member. Please try again.";
                return RedirectToAction("Members", "Admin");
            }

            TempData["Success"] = "Member updated successfully!";
            return RedirectToAction("Members", "Admin");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred: {ex.Message}";
            return RedirectToAction("Members", "Admin");
        }
    }

    /// <summary>
    /// Handles the HTTP POST request to add a new member (POST: /Members/AddMember).
    /// Creates the user account and updates additional profile details, including image upload.
    /// </summary>
    /// <param name="form">The form containing the new member's information.</param>
    /// <returns>Redirects to the members list page with a status message.</returns>
    [HttpPost]
    [Route("AddMember")]
    public async Task<IActionResult> AddMember(AddMemberForm form)
    {
        await SetCurrentUserAsync();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Failed to add member. Please check the form and try again.";
            return RedirectToAction("Members", "Admin");
        }

        try
        {
            // Perform registration operation using AuthService
            var result = await _authService.RegisterUserAsync(form.Email, form.Password);

            if (result.Succeeded)
            {
                // Get the newly created user
                var newUser = await _userManager.FindByEmailAsync(form.Email);

                if (newUser != null)
                {
                    // Update the additional fields
                    newUser.JobTitle = form.JobTitle;
                    newUser.PhoneNumber = form.Phone;
                    newUser.IsActive = form.IsActive;
                    newUser.FirstName = form.FirstName;
                    newUser.LastName = form.LastName;

                    // Handle image upload if provided
                    if (form.MemberImage != null && form.MemberImage.Length > 0)
                    {
                        try
                        {
                            // Ensure the target directory exists
                            var uploadsFolderPath = Path.Combine(
                                _webHostEnvironment.WebRootPath,
                                "images",
                                "members"
                            );
                            if (!Directory.Exists(uploadsFolderPath))
                            {
                                Directory.CreateDirectory(uploadsFolderPath);
                            }

                            // Generate unique filename and save
                            var uniqueFileName =
                                Guid.NewGuid().ToString()
                                + "_"
                                + Path.GetFileName(form.MemberImage.FileName);
                            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await form.MemberImage.CopyToAsync(stream);
                            }

                            // Store the relative path for web access
                            newUser.ImageUrl = $"/images/members/{uniqueFileName}";
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with user creation
                            Console.WriteLine($"Error saving member image: {ex.Message}");
                        }
                    }
                    // If an avatar was selected but no image uploaded
                    else if (!string.IsNullOrEmpty(form.ImageUrl))
                    {
                        newUser.ImageUrl = form.ImageUrl;
                    }
                    else
                    {
                        // Set default avatar if no image was uploaded or selected
                        newUser.ImageUrl = "/images/Avatar_male_1.svg";
                    }

                    // Save the changes
                    var updateResult = await _userManager.UpdateAsync(newUser);
                    if (updateResult.Succeeded)
                    {
                        TempData["Success"] = "Member added successfully!";
                    }
                    else
                    {
                        TempData["Warning"] = "Member created but some details could not be saved.";
                    }
                }
                else
                {
                    TempData["Success"] =
                        "Member added successfully, but additional details could not be saved.";
                }
            }
            else
            {
                TempData["Error"] = "Failed to add member. Please try again.";
            }
        }
        catch (Exception ex)
        {
            // Enhanced error logging
            var errorMessage = ex.Message;

            // Get inner exception details if available
            var innerException = ex.InnerException;
            if (innerException != null)
            {
                errorMessage += " Inner exception: " + innerException.Message;

                // Handle common database errors with specific messages
                if (
                    innerException.Message.Contains("duplicate")
                    || innerException.Message.Contains("unique constraint")
                )
                {
                    errorMessage = "A member with this email already exists.";
                }
                else if (
                    innerException.Message.Contains("null value")
                    || innerException.Message.Contains("not nullable")
                )
                {
                    errorMessage = "Required field is missing: " + innerException.Message;
                }
            }

            // Save full exception details for debugging
            Console.WriteLine($"ERROR in AddMember: {ex.ToString()}");

            TempData["Error"] = $"An error occurred: {errorMessage}";
            return RedirectToAction("Members", "Admin");
        }

        return RedirectToAction("Members", "Admin");
    }

    /// <summary>
    /// Handles the HTTP POST request to delete a member (POST: /Members/DeleteMember).
    /// </summary>
    /// <param name="id">The unique identifier of the member to delete.</param>
    /// <returns>Redirects to the members list page with a status message.</returns>
    [HttpPost]
    [Route("DeleteMember")]
    public async Task<IActionResult> DeleteMember(string id)
    {
        try
        {
            await SetCurrentUserAsync();

            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid member ID.";
                return RedirectToAction("Members", "Admin");
            }

            var success = await _memberService.DeleteMemberAsync(id);
            if (success)
            {
                TempData["Success"] = "Member deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete member. Please try again.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("Members", "Admin");
    }

    /// <summary>
    /// Handles the HTTP POST request to assign the 'Admin' role to a member (POST: /Members/AssignAdminRole).
    /// Requires the user performing the action to have the 'Admin' role.
    /// </summary>
    /// <param name="id">The unique identifier of the member to assign the role to.</param>
    /// <returns>Redirects to the members list page with a status message.</returns>
    [HttpPost("AssignAdminRole")] // Expect ID from form body, not route
    [Authorize(Roles = "Admin")] // Ensure only admins can perform this
    public async Task<IActionResult> AssignAdminRole(string id) // Model binder will match 'id' from hidden input
    {
        if (string.IsNullOrEmpty(id))
        {
            TempData["Error"] = "Invalid member ID.";
            return RedirectToAction("Members", "Admin");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Member not found.";
            return RedirectToAction("Members", "Admin");
        }

        // Check if user is already an Admin
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        if (isAdmin)
        {
            TempData["Warning"] = $"{user.FirstName} {user.LastName} is already an Admin.";
            return RedirectToAction("Members", "Admin");
        }

        // Attempt to add to Admin role
        var result = await _userManager.AddToRoleAsync(user, "Admin");
        if (result.Succeeded)
        {
            TempData["Success"] =
                $"Successfully assigned Admin role to {user.FirstName} {user.LastName}.";
        }
        else
        {
            TempData["Error"] =
                $"Failed to assign Admin role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        return RedirectToAction("Members", "Admin");
    }
}
