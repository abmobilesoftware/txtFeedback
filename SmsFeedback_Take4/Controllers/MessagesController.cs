using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_Take4.Models;
using SmsFeedback_EFModels;
using Twilio;
using Newtonsoft.Json;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]   
    public class MessagesController : BaseController
   {
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private smsfeedbackEntities mContext = new smsfeedbackEntities();

      private AggregateSmsRepository mSmsRepository;
      private AggregateSmsRepository SMSRepository
      {
         get {
            if (mSmsRepository == null) 
               mSmsRepository = AggregateSmsRepository.GetInstance(User.Identity.Name);            
            return mSmsRepository; 
         }        
      }
      
      private EFInteraction mEFInterface = new EFInteraction();      
      public ActionResult Index()
      {
         ViewData["Title"] = "SmsFeedback/Messages";
         return View();
      }

      public JsonResult WorkingPointsPerUser()
      {
         logger.Info("getting workingPoints per logged in user");
         try
         {
            if (HttpContext.Request.IsAjaxRequest())
            {
               var userId = User.Identity.Name;
               var workingPoints = SMSRepository.GetWorkingPointsPerUser(userId);
               System.Threading.Thread.Sleep(1000);
               return Json(workingPoints, JsonRequestBehavior.AllowGet);
            }
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in WorkingPointsPerUser", ex);
            return null;
         }
         return null;
      }
      public JsonResult MessagesList(string conversationID)
      {         
         //defend when conversationID is null
         if (conversationID == null)
         {            
            logger.Error("conversationID was null");
            return null;
         }
         try
         {
            logger.Debug(String.Format("Show messages for conversation: {0}", conversationID));
            return Json(SMSRepository.GetMessagesForConversation(conversationID), JsonRequestBehavior.AllowGet);
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in MessagesList", ex);
            return null;
         }

      }

      [HttpGet]
      public JsonResult MarkConversationAsRead(string conversationId)
      {
         logger.InfoFormat("Marking conversation [{0}] as read", conversationId);
         if (string.IsNullOrEmpty(conversationId))
            return Json("Please provide a conversationId", JsonRequestBehavior.AllowGet);

         mEFInterface.UpdateAddConversation(null, null, conversationId, null, true,null,true);
         return Json("Update successful", JsonRequestBehavior.AllowGet);         
      }

      private JsonResult GetMessagesFromDb(string conversationID)
      {         
         var records = from c in mContext.Messages
                       where c.ConversationId == conversationID
                       select
                          new { Id = c.Id, ConvID = conversationID, From = c.From, To = c.To, Text = c.Text, TimeReceived = c.TimeReceived, Read = c.Read };
         if (HttpContext.Request.IsAjaxRequest())
         {
            //System.Threading.Thread.Sleep(1000);
            return Json(records,
                          JsonRequestBehavior.AllowGet);
         }
         return null;
      }

      //todo add date from/date to to list
      public JsonResult ConversationsList(bool showAll,
                                            bool showFavourites,
                                            string[] tags,
                                            string[] workingPointsNumbers,
                                            int skip,
                                            int top)
      {
         //in our DB we hold the conversations we updated while online
         //it could be that we received messages while offline - so we require also these conversations
         try
         {
            System.Text.StringBuilder receivedTags = new System.Text.StringBuilder();
            if (tags != null)
            {
               foreach (String tag in tags)
               {
                  receivedTags.Append(tag);
                  receivedTags.Append(" ");
               }
            }
            System.Text.StringBuilder wps = new System.Text.StringBuilder();
            if (workingPointsNumbers != null)
            {
               foreach (String wp in workingPointsNumbers)
               {
                  wps.Append(wp);
                  wps.Append(" ");
               }
            }
            else
            {
               wps.Append(" []");
            }
            logger.Debug(String.Format("ConversationList requested: showAll: {0}, showFavourites {1}, tags: {2}, working points: {3}, skip: {4}, top: {5}",
                                        showAll.ToString(),
                                        showFavourites.ToString(),
                                        receivedTags.ToString(),
                                        wps.ToString(),
                                        skip,
                                        top));
            if (HttpContext.Request.IsAjaxRequest())
            {               
               var userId = User.Identity.Name;               
               var conversations = SMSRepository.GetConversationsForNumbers(showAll, showFavourites, tags, workingPointsNumbers, skip, top, null, userId);
               //System.Threading.Thread.Sleep(1000);
               return Json(conversations, JsonRequestBehavior.AllowGet);
            }
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in ConversationsList", ex);
         }
         return null;
      }

      private IQueryable GetConversationsFromDb(bool showAll,
                                               bool showFavourites,
                                               string[] tags,
                                               string[] workingPointsNumbers,
                                               int skip,
                                               int top)
      {
         IQueryable conversations;
         if (showAll)
            conversations = (from c in mContext.Conversations orderby c.TimeUpdated descending select new { ConvID = c.ConvId, TimeUpdated = c.TimeUpdated, Read = c.Read, Text = c.Text, From = c.From }).Skip(skip).Take(top);
         else
         {
            //filter according to numbers
            conversations = (from c in mContext.Conversations orderby c.TimeUpdated descending select new { ConvID = c.ConvId, TimeUpdated = c.TimeUpdated, Read = c.Read, Text = c.Text, From = c.From }).Skip(skip).Take(top);
         }
         return conversations;
      }

      public ActionResult MessageReceived(String from, String to, String text, DateTime receivedTime, bool readStatus)
      {
         logger.Debug(String.Format("Message received: from: {0}, to: {1}, text: {2}, receivedTime: {3}, readStatus: {4}", from, to, text, receivedTime.ToString(), readStatus.ToString()));
         String conversationId = from + "-" + to;
         AddMessageAndUpdateConversation(from, to, conversationId, text, readStatus, receivedTime);
         if (HttpContext.Request.IsAjaxRequest())
         {
            //I should return the sent time (if successful)              
            String response = "received successfully"; //TODO should be a class
            return Json(response, JsonRequestBehavior.AllowGet);
         }
         return View();
      }

      private void AddMessageAndUpdateConversation(String from, String to, String conversationId, String text, Boolean readStatus, DateTime updateTime)
      {
         string convID = mEFInterface.UpdateAddConversation(from, to, conversationId, text, readStatus, updateTime);
         //since now we guarantee that the conversation exits there is no need for convID
         mEFInterface.AddMessage(from, to, conversationId, text, readStatus, updateTime);         
      }

      public JsonResult SendMessage(String from, String to, String text)
      {
         if (HttpContext.Request.IsAjaxRequest())
         {
            String conversationId = to + "-" + from;
            AddMessageAndUpdateConversation(from, to, conversationId, text, true, DateTime.Now);
            //send message via twilio
            from = "442033221134";
            to = "442033221909";
            SMSRepository.SendMessage(from, to, text, (msg) =>
            {

            });
            //I should wait for the twilio call to finish
            //I should return the sent time (if successful)              
            String response = "sent successfully"; //TODO should be a class
            return Json(response, JsonRequestBehavior.AllowGet);
         }
         return null;
      }

   }
}
