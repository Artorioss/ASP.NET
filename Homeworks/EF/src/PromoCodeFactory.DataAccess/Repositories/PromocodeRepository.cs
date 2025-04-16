using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    internal class PromocodeRepository : EFRepository<PromoCode>
    {
        public PromocodeRepository(Context context) : base(context)
        {
        }

        public async Task<List<PromoCode>> GetPromoCodes(bool asNoTracking = false) 
        {
            IQueryable<PromoCode> query = Context.PromoCodes.Include(p => p.PartnerManager)
                                                            .Include(p => p.Customer)
                                                            .Include(p => p.Preference);
            checkAsNoTracking(ref query, asNoTracking);
            return await query.ToListAsync();
        }
    }
}
