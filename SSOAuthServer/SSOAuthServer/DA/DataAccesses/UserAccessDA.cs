using DA.DataAccesses.Common;
using DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.DataAccesses
{
    public class UserAccessDA : BaseDA
    {
        public UserAccessDA(string connstr) : base(connstr)
        {
        }

        public UserAccess CreateUserAccess(UserAccess userAccess)
        {
            var resultSet = GetList<UserAccess>("usp_create_useraccess", userAccess);

            return resultSet.FirstOrDefault();
        }

        public List<UserAccess> GetUserAccesses(int userId, int ssoClientId)
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            List<UserAccess> collection = new List<UserAccess>();

            arg["UserId"] = userId;
            arg["SSOClientId"] = ssoClientId;

            collection = GetList<UserAccess>("usp_get_useraccess", arg);

            return collection;
        }
    }
}
