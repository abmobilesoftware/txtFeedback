using SmsFeedback_EFModels;
using SmsFeedback_Take4.Models;
using SmsFeedback_Take4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
    public class WorkingPointsController : BaseController
    {
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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

       #region JSON interface
      public JsonResult WorkingPointsPerUser()
      {
         logger.Info("getting workingPoints per logged in user");
         try
         {
            if (HttpContext.Request.IsAjaxRequest())
            {
               var userId = User.Identity.Name;

               var workingPoints = SMSRepository.GetWorkingPointsPerUser(userId, context);
               return Json(workingPoints, JsonRequestBehavior.AllowGet);
            }
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in WorkingPointsPerUser", ex);
            return null;
         }
         return null;
      }

      protected override void Dispose(bool disposing)
      {
         context.Dispose();
         base.Dispose(disposing);
      }

      [CustomAuthorizeAtribute(Roles = SettingsController.cRoleForConfigurators)]
      [HttpGet]
      public ActionResult WorkingPointsInfo()
      {
         var userName = User.Identity.Name;
         var wps = (from u in context.Users where u.UserName == userName select u.WorkingPoints).SelectMany(x=>x);
         return View("WorkingPointsInfo", wps);
      }

      [HttpGet]
      [CustomAuthorizeAtribute(Roles = SettingsController.cRoleForConfigurators)]
      public ActionResult EditWorkingPointInfo(String wpId)
      {
         var wp = context.WorkingPoints.Find(wpId);
         return View("EditWorkingPointInfo", new SmsFeedback_Take4.Models.WorkingPoint(wp));
      }

      [HttpPost]
      [CustomAuthorizeAtribute(Roles = SettingsController.cRoleForConfigurators)]
      public ActionResult EditWorkingPointInfo(SmsFeedback_Take4.Models.WorkingPoint wp)
      {
         if (ModelState.IsValid)
         {
            //with the received info save to the db
            var w = context.WorkingPoints.Find(wp.TelNumber);
            if (w != null)
            {
               w.Name = wp.Name;
               w.Description = wp.Description;
               w.WelcomeMessage = wp.WelcomeMessage;
               w.BusyMessage = wp.BusyMessage;
               w.BusyMessageTimer = wp.BusyMessageTimer;
               context.SaveChanges();
            }
         }
         return View("EditWorkingPointInfo", wp);
      }
       #endregion

    }
}
