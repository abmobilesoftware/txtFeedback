using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;

namespace SmsFeedback_Take4.Mailers
{ 
    public interface IUserMailer
    {
				
		MailMessage SendMessageContent(string email, string subject, string content,string from);
			
	}
}