using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SSOAuthServer.Models.AccountViewModels
{
    public class GrantAccessViewModel
    {
        public string ReturnUrl { get; set; }
        public string ClientId { get; set; }
    }
}