using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_EFModels;

namespace SmsFeedback_Take4.Models
{

   interface IInternalSMSRepository
   {
      
      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="onlyFavorites"></param>
      /// <param name="tags"></param>
      /// <param name="workingPointsNumber"></param>
      /// <param name="startDate">null if no condition is wanted</param>
      /// <param name="endDate">null if no condition is wanted</param>
      /// <param name="skip"></param>
      /// <param name="top"></param>
      /// <param name="lastUpdate"></param>
      /// <param name="userName"></param>
      /// <param name="dbContext"></param>
      /// <returns></returns>
      System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumbers(
                                                                                    bool onlyFavorites,
                                                                                    string[] tags,
                                                                                    string[] workingPointsNumber,
                                                                                    DateTime? startDate,
                                                                                    DateTime? endDate,
                                                                                    bool onlyUnread,
                                                                                    int skip,
                                                                                    int top,
                                                                                    DateTime? lastUpdate,
                                                                                    String userName,
                                                                                    smsfeedbackEntities dbContext);
      /// <summary>
      /// 
      /// </summary>
      /// <param name="onlyFavorites"></param>
      /// <param name="tags"></param>
      /// <param name="workingPointsNumber"></param>
      /// <param name="startDate">null if no condition is wanted</param>
      /// <param name="endDate">null if no condition is wanted</param>
      /// <param name="skip"></param>
      /// <param name="top"></param>
      /// <param name="lastUpdate"></param>
      /// <param name="userName"></param>
      /// <param name="dbContext"></param>
      /// <returns></returns>
      IEnumerable<SmsMessage> GetConversationsForNumber(
                                                               bool onlyFavorites,
                                                               string[] tags,
                                                               string workingPointsNumber,
                                                               DateTime? startDate,
                                                               DateTime? endDate,
                                                               bool onlyUnread,
                                                               int skip,
                                                               int top,
                                                               DateTime? lastUpdate,
                                                               String userName,
                                                               smsfeedbackEntities dbContext);
      System.Collections.Generic.IEnumerable<ConversationTag> GetTagsForConversation(string convID,  smsfeedbackEntities dbContext);
      System.Collections.Generic.IEnumerable<WorkingPoint> GetWorkingPointsPerUser(String userName, smsfeedbackEntities dbContext);
      Dictionary<string, SmsMessage> GetLatestConversationForNumbers(string[] workingPointNumbers,string userName, smsfeedbackEntities dbContext);
   }
   
}