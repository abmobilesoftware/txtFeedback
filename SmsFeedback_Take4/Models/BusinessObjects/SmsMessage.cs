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

      public SmsMessage(int id, string from, string to, string text, DateTime timeReceived, bool readStatus, string convID, int day = 0, int month = 0, int year = 0, int hours = 0, int minutes = 0, int seconds = 0)
      {
         Id = id;
         From = from;
         To = to;
         Text = text;
         TimeReceived = TimeReceived;
         Read = readStatus;
         ConvID = convID;
         Day = day;
         Month = month;
         Year = year;
         Hours = hours;
         Minutes = minutes;
         Seconds = seconds;
         
      }
      public int Id { get; set; }
      public string From { get; set; }
      public string To { get; set; }
      public string Text { get; set; }
      public DateTime TimeReceived { get; set; }
      public bool Read { get; set; }
      public string ConvID { get; set; }
      public bool Starred { get; set; }
      public int Day { get; set; }
      public int Month { get; set; }
      public int Year { get; set; }
      public int Hours { get; set; }
      public int Minutes { get; set; }
      public int Seconds { get; set; }
   }
}