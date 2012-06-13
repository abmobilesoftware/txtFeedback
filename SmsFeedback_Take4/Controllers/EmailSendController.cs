using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Mailer;
using SmsFeedback_Take4.Mailers;

namespace SmsFeedback_Take4.Controllers
{    
    public class EmailSendController : BaseController
    {
       private static readonly log4net.ILog loggerEmailForm = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /EmailSend/

        public ActionResult Index()
        {
            return View();
        }

       [HttpGet]
        public ActionResult GetEmailMessageForm(string emailText, string convID)
        {
           ViewResult result = null;
           try
           {
              ViewData["emailText"] = emailText;
              string[] fromTo =convID.Split('-');
              ViewData["emailSubject"] = "Txt exchange between " + fromTo[0] + " and " + fromTo[1];
              result = View();
           }
           catch (Exception ex)
           {
              loggerEmailForm.Error("GetEmailMessageForm", ex);
           }
           return result;
        }
       private IUserMailer _mailer = new UserMailer();
       public IUserMailer Mailer
       {
          get { return _mailer; }
          set { _mailer = value; }
       }


       [HttpPost]
       public ActionResult SendEmail(FormCollection formData)
        {
          //get the email address
           string email = formData["email"];
          string message = formData["message"];
          string subject = formData["subject"];
          string from = this.User.Identity.Name;
          System.Net.Mail.MailMessage msg = Mailer.SendMessageContent(email, subject, message, from);

          //msg.From = new System.Net.Mail.MailAddress(System.Web.Security.Membership.GetUser(this.User.Identity.Name).Email);
          msg.Send();
          return Json("Sent successfully");
        }

    }
}
