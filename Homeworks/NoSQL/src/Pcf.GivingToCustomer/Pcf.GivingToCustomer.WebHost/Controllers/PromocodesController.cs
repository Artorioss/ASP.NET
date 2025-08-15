using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Customer> _customersRepository;
        private readonly IPreferenceGateway _preferenceGateway;

        public PromocodesController(IRepository<PromoCode> promoCodesRepository, IRepository<Customer> customersRepository,
            IPreferenceGateway preferenceGateway)
        {
            _preferenceGateway = preferenceGateway;
            _promoCodesRepository = promoCodesRepository;
            _customersRepository = customersRepository;
        }
        
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promocodes = await _promoCodesRepository.GetAllAsync();

            var response = promocodes.Select(x => new PromoCodeShortResponse()
            {
                Id = x.Id,
                Code = x.Code,
                BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                PartnerId = x.PartnerId,
                ServiceInfo = x.ServiceInfo
            }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync([FromBody] GivePromoCodeRequest request, CancellationToken ct)
        {
            if (request == null || request.PreferenceId == Guid.Empty)
                return BadRequest("PreferenceId is required.");

            var preference = await _preferenceGateway.GetByIdAsync(request.PreferenceId);
            if (preference is null)
                return NotFound($"Preference '{request.PreferenceId}' not found.");

            var customers = await _customersRepository.GetWhere(
                c => c.CustomerPreferences.Any(cp => cp.PreferenceId == request.PreferenceId));

            if (customers == null || customers.Count() == 0)
                return NotFound("No customers found with this preference.");

            var promoCode = PromoCodeMapper.MapFromModel(request, preference.Id, customers);

            await _promoCodesRepository.AddAsync(promoCode);

            return CreatedAtAction(nameof(GetPromocodesAsync), new { id = promoCode.Id }, null);
        }
    }
}