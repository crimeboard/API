using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbGroupXTransaction
    {
        public int TransactionId { get; set; }
        public int GroupId { get; set; }
        public bool Direction { get; set; }

        public virtual CbGroup Group { get; set; }
        public virtual CbTransaction Transaction { get; set; }
    }
}
