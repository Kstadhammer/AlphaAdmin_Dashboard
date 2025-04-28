using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Forms;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

/// <summary>
/// Service responsible for managing client data, including retrieval, creation, updates, and deletion.
/// It ensures business rules are followed, such as preventing deletion of clients with associated projects.
/// </summary>
public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IProjectRepository _projectRepository; // Inject Project Repository
    private readonly IClientFactory _clientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientService"/> class.
    /// </summary>
    /// <param name="clientRepository">Repository for client data access.</param>
    /// <param name="clientFactory">Factory for creating client-related objects.</param>
    /// <param name="projectRepository">Repository for project data access (used for deletion check).</param>
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

    /// <summary>
    /// Retrieves all clients as raw entities.
    /// </summary>
    /// <returns>A <see cref="ServiceResult{T}"/> containing the client entities or an error.</returns>
    public async Task<ServiceResult<object>> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return new ServiceResult<object>
        {
            Succeeded = result.Succeeded,
            StatusCode = result.StatusCode,
            Error = result.Error,
            Result = result.Result,
        };
    }

    /// <summary>
    /// Retrieves all clients formatted as list items suitable for display.
    /// </summary>
    /// <returns>A list of <see cref="ClientListItem"/> objects.</returns>
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

    /// <summary>
    /// Retrieves client details formatted for an edit form.
    /// </summary>
    /// <param name="id">The unique identifier of the client to edit.</param>
    /// <returns>An <see cref="EditClientForm"/> populated with client data, or null if not found or on error.</returns>
    public async Task<EditClientForm?> GetClientForEditAsync(string id)
    {
        try
        {
            var result = await _clientRepository.GetByIdAsync(id);

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

    /// <summary>
    /// Adds a new client based on the submitted form data.
    /// </summary>
    /// <param name="form">The form containing the new client information.</param>
    /// <returns>True if the client was added successfully, false otherwise.</returns>
    public async Task<bool> AddClientAsync(AddClientForm form)
    {
        if (form == null)
            return false;

        var entity = _clientFactory.CreateClientEntity(form);
        var result = await _clientRepository.AddAsync(entity);

        return result.Succeeded;
    }

    /// <summary>
    /// Updates an existing client's details based on the submitted form data.
    /// </summary>
    /// <param name="form">The form containing the updated client information.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
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

    /// <summary>
    /// Deletes a client, but only if they have no associated projects.
    /// </summary>
    /// <param name="id">The unique identifier of the client to delete.</param>
    /// <returns>True if the deletion was successful, false if the client has projects or an error occurred.</returns>
    public async Task<bool> DeleteClientAsync(string id)
    {
        try
        {
            // Check if any projects are associated with this client
            var projectsExistResult = await _projectRepository.ExistsAsync(p => p.ClientId == id);

            if (projectsExistResult.Succeeded && projectsExistResult.Result)
            {
                // Found associated projects, prevent deletion
                Console.WriteLine($"Attempted to delete client {id} with existing projects."); // Optional logging
                return false; // Indicate failure because projects exist
            }
            else if (!projectsExistResult.Succeeded)
            {
                // Handle error during project check
                Console.WriteLine(
                    $"Error checking projects for client {id}: {projectsExistResult.Error}"
                ); // Optional logging
                return false; // Indicate failure due to error
            }

            // No associated projects found (or check failed but we proceed cautiously), attempt deletion
            var deleteResult = await _clientRepository.DeleteAsync(id);
            return deleteResult.Succeeded;
        }
        catch (Exception ex) // Catch potential exceptions during the process
        {
            Console.WriteLine($"Error deleting client {id}: {ex.Message}"); // Log error
            return false;
        }
    }
}
