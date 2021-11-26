using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class Folder
    {
        public Folder()
        {
            ImageBlobs = new HashSet<ImageBlob>();
            InverseParentFolder = new HashSet<Folder>();
        }

        public int? SiteId { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int? TemplateId { get; set; }
        public int? ParentFolderId { get; set; }
        public int? SortOrder { get; set; }
        public int? ParentFolderPageId { get; set; }
        public int? GroupId { get; set; }
        public int? Creator { get; set; }
        public DateTime? Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsRestricted { get; set; }

        public virtual Folder ParentFolder { get; set; }
        public virtual Template Template { get; set; }
        public virtual ICollection<ImageBlob> ImageBlobs { get; set; }
        public virtual ICollection<Folder> InverseParentFolder { get; set; }
    }
}
