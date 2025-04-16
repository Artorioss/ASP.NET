using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController: ControllerBase
    {
        private readonly CustomerRepository _customerRepository;
        private readonly PreferenceRepository _preferenceRepository;
        public CustomersController(CustomerRepository customerRepository, PreferenceRepository preferenceRepository) 
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Получить данные всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync(true);
            var responseList = customers.Select(c => new CustomerResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PromoCodes = new List<PromoCodeShortResponse>(c.PromoCodes.Select(prom => new PromoCodeShortResponse 
                {
                    Id = prom.Id,
                    BeginDate = prom.BeginDate.ToString(),
                    EndDate = prom.EndDate.ToString(),
                    Code = prom.Code,
                    PartnerName = prom.PartnerName,
                    ServiceInfo = prom.ServiceInfo,
                }))
            });
            return Ok(customers);
        }

        /// <summary>
        /// Получить данные клиента по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            Customer customer = await _customerRepository.GetCustomerByIdAsync(id, true);

            if (customer is null)
                return NotFound();
            CustomerResponse customerModel = new CustomerResponse();
            try
            {
                customerModel = new CustomerResponse()
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    PromoCodes = new List<PromoCodeShortResponse>(customer.PromoCodes.Select(prom => new PromoCodeShortResponse
                    {
                        Id = prom.Id,
                        Code = prom.Code,
                        BeginDate = prom.BeginDate.ToString(),
                        EndDate = prom.EndDate.ToString(),
                        PartnerName = prom.PartnerName,
                        ServiceInfo = prom.ServiceInfo,
                    })),
                    Preferences = new List<PreferenceItemResponse>(customer.Preferences.Select(pref => new PreferenceItemResponse()
                    {
                        Id = pref.Id,
                        Name = pref.Name
                    }))
                };
            }
            catch (Exception ex) 
            {
                
            }

            return customerModel;
        }

        /// <summary>
        /// Создать клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            Customer customer = new Customer() 
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Preferences = new List<Preference>(await _preferenceRepository.GetPreferencesAsync(request.PreferenceIds))
            };

            await _customerRepository.AddCustomerAsync(customer);
            return Created();
        }

        /// <summary>
        /// Редактировать данные клиента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            Customer customer = await _customerRepository.GetCustomerByIdAsync(id);

            if (customer is null)
                return NotFound();

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.Preferences = new List<Preference>(await _preferenceRepository.GetPreferencesAsync(customer.Preferences.Select(p => p.Id)
                                                                                                                            .ToList()));
            await _preferenceRepository.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            await _customerRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}