using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Threading;

namespace SmsFeedback_Take4.Utilities
{
public static class SwitchLanguageHelper
    {
        public class Language
        {
            public string Url { get; set; }
            public string ActionName { get; set; }
            public string ControllerName { get; set; }
            public RouteValueDictionary RouteValues { get; set; }
            public bool IsSelected { get; set; }

            public MvcHtmlString HtmlSafeUrl
            {
                get
                {
                    return MvcHtmlString.Create(Url);
                }
            }
        }

        public static Language LanguageUrl(this HtmlHelper helper, string cultureName,
            string languageRouteName = "lang", bool strictSelected = false)
        {
            // set the input language to lower
            cultureName = cultureName.ToLower();
            // retrieve the route values from the view context
            var routeValues = new RouteValueDictionary(helper.ViewContext.RouteData.Values);
            // copy the query strings into the route values to generate the link
            var queryString = helper.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in queryString)
            {
                if (queryString[key] != null && !string.IsNullOrWhiteSpace(key))
                {
                    if (routeValues.ContainsKey(key))
                    {
                        routeValues[key] = queryString[key];
                    }
                    else
                    {
                        routeValues.Add(key, queryString[key]);
                    }
                }
            }
            var actionName = routeValues["action"].ToString();
            var controllerName = routeValues["controller"].ToString();
            // set the language into route values
            routeValues[languageRouteName] = cultureName;
            // generate the language specify url
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var url = urlHelper.RouteUrl("Localization", routeValues);
            // check whether the current thread ui culture is this language
            var current_lang_name = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
            var isSelected = strictSelected ?
                current_lang_name == cultureName :
                current_lang_name.StartsWith(cultureName);
            return new Language()
            {
                Url = url,
                ActionName = actionName,
                ControllerName = controllerName,
                RouteValues = routeValues,
                IsSelected = isSelected
            };
        }

        public static MvcHtmlString LanguageSelectorLink(this HtmlHelper helper,
            string cultureName, string selectedText, string unselectedText,
            IDictionary<string, object> htmlAttributes, string languageRouteName = "lang", bool strictSelected = false)
        {
            var language = helper.LanguageUrl(cultureName, languageRouteName, strictSelected);
            var link = helper.RouteLink(language.IsSelected ? selectedText : unselectedText,
                "Localization", language.RouteValues, htmlAttributes);
            return link;
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
    }
}