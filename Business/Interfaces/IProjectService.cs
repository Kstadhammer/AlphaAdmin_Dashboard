using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IProjectService
{
    Task<ProjectResult<Project>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult> CreateProjectAsync(Business.Models.AddProjectFormData formData);
    Task<List<ProjectListItem>> GetAllProjectsAsync();
    Task<EditProjectForm> GetProjectForEditAsync(int id);
    Task<bool> AddProjectAsync(AddProjectForm form);
    Task<bool> EditProjectAsync(EditProjectForm form);
    Task<bool> DeleteProjectAsync(int id);
}
