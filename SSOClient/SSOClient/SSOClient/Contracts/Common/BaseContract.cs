using SSOClient.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOClient.Contracts.Common
{
    public class BaseRequest
    {
    }
    public class BaseResponse
    {
        public StatusInfo Status { get; set; }
    }
}
