﻿using PromoCodeFactory.Core.Domain.Administration;
using System;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class PromoCode: BaseEntity
    {
        public string Code { get; set; }
        public string ServiceInfo { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PartnerName { get; set; }
        public int CustomerId { get; set; }
        public Employee PartnerManager { get; set; }
        public Preference Preference { get; set; }
        public Customer Customer { get; set; }
    }
}