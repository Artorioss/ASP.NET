using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Integration.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration
{
    public class PreferenceGateway : IPreferenceGateway
    {
        private readonly HttpClient _httpClient;

        public PreferenceGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Preference>> GetRangeByIdsAsync(List<Guid> guids)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/v1/Preferences/GetRangeByIdsAsync",
                guids
            );
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<PreferenceDto>>();
            IEnumerable<Preference> preferences = null;

            if (result != null)
            {
                preferences = result.Select(dto => new Preference()
                {
                    Id = dto.Id,
                    Name = dto.Name
                });
            }

            return preferences ?? Enumerable.Empty<Preference>();
        }

        public async Task<IEnumerable<Preference>> GetPreferencesAsync()
        {
            var response = await _httpClient.GetAsync($"api/v1/Preferences/GetPreferences");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Не удалось получить промокоды");


            var items = await response.Content.ReadFromJsonAsync<IEnumerable<PreferenceDto>>();
            IEnumerable<Preference> preferences = null;
            
            if (items != null) 
            {
                preferences = items.Select(dto => new Preference() 
                {
                    Id = dto.Id,
                    Name = dto.Name
                });
            }

            return preferences ?? Enumerable.Empty<Preference>();
        }

        public async Task<Preference?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/v1/Preferences/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<PreferenceDto>();
            if (dto is null) return null;

            return new Preference
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }
    }
}
