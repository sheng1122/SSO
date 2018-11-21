using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class Config
    {
        public int WebSessionIdleTimeout { get; set; } = 1800;

        public string CookieName { get; set; }

        public int DataRefreshSecondsTimer { get; set; } = 30;

        public string DefaultPassword { get; set; } = "#EDC4rfv";

        public string DirTemp { get; set; }

        public int ServiceTimeoutMilliseconds { get; set; } = 15000;

        public string SystemEmailAddress { get; set; }

        public string SystemEmailPassword { get; set; }

        public string SmtpServerAddress { get; set; }

        public int SmtpServerPort { get; set; }

        public string ReturnUrlFormat { get; set; } = "{clientReturnUrl}?authToken={authToken}";

        public int UserAccessTokenAvailabilityHours { get; set; } = 150;

        public int ClientAuthTokenAvailabilityMinute { get; set; } = 10;
    }
}
