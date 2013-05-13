using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_EFModels;

namespace mobile.TxtFeedback_take1.Controllers
{
    public class MessagesController : Controller
    {
        smsfeedbackEntities dbContext = new smsfeedbackEntities();
        /*
         * xmppID has the format user@domain.tld
         * xmppUser is the user part
         */
        public JsonResult GetConversationHistory(String convId, int top)
        {
           if (convId != null)
            {                
                var messages = (from conv in dbContext.Conversations
                                where conv.ConvId == convId
                                select
                                    (from msg in conv.Messages
                                     select new
                                     {
                                         From = msg.From,
                                         To = msg.To,
                                         Id = msg.Id,
                                         Text = msg.Text,
                                         TimeReceived = msg.TimeReceived,
                                         ConvId = conv.ConvId
                                     })).SelectMany(x => x).
                                   OrderByDescending(msg => msg.TimeReceived).
                                   Take(top);
                return Json(messages, JsonRequestBehavior.AllowGet);
            }
            return Json("request failed", JsonRequestBehavior.AllowGet);
        }

    }
}
