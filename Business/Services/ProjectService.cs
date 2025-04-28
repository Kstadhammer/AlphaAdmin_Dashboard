using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.Forms;
using Business.Interfaces;
using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// I got assistance by Gemini 2.5 to implement and refactor this codebase

namespace Business.Services;

/// <summary>
/// Service responsible for all project-related business logic, including CRUD operations,
/// file handling, and data transformation between different layers of the application.
/// </summary>
public class ProjectService : IProjectService
{
    // Repositories for data access
    private readonly IProjectRepository _projectRepository;
    private readonly IClientRepository _clientRepository;

    // Factory for creating business models
    private readonly IProjectFactory _projectFactory;

    // Identity and context access
    private readonly UserManager<MemberEntity> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // For file operations and hosting environment access
    private readonly IWebHostEnvironment _webHostEnvironment;

    // Direct database context for complex queries
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Constructor that injects all required dependencies for project operations
    /// </summary>
    /// <param name="projectRepository">Repository for project data access</param>
    /// <param name="projectFactory">Factory for creating project models</param>
    /// <param name="userManager">ASP.NET Identity user manager</param>
    /// <param name="httpContextAccessor">For accessing the current HTTP context</param>
    /// <param name="webHostEnvironment">For file system operations and paths</param>
    /// <param name="clientRepository">Repository for client data access</param>
    /// <param name="dbContext">Direct database context for complex queries</param>
    public ProjectService(
        IProjectRepository projectRepository,
        IProjectFactory projectFactory,
        UserManager<MemberEntity> userManager,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment,
        IClientRepository clientRepository,
        AppDbContext dbContext
    )
    {
        _projectRepository = projectRepository;
        _projectFactory = projectFactory;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _webHostEnvironment = webHostEnvironment;
        _clientRepository = clientRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves a single project by its unique identifier
    /// Transforms the data entity to a business model
    /// </summary>
    /// <param name="id">Unique identifier of the project</param>
    /// <returns>Service result containing the project or error information</returns>
    public async Task<ServiceResult<Project>> GetProjectAsync(string id)
    {
        // Get project entity from repository
        var result = await _projectRepository.GetByIdAsync(id);

        // Return error if project not found or other repository error
        if (!result.Succeeded)
        {
            return new ServiceResult<Project>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error,
            };
        }

        // Transform entity to business model using factory
        var project = _projectFactory.CreateProjectModel(result.Result);

        // Return successful result with project data
        return new ServiceResult<Project>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = project,
        };
    }

    /// <summary>
    /// Retrieves all projects from the database
    /// Transforms data entities to business models
    /// </summary>
    /// <returns>Service result containing collection of projects or error information</returns>
    public async Task<ServiceResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        // Get all project entities from repository
        var result = await _projectRepository.GetAllAsync();

        // Return error if repository operation failed
        if (!result.Succeeded)
        {
            return new ServiceResult<IEnumerable<Project>>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error,
            };
        }

        // Transform each entity to a business model
        var projects = result
            .Result.Select(entity => _projectFactory.CreateProjectModel(entity))
            .ToList();

        // Return successful result with projects collection
        return new ServiceResult<IEnumerable<Project>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = projects,
        };
    }

    /// <summary>
    /// Retrieves all projects with related data (status, members) for display in list views
    /// Uses eager loading to optimize data access
    /// </summary>
    /// <returns>List of project list items with complete information</returns>
    public async Task<List<ProjectListItem>> GetAllProjectsAsync()
    {
        // Get all projects with their status information using eager loading
        var result = await _projectRepository.GetAllAsync(include: query =>
            query.Include(p => p.Status)
        );

        // Return empty list if repository operation failed
        if (!result.Succeeded)
        {
            return new List<ProjectListItem>();
        }

        var projects = new List<ProjectListItem>();
        foreach (var entity in result.Result)
        {
            // For each project, load its members from the many-to-many relationship table
            var memberIds = await _dbContext
                .ProjectMembers.Where(pm => pm.ProjectId == entity.Id)
                .Select(pm => pm.MemberId)
                .ToListAsync();

            // Load complete member information for each assigned member
            var members = await _userManager
                .Users.Where(m => memberIds.Contains(m.Id))
                .ToListAsync();

            // Populate the navigation property with loaded members
            entity.Members = members;

            // Transform to list item model and add to result collection
            projects.Add(_projectFactory.CreateProjectListItem(entity));
        }

        return projects;
    }

    /// <summary>
    /// Retrieves a project with all data needed for the edit form
    /// Includes associated members for selection in the UI
    /// </summary>
    /// <param name="id">Unique identifier of the project to edit</param>
    /// <returns>Form object with project data or null if not found/error</returns>
    public async Task<EditProjectForm?> GetProjectForEditAsync(string id)
    {
        try
        {
            // Retrieve the project entity by ID
            var result = await _projectRepository.GetByIdAsync(id);

            // Return null if project not found or repository error
            if (!result.Succeeded || result.Result == null)
            {
                return null;
            }

            // Load project members from the many-to-many relationship table
            var memberIds = await _dbContext
                .ProjectMembers.Where(pm => pm.ProjectId == id)
                .Select(pm => pm.MemberId)
                .ToListAsync();

            // Load complete member information for each assigned member
            var members = await _userManager
                .Users.Where(m => memberIds.Contains(m.Id))
                .ToListAsync();

            // Populate the navigation property with loaded members
            result.Result.Members = members;

            // Transform entity to edit form model using factory
            return _projectFactory.CreateEditProjectForm(result.Result);
        }
        catch (Exception ex)
        {
            // Log error and return null on exception
            Console.WriteLine($"Error in GetProjectForEditAsync: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Creates a new project based on the submitted form data
    /// Handles related operations like image uploads and member assignments
    /// </summary>
    /// <param name="form">Form containing the new project data</param>
    /// <returns>True if project was created successfully, false otherwise</returns>
    public async Task<bool> AddProjectAsync(AddProjectForm form)
    {
        // Validate form and context
        if (form == null || _httpContextAccessor.HttpContext == null)
            return false;

        try
        {
            // Get current authenticated user for audit info
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
                return false; // Not authenticated or user not found
            var userId = user.Id;

            // Retrieve client information from the selected client ID
            if (string.IsNullOrWhiteSpace(form.ClientId))
                return false; // Client selection is required

            var clientResult = await _clientRepository.GetByIdAsync(form.ClientId);
            if (!clientResult.Succeeded || clientResult.Result == null)
            {
                // Client not found in database
                Console.WriteLine($"Client not found for ID: {form.ClientId}");
                return false;
            }
            var clientName = clientResult.Result.ClientName; // Get client name for project
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
            // 3. Create Entity using Factory (passing userId, clientName, and imageUrl)
            var entity = _projectFactory.CreateProjectEntity(form, userId, clientName, imageUrl);

            // 4. Add Entity via Repository
            Console.WriteLine(
                $"--- Attempting to add project with: ClientId={entity.ClientId}, UserId={entity.UserId}, StatusId={entity.StatusId} ---"
            );
            // ### DIAGNOSTIC LOGGING: Check available Status IDs right before save ###
            try
            {
                // We need the DbContext. Access it through one of the injected repositories.
                // Use the injected DbContext directly
                var dbContext = _dbContext;
                var allStatuses = await _dbContext.Statuses.Select(s => s.Id).ToListAsync(); // Use DbSet directly
                Console.WriteLine(
                    $"--- Available Status IDs in DB before save: [{string.Join(", ", allStatuses)}] ---"
                );
            }
            catch (Exception dbEx)
            {
                Console.WriteLine($"--- ERROR fetching Status IDs before save: {dbEx.Message} ---");
            }
            // ### END DIAGNOSTIC LOGGING ###



            var result = await _projectRepository.AddAsync(entity);

            // If project was added successfully and there are members to add
            if (result.Succeeded && form.MemberIds != null && form.MemberIds.Any())
            {
                // Add project members to the join table
                foreach (var memberId in form.MemberIds)
                {
                    await _dbContext.ProjectMembers.AddAsync(
                        new ProjectMemberEntity { ProjectId = entity.Id, MemberId = memberId }
                    );
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            // TODO: Add proper logging here
            Console.WriteLine($"Error adding project: {ex.Message}"); // Basic console logging
            return false;
        }
    }

    /// <summary>
    /// Updates an existing project based on the submitted form data.
    /// Handles related operations like member assignment updates.
    /// </summary>
    /// <param name="form">Form containing the updated project data.</param>
    /// <returns>True if the project was updated successfully, false otherwise.</returns>
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

            if (result.Succeeded)
            {
                // Update project members
                // First, remove all existing project members
                var existingMembers = await _dbContext
                    .ProjectMembers.Where(pm => pm.ProjectId == form.Id)
                    .ToListAsync();

                _dbContext.ProjectMembers.RemoveRange(existingMembers);

                // Then add the new members
                if (form.MemberIds != null && form.MemberIds.Count > 0)
                {
                    foreach (var memberId in form.MemberIds)
                    {
                        await _dbContext.ProjectMembers.AddAsync(
                            new ProjectMemberEntity { ProjectId = form.Id, MemberId = memberId }
                        );
                    }
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing project: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Deletes a project and its associated member relationships.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <returns>True if the project was deleted successfully, false otherwise.</returns>
    public async Task<bool> DeleteProjectAsync(string id) // Changed id to string
    {
        try
        {
            // First, delete all project members
            var projectMembers = await _dbContext
                .ProjectMembers.Where(pm => pm.ProjectId == id)
                .ToListAsync();

            if (projectMembers.Any())
            {
                _dbContext.ProjectMembers.RemoveRange(projectMembers);
                await _dbContext.SaveChangesAsync();
            }

            // Then delete the project
            var result = await _projectRepository.DeleteAsync(id);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting project: {ex.Message}");
            return false;
        }
    }
}
