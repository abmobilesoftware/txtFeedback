using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Models;
using SmsFeedback_Take4.Models.BusinessObjects;
using SmsFeedback_Take4.Models.Helpers;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{   
   [CustomAuthorizeAtribute]
   public class SettingsController : BaseController
    {      
      private const string cRoleForConfigurators = "WorkingPointsConfigurator";

      private AggregateSmsRepository mSmsRepository;
      private AggregateSmsRepository SMSRepository
      {
         get
         {
            if (mSmsRepository == null)
               mSmsRepository = AggregateSmsRepository.GetInstance(User.Identity.Name);
            return mSmsRepository;
         }
      }
      private EFInteraction mEFInterface = new EFInteraction();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMenuItems()
        {
           List<ReportsMenuItem> reportsMenuItems = new List<ReportsMenuItem> {
               new ReportsMenuItem(1, Resources.Global.settingUserPreferences, false, 0),
              new ReportsMenuItem(20, Resources.Global.settingsPrivacy, false, 0),
              new ReportsMenuItem(21, Resources.Global.settingsChangePassword, true, 20),
           };
           if (HttpContext.User.IsInRole(cRoleForConfigurators))
           {
              reportsMenuItems.Add(new ReportsMenuItem(30, "Working points", false, 0));
              reportsMenuItems.Add(new ReportsMenuItem(31, "Define your working points", true, 30));              
           };
           return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);           
        }
        #region Change password
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
                 MembershipUser currentUser = System.Web.Security.Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
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
        #endregion

       #region Define working points
      [CustomAuthorizeAtribute(Roles = cRoleForConfigurators)]
        public ActionResult GetDefineWorkingPointsForm()
        {
           return GetDefineWorkingPointsFormInternal();      
        }

      private ActionResult GetDefineWorkingPointsFormInternal()
      {
         var user = User.Identity.Name;          
         smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();         
         return View(SMSRepository.GetWorkingPointsPerUser(user, lContextPerRequest));
      }

      [CustomAuthorizeAtribute(Roles = cRoleForConfigurators)]
      [HttpPost]      
      public ActionResult GetDefineWorkingPointsForm(List<SmsFeedback_Take4.Models.WorkingPoint> wps)
      {
            var user = User.Identity.Name;
            smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
            mEFInterface.SaveWpsForUser(user,wps, lContextPerRequest);
            //ModelState.AddModelError("", Resources.Global.loginUnsuccessfulDetails);
            ViewData["saveMessage"] = Resources.Global.settingWpConfigSavedSuccessfuly ;
            return GetDefineWorkingPointsFormInternal();
      }
       #endregion
    }
   
}
