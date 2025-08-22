using MassTransit;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using SharedModels;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PartnerManagerPromoCodeAppliedConsumer: IConsumer<PartnerManagerPromoCodeApplied>
    {
        private readonly IRepository<Employee> _employeeRepository;

        public PartnerManagerPromoCodeAppliedConsumer(IRepository<Employee> employeeRepository) 
        {
            _employeeRepository = employeeRepository;
        } 
        public async Task Consume(ConsumeContext<PartnerManagerPromoCodeApplied> context)
        {
            var id = context.Message.PartnerManagerId;

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new Exception();
            }

            employee.AppliedPromocodesCount++;
            await _employeeRepository.UpdateAsync(employee);
        }
    }
}
