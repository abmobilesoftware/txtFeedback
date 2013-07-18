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
using SmsFeedback_Take4.Models.Helpers;

namespace SmsFeedback_Take4.Controllers
{
   
    [CustomAuthorizeAtribute]
    public class MessagesController : BaseController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        smsfeedbackEntities context = new smsfeedbackEntities();
        public const string cMessageOrganizer = "MessageOrganizer";
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
        //public ActionResult Index()
        //{
        //    ViewData["currentCulture"] = getCurrentCulture();
        //    ViewData["messageOrganizer"] = HttpContext.User.IsInRole(cMessageOrganizer);
        //    return View();
        //}
        
        public JsonResult MessagesList(string conversationId, int top, int skip)
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
                return Json(SMSRepository.GetMessagesForConversation(conversationId, top, skip, context), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred in MessagesList", ex);
                return null;
            }

        }

        //todo add date from/date to to list
       
        public JsonResult DeleteMessage(String messageText, String convId, DateTime timeReceived)
        {
            if (convId == null)
            {
                logger.Error("no conversationId passed");
                return Json(new Error(Constants.NO_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
            }

            if (convId.Equals(Constants.NULL_STRING))
            {
                logger.Error("Conversation Id was null");
                return Json(new Error(Constants.NULL_CONVID_ERROR_MESSAGE), JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (HttpContext.User.IsInRole(cMessageOrganizer))
                {                    
                    string queryResult = mEFInterface.DeleteMessage(messageText, convId, timeReceived.ToUniversalTime(), context);
                    if (queryResult.Equals("last message"))
                    {
                        return Json("lastMessage", JsonRequestBehavior.AllowGet);
                    }
                    else if (queryResult.Equals("normal message"))
                    {
                        return Json(JsonReturnMessages.OP_SUCCESSFUL, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteMessage " + ex.Message);
                return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SMSSSubscriptionStatus()
        {
           var status = mEFInterface.GetCompanySubscriptionSMSStatus(User.Identity.Name, context);
           return Json(status, JsonRequestBehavior.AllowGet);
        }

       protected override void Dispose(bool disposing)
        {
           context.Dispose();
           base.Dispose(disposing);
        }       
    }
}
