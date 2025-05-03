using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

/// <summary>
/// Repository for managing status entities in the database.
/// Inherits from BaseRepository to provide CRUD operations for StatusEntity.
/// </summary>
/// <remarks>
/// This repository handles all data access operations for status records:
/// - Managing status types and states
/// - Retrieving status information
/// - Updating status details
/// - Maintaining status relationships
/// - Had help from AI to fix errors and refactor the code
///
/// Status entities are typically used to track the state of other entities
/// in the system (e.g., project status, task status).
/// The repository provides the data access layer for these status operations
/// while leveraging the base repository implementation.
/// </remarks>
public class StatusRepository : BaseRepository<StatusEntity>, IStatusRepository
{
    /// <summary>
    /// Initializes a new instance of the StatusRepository class.
    /// </summary>
    /// <param name="context">The database context used for status operations.</param>
    public StatusRepository(AppDbContext context)
        : base(context) { }
}
