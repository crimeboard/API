using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace BackendAPI.Models
{
    [DataContract]
    public partial class CbIndividual
    {
        public CbIndividual()
        {
            CbIndividualXRelationships = new HashSet<CbIndividualXRelationship>();
            CbIndividualXTransactions = new HashSet<CbIndividualXTransaction>();
            CbTrumpAppointments = new HashSet<CbTrumpAppointment>();
        }

        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public string Firstname { get; set; }
        [DataMember]
        public string CommonName { get; set; }
        [DataMember]
        public string CountryOfBirth { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int? Importance { get; set; }
        [DataMember]
        public string JobTitle { get; set; }
        [DataMember]
        public string WikipediaUrl { get; set; }
        [DataMember]
        public string TwitterHandle { get; set; }
        [DataMember]
        public int? ImageId { get; set; }
        [DataMember]
        public int Creator { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public int? UpdatedBy { get; set; }
        [DataMember]
        public DateTime? LastUpdated { get; set; }

        public virtual CbAliasName CbAliasName { get; set; }
        public virtual ICollection<CbIndividualXRelationship> CbIndividualXRelationships { get; set; }
        public virtual ICollection<CbIndividualXTransaction> CbIndividualXTransactions { get; set; }
        public virtual ICollection<CbTrumpAppointment> CbTrumpAppointments { get; set; }
    }
}
