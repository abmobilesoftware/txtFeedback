using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Mailers;
using Mvc.Mailer;

namespace SmsFeedback_Take4.Controllers
{
    public class ActivityReportController : Controller
    {
       private ActivityReportMailer _mailer = new ActivityReportMailer();
       
       [HttpGet]
       public void SendActivityReport()
       {
          smsfeedbackEntities dbContext = new smsfeedbackEntities();
          EFInteraction efInteraction = new EFInteraction();
          DateTime today = DateTime.UtcNow;
          DateTime sevenDaysAgo = today.AddDays(-7);

          var wpsAndConversations = efInteraction.GetWorkingPointsAndConversations(
             User.Identity.Name,
             sevenDaysAgo,
             today,
             dbContext);
          var conversationsGroupedByUser = wpsAndConversations.GroupBy(x => x.user);
          foreach (var conversationsGroup in conversationsGroupedByUser)
          {
             var noOfWorkingPointsWithUnreadConversations = conversationsGroup.ToList().
                Where(x => x.conversations.Count() > 0).Count();
             var hasActivity = noOfWorkingPointsWithUnreadConversations > 0 ? true : false;
             if (hasActivity)
             {
                System.Net.Mail.MailMessage newsletter = 
                   _mailer.BuildActivityReportMail("mihai@txtfeedback.net",
                 "Weekly newsletter",
                 conversationsGroup.ToList(),
                 sevenDaysAgo,
                 today
                 );
                newsletter.Send();
             }
          }
       }

    }
}
