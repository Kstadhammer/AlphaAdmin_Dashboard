using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

/// <summary>
/// Repository for managing project entities in the database.
/// Inherits from BaseRepository to provide CRUD operations for ProjectEntity.
/// </summary>
/// <remarks>
/// This repository handles all data access operations for projects including:
/// - Creating new projects
/// - Retrieving project details
/// - Updating project information
/// - Deleting projects
/// - Querying project data
///
/// It uses the base repository implementation while allowing for
/// project-specific extensions when needed.
/// </remarks>
public class ProjectRepository : BaseRepository<ProjectEntity>, IProjectRepository
{
    /// <summary>
    /// Initializes a new instance of the ProjectRepository class.
    /// </summary>
    /// <param name="context">The database context used for project operations.</param>
    public ProjectRepository(AppDbContext context)
        : base(context) { }
}
