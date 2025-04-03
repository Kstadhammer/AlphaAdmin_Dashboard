using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore.Query; // Add for IIncludableQueryable

namespace Data.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : class
{
    Task<BaseResult<IEnumerable<TEntity>>> GetAllAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null
    );
    Task<BaseResult<TEntity>> GetByIdAsync(string id);
    Task<BaseResult<TEntity>> AddAsync(TEntity entity);
    Task<BaseResult<TEntity>> UpdateAsync(TEntity entity);
    Task<BaseResult<bool>> DeleteAsync(string id);
    Task<BaseResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<BaseResult<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> predicate); // Find first matching entity or null
}
