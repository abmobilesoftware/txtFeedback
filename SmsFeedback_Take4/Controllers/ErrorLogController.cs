using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Controllers
{
    [Authorize]
    public class ErrorLogController : BaseController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       [HttpPost]
        public ActionResult LogError(string errorMsg, string context)
        {
           logger.ErrorFormat("Client side error: [{0}], context: [{1}]", errorMsg,context);
           return new EmptyResult();
        }

       [HttpPost]
       public ActionResult LogDebug(string errorMsg, string context)
       {
          logger.DebugFormat("Client side debug: [{0}], context: [{1}]", errorMsg, context);
          return new EmptyResult();
       }
    }
}
