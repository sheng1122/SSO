using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class UserSessionToken : BaseModel
    {
        public int UserSessionTokenId { get; set; }
        public Guid Token { get; set; }
        public string SSOProviderName { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiredUtcDate { get; set; }
    }
}