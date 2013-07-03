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
      internal const string cRoleForConfigurators = "WorkingPointsConfigurator";
      internal const string cCompanyConfigurators = "CompanyConfigurator";

      smsfeedbackEntities context = new smsfeedbackEntities();
      private static readonly log4net.ILog logger = 
         log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
               new ReportsMenuItem(1, Resources.Global.settingUserPreferences, false, 0, "", "UserPreferences"),
               new ReportsMenuItem(2, Resources.Global.settingsNotifications, true, 1, "ConfigureNotifications", "Notifications"),
               new ReportsMenuItem(20, Resources.Global.settingsPrivacy, false, 0, "","Privacy"),
               new ReportsMenuItem(21, Resources.Global.settingsChangePassword, true, 20, "ConfigurePassword","ChangePassword")               
           };
         if (HttpContext.User.IsInRole(cRoleForConfigurators))
         {
            reportsMenuItems.Add(new ReportsMenuItem(30, Resources.Global.settingsWpMenuName, false, 0, "", ""));
            reportsMenuItems.Add(new ReportsMenuItem(31, Resources.Global.settingsWpDefineWpsMenu, true, 30, "ConfigureWorkingPoints", "WorkingPoints"));
         };
         if (HttpContext.User.IsInRole(cCompanyConfigurators))
         {
            reportsMenuItems.Add(new ReportsMenuItem(40, Resources.Global.settingsCompanyMenuName, false, 0, "", ""));
            reportsMenuItems.Add(new ReportsMenuItem(41, Resources.Global.settingsCompanyBillingMenuName, true, 40, "ConfigureCompanyBillingInfo", "BillingInfo"));
         }

         return Json(reportsMenuItems, JsonRequestBehavior.AllowGet);
      }
      #region Change password
      public ActionResult GetChangePasswordForm()
      {
         return View();
      }

      [HttpPost]
        [ValidateInput(false)]
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
         context.Entry(user.Company.SubscriptionDetail).Reference(u => u.PrimaryContact).Load();
         context.Entry(user.Company.SubscriptionDetail).Reference(u => u.SecondaryContact).Load();
         return View(user.Company.SubscriptionDetail);
      }

      [CustomAuthorizeAtribute(Roles = cCompanyConfigurators)]
      [HttpPost]
      public ActionResult CompanyBillingInfo(SubscriptionDetail details)
      {
         if (ModelState.IsValid)
         {

            ViewBag.SaveMessage = Resources.Global.settingWpConfigSavedSuccessfuly;
            var sd = context.SubscriptionDetails.Find(details.Id);
            context.Entry(sd).CurrentValues.SetValues(details);
            var primaryContact = context.Contacts.Find(details.PrimaryContact_Id);
            if (primaryContact != null)
            {
               context.Entry(primaryContact).CurrentValues.SetValues(details.PrimaryContact);
            }
            //in case they are pointing to same contact            
            var secondaryContact = context.Contacts.Find(details.SecondaryContact_Id);

            if (secondaryContact != null)
            {
               context.Entry(secondaryContact).CurrentValues.SetValues(details.SecondaryContact);
            }
            context.SaveChanges();
         }

         return View(details);
      }
      #endregion

      #region Notifications
      public ActionResult NotificationsPage()
      {
         var username = User.Identity.Name;
         var user = (from u in context.Users
                     where u.UserName.Equals(username) 
                     select u).FirstOrDefault();
         ViewData["notification"] = "noNotification";
         return View(user);
      }

      public ActionResult SaveNotificationsSettings(
         string typeOfActivityReport, 
         bool soundNotificationsEnabled)
      {
         User user = null;
         try
         {
            var username = User.Identity.Name;
            user = (from u in context.Users
                        where u.UserName.Equals(username)
                        select u).FirstOrDefault();
            user.ActivityReportDelivery = typeOfActivityReport;
            user.SoundNotifications = soundNotificationsEnabled;
            context.SaveChanges();
            ViewData["notification"] = "success";
            ViewData["activityReportChose"] = typeOfActivityReport;
            return View(user);
         }
         catch (Exception e)
         {
            ViewData["notification"] = "error";
            logger.Error("SaveNotificationsSettings " + e.Message);
            return View(user);
         }
      }

      public JsonResult AreSoundNotificationsOn()
      {
         var username = User.Identity.Name;
         var user = (from u in context.Users
                 where u.UserName.Equals(username)
                 select u).FirstOrDefault();
         return Json(user.SoundNotifications ? 
            "enabled" : "disabled", JsonRequestBehavior.AllowGet);
      }

      #endregion

      protected override void Dispose(bool disposing)
      {
         context.Dispose();
         base.Dispose(disposing);
      }

   }

}
