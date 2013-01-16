using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace SmsFeedback_Take4.Mailers
{ 
    public class UserMailer : MailerBase     
	{
		public UserMailer():
			base()
		{
			MasterName="_Layout";
		}

		
		public virtual MailMessage SendMessageContent(string email, string subject, string content,string from, string location ="")
		{
			var mailMessage = new MailMessage{Subject = subject};			
			mailMessage.To.Add(email);
         ViewData["From"] = from;
         ViewData["Content"] = content;
         ViewData["Location"] = location;
			PopulateBody(mailMessage, viewName: "EmailTemplate");
			return mailMessage;
		}
      
	}
}