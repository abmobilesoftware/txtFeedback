using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class ExtensionMethods
{
   // returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
   public static double UnixTicks(this DateTime dt)
   {
      DateTime d1 = new DateTime(1970, 1, 1);
      DateTime d2 = dt.ToUniversalTime();
      TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
      return ts.TotalMilliseconds;
   }
}

namespace SmsFeedback_Take4.Models
{
  
   public class Message
   {
      static Random lRand = new Random(DateTime.Now.Millisecond);
      public Message(string from, string to, string text, DateTime timeReceived, int id, string direction, bool read=true)
      {
         From = from;
         To = to;
         Text = text;
         TimeReceived = timeReceived;       
         ID = id;         
         Direction = direction;
         if (read)
         {
            ReadStatus = "read";
         }
         else
         {
            ReadStatus = "unread";
         }

      }
      
      public string From { get; set; }
      public string To { get; set; }
      public string Text { get; set; }
      public DateTime TimeReceived { get; set; }
      public double DateTimeInTicks { 
         get {
            return ExtensionMethods.UnixTicks(this.TimeReceived);
         }
      }
      public int ID { get; set; }
      public string ConvID
      {
         get
         {
            return From + "-" + To;
         }
      } //this should be from-to
      public string Direction { get; set; }
      public string ReadStatus { get; set; }      
      
      private const int cMaxTextLenght=40;
      public string TrimmedText
      {
         
         get {
            if (Text.Length > cMaxTextLenght)
            {
               return Text.Substring(0, cMaxTextLenght-3) + "...";
            }
            return Text; 
         }         
      }
      
     
   }

   
}