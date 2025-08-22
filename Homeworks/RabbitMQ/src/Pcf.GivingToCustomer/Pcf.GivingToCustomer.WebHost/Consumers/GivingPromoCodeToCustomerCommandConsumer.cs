using MassTransit;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using SharedModels;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class GivingPromoCodeToCustomerCommandConsumer: IConsumer<GivePromoCodeToCustomerCommand>
    {
        IRepository<Preference> _preferencesRepository;
        IRepository<Customer> _customerRepository;
        IRepository<PromoCode> _promoCodeRepository;

        public GivingPromoCodeToCustomerCommandConsumer(IRepository<Preference> preferencesRepository, IRepository<Customer> customerRepository, IRepository<PromoCode> promoCodeRepository)
        {
            _preferencesRepository = preferencesRepository;
            _customerRepository = customerRepository;
            _promoCodeRepository = promoCodeRepository;
        }

        public async Task Consume(ConsumeContext<GivePromoCodeToCustomerCommand> context) 
        {
            //Получаем предпочтение по имени
            var preference = await _preferencesRepository.GetByIdAsync(context.Message.PreferenceId);

            if (preference == null)
            {
                throw new System.Exception();
            }

            //  Получаем клиентов с этим предпочтением:
            var customers = await _customerRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

            PromoCode promoCode = PromoCodeMapper.MapFromModel(context.Message, preference, customers);

            await _promoCodeRepository.AddAsync(promoCode);
        }
    }
}
