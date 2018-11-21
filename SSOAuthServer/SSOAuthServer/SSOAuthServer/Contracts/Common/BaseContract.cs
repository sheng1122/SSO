using SSOAuthServer.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuthServer.Contracts.Common
{
    public class BaseRequest
    {
    }
    public class BaseResponse
    {
        public StatusInfo Status { get; set; }

        public void SetResponse(int statusCode, string statusMsg = null)
        {
            StatusInfo status = new StatusInfo
            {
                Code = statusCode
            };

            switch (statusCode)
            {
                case 0:
                    status.Msg = "Success";
                    break;
                case 1:
                    status.Msg = statusMsg;
                    break;
                case -500:
                    status.Msg = string.IsNullOrWhiteSpace(statusMsg) ? statusMsg : "Unknown exception";
                    break;
                default:
                    status.Msg = string.IsNullOrWhiteSpace(statusMsg) ? statusMsg : "Unknown exception";
                    break;
            }

            Status = status;
        }
    }
}
