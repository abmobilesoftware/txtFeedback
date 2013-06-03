using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
   public class WpAndConversations
   {
      public WorkingPoint workingPoint;
      public IEnumerable<SmsMessage> conversations;

      public WpAndConversations(WorkingPoint iWorkingPoint, IEnumerable<SmsMessage> iConversations)
      {
         workingPoint = iWorkingPoint;
         conversations = iConversations;
      }
   }
}