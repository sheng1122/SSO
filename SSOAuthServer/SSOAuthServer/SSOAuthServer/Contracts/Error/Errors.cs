using SSOAuthServer.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSOAuthServer.Contracts.Error
{
    public class Errors
    {
        public static readonly StatusInfo AuthTokenRequired = new StatusInfo
        {
            Code = 7001,
            Msg = "Parameter 'authToken' is required."
        };
        public static readonly StatusInfo ClientSecretRequired = new StatusInfo
        {
            Code = 7002,
            Msg = "Parameter 'clientSecret' is required."
        };
        public static readonly StatusInfo InvalidClientSecret = new StatusInfo
        {
            Code = 7003,
            Msg = "Invalid client secret."
        };
        public static readonly StatusInfo InvalidAuthToken = new StatusInfo
        {
            Code = 7004,
            Msg = "Invalid AuthToken."
        };
        public static readonly StatusInfo AuthTokenExpired = new StatusInfo
        {
            Code = 7005,
            Msg = "AuthToken Expired."
        };
    }
}
