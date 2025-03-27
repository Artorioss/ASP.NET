using System.Collections.Generic;
using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class EmployeeUpdateRequest
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Guid> RoleIds { get; set; }
        public int AppliedPromocodesCount { get; set; }
    }
}
