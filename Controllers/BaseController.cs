using Microsoft.AspNetCore.Mvc;

namespace FarmOps.Controllers
{
    public class BaseController : Controller
    {
        public string GetUserId()
        {
            return HttpContext.Session.GetString("FarmOpsUserId");
        }
        public string GetUserEmail()
        {
            return HttpContext.Session.GetString("FarmOpsUserEmail");
        }
        public string GetUserType()
        {
            return HttpContext.Session.GetString("FarmOpsUserType");
        }
    }
}
