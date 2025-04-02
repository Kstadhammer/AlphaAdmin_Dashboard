using System;
using Business.Interfaces;
using Business.Models;
using Data.Entities;

namespace Business.Factories;

public class StatusFactory : IStatusFactory
{
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
