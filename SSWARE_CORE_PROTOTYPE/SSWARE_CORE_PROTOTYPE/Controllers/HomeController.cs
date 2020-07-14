using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SSWARE_CORE_PROTOTYPE.Controllers
{
    public class HomeController : ControllerBase
    {        
        public HomeController(IConfiguration configuration) : base(configuration)
        {
            
        }
        
        public IActionResult Home()
        {            
            string message = "";
            if (CheckSigned(ref message))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Entrance", "Main");                
            }
        }
    }
}