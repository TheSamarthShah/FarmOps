using Microsoft.AspNetCore.Mvc;

namespace FarmOps.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
