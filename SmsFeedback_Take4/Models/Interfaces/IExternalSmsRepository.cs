using System;
using System.Collections.Generic;
namespace SmsFeedback_Take4.Models
{
   interface IExternalSmsRepository
   {
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumber(bool showAll,
                                                                                          bool showFavourites,
                                                                                          string[] tags,
                                                                                          string workingPointsNumber,
                                                                                          int skip,
                                                                                          int top,
                                                                                          DateTime? lastUpdate,
                                                                                          String userName);      
      System.Collections.Generic.IEnumerable<SmsMessage> GetMessagesForConversation(string convID);
      void SendMessage(string from, string to, string message, Action<string> callback);
   }
     
}
