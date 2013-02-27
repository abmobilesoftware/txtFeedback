using System.Web;
using System.Web.Optimization;

namespace mobile.TxtFeedback_take1
{
   public class BundleConfig
   {
      // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
      public static void RegisterBundles(BundleCollection bundles)
      {
         BundleTable.EnableOptimizations = true;         
         
         bundles.Add(new ScriptBundle("~/bundles/myscripts").Include(                                        
                     "~/MyScripts/Utilities.js",                    
                     "~/MyScripts/Home.js",
                     "~/MyScripts/XMPP.js"));
                     

         bundles.Add(new ScriptBundle("~/bundles/lessLikelyToChange").Include(
                     "~/Scripts/underscore*",
                     "~/Scripts/backbone*",
                     "~/MyScripts/strophe.js",
                     "~/Scripts/persist-min.js",
                     "~/Scripts/json2.js",
                     "~/MyScripts/strophe.register.js",
                     "~/Scripts/jQuery.ui.datepicker.js", 
                     "~/Scripts/jquery.ui.datepicker.mobile.js",
                     "~/Scripts/jquery.ui.datepicker-de.js",
                     "~/Scripts/jquery.ui.datepicker-en-GB.js",
                     "~/Scripts/jquery.ui.datepicker-ro.js",
                     "~/Scripts/jquery.ui.datepicker-es.js"));
                          
         bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-1.8*"));

         bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                     "~/Scripts/jquery-ui*"));

         bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                     "~/Scripts/jquery.unobtrusive*",
                     "~/Scripts/jquery.validate*"));

         bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                     "~/Scripts/modernizr-*"));

         bundles.Add(new ScriptBundle("~/bundles/jquerymobile").Include("~/Scripts/jquery.mobile-1.2*"));

         bundles.Add(new StyleBundle("~/Content/css").Include(
                  "~/Content/site.css",
                  "~/Content/messages.css",
                  "~/Content/jquery.ui.datepicker.mobile.css"));         

         bundles.Add(new StyleBundle("~/Content/mobilecss_m").Include("~/Content/jquery.mobile-1.2*"));                  

         bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                     "~/Content/themes/try1.css",
                     "~/Content/themes/base/jquery.ui.core.css",
                     "~/Content/themes/base/jquery.ui.resizable.css",
                     "~/Content/themes/base/jquery.ui.selectable.css",
                     "~/Content/themes/base/jquery.ui.accordion.css",
                     "~/Content/themes/base/jquery.ui.autocomplete.css",
                     "~/Content/themes/base/jquery.ui.button.css",
                     "~/Content/themes/base/jquery.ui.dialog.css",
                     "~/Content/themes/base/jquery.ui.slider.css",
                     "~/Content/themes/base/jquery.ui.tabs.css",
                     "~/Content/themes/base/jquery.ui.datepicker.css",
                     "~/Content/themes/base/jquery.ui.progressbar.css",
                     "~/Content/themes/base/jquery.ui.theme.css"));
      }
   }
}