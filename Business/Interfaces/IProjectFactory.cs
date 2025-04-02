using Business.Models;
using Data.Entities;
using Domain.Models;

namespace Business.Interfaces;

public interface IProjectFactory
{
    ProjectEntity CreateProjectEntity(
        AddProjectForm form,
        string userId,
        // Removed clientId parameter
        string? imageUrl = null
    ); // Reverted: Removed clientId parameter
    ProjectEntity UpdateProjectEntity(
        ProjectEntity entity,
        EditProjectForm form,
        string? imageUrl = null
    );
    ProjectListItem CreateProjectListItem(ProjectEntity entity);
    EditProjectForm CreateEditProjectForm(ProjectEntity entity);
    Project CreateProjectModel(ProjectEntity entity);
}
