using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}