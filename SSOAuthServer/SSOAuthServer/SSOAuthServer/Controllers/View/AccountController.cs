using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SSOAuthServer.Models.AccountViewModels;
using SSOAuthServer.Helpers;
using SSOAuthServerHelpers;
using DA.DataAccesses;
using DA.Models;
using System.Security.Cryptography;
using System.Text;

namespace SSOAuthServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;

        public AccountController(
            ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SSOLogin(string clientId, string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl) == true && string.IsNullOrWhiteSpace(returnUrl))
            {
                ViewBag.ErrorMessage = "Invalid return URL";
            }

            //check is valid clientId
            Guid parsedClientId = Guid.Empty;
            Guid.TryParse(clientId, out parsedClientId);
            var ssoClient = AppConfig.DBSSOClients.Where(x => x.ClientId == parsedClientId).FirstOrDefault();

            if (ssoClient == null)
            {
                ViewBag.ErrorMessage = "Invalid clientId";
            }
            else
            {
                //check user logged in
                if (UserSession.UserId != null)
                {
                    //proceed grants action
                    //check is user already granted access to ssoClient
                    UserAccessDA userAccessDA = new UserAccessDA(AppConfig.AppDbConn);
                    var userAccesses = userAccessDA.GetUserAccesses(UserSession.UserId.Value, ssoClient.SSOClientId);
                    if (userAccesses.Count > 0)
                    {
                        //return authToken
                        return GrantToken(clientId, returnUrl);
                    }
                    else
                    {
                        //ask for grant permission
                        return RedirectToAction(nameof(AccountController.GrantAccess), new { clientId, returnUrl });
                    }
                }
                else
                {
                    //prompt login/register
                    return RedirectToAction(nameof(AccountController.Login), new { clientId, returnUrl });
                }
            }
            return View("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string clientId, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(returnUrl))
            {
                //check user logged in
                if (UserSession.UserId != null)
                {
                    //proceed grants action
                    return RedirectToAction(nameof(AccountController.GrantAccess), new { clientId, returnUrl });
                }
            }
            RegisterViewModel model = new RegisterViewModel();
            model.ReturnUrl = returnUrl;
            model.ClientId = clientId;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string button)
        {
            // the user clicked the "cancel" button
            if (button != "register")
            {
                // since we don't have a valid context, then we just go back to the home page
                return RedirectToAction(nameof(AccountController.Login));
            }

            if (ModelState.IsValid)
            {
                var md5 = new MD5CryptoServiceProvider();
                var hashPassword = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(model.Password))).Replace("-", string.Empty);

                UserDA userDA = new UserDA(AppConfig.AppDbConn);
                User user = new User();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.HashPassword = hashPassword;
                var result = userDA.CreateUser(user);

                if (result != null)//success created
                {
                    //log user in
                    return await LogUserIn(result, model.ClientId, model.ReturnUrl);
                }
            }
            ViewBag.ErrorMessage = "Register failed. Please retry.";
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string clientId, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(returnUrl))
            {
                //check user logged in
                if (UserSession.UserId != null)
                {
                    //proceed grants action
                    return RedirectToAction(nameof(AccountController.GrantAccess), new { clientId, returnUrl });
                }
            }
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = returnUrl;
            model.ClientId = clientId;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string button)
        {
            // the user clicked the "cancel" button
            if (button != "login")
            {
                // since we don't have a valid context, then we just go back to the home page
                return RedirectToAction(nameof(AccountController.Login));
            }

            if (ModelState.IsValid)
            {
                var md5 = new MD5CryptoServiceProvider();
                var hashPassword = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(model.Password))).Replace("-", string.Empty);

                UserDA userDA = new UserDA(AppConfig.AppDbConn);
                var results = userDA.GetUsers(model.Email, hashPassword);
                var result = results.FirstOrDefault();

                if (result != null)//valid user
                {
                    //log user in
                    return await LogUserIn(result, model.ClientId, model.ReturnUrl);
                }
            }
            ViewBag.ErrorMessage = "Login failed. Please retry.";
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GrantAccess(string clientId, string returnUrl)
        {
            Guid parsedClientId = Guid.Empty;
            Guid.TryParse(clientId, out parsedClientId);
            var ssoClient = AppConfig.DBSSOClients.Where(x => x.ClientId == parsedClientId).FirstOrDefault();

            //check is user already granted access to ssoClient
            UserAccessDA userAccessDA = new UserAccessDA(AppConfig.AppDbConn);
            var userAccesses = userAccessDA.GetUserAccesses(UserSession.UserId.Value, ssoClient.SSOClientId);
            if (userAccesses.Count > 0)
            {
                //return authToken
                return GrantToken(clientId, returnUrl);
            }
            else
            {
                //ask for grant permission
                GrantAccessViewModel model = new GrantAccessViewModel();
                model.ReturnUrl = returnUrl;
                model.ClientId = clientId;

                ViewBag.SSOClientName = ssoClient.ClientName;
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult GrantAccess(GrantAccessViewModel model, string button)
        {
            // the user clicked the "cancel" button
            if (button != "confirm")
            {
                // since we don't have a valid context, then we just go back to the home page
                return RedirectToAction(nameof(AccountController.Login));
            }

            if (ModelState.IsValid)
            {
                Guid parsedClientId = Guid.Empty;
                Guid.TryParse(model.ClientId, out parsedClientId);
                var ssoClient = AppConfig.DBSSOClients.Where(x => x.ClientId == parsedClientId).FirstOrDefault();

                //log grant
                UserAccessDA userAccessDA = new UserAccessDA(AppConfig.AppDbConn);
                UserAccess userAccess = new UserAccess();
                userAccess.SSOClientId = ssoClient.SSOClientId;
                userAccess.UserId = UserSession.UserId.Value;
                var result = userAccessDA.CreateUserAccess(userAccess);

                if (result != null)
                {
                    return GrantToken(model.ClientId, model.ReturnUrl);
                }

            }
            ViewBag.ErrorMessage = "Grant access failed. Please retry.";
            return View(model);
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

        private IActionResult GrantToken(string clientId, string returnUrl)
        {
            Guid parsedClientId = Guid.Empty;
            Guid.TryParse(clientId, out parsedClientId);
            var ssoClient = AppConfig.DBSSOClients.Where(x => x.ClientId == parsedClientId).FirstOrDefault();

            //create authToken
            SSOClientAuthTokenDA SSOClientAuthTokenDA = new SSOClientAuthTokenDA(AppConfig.AppDbConn);
            SSOClientAuthToken ssoClientAuthToken = new SSOClientAuthToken();
            ssoClientAuthToken.AuthToken = Guid.NewGuid();
            ssoClientAuthToken.SSOClientId = ssoClient.SSOClientId; ;
            ssoClientAuthToken.UserId = UserSession.UserId.Value;
            ssoClientAuthToken.ExpiredUtcDate = DateTime.UtcNow.AddMinutes(AppConfig.DBConfig.ClientAuthTokenAvailabilityMinute);
            var tokenResult = SSOClientAuthTokenDA.CreateSSOClientAuthToken(ssoClientAuthToken);

            if (tokenResult != null)
            {
                var rclientRturnUrl = AppConfig.DBConfig.ReturnUrlFormat;
                rclientRturnUrl = rclientRturnUrl.Replace("{clientReturnUrl}", returnUrl);
                rclientRturnUrl = rclientRturnUrl.Replace("{authToken}", ssoClientAuthToken.AuthToken.ToString());

                return Redirect(rclientRturnUrl);
            }
            ViewBag.ErrorMessage = "Grant token failed. Please retry.";
            return View("Login", new LoginViewModel { ClientId = clientId, ReturnUrl = returnUrl });
        }

        private async Task<IActionResult> LogUserIn(User result, string clientId, string returnUrl)
        {
            UserSessionTokenDA userSessionTokenDA = new UserSessionTokenDA(AppConfig.AppDbConn);
            UserSessionToken userSessionToken = new UserSessionToken();
            userSessionToken.SessionToken = Guid.NewGuid();
            userSessionToken.SSOClientId = 0;
            userSessionToken.UserId = result.UserId;
            userSessionToken.ExpiredUtcDate = DateTime.UtcNow.AddHours(AppConfig.DBConfig.UserAccessTokenAvailabilityHours);
            var tokenResult = userSessionTokenDA.CreateUserSessionToken(userSessionToken);

            if (tokenResult != null)
            {
                var userName = string.Format("{0} {1}", result.FirstName, result.LastName); ;
                await UserSession.Set(result.UserId, result.Email, userName, userSessionToken.SessionToken, userSessionToken.ExpiredUtcDate);

                if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(returnUrl))
                {
                    return RedirectToAction(nameof(AccountController.GrantAccess), new { clientId, returnUrl });
                }
            }
            ViewBag.ErrorMessage = "Login failed. Please retry.";
            return View("Login", new LoginViewModel { ClientId = clientId, ReturnUrl = returnUrl });
        }
    }
}
