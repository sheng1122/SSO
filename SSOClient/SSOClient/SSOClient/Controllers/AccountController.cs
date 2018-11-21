using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SSOClient.Helpers;
using SSOClient.Models.AccountViewModels;
using System.Net.Http;
using SSOClient.Enums;
using SSOClient.Contracts;
using Newtonsoft.Json;
using DA.DataAccesses;
using DA.Models;

namespace SSOClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;

        [TempData]
        public string ErrorMessage { get; set; }

        public AccountController(
            ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();

            return View(model);
        }

        //handle external login from TEST-SSO-PROVIDER
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin_Test(string authToken)
        {
            //validate token + get userInfo
            var testSSO = AppConfig.DBSSOProviders.Where(x => x.ProviderName == SSOProviderName._TEST_SSO_PROVIDER).FirstOrDefault();

            var destinationUrl = testSSO.ValidateAuthTokenUrl;
            destinationUrl = destinationUrl.Replace("{authToken}", authToken);
            destinationUrl = destinationUrl.Replace("{clientSecret}", testSSO.ClientSecret);

            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(destinationUrl);
            var httpResult = await response.Content.ReadAsStringAsync();
            CheckAuthResponse responseObj = JsonConvert.DeserializeObject<CheckAuthResponse>(httpResult);

            if (responseObj.Status.Code == 0)
            {
                //find user in own db
                UserDA userDA = new UserDA(AppConfig.AppDbConn);
                var results = userDA.GetUsers(responseObj.UserInfo.Email);
                var result = results.FirstOrDefault();

                if (results.Count == 0)//new user, register new user
                {
                    User user = new User();
                    user.FirstName = responseObj.UserInfo.FirstName;
                    user.LastName = responseObj.UserInfo.LastName;
                    user.Email = responseObj.UserInfo.Email;
                    user.HashPassword = Guid.NewGuid().ToString();
                    result = userDA.CreateUser(user);
                }

                if (result != null)//proceed login
                {
                    //log user in
                    UserSessionTokenDA userSessionTokenDA = new UserSessionTokenDA(AppConfig.AppDbConn);
                    UserSessionToken userSessionToken = new UserSessionToken();
                    userSessionToken.Token = Guid.NewGuid();
                    userSessionToken.SSOProviderName = SSOProviderName._TEST_SSO_PROVIDER;
                    userSessionToken.UserId = result.UserId;
                    userSessionToken.ExpiredUtcDate = DateTime.UtcNow.AddHours(AppConfig.DBConfig.UserAccessTokenAvailabilityHours);
                    var tokenResult = userSessionTokenDA.CreateUserSessionToken(userSessionToken);

                    if (tokenResult != null)
                    {
                        await UserSession.Set(result.UserId, result.Email, result.LastName, userSessionToken.Token, userSessionToken.ExpiredUtcDate);
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = responseObj.Status.Msg;
                return View("Login");
            }

            //return home
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            if (string.IsNullOrWhiteSpace(UserSession.Email) == false)
            {
                _logger.LogInformation("User logged out.");
                UserSession.Clear();
            }
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
    }
}
