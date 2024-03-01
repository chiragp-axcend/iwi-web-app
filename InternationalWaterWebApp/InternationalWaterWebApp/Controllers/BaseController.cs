using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternationalWaterWebApp.Controllers
{
    public class BaseController : Controller
    {
        protected int LoginUserId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Session.GetString("Id"));
            }
        }
        protected int LoginUserRoleId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Session.GetString("UserRole"));
            }
        }
        protected string LoginUserRole
        {
            get
            {
                return HttpContext.Session.GetString("UserRole");
            }
        }

        protected int AdvisorId
        {
            get
            {
                return Convert.ToInt32(HttpContext.Session.GetString("AdvisorId"));
            }
        }

    }
}
