using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Contexts;
using Data.Interfaces;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query; // Add for IIncludableQueryable

namespace Data.Repositories;

/// <summary>
/// Generic base repository providing common CRUD operations for entities.
/// Uses Entity Framework Core for data access.
/// </summary>
/// <typeparam name="TEntity">The type of the entity this repository handles.</typeparam>
public class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>A <see cref="BaseResult{TEntity}"/> containing the entity or an error if not found.</returns>
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

    /// <summary>
    /// Retrieves all entities, optionally including related data.
    /// </summary>
    /// <param name="include">A function to specify related data to include in the query (e.g., `q => q.Include(p => p.Status)`).</param>
    /// <returns>A <see cref="BaseResult{T}"/> containing an enumerable of entities or an error.</returns>
    public async Task<BaseResult<IEnumerable<TEntity>>> GetAllAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null
    )
    {
        try
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
            {
                query = include(query); // Apply includes if provided
            }

            var entities = await query.ToListAsync();
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

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A <see cref="BaseResult{TEntity}"/> containing the added entity (with potential DB-generated values) or an error.</returns>
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

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    /// <returns>A <see cref="BaseResult{TEntity}"/> containing the updated entity or an error.</returns>
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

    /// <summary>
    /// Deletes an entity from the database by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns>A <see cref="BaseResult{T}"/> indicating success (true) or failure (false).</returns>
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

    /// <summary>
    /// Checks if any entity exists that matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>A <see cref="BaseResult{T}"/> indicating whether a matching entity exists (true) or not (false).</returns>
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

    /// <summary>
    /// Finds the first entity that matches the specified predicate, or null if none is found.
    /// </summary>
    /// <param name="predicate">The condition to search for.</param>
    /// <returns>A <see cref="BaseResult{T}"/> containing the found entity (or null) or an error.</returns>
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
