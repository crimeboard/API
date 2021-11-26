using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbAudit
    {
        public int UserId { get; set; }
        public string TableName { get; set; }
        public int Id { get; set; }
        public string FieldName { get; set; }
        public DateTime DateChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
