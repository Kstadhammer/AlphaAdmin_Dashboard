using Business.Models;

namespace Business.Interfaces;

public interface IStatusService
{
    Task<ServiceResult<IEnumerable<Business.Models.Status>>> GetStatusesAsync();
    Task<ServiceResult<Business.Models.Status>> GetStatusByNameAsync(string statusName);
    Task<ServiceResult<Business.Models.Status>> GetStatusByIdAsync(int id);
}
