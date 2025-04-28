using System;
using System.Collections.Generic; // Add for List
using System.Linq; // Add for Count()
using Business.Forms;
using Business.Interfaces;
using Business.Models;
using Data.Entities;

namespace Business.Factories
{
    /// <summary>
    /// Factory responsible for creating and mapping Client-related objects
    /// (Entities, Models, Forms, ListItems) between different layers.
    /// Handles assignment of random default avatars for new clients if no image is provided.
    /// </summary>
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

        /// <summary>
        /// Creates a new <see cref="ClientEntity"/> from an <see cref="AddClientForm"/>.
        /// Assigns a random default avatar if no image URL is provided.
        /// </summary>
        /// <param name="form">The form data for the new client.</param>
        /// <param name="imageUrl">Optional URL of an uploaded image for the client.</param>
        /// <returns>A new <see cref="ClientEntity"/> instance.</returns>
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

        /// <summary>
        /// Updates an existing <see cref="ClientEntity"/> with data from an <see cref="EditClientForm"/>.
        /// </summary>
        /// <param name="entity">The existing client entity to update.</param>
        /// <param name="form">The form data containing updates.</param>
        /// <param name="imageUrl">Optional new image URL if the avatar was updated.</param>
        /// <returns>The updated <see cref="ClientEntity"/> instance.</returns>
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

        /// <summary>
        /// Creates a <see cref="ClientListItem"/> suitable for display from a <see cref="ClientEntity"/>.
        /// </summary>
        /// <param name="entity">The client data entity.</param>
        /// <returns>A new <see cref="ClientListItem"/> instance.</returns>
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

        /// <summary>
        /// Creates an <see cref="EditClientForm"/> from a <see cref="ClientEntity"/> for editing purposes.
        /// </summary>
        /// <param name="entity">The client data entity.</param>
        /// <returns>A new <see cref="EditClientForm"/> instance.</returns>
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
