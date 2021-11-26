using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbResource
    {
        public int Id { get; set; }
        public int? TransactionId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int? Importance { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }

        public virtual CbTransaction Transaction { get; set; }
    }
}
