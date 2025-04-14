using Business.Forms;
using Business.Models;
using Data.Entities;
using Domain.Models;

namespace Business.Interfaces;

public interface IProjectFactory
{
    ProjectEntity CreateProjectEntity(
        AddProjectForm form,
        string userId,
        string clientName, // Added clientName parameter
        string? imageUrl = null
    );
    ProjectEntity UpdateProjectEntity(
        ProjectEntity entity,
        EditProjectForm form,
        string? imageUrl = null
    );
    ProjectListItem CreateProjectListItem(ProjectEntity entity);
    EditProjectForm CreateEditProjectForm(ProjectEntity entity);
    Project CreateProjectModel(ProjectEntity entity);
}
