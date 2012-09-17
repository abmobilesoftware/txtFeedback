﻿using System;
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
            return FindMatchingTagsForCompany(queryString, companyName, dbContext);
        }

        /// <summary>
        /// Add an event in the history of a conversation
        /// </summary>
        /// <param name="conversationId">Id of the conversation</param>
        /// <param name="eventType">The type of the event</param>
        /// <returns>The conversation history event</returns>
        public ConversationHistory AddAnEventInConversationHistory(string conversationId, string eventType, smsfeedbackEntities dbContext)
        {
            var conversation = (from conv in dbContext.Conversations where conv.ConvId == conversationId select conv).First();
            var message = (from msg in conversation.Messages where (msg.From != conversation.WorkingPoint_TelNumber) select msg).OrderByDescending(m => m.TimeReceived);
           
                try
                {
                    if (message.Count() > 0)
                    {
                        var conversationEvent = new ConversationHistory()
                        {
                            ConversationConvId = conversationId,
                            Sequence = conversation.LastSequence,
                            EventTypeName = eventType,
                            Date = DateTime.Now.ToUniversalTime(),
                            MessageId = message.First().Id
                        };
                        dbContext.ConversationHistories.AddObject(conversationEvent);
                        dbContext.SaveChanges();
                        return conversationEvent;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred in AddAnEventInConversationHistory", ex);
                    return null;
                }
            
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
                                            bool markConversationAsRead = false
                                            )
        {
            logger.Info("Call made");
            try
            {
                var conversations = from c in dbContext.Conversations where c.ConvId == conversationId select c;
                string convId = CONVERSATION_NOT_MODIFIED;
                var updateDateToInsert = updateTime.HasValue ? updateTime.Value.ToString() : "null";
                if (conversations.Count() > 0)
                {
                    logger.InfoFormat("Updating conversation: [{0}] with read: {1}, updateTime: {2},  text: {3}, from {4}", conversationId, readStatus.ToString(), updateDateToInsert, text, from);
                    var conv = conversations.First();
                    convId = conv.ConvId;
                    //since twilio returns (messages >= the latest message) it could be that the latest message is returned again - the only difference is that now "read" is false
                    //so make sure that something changed, besides "read"
                    if (conv.Text != text && (updateTime.HasValue && updateTime.Value != conv.TimeUpdated))
                    {
                        //updateTime for when marking a conversation as read will be "null"
                        if (updateTime.HasValue) conv.TimeUpdated = updateTime.Value;
                        if (!string.IsNullOrEmpty(text)) conv.Text = text;
                        //show the direction of the last message
                        conv.From = from;
                        conv.Read = readStatus;
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

        public Conversation MarkConversationAsRead(string convID, smsfeedbackEntities dbContext)
        {
            var conversations = from c in dbContext.Conversations where c.ConvId == convID select c;
            if (conversations.Count() > 0)
            {
                var conv = conversations.First();
                conv.Read = true;
                dbContext.SaveChanges();
                return conv;
            }
            //if there was no conversation associated to this convID 
            logger.ErrorFormat("No conversation with id {0} found", convID);
            return null;
        }

        public Conversation UpdateStarredStatusForConversation(string convID, bool newStarredStatus, smsfeedbackEntities dbContext)
        {
            var conversations = from c in dbContext.Conversations where c.ConvId == convID select c;
            if (conversations.Count() > 0)
            {
                var conv = conversations.First();
                conv.Starred = newStarredStatus;
                dbContext.SaveChanges();
                return conv;
            }
            //if there was no conversation associated to this convID 
            logger.ErrorFormat("No conversation with id {0} found", convID);
            return null;
        }

        public Message AddMessage(String userID, String from, String to, String conversationId, String text, Boolean readStatus, DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime, smsfeedbackEntities dbContext)
        {
            //assume userID and convID are valid
            logger.Info("Call made");
            var userGuids = from usr in dbContext.Users where usr.UserName == userID select usr.UserId;
            var userGuid = userGuids.First();
            //for the responce time -> the lastest details are always in the conversation

            long? responceTime = null;
            if (ConversationUtilities.GetDirectionForMessage(prevConvFrom, conversationId) == ConversationUtilities.Direction.from)
            {
                responceTime = updateTime.Subtract(prevConvUpdateTime).Ticks;
            }
            try
            {
                var msg = new Message()
                {
                    ResponseTime = responceTime,
                    UserUserId = userGuid,
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

        public void IncrementNumberOfSentSms(String wpID, smsfeedbackEntities dbContext)
        {
            var wps = from wp in dbContext.WorkingPoints where wp.TelNumber == wpID select wp;
            if (wps.Count() == 1)
            {
                var wp = wps.First();
                wp.SentSms += 1;
                dbContext.SaveChanges();
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
                    var newTag = new Tag() { Name = tagName, Description = tagDescription, CompanyName = companyName };
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
            if (tags.Count() > 0)
            {
                AddTagToConversation(tags.First(), convID, dbContext);
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
                    var conv = convs.First();
                    var convTag = new SmsFeedback_EFModels.ConversationTag() { ConversationConvId = convID, TagCompanyName = tag.CompanyName, TagName = tag.Name, DateAdded = DateTime.UtcNow };
                    conv.ConversationTags.Add(convTag);
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
                    var conv = convs.First();
                    var tags = from t in conv.ConversationTags where t.TagName == tagName select t;
                    if (tags.Count() == 1)
                    {
                        conv.ConversationTags.Remove(tags.First());
                        dbContext.SaveChanges();
                    }
                    //var tags = from t in dbContext.Tags where t.Name == tagName select t;
                    //if (tags.Count() > 0)
                    //{
                    //   convs.First().Tags.Remove(tags.First());
                    //   dbContext.SaveChanges();
                    //}
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
                if (conv.Count() > 0) { return conv.First(); }
                else
                {
                    //this conversationID was not found in our db - for sure it's not a favourite
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("IsConversationFavourite error", ex);
                return false;
            }
        }

        public IEnumerable<SmsMessage> GetMessagesForConversation(string convID, smsfeedbackEntities dbContext)
        {
            //TODO: error handling & sanity checks
            //if the conversation is marked as "favourite" then all the messages will be "favourite"
            var isConvFavourite = IsConversationFavourite(convID, dbContext);
            var msgs = from conv in dbContext.Conversations
                       where conv.ConvId == convID 
                       select
                          (from msg in conv.Messages 
                           select new SmsMessage()
                           {
                               From = msg.From,
                               To = msg.To,
                               ConvID = msg.ConversationId,
                               Read = msg.Read,
                               Id = msg.Id,
                               Starred = isConvFavourite,
                               Text = msg.Text,
                               TimeReceived = msg.TimeReceived,
                               Day = msg.TimeReceived.Day,
                               Month = msg.TimeReceived.Month,
                               Year = msg.TimeReceived.Year,
                               Hours = msg.TimeReceived.Hour,
                               Minutes = msg.TimeReceived.Minute,
                               Seconds = msg.TimeReceived.Second
                           });
            if (msgs.Count() > 0)
            {
                return msgs.First().OrderBy(x=>x.TimeReceived);
            }
            else
            {
                return new SmsMessage[] { };
            }
        }
        public string GetNameForWorkingPoint(string wpTelNumber, smsfeedbackEntities dbContext)
        {
            var wpName = from wp in dbContext.WorkingPoints where wp.TelNumber == wpTelNumber select wp.Name;
            if (wpName.Count() == 1)
            {
                return wpName.First();
            }
            else
            {
                return "";
            }
        }

        public Conversation GetLatestConversationDetails(string convId, smsfeedbackEntities dbContext)
        {
            var conv = (from c in dbContext.Conversations where c.ConvId == convId select c).First();
            return conv;
        }

        public IEnumerable<SmsFeedback_EFModels.WorkingPoint> GetWorkingPointsForAUser(String scope, String userName, smsfeedbackEntities dbContext)
        {
            IEnumerable<SmsFeedback_EFModels.WorkingPoint> incommingMsgs;
            if (scope.Equals(Constants.GLOBAL_SCOPE))
            {
                var workingPoints = from u in dbContext.Users where u.UserName == userName select (from wp in u.WorkingPoints select wp);
                incommingMsgs = workingPoints.First();
            }
            else
            {
                incommingMsgs = from wp in dbContext.WorkingPoints where wp.TelNumber == scope select wp;
            }

            return incommingMsgs;
        }

     public void SaveWpsForUser(string user, List<Models.WorkingPoint> wps, smsfeedbackEntities dbContext)
     {
        //to avoid security issues - work only on the working points accessible to this user
        var users = from us in dbContext.Users where us.UserName == user select us;
        if (users.Count() == 1)
        {
           var usr = users.First();
           foreach (Models.WorkingPoint wp in wps)
           {
              var newWp = from w in usr.WorkingPoints where w.TelNumber == wp.TelNumber select w;
              if (newWp.Count() == 1)
              {
                 newWp.First().Name = wp.Name;
                 newWp.First().Description = wp.Description;
              }
           }
           dbContext.SaveChanges();
        }

     }
    }
}