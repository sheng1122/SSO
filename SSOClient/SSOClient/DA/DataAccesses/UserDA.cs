using DA.DataAccesses.Common;
using DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.DataAccesses
{
    public class UserDA : BaseDA
    {
        public UserDA(string connstr) : base(connstr)
        {
        }

        public User CreateUser(User user)
        {
            var resultSet = GetList<User>("usp_create_user", user);

            return resultSet.FirstOrDefault(); ;
        }

        public List<User> GetUsers(string email)
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            List<User> collection = new List<User>();

            arg["Email"] = email;

            collection = GetList<User>("usp_get_user", arg);

            return collection;
        }
    }
}
