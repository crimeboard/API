using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbUser
    {
        public string TwitterId { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string TwitterHandle { get; set; }
        public string GoogleHandle { get; set; }
        public bool AutoApprove { get; set; }
        public string GuidVerificationCode { get; set; }
        public bool? Active { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
