﻿using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Models
{
    public class LeadUserSecurityBo
    {
        public Guid Key { get; set; }
        public LeadUserModel? User { get; set; }
        public string? Reason { get; set; }
    }
}
