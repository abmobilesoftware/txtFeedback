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
      private const string cCompanyConfigurators = "CompanyConfigurator";

      smsfeedbackEntities context = new smsfeedbackEntities();

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
               new ReportsMenuItem(1, Resources.Global.settingUserPreferences, false, 0, "","UserPreferences"),
               new ReportsMenuItem(20, Resources.Global.settingsPrivacy, false, 0, "","Privacy"),
               new ReportsMenuItem(21, Resources.Global.settingsChangePassword, true, 20, "ConfigurePassword","ChangePassword"),
           };
           if (HttpContext.User.IsInRole(cRoleForConfigurators))
           {
              reportsMenuItems.Add(new ReportsMenuItem(30, Resources.Global.settingsWpMenuName, false, 0, "",""));
              reportsMenuItems.Add(new ReportsMenuItem(31, Resources.Global.settingsWpDefineWpsMenu, true, 30, "ConfigureWorkingPoints","WorkingPoints"));              
           };
           if (HttpContext.User.IsInRole(cCompanyConfigurators))
           {
              reportsMenuItems.Add(new ReportsMenuItem(40, Resources.Global.settingsCompanyMenuName, false, 0, "",""));
              reportsMenuItems.Add(new ReportsMenuItem(41, Resources.Global.settingsCompanyBillingMenuName, true, 40, "ConfigureCompanyBillingInfo","BillingInfo"));
           }

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
         return View(SMSRepository.GetWorkingPointsPerUser(user, context));
      }

      [CustomAuthorizeAtribute(Roles = cRoleForConfigurators)]
      [HttpPost]      
      public ActionResult GetDefineWorkingPointsForm(List<SmsFeedback_Take4.Models.WorkingPoint> wps)
      {
         if (ModelState.IsValid)
         {
            var user = User.Identity.Name;
            mEFInterface.SaveWpsForUser(user, wps, context);
            //ModelState.AddModelError("", Resources.Global.loginUnsuccessfulDetails);
            ViewData["saveMessage"] = Resources.Global.settingWpConfigSavedSuccessfuly;           
         }
         return GetDefineWorkingPointsFormInternal();
      }
       #endregion

      #region "Billing info"
      [CustomAuthorizeAtribute(Roles = cCompanyConfigurators)]
      public ActionResult CompanyBillingInfo()
      {
         var userName = User.Identity.Name;
         var user = (from u in context.Users where u.UserName == userName select u).FirstOrDefault();
         return View(user.Company.SubscriptionDetail); 
      }
      
      [CustomAuthorizeAtribute(Roles = cCompanyConfigurators)]
      [HttpPost]
      [ValidateInput(false)]
      public ActionResult CompanyBillingInfo(SubscriptionDetail details)
      {
         if (ModelState.IsValid)
         {                        
            ViewData["saveMessage"] = Resources.Global.settingWpConfigSavedSuccessfuly;
         }
         return View(details);
      }
      #endregion

      protected override void Dispose(bool disposing)
      {
         context.Dispose();
         base.Dispose(disposing);
      }

    }
   
}
