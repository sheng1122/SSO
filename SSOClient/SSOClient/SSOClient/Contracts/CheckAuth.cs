using SSOClient.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOClient.Contracts
{
    public class CheckAuthRequest : BaseRequest
    {
    }
    public class CheckAuthResponse : BaseResponse
    {
        public UserInfo UserInfo { get; set; }
    }
}
