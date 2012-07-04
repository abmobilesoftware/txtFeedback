using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models
{
   public class SmsMessage
   {
      public SmsMessage()
      {         
      }

      public SmsMessage(int id, string from, string to, string text, DateTime timeReceived, bool readStatus, string convID)
      {
         Id = id;
         From = from;
         To = to;
         Text = text;
         TimeReceived = TimeReceived;
         Read = readStatus;
         ConvID = convID;
      }
      public int Id { get; set; }
      public string From { get; set; }
      public string To { get; set; }
      public string Text { get; set; }
      public DateTime TimeReceived { get; set; }
      public bool Read { get; set; }
      public string ConvID { get; set; }
   }
}