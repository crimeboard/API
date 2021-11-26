using System;
using System.Collections.Generic;

#nullable disable

namespace BackendAPI.Models
{
    public partial class CbGroupXImage
    {
        public int GroupId { get; set; }
        public int ImageId { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }

        public virtual CbGroup Group { get; set; }
        public virtual ImageBlob Image { get; set; }
    }
}
