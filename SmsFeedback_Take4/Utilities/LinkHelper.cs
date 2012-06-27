using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
   /* 
    * Based on http://stackoverflow.com/questions/4728777/active-menu-item-asp-net-mvc3-master-page
    * We extend the default ActionLink with our own handler where we add an "active class" to the selected menu item (so we can style it)
    */
   public static class LinkHelper
   {
      public static MvcHtmlString MenuLink(
             this HtmlHelper htmlHelper,
             string linkText,
             string actionName,
             string controllerName
)
      {
         string currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
         string currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
         if (actionName == currentAction && controllerName == currentController)
         {
            return htmlHelper.ActionLink(
                linkText,
                actionName,
                controllerName,
                null,
                new
                {
                   @class = "currentMenuItem"
                });
         }
         return htmlHelper.ActionLink(linkText, actionName, controllerName);
      }
   }
}