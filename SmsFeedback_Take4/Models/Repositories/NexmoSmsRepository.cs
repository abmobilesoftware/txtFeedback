using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nexmo_CSharp_lib;
using Nexmo_CSharp_lib.Model.Request;
using Nexmo_CSharp_lib.Model.Response;


namespace SmsFeedback_Take4.Models
{
   [Serializable]
   public class MessageStatus
   {
      public bool MessageSent { get; set; }
      public string Status { get; set; }
      public DateTime DateSent { get; set; }
      public String ExternalID { get; set; }
      public String Price { get; set; }
   }

   [Serializable]
   public class NexmoSmsRepository : IExternalSmsRepository
   {      
      public System.Collections.Generic.IEnumerable<SmsMessage> GetConversationsForNumber(string workingPointsNumber,
                                                                                   DateTime? lastUpdate,
                                                                                   String userName)
      {
         throw new NotImplementedException();
      }

      public System.Collections.Generic.IEnumerable<SmsMessage> GetMessagesForConversation(string convID, bool isConvFavourite)
      {
         throw new NotImplementedException();
      }

      public void SendMessage(string from, string to, string message, Action<MessageStatus> callback)
      {
         var result = StaticSendMessage(from, to, message);
         callback(result);
         
      }
      public MessageStatus SendMessage(string from, string to, string message)
      {
         return StaticSendMessage(from, to, message);
      }

      
      private static bool ContainsUnicodeCharacter(string input)
      {
         //This will detect for extended ASCII. If you only detect for the true ASCII character range (up to 127), then you could potentially get false positives for extended ASCII characters which does not denote Unicode
         //http://blog.platformular.com/2012/03/07/determine-a-string-contains-unicode-character-c/
         const int MaxAnsiCode = 255;
         return input.Any(c => c > MaxAnsiCode);
      }

      public static MessageStatus StaticSendMessage(string from, string to, string message)
      {
         var textModel = new TextRequestModel { Text = message };
         var username = "5546d7a3";
         var password = "48e9a393";        
         var requestModel = RequestModelBuilder.Create(username, password, from, to, textModel);
         //find out if we are dealing with unicode characters
         if (ContainsUnicodeCharacter(message))
         {
            requestModel.Type = "unicode";
         }
         var nexmo = new Nexmo_CSharp_lib.Nexmo();
         JsonResponseModel responseModel = nexmo.Send(requestModel, ResponseObjectType.Json) as JsonResponseModel;
         var msg = responseModel.MessageModels.First();
         var status = msg.Status;
         var sent = msg.Status.Equals("Success", StringComparison.InvariantCultureIgnoreCase) ? true : false;
         var sentDate = DateTime.Now.ToUniversalTime();
         //DA: atm, when sending the message via nexmo we don't receive the sent date (or created date) so we use the current datestamp of the server (UTC format)        
         var response = new MessageStatus() { MessageSent = sent, DateSent = sentDate, Status = status, ExternalID=msg.MessageId,Price=msg.MessagePrice };
         return response;
      }
   }
}