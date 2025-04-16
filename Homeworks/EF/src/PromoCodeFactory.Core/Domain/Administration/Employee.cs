using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}