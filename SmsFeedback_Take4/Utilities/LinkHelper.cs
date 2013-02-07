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
         string controllerName,
         string tooltip = "")
      {
         string currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
         string currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
         if (controllerName == "Home" && actionName=="Index")
         {
            return MenuLinkForMessages(htmlHelper, linkText, actionName, controllerName, tooltip);
           }
         if (actionName == currentAction && controllerName == currentController)
         {
            string classForElement = "currentMenuItem";
            return htmlHelper.ActionLink(
                linkText,
                actionName,
                controllerName,
                null,
                new
                {
                   @class = classForElement
                });
         }
         return htmlHelper.ActionLink(linkText, actionName, controllerName);
      }
      public static MvcHtmlString MenuLinkForMessages(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string tooltip) 
      {
         var urlHelper = ((Controller)htmlHelper.ViewContext.Controller).Url;
         var url = "#";
         if (!string.IsNullOrEmpty(actionName))
            url = urlHelper.Action(actionName, controllerName, null);
         TagBuilder countTag = new TagBuilder("span");
         countTag.MergeAttribute("id", "msgTabcount");
         countTag.MergeAttribute("tooltiptitle", tooltip);
         countTag.SetInnerText("(0)");

         TagBuilder txtlink = new TagBuilder("a");
         txtlink.MergeAttribute("href", url);
         txtlink.InnerHtml = linkText + " " + countTag.ToString();

         string currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
         string currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
         if (actionName == currentAction && controllerName == currentController)
         {
            txtlink.AddCssClass("currentMenuItem");
         }
         return new MvcHtmlString(txtlink.ToString());
      }
   }

   
}