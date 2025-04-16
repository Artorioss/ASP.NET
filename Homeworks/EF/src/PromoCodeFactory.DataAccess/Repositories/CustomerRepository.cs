using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class CustomerRepository : EFRepository<Customer>
    {
        public CustomerRepository(Context context) : base(context)
        {
        }

        public async Task<List<Customer>> GetAllCustomersAsync(bool asNoTracking = false) 
        {
            IQueryable<Customer> query = Context.Customers.Include(c => c.PromoCodes).Include(c => c.Preferences);
            checkAsNoTracking(ref query, asNoTracking);
            return await query.ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid id, bool asNoTracking = false) 
        {
            IQueryable<Customer> query = Context.Customers.Include(c => c.PromoCodes).Include(c => c.Preferences);
            checkAsNoTracking(ref query, asNoTracking);
            return await query.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCustomerAsync(Customer customer) 
        {
            await AddAsync(customer);
            await SaveChangesAsync();
        }
    }
}
