using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Models.ViewModels;

namespace SmsFeedback_Take4.Services
{
   // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Conversations" in code, svc and config file together.
   // NOTE: In order to launch WCF Test Client for testing this service, please select Conversations.svc or Conversations.svc.cs at the Solution Explorer and start debugging.
   [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
   public class Conversations : IConversations
   {
      public System.Linq.IQueryable GetConversationsList(bool showAll, bool showTagged, string[] tags)
      {
         MessagesContext mMsgContext = MessagesContext.getInstance();
         System.Linq.IQueryable conversations;
         if (showAll)
            conversations = mMsgContext.Conversations;
         else
         {
            conversations = mMsgContext.Conversations;
         }
         return conversations;
      }
      public string DoWork()
      {
         return "successful";
      }
   }
}
