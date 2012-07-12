using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Models
{
   public class EFSmsRepository : SmsFeedback_Take4.Models.IInternalSMSRepository
   {
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public IEnumerable<SmsMessage> GetConversationsForNumber(bool showAll,
                                                               bool showFavourites,
                                                               string[] tags,
                                                               string workingPointsNumber,
                                                               DateTime? startDate,
                                                               DateTime? endDate,
                                                               bool onlyUnread,
                                                               int skip,
                                                               int top,
                                                               DateTime? lastUpdate,
                                                               String userName, 
                                                               smsfeedbackEntities dbContext)
      {
         //here we don't aplly skip or load, as they will be applied on the merged list
         logger.Info("Call made");
         //the strongest filter first
         //1.favourites
         //2.startDate
         //3.endDate
         //4.tags    
         //since we only give as input the dd/mm/yy we make sure that the comparison date is the lowest/highest possible
         DateTime? earliestStartDate = null;
         DateTime? latestEndDate = null;
         if (startDate.HasValue)
         {
            var oldVal = startDate.Value;
            earliestStartDate = new DateTime(oldVal.Year, oldVal.Month, oldVal.Day, 0, 0, 0);
         }
         if (endDate.HasValue)
         {
            var oldVal = endDate.Value;
            latestEndDate = new DateTime(oldVal.Year, oldVal.Month, oldVal.Day, 23, 59, 59);
         }
         IQueryable<IEnumerable<SmsMessage>> convs = null;
         string consistentWP = ConversationUtilities.RemovePrefixFromNumber(workingPointsNumber);
         if (tags != null && tags.Count() != 0)
         {
            convs = from wp in dbContext.WorkingPoints
                    where wp.TelNumber == consistentWP
                    select (from c in wp.Conversations
                            where (showFavourites ? c.Starred == true : true) &&
                            (onlyUnread? c.Read == false : true) &&
                            (earliestStartDate.HasValue ? c.StartTime >= earliestStartDate.Value : true) &&
                            (latestEndDate.HasValue ? c.TimeUpdated <= latestEndDate.Value : true) &&
                            !tags.Except(c.Tags.Select(tag => tag.Name)).Any()
                            orderby c.TimeUpdated descending
                            select (new SmsMessage() { From = c.From, To = wp.Name, Text = c.Text, TimeReceived = c.TimeUpdated, Read = c.Read, ConvID = c.ConvId, Starred = c.Starred }));
         }
         else
         {
            convs = from wp in dbContext.WorkingPoints
                    where wp.TelNumber == consistentWP
                    select (from c in wp.Conversations
                            where (showFavourites ? c.Starred == true : true) &&
                            (onlyUnread ? c.Read == false : true) &&
                            (earliestStartDate.HasValue ? c.StartTime >= earliestStartDate.Value : true) &&
                            (latestEndDate.HasValue ? c.TimeUpdated <= latestEndDate.Value : true)
                            orderby c.TimeUpdated descending
                            select (new SmsMessage() { From = c.From, To = wp.Name, Text = c.Text, TimeReceived = c.TimeUpdated, Read = c.Read, ConvID = c.ConvId, Starred = c.Starred }));
         }
         if (convs != null && convs.Count() > 0)
         {
            var conversations = convs.First().AsQueryable();
            logger.InfoFormat("Records returned from EF db: {0}", conversations.Count());
            return conversations;
         }
         else
         {
            logger.InfoFormat("No records returned from db for nr: {0}", workingPointsNumber);
            return null;
         }
      }
      public IEnumerable<SmsMessage> GetConversationsForNumbers(bool showAll,
                                                                bool showFavourites,
                                                                string[] tags,
                                                                string[] workingPointsNumbers,
                                                                DateTime? startDate,
                                                                DateTime? endDate,
                                                                bool onlyUnread,
                                                                int skip,
                                                                int top,
                                                                DateTime? lastUpdate,
                                                                String userName,
                                                                smsfeedbackEntities dbContext)
      {
         //TODO we could have a performance penalty for not applying top and skip
         logger.Info("Call made");
         IEnumerable<SmsMessage> results = null;
         foreach (var wp in workingPointsNumbers)
         {
            var conversations = GetConversationsForNumber(showAll, showFavourites, tags, wp, startDate, endDate,onlyUnread, skip, top, lastUpdate, userName,dbContext);
            if (conversations != null)
            {
               if (results == null) results = conversations;
               //merge all the results 
               results = results.Union(conversations);
            }

         }
         //order then descending by TimeReceived
         if (results != null)
         {
            results = results.OrderByDescending(record => record.TimeReceived);
            logger.InfoFormat("Records returned from EF db: {0}", results != null ? results.Count() : 0);
            return results.Skip(skip).Take(top);
         }
         else
         {
            logger.Info("No records for numbers returned from db");
            return null;
         }
      }

      public IEnumerable<WorkingPoint> GetWorkingPointsPerUser(string userName, smsfeedbackEntities dbContext)
      {
         //get logged in user
         //var userID = new Guid("fca4bd52-b855-440d-9611-312708b14c2f");
         var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select new WorkingPoint() { TelNumber = wp.TelNumber, Name = wp.Name, Description = wp.Description });
         if (workingPoints.Count() >= 0)
            return workingPoints.First();
         else return null;
      }

      public Dictionary<string, SmsMessage> GetLatestConversationForNumbers(string[] workingPointNumbers, string userName, smsfeedbackEntities dbContext)
      {
         logger.Info("Call made");
         Dictionary<string, SmsMessage> res = new Dictionary<string, SmsMessage>();
         foreach (string wp in workingPointNumbers)
         {
            var conversations = GetConversationsForNumber(true, false, null, wp, null, null,false, 0, 1, null, userName,dbContext);
            if (conversations != null && conversations.Count() > 0)
            {
               res.Add(wp, conversations.First());
            }
            else
            {
               res.Add(wp, null);
            }
         }
         return res;
      }

      public System.Collections.Generic.IEnumerable<ConversationTag> GetTagsForConversation(string convID, smsfeedbackEntities dbContext)
      {
         logger.Info("Call made");
         var res = from conv in dbContext.Conversations
                   where conv.ConvId == convID
                   select (from tag in conv.Tags
                           select new ConversationTag() { CompanyName = tag.CompanyName, Name = tag.Name, Description = tag.Description });
         if (res != null && res.Count() > 0)
         {
            return res.First();
         }
         else
         {
            return null;
         }
      }

   }
}