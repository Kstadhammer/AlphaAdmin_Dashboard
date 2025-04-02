using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;

namespace Business.Services;

public class StatusService : IStatusService
{
    private readonly IStatusRepository _statusRepository;
    private readonly IStatusFactory _statusFactory;

    public StatusService(IStatusRepository statusRepository, IStatusFactory statusFactory)
    {
        _statusRepository = statusRepository;
        _statusFactory = statusFactory;
    }

    public async Task<StatusResult<IEnumerable<Business.Models.Status>>> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync();
        if (!result.Succeeded)
        {
            return new StatusResult<IEnumerable<Business.Models.Status>>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error,
            };
        }

        var statuses = result
            .Result.Select(entity => _statusFactory.CreateStatusModel(entity))
            .ToList();

        return new StatusResult<IEnumerable<Business.Models.Status>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = statuses,
        };
    }

    public async Task<StatusResult<Business.Models.Status>> GetStatusByNameAsync(string statusName)
    {
        var result = await _statusRepository.ExistsAsync(x => x.Name == statusName);
        if (!result.Succeeded || !result.Result)
        {
            return new StatusResult<Business.Models.Status>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "Status not found",
            };
        }

        // This is a simplified implementation since we don't have GetAsync
        // In a real implementation, you would get the entity and convert it
        return new StatusResult<Business.Models.Status>
        {
            Succeeded = false,
            StatusCode = 501,
            Error = "Not implemented",
        };
    }

    public async Task<StatusResult<IEnumerable<Business.Models.Status>>> GetStatusByIdAsync(int id)
    {
        // Implementation
        throw new NotImplementedException();
    }
}
