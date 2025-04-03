using System;
using System.IO;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("[controller]")]
public class MembersController : Controller
{
    private readonly IMemberService _memberService;
    private readonly UserManager<MemberEntity> _userManager;
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _webHostEnvironment;

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
            // Convert AddMemberForm to MemberSignUpForm
            var signUpForm = new MemberSignUpForm
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                Password = form.Password,
                ConfirmPassword = form.ConfirmPassword,
            };

            var success = await _authService.SignUpAsync(signUpForm);
            if (success)
            {
                // Update additional fields like JobTitle, Phone, etc.
                // Find the newly created user by email
                var newUser = await _userManager.FindByEmailAsync(form.Email);
                if (newUser != null)
                {
                    // Update the additional fields
                    newUser.JobTitle = form.JobTitle;
                    newUser.PhoneNumber = form.Phone;
                    newUser.IsActive = form.IsActive;

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
                    else
                    {
                        // Set default avatar if no image was uploaded
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
            TempData["Error"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("Members", "Admin");
    }

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
}
