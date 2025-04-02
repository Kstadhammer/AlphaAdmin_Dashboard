using Business.Models;
using Data.Entities;

namespace Business.Interfaces;

public interface IStatusFactory
{
    StatusEntity CreateStatusEntity(Status status);
    Status CreateStatusModel(StatusEntity entity);
}
