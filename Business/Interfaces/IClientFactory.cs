using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IClientFactory
{
    ClientEntity CreateClientEntity(AddClientForm form, string? imageUrl = null);
    ClientEntity UpdateClientEntity(
        ClientEntity entity,
        EditClientForm form,
        string? imageUrl = null
    );
    ClientListItem CreateClientListItem(ClientEntity entity);
    EditClientForm CreateEditClientForm(ClientEntity entity);
}
