using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbTrumpAppointment
    {
        public string Position { get; set; }
        public string Name { get; set; }
        public int? IndividualId { get; set; }
        public int YearOfTerm { get; set; }
        public string PriorJob { get; set; }
        public string Reason { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string WhereTo { get; set; }
        public string Successor { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }

        public virtual CbIndividual Individual { get; set; }
    }
}
