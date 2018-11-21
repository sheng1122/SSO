using DA.DataAccesses.Common;
using DA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.DataAccesses
{
    public class SSOProviderDA : BaseDA
    {
        public SSOProviderDA(string connstr) : base(connstr)
        {
        }

        public SSOProvider CreateSSOProvider(SSOProvider ssoProvider)
        {
            var resultSet = GetList<SSOProvider>("usp_create_ssoprovider", ssoProvider);

            return resultSet.FirstOrDefault();
        }

        public List<SSOProvider> GetSSOProviders()
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            List<SSOProvider> collection = new List<SSOProvider>();

            collection = GetList<SSOProvider>("usp_get_ssoprovider", arg);

            return collection;
        }
    }
}
