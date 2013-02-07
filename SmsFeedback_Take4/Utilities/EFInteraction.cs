using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Models;
using System.Globalization;
using Mvc.Mailer;
using System.Web.Mvc;
using SmsFeedback_Take4.Models.Helpers;

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
        public void AddAnEventInConversationHistory(string conversationId, string eventType, smsfeedbackEntities dbContext)
        {
            Dictionary<string, string> eventTable = new Dictionary<string, string>();
            eventTable.Add("pos-pos", Constants.NOE);
            eventTable.Add("pos-neuter", Constants.POS_REMOVE_EVENT);
            eventTable.Add("pos-neg", Constants.POS_TO_NEG_EVENT);
            eventTable.Add("neg-pos", Constants.NEG_TO_POS_EVENT);
            eventTable.Add("neg-neuter", Constants.NEG_REMOVE_EVENT);
            eventTable.Add("neg-neg", Constants.NOE);
            eventTable.Add("neuter-pos", Constants.POS_ADD_EVENT);
            eventTable.Add("neuter-neuter", Constants.NOE);
            eventTable.Add("neuter-neg", Constants.NEG_ADD_EVENT);

            Dictionary<string, string> eventStateValue = new Dictionary<string, string>();
            eventStateValue.Add(Constants.POS_ADD_EVENT, Constants.POSITIVE);
            eventStateValue.Add(Constants.NEG_ADD_EVENT, Constants.NEGATIVE);
            eventStateValue.Add(Constants.NEG_TO_POS_EVENT, Constants.POSITIVE);
            eventStateValue.Add(Constants.POS_TO_NEG_EVENT, Constants.NEGATIVE);
            eventStateValue.Add(Constants.POS_REMOVE_EVENT, Constants.NEUTER);
            eventStateValue.Add(Constants.NEG_REMOVE_EVENT, Constants.NEUTER);

            Dictionary<string, string> stateToEvent = new Dictionary<string, string>();
            stateToEvent.Add(Constants.POSITIVE, Constants.POS_ADD_EVENT);
            stateToEvent.Add(Constants.NEGATIVE, Constants.NEG_ADD_EVENT);
            stateToEvent.Add(Constants.NEUTER, Constants.NOE);

            var conversation = dbContext.Conversations.Find(conversationId);
            if (conversation != null)
            {
               // get the last received message of the conversation
               var message = (from msg in conversation.Messages where (msg.From != conversation.WorkingPoint_TelNumber) select msg).OrderByDescending(m => m.TimeReceived);
               try
               {
                  // in normal conditions this condition should be always true
                  if (message.Count() > 0)
                  {
                     var convEvents = (from convEvent in conversation.ConversationEvents select convEvent).OrderByDescending(c => c.Date);
                     // more than one event were added 
                     if (convEvents.Count() > 1)
                     {
                        // test if at least one new message had arrived since the last event was added
                        if (convEvents.First().Message.Id < message.First().Id)
                        {
                           if (convEvents.First().EventTypeName.Equals(Constants.NOE))
                           {
                              var eventName = convEvents.First().EventTypeName;
                              convEvents.First().EventTypeName = convEvents.ElementAt(1).EventTypeName;
                              convEvents.ElementAt(1).EventTypeName = eventName;
                              dbContext.SaveChanges();
                           }
                           AddEventInConversationHistory(conversationId,
                               conversation.LastSequence,
                               eventTable[eventStateValue[convEvents.First().EventTypeName] + "-" + eventType],
                               DateTime.Now.ToUniversalTime(),
                               message.First().Id,
                               dbContext);
                        }
                        // no new message arrived
                        else if (convEvents.First().Message.Id == message.First().Id)
                        {
                           ConversationHistory convEvent = convEvents.First();
                           convEvent.EventTypeName = eventTable[eventStateValue[convEvents.ElementAt(1).EventTypeName] + "-" + eventType];
                           dbContext.SaveChanges();
                        }
                     }
                     else if (convEvents.Count() == 1)
                     {
                        // test if at least one new message had arrived since the last event was added
                        if (convEvents.First().Message.Id < message.First().Id)
                        {
                           AddEventInConversationHistory(conversationId,
                               conversation.LastSequence,
                               eventTable[eventStateValue[convEvents.First().EventTypeName] + "-" + eventType],
                               DateTime.Now.ToUniversalTime(),
                               message.First().Id,
                               dbContext);
                        }
                        // no new message arrived
                        else if (convEvents.First().Message.Id == message.First().Id)
                        {
                           var currentEvent = eventTable[eventStateValue[convEvents.First().EventTypeName] + "-" + eventType];
                           if (!(currentEvent.Equals(Constants.POS_REMOVE_EVENT) || currentEvent.Equals(Constants.NEG_REMOVE_EVENT)))
                           {
                              var eventTransformed = Constants.POS_ADD_EVENT;
                              if (currentEvent.Equals(Constants.POS_TO_NEG_EVENT))
                              {
                                 eventTransformed = Constants.NEG_ADD_EVENT;
                              }
                              else if (currentEvent.Equals(Constants.NEG_TO_POS_EVENT))
                              {
                                 eventTransformed = Constants.POS_ADD_EVENT;
                              }
                              else
                              {
                                 eventTransformed = currentEvent;
                              }
                              ConversationHistory convEvent = convEvents.First();
                              convEvent.EventTypeName = eventTransformed;
                              dbContext.SaveChanges();
                           }
                           else
                           {
                              dbContext.ConversationHistories.Remove(convEvents.First());
                              dbContext.SaveChanges();
                           }
                        }

                     }
                     // there's no event for this conversation, add the first event
                     else
                     {
                        AddEventInConversationHistory(conversationId,
                                conversation.LastSequence,
                                stateToEvent[eventType],
                                DateTime.Now.ToUniversalTime(),
                                message.First().Id,
                                dbContext);
                     }
                  }
               }
               catch (Exception ex)
               {
                  logger.Error("Error occurred in AddAnEventInConversationHistory", ex);
               }
            }
        }

        public string DeleteMessage(String messageText, String convId, DateTime receivedDate, smsfeedbackEntities dbContext)
        {
           // sanitize input            
            convId = convId.Trim();
           //TODO DA - there should be no trim on the text (???)
            messageText = messageText.Trim();
            var conv = dbContext.Conversations.Find(convId);
            if (conv != null)                
            {                
                var messages = from message in conv.Messages where (Math.Abs(message.TimeReceived.Ticks - receivedDate.Ticks) < 10000000 && message.Text.Trim().Equals(messageText)) select message;
                if (messages.Count() > 0)
                {
                    Message firstMessage = messages.First();
                    
                    List<ConversationHistory> events = (from convEvent in dbContext.ConversationHistories where convEvent.MessageId == firstMessage.Id select convEvent).ToList();
                    foreach (var singleEvent in events)
                    {
                        dbContext.ConversationHistories.Remove(singleEvent);
                        dbContext.SaveChanges();
                    }
                    dbContext.Messages.Remove(messages.First());
                    dbContext.SaveChanges();
                    if ((firstMessage.TimeReceived.Ticks - conv.TimeUpdated.Ticks) < 10000000 && conv.Text.Trim().Equals(firstMessage.Text.Trim()))
                    {
                        return "last message";
                    }
                    else
                    {
                        return "normal message";
                    }
                }
                return "not executed";
            }
            return "not executed";
        }

        #region Conversations
        public void DeleteConversation(String convId, smsfeedbackEntities dbContext)
        {
            // sanitize input
            convId = convId.Trim();            
            var conv = dbContext.Conversations.Find(convId);
            if (conv != null)
            {                
                List<ConversationTag> tagsForConversation = (from tag in conv.ConversationTags select tag).ToList();
                foreach (var tag in tagsForConversation)
                {
                    dbContext.ConversationTags.Remove(tag);
                    dbContext.SaveChanges();
                }

                List<Message> messagesForConversation = (from msg in conv.Messages select msg).ToList();
                foreach (var message in messagesForConversation)
                {
                    DeleteMessage(message.Text, message.ConversationId, message.TimeReceived, dbContext); 
                }

                dbContext.Conversations.Remove(conv);
                dbContext.SaveChanges();
            }            
        }
       
       // TODO: Merge UpdateConversationText with UpdateAddConversation       
       public void UpdateConversationText(string convId, string newText, DateTime newTextDateReceived, smsfeedbackEntities dbContext)
        {
            convId = convId.Trim();
            newText = newText.Trim();
            var conversations = from conversation in dbContext.Conversations where conversation.ConvId.Equals(convId) select conversation;
            if (conversations.Count() > 0)
            {
                Conversation conversation = conversations.First();
                conversation.Text = newText;
                conversation.TimeUpdated = newTextDateReceived;
                dbContext.SaveChanges();
            }
        }
       
       private void AddEventInConversationHistory(string iConversationId, int iSequence, string iEventTypeName, DateTime iEventDate, int iMessageId, smsfeedbackEntities dbContext)
        {
            var conversationEvent = new ConversationHistory()
            {
                ConversationConvId = iConversationId,
                Sequence = iSequence,
                EventTypeName = iEventTypeName,
                Date = iEventDate,
                MessageId = iMessageId
            };
            dbContext.ConversationHistories.Add(conversationEvent);
            dbContext.SaveChanges();
        }
        
        public string UpdateOrAddConversation(
                                            String sender,
                                            String addressee,
                                            String conversationId,
                                            String text,
                                            Boolean readStatus,
                                            DateTime? updateTime,
                                            bool isSmsBased,
                                            smsfeedbackEntities dbContext,
                                            bool markConversationAsRead = false
                                            )
        {
           logger.Info("Call made");
           try
           {
              var conv = dbContext.Conversations.Find(conversationId);
              var updateDateToInsert = updateTime.HasValue ? updateTime.Value.ToString() : "null";
              if (conv != null)
              {
                 return UpdateExistingConversation(sender, text, readStatus, updateTime, dbContext, conv, updateDateToInsert);
              }
              else
              {
                 return AddNewConversation(sender, addressee, conversationId, text, readStatus, updateTime, isSmsBased, dbContext, updateDateToInsert);
              }
           }
           catch (Exception ex)
           {
              logger.Error("Error occurred in AddMessageAndUpdateConversation", ex);
              return CONVERSATION_NOT_MODIFIED;
           }
        }

        private static string AddNewConversation(
            String from,
            String to,
            String conversationId,
            String text,
            Boolean readStatus,
            DateTime? updateTime,
            bool isSmsBased,
            smsfeedbackEntities dbContext,
            string updateDateToInsert)
        {
           logger.InfoFormat("Add new conversation: [{0}] with read: {1}, updateTime: {2}, text: [{3}], from: [{4}]", conversationId, readStatus.ToString(), updateDateToInsert, text, from);
           // Check if the client of this coversation it's in db. 
           Client clientForThisConversation = GetClientByName(from, dbContext);
           IEnumerable<SmsFeedback_EFModels.WorkingPoint> workingPointIDs;
           if (isSmsBased)
           {
              string consistentWP = ConversationUtilities.CleanUpPhoneNumber(to);
              workingPointIDs = from wp in dbContext.WorkingPoints where wp.TelNumber == consistentWP select wp;
           }
           else
           {
              string workingPointShortId = ConversationUtilities.GetFromAndToFromConversationID(conversationId)[1];
              workingPointIDs = from wp in dbContext.WorkingPoints where wp.ShortID == workingPointShortId select wp;
           }
           if (workingPointIDs != null && workingPointIDs.Count() > 0)
           {
              var wpId = workingPointIDs.First();
              //if we add a new conversation then we the start time will be the update time of the first message
              dbContext.Conversations.Add(new Conversation
              {
                 ConvId = conversationId,
                 Text = text,
                 Read = readStatus,
                 TimeUpdated = updateTime.Value,
                 From = from,
                 WorkingPoint = wpId,
                 StartTime = updateTime.Value,
                 IsSmsBased = isSmsBased,
                 Client = clientForThisConversation
              });
              dbContext.SaveChanges();
              return JsonReturnMessages.OP_SUCCESSFUL;
           }
           else
           {
              return JsonReturnMessages.INVALID_WPID;
           }
        }

        private string UpdateExistingConversation(
           String sender,
           String text,
           Boolean readStatus,
           DateTime? updateTime,
           smsfeedbackEntities dbContext,
           Conversation currentConversation,
           string updateDateToInsert)
        {
           logger.InfoFormat("Updating conversation: [{0}] with read: {1}, updateTime: {2},  text: {3}, from {4}", currentConversation.ConvId, readStatus.ToString(), updateDateToInsert, text, sender);
           /*
            * since twilio returns (messages >= the latest message) it could be that the latest message is returned again - the only difference is that now "read" is false
            * so make sure that something changed, besides "read"
            */
           bool differentMsgBody = currentConversation.Text != text;
           bool differentMsgDate = (updateTime.HasValue && updateTime.Value != currentConversation.TimeUpdated);
           bool newMessage = differentMsgBody && differentMsgDate;

           if (newMessage)
           {
              //updateTime for when marking a conversation as read will be "null"
              if (updateTime.HasValue) currentConversation.TimeUpdated = updateTime.Value;
              if (!string.IsNullOrEmpty(text)) currentConversation.Text = text;
              currentConversation.From = sender;
              currentConversation.Read = readStatus;
              try
              {
                 dbContext.SaveChanges();
              }
              catch (Exception e)
              {
                 logger.Error(e.Message);
                 return JsonReturnMessages.EXCEPTION;
              }
              return JsonReturnMessages.OP_SUCCESSFUL;
           }
           else
           {
              return JsonReturnMessages.DUPLICATE_MESSAGE;
           }
        }

        public Conversation UpdateStarredStatusForConversation(string convID, bool newStarredStatus, smsfeedbackEntities dbContext)
        {
           var conv = dbContext.Conversations.Find(convID);
           if (conv != null)
           {
              conv.Starred = newStarredStatus;
              dbContext.SaveChanges();
              return conv;
           }
           //if there was no conversation associated to this convID 
           logger.ErrorFormat("No conversation with id {0} found", convID);
           return null;
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
              var conv = dbContext.Conversations.Find(convID);
              if (conv != null)
              {
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
              var conv = dbContext.Conversations.Find(convID);
              if (conv != null)
              {
                 var tags = from t in conv.ConversationTags where t.TagName == tagName select t;
                 if (tags.Count() == 1)
                 {
                    conv.ConversationTags.Remove(tags.First());
                    dbContext.SaveChanges();
                 }
              }
           }
           catch (Exception ex)
           {
              logger.Error("Error in RemoveTagFromConversation", ex);
           }
        }

        private bool IsConversationFavourite(string convID, smsfeedbackEntities dbContext)
        {
           try
           {
              var conv = dbContext.Conversations.Find(convID);
              if (conv != null) { return conv.Starred; }
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

        public Conversation GetLatestConversationDetails(string convId, smsfeedbackEntities dbContext)
        {
           var conv = dbContext.Conversations.Find(convId);
           return conv;
        }

        public IEnumerable<SmsMessage> GetMessagesForConversation(string convID, smsfeedbackEntities dbContext)
        {
           //TODO: error handling & sanity checks
           //if the conversation is marked as "favourite" then all the messages will be "favorite"
           var isConvFavourite = IsConversationFavourite(convID, dbContext);
           var msgs = dbContext.Conversations.Find(convID).Messages.Select(msg =>
                           new SmsMessage()
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
                              Seconds = msg.TimeReceived.Second,
                              IsSmsBased = msg.IsSmsBased,
                              ClientDisplayName = msg.Conversation.Client.DisplayName,
                              ClientAcknowledge = msg.ClientAcknowledge                              
                           });
           if (msgs.Count() > 0)
           {
              return msgs.OrderBy(x => x.TimeReceived);
           }
           else
           {
              return new SmsMessage[] { };
           }
        }

        public Conversation MarkConversationAsRead(string convID, smsfeedbackEntities dbContext)
        {
           var conv = dbContext.Conversations.Find(convID);
           if (conv != null)
           {
              conv.Read = true;
              dbContext.SaveChanges();
              return conv;
           }
           //if there was no conversation associated to this convID 
           logger.ErrorFormat("No conversation with id {0} found", convID);
           return null;
        }

        #endregion

        /// <summary>
        /// Return all tags containing a certain string
        /// </summary>
        /// <param name="queryString"> the string sequence to look for</param>
        /// <param name="companyName"> the company to whom these tags belong to</param>
        /// <returns></returns>
        private IEnumerable<string> FindMatchingTagsForCompany(string queryString, string companyName, smsfeedbackEntities dbContext)
        {
            logger.Info("Call made");
            var tags = from tag in dbContext.Tags where (tag.CompanyName == companyName && tag.Name.Contains(queryString)) select tag.Name;
            return tags;
        }       

        private static Client GetClientByName(String sender, smsfeedbackEntities dbContext)
        {
            var clients = from cl in dbContext.Clients where cl.TelNumber == sender select cl;
            Client clientToBeAddedInDb;
            if (clients.Count() == 0)
            {
                var newClient = new Client()
                {
                    TelNumber = ConversationUtilities.CleanUpPhoneNumber(sender),
                    DisplayName = sender,
                    Description = "new client",
                    isSupportClient = false
                };
                dbContext.Clients.Add(newClient);
                dbContext.SaveChanges();
                clientToBeAddedInDb = newClient;
            }
            else clientToBeAddedInDb = clients.First();
            return clientToBeAddedInDb;
        }      

        /// <summary>
       /// Adds a message in the db - if outgoing computes response time
       /// </summary>
       /// <param name="from"></param>
       /// <param name="to"></param>
       /// <param name="conversationId"></param>
       /// <param name="text"></param>
       /// <param name="readStatus"></param>
       /// <param name="updateTime"></param>
        /// <param name="direction"></param>
       /// <param name="prevConvFrom"></param>
       /// <param name="prevConvUpdateTime"></param>
       /// <param name="isSmsBased"></param>
       /// <param name="XmppUser"></param>
       /// <param name="price"></param>
       /// <param name="externalID"></param>
       /// <param name="dbContext"></param>
       /// <returns>The ID of the inserted message</returns>
        public int AddMessage(
           String from, String to, String conversationId, String text,
           Boolean readStatus, DateTime updateTime, ConversationUtilities.Direction direction, String prevConvFrom, DateTime prevConvUpdateTime,
           bool isSmsBased, String XmppUser, String price, String externalID,
           smsfeedbackEntities dbContext)
        {
            //assume userID and convID are valid
            logger.Info("Call made");
            //for the response time -> the latest details are always in the conversation
            bool linkMessageWithXmppUser = !XmppUser.Equals(Constants.DONT_ADD_XMPP_USER);
            if (linkMessageWithXmppUser)
            {
                /* 
                 * It's very unlikely that the xmpp user which sends the message is not in db.
                 * However if this happens, the xmpp user is added in db.
                 */
                var xmppUsers = from xu in dbContext.XmppConnections where xu.XmppUser == XmppUser select xu;
                if (xmppUsers.Count() == 0)
                {
                    var newXmppUser = new XmppConnection()
                    {
                        XmppUser = XmppUser,
                        XmppPassword = "123456"
                    };
                    dbContext.XmppConnections.Add(newXmppUser);
                    dbContext.SaveChanges();
                }
            }

            long? responseTime = null;
            if (direction == ConversationUtilities.Direction.to)
            {
               //only compute for outgoing messages
               if (ConversationUtilities.GetDirectionForMessage(prevConvFrom, conversationId) == ConversationUtilities.Direction.from)
               {
                  //only compute for outgoing messages that follow an incoming message
                  responseTime = updateTime.Subtract(prevConvUpdateTime).Ticks;
               }
            }
            try
            {
                var msg = new Message()
                {
                    ResponseTime = responseTime,
                    From = from,
                    To = to,
                    Text = text,
                    TimeReceived = updateTime,
                    ConversationId = conversationId,
                    Read = readStatus,
                    IsSmsBased = isSmsBased,                    
                };
                if (!String.IsNullOrEmpty(price)) msg.Price = price;
                if (!String.IsNullOrEmpty(externalID)) msg.ExternalID = externalID;
                if (!XmppUser.Equals(Constants.DONT_ADD_XMPP_USER)) msg.XmppConnectionXmppUser = XmppUser;
                dbContext.Messages.Add(msg);
                dbContext.SaveChanges();
                return msg.Id;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in AddMessage", ex);
                Console.WriteLine("AddMessage Error = " + ex.Message);
                Console.WriteLine("AddMessage Error stack = " + ex.StackTrace + "||| & source " + ex.Source);
                return -1;
            }
        }

        public SmsFeedback_EFModels.SubscriptionDetail UpdateSMSstatusForCompany(String wpID, String price, smsfeedbackEntities dbContext)
        {            
            var wp = dbContext.WorkingPoints.Find(wpID);
            if (wp != null)
            {               
               var sd = wp.Users.FirstOrDefault().Company.SubscriptionDetail;
               UpdateSMSForSubscription(price, sd, dbContext);
               return sd;
            }
            return null;
        }

        public void UpdateSMSForSubscription(String price, SubscriptionDetail sd, smsfeedbackEntities dbContext)
        {
           /**
            *RemainingSMS - if still > 0 decrease
            *SpentAmount - increase with price only if RemainingSMS == 0           
            */
           bool saveFailed = true;
           do
           {
              saveFailed = false;
              try
              {
                 if (sd.RemainingSMS > 0)
                 {
                    sd.RemainingSMS = sd.RemainingSMS - 1;
                 }
                 else
                 {
                    decimal priceAsDecimal = 0;
                    try
                     {
                        priceAsDecimal = Decimal.Parse(price, CultureInfo.InvariantCulture);
                     }
                    catch(Exception ex)
                    {
                       logger.ErrorFormat("Price not a valid decimal. ErrorType: {0}, ErrorMessage: {1}",ex.GetType().ToString(), ex.Message);
                    }
                    sd.SpentThisMonth += priceAsDecimal;
                 }
                 dbContext.SaveChanges();
              }
              catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
              {
                 saveFailed = true;
                 ex.Entries.Single().Reload();
              }
           } while (saveFailed);                   
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
               //TODO issue if tag is found for another company!!!
               var companies = from u in dbContext.Users where u.UserName == userName select u.Company;
               var companyName = companies.First().Name;
                var tags = from t in dbContext.Tags where t.Name == tagName && t.Company.Name == companyName select t;
                if (tags.Count() == 0)
                {
                    var newTag = new Tag() { Name = tagName, Description = tagDescription, CompanyName = companyName };
                    dbContext.Tags.Add(newTag);
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
               
        public string GetNameForWorkingPoint(string wpTelNumberOrShortID, smsfeedbackEntities dbContext)
        {
            var wpName = from wp in dbContext.WorkingPoints where ((wp.TelNumber == wpTelNumberOrShortID) || (wp.ShortID == wpTelNumberOrShortID))  select wp.Name;
            if (wpName.Count() == 1)
            {
                return wpName.First();
            }
            else
            {
                return "";
            }
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
                   var newWP = dbContext.WorkingPoints.Find(wp.TelNumber);
                    //var newWp = from w in usr.WorkingPoints where w.TelNumber == wp.TelNumber select w;
                    if (newWP != null)
                    {
                        newWP.Name = wp.Name;
                        newWP.Description = wp.Description;
                        newWP.WelcomeMessage = wp.WelcomeMessage;
                    }
                }
                dbContext.SaveChanges();
            }

        }

        public SubscriptionSmsStatus MarkMessageActivityInDB(        
           String from, String to, String conversationId, String text, Boolean readStatus,
           DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime, bool isSmsBased, String XmppUser,  
           String price, String externalID, String direction, smsfeedbackEntities dbContext)
        {
            string updateAddConversationResult = UpdateOrAddConversation(from, to, conversationId, text, readStatus, updateTime, isSmsBased, dbContext);
            string addMessageResult = updateAddConversationResult;
            int newInsertedMessageID = -1;
           bool warningLimitReached = false;
           bool spendingLimitReached = false;           
            if (updateAddConversationResult.Equals(JsonReturnMessages.OP_SUCCESSFUL))
            {
               //compute direction and 
               ConversationUtilities.Direction dir = ConversationUtilities.Direction.from;
               if(direction == Constants.DIRECTION_OUT) { dir = ConversationUtilities.Direction.to;}
               newInsertedMessageID = AddMessage(from, to, conversationId, text, readStatus, updateTime,dir, prevConvFrom, prevConvUpdateTime, isSmsBased, XmppUser, price, externalID, dbContext);
            }
           //we added the message - now if SMS based, mark this 
            if ((newInsertedMessageID > 0) && isSmsBased && (direction == Constants.DIRECTION_OUT)) {
               var sd = UpdateSMSstatusForCompany(from,price, dbContext); 
               //if required emit warnings
               bool warningsRequired = sd.WarningsRequired();
               if (warningsRequired && sd != null)
               {
                  var mailer = new SmsFeedback_Take4.Mailers.WarningMailer();
                  var companyName = sd.Companies.FirstOrDefault().Name;
                  if (sd.CanSendSMS)
                  {
                     warningLimitReached = true;
                     //we need to send warnings
                     System.Net.Mail.MailMessage msgPrimary = mailer.WarningEmail(sd, sd.PrimaryContact.Email, sd.PrimaryContact.Name, sd.PrimaryContact.Surname);
                     msgPrimary.Send();
                     System.Net.Mail.MailMessage msgSecondary = mailer.WarningEmail(sd, sd.SecondaryContact.Email, sd.SecondaryContact.Name, sd.SecondaryContact.Surname);
                     msgSecondary.Send();
                  }
                  else
                  {
                     spendingLimitReached = true;
                     //we need to send SpendingLimit reached emails
                     System.Net.Mail.MailMessage msgPrimary = mailer.SpendingLimitReachedEmail(sd, sd.PrimaryContact.Email, sd.PrimaryContact.Name, sd.PrimaryContact.Surname);
                     msgPrimary.Send();
                     System.Net.Mail.MailMessage msgSecondary = mailer.SpendingLimitReachedEmail(sd, sd.PrimaryContact.Email, sd.SecondaryContact.Name, sd.SecondaryContact.Surname);
                     msgSecondary.Send();
                  }                                                      
               }
            }
            SubscriptionSmsStatus messageStatus = new SubscriptionSmsStatus(newInsertedMessageID, !spendingLimitReached, warningLimitReached, spendingLimitReached);
            return messageStatus;
        }

        public SubscriptionSmsStatus GetCompanySubscriptionSMSStatus(string loggedInUser, smsfeedbackEntities dbContext)
        {
           var user = (from usr in dbContext.Users where usr.UserName == loggedInUser select usr).FirstOrDefault();
           if (user != null)
           {
              Company company = user.Company;
              var sd = company.SubscriptionDetail;
              bool warningReached = false;
              bool spendingReached = false;
              SubscriptionSmsStatus status = new SubscriptionSmsStatus(0,false,warningReached,spendingReached);
              bool warningsRequired = sd.WarningsRequired();
              
              if (warningsRequired)
              {
                 if (sd.CanSendSMS)
                 {
                    status.WarningLimitReached = true;
                    status.WarningLimitReachedMessage = String.Format(Resources.Global.subscriptionWarningReached, sd.SpentThisMonth, sd.DefaultCurrency, sd.SpendingLimit, sd.GetNextBillingDate(DateTime.Now).ToLongDateString());
                 }
                 else {
                    status.SpendingLimitReached = true;
                    status.SpendingLimitReachedMessage = String.Format(Resources.Global.subscriptionSpendingReached, sd.SpendingLimit, sd.DefaultCurrency, sd.GetNextBillingDate(DateTime.Now).ToLongDateString());
                 }                 
              }
              return status;
           }
           logger.ErrorFormat("Invalid user id: {0}", loggedInUser);
           return null;
        }
        public void updateMsgClientAckField(int msgID, bool clientAcknowledge, smsfeedbackEntities dbContext)
        {
           var message = dbContext.Messages.Find(msgID);
           if (message != null)
           {
              message.ClientAcknowledge = clientAcknowledge;
              dbContext.SaveChanges();
           }
        }

    }
}