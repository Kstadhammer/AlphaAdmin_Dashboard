using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class StatusRepository : BaseRepository<StatusEntity>, IStatusRepository
{
    public StatusRepository(AppDbContext context)
        : base(context) { }
}
