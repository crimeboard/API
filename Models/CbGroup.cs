using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace BackendAPI.Models
{
    [DataContract]
    public partial class CbGroup
    {
        public CbGroup()
        {
            CbGroupXImages = new HashSet<CbGroupXImage>();
            CbGroupXRelationships = new HashSet<CbGroupXRelationship>();
            CbGroupXTransactions = new HashSet<CbGroupXTransaction>();
            InverseParent = new HashSet<CbGroup>();
        }

        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? ParentId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string WikipediaUrl { get; set; }
        [DataMember]
        public int? ImageId { get; set; }
        [DataMember]
        public bool? Active { get; set; }
        [DataMember]
        public int Creator { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public int? UpdatedBy { get; set; }
        [DataMember]
        public DateTime? LastUpdated { get; set; }

        public virtual CbGroup Parent { get; set; }
        public virtual ICollection<CbGroupXImage> CbGroupXImages { get; set; }
        public virtual ICollection<CbGroupXRelationship> CbGroupXRelationships { get; set; }
        public virtual ICollection<CbGroupXTransaction> CbGroupXTransactions { get; set; }
        public virtual ICollection<CbGroup> InverseParent { get; set; }
    }
}
