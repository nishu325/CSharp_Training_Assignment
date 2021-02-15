using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;
//using IAuthenticationFilter = System.Web.Http.Filters.IAuthenticationFilter;

namespace TrainingAssignment.Auth
{
    public class CustomAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
       

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["Id"])))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                //Redirecting the user to the Login View of Account Controller  
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                     { "controller", "Authentication" },
                     { "action", "Login" }
                });
            }
        }
    }
}