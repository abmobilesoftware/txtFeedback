using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mobile.TxtFeedback_take1.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult General()
        {
           return View();
        }
        public ActionResult Error404()
        {
           return View();
        }
    }
}
