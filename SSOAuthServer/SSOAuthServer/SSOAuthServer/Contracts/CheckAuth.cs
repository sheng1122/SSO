using SSOAuthServer.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuthServer.Contracts
{
    public class CheckAuthRequest : BaseRequest
    {
    }
    public class CheckAuthResponse : BaseResponse
    {
        public UserInfo UserInfo { get; set; }

        public CheckAuthResponse(StatusInfo statusInfo, UserInfo data)
        {
            Status = statusInfo;
            UserInfo = data;
        }

        public CheckAuthResponse(int statusCode, UserInfo data)
        {
            SetResponse(statusCode, null);
            UserInfo = data;
        }
    }
}
