using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Role> _roleRepository;

        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound("Сотрудник не найден");

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать сотрудника
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] EmployeeCreateRequest request) 
        {
            if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) || string.IsNullOrEmpty(request.Email))
                return BadRequest("Имя, фамилия и почта - обязательные поля");

            var roles = await Task.WhenAll(request.RoleIds.Select(_roleRepository.GetByIdAsync));

            Employee employee = new Employee() 
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Roles = roles.ToList()
            };

            await _employeeRepository.AddAsync(employee);

            return new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                FullName = employee.FullName,
                Roles = employee.Roles.Select(r => new RoleItemResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList()
            };
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id) 
        {
            Employee? employee = await _employeeRepository.GetByIdAsync(id);
            if (employee is null)
                return NotFound("Сотрудник не найден");

            if (!await _employeeRepository.DeleteAsync(id))
                return StatusCode(500);

            return NoContent();
        }

        /// <summary>
        /// Обновить сотрудника
        /// </summary>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult> UpdateEmployeeAsync([FromBody] EmployeeUpdateRequest updateRequest)
        {
            Employee? employee = await _employeeRepository.GetByIdAsync(updateRequest.Id);
            if (employee is null)
                return NotFound("Сотрудник не найден");

            var roles = await Task.WhenAll(updateRequest.RoleIds.Select(_roleRepository.GetByIdAsync)
                                                                .Where(r => r != null).ToList());

            employee.FirstName = updateRequest.FirstName;
            employee.LastName = updateRequest.LastName;
            employee.Email = updateRequest.Email;
            employee.AppliedPromocodesCount = updateRequest.AppliedPromocodesCount;

            bool updated = await _employeeRepository.UpdateAsync(employee);
            if (!updated)
                return StatusCode(500, "Ошибка при обновлении сотрудника");

            return NoContent();
        }
    }
}