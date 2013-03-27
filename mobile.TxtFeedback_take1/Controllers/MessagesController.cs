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
        public JsonResult GetUserHistory(String xmppUser, int top)
        {
            if (xmppUser != null)
            {
                top = (top == null) ? 4 : top;
                var messages = (from conv in dbContext.Conversations
                                where conv.ConvId.StartsWith(xmppUser)
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
