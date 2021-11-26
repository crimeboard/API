using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace BackendAPI.Models
{
    [DataContract]
    public partial class VwGroupTree
    {
        [DataMember]
        public int? Id { get; set; }
        [DataMember]
        public int? ParentId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? IsGroup { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string WikipediaUrl { get; set; }
        [DataMember]
        public int? Level { get; set; }
    }
}
