using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
   public class XmppController : BaseController
    {
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private EFInteraction mEFInterface = new EFInteraction();    
        //
        // GET: /Xmpp/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetConnectionDetailsForLoggedInUser()
        {
           logger.Info("getting xmmpConn details per logged in user");
           try
           {
              if (HttpContext.Request.IsAjaxRequest())
              {
                 var userId = User.Identity.Name;
                 var connectionDetails = mEFInterface.GetXmppConnectionDetailsPerUser(userId);
                 return Json(connectionDetails, JsonRequestBehavior.AllowGet);
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
