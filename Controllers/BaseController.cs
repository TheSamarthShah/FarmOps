using Microsoft.AspNetCore.Mvc;

namespace FarmOps.Controllers
{
    public class BaseController : Controller
    {
        protected string GetUserId()
        {
            return HttpContext.Session.GetString("FarmOpsUserId");
        }
        protected string GetUserEmail()
        {
            return HttpContext.Session.GetString("FarmOpsUserEmail");
        }
        protected string GetUserType()
        {
            return HttpContext.Session.GetString("FarmOpsUserType");
        }
    }
}
