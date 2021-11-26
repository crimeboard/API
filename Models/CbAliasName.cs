using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbAliasName
    {
        public int Id { get; set; }
        public int IndividualId { get; set; }
        public string Name { get; set; }
        public bool? IsComic { get; set; }
        public int Ranking { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? LastUpdated { get; set; }

        public virtual CbIndividual IdNavigation { get; set; }
    }
}
