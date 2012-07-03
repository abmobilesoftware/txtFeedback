using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models
{

   interface IInternalSMSRepository
   {
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumbers(bool showAll,
                                                                                    bool showFavourites,
                                                                                    string[] tags,
                                                                                    string[] workingPointsNumber,
                                                                                    int skip,
                                                                                    int top,
                                                                                    DateTime? lastUpdate,
                                                                                    String userName);
      System.Collections.Generic.IEnumerable<ConversationTag> GetTagsForConversation(string convID);
      System.Collections.Generic.IEnumerable<WorkingPoint> GetWorkingPointsPerUser(String userName);
      Dictionary<string, SmsMessage> GetLatestConversationForNumbers(string[] workingPointNumbers);
   }
   
}