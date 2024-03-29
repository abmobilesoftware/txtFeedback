﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Models;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
    public class ConversationsController : BaseController
    {
       private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       smsfeedbackEntities context = new smsfeedbackEntities();
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

       #region JSON Interface
       [HttpGet]
       public JsonResult MarkConversationAsRead(string conversationId)
       {
          if (conversationId == null)
          {
             logger.Error("no conversationId passed");
             return Json(new Error(Constants.NO_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          if (conversationId.Equals(Constants.NULL_STRING))
          {
             logger.Error("conversationId was null");
             return Json(new Error(Constants.NULL_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }
          logger.InfoFormat("Marking conversation [{0}] as read", conversationId);
          var conv = mEFInterface.MarkConversationAsRead(conversationId, context);
          if (conv != null)
          {
             return Json("Update successful", JsonRequestBehavior.AllowGet);
          }
          else
          {
             //convId was invalid
             return null;
          }
       }

       [HttpGet]
       public JsonResult ChangeStarredStatusForConversation(string conversationId, bool? newStarredStatus)
       {
          if (conversationId == null)
          {
             logger.Error("no conversationId passed");
             return Json(new Error(Constants.NO_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          if (conversationId.Equals(Constants.NULL_STRING))
          {
             logger.Error("conversationId was null");
             return Json(new Error(Constants.NULL_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }
          if (newStarredStatus.HasValue)
          {
             logger.InfoFormat("Changing starred status for conversation [{0}] to {1}", conversationId, ((bool)newStarredStatus ? "True" : "False"));
             var conv = mEFInterface.UpdateStarredStatusForConversation(conversationId, newStarredStatus.Value, context);
             if (conv != null)
             {
                return Json("Update successful", JsonRequestBehavior.AllowGet);
             }
             else
             {
                //most likely the convId was invalid
                return null;
             }
          }
          else
          {
             logger.Error("Please provide a valid starredStatus");
             return Json(new Error(Constants.NULL_STARRED_STATUS_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }
       }

       public JsonResult NrOfUnreadConversations(bool? performUpdateBefore)
       {
          try
          {
             logger.DebugFormat("performUpdateBefore: {0}", performUpdateBefore.HasValue ? performUpdateBefore.Value.ToString() : "null");
             if (HttpContext.Request.IsAjaxRequest())
             {
                var userId = User.Identity.Name;
                if (performUpdateBefore.HasValue && performUpdateBefore.Value == true)
                {
                   SMSRepository.UpdateConversationsFromExternalSources(null, null, userId, context);
                }
                var lUnreadMsg = new KeyValuePair<string, int>("unreadConvs", SMSRepository.NrOfUnreadConversations(userId, context));
                return Json(lUnreadMsg, JsonRequestBehavior.AllowGet);
             }
             return null;
          }
          catch (Exception ex)
          {
             logger.Error("Error occurred in NrOfUnreadConversations", ex);
             return null;
          }
       }

       public JsonResult ConversationsList(
                                              bool onlyFavorites,
                                              string[] tags,
                                              string[] workingPointsNumbers,
                                              string startDate,
                                              string endDate,
                                              bool? onlyUnread,
                                              int requestIndex,
                                              int skip,
                                              int top,
                                              bool popUpSupport)
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
             logger.Debug(String.Format("ConversationList requested: onlyFavourites {0}, tags: {1}, working points: {2}, skip: {3}, top: {4}, requestIndex: {5}",
                                         onlyFavorites.ToString(),
                                         receivedTags.ToString(),
                                         wps.ToString(),
                                         skip,
                                         top, requestIndex));
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
                bool retrieveOnlyUnreadConversations = false;
                if (onlyUnread.HasValue && onlyUnread.Value == true) retrieveOnlyUnreadConversations = true;
                //decide if we want to update from external sources or not
                var updateFromExternalSources = DecideIfUpdateFromExternalSourcesIsRequired(requestIndex);
                if (!popUpSupport)
                {
                   var conversations = SMSRepository.GetConversationsForNumbers(
                      onlyFavorites,
                      tags,
                      workingPointsNumbers,
                      startDateAsDate,
                      endDateAsDate,
                      retrieveOnlyUnreadConversations,
                      skip,
                      top,
                      null,
                      userId,
                      updateFromExternalSources,
                      context);
                   return Json(conversations, JsonRequestBehavior.AllowGet);
                }
                else
                {
                   var conversations = SMSRepository.GetSupportConversationsForWorkingPoints(userId, workingPointsNumbers, skip, top, context);
                   return Json(conversations, JsonRequestBehavior.AllowGet);
                }
             }
          }
          catch (Exception ex)
          {
             logger.Error("Error occurred in ConversationsList", ex);
          }
          return null;
       }

       public JsonResult DeleteConversation(String convId)
       {
          if (convId == null)
          {
             logger.Error("No conversationId passed");
             return Json(new Error(Constants.NO_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          if (convId.Equals(Constants.NULL_STRING))
          {
             logger.Error("Conversation Id was null");
             return Json(new Error(Constants.NULL_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          try
          {
             if (HttpContext.User.IsInRole(MessagesController.cMessageOrganizer))
             {
                mEFInterface.DeleteConversation(convId, context);
                return Json(JsonReturnMessages.OP_SUCCESSFUL, JsonRequestBehavior.AllowGet);
             }
             return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
          }
          catch (Exception ex)
          {
             logger.Error("DeleteConversation " + ex.Message);
             return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
          }
       }

       public JsonResult UpdateConversation(string convId, string newText, DateTime newTextReceivedDate)
       {
          DateTime newTextReceivedDateUTC = newTextReceivedDate.ToUniversalTime();
          if (convId == null)
          {
             logger.Error("No conversationId passed");
             return Json(new Error(Constants.NO_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          if (convId.Equals(Constants.NULL_STRING))
          {
             logger.Error("Conversation Id was null");
             return Json(new Error(Constants.NULL_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          try
          {
             if (HttpContext.User.IsInRole(MessagesController.cMessageOrganizer))
             {
                mEFInterface.UpdateConversationText(convId, newText, newTextReceivedDateUTC, context);
                return Json(JsonReturnMessages.OP_SUCCESSFUL, JsonRequestBehavior.AllowGet);
             }
             return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
          }
          catch (Exception ex)
          {
             logger.Error("UpdateConversation " + ex.Message);
             return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
          }
       }

       public JsonResult AddAnEventInConversationHistory(String conversationId, String eventType)
       {
          if (conversationId == null)
          {
             logger.Error("no conversationId passed");
             return Json(new Error(Constants.NO_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          if (conversationId.Equals(Constants.NULL_STRING))
          {
             logger.Error("conversationId was null");
             return Json(new Error(Constants.NULL_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
          }

          try
          {
             mEFInterface.AddAnEventInConversationHistory(conversationId, eventType, context);
             return Json("Successfully added", JsonRequestBehavior.AllowGet);
          }
          catch (Exception ex)
          {
             logger.Error("SendMessage error", ex);
          }

          return Json("Error", JsonRequestBehavior.AllowGet);
       }

       #endregion

       #region Internal Methods
       private bool DecideIfUpdateFromExternalSourcesIsRequired(int requestIndex)
       {
          return requestIndex % 5 == 0;
       }
       #endregion

       protected override void Dispose(bool disposing)
       {
          context.Dispose();
          base.Dispose(disposing);
       }
       
    }
}
