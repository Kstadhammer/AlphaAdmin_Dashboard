using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Data.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : class
{
    Task<BaseResult<IEnumerable<TEntity>>> GetAllAsync();
    Task<BaseResult<TEntity>> GetByIdAsync(string id);
    Task<BaseResult<TEntity>> AddAsync(TEntity entity);
    Task<BaseResult<TEntity>> UpdateAsync(TEntity entity);
    Task<BaseResult<bool>> DeleteAsync(string id);
    Task<BaseResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}
