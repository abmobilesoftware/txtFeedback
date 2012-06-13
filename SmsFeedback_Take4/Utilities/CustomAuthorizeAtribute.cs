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
   }
}