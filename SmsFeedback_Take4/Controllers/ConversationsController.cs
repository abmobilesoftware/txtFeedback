using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Models.ViewModels ;

namespace SmsFeedback_Take4.Controllers
{    
    public class ConversationsController : BaseController
    {
       private MessagesContext mMsgContext = MessagesContext.getInstance();
       //
       // GET: /Conversations/

       public ActionResult Index()
       {
          return View();
       }
       
    }
}
