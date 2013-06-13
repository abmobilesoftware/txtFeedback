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
      /**
       * TODO: change the logic this way: look for user
       * with the daily or weekly digest option enabled, 
       * then for each user extract the pair wp and the unread 
       * conversations.
       * 
       * Current implementation: starts from current unread 
       * conversations and goes to working point and then user.
       */ 
      [HttpGet]
      public void SendActivityReport()
      {
         String ACTIVITY_REPORT_DAILY = "Daily digest";
         String ACTIVITY_REPORT_WEEKLY = "Weekly digest";
         DateTime endOfInterval = DateTime.UtcNow;
         DateTime startOfInterval = DateTime.MinValue;
         smsfeedbackEntities dbContext = new smsfeedbackEntities();
         EFInteraction efInteraction = new EFInteraction();
         var wpsAndConversations = efInteraction.GetWorkingPointsAndConversations(
            startOfInterval,
            endOfInterval,
            dbContext);
         var conversationsGroupedByUser = wpsAndConversations.GroupBy(x => x.user);
         foreach (var conversationsGroup in conversationsGroupedByUser)
         {            
            bool sendActivityReport = false;
            SmsFeedback_EFModels.User user = conversationsGroup.First().user;
            if (user.ActivityReportDelivery.Equals(ACTIVITY_REPORT_DAILY))
            {
               startOfInterval = endOfInterval.Date;
               sendActivityReport = true;
            }
            else if (user.ActivityReportDelivery.Equals(ACTIVITY_REPORT_WEEKLY)
             && endOfInterval.DayOfWeek.Equals(DayOfWeek.Monday))
            {
               sendActivityReport = true;
               startOfInterval = endOfInterval.AddDays(-7);
            }
            if (sendActivityReport)
            {
               var noOfWorkingPointsWithUnreadConversations = conversationsGroup.ToList().
               Where(x => x.conversations.Count() > 0).Count();
               var hasActivity = noOfWorkingPointsWithUnreadConversations > 0
                  ? true : false;
               if (hasActivity)
               {

                  System.Net.Mail.MailMessage newsletter =
                     _mailer.BuildActivityReportMail(
                     user.Membership.Email,
                   "Weekly newsletter",
                   conversationsGroup.ToList(),
                   startOfInterval,
                   endOfInterval
                   );
                  newsletter.Send();
               }
            }
         }
      }
   }
}
