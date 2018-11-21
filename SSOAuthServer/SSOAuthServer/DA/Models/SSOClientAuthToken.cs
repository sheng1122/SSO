using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class SSOClientAuthToken : BaseModel
    {
        public int SSOClientAuthTokenId { get; set; }
        public Guid AuthToken { get; set; }
        public int SSOClientId { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiredUtcDate { get; set; }
    }
}