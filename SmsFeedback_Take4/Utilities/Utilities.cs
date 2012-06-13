using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Utilities
{
   public class ConversationUtilities
   {
      private const char cIDSeparator = '-';
      public static string[] GetFromAndToFromConversationID(string conversationID)
      {
         return conversationID.Split(cIDSeparator);
      }

      public static string BuildConversationIDFromFromAndTo(string from, string to)
      {
         return from + cIDSeparator + to;
      }
   }
}