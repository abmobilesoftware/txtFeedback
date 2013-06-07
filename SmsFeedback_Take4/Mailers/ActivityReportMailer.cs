using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using SmsFeedback_Take4.Models.Helpers;
using System.Net.Mail;

namespace SmsFeedback_Take4.Mailers
{
   public class ActivityReportMailer : MailerBase
   {
      public ActivityReportMailer() :
         base()
      {
         MasterName = "_Layout";
      }

      public virtual MailMessage BuildActivityReportMail(string to,
         string subject,
         List<WpAndConversations> conversations,
         DateTime startDate,
         DateTime endDate)
      {
         var mailMessage = new MailMessage { Subject = subject };
         mailMessage.To.Add(to);
         ViewData["conversations"] = conversations;
         ViewData["startDate"] = startDate;
         ViewData["endDate"] = endDate;
         PopulateBody(mailMessage, viewName: "NewsletterTemplate");
         return mailMessage;
      }
   }
}