using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_EFModels;

namespace SmsFeedback_Take4.Models
{

   public class AggregateSmsRepository 
   {

      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private TwilioIntegrationSmsRepository mTwilioRep = new TwilioIntegrationSmsRepository();
      private EFSmsRepository mEFRep = new EFSmsRepository();
      private EFInteraction mEFInterface = new EFInteraction();
      private string LoggedInUser { get; set; }
      private static AggregateSmsRepository _instance = null;
      #region "Constructors"
      private AggregateSmsRepository()
      {

      }
      private AggregateSmsRepository(string loggedInUserName)
      {
         LoggedInUser = loggedInUserName;
      }

      //right now this is a singleton - check with multiple logged in users for issues
      public static AggregateSmsRepository GetInstance(string loggedInUserName)
      {
         if (_instance == null)
         {
            _instance = new AggregateSmsRepository(loggedInUserName);
         }
         return _instance;
      }
#endregion
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
         //the convention is that if workingPoints number is empty then we retrieve all the conversations
         if (workingPointsNumbers == null)
         {//we have to get all the working points 
            workingPointsNumbers = (from wp in mEFRep.GetWorkingPointsPerUser(userName,dbContext) select wp.TelNumber).ToArray();            
         }
        
         try
         {
            //in our db we have the conversations
            //we could have additional conversations (which have not been yet read) in Twilio
            //A possible optimization would be that for favourites we don't update with the twilio conversations (as the favourites is our own concept)
            //for Unread we have to retrieve data from Twilio as new twilio conversations are by default unread
            
               //we may have the following scenario - we get ask for first 10 records, skip 0
               //in our db we have only "old" conversations, so the twilio db returns 40 records
               //we then ask for "more conversations"- next 10, skip 10 - this will end up being applied on the twilio data
               //it is inneficient to retrieve all records from our db, and to merge them afterwards (not to mention very slow)

               //TODO check if we have access to the given numbers to avoid a security breach
               //first update our db with the latest from twilio (nexmo) then do our conditional select
               Dictionary<string, SmsMessage> lastConversations = mEFRep.GetLatestConversationForNumbers(workingPointsNumbers, userName,dbContext);
               foreach (var item in lastConversations)
               {
                  if (item.Value != null)
                  {
                     //unfortunatelly twilio is rather imprecisse (YYYY-MM-DD) -> we could/will get the same messages twice 
                     //we'll tackle this in the addupdate logic
                     var lastConversationTime = item.Value.TimeReceived; 
                     IEnumerable<SmsMessage> twilioConversationsForNumbers = mTwilioRep.GetConversationsForNumber(showAll, showFavourites, tags, item.Key, skip, top, lastConversationTime, userName);
                     //we need to add the twilio conversations to our conversations list
                     AddTwilioConversationsToDb(twilioConversationsForNumbers,dbContext);
                     //TODO - remove the break
                     break; //temporary - until we have real data
                     //now we can select from our db the latest conversations
                  }
                  else
                  {
                     IEnumerable<SmsMessage> twilioConversationsForNumbers = mTwilioRep.GetConversationsForNumber(showAll, showFavourites, tags, item.Key, skip, top, lastUpdate, userName);
                     AddTwilioConversationsToDb(twilioConversationsForNumbers,dbContext);
                  }
               }
               IEnumerable<SmsMessage> efConversationsForNumbers = mEFRep.GetConversationsForNumbers(showAll, showFavourites, tags, workingPointsNumbers,startDate,endDate,onlyUnread, skip, top, lastUpdate, userName,dbContext);
               return efConversationsForNumbers;           
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred", ex);
         }
         return null;
      }

      private void AddTwilioConversationsToDb(IEnumerable<SmsMessage> twilioConversationsForNumbers, smsfeedbackEntities dbContext)
      {
         foreach (SmsMessage sms in twilioConversationsForNumbers)
         {
            mEFInterface.UpdateAddConversation(sms.From, sms.To, sms.ConvID, sms.Text, sms.Read, sms.TimeReceived,dbContext);
         }
      }

      public IEnumerable<SmsMessage> GetMessagesForConversation(string convID, smsfeedbackEntities dbContext)
      {
         //if the conversation is marked as "favourite" then all the messages will be "favourite"
         var isConvFavourite = mEFInterface.IsConversationFavourite(convID, dbContext);
         //we get the messages for a certain conversation from Twilio 
         return mTwilioRep.GetMessagesForConversation(convID,isConvFavourite);
      }

      public IEnumerable<ConversationTag> GetTagsForConversation(string convID, smsfeedbackEntities dbContext)
      {
         return mEFRep.GetTagsForConversation(convID,dbContext);
      }

      public void SendMessage(string from, string to, string message, Action<string> callback)
      {
         mTwilioRep.SendMessage(from, to, message, callback);
      }

      public System.Collections.Generic.IEnumerable<WorkingPoint> GetWorkingPointsPerUser(String userName, smsfeedbackEntities dbContext)
      {
         return mEFRep.GetWorkingPointsPerUser(userName,dbContext);
      }      
   }
}