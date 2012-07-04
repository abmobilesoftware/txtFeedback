using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models
{

   interface IInternalSMSRepository
   {
      
      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="showAll"></param>
      /// <param name="showFavourites"></param>
      /// <param name="tags"></param>
      /// <param name="workingPointsNumber"></param>
      /// <param name="startDate">null if no condition is wanted</param>
      /// <param name="endDate">null if no condition is wanted</param>
      /// <param name="skip"></param>
      /// <param name="top"></param>
      /// <param name="lastUpdate"></param>
      /// <param name="userName"></param>
      /// <returns></returns>
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumbers(bool showAll,
                                                                                    bool showFavourites,
                                                                                    string[] tags,
                                                                                    string[] workingPointsNumber,
                                                                                    DateTime? startDate,
                                                                                    DateTime? endDate,
                                                                                    int skip,
                                                                                    int top,
                                                                                    DateTime? lastUpdate,
                                                                                    String userName);
      /// <summary>
      /// 
      /// </summary>
      /// <param name="showAll"></param>
      /// <param name="showFavourites"></param>
      /// <param name="tags"></param>
      /// <param name="workingPointsNumber"></param>
      /// <param name="startDate">null if no condition is wanted</param>
      /// <param name="endDate">null if no condition is wanted</param>
      /// <param name="skip"></param>
      /// <param name="top"></param>
      /// <param name="lastUpdate"></param>
      /// <param name="userName"></param>
      /// <returns></returns>
      IEnumerable<SmsMessage> GetConversationsForNumber(bool showAll,
                                                               bool showFavourites,
                                                               string[] tags,
                                                               string workingPointsNumber,
                                                               DateTime? startDate,
                                                               DateTime? endDate,
                                                               int skip,
                                                               int top,
                                                               DateTime? lastUpdate,
                                                               String userName);
      System.Collections.Generic.IEnumerable<ConversationTag> GetTagsForConversation(string convID);
      System.Collections.Generic.IEnumerable<WorkingPoint> GetWorkingPointsPerUser(String userName);
      Dictionary<string, SmsMessage> GetLatestConversationForNumbers(string[] workingPointNumbers,string userName);
   }
   
}