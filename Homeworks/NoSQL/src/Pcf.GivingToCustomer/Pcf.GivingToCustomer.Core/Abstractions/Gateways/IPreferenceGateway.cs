using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways
{
    public interface IPreferenceGateway
    {
        Task<IEnumerable<Preference>> GetRangeByIdsAsync(List<Guid> guids);
        Task<IEnumerable<Preference>> GetPreferencesAsync();
        Task<Preference> GetByIdAsync(Guid id);
    }
}
