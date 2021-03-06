﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace mobile.TxtFeedback_take1.Utilities
{
   public class ConversationUtilities
   {
      public static Random RandomNumberGenerator = new Random();
      private const char cIDSeparator = '-';
      public static string[] GetFromAndToFromConversationID(string conversationID)
      {
         return conversationID.Split(cIDSeparator);
      }

      public static string BuildConversationIDFromFromAndTo(string from, string to)
      {         
         return CleanUpPhoneNumber(from) + cIDSeparator + CleanUpPhoneNumber(to);
      }

      public static string CleanUpPhoneNumber(string number)
      {
         string[] prefixes = { "00", "\\+" , "@"};
         string pattern = "^(" + String.Join("|", prefixes) + ")";

         Regex rgx = new Regex(pattern);
         return rgx.Replace(number, "");
      }
      
      public enum Direction { 
         from = 0,
         to = 1
      }

      public static Direction GetDirectionForMessage(string latestFrom, string convID)
      {
         var fromTo = GetFromAndToFromConversationID(convID);
         var res = Direction.from;
         if (fromTo[1] == latestFrom)
         {
            res = Direction.to;
         }
         return res;
      }
   }


}