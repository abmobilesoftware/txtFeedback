using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using System.Web;

namespace SmsFeedback_Take4.Controllers
{
    public abstract class BaseController : Controller
    {
        public string currentCulture = "en";
        private static List<String> supportedLanguage = new List<String> { "en", "de", "ro", "es" };
        protected override void ExecuteCore()
        {
           //DA if the user language is not supported then we should not use it but default to en-US
           
           //we are only interested in the language part, not the country specific part

            if (RouteData.Values["lang"] != null &&
                !string.IsNullOrWhiteSpace(RouteData.Values["lang"].ToString()))
            {
                // set the culture from the route data (url)
                var lang = RouteData.Values["lang"].ToString();
                String countryCode = lang.Substring(0, 2);
                if (!supportedLanguage.Contains(countryCode))
                {
                   //language not support - default to en
                   lang = "en-US";
                }
                currentCulture = lang;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            }
            else
            {
                // load the culture info from the cookie
                var cookie = HttpContext.Request.Cookies["TxtFeedback.MvcLocalization.CurrentUICulture"];
                var langHeader = string.Empty;
                if (cookie != null)
                {
                    // set the culture by the cookie content
                    langHeader = cookie.Value;
                    currentCulture = langHeader;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                else
                {
                    // set the culture by the location if not specified
                   if (HttpContext.Request.UserLanguages != null && HttpContext.Request.UserLanguages.Count() > 0) 
                   {
                      langHeader = HttpContext.Request.UserLanguages[0];
                      String countryCode = langHeader.Substring(0, 2);
                      if (supportedLanguage.Contains(countryCode))
                      {
                         Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                      }
                      else
                      {
                         //language not support - default to en
                         langHeader = "en-US";
                         Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                      }
                   }
                   else
                   {
                      //when not called via a browser HttpContext.Request.UserLanguages will be null
                      langHeader = "en-US";
                      Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                   }
                }
                // set the lang value into route data
                RouteData.Values["lang"] = langHeader;
            }

            // save the location into cookie
            HttpCookie _cookie = new HttpCookie("TxtFeedback.MvcLocalization.CurrentUICulture", Thread.CurrentThread.CurrentUICulture.Name);
            _cookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Response.SetCookie(_cookie);

            base.ExecuteCore();
        }

        public string getCurrentCulture() {
            return currentCulture;
        }
    }

    

}
