using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nexmo_CSharp_lib;
using Nexmo_CSharp_lib.Model.Request;

namespace SmsFeedback_Take4.Models
{
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

      public void SendMessage(string from, string to, string message, Action<DateTime> callback)
      {
         StaticSendMessage(from, to, message, callback);
      }
      public static void StaticSendMessage(string from, string to, string message, Action<DateTime> callback)
      {
         var textModel = new TextRequestModel { Text = message };
         var username = "5546d7a3";
         var password = "48e9a393";
         //var from = "4915706100037"; 
         //var to = "4915706100034";

         var requestModel = RequestModelBuilder.Create(username, password, from, to, textModel);
         var nexmo = new Nexmo_CSharp_lib.Nexmo();
         var responseModel = nexmo.Send(requestModel, ResponseObjectType.Json);
         //dragos: atm, when sending the message via nexmo we don't receive the sent date (or created date) so we use the current datestamp of the server (UTC format)        
         callback(DateTime.Now.ToUniversalTime());
      }
   }
}