using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class SSOProvider : BaseModel
    {
        public int SSOProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string ValidateAuthTokenUrl { get; set; }
    }
}