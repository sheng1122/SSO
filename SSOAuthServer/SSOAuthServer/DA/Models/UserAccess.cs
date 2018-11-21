using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class UserAccess : BaseModel
    {
        public int UserAccessId { get; set; }
        public int SSOClientId { get; set; }
        public int UserId { get; set; }
    }
}