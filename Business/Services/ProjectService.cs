using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Contexts; // Add using for AppDbContext
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Add for ToListAsync
using Microsoft.EntityFrameworkCore; // Add for Include

namespace Business.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectFactory _projectFactory;

    private readonly UserManager<MemberEntity> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _webHostEnvironment; // For handling file uploads

    private readonly IClientRepository _clientRepository; // Inject Client Repository
    private readonly AppDbContext _dbContext; // Inject DbContext

    public ProjectService(
        IProjectRepository projectRepository,
        IProjectFactory projectFactory,
        UserManager<MemberEntity> userManager,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment,
        IClientRepository clientRepository, // Add Client Repository parameter
        AppDbContext dbContext // Add DbContext parameter
    )
    {
        _projectRepository = projectRepository;
        _projectFactory = projectFactory;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _webHostEnvironment = webHostEnvironment; // Assign injected service
        _clientRepository = clientRepository; // Assign Client Repository
        _dbContext = dbContext; // Assign DbContext
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

    // Removed unused CreateProjectAsync(AddProjectFormData formData) method
    public async Task<List<ProjectListItem>> GetAllProjectsAsync()
    {
        // Include Status navigation property
        var result = await _projectRepository.GetAllAsync(include: query =>
            query.Include(p => p.Status)
        );
        if (!result.Succeeded)
        {
            return new List<ProjectListItem>();
        }

        var projects = new List<ProjectListItem>();
        foreach (var entity in result.Result)
        {
            // For each project, load its members from the ProjectMembers table
            var memberIds = await _dbContext
                .ProjectMembers.Where(pm => pm.ProjectId == entity.Id)
                .Select(pm => pm.MemberId)
                .ToListAsync();

            // Load member details for each member ID
            var members = await _userManager
                .Users.Where(m => memberIds.Contains(m.Id))
                .ToListAsync();

            // Assign members to the project entity
            entity.Members = members;

            // Add the project to the list
            projects.Add(_projectFactory.CreateProjectListItem(entity));
        }

        return projects;
    }

    public async Task<EditProjectForm?> GetProjectForEditAsync(string id) // Changed id to string, added nullable return
    {
        try
        {
            // ID is already a string
            var result = await _projectRepository.GetByIdAsync(id); // Use id directly

            if (!result.Succeeded || result.Result == null)
            {
                return null; // Return null if not found
            }

            return _projectFactory.CreateEditProjectForm(result.Result);
        }
        catch (Exception ex) // Declare exception variable ex
        {
            Console.WriteLine($"Error in GetProjectForEditAsync: {ex.Message}"); // Basic logging
            return null; // Return null on exception
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

            // 1.5 Get Client Name from ClientId
            if (string.IsNullOrWhiteSpace(form.ClientId))
                return false; // No client selected
            var clientResult = await _clientRepository.GetByIdAsync(form.ClientId);
            if (!clientResult.Succeeded || clientResult.Result == null)
            {
                // Selected client not found in DB (shouldn't happen with dropdown)
                Console.WriteLine($"Client not found for ID: {form.ClientId}");
                return false;
            }
            var clientName = clientResult.Result.ClientName; // Get the name
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
