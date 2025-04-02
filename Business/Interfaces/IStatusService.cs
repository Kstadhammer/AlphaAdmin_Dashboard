using Business.Models;

namespace Business.Interfaces;

public interface IStatusService
{
    Task<StatusResult<IEnumerable<Business.Models.Status>>> GetStatusesAsync();
    Task<StatusResult<Business.Models.Status>> GetStatusByNameAsync(string statusName);
    Task<StatusResult<Business.Models.Status>> GetStatusByIdAsync(int id);
}
