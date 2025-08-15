using Microsoft.AspNetCore.Mvc;
using Pcf.Preferences.Core.Abstractions.Repositories;
using Pcf.Preferences.Core.Domain;
using Pcf.Preferences.WebHost.Models;

namespace Pcf.Preferences.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController: ControllerBase
    {
        private readonly IRepository<Preference> _preferencesRepository;

        public PreferencesController(IRepository<Preference> preferencesRepository)
        {
            _preferencesRepository = preferencesRepository;
        }

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            var preferences = await _preferencesRepository.GetAllAsync();

            var response = preferences.Select(x => new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Ok(response);
        }

        /// <summary>Получить конкретный список предпочтений</summary>
        [HttpGet("range")]
        public async Task<ActionResult<List<PreferenceResponse>>> GetRangeByIdsAsync([FromBody] List<Guid> guids)
        {
            if (guids is null || guids.Count == 0)
                return BadRequest("Передайте хотя бы один guid.");

            var preferences = await _preferencesRepository.GetRangeByIdsAsync(guids.ToList());

            var response = preferences.Select(x => new PreferenceResponse
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получить предпочтение по id
        /// </summary>
        /// <param name="id">Id предпочтения, например <example>a6c8c6b1-4349-45b0-ab31-244740aaf0f0</example></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PreferenceResponse>> GetPreferenceAsync(Guid id)
        {
            var preference = await _preferencesRepository.GetByIdAsync(id);

            var response = new PreferenceResponse() 
            {
                Id = preference.Id,
                Name = preference.Name
            };

            return Ok(response);
        }
    }
}
