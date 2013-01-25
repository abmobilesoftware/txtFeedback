using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
   public class SubscriptionSmsStatus
   {
      public bool MessageSent { get; set; }
      public int MessageID { get; set; }
      public bool WarningLimitReached { get; set; }
      public bool SpendingLimitReached { get; set; }
      public String Reason { get; set; }
      
      public SubscriptionSmsStatus(int msgID, bool msgSent, bool warningLimitReached, bool spendingLimitReached, string reason = "")
      {
         MessageID = msgID;
         MessageSent = msgSent;
         WarningLimitReached = warningLimitReached;
         SpendingLimitReached = spendingLimitReached;
         Reason = reason;
      }
   }
}