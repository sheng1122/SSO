using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Models
{
    public class Audit
    {
        public string AppName { get; set; }
        public string ServiceName { get; set; }
        public string OperationName { get; set; }
        public string RequestContent { get; set; }
        public string ResponseContent { get; set; }
        public string Token { get; set; }
        public string RemoteAddress { get; set; }
        public string HttpMethod { get; set; }
        public string ClientAddress { get; set; }
        public int ClientPort { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime ResponseDateTime { get; set; }
        public int StatusCode { get; set; }
    }
}
