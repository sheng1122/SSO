using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class UserSessionToken : BaseModel
    {
        public int UserSessionTokenId { get; set; }
        public Guid SessionToken { get; set; }
        public int SSOClientId { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiredUtcDate { get; set; }
    }
}

