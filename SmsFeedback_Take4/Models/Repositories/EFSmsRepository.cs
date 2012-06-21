using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Models
{
   public class EFSmsRepository : SmsFeedback_Take4.Models.IInternalSmsSourceRepository 
   {
      private smsfeedbackEntities mContext = new smsfeedbackEntities();
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      public IEnumerable<SmsMessage> GetConversationsForNumber(bool showAll,
                                                                      bool showFavourites,
                                                                      string[] tags,
                                                                      string workingPointsNumber,
                                                                      int skip,
                                                                      int top,
                                                                      DateTime? lastUpdate)
      {
         //here we don't aplly skip or load, as they will be applied on the merged list
         //TODO add filtering
         logger.Info("Call made");         
         //if (showAll)
         //{
         string consistentWP = ConversationUtilities.RemovePrefixFromNumber(workingPointsNumber);
         var convs1 = from wp in mContext.WorkingPoints
                      where wp.TelNumber == consistentWP 
                         select (from c in wp.Conversations orderby c.TimeUpdated descending 
                                 select (new SmsMessage() { Id = c.Id, From = c.From, To = wp.Name, Text = c.Text, TimeReceived = c.TimeUpdated, Read = c.Read, ConvID = c.ConvId }));
          var  conversations = convs1.First().AsQueryable();
         //}
         //conversations = (from c in mContext.Conversations orderby c.TimeUpdated descending select new SmsMessage() { Id = c.Id, From = c.From, To = null, Text = c.Text, TimeReceived = c.TimeUpdated, Read = c.Read, ConvID =c.ConvId });
         //else
         //{
         //   //filter according to numbers
         //   conversations = (from c in mContext.Conversations orderby c.TimeUpdated descending select new SmsMessage() { Id = c.Id, From = c.From, To = null, Text = c.Text, TimeReceived = c.TimeUpdated, Read = c.Read, ConvID = c.ConvId });
         //}
         logger.InfoFormat("Records returned from EF db: {0}", conversations.Count());
         return conversations;
      }
      public IEnumerable<SmsMessage> GetConversationsForNumbers(bool showAll,
                                                             bool showFavourites,
                                                             string[] tags,
                                                             string[] workingPointsNumbers,
                                                             int skip,
                                                             int top,
                                                             DateTime? lastUpdate)
      {         
         //TODO we could have a performance penalty for not applying top and skip
         logger.Info("Call made");
         IEnumerable<SmsMessage> results = null;
         foreach (var wp in workingPointsNumbers)
         {            
            var conversations = GetConversationsForNumber(showAll,showFavourites,tags,wp,skip,top,lastUpdate);                         
            if (results == null) results = conversations;
            //merge all the results 
            results = results.Union(conversations);
         }
         //order then descending by TimeReceived
         results = results.OrderByDescending(record => record.TimeReceived);
         logger.InfoFormat("Records returned from EF db: {0}", results != null ? results.Count() : 0);
         return results.Skip(skip).Take(top);
      }

      public IEnumerable<SmsMessage> GetMessagesForConversation(string convID)
      {         
         logger.Info("Call made");
         var res = from conv in mContext.Conversations where conv.ConvId == convID 
                   select (from msg in conv.Messages
                           select new SmsMessage() { Id = msg.Id, From = msg.From, To = msg.To, Text = msg.Text, TimeReceived = msg.TimeReceived, Read = msg.Read, ConvID = convID });       
         if (res != null && res.Count() > 0)         {
            return res.First();
         }
         else         {
            return null;
         }
      }

      public void SendMessage(string from, string to, string message, Action<string> callback)
      {
         throw new NotImplementedException();
      }

      public IEnumerable<WorkingPoint> GetWorkingPointsPerUser(string userName)
      {
         //get logged in user
         //var userID = new Guid("fca4bd52-b855-440d-9611-312708b14c2f");
         var workingPoints = from u in mContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select new WorkingPoint() {TelNumber = wp.TelNumber, Name=wp.Name, Description=wp.Description });
         if (workingPoints.Count() >= 0)
            return workingPoints.First();
         else return null;
      }

      public Dictionary<string, SmsMessage> GetLatestConversationForNumbers(string[] workingPointNumbers)
      {
         logger.Info("Call made");
         Dictionary<string, SmsMessage> res = new Dictionary<string, SmsMessage>();
         foreach(string wp in workingPointNumbers)
         {            
            var conversations = GetConversationsForNumber(true, false, null, wp, 0, 1, null);          
            if (conversations.Count() > 0) {
               res.Add(wp, conversations.First());
            }
            else {
               res.Add(wp, null);
            }
         }
         return res;
      }

      public System.Collections.Generic.IEnumerable<ConversationTag> GetTagsForConversation(string convID)
      {
         logger.Info("Call made");
         var res = from conv in mContext.Conversations
                   where conv.ConvId == convID
                   select (from tag in conv.Tags
                           select new ConversationTag() { Id=tag.Id, Name = tag.Name, Description = tag.Description });
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