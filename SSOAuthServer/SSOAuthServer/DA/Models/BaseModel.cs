using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class BaseModel
    {
        public DateTime CreatedUTCDate { get; set; }
        public DateTime ModifiedUTCDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public bool Active { get; set; } = true;
    }
}
