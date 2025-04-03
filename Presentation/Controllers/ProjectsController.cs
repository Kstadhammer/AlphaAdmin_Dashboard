using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("[controller]")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;
    private readonly IMemberService _memberService;
    private readonly UserManager<MemberEntity> _userManager;

    public ProjectsController(
        IProjectService projectService,
        IMemberService memberService,
        UserManager<MemberEntity> userManager
    )
    {
        _projectService = projectService;
        _memberService = memberService;
        _userManager = userManager;
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
    [Route("GetProject/{id}")]
    public async Task<IActionResult> GetProject(string id) // Change parameter type to string
    {
        var project = await _projectService.GetProjectForEditAsync(id);

        if (project == null)
        {
            return Json(new { success = false, error = "Project not found" });
        }

        return Json(
            new
            {
                success = true,
                project = new
                {
                    name = project.Name,
                    clientName = project.ClientName,
                    description = project.Description,
                    startDate = project.StartDate,
                    endDate = project.EndDate,
                    budget = project.Budget,
                    isActive = project.IsActive,
                    statusId = project.StatusId,
                    memberIds = project.MemberIds,
                },
            }
        );
    }

    [HttpPost]
    [Route("AddProject")]
    public async Task<IActionResult> AddProject(AddProjectForm form)
    {
        Console.WriteLine("--- AddProject Action Entered ---"); // Log entry

        try
        {
            Console.WriteLine("AddProject method called in ProjectsController");
            Console.WriteLine(
                $"Form data: Name={form.Name}, ClientId={form.ClientId}, Description={form.Description?.Length ?? 0} chars" // Changed ClientName to ClientId
            );
            Console.WriteLine(
                $"Form data: StartDate={form.StartDate}, EndDate={form.EndDate}, Budget={form.Budget}"
            );
            Console.WriteLine(
                $"Form data: MemberIds={form.MemberIds?.Count ?? 0}, HasImage={form.ProjectImage != null}"
            );

            await SetCurrentUserAsync();

            // Log all model state errors regardless of validity
            if (ModelState.Keys.Count() > 0)
            {
                Console.WriteLine("ModelState keys present: " + string.Join(", ", ModelState.Keys));
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    Console.WriteLine(
                        $"Field: {key}, Valid: {state.ValidationState}, Errors: {state.Errors.Count}"
                    );

                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"  - Error: {error.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid, calling AddProjectAsync");
                var result = await _projectService.AddProjectAsync(form);

                if (result)
                {
                    Console.WriteLine("Project added successfully");
                    TempData["Success"] = "Project added successfully!";
                }
                else
                {
                    Console.WriteLine("Service returned false for AddProjectAsync");
                    TempData["Error"] = "Failed to add project. Please try again.";
                }
            }
            else
            {
                Console.WriteLine("Model state is invalid");
                var errorMessages = string.Join(
                    "; ",
                    ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
                );
                Console.WriteLine($"Validation errors: {errorMessages}");
                TempData["Error"] = $"Failed to add project. {errorMessages}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddProject: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            TempData["Error"] = "There was an error adding the project.";
        }

        return RedirectToAction("Projects", "Admin");
    }

    [HttpPost]
    [Route("EditProject")]
    public async Task<IActionResult> EditProject(EditProjectForm form)
    {
        await SetCurrentUserAsync();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Failed to update project. Please check the form and try again.";
            return RedirectToAction("Projects", "Admin");
        }

        var success = await _projectService.EditProjectAsync(form);
        if (!success)
        {
            TempData["Error"] = "Failed to update project. Please try again.";
            return RedirectToAction("Projects", "Admin");
        }

        TempData["Success"] = "Project updated successfully!";
        return RedirectToAction("Projects", "Admin");
    }

    [HttpPost]
    [Route("DeleteProject")]
    public async Task<IActionResult> DeleteProject(string id) // Change parameter type to string
    {
        try
        {
            Console.WriteLine($"DeleteProject method called for project ID: {id}");
            await SetCurrentUserAsync();

            // Check if the string ID is null or empty instead of <= 0
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid project ID.";
                return RedirectToAction("Projects", "Admin");
            }

            var success = await _projectService.DeleteProjectAsync(id);
            if (success)
            {
                TempData["Success"] = "Project deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete project. Please try again.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteProject: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            TempData["Error"] = "There was an error deleting the project.";
        }

        return RedirectToAction("Projects", "Admin");
    }
}
