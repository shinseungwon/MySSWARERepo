using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SSWARE_CORE_PROTOTYPE.Models.Shared;

namespace SSWARE_CORE_PROTOTYPE.Controllers
{
    public class MainController : ControllerBase
    {              
        public MainController(IConfiguration configuration) : base(configuration)
        {
            
        }

        public IActionResult Test(string a)
        {
            ViewData["Body"] = a + " attached";
            return View("Dummy");
        }

        public IActionResult Entrance()
        {
            string message = "";
            if (CheckSigned(ref message))
            {
                return RedirectToAction("Home", "Home");
            }
            else
            {
                return View();
            }                
        }

        [HttpPost]
        public IActionResult SignIn(string id, string password)
        {
            UserManagementModel userManagementModel 
                = new UserManagementModel(configuration);
            string result = "";
            int res = userManagementModel.SignIn(id, password, ref result);
            ViewData["Result"] = result;

            if(res == 0)
            {
                Microsoft.AspNetCore.Http.CookieOptions cookieOptions 
                    = new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    IsEssential = true,
                    Expires = DateTime.Now.AddHours(1),
                };
                
                Response.Cookies.Append("ID"
                    , Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(id))
                    , cookieOptions);

                return RedirectToAction("Home", "Home");
            }
            else
            {
                return RedirectToAction("Entrance", "Main");
            }
        }
                
        public IActionResult LogOut()
        {
            string idBase64 = Request.Cookies["ID"];
            if (!string.IsNullOrEmpty(idBase64))
            {
                Response.Cookies.Delete("ID");
            }

            return RedirectToAction("Entrance", "Main");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
