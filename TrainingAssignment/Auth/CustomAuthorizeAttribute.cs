using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TrainingAssignment.Auth
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            var userId = Convert.ToString(httpContext.Session["Id"]);
            if (!string.IsNullOrEmpty(userId))
                using (var context = new ProductManagementAssignmentEntities())
                {
                    var userRole = (from u in context.Users                                  
                                    where u.Id.ToString() == userId select u).FirstOrDefault();
                    if(userRole!=null)
                    {
                        return true;
                    }
                }


            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Authentication" },
                    { "action", "Login" }
               });
        }
    }
}