using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbIndividualXTransaction
    {
        public int TransactionId { get; set; }
        public int IndividualId { get; set; }
        public bool Direction { get; set; }

        public virtual CbIndividual Individual { get; set; }
        public virtual CbTransaction Transaction { get; set; }
    }
}
