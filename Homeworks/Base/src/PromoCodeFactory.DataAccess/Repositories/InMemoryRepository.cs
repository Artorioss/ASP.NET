using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> AddAsync(T entity)
        {
            Data.Append(entity);
            return Task.FromResult(entity);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            if(GetByIdAsync(id) is null)
                return Task.FromResult(false);

            Data = Data.Where(en => en.Id != id);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateAsync(T entity)
        {
            Data = Data.Where(en => en.Id != entity.Id);
            Data = Data.Append(entity);
            return Task.FromResult(true);
        }
    }
}