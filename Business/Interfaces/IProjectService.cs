using Business.Forms;
using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IProjectService
{
    Task<ServiceResult<Project>> GetProjectAsync(string id);
    Task<ServiceResult<IEnumerable<Project>>> GetProjectsAsync();

    // Removed unused CreateProjectAsync(AddProjectFormData formData) signature
    Task<List<ProjectListItem>> GetAllProjectsAsync();
    Task<EditProjectForm?> GetProjectForEditAsync(string id); // Changed id to string, added nullable return
    Task<bool> AddProjectAsync(AddProjectForm form);
    Task<bool> EditProjectAsync(EditProjectForm form);
    Task<bool> DeleteProjectAsync(string id); // Changed id to string
}
