using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

/// <summary>
/// Repository for managing user entities in the database.
/// Inherits from BaseRepository to provide CRUD operations for UserEntity.
/// </summary>
/// <remarks>
/// This repository is responsible for all data access operations related to users:
/// - User account management
/// - Profile information retrieval and updates
/// - User authentication data
/// - User-specific queries and operations
///
/// The repository handles sensitive user data and should be used in conjunction
/// with appropriate security measures and authentication services.
/// It leverages the base repository implementation while allowing for
/// user-specific data access methods when needed.
/// </remarks>
public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The database context used for user operations.</param>
    public UserRepository(AppDbContext context)
        : base(context) { }
}
