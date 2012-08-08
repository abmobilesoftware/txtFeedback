using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
   public class HelpController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
