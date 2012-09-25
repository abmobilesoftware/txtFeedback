using SmsFeedback_Take4.Models;
using SmsFeedback_Take4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Nexmo
{
   [CustomAuthorizeAtribute(Roles="SenderOfSms")]
    public class NexmoTestController : Controller
    {
        //
        // GET: /NexmoTest/

        public ActionResult Index()
        {
           //get the list of numbers from the db and display it (to ease the work of the users)
           SmsFeedback_EFModels.smsfeedbackEntities dbCon = new SmsFeedback_EFModels.smsfeedbackEntities();
           var wps = dbCon.WorkingPoints.Select(c => new WorkingPoint { Name= c.Name, TelNumber= c.TelNumber }).ToList();       
            return View(wps);
        }

        public JsonResult SendMessage(string from, string to, string msgText)
        {
           if (HttpContext.Request.IsAjaxRequest())
           {
              SmsFeedback_Take4.Models.NexmoSmsRepository.StaticSendMessage(from, to, msgText, dateSent => {
                 
              });
              return Json("success", JsonRequestBehavior.AllowGet);
           }
           return Json("Ajax requests only",JsonRequestBehavior.AllowGet);
        }
    }
}
