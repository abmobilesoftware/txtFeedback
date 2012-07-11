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
      public IEnumerable<string> FindMatchingTagsForUser(string queryString, string userName, smsfeedbackEntities dbContext)
      {
         logger.Info("Call made");
         //get the company name
         var companyNames = from u in dbContext.Users where u.UserName == userName select u.Company_Name;
         var companyName = companyNames.First();
         return FindMatchingTagsForCompany(queryString, companyName,dbContext);
      }
      /// <summary>
      /// Return all tags containing a certain string
      /// </summary>
      /// <param name="queryString"> the string sequence to look for</param>
      /// <param name="companyName"> the company to whom these tags belong to</param>
      /// <returns></returns>
      public IEnumerable<string> FindMatchingTagsForCompany(string queryString, string companyName, smsfeedbackEntities dbContext)
      {
         logger.Info("Call made");
         var tags = from tag in dbContext.Tags where (tag.CompanyName == companyName && tag.Name.Contains(queryString)) select tag.Name;
         return tags;
      }


      public string UpdateAddConversation(
                                          String from,
                                          String to, 
                                          String conversationId,
                                          String text, 
                                          Boolean readStatus, 
                                          DateTime? updateTime,
                                          smsfeedbackEntities dbContext, 
                                          bool markConversationAsRead = false,
                                          bool? newStarredStatus = null )
      {
         logger.Info("Call made");
         try
         {
            var conversations = from c in dbContext.Conversations where c.ConvId == conversationId select c;
            string convId = CONVERSATION_NOT_MODIFIED;
            var updateDateToInsert = updateTime.HasValue ? updateTime.Value.ToString() : "null";
            if (conversations.Count() > 0)
            {
               logger.InfoFormat("Updating conversation: [{0}] with read: {1}, updateTime: {2},  text: {3}" , conversationId, readStatus.ToString(), updateDateToInsert, text);
               var conv = conversations.First();
               convId = conv.ConvId;
               //since twilio returns messages >= the latest message it could be that the latest message is returned again - the only difference is that now "read" is false
               //so make sure that something changed, besides "read"
               if (markConversationAsRead || (conv.Text != text && (updateTime.HasValue && updateTime.Value != conv.TimeUpdated)))
               {
                  //updateTime for when marking a conversation as read will be "null"
                  if(updateTime.HasValue) conv.TimeUpdated = updateTime.Value;
                  if (!string.IsNullOrEmpty(text)) conv.Text = text;
                  conv.Read = readStatus;
                  dbContext.SaveChanges();              
               }
               //I made a new branch as this will be a separate call (maybe deserves another method)
               if (newStarredStatus.HasValue)
               {
                  conv.Starred = newStarredStatus.Value;
                  dbContext.SaveChanges();
               }
            }
            else
            {
               logger.InfoFormat("Adding conversation: [{0}] with read: {1}, updateTime: {2}, text: [{3}], from: [{4}]", conversationId, readStatus.ToString(), updateDateToInsert, text, from);
               //get the working point id
               string consistentWP = ConversationUtilities.RemovePrefixFromNumber(to);
               var workingPointIDs = from wp in dbContext.WorkingPoints where wp.TelNumber == consistentWP select wp;
               if (workingPointIDs != null && workingPointIDs.Count() > 0)
               {
                  var wpId = workingPointIDs.First();
                  //add the conversation and give back the id
                  //if we add a new conversation then we the start time will be the update time of the first message
                  dbContext.Conversations.AddObject(new Conversation
                  {
                     ConvId = conversationId,
                     Text = text,
                     Read = readStatus,
                     TimeUpdated = updateTime.Value,
                     From = from,
                     WorkingPoint = wpId,
                     StartTime = updateTime.Value
                  });
                  dbContext.SaveChanges();
                  //now get the id of the added conversation
                  conversations = from c in dbContext.Conversations where c.ConvId == conversationId select c;
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
    
      public Message AddMessage(String from, String to, String conversationId, String text, Boolean readStatus, DateTime updateTime, smsfeedbackEntities dbContext)
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
            dbContext.Messages.AddObject(msg);
            dbContext.SaveChanges();
            return msg;
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in AddMessage", ex);
            return null;
         }
      }

      public XmppConn GetXmppConnectionDetailsPerUser(string userName, smsfeedbackEntities dbContext)
      {
         logger.Info("Call made");         
         var connection = from u in dbContext.Users where u.UserName == userName select new XmppConn() { XmppUser = u.XmppConnection.XmppUser, XmppPassword = u.XmppConnection.XmppPassword };
         return connection.First();
      }

      public Tag AddTagToDB(string tagName, string tagDescription, string userName, smsfeedbackEntities dbContext)
      {
         logger.Info("Call made");
         try
         {
            //don't add the same tag twice (since the Name is not unique)
            var tags = from t in dbContext.Tags where t.Name == tagName select t;
            if (tags.Count() == 0)
            {
               var companies = from u in dbContext.Users where u.UserName == userName select u.Company;
               var companyName = companies.First().Name;
               var newTag = new Tag() { Name = tagName, Description = tagDescription, CompanyName= companyName };
               dbContext.Tags.AddObject(newTag);
               dbContext.SaveChanges();
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

      public void AddTagToConversation(string tagName, string convID, smsfeedbackEntities dbContext)
      {         
         var tags = from t in dbContext.Tags where t.Name == tagName select t;
         if( tags.Count() > 0)
         {
            AddTagToConversation(tags.First(), convID,dbContext);
         }
      }

      public void AddTagToConversation(Tag tag, string convID, smsfeedbackEntities dbContext)
      {
          logger.Info("Call made");
          try
          {
             var convs = from c in dbContext.Conversations where c.ConvId == convID select c;
             if (convs.Count() > 0)
             {
                convs.First().Tags.Add(tag);
                dbContext.SaveChanges();
             }
          }
          catch (Exception ex)
          {
             logger.Error("Error in AddTagToConversation", ex);          
          }
      }

      public void RemoveTagFromConversation(string tagName, string convID, smsfeedbackEntities dbContext)
      {
          logger.Info("Call made");
          try
          {
             var convs = from c in dbContext.Conversations where c.ConvId == convID select c;
             if (convs.Count() > 0)
             {
                var tags = from t in dbContext.Tags where t.Name == tagName select t;
                if (tags.Count() > 0)
                {
                   convs.First().Tags.Remove(tags.First());
                   dbContext.SaveChanges();
                }
             }
          }
          catch (Exception ex)
          {
             logger.Error("Error in RemoveTagFromConversation", ex);           
          }
      }

      public bool IsConversationFavourite(string convID, smsfeedbackEntities dbContext)
      {
         try
         {
            var conv = from c in dbContext.Conversations where c.ConvId == convID select c.Starred;
            if(conv.Count() > 0)return conv.First();
            return false;
         }
         catch (Exception ex)
         {
            logger.Error("IsConversationFavourite error", ex);
            return false;
         }
      }
   }
}