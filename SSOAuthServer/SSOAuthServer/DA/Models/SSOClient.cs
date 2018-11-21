using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class SSOClient : BaseModel
    {
        public int SSOClientId { get; set; }
        public string ClientName { get; set; }
        public Guid ClientId { get; set; }
        public Guid ClientSecret { get; set; }
        public string HashPassword { get; set; }
    }
}