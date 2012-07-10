using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_EFModels;

namespace SmsFeedback_Take4.Models
{
   public class ConversationsComparer : IEqualityComparer<SmsMessage>
   {
      bool IEqualityComparer<SmsMessage>.Equals(SmsMessage x, SmsMessage y)
      {
         return x.From.Equals(y.From);
      }

      int IEqualityComparer<SmsMessage>.GetHashCode(SmsMessage obj)
      {
         return obj.From.GetHashCode();
      }
   }

   public class MessagesComparer : IEqualityComparer<Twilio.SMSMessage>
   {
      //if we are dealing with 2 Twilio numbers then we will have the same message twice once with Sent, once with Received - so we have to consider them as 1 message
      bool IEqualityComparer<SMSMessage>.Equals(SMSMessage x, SMSMessage y)      {
         return x.From == y.From && x.To == y.To && x.DateSent == y.DateSent;
      }

      int IEqualityComparer<SMSMessage>.GetHashCode(SMSMessage obj)      {
         return obj.DateSent.GetHashCode();
      }
   }
   public class TwilioIntegrationSmsRepository : SmsFeedback_Take4.Models.IExternalSmsRepository 
    {
      private static  readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      
      public void SendMessage(string from, string to, string message, Action<string> callback )
      {
         logger.Info("Call made");
            var accoundSID = "ACf79c0f33a5f74621ac527c0d2ab30981";
            var authToken  = "cfdeca286645c1dca6674b45729a895c";            
           var twilio = new TwilioRestClient(accoundSID, authToken);
           twilio.SendSmsMessage(from, to, message,  (msg) =>  {
              //message was successfully sent
              logger.DebugFormat("Sent message with id: {0}", msg.Sid );
              callback("Sent successfully");
           });           
      }

      public IEnumerable<SmsMessage> GetMessagesForConversation(string convID)
      {
         logger.Info("Call made");
         //authenticate 
         var accoundSID = "ACf79c0f33a5f74621ac527c0d2ab30981";
         var authToken = "cfdeca286645c1dca6674b45729a895c";
         var twilio = new TwilioRestClient(accoundSID, authToken);

         string[] utilitiesGetFromAndToFromConversationID = ConversationUtilities.GetFromAndToFromConversationID(convID);
         var fromNumber = utilitiesGetFromAndToFromConversationID[0];
         var toNumber = utilitiesGetFromAndToFromConversationID[1];
         //var fromNumber = "442033221909"; //the client
         //var toNumber = "442033221134"; //the twilio number
         //get messages from a person
         SmsMessageResult listOfIncomingMessages = twilio.ListSmsMessages(toNumber, fromNumber, null, null, null);
         //get message to that person         
         SmsMessageResult listOfOutgoingMessages = twilio.ListSmsMessages(fromNumber, toNumber, null, null, null);
         //combine the results

         
         IEnumerable<SMSMessage> union = listOfIncomingMessages.SMSMessages.Union(listOfOutgoingMessages.SMSMessages).Distinct(new MessagesComparer());
         //this should be ordered ascending by datesent as the last will be at the end
         var records = from c in union orderby c.DateSent select new SmsMessage(){Id=c.Sid.GetHashCode(),From=c.From, To= c.To,Text= c.Body, TimeReceived =c.DateSent,Read=true, ConvID=convID };
         return records;
      }

      public IEnumerable<SmsMessage> GetConversationsForNumber(bool showAll,
                                                               bool showFavourites,
                                                               string[] tags,
                                                               string workingPointsNumber,
                                                               int skip,
                                                               int top,
                                                               DateTime? lastUpdate,
                                                               String userName)
      {
         //here we don't aplly skip or load, as they will be applied on the merged list
         logger.InfoFormat("Retrieving conversations, lastUpdate:{0}",lastUpdate);
         var accoundSID = "ACf79c0f33a5f74621ac527c0d2ab30981";
         var authToken = "cfdeca286645c1dca6674b45729a895c";
         var twilio = new TwilioRestClient(accoundSID, authToken);         
         //var toNumber = workingPointsNumbers;
         var toNumber = "+442033221134";         
         //the lastUpdate parameter will be used by default with the = operator. I need the >= parameter.
         SmsMessageResult res = twilio.ListSmsMessages(toNumber, "", lastUpdate, ComparisonType.GreaterThanOrEqualTo, null, null);
         //twilio.ListSmsMessages(
         var result = from c in res.SMSMessages orderby c.DateSent descending select  
                         new SmsMessage(){
                            Id=c.Sid.GetHashCode(),
                            From= c.From, 
                            To= c.To, 
                            Text= c.Body, 
                            TimeReceived= c.DateSent,
                            Read=false,
                            ConvID=Utilities.ConversationUtilities.BuildConversationIDFromFromAndTo(c.From,c.To) ,                            
                         };
         var ret = result.Distinct(new ConversationsComparer());
         logger.InfoFormat("Records returned by Twilio: {0}", ret.Count());
         return ret;
      }     
    }
}
