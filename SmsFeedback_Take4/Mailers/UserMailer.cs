using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;
using SmsFeedback_Take4.Models.Helpers;

namespace SmsFeedback_Take4.Mailers
{ 
    public class UserMailer : MailerBase     
	{
		public UserMailer():
			base()
		{
			MasterName="_Layout";
		}

		
		public virtual MailMessage SendMessageContent(string email, 
         string subject, 
         string content,
         string from, 
         string location ="")
		{
			var mailMessage = new MailMessage{Subject = subject};			
			mailMessage.To.Add(email);
         ViewData["From"] = from;
         ViewData["Content"] = content;
         ViewData["Location"] = location;
			PopulateBody(mailMessage, viewName: "EmailTemplate");
			return mailMessage;
		}

      public virtual MailMessage BuildNewsletterMail(string to, 
         string subject,
         List<WpAndConversations> conversations, 
         DateTime startDate, 
         DateTime endDate) {
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