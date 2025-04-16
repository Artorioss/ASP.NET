using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EmployeeRepository : EFRepository<Employee>
    {
        public EmployeeRepository(Context context) : base(context)
        {
        }
            
        public async Task<List<Employee>> GetEmployeesAsync(bool asNoTracking = false) 
        {
            IQueryable<Employee> query = Context.Employees.Include(e => e.Role);
            checkAsNoTracking(ref query, asNoTracking);
            return await query.ToListAsync();
        }

        public async Task<Employee> GetEmployeeById(Guid id, bool asNoTracking = false) 
        {
            IQueryable<Employee> query = Context.Employees.Include(e => e.Role);
            checkAsNoTracking(ref query, asNoTracking);
            return await query.SingleOrDefaultAsync(e => e.Id == id);
        }
    }
}
