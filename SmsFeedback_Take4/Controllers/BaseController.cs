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
        protected override void ExecuteCore()
        {
            if (RouteData.Values["lang"] != null &&
                !string.IsNullOrWhiteSpace(RouteData.Values["lang"].ToString()))
            {
                // set the culture from the route data (url)
                currentCulture = RouteData.Values["lang"].ToString();
                var lang = RouteData.Values["lang"].ToString();
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
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                else
                {
                    // set the culture by the location if not specified
                   if (HttpContext.Request.UserLanguages != null && HttpContext.Request.UserLanguages.Count() > 0) 
                   {
                      langHeader = HttpContext.Request.UserLanguages[0];
                      Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                   }
                   else
                   {
                      //when not called via a browser HttpContext.Request.UserLanguages will be null
                      langHeader = "en-US";
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
