using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Models;
using SmsFeedback_Take4.Models.ViewModels;
using SmsFeedback_EFModels;

namespace SmsFeedback_Take4.Controllers
{
   [Authorize]
    public class MessagesController : Controller
    {
       private MessagesContext mMsgContext = MessagesContext.getInstance();
        //
        // GET: /Messages/

        public ActionResult Index()
        {
           return View(mMsgContext);
        }
   
        public ActionResult MessagesList(string conversationId)
        {
           var messages = mMsgContext.getMessagesForConversation(conversationId);
           if (HttpContext.Request.IsAjaxRequest())
           {
              System.Threading.Thread.Sleep(1000);
              return Json(messages,
                            JsonRequestBehavior.AllowGet);
           }

           return View(mMsgContext);
        }

      //todo add date from/date to to list
        public ActionResult ConversationsList(bool showAll,
                                              bool showTagged,
                                              string[] tags)
        {
           //todo here I have a small bug - the tags contains all the tags in the first element
           IQueryable  conversations;
           if (showAll)
              conversations = mMsgContext.Conversations;
           else           {
              conversations = mMsgContext.Conversations;
           }
           if (HttpContext.Request.IsAjaxRequest())
           {
              System.Threading.Thread.Sleep(1000);
              return Json(conversations, JsonRequestBehavior.AllowGet);
           }
           return View(mMsgContext);
        }

        public ActionResult SendMessage(String from, String to, String text)
        {
           if (HttpContext.Request.IsAjaxRequest())
           {
              System.Threading.Thread.Sleep(1000);
              String response = "sent successfully"; //TODO should be a class
              return Json(response, JsonRequestBehavior.AllowGet);
           }
           return View(mMsgContext);
        }

        //delegate Message MsgDelegate(int messageID);
        //public ActionResult Details(int id = 0)
        //{
        //   foreach (Message msg in mMsgContext.MyMessages )
        //   {
        //      if (msg.ID == id)
        //      {
        //         return View(msg);
        //      }
        //   }
        //   return HttpNotFound();
        //}       

      //[HttpPost, ActionName("Delete")]
      //  public ActionResult DeleteConfirmed(int id = 0)
      //  {
      //     Message msgToDelete = null;
      //     foreach (Message msg in mMsgContext.MyMessages)
      //     {
      //        if (msg.ID == id)
      //        {
      //           msgToDelete = msg;
      //        }
      //     }            
      //     if (msgToDelete != null)
      //     {
      //        mMsgContext.MyMessages.Remove(msgToDelete);
      //     }
      //     return RedirectToAction("Index");
      //  }
    }
}
