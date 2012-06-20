using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
         return RemovePrefixFromNumber(from) + cIDSeparator + RemovePrefixFromNumber(to);
      }

      public static string RemovePrefixFromNumber(string number)
      {
         string[] prefixes = { "00", "\\+" };
         string pattern = "^(" + String.Join("|", prefixes) + ")";

         Regex rgx = new Regex(pattern);
         return rgx.Replace(number, "");
      }
   }
}