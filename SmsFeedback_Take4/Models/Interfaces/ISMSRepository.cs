using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models
{
   interface ISMSRepository
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