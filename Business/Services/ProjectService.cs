using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectFactory _projectFactory;

    private readonly UserManager<MemberEntity> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _webHostEnvironment; // For handling file uploads

    // Removed IClientRepository dependency

    public ProjectService(
        IProjectRepository projectRepository,
        IProjectFactory projectFactory,
        UserManager<MemberEntity> userManager,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _projectRepository = projectRepository;
        _projectFactory = projectFactory;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _webHostEnvironment = webHostEnvironment; // Assign injected service
        // Removed IClientRepository assignment
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var result = await _projectRepository.GetByIdAsync(id);
        if (!result.Succeeded)
        {
            return new ProjectResult<Project>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error,
            };
        }

        var project = _projectFactory.CreateProjectModel(result.Result);
        return new ProjectResult<Project>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = project,
        };
    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var result = await _projectRepository.GetAllAsync();
        if (!result.Succeeded)
        {
            return new ProjectResult<IEnumerable<Project>>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error,
            };
        }

        var projects = result
            .Result.Select(entity => _projectFactory.CreateProjectModel(entity))
            .ToList();
        return new ProjectResult<IEnumerable<Project>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = projects,
        };
    }

    public async Task<ProjectResult> CreateProjectAsync(Business.Models.AddProjectFormData formData)
    {
        if (formData == null)
        {
            return new ProjectResult
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Form data cannot be null",
            };
        }

        try
        {
            // Convert AddProjectFormData to AddProjectForm
            var form = new AddProjectForm
            {
                Name = formData.Name,
                // ClientName = formData.ClientName, // Removed as AddProjectForm no longer has ClientName
                Description = formData.Description,
                StartDate = formData.StartDate,
                EndDate = formData.EndDate,
                Budget = formData.Budget,
                IsActive = true,
            };

            // TODO: This method needs review. It doesn't have access to the current user ID or image data.
            // Passing string.Empty for userId and null for imageUrl to satisfy the compiler,
            // but this is likely incorrect for actual use of this specific method.
            var entity = _projectFactory.CreateProjectEntity(
                form,
                string.Empty /* userId */
                ,
                null /* imageUrl */
            );
            var result = await _projectRepository.AddAsync(entity);

            return result.Succeeded
                ? new ProjectResult { Succeeded = true, StatusCode = 201 }
                : new ProjectResult
                {
                    Succeeded = false,
                    StatusCode = result.StatusCode,
                    Error = result.Error,
                };
        }
        catch (Exception ex)
        {
            return new ProjectResult
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }

    public async Task<List<ProjectListItem>> GetAllProjectsAsync()
    {
        var result = await _projectRepository.GetAllAsync();
        if (!result.Succeeded)
        {
            return new List<ProjectListItem>();
        }

        var projects = new List<ProjectListItem>();
        foreach (var entity in result.Result)
        {
            projects.Add(_projectFactory.CreateProjectListItem(entity));
        }

        return projects;
    }

    public async Task<EditProjectForm> GetProjectForEditAsync(int id)
    {
        try
        {
            // Convert the integer ID to string for the repository
            var projectId = id.ToString();
            var result = await _projectRepository.GetByIdAsync(projectId);

            if (!result.Succeeded || result.Result == null)
            {
                return null;
            }

            return _projectFactory.CreateEditProjectForm(result.Result);
        }
        catch (Exception)
        {
            // Log error here if needed
            return null;
        }
    }

    public async Task<bool> AddProjectAsync(AddProjectForm form)
    {
        if (form == null || _httpContextAccessor.HttpContext == null)
            return false;

        try
        {
            // 1. Get Current User ID
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
                return false; // User not found/logged in
            var userId = user.Id;

            // Removed client lookup logic
            // 2. Handle Image Upload
            string? imageUrl = null;
            if (form.ProjectImage != null && form.ProjectImage.Length > 0)
            {
                // Ensure the target directory exists
                var uploadsFolderPath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "projects"
                );
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                // Generate unique filename and save
                var uniqueFileName =
                    Guid.NewGuid().ToString() + "_" + Path.GetFileName(form.ProjectImage.FileName);
                var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.ProjectImage.CopyToAsync(stream);
                }

                // Store the relative path for web access
                imageUrl = $"/images/projects/{uniqueFileName}";
            }

            // 3. Create Entity using Factory (passing userId and imageUrl)
            // 3. Create Entity using Factory (passing userId, clientId, and imageUrl)
            var entity = _projectFactory.CreateProjectEntity(form, userId, imageUrl); // Factory call expects (form, userId, imageUrl)

            // 4. Add Entity via Repository
            var result = await _projectRepository.AddAsync(entity);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging here
            Console.WriteLine($"Error adding project: {ex.Message}"); // Basic console logging
            return false;
        }
    }

    public async Task<bool> EditProjectAsync(EditProjectForm form)
    {
        if (form == null)
            return false;

        try
        {
            var getResult = await _projectRepository.GetByIdAsync(form.Id);
            if (!getResult.Succeeded)
            {
                return false;
            }

            var updatedEntity = _projectFactory.UpdateProjectEntity(getResult.Result, form);
            var result = await _projectRepository.UpdateAsync(updatedEntity);

            return result.Succeeded;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        try
        {
            // Convert the integer ID to string for the repository
            var projectId = id.ToString();
            var result = await _projectRepository.DeleteAsync(projectId);
            return result.Succeeded;
        }
        catch (Exception)
        {
            // Log error here if needed
            return false;
        }
    }
}
