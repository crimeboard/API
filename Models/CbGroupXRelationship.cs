using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbGroupXRelationship
    {
        public int RelationshipId { get; set; }
        public int GroupId { get; set; }
        public bool Direction { get; set; }

        public virtual CbGroup Group { get; set; }
        public virtual CbRelationship Relationship { get; set; }
    }
}
