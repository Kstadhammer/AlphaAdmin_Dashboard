using Business.Forms;
using Business.Models;

namespace Business.Interfaces;

public interface IClientService
{
    Task<List<ClientListItem>> GetAllClientsAsync();
    Task<EditClientForm> GetClientForEditAsync(int id);
    Task<bool> AddClientAsync(AddClientForm form);
    Task<bool> EditClientAsync(EditClientForm form);
    Task<bool> DeleteClientAsync(int id);
}
