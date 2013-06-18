using System.Web;
using System.Web.Optimization;

namespace SmsFeedback_Take4
{
   public class BundleConfig
   {
      // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
      public static void RegisterBundles(BundleCollection bundles)
      {
         BundleTable.EnableOptimizations = false;
         //master layout
         //css
         bundles.Add(new ScriptBundle("~/bundles/corejs").Include(
            "~/Scripts/persist-min.js",
            "~/Scripts/Strophe/strophe.js",
            "~/MyScripts/Utilities.js",
            "~/MyScripts/XMPP.js",
            "~/Scripts/jquery.cookie.js",
            "~/Scripts/jquery.simplemodal.js",
            "~/MyScripts/contact.js",
            "~/MyScripts/Transition.js"
       ));
         //js
         bundles.Add(new StyleBundle("~/Content/masterCss").Include(
            "~/Content/reset.css",
            "~/Content/text.css",
            "~/Content/grid.css",
            "~/Content/layout.css",
            "~/Content/nav.css",
            "~/Content/Site.css",
            "~/Content/contact.css",
            "~/Content/css/areaDefinitions.css",
            "~/Content/css/bootstrap.css"));
         //conversations tab
         //css
         bundles.Add(new StyleBundle("~/Content/homeCss").Include(
            "~/Content/phonenumbers.css",
            "~/Content/messages.css",
            "~/Content/conversations.css",
            "~/Content/filtersStrip.css",
            "~/Content/tags.css",            
            "~/Content/jquery.tagsinput.css",
             "~/Content/quickActionBtns.css"));
         //js
         bundles.Add(new ScriptBundle("~/bundles/homejs").Include(
            "~/Scripts/spin.js",
            "~/Scripts/jquery.tagsinput.js",
            "~/Scripts/jquery.ui.datepicker-de.js",
            "~/Scripts/jquery.ui.datepicker-ro.js",
            "~/Scripts/jquery.ui.datepicker-en-GB.js",
            "~/Scripts/jquery.ui.datepicker-es.js",
            "~/MyScripts/WorkingPoints.js",
            "~/MyScripts/Messages.js",
            "~/MyScripts/Conversations.js",
            "~/MyScripts/Filtering.js",
            "~/MyScripts/ConversationTags.js",
            "~/MyScripts/Facade.js",
            "~/MyScripts/QuickActions.js"));
         //Reports tab
         //css
         bundles.Add(new StyleBundle("~/Content/reportsCss").Include(
            "~/Content/css/reports.css",
            "~/Content/tags.css",
             "~/Content/jquery.tagsinput.css"
            ));
         //js
         bundles.Add(new ScriptBundle("~/bundles/reportsjs").Include(
            "~/Scripts/spin.js", "~/Scripts/jquery.ui.datepicker-de.js",
            "~/Scripts/jquery.ui.datepicker-ro.js",
            "~/Scripts/jquery.ui.datepicker-en-GB.js",
            "~/Scripts/jquery.ui.datepicker-es.js",
            "~/MyScripts/Helpers/Debounce.js",
            "~/MyScripts/Reports/DateHelper.js",
            "~/MyScripts/Reports/GlobalVariables.js",
            "~/MyScripts/Reports/Json2Csv.js",
            "~/MyScripts/Reports/FirstArea.js",
            "~/MyScripts/Reports/SecondArea.js",
            "~/MyScripts/Reports/ThirdArea.js",
            "~/MyScripts/Base/BaseLeftSideMenu.js",
            "~/MyScripts/Reports/Reports.js",
            "~/MyScripts/Reports/RepFacade.js",
            "~/Scripts/CollapsibleLists.js",
            "~/Scripts/jquery.tagsinput.js"
            ));
         //Settings tab
         //css
         bundles.Add(new StyleBundle("~/Content/settingsCss").Include(
            "~/Content/css/settings.css"
            ));
         //js
         bundles.Add(new ScriptBundle("~/bundles/settingsjs").Include(
            "~/Scripts/CollapsibleLists.js",
            "~/MyScripts/Base/BaseLeftSideMenu.js",
            "~/MyScripts/Settings/settings.js",
            "~/MyScripts/Settings/SettingsFacade.js"));
         //LogOn
         //css
         bundles.Add(new StyleBundle("~/Content/logonCss").Include(
            "~/Content/themes/base/jquery.ui.all.css",
            "~/Content/reset.css",
            "~/Content/text.css",
            "~/Content/grid.css",
            "~/Content/layout.css",
            "~/Content/nav.css",
            "~/Content/Site.css"));            
         //js
         bundles.Add(new ScriptBundle("~/bundles/logonjs").Include(
            "~/Scripts/jquery-1.6.2.min.js",
            "~/Scripts/jquery.validate.min.js",
            "~/Scripts/jquery.validate.unobtrusive.min.js"
            ));
      }
   }
}