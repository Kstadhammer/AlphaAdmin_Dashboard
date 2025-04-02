using System;
using Business.Interfaces;
using Business.Models;
using Data.Entities;

namespace Business.Factories
{
    public class ClientFactory : IClientFactory
    {
        public ClientEntity CreateClientEntity(AddClientForm form, string? imageUrl = null)
        {
            return new ClientEntity
            {
                ClientName = form.ClientName,
                Email = form.Email,
                Location = form.Location,
                Phone = form.Phone,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
        }

        public ClientEntity UpdateClientEntity(
            ClientEntity entity,
            EditClientForm form,
            string? imageUrl = null
        )
        {
            entity.ClientName = form.ClientName;
            entity.Email = form.Email;
            entity.Location = form.Location;
            entity.Phone = form.Phone;
            entity.UpdatedAt = DateTime.UtcNow;

            if (imageUrl != null)
            {
                entity.ImageUrl = imageUrl;
            }

            return entity;
        }

        public ClientListItem CreateClientListItem(ClientEntity entity)
        {
            return new ClientListItem
            {
                Id = entity.Id,
                ClientName = entity.ClientName,
                Email = entity.Email,
                Location = entity.Location,
                Phone = entity.Phone,
                ImageUrl = entity.ImageUrl,
                IsActive = entity.IsActive,
            };
        }

        public EditClientForm CreateEditClientForm(ClientEntity entity)
        {
            return new EditClientForm
            {
                Id = entity.Id,
                ClientName = entity.ClientName,
                Email = entity.Email,
                Location = entity.Location,
                Phone = entity.Phone,
            };
        }
    }
}
