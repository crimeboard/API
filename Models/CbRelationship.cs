using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace BackendAPI.Models
{
    [DataContract]
    public partial class CbRelationship
    {
        public CbRelationship()
        {
            CbGroupXRelationships = new HashSet<CbGroupXRelationship>();
            CbIndividualXRelationships = new HashSet<CbIndividualXRelationship>();
        }

        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataMember]
        public int? RelationshipTypeId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime? Started { get; set; }
        [DataMember]
        public DateTime? Ended { get; set; }
        [DataMember]
        public int Creator { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public int? UpdatedBy { get; set; }
        [DataMember]
        public DateTime? LastUpdated { get; set; }

        public virtual ICollection<CbGroupXRelationship> CbGroupXRelationships { get; set; }
        public virtual ICollection<CbIndividualXRelationship> CbIndividualXRelationships { get; set; }
    }
}
