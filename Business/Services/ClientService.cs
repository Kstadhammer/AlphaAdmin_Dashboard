using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IProjectRepository _projectRepository; // Inject Project Repository
    private readonly IClientFactory _clientFactory;

    public ClientService(
        IClientRepository clientRepository,
        IClientFactory clientFactory,
        IProjectRepository projectRepository // Add Project Repository parameter
    )
    {
        _clientRepository = clientRepository;
        _clientFactory = clientFactory;
        _projectRepository = projectRepository; // Assign Project Repository
    }

    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return result.MapTo<ClientResult>();
    }

    public async Task<List<ClientListItem>> GetAllClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        if (!result.Succeeded)
        {
            return new List<ClientListItem>();
        }

        var clients = new List<ClientListItem>();
        foreach (var entity in result.Result)
        {
            clients.Add(_clientFactory.CreateClientListItem(entity));
        }

        return clients;
    }

    public async Task<EditClientForm> GetClientForEditAsync(int id)
    {
        try
        {
            // Convert the integer ID to string for the repository
            var clientId = id.ToString();
            var result = await _clientRepository.GetByIdAsync(clientId);

            if (!result.Succeeded || result.Result == null)
            {
                return null;
            }

            return _clientFactory.CreateEditClientForm(result.Result);
        }
        catch (System.Exception)
        {
            // Log error here if needed
            return null;
        }
    }

    public async Task<bool> AddClientAsync(AddClientForm form)
    {
        if (form == null)
            return false;

        var entity = _clientFactory.CreateClientEntity(form);
        var result = await _clientRepository.AddAsync(entity);

        return result.Succeeded;
    }

    public async Task<bool> EditClientAsync(EditClientForm form)
    {
        if (form == null)
            return false;

        var getResult = await _clientRepository.GetByIdAsync(form.Id);
        if (!getResult.Succeeded)
        {
            return false;
        }

        var updatedEntity = _clientFactory.UpdateClientEntity(getResult.Result, form);
        var result = await _clientRepository.UpdateAsync(updatedEntity);

        return result.Succeeded;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var clientId = id.ToString(); // Convert int ID to string

        try
        {
            // Check if any projects are associated with this client
            var projectsExistResult = await _projectRepository.ExistsAsync(p =>
                p.ClientId == clientId
            );

            if (projectsExistResult.Succeeded && projectsExistResult.Result)
            {
                // Found associated projects, prevent deletion
                Console.WriteLine($"Attempted to delete client {clientId} with existing projects."); // Optional logging
                return false; // Indicate failure because projects exist
            }
            else if (!projectsExistResult.Succeeded)
            {
                // Handle error during project check
                Console.WriteLine(
                    $"Error checking projects for client {clientId}: {projectsExistResult.Error}"
                ); // Optional logging
                return false; // Indicate failure due to error
            }

            // No associated projects found (or check failed but we proceed cautiously), attempt deletion
            var deleteResult = await _clientRepository.DeleteAsync(clientId);
            return deleteResult.Succeeded;
        }
        catch (Exception ex) // Catch potential exceptions during the process
        {
            Console.WriteLine($"Error deleting client {clientId}: {ex.Message}"); // Log error
            return false;
        }
    }
}
