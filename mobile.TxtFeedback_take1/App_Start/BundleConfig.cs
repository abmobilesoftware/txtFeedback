using System.Web;
using System.Web.Optimization;

namespace mobile.TxtFeedback_take1
{
   public class BundleConfig
   {
      // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
      public static void RegisterBundles(BundleCollection bundles)
      {
         bundles.Add(new ScriptBundle("~/bundles/myscripts").Include(                     
                     "~/MyScripts/Utilities.js",
                     "~/MyScripts/XMPP.js",
                     "~/MyScripts/strophe.js",
                     "~/MyScripts/Home.js"));

         bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                     "~/Scripts/jQuery.ui.datepicker.js", 
                     "~/Scripts/jquery.ui.datepicker.mobile.js",
                     "~/Scripts/jquery.ui.datepicker-de.js",
                     "~/Scripts/jquery.ui.datepicker-en-GB.js",
                     "~/Scripts/jquery.ui.datepicker-ro.js",
                     "~/Scripts/jquery.ui.datepicker-es.js"));

         bundles.Add(new StyleBundle("~/bundles/mycss").Include(
                     "~/Content/messages.css"));
        
         bundles.Add(new ScriptBundle("~/bundles/backbone").Include(
                     "~/Scripts/underscore*",
                     "~/Scripts/backbone*"));
         bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-1.*"));

         bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                     "~/Scripts/jquery-ui*"));

         bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                     "~/Scripts/jquery.unobtrusive*",
                     "~/Scripts/jquery.validate*"));

         bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                     "~/Scripts/modernizr-*"));

         bundles.Add(new ScriptBundle("~/bundles/jquerymobile").Include("~/Scripts/jquery.mobile*"));

         bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

         bundles.Add(new StyleBundle("~/Content/mycss").Include("~/Content/messages.css"));        

         bundles.Add(new StyleBundle("~/Content/mobilecss").Include("~/Content/jquery.mobile*"));
         
         bundles.Add(new StyleBundle("~/Content/datepickercss").Include("~/Content/jquery.ui.datepicker.mobile.css"));

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