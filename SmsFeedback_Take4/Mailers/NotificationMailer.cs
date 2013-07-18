using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace SmsFeedback_Take4.Mailers
{
   public class NotificationMailer : MailerBase
   {
      public NotificationMailer() :
         base()
      {
         MasterName = "_Layout";
      }

      public MailMessage BuildNotificationMail(string to,
         string from,
         string subject,
         string message,
         string wpName,
         string language,
         bool isSms,
         DateTime messageDate)
      {
         var mailMessage = new MailMessage
         {
            Subject = subject
         };
         mailMessage.To.Add(to);
         ViewData["conversation-text"] = message;
         ViewData["conversation-wp"] = wpName;
         ViewData["conversation-date"] = messageDate;
         ViewData["conversation-from"] = from;
         ViewData["language"] = language;
         ViewData["isSms"] = isSms;
         PopulateBody(mailMessage, viewName: "MailNotificationTemplate");
         return mailMessage;
      }
   }
}