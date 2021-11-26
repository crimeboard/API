using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbViewAuditUser
    {
        public int UserId { get; set; }
        public string TableName { get; set; }
        public int Id { get; set; }
        public string FieldName { get; set; }
        public DateTime DateChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TwitterHandle { get; set; }
        public string GoogleHandle { get; set; }
        public bool AutoApprove { get; set; }
        public bool? Active { get; set; }
    }
}
