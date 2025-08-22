using MassTransit;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using SharedModels;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGateway: IGivingPromoCodeToCustomerGateway
    {
        //private readonly HttpClient _httpClient;
        IPublishEndpoint _publisher;

        public GivingPromoCodeToCustomerGateway(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var dto = new GivePromoCodeToCustomerCommand()
            {
                PartnerId = promoCode.Partner.Id,
                BeginDate = promoCode.BeginDate.ToShortDateString(),
                EndDate = promoCode.EndDate.ToShortDateString(),
                PreferenceId = promoCode.PreferenceId,
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId
            };

            await _publisher.Publish(dto);

            //var response = await _httpClient.PostAsJsonAsync("api/v1/promocodes", dto);

            //response.EnsureSuccessStatusCode();
        }
    }
}