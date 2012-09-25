using System;
using System.Collections.Generic;

namespace SmsFeedback_Take4.Models
{
   interface IExternalSmsRepository
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="workingPointsNumber"></param>
      /// <param name="lastUpdate"> pass value if you want the conversations from a certain date onwards</param>
      /// <param name="userName"></param>
      /// <returns></returns>
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumber(                                                                                         
                                                                                          string workingPointsNumber,
                                                                                          DateTime? lastUpdate,
                                                                                          String userName);
      System.Collections.Generic.IEnumerable<SmsMessage> GetMessagesForConversation(string convID, bool isConvFavourite);
      void SendMessage(string from, string to, string message, Action<MessageStatus> callback);
   }
     
}
