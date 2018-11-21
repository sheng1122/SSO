using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SSOClient.Helpers;
using System;
using System.Linq;

namespace SSOClient.Controllers
{
    public class ExternalController : Controller
    {
        private readonly ILogger _logger;

        public ExternalController(
            ILogger<ExternalController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Route(int providerId)
        {
            var ssoProvider = AppConfig.DBSSOProviders.Where(x => x.SSOProviderId == providerId).FirstOrDefault();

            if (ssoProvider != null)
            {
                var redirectUrl = ssoProvider.RedirectUrl;
                redirectUrl = redirectUrl.Replace("{returnUrl}", AppConfig.DBConfig.SSOReturnUrl);
                redirectUrl = redirectUrl.Replace("{clientId}", ssoProvider.ClientId);

                return Redirect(redirectUrl);
            }
            else
            {
                throw new Exception("invalid return URL");
            }
        }
    }
}