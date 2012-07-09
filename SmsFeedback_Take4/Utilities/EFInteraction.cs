using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Models;

namespace SmsFeedback_Take4.Utilities
{
   public class EFInteraction
   {
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private smsfeedbackEntities mContext = SmsFeedback_Take4.Models.Repositories.EFContext.GetEFContext();
      public const string CONVERSATION_NOT_MODIFIED = "invalidConvId";      
      public EFInteraction()
      {

      }
      /// <summary>
      /// Return all tags containing a certain string
      /// </summary>
      /// <param name="queryString">the string sequence to look for</param>
      /// <param name="userName">the logged-in user</param>
      /// <returns></returns>
      public IEnumerable<string> FindMatchingTagsForUser(string queryString, string userName)
      {
         logger.Info("Call made");
         //get the company name
         var companyNames = from u in mContext.Users where u.UserName == userName select u.Company_Name;
         var companyName = companyNames.First();
         return FindMatchingTagsForCompany(queryString, companyName);
      }
      /// <summary>
      /// Return all tags containing a certain string
      /// </summary>
      /// <param name="queryString"> the string sequence to look for</param>
      /// <param name="companyName"> the company to whom these tags belong to</param>
      /// <returns></returns>
      public IEnumerable<string> FindMatchingTagsForCompany(string queryString, string companyName)
      {
         logger.Info("Call made");
         var tags = from tag in mContext.Tags where (tag.CompanyName == companyName && tag.Name.Contains(queryString)) select tag.Name;
         return tags;
      }


      public string UpdateAddConversation(String from, String to, String conversationId, String text, Boolean readStatus, DateTime? updateTime, bool markConversationAsRead = false)
      {
         logger.Info("Call made");
         try
         {
            var conversations = from c in mContext.Conversations where c.ConvId == conversationId select c;
            string convId = CONVERSATION_NOT_MODIFIED;
            var updateDateToInsert = updateTime.HasValue ? updateTime.Value.ToString() : "null";
            if (conversations.Count() > 0)
            {
               logger.InfoFormat("Updating conversation: [{0}] with read: {1}, updateTime: {2}", conversationId, readStatus.ToString(), updateDateToInsert);
               var conv = conversations.First();
               convId = conv.ConvId;
               //since twilio returns messages >= the latest message it could be that the latest message is returned again - the only difference is that now "read" is false
               //so make sure that something changed, besides "read"
               if (markConversationAsRead || (conv.Text != text && updateTime.Value != conv.TimeUpdated))
               {
                  //updateTime for when marking a conversation as read will be "null"
                  if(updateTime.HasValue) conv.TimeUpdated = updateTime.Value;
                  if (!string.IsNullOrEmpty(text)) conv.Text = text;
                  conv.Read = readStatus;
                  mContext.SaveChanges();              
               }
            }
            else
            {
               logger.InfoFormat("Adding conversation: [{0}] with read: {1}, updateTime: {2}, text: [{3}], from: [{4}]", conversationId, readStatus.ToString(), updateDateToInsert, text, from);
               //get the working point id
               string consistentWP = ConversationUtilities.RemovePrefixFromNumber(to);
               var workingPointIDs = from wp in mContext.WorkingPoints where wp.TelNumber == consistentWP select wp;
               if (workingPointIDs != null && workingPointIDs.Count() > 0)
               {
                  var wpId = workingPointIDs.First();
                  //add the conversation and give back the id
                  //if we add a new conversation then we the start time will be the update time of the first message
                  mContext.Conversations.AddObject(new Conversation
                  {
                     ConvId = conversationId,
                     Text = text,
                     Read = readStatus,
                     TimeUpdated = updateTime.Value,
                     From = from,
                     WorkingPoint = wpId,
                     StartTime = updateTime.Value
                  });
                  mContext.SaveChanges();
                  //now get the id of the added conversation
                  conversations = from c in mContext.Conversations where c.ConvId == conversationId select c;
                  convId = conversations.First().ConvId;
               }
            }
            return convId;
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in AddMessageAndUpdateConversation", ex);
            return CONVERSATION_NOT_MODIFIED;
         }
      }

      public Message AddMessage(String from, String to, String conversationId, String text, Boolean readStatus, DateTime updateTime)
      {
         logger.Info("Call made");
         try
         {
            var msg = new Message()
            {
               From = from,
               To = to,
               Text = text,
               TimeReceived = updateTime,
               ConversationId = conversationId,
               Read = readStatus
            };
            mContext.Messages.AddObject(msg);
            mContext.SaveChanges();
            return msg;
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in AddMessage", ex);
            return null;
         }
      }

      public XmppConn GetXmppConnectionDetailsPerUser(string userName)
      {
         logger.Info("Call made");         
         var connection = from u in mContext.Users where u.UserName == userName select new XmppConn() { XmppUser = u.XmppConnection.XmppUser, XmppPassword = u.XmppConnection.XmppPassword };
         return connection.First();
      }

      public Tag AddTagToDB(string tagName, string tagDescription, string userName)
      {
         logger.Info("Call made");
         try
         {
            //don't add the same tag twice (since the Name is not unique)
            var tags = from t in mContext.Tags where t.Name == tagName select t;
            if (tags.Count() == 0)
            {
               var companies = from u in mContext.Users where u.UserName == userName select u.Company;
               var companyName = companies.First().Name;
               var newTag = new Tag() { Name = tagName, Description = tagDescription, CompanyName= companyName };
               mContext.Tags.AddObject(newTag);
               mContext.SaveChanges();
               return newTag;
            }
            else
            {
               return tags.First();
            }
         }
         catch (Exception ex)
         {
            logger.Error("Error in AddTagToDB", ex);
            return null;
         }
      }

      public void AddTagToConversation(string tagName, string convID)
      {
         var tags = from t in mContext.Tags where t.Name == tagName select t;
         if( tags.Count() > 0)
         {
            AddTagToConversation(tags.First(), convID);
         }
      }
  
      public void AddTagToConversation(Tag tag, string convID)
      {
          logger.Info("Call made");
          try
          {
             var convs = from c in mContext.Conversations where c.ConvId == convID select c;
             if (convs.Count() > 0)
             {
                convs.First().Tags.Add(tag);
                mContext.SaveChanges();
             }
          }
          catch (Exception ex)
          {
             logger.Error("Error in AddTagToConversation", ex);          
          }
      }

      public void RemoveTagFromConversation(string tagName, string convID)
      {
          logger.Info("Call made");
          try
          {
             var convs = from c in mContext.Conversations where c.ConvId == convID select c;
             if (convs.Count() > 0)
             {
                var tags = from t in mContext.Tags where t.Name == tagName select t;
                if (tags.Count() > 0)
                {
                   convs.First().Tags.Remove(tags.First());
                   mContext.SaveChanges();
                }
             }
          }
          catch (Exception ex)
          {
             logger.Error("Error in RemoveTagFromConversation", ex);           
          }
      }
   }
}