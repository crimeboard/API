using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class Template
    {
        public Template()
        {
            Folders = new HashSet<Folder>();
        }

        public int SiteId { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string Type { get; set; }
        public string Layout { get; set; }
        public int? Creator { get; set; }
        public DateTime? Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<Folder> Folders { get; set; }
    }
}
