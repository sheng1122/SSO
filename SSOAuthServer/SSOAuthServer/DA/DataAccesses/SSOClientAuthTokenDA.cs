using DA.DataAccesses.Common;
using DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.DataAccesses
{
    public class SSOClientAuthTokenDA : BaseDA
    {
        public SSOClientAuthTokenDA(string connstr) : base(connstr)
        {
        }

        public SSOClientAuthToken CreateSSOClientAuthToken(SSOClientAuthToken ssoClientAuthToken)
        {
            var resultSet = GetList<SSOClientAuthToken>("usp_create_ssoclientauthtoken", ssoClientAuthToken);

            return resultSet.FirstOrDefault();
        }

        public List<SSOClientAuthToken> GetSSOClientAuthTokens(Guid authToken, int ssoClientId)
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            List<SSOClientAuthToken> collection = new List<SSOClientAuthToken>();

            arg["AuthToken"] = authToken;
            arg["SSOClientId"] = ssoClientId;
            
            collection = GetList<SSOClientAuthToken>("usp_get_ssoclientauthtoken", arg);

            return collection;
        }

        public int InactiveAuthToken(int ssoClientAuthTokenId)
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            int rowAffected = 0;

            arg["SSOClientAuthTokenId"] = ssoClientAuthTokenId;

            rowAffected = ExecuteNonQuery("usp_inactive_ssoclientauthtoken", arg);

            return rowAffected;
        }
    }
}
