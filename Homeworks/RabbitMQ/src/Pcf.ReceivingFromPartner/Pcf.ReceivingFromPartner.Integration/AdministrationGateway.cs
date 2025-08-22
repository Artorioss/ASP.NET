using System;
using System.Net.Http;
using System.Threading.Tasks;
using MassTransit;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using SharedModels;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class AdministrationGateway
        : IAdministrationGateway
    {
        //private readonly HttpClient _httpClient;
        private readonly IPublishEndpoint _publisher;

        public AdministrationGateway(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        {
            await _publisher.Publish(new PartnerManagerPromoCodeApplied()
            {
                PartnerManagerId = partnerManagerId
            });
        }

        //Синхронный вариант

        //public async Task NotifyAdminAboutPartnerManagerPromoCode(Guid partnerManagerId)
        //{
        //    var response = await _httpClient.PostAsync($"api/v1/employees/{partnerManagerId}/appliedPromocodes",
        //        new StringContent(string.Empty));

        //    response.EnsureSuccessStatusCode();
        //}
    }
}