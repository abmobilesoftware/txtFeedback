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

            var conversation = (from conv in dbContext.Conversations where conv.ConvId == conversationId select conv).First();
            // get the last message of the conversation
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
                            addEventInConversationHistory(conversationId,
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
                            addEventInConversationHistory(conversationId,
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
                                dbContext.ConversationHistories.DeleteObject(convEvents.First());
                                dbContext.SaveChanges();
                            }
                        }

                    }
                    // there's no event for this conversation, add the first event
                    else
                    {
                        addEventInConversationHistory(conversationId,
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

        private void addEventInConversationHistory(string iConversationId, int iSequence, string iEventTypeName, DateTime iEventDate, int iMessageId, smsfeedbackEntities dbContext)
        {
            var conversationEvent = new ConversationHistory()
            {
                ConversationConvId = iConversationId,
                Sequence = iSequence,
                EventTypeName = iEventTypeName,
                Date = iEventDate,
                MessageId = iMessageId
            };
            dbContext.ConversationHistories.AddObject(conversationEvent);
            dbContext.SaveChanges();
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
                var conversations = from c in dbContext.Conversations where c.ConvId == conversationId select c;
                string convId = CONVERSATION_NOT_MODIFIED;
                var updateDateToInsert = updateTime.HasValue ? updateTime.Value.ToString() : "null";
                if (conversations.Count() > 0)
                {
                    return UpdateExistingConversation(sender, conversationId, text, readStatus, updateTime, dbContext, conversations, convId, updateDateToInsert);
                }
                else
                {
                    return AddNewConversation(sender, addressee, conversationId, text, readStatus, updateTime, isSmsBased, dbContext, ref conversations, ref convId, updateDateToInsert);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in AddMessageAndUpdateConversation", ex);
                return CONVERSATION_NOT_MODIFIED;
            }
        }

        private static string AddNewConversation(String sender, String addressee, String conversationId, String text, Boolean readStatus, DateTime? updateTime, bool isSmsBased, smsfeedbackEntities dbContext, ref IQueryable<Conversation> conversations, ref string convId, string updateDateToInsert)
        {
            logger.InfoFormat("Add new conversation: [{0}] with read: {1}, updateTime: {2}, text: [{3}], from: [{4}]", conversationId, readStatus.ToString(), updateDateToInsert, text, sender);
            // Check if the client of this coversation it's in db. 
            Client clientForThisConversation = GetClientByName(sender, dbContext);
            IEnumerable<SmsFeedback_EFModels.WorkingPoint> workingPointIDs;
            if (isSmsBased)
            {
                string consistentWP = ConversationUtilities.CleanUpPhoneNumber(addressee);
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
                dbContext.Conversations.AddObject(new Conversation
                {
                    ConvId = conversationId,
                    Text = text,
                    Read = readStatus,
                    TimeUpdated = updateTime.Value,
                    From = sender,
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
                dbContext.Clients.AddObject(newClient);
                dbContext.SaveChanges();
                clientToBeAddedInDb = newClient;
            }
            else clientToBeAddedInDb = clients.First();
            return clientToBeAddedInDb;
        }

        private string UpdateExistingConversation(String sender, String conversationId, String text, Boolean readStatus, DateTime? updateTime, smsfeedbackEntities dbContext, IQueryable<Conversation> conversations, string convId, string updateDateToInsert)
        {
            logger.InfoFormat("Updating conversation: [{0}] with read: {1}, updateTime: {2},  text: {3}, from {4}", conversationId, readStatus.ToString(), updateDateToInsert, text, sender);
            var currentConversation = conversations.First();
            convId = currentConversation.ConvId;
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

        public String AddMessage(String from, String to, String conversationId, String text,
           Boolean readStatus, DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime,
           bool isSmsBased, String XmppUser, smsfeedbackEntities dbContext)
        {
            //assume userID and convID are valid
            logger.Info("Call made");
            //for the response time -> the lastest details are always in the conversation
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
                    dbContext.XmppConnections.AddObject(newXmppUser);
                    dbContext.SaveChanges();
                }
            }

            long? responseTime = null;
            if (ConversationUtilities.GetDirectionForMessage(prevConvFrom, conversationId) == ConversationUtilities.Direction.from)
            {
                responseTime = updateTime.Subtract(prevConvUpdateTime).Ticks;
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
                    IsSmsBased = isSmsBased
                };
                if (!XmppUser.Equals(Constants.DONT_ADD_XMPP_USER)) msg.XmppConnectionXmppUser = XmppUser;
                dbContext.Messages.AddObject(msg);
                dbContext.SaveChanges();
                return JsonReturnMessages.OP_SUCCESSFUL;
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in AddMessage", ex);
                Console.WriteLine("AddMessage Error = " + ex.Message);
                Console.WriteLine("AddMessage Error stack = " + ex.StackTrace + "||| & source " + ex.Source);
                return ex.ToString();
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
            //if the conversation is marked as "favourite" then all the messages will be "favorite"
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
                               Seconds = msg.TimeReceived.Second,
                               IsSmsBased = msg.IsSmsBased
                           });
            if (msgs.Count() > 0)
            {
                return msgs.First().OrderBy(x => x.TimeReceived);
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
            var conv = (from c in dbContext.Conversations where c.ConvId == convId select c);
            if (conv.Count() > 0) return conv.First();
            else return null;
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

        public String UpdateDb(String from, String to, String conversationId, String text, Boolean readStatus,
                                                     DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime, bool isSmsBased, String XmppUser, smsfeedbackEntities dbContext)
        {
            string updateAddConversationResult = UpdateAddConversation(from, to, conversationId, text, readStatus, updateTime, isSmsBased, dbContext);
            string addMessageResult = updateAddConversationResult;
            if (updateAddConversationResult.Equals(JsonReturnMessages.OP_SUCCESSFUL)) addMessageResult = AddMessage(from, to, conversationId, text, readStatus, updateTime, prevConvFrom, prevConvUpdateTime, isSmsBased, XmppUser, dbContext);
            if (addMessageResult.Equals(JsonReturnMessages.OP_SUCCESSFUL)) IncrementNumberOfSentSms(from, dbContext);
            return addMessageResult;
        }

    }
}