using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class ClientRepository : BaseRepository<ClientEntity>, IClientRepository
{
    public ClientRepository(AppDbContext context)
        : base(context) { }
}
