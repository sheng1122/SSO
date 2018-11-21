using DA.DataAccesses.Common;
using DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.DataAccesses
{
    public class SSOClientDA : BaseDA
    {
        public SSOClientDA(string connstr) : base(connstr)
        {
        }

        public SSOClient CreateSSOClient(SSOClient ssoClient)
        {
            var resultSet = GetList<SSOClient>("usp_create_ssoclient", ssoClient);

            return resultSet.FirstOrDefault();
        }

        public List<SSOClient> GetSSOClients()
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            List<SSOClient> collection = new List<SSOClient>();

            collection = GetList<SSOClient>("usp_get_ssoclient", arg);

            return collection;
        }
    }
}
