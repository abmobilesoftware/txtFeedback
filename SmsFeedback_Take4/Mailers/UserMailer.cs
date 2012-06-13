using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace SmsFeedback_Take4.Mailers
{ 
    public class UserMailer : MailerBase, IUserMailer     
	{
		public UserMailer():
			base()
		{
			MasterName="_Layout";
		}

		
		public virtual MailMessage SendMessageContent(string email, string subject, string content,string from)
		{
			var mailMessage = new MailMessage{Subject = subject};			
			mailMessage.To.Add(email);
         ViewData["From"] = from;
         ViewData["Content"] = content;  
			PopulateBody(mailMessage, viewName: "Welcome");
			return mailMessage;
		}
       		
	}
}