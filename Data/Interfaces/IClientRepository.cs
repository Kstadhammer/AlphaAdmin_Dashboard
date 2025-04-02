using Data.Entities;

namespace Data.Interfaces;

public interface IClientRepository : IBaseRepository<ClientEntity>
{
    // Add client-specific repository methods here
}
