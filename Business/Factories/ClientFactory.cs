using System;
using System.Collections.Generic; // Add for List
using System.Linq; // Add for Count()
using Business.Forms;
using Business.Interfaces;
using Business.Models;
using Data.Entities;

namespace Business.Factories
{
    public class ClientFactory : IClientFactory
    {
        // List of available default avatar paths (relative to wwwroot)
        private static readonly List<string> _defaultAvatarPaths = new List<string>
        {
            "/images/avatars/avatars1.png",
            "/images/avatars/avatars2.png",
            "/images/avatars/avatars3.png",
            "/images/avatars/avatars4.png",
            "/images/avatars/avatars5.png",
        };
        private static readonly Random _random = new Random();

        public ClientEntity CreateClientEntity(AddClientForm form, string? imageUrl = null)
        {
            string finalImageUrl = imageUrl;

            // If no specific image was uploaded, assign a random default avatar
            if (string.IsNullOrEmpty(finalImageUrl) && _defaultAvatarPaths.Any())
            {
                int randomIndex = _random.Next(_defaultAvatarPaths.Count);
                finalImageUrl = _defaultAvatarPaths[randomIndex];
            }

            return new ClientEntity
            {
                ClientName = form.ClientName,
                Email = form.Email,
                Location = form.Location,
                Phone = form.Phone,
                ImageUrl = finalImageUrl, // Use the determined image URL
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
