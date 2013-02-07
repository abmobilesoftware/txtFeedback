using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;
using SmsFeedback_EFModels;

namespace SmsFeedback_Take4.Mailers
{ 
    public class WarningMailer : MailerBase
	{
		public WarningMailer():
			base()
		{
			MasterName="_Layout";
		}
    
      public virtual MailMessage WarningEmail(SubscriptionDetail sd, string toEmailAddress, string name, string surname)
      {
         //TODO DA make this localizable
         var mailMessage = new MailMessage { Subject = "Approaching spending limit for your company" };
         mailMessage.Priority = MailPriority.High;
         mailMessage.To.Add(new MailAddress(toEmailAddress, name + " " + surname));
         mailMessage.To.Add(toEmailAddress);         
         ViewBag.CompanyName = sd.Companies.FirstOrDefault().Name;
         ViewBag.DearSir = "Mr./Ms. " + surname;
         var today = DateTime.Now;
         ViewBag.ResetDate = sd.GetNextBillingDate(today).ToLongDateString();
         ViewData.Model = sd;
         PopulateBody(mailMessage, viewName: "WarningEmail");
         return mailMessage;
      }

      public virtual MailMessage SpendingLimitReachedEmail(SubscriptionDetail sd, string toEmailAddress, string name, string surname)
      {
         var mailMessage = new MailMessage { Subject = "Spending limit reached for your company" };
         mailMessage.Priority = MailPriority.High;
         mailMessage.To.Add(new MailAddress(toEmailAddress, name + " " + surname));
         mailMessage.To.Add(toEmailAddress);
         ViewBag.CompanyName = sd.Companies.FirstOrDefault().Name;
         ViewBag.DearSir = "Mr./Ms. " + surname;
         var today = DateTime.Now;
         ViewBag.ResetDate = sd.GetNextBillingDate(today).ToLongDateString();
         ViewData.Model = sd;
         PopulateBody(mailMessage, viewName: "SpendingLimitReachedEmail");
         return mailMessage;
      }

	}
}