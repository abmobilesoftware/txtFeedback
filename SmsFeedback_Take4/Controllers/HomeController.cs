using SmsFeedback_Take4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
   public class HomeController : BaseController
    {

       public ActionResult Index()
       {
          ViewData["currentCulture"] = getCurrentCulture();
          ViewData["messageOrganizer"] = HttpContext.User.IsInRole(MessagesController.cMessageOrganizer);
          return View();
       }

    }
}
