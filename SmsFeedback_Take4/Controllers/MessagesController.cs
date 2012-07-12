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

      private AggregateSmsRepository mSmsRepository;
      private AggregateSmsRepository SMSRepository
      {
         get
         {
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
               smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();

               var workingPoints = SMSRepository.GetWorkingPointsPerUser(userId, lContextPerRequest);
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
            smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
            return Json(SMSRepository.GetMessagesForConversation(conversationID,lContextPerRequest), JsonRequestBehavior.AllowGet);
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
         {
            return Json("Please provide a conversationId", JsonRequestBehavior.AllowGet);
         }
         smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();

         mEFInterface.MarkConversationAsRead(conversationId, lContextPerRequest);
         return Json("Update successful", JsonRequestBehavior.AllowGet);

      }
      [HttpGet]
      public JsonResult ChangeStarredStatusForConversation(string convID, bool? newStarredStatus)
      {
         logger.InfoFormat("Changing starred status for conversation [{0}] ", convID);
         if (string.IsNullOrEmpty(convID))
         {
            return Json("Please provide a conversationId", JsonRequestBehavior.AllowGet);
         }
         if (newStarredStatus.HasValue)
         {
            smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
            mEFInterface.UpdateStarredStatusForConversation(convID, newStarredStatus.Value, lContextPerRequest);
            return Json("Update successful", JsonRequestBehavior.AllowGet);
         }
         else {
            return Json("Please provide a valid starredStatus", JsonRequestBehavior.AllowGet);
         }
      }

      //todo add date from/date to to list
      public JsonResult ConversationsList(
                                            bool onlyFavorites,
                                            string[] tags,
                                            string[] workingPointsNumbers,
                                            string startDate,
                                            string endDate,
                                            bool? onlyUnread,
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
            logger.Debug(String.Format("ConversationList requested: onlyFavourites {0}, tags: {1}, working points: {2}, skip: {3}, top: {4}",                                        
                                        onlyFavorites.ToString(),
                                        receivedTags.ToString(),
                                        wps.ToString(),
                                        skip,
                                        top));
            if (HttpContext.Request.IsAjaxRequest())
            {
               var userId = User.Identity.Name;
               //we get the dates as strings so it is up to us to convert them to dates
               DateTime? startDateAsDate = null;
               DateTime? endDateAsDate = null;
               if (!String.IsNullOrEmpty(startDate) && !startDate.Equals("null"))
               {
                  //dateFormatForDatePicker = "dd-mm-yy";
                  var dateInfo = startDate.Split('-');
                  startDateAsDate = new DateTime(Int32.Parse(dateInfo[2]), Int32.Parse(dateInfo[1]), Int32.Parse(dateInfo[0]));
                  if (startDateAsDate.Value.Date == DateTime.Now.Date) startDateAsDate = null;
               }
               if (!String.IsNullOrEmpty(endDate) && !endDate.Equals("null"))
               {
                  var dateInfo = endDate.Split('-');
                  endDateAsDate = new DateTime(Int32.Parse(dateInfo[2]), Int32.Parse(dateInfo[1]), Int32.Parse(dateInfo[0]));
                  if (endDateAsDate.Value.Date == DateTime.Now.Date) endDateAsDate = null;
               }
               smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
               bool retrieveOnlyUnreadConversations = false;
               if (onlyUnread.HasValue && onlyUnread.Value == true) retrieveOnlyUnreadConversations = true;
               var conversations = SMSRepository.GetConversationsForNumbers(onlyFavorites, tags, workingPointsNumbers, startDateAsDate, endDateAsDate, retrieveOnlyUnreadConversations, skip, top, null, userId, lContextPerRequest);
               return Json(conversations, JsonRequestBehavior.AllowGet);

            }
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in ConversationsList", ex);
         }
         return null;
      }

      public JsonResult MessageReceived(String from, String to, String text, string receivedTime, bool readStatus)
      {
         logger.Debug(String.Format("Message received: from: {0}, to: {1}, text: {2}, receivedTime: {3}, readStatus: {4}", from, to, text, receivedTime.ToString(), readStatus.ToString()));
         if (HttpContext.Request.IsAjaxRequest())
         {
            try
            {
               String conversationId = ConversationUtilities.BuildConversationIDFromFromAndTo(from, to);
               DateTime receivedTimeAsDate = Utilities.Rfc822DateTime.Parse(receivedTime);
               smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();

               AddMessageAndUpdateConversation(from, to, conversationId, text, readStatus, receivedTimeAsDate, lContextPerRequest);
               //I should return the sent time (if successful)              
               String response = "received successfully"; //TODO should be a class
               return Json(response, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
               logger.Error("MessageReceived error", ex);
            }
         }
         return null;
      }

      private void AddMessageAndUpdateConversation(String from, String to, String conversationId, String text, Boolean readStatus, DateTime updateTime, smsfeedbackEntities dbContext)
      {
         string convID = mEFInterface.UpdateAddConversation(from, to, conversationId, text, readStatus, updateTime, dbContext);
         //since now we guarantee that the conversation exits there is no need for convID
         // mEFInterface.AddMessage(from, to, conversationId, text, readStatus, updateTime);         
      }

      public JsonResult SendMessage(String from, String to, String text)
      {
         if (HttpContext.Request.IsAjaxRequest())
         {
            logger.InfoFormat("SendMessage - from: [{0}], to: [{1}], text: [{2}]", from, to, text);
            String conversationId = to + "-" + from;
            try
            {
               smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
               AddMessageAndUpdateConversation(from, to, conversationId, text, true, DateTime.Now, lContextPerRequest);

               //send message via twilio
               //from = "442033221134";
               //to = "442033221909";
               SMSRepository.SendMessage(from, to, text, (msg) =>
               {

               });
               //I should wait for the twilio call to finish
               //I should return the sent time (if successful)              
               String response = "sent successfully"; //TODO should be a class
               return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
               logger.Error("SendMessage error", ex);
            }
         }
         return null;
      }

   }
}
