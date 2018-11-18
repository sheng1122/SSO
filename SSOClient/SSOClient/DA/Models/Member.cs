using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DA.Models
{
    public class Member : BaseModel
    {
        public string UserId { get; set; }
        public string MemberId { get; set; }
        public string MemberIC { get; set; }
    }
}
