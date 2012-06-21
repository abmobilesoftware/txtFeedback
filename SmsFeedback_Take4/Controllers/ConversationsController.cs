using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Controllers
{    
    public class ConversationsController : BaseController
    {
       
       // GET: /Conversations/

       public ActionResult Index()
       {
          return View();
       }
       
    }
}
