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
    public class WorkingPointsController : Controller
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
       #endregion

    }
}
