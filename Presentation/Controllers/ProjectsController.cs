using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Forms;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

/// <summary>
/// Controller responsible for handling all project-related operations including
/// retrieving, creating, updating, and deleting projects.
/// </summary>
[Route("[controller]")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;
    private readonly IMemberService _memberService;
    private readonly UserManager<MemberEntity> _userManager;

    /// <summary>
    /// Constructor that injects required services for project operations
    /// </summary>
    /// <param name="projectService">Service to handle project operations</param>
    /// <param name="memberService">Service to handle member operations</param>
    /// <param name="userManager">Identity user manager for authentication</param>
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

    /// <summary>
    /// Helper method to set the current user in ViewBag for use in views
    /// This provides user information and authentication status across the application
    /// </summary>
    /// <returns>Task representing the asynchronous operation</returns>
    private async Task SetCurrentUserAsync()
    {
        // Get user ID from the current ClaimsPrincipal
        var userId = _userManager.GetUserId(User);
        if (userId != null)
        {
            // Retrieve complete user profile with additional information
            var currentUser = await _memberService.GetCurrentUserAsync(userId);
            if (currentUser != null)
            {
                // Store the user model in ViewBag for use in views
                ViewBag.CurrentUser = currentUser;
            }
        }
    }

    /// <summary>
    /// API endpoint to retrieve project details for editing
    /// Returns project data in JSON format for use in the edit modal
    /// </summary>
    /// <param name="id">Unique identifier of the project to retrieve</param>
    /// <returns>JSON response containing project details or error message</returns>
    [HttpGet]
    [Route("GetProject/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        // Call service to retrieve project details including related data
        var project = await _projectService.GetProjectForEditAsync(id);

        // Return error response if project not found
        if (project == null)
        {
            return Json(new { success = false, error = "Project not found" });
        }

        // Return success response with project details
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

    /// <summary>
    /// Adds a new project to the system based on submitted form data
    /// Includes comprehensive logging for debugging and validation
    /// </summary>
    /// <param name="form">Form data containing project details</param>
    /// <returns>Redirect to projects list page with status message</returns>
    [HttpPost]
    [Route("AddProject")]
    public async Task<IActionResult> AddProject(AddProjectForm form)
    {
        Console.WriteLine("--- AddProject Action Entered ---"); // Log entry

        try
        {
            // Log form data for debugging purposes
            Console.WriteLine("AddProject method called in ProjectsController");
            Console.WriteLine(
                $"Form data: Name={form.Name}, ClientId={form.ClientId}, Description={form.Description?.Length ?? 0} chars"
            );
            Console.WriteLine(
                $"Form data: StartDate={form.StartDate}, EndDate={form.EndDate}, Budget={form.Budget}"
            );
            Console.WriteLine(
                $"Form data: MemberIds={form.MemberIds?.Count ?? 0}, HasImage={form.ProjectImage != null}"
            );

            // Set current user in ViewBag for the view
            await SetCurrentUserAsync();

            // Detailed logging of ModelState for validation troubleshooting
            if (ModelState.Keys.Count() > 0)
            {
                Console.WriteLine("ModelState keys present: " + string.Join(", ", ModelState.Keys));
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state != null)
                    {
                        Console.WriteLine(
                            $"Field: {key}, Valid: {state.ValidationState}, Errors: {state.Errors.Count}"
                        );

                        foreach (var error in state.Errors)
                        {
                            Console.WriteLine($"  - Error: {error.ErrorMessage}");
                        }
                    }
                }
            }

            // Process the form if valid
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
                // Collect and display validation errors
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
            // Exception handling with detailed logging
            Console.WriteLine($"Error in AddProject: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            TempData["Error"] = "There was an error adding the project.";
        }

        // Redirect to the projects list page
        return RedirectToAction("Projects", "Admin");
    }

    /// <summary>
    /// Updates an existing project with the submitted form data
    /// Performs validation and redirects with appropriate status message
    /// </summary>
    /// <param name="form">Form containing updated project information</param>
    /// <returns>Redirect to projects list page with status message</returns>
    [HttpPost]
    [Route("EditProject")]
    public async Task<IActionResult> EditProject(EditProjectForm form)
    {
        // Set current user in ViewBag for the view
        await SetCurrentUserAsync();

        // Validate the submitted form data
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Failed to update project. Please check the form and try again.";
            return RedirectToAction("Projects", "Admin");
        }

        // Call service to update the project
        var success = await _projectService.EditProjectAsync(form);
        if (!success)
        {
            TempData["Error"] = "Failed to update project. Please try again.";
            return RedirectToAction("Projects", "Admin");
        }

        // Set success message and redirect
        TempData["Success"] = "Project updated successfully!";
        return RedirectToAction("Projects", "Admin");
    }

    /// <summary>
    /// Deletes a project from the system based on its unique identifier
    /// Includes error handling and logging
    /// </summary>
    /// <param name="id">Unique identifier of the project to delete</param>
    /// <returns>Redirect to projects list page with status message</returns>
    [HttpPost]
    [Route("DeleteProject")]
    public async Task<IActionResult> DeleteProject(string id)
    {
        try
        {
            // Log the deletion request
            Console.WriteLine($"DeleteProject method called for project ID: {id}");

            // Set current user in ViewBag
            await SetCurrentUserAsync();

            // Validate the project ID
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid project ID.";
                return RedirectToAction("Projects", "Admin");
            }

            // Call service to delete the project
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
            // Log any exceptions that occur
            Console.WriteLine($"Error in DeleteProject: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            TempData["Error"] = "There was an error deleting the project.";
        }

        // Redirect to the projects list page
        return RedirectToAction("Projects", "Admin");
    }
}
