using System;
using System.Collections.Generic;
using System.Diagnostics; // Add for Debug.WriteLine
using System.Linq;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
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

    public async Task<ServiceResult<IEnumerable<Business.Models.Status>>> GetStatusesAsync()
    {
        Debug.WriteLine("StatusService: Fetching all statuses from repository...");
        var result = await _statusRepository.GetAllAsync();
        Debug.WriteLine(
            $"StatusService: Repository result Succeeded={result.Succeeded}, StatusCode={result.StatusCode}, Error={result.Error ?? "None"}, Count={result.Result?.Count() ?? 0}"
        );

        if (!result.Succeeded)
        {
            return new ServiceResult<IEnumerable<Business.Models.Status>>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error,
            };
        }

        var statuses = result
            .Result.Select(entity => _statusFactory.CreateStatusModel(entity))
            .ToList();
        Debug.WriteLine($"StatusService: Mapped {statuses.Count} status models.");

        return new ServiceResult<IEnumerable<Business.Models.Status>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = statuses,
        };
    }

    public async Task<ServiceResult<Business.Models.Status>> GetStatusByNameAsync(string statusName)
    {
        if (string.IsNullOrEmpty(statusName))
        {
            return new ServiceResult<Business.Models.Status>
            {
                Succeeded = false,
                StatusCode = 400,
                Error = "Status name cannot be empty",
            };
        }

        try
        {
            // Get all statuses and filter by name
            var result = await _statusRepository.GetAllAsync();
            if (!result.Succeeded)
            {
                return new ServiceResult<Business.Models.Status>
                {
                    Succeeded = false,
                    StatusCode = result.StatusCode,
                    Error = result.Error,
                };
            }

            var statusEntity = result.Result.FirstOrDefault(s =>
                s.Name.Equals(statusName, StringComparison.OrdinalIgnoreCase)
            );
            if (statusEntity == null)
            {
                return new ServiceResult<Business.Models.Status>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = $"Status with name '{statusName}' not found",
                };
            }

            var status = _statusFactory.CreateStatusModel(statusEntity);
            return new ServiceResult<Business.Models.Status>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = status,
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult<Business.Models.Status>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = $"Error retrieving status: {ex.Message}",
            };
        }
    }

    public async Task<ServiceResult<Business.Models.Status>> GetStatusByIdAsync(int id)
    {
        try
        {
            // Convert the integer ID to string for the repository
            var statusId = id.ToString();
            var result = await _statusRepository.GetByIdAsync(statusId);

            if (!result.Succeeded)
            {
                return new ServiceResult<Business.Models.Status>
                {
                    Succeeded = false,
                    StatusCode = result.StatusCode,
                    Error = result.Error,
                };
            }

            if (result.Result == null)
            {
                return new ServiceResult<Business.Models.Status>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = $"Status with ID {id} not found",
                };
            }

            var status = _statusFactory.CreateStatusModel(result.Result);
            return new ServiceResult<Business.Models.Status>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = status,
            };
        }
        catch (Exception ex)
        {
            return new ServiceResult<Business.Models.Status>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = $"Error retrieving status: {ex.Message}",
            };
        }
    }
}
