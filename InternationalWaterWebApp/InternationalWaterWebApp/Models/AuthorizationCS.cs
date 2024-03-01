using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternationalWaterWebApp.Models
{
    public class AuthorizationCS : ActionFilterAttribute
    {
        private int UserRole { get; set; }
        private List<int> UserRoles { get; set; }
        public AuthorizationCS(int UserRole)
        {
            this.UserRole = UserRole;
        }

        public AuthorizationCS(int[] UserRole)
        {
            this.UserRoles = UserRole.ToList();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
            if ((actionName != "Login" && actionName != "SignUp" && actionName != "ResetPassword" && actionName != "SplashPage"))
            {
                int UsersUserRole = Convert.ToInt32(context.HttpContext.Session.GetString("UserRole"));
                if (context.HttpContext.Session.GetString("Id") == null)
                {
                    context.HttpContext.Response.Redirect("/WebApp/Login", false);
                    return;
                }
                else if (UserRole > 0)
                {
                    if (UsersUserRole != UserRole)
                    {
                        context.HttpContext.Session.Clear();
                        context.HttpContext.Response.Redirect("/WebApp/Login", false);
                        return;
                    }
                }
                else if (UserRoles.Count > 0)
                {
                    if (!UserRoles.Contains(UsersUserRole))
                    {
                        context.HttpContext.Session.Clear();
                        context.HttpContext.Response.Redirect("/WebApp/Login", false);
                        return;
                    }
                }
                base.OnActionExecuting(context);
            }
        }
    }
}
