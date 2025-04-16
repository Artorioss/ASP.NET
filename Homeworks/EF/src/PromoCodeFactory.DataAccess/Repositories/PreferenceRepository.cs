using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class PreferenceRepository : EFRepository<Preference>
    {
        public PreferenceRepository(Context context) : base(context)
        {
        }

        public async Task<List<Preference>> GetPreferencesAsync(List<Guid> idRange, bool asNoTracking = false) 
        {
            IQueryable<Preference> query = Context.Preferences.Where(p => Enumerable.Contains(idRange, p.Id));
            checkAsNoTracking(ref query, asNoTracking);
            return await query.ToListAsync();
        }

        public async Task<List<Preference>> GetPreferencesAsync(bool asNoTracking = false) 
        {
            IQueryable<Preference> query = Context.Preferences;
            checkAsNoTracking(ref query, asNoTracking);
            return await query.ToListAsync();
        }
    }
}
