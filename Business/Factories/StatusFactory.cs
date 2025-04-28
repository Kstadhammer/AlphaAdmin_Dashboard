using System;
using Business.Interfaces;
using Business.Models;
using Data.Entities;

namespace Business.Factories;

/// <summary>
/// Factory responsible for creating Status-related objects (Entities and Models).
/// </summary>
public class StatusFactory : IStatusFactory
{
    /// <summary>
    /// Creates a <see cref="StatusEntity"/> from a <see cref="Status"/> model.
    /// </summary>
    /// <param name="model">The status business model.</param>
    /// <returns>A new <see cref="StatusEntity"/> instance populated from the model.</returns>
    public StatusEntity CreateStatusEntity(Status model)
    {
        return new StatusEntity
        {
            Name = model.Name,
            Color = model.Color,
            Order = model.Order,
            IsDefault = model.IsDefault,
            CreatedAt = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Creates a <see cref="Status"/> business model from a <see cref="StatusEntity"/>.
    /// </summary>
    /// <param name="entity">The status data entity.</param>
    /// <returns>A new <see cref="Status"/> instance populated from the entity.</returns>
    public Status CreateStatusModel(StatusEntity entity)
    {
        return new Status
        {
            Id = entity.Id,
            Name = entity.Name,
            Color = entity.Color,
            Order = entity.Order,
            IsDefault = entity.IsDefault,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}
