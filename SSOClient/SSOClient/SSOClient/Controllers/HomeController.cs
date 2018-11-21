using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SSOClient.Helpers;
using SSOClient.Models;

namespace SSOClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (UserSession.UserId == null || (UserSession.TokenExpiredDate != null && DateTime.UtcNow > UserSession.TokenExpiredDate))
            {
                UserSession.Clear();
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
