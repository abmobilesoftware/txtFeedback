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
            ViewData["currentCulture"] = getCurrentCulture();
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
        public JsonResult MessagesList(string conversationId)
        {
            //defend when conversationID is null
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
                logger.Debug(String.Format("Show messages for conversation: {0}", conversationId));
                smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                return Json(SMSRepository.GetMessagesForConversation(conversationId, lContextPerRequest), JsonRequestBehavior.AllowGet);
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
            smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();

            var conv = mEFInterface.MarkConversationAsRead(conversationId, lContextPerRequest);
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
                smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                var conv = mEFInterface.UpdateStarredStatusForConversation(conversationId, newStarredStatus.Value, lContextPerRequest);
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
                    smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                    if (performUpdateBefore.HasValue && performUpdateBefore.Value == true)
                    {
                        SMSRepository.UpdateConversationsFromExternalSources(null, null, userId, lContextPerRequest);
                    }
                    var lUnreadMsg = new KeyValuePair<string, int>("unreadConvs", SMSRepository.NrOfUnreadConversations(userId, lContextPerRequest));
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
        //todo add date from/date to to list
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
                    smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                    bool retrieveOnlyUnreadConversations = false;
                    if (onlyUnread.HasValue && onlyUnread.Value == true) retrieveOnlyUnreadConversations = true;
                    //decide if we want to update from external sources or not
                    var updateFromExternalSources = DecideIfUpdateFromExternalSourcesIsRequired(requestIndex);
                    if (!popUpSupport)
                    {
                        var conversations = SMSRepository.GetConversationsForNumbers(onlyFavorites, tags, workingPointsNumbers, startDateAsDate, endDateAsDate, retrieveOnlyUnreadConversations, skip, top, null, userId, updateFromExternalSources, lContextPerRequest);
                        return Json(conversations, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var conversations = SMSRepository.GetSupportConversationsForWorkingPoints(userId, workingPointsNumbers, skip, top, lContextPerRequest);
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

        private bool DecideIfUpdateFromExternalSourcesIsRequired(int requestIndex)
        {
            return requestIndex % 5 == 0;
        }

        //public JsonResult MessageReceived(String from, String to, String convId, String text, string receivedTime, bool readStatus)
        //{
        //   logger.Debug(String.Format("Message received: from: {0}, to: {1}, convId: {2}, text: {3}, receivedTime: {4}, readStatus: {5}", from, to, convId, text, receivedTime.ToString(), readStatus.ToString()));
        //   if (HttpContext.Request.IsAjaxRequest())
        //   {
        //      try
        //      {
        //         String conversationId = ConversationUtilities.BuildConversationIDFromFromAndTo(from, to);
        //         DateTime receivedTimeAsDate = Utilities.Rfc822DateTime.Parse(receivedTime);
        //         smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();

        //         AddMessageAndUpdateConversation(from, to, convId, text, readStatus, receivedTimeAsDate, lContextPerRequest);
        //         //I should return the sent time (if successful)              
        //         String response = "received successfully"; //TODO should be a class
        //         return Json(response, JsonRequestBehavior.AllowGet);

        //      }
        //      catch (Exception ex)
        //      {
        //         logger.Error("MessageReceived error", ex);
        //      }
        //   }
        //   return null;
        //}

        private void UpdateDbAfterMessageWasSent(String userId, String from, String to, String conversationId, String text, Boolean readStatus,
                                                     DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime, smsfeedbackEntities dbContext)
        {
            string convID = mEFInterface.UpdateAddConversation(from, to, conversationId, text, readStatus, updateTime, dbContext);
            mEFInterface.AddMessage(userId, from, to, conversationId, text, readStatus, updateTime, prevConvFrom, prevConvUpdateTime, dbContext);
            mEFInterface.IncrementNumberOfSentSms(from, dbContext);
        }

        public JsonResult SendMessage(String from, String to, String convId, String text)
        {
            if (HttpContext.Request.IsAjaxRequest())
            {
                logger.InfoFormat("SendMessage - from: [{0}], to: [{1}], convId: [{2}] text: [{3}]", from, to, convId, text);
                var userId = User.Identity.Name;

                try
                {
                    smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                    var previousConv = mEFInterface.GetLatestConversationDetails(convId, lContextPerRequest);

                    var prevConvFrom = previousConv.From;
                    var prevConvUpdateTime = previousConv.TimeUpdated;
                    SMSRepository.SendMessage(from, to, text, lContextPerRequest, (msgDateSent) =>
                    {
                        UpdateDbAfterMessageWasSent(userId, from, to, convId, text, false, msgDateSent, prevConvFrom, prevConvUpdateTime, lContextPerRequest);
                    });
                    //we should wait for the call to finish
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
                smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                var previousConv = mEFInterface.AddAnEventInConversationHistory(conversationId, eventType, lContextPerRequest);
                return Json("Successfully added", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("SendMessage error", ex);
            }

            return Json("Error", JsonRequestBehavior.AllowGet);
        }
    }
}
