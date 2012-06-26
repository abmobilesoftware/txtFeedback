using System;
using System.Collections.Generic;
namespace SmsFeedback_Take4.Models
{
   interface IInternalSmsSourceRepository
   {
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumber(bool showAll,
                                                                                          bool showFavourites,
                                                                                          string[] tags,
                                                                                          string workingPointsNumber,
                                                                                          int skip,
                                                                                          int top,
                                                                                          DateTime? lastUpdate,
                                                                                          String userName);      
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumbers(bool showAll,
                                                                                          bool showFavourites,
                                                                                          string[] tags,
                                                                                          string[] workingPointsNumbers,
                                                                                          int skip,
                                                                                          int top,
                                                                                          DateTime? lastUpdate,
                                                                                          String userName);
      System.Collections.Generic.IEnumerable<SmsMessage> GetMessagesForConversation(string convID);
      System.Collections.Generic.IEnumerable<ConversationTag> GetTagsForConversation(string convID);

      Dictionary<string,SmsMessage> GetLatestConversationForNumbers(string[] workingPointNumbers);
      void SendMessage(string from, string to, string message, Action<string> callback);
      System.Collections.Generic.IEnumerable<WorkingPoint> GetWorkingPointsPerUser(String userName);
   }
   
   interface ISmsSourceRepository
   {
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumbers(bool showAll,
                                                                                    bool showFavourites,
                                                                                    string[] tags,
                                                                                    string[] workingPointsNumber,
                                                                                    int skip,
                                                                                    int top,
                                                                                    DateTime? lastUpdate,
                                                                                    String userName);
      System.Collections.Generic.IEnumerable<SmsMessage> GetMessagesForConversation(string convID);
      System.Collections.Generic.IEnumerable<ConversationTag> GetTagsForConversation(string convID);
      void SendMessage(string from, string to, string message, Action<string> callback);
      System.Collections.Generic.IEnumerable<WorkingPoint> GetWorkingPointsPerUser(String userName);
   }
}
