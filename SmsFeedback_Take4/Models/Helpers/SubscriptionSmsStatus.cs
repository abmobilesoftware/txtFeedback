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
      public string WarningLimitReachedMessage { get; set; }
      public string SpendingLimitReachedMessage { get; set; }
      
      public SubscriptionSmsStatus(bool msgSent, int msgID, bool warningLimitReached, bool spendingLimitReached, string warningMSG = "", string spendingMSG= "")
      {
         MessageSent = msgSent;
         MessageID = msgID;
         WarningLimitReached = warningLimitReached;
         SpendingLimitReached = spendingLimitReached;
         WarningLimitReachedMessage = warningMSG;
         SpendingLimitReachedMessage = spendingMSG;
      }
   }
}