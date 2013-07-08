using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VivacomLib;

namespace SmsFeedback_Take4.Models
{
   public class VivacomSmsRepository : IExternalSmsRepository
   {
      private Gateway gateway;

      public VivacomSmsRepository()
      {
         gateway = new Gateway();
      }

      public IEnumerable<SmsMessage> GetConversationsForNumber(string workingPointsNumber, DateTime? lastUpdate, string userName)
      {
         throw new NotImplementedException();
      }

      public IEnumerable<SmsMessage> GetMessagesForConversation(string convID, bool isConvFavourite)
      {
         throw new NotImplementedException();
      }

      public void SendMessage(string from, string to, string message, Action<MessageStatus> callback)
      {
         throw new NotImplementedException();
      }

      public MessageStatus SendMessage(string from, string to, string message)
      {         
         ResponseCode response = gateway.SendSM(from, to, message, false);
         MessageStatus messageStatus = new MessageStatus();
         messageStatus.MessageSent = response == ResponseCode.OK ? true : false;
         messageStatus.DateSent = DateTime.UtcNow;
         return messageStatus;
      }
   }
}