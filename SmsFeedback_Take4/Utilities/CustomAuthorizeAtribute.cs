using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Utilities
{
   public class CustomAuthorizeAtribute: AuthorizeAttribute
   {      
      private string cDefaultRoleToConformTo = "ComunicationResponsible";     
      public CustomAuthorizeAtribute()
      {
         Roles = cDefaultRoleToConformTo;
      }      

      //public override void OnAuthorization(AuthorizationContext filterContext)
      //{
      //   if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
      //   {
      //      filterContext.HttpContext.SkipAuthorization = true;
      //      filterContext.HttpContext.Response.Clear();
      //      filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
      //      filterContext.Result = new HttpUnauthorizedResult("Unauthorized");
      //      filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
      //      filterContext.HttpContext.Response.End();
      //      return;
      //   }
      //   //if (filterContext.HttpContext.Request.IsAjaxRequest()                
      //   //       && (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Count() > 0
      //   //       || filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Count() > 0))
      //   //{            
      //   //}         
      //   //base.OnAuthorization(filterContext);
      //}

      protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
      {
         if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
         {
            filterContext.HttpContext.SkipAuthorization = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            filterContext.Result = new HttpUnauthorizedResult("Unauthorized");
            filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            filterContext.HttpContext.Response.End();
            base.HandleUnauthorizedRequest(filterContext);
         }
         if (!this.Roles.Split(',').Any(filterContext.HttpContext.User.IsInRole))
         {
            // The user is not in any of the listed roles => 
            // show the unauthorized view
            filterContext.Result = new ViewResult
            {
               ViewName = "~/Views/Shared/Unauthorized.aspx"
            };
         }
         else {
            base.HandleUnauthorizedRequest(filterContext);
         }
      }
   }
}