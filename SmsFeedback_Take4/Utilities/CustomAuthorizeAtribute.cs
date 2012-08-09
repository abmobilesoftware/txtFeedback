using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Utilities
{
   public class CustomAuthorizeAtribute: AuthorizeAttribute
   {
      protected override bool AuthorizeCore(HttpContextBase httpContext)
      {
         //here I would dinamically get the required roles and decide if we are allowed or not to view that area
         Roles = "ComunicationResponsible";
         return base.AuthorizeCore(httpContext);
      }
      public override void OnAuthorization(AuthorizationContext filterContext)
      {
         if (filterContext.HttpContext.Request.IsAjaxRequest()
                && !filterContext.HttpContext.User.Identity.IsAuthenticated
                && (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Count() > 0
                || filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Count() > 0))
         {
            filterContext.HttpContext.SkipAuthorization = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            filterContext.Result = new HttpUnauthorizedResult("Unauthorized");
            filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            filterContext.HttpContext.Response.End();
         }
         //base.OnAuthorization(filterContext);
      }
   }
}