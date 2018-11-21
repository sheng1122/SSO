using DA.DataAccesses.Common;
using DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.DataAccesses
{
    public class UserSessionTokenDA : BaseDA
    {
        public UserSessionTokenDA(string connstr) : base(connstr)
        {
        }

        public UserSessionToken CreateUserSessionToken(UserSessionToken userSessionToken)
        {
            var resultSet = GetList<UserSessionToken>("usp_create_usersessiontoken", userSessionToken);

            return resultSet.FirstOrDefault();
        }

        public List<UserSessionToken> GetUserSessionTokens(Guid userId)
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            List<UserSessionToken> collection = new List<UserSessionToken>();

            arg["UserId"] = userId;

            collection = GetList<UserSessionToken>("usp_get_usersessiontoken", arg);

            return collection;
        }
    }
}
