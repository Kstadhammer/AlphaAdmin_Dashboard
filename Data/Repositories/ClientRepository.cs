using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

/// <summary>
/// Repository for managing client entities in the database.
/// Inherits from BaseRepository to provide CRUD operations for ClientEntity.
/// </summary>
/// <remarks>
/// This repository is responsible for all data access operations related to clients:
/// - Managing client records
/// - Retrieving client information
/// - Updating client details
/// - Handling client deletion
/// - Supporting client-related queries
///
/// The repository uses Entity Framework Core through the base repository
/// implementation while maintaining the flexibility to add client-specific
/// data access methods when needed.
/// </remarks>
public class ClientRepository : BaseRepository<ClientEntity>, IClientRepository
{
    /// <summary>
    /// Initializes a new instance of the ClientRepository class.
    /// </summary>
    /// <param name="context">The database context used for client operations.</param>
    public ClientRepository(AppDbContext context)
        : base(context) { }
}
