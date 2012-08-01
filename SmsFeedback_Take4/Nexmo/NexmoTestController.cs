using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Nexmo
{
    public class NexmoTestController : Controller
    {
        //
        // GET: /NexmoTest/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SendMessage(string msgText)
        {
           if (HttpContext.Request.IsAjaxRequest())
           {
              var from = "4915706100037"; 
              var to = "4915706100034";
              SmsFeedback_Take4.Models.NexmoSmsRepository.StaticSendMessage(from, to, msgText, dateSent => {
                 
              });
              return Json("success", JsonRequestBehavior.AllowGet);
           }
           return Json("Ajax requests only",JsonRequestBehavior.AllowGet);
        }
    }
}
