using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

#nullable disable

namespace BackendAPI.Models
{
    [DataContract]
    public partial class ImageBlob
    {
        public ImageBlob()
        {
            CbGroupXImages = new HashSet<CbGroupXImage>();
            CbTransactionXImages = new HashSet<CbTransactionXImage>();
        }

        [DataMember]
        public int SiteId { get; set; }
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        [DataMember]
        public int? FolderId { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string AltText { get; set; }
        [DataMember]
        public int? Creator { get; set; }
        [DataMember]
        public DateTime? Created { get; set; }
        [DataMember]
        public int? UpdatedBy { get; set; }
        [DataMember]
        public DateTime? LastUpdated { get; set; }
        [DataMember]
        public int? Width { get; set; }
        [DataMember]
        public int? Height { get; set; }
        [DataMember]
        public byte[] ImageBlob1 { get; set; }
        [DataMember]
        public byte[] ThumbnailBlob { get; set; }

        public virtual Folder Folder { get; set; }
        public virtual ICollection<CbGroupXImage> CbGroupXImages { get; set; }
        public virtual ICollection<CbTransactionXImage> CbTransactionXImages { get; set; }
    }
}
