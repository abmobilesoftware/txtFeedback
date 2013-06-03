using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using SmsFeedback_Take4.Utilities;
using System.Web.Optimization;
//required by log4net
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SmsFeedback_Take4
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*bosh}", new { bosh = @"^(.*)/http-bindours(\/?)$" });

            routes.MapRoute(
                "LogOn", // Route name
                 "Account/{action}", // URL with parameters
                 new { controller = "Account", action = "LogOn" } // Parameter defaults
                );

            routes.MapRoute(
             "Localization", // Route name
             "{lang}/{controller}/{action}/{id}", // URL with parameters
             new { controller = "Home", action = "Index", id = UrlParameter.Optional },// Parameter defaults
             new { lang = @"\w{2,3}(-\w{4})?(-\w{2,3})?" } //match the language and pass it as a parameter
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // Parameter defaults
            );           
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory("Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }
}