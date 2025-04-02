using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectFactory _projectFactory;

    public ProjectService(IProjectRepository projectRepository, IProjectFactory projectFactory)
    {
        _projectRepository = projectRepository;
        _projectFactory = projectFactory;
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
                ClientName = formData.ClientName,
                Description = formData.Description,
                StartDate = formData.StartDate,
                EndDate = formData.EndDate,
                Budget = formData.Budget,
                IsActive = true,
            };

            var entity = _projectFactory.CreateProjectEntity(form);
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
        // Note: This is a stub implementation since your actual repository uses string IDs
        var result = await _projectRepository.GetByIdAsync(id.ToString());
        if (!result.Succeeded)
        {
            return null;
        }

        return _projectFactory.CreateEditProjectForm(result.Result);
    }

    public async Task<bool> AddProjectAsync(AddProjectForm form)
    {
        if (form == null)
            return false;

        try
        {
            var entity = _projectFactory.CreateProjectEntity(form);
            var result = await _projectRepository.AddAsync(entity);

            return result.Succeeded;
        }
        catch (Exception)
        {
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
            // Note: This is a stub implementation since your actual repository uses string IDs
            var result = await _projectRepository.DeleteAsync(id.ToString());
            return result.Succeeded;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
