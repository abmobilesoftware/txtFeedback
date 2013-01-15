using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc.Mailer;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Mailers;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
    public class EmailSendController : BaseController
    {
       #region Private members and properties
       private static readonly log4net.ILog loggerEmailForm = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       UserMailer _mailer = new UserMailer();      
       #endregion

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
              string[] fromTo = ConversationUtilities.GetFromAndToFromConversationID(convID);
              smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
              var lWpTelNoOrShortID = fromTo[1];
              var lEfInteraction = new EFInteraction();
              var wpName = lEfInteraction.GetNameForWorkingPoint(lWpTelNoOrShortID, lContextPerRequest);
              if (String.IsNullOrEmpty(wpName))
              {
                 wpName = lWpTelNoOrShortID;
              }
              ViewData["emailSubject"] = Resources.Global.sendEmailPrefixSubject + fromTo[0] + Resources.Global.sendEmailConjuctionSubject + wpName + "]";
              result = View();
           }
           catch (Exception ex)
           {
              loggerEmailForm.Error("GetEmailMessageForm", ex);
           }
           return result;
        }

       [HttpGet]
       public ActionResult GetFeedbackForm(bool positiveFeedback, string url)
       {
          ViewData["emailText"] = Resources.Global.sendFeedbackPlaceholderMessage;
          string subject = Resources.Global.sendFeedbackPositiveFeedbackSubject;
          if (!positiveFeedback) subject = Resources.Global.sendFeedbackNegativeFeedbackSubject;
          ViewData["emailSubject"] = subject;
          ViewData["emailTo"] = "support@txtfeedback.net";
          ViewData["url"] = url;
          ViewResult res = View();
          return res;
       }

       [HttpPost]
       public ActionResult SendEmail(FormCollection formData)
        {
          //get the email address
          string email = formData["email"];
          string message = formData["message"];
          string subject = formData["subject"];
          string from = this.User.Identity.Name;
          bool isFeedbackForm = Boolean.Parse(formData["isFeedbackForm"]);          
          System.Net.Mail.MailMessage msg = _mailer.SendMessageContent(email, subject, message, from, formData["url"]);          
          msg.Send();          
          string msgSentAcknoledgement = Resources.Global.sendEmailEmailSentSuccessfuly;
          if (isFeedbackForm) msgSentAcknoledgement = Resources.Global.sendFeedbackFeedbackSentSuccessfully;
          return Json(msgSentAcknoledgement);
        }

      [HttpGet]
       public void SendWarningEmail(string emailAddress)
       {
          var mailer = new WarningMailer();
          var sd = new SubscriptionDetail() { BillingDay = 1, DefaultCurrency = "EUR", SpentThisMonth = 2, SpendingLimit= 3 };
          sd.Companies.Add(new Company() { Name = "Ab Mobile Apps" });
          var msg = mailer.WarningEmail(sd, emailAddress, "Dragos", "Andronic");
          msg.Send();
      } 
      
      [HttpGet]
      public void SendSpendingLimitReachedEmail(string emailAddress)
      {
         var mailer = new WarningMailer();
         var sd = new SubscriptionDetail() { BillingDay = 1, DefaultCurrency = "EUR", SpentThisMonth = 2, SpendingLimit = 3 };
         sd.Companies.Add(new Company() { Name = "Ab Mobile Apps" });
         var msg = mailer.SpendingLimitReachedEmail(sd, emailAddress, "Dragos", "Andronic");
         msg.Send();
      }
    }
}
