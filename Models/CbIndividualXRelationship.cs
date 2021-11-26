using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbIndividualXRelationship
    {
        public int RelationshipId { get; set; }
        public int IndividualId { get; set; }
        public bool Direction { get; set; }

        public virtual CbIndividual Individual { get; set; }
        public virtual CbRelationship Relationship { get; set; }
    }
}
