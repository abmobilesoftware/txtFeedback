using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4;
namespace SmsFeedback_Take4.Controllers
{
   [HandleError]      
    public class HomeController : BaseController
   {
      public ActionResult Index()
      {
         ViewBag.Message =Resources.Global.welcomeMessage;

         return View();
      }

      public ActionResult About()
      {
         return View();
      }

      
   }
}
