using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppsDBLib.Core.Interfaces;
using AppsDBLib.Infrastructure.Contexts;

namespace AppsDBLib.Infrastructure.Repositories
{
    internal abstract class RepositoryBase<TEntity, TDomainModel, TId> : IAsyncRepository<TDomainModel, TId>
        where TEntity : class
        where TDomainModel : class
    {
        protected readonly AppDbContext DbContext;

        protected RepositoryBase(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<TDomainModel?> GetByIdAsync(TId id)
        {
            var entity = await DbContext.Set<TEntity>().FindAsync(id);
            return entity == null ? null : entity.Adapt<TDomainModel>();
        }

        public virtual async Task<IReadOnlyList<TDomainModel>> GetAllAsync()
        {
            // ProjectToType directly translates the SQL query to the Domain Model for maximum performance
            return await DbContext.Set<TEntity>()
                .ProjectToType<TDomainModel>()
                .ToListAsync();
        }

        public virtual async Task<TDomainModel> AddAsync(TDomainModel domainModel)
        {
            var entity = domainModel.Adapt<TEntity>();
            
            await DbContext.Set<TEntity>().AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity.Adapt<TDomainModel>();
        }

        public virtual async Task UpdateAsync(TDomainModel domainModel)
        {
            var entity = domainModel.Adapt<TEntity>();
            
            DbContext.Entry(entity).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TDomainModel domainModel)
        {
            var entity = domainModel.Adapt<TEntity>();
            
            DbContext.Set<TEntity>().Remove(entity);
            await DbContext.SaveChangesAsync();
        }
    }
}