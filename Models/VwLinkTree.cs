using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace BackendAPI.Models
{
    [DataContract]
    public partial class VwLinkTree
    {
        [DataMember]
        public int? Source { get; set; }
        [DataMember]
        public int? Target { get; set; }
        [DataMember]
        public int? Type { get; set; }
    }
}
