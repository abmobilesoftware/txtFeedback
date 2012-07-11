using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
   public class XmppController : BaseController
   {
      #region "member variables"
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private EFInteraction mEFInterface = new EFInteraction();
      #endregion

      public JsonResult GetConnectionDetailsForLoggedInUser()
      {
         logger.Info("getting xmmpConn details per logged in user");
         try
         {
            if (HttpContext.Request.IsAjaxRequest())
            {
               var userId = User.Identity.Name;
               smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
               using (lContextPerRequest)
               {
                  var connectionDetails = mEFInterface.GetXmppConnectionDetailsPerUser(userId, lContextPerRequest);
                  return Json(connectionDetails, JsonRequestBehavior.AllowGet);   
               }
            }
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in GetConnectionDetailsForLoggedInUser", ex);
            return null;
         }
         return null;
      }
   }
}
