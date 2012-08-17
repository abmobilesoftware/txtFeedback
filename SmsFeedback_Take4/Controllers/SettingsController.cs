using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Models.Helpers;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
   public class SettingsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMenuItems()
        {
           ReportsMenuItem[] reportsMenuItems = new ReportsMenuItem[] { 
              new ReportsMenuItem(1, Resources.Global.settingsPrivacy, false, 0),
              new ReportsMenuItem(2, Resources.Global.settingsChangePassword, true, 1),
              new ReportsMenuItem(3, Resources.Global.settingUserPreferences, false, 0)
           };           
           
           return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
        }
    }
}
