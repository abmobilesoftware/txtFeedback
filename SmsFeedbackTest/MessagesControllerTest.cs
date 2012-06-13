using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmsFeedback_Take4.Controllers;

namespace SmsFeedbackTest
{
   [TestClass]
   public class MessagesControllerTest
   {
      [TestMethod]
      public void Conversations_ShowAll_returnsConversationsWithACertainFormat()
      {
         //the JSON format we are looking for contains (case sensitive)
         //ConvID, TimeUpdated, Read, Text, From
         var controller = new MessagesController();         
         System.Web.Mvc.JsonResult lConversations = controller.ConversationsList(true, false, null, null, 0, 10);
         var data = lConversations.Data;

         //ConvID = c.ConvId, TimeUpdated = c.TimeUpdated, Read = c.Read, Text = c.Text, From = c.From }
      }
   }
}
