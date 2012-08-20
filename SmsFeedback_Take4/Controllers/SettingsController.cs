using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SmsFeedback_Take4.Models;
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
              new ReportsMenuItem(1, Resources.Global.settingUserPreferences, false, 0),
              new ReportsMenuItem(20, Resources.Global.settingsPrivacy, false, 0),
              new ReportsMenuItem(21, Resources.Global.settingsChangePassword, true, 20)
              
           };           
           
           return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetChangePasswordForm()
        {
           return View();
        }
      
        [HttpPost]
        public ActionResult GetChangePasswordForm(ChangePasswordModel model)
        {
           if (ModelState.IsValid)
           {
              // ChangePassword will throw an exception rather
              // than return false in certain failure scenarios.
              bool changePasswordSucceeded;
              try
              {
                 MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                 changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
              }
              catch (Exception)
              {
                 changePasswordSucceeded = false;
              }

              if (changePasswordSucceeded)
              {
                 return RedirectToAction("ChangePasswordSuccess");
              }
              else
              {
                 ModelState.AddModelError("", Resources.Global.settingsErrorPasswordIncorrect);
              }
           }
           // If we got this far, something failed, redisplay form
           return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
           return View();
        }
    }

}
