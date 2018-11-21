using System;
using System.Linq;
using DA.DataAccesses;
using Microsoft.AspNetCore.Mvc;
using SSOAuthServer.Contracts;
using SSOAuthServer.Contracts.Common;
using SSOAuthServer.Contracts.Error;
using SSOAuthServerHelpers;

namespace SSOAuthServer.Controllers
{
    [Route("api/authorization")]
    [ApiController]
    public class AuthorizationApiController : ControllerBase
    {
        [HttpGet]
        [Route("checkAuth")]
        public ActionResult<CheckAuthResponse> CheckAuth(string authToken, string clientSecret)
        {
            //validate input
            if (string.IsNullOrWhiteSpace(authToken))
            {
                return new CheckAuthResponse(Errors.AuthTokenRequired, null);
            }
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                return new CheckAuthResponse(Errors.ClientSecretRequired, null);
            }

            //get ssoClientId
            Guid parsedAuthToken = Guid.Empty;
            Guid.TryParse(authToken, out parsedAuthToken);

            Guid parsedClientSecret = Guid.Empty;
            Guid.TryParse(clientSecret, out parsedClientSecret);

            var selectedSSOClient = AppConfig.DBSSOClients.Where(x => x.ClientSecret == parsedClientSecret).FirstOrDefault();

            if (selectedSSOClient != null)
            {
                SSOClientAuthTokenDA ssoClientAuthTokenDA = new SSOClientAuthTokenDA(AppConfig.AppDbConn);
                var authTokenItem = ssoClientAuthTokenDA.GetSSOClientAuthTokens(parsedAuthToken, selectedSSOClient.SSOClientId).FirstOrDefault();

                if (authTokenItem != null)
                {
                    if (DateTime.UtcNow > authTokenItem.ExpiredUtcDate)
                    {
                        return new CheckAuthResponse(Errors.InvalidAuthToken, null);
                    }
                    else
                    {
                        //inactive this authToken
                        var rowAffected = ssoClientAuthTokenDA.InactiveAuthToken(authTokenItem.SSOClientAuthTokenId);

                        if (rowAffected > 0)
                        {
                            //get user info
                            UserDA userDA = new UserDA(AppConfig.AppDbConn);
                            var user = userDA.GetUsersById(authTokenItem.UserId).FirstOrDefault();

                            if (user != null)
                            {
                                var userInfo = new UserInfo
                                {
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Email = user.Email
                                };
                                return new CheckAuthResponse(0, userInfo);
                            }
                        }
                    }
                }
                else
                {
                    return new CheckAuthResponse(Errors.InvalidAuthToken, null);
                }
            }
            else
            {
                return new CheckAuthResponse(Errors.InvalidClientSecret, null);
            }
            return new CheckAuthResponse(500, null);
        }
    }
}
