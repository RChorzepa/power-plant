using Microsoft.EntityFrameworkCore;
using PowerPlant.Core.Entities;
using PowerPlant.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PowerPlant.Infrastructure.Persistence.Repositories
{
    public class RepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : Entity
    {
        protected readonly PowerPlantDbContext DbContext;

        public RepositoryAsync(PowerPlantDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<TEntity> FindAsync(int id)
        {
            return await  DbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async Task<ICollection<TEntity>> GetAsync()
        {
            return await DbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async void AddAsync(TEntity entity)
        {
             DbContext.Set<TEntity>().Add(entity);
            await DbContext.SaveChangesAsync();
        }

        public async void AddRangeAsync(ICollection<TEntity> entities)
        {
        try
            {
                 DbContext.Set<TEntity>().AddRange(entities);
                await DbContext.SaveChangesAsync();
            } 
            catch(Exception exception)
            {
                throw;
            }
        }

    }
}
