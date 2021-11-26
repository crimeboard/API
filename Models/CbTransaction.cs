using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace BackendAPI.Models
{
    [DataContract]
    public partial class CbTransaction
    {
        public CbTransaction()
        {
            CbGroupXTransactions = new HashSet<CbGroupXTransaction>();
            CbIndividualXTransactions = new HashSet<CbIndividualXTransaction>();
            CbResources = new HashSet<CbResource>();
            CbTransactionXImages = new HashSet<CbTransactionXImage>();
        }

        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int? Importance { get; set; }
        [DataMember]
        public decimal? MonetaryAmount { get; set; }
        [DataMember]
        public decimal? MinFine { get; set; }
        [DataMember]
        public decimal? MaxFine { get; set; }
        [DataMember]
        public int? MinSentenceYears { get; set; }
        [DataMember]
        public int? MaxSentenceYears { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public int? JuristictionId { get; set; }
        [DataMember]
        public string TwitterPostUrl { get; set; }
        [DataMember]
        public string YouTubeUrl { get; set; }
        [DataMember]
        public string NewspaperArticleUrl { get; set; }
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

        public virtual ICollection<CbGroupXTransaction> CbGroupXTransactions { get; set; }
        public virtual ICollection<CbIndividualXTransaction> CbIndividualXTransactions { get; set; }
        public virtual ICollection<CbResource> CbResources { get; set; }
        public virtual ICollection<CbTransactionXImage> CbTransactionXImages { get; set; }
    }
}
