using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IProjectService
{
    Task<ProjectResult<Project>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();

    // Removed unused CreateProjectAsync(AddProjectFormData formData) signature
    Task<List<ProjectListItem>> GetAllProjectsAsync();
    Task<EditProjectForm?> GetProjectForEditAsync(string id); // Changed id to string, added nullable return
    Task<bool> AddProjectAsync(AddProjectForm form);
    Task<bool> EditProjectAsync(EditProjectForm form);
    Task<bool> DeleteProjectAsync(string id); // Changed id to string
}
