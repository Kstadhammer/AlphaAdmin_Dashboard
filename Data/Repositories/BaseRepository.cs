using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Contexts;
using Data.Interfaces;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<BaseResult<TEntity>> GetByIdAsync(string id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
                return new BaseResult<TEntity>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Entity not found",
                };

            return new BaseResult<TEntity> { Succeeded = true, Result = entity };
        }
        catch (Exception ex)
        {
            return new BaseResult<TEntity>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }

    public async Task<BaseResult<IEnumerable<TEntity>>> GetAllAsync()
    {
        try
        {
            var entities = await _dbSet.ToListAsync();

            return new BaseResult<IEnumerable<TEntity>> { Succeeded = true, Result = entities };
        }
        catch (Exception ex)
        {
            return new BaseResult<IEnumerable<TEntity>>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }

    public async Task<BaseResult<TEntity>> AddAsync(TEntity entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new BaseResult<TEntity>
            {
                Succeeded = true,
                StatusCode = 201,
                Result = entity,
            };
        }
        catch (Exception ex)
        {
            return new BaseResult<TEntity>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }

    public async Task<BaseResult<TEntity>> UpdateAsync(TEntity entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return new BaseResult<TEntity> { Succeeded = true, Result = entity };
        }
        catch (Exception ex)
        {
            return new BaseResult<TEntity>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }

    public async Task<BaseResult<bool>> DeleteAsync(string id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
                return new BaseResult<bool>
                {
                    Succeeded = false,
                    StatusCode = 404,
                    Error = "Entity not found",
                    Result = false,
                };

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return new BaseResult<bool> { Succeeded = true, Result = true };
        }
        catch (Exception ex)
        {
            return new BaseResult<bool>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
                Result = false,
            };
        }
    }

    public async Task<BaseResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var exists = await _dbSet.AnyAsync(predicate);

            return new BaseResult<bool> { Succeeded = true, Result = exists };
        }
        catch (Exception ex)
        {
            return new BaseResult<bool>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
                Result = false,
            };
        }
    }

    public async Task<BaseResult<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var entity = await _dbSet.FirstOrDefaultAsync(predicate);
            // Note: entity can be null here if not found, which is expected.
            return new BaseResult<TEntity?> { Succeeded = true, Result = entity };
        }
        catch (Exception ex)
        {
            return new BaseResult<TEntity?>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message,
            };
        }
    }
}
