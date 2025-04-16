using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public abstract class EFRepository<T>: IRepository<T> where T : BaseEntity
    {
        protected readonly Context Context;
        private readonly DbSet<T> _data;

        protected EFRepository(Context context)
        {
            Context = context;
            _data = Context.Set<T>();
            
        }

        public Task<List<T>> GetAllAsync(bool asNoTracking = false)
        {
            return asNoTracking ? _data.AsNoTracking().ToListAsync() : _data.ToListAsync();
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return _data.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            T entity = await _data.FindAsync(id);
            bool result = false;
            if (entity != null) 
            {
                _data.Remove(entity);
                result = true;
            }
            return result;
        }

        public bool Delete(T entity)
        {
            bool result = false;
            if(entity != null) 
            {
                Context.Entry(entity).State = EntityState.Modified;
                result = true;
            }
            return result;
        }

        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<T> AddAsync(T entity)
        {
            return (await _data.AddAsync(entity)).Entity;
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        protected void checkAsNoTracking(ref IQueryable<T> query, bool asNoTracking)
        {
            if (asNoTracking)
                query = query.AsNoTracking();
        }
    }
}