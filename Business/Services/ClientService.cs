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
    private readonly IClientFactory _clientFactory;

    public ClientService(IClientRepository clientRepository, IClientFactory clientFactory)
    {
        _clientRepository = clientRepository;
        _clientFactory = clientFactory;
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
        // Note: This is a stub implementation since your actual repository uses string IDs
        // You may need to adapt this to your specific ID handling
        var result = await _clientRepository.GetByIdAsync(id.ToString());
        if (!result.Succeeded)
        {
            return null;
        }

        return _clientFactory.CreateEditClientForm(result.Result);
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
        // Note: This is a stub implementation since your actual repository uses string IDs
        var result = await _clientRepository.DeleteAsync(id.ToString());
        return result.Succeeded;
    }
}
