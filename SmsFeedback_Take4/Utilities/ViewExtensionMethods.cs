using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
namespace SmsFeedback_Take4.Utilities
{
   public static class ViewExtensionMethods
   {

      public static MvcHtmlString ImageLink(this UrlHelper helper, string imageUrl, string altText, string actionName, string controllerName, object routeValues, string imgID = null)
      {
         var imgTag = new TagBuilder("img");
         imgTag.MergeAttribute("src", imageUrl);
         imgTag.MergeAttribute("alt", altText);
         if (!String.IsNullOrEmpty(imgID))
         {
            imgTag.GenerateId(imgID);
         }

         var link = helper.Action(actionName, controllerName);
         TagBuilder imglink = new TagBuilder("a");
         imglink.MergeAttribute("href", link.ToString());
         imglink.InnerHtml = imgTag.ToString();
         return new MvcHtmlString(imglink.ToString()); ;
      }
      public static MvcHtmlString ImageLink(this HtmlHelper htmlHelper, string imgSrc, string cultureName, object htmlAttributes, object imgHtmlAttributes, string languageRouteName = "lang", bool strictSelected = false)
      {
         UrlHelper urlHelper = ((Controller)htmlHelper.ViewContext.Controller).Url;
         TagBuilder imgTag = new TagBuilder("img");
         imgTag.MergeAttribute("src", imgSrc);
         imgTag.MergeAttributes((IDictionary<string, string>)imgHtmlAttributes, true);

         var language = htmlHelper.LanguageUrl(cultureName, languageRouteName, strictSelected);
         string url = language.Url;

         TagBuilder imglink = new TagBuilder("a");
         imglink.MergeAttribute("href", url);
         imglink.InnerHtml = imgTag.ToString();
         imglink.MergeAttributes((IDictionary<string, string>)htmlAttributes, true);

         //if the current page already contains the language parameter make sure the corresponding html element is marked
         string currentLanguage = htmlHelper.ViewContext.RouteData.GetRequiredString("lang");
         if (cultureName.Equals(currentLanguage, StringComparison.InvariantCultureIgnoreCase))
         {
            imglink.AddCssClass("selectedLanguage");
         }
         return new MvcHtmlString(imglink.ToString());
      }

      public static MvcHtmlString UpdatedResourceLink(this UrlHelper helper, string resourceURL)
      {
         var link = helper.Content(resourceURL);
         try
         {
            var fileLink = HttpContext.Current.Server.MapPath(resourceURL);
            var file = File.GetLastWriteTime(fileLink);
            var fileTimestamp = file.ToString("yyyyMMddHHmmss");

            var strRegex = @"(.*)(\.css|\.js)$";
            RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);
            var matches = myRegex.Matches(link);
            if (matches.Count == 1)
            {
               var match = matches[0];
               link = match.Groups[1].Value + "." + fileTimestamp + match.Groups[2];
            }
         }
         catch (Exception ex)
         {
            //TODO we should log this somewhere
         }
         return new MvcHtmlString(link);
      }
   }
}