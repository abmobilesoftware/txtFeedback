using SmsFeedback_Take4.Models;
using SmsFeedback_Take4.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_Take4.Utilities;
using SmsFeedback_EFModels;
using Twilio;
using Newtonsoft.Json;

namespace SmsFeedback_Take4.Controllers
{
    public class ComponentController : Controller
    {
        private EFInteraction mEFInterface = new EFInteraction();
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

        // GET: /Component/GetHandlerForMessage
        public JsonResult GetHandlerForMessage()
        {
            Agent agent1 = new Agent("support_dev_de@txtfeedback.net", 7);
            Agent agent2 = new Agent("dragos@txtfeedback.net", 7);
            List<Agent> agents = new List<Agent> { agent1, agent2 };
            MsgHandlers listOfAgents = new MsgHandlers(agents);
            return Json(listOfAgents, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkingPointForCertainAddress(String iIMAddress)
        {
            SmsFeedback_Take4.Models.WorkingPoint wp = new SmsFeedback_Take4.Models.WorkingPoint("07897951452", "hei", "nexmo");
            wp.NrOfSentSmsThisMonth = 10;
            wp.MaxNrOfSmsToSendPerMonth = 100;
            return Json(wp, JsonRequestBehavior.AllowGet);
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
                    SMSRepository.SendMessage(from, to, text, lContextPerRequest, (msgResponse) =>
                    {
                        //TODO add check if message was sent successfully 
                        UpdateDbAfterMessageWasSent(userId, from, to, convId, text, false, msgResponse.DateSent, prevConvFrom, prevConvUpdateTime, lContextPerRequest);
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

         private void UpdateDbAfterMessageWasSent(String userId, String from, String to, String conversationId, String text, Boolean readStatus,
                                                     DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime, smsfeedbackEntities dbContext)
        {
            string convID = mEFInterface.UpdateAddConversation(from, to, conversationId, text, readStatus, updateTime, dbContext);
            mEFInterface.AddMessage(userId, from, to, conversationId, text, readStatus, updateTime, prevConvFrom, prevConvUpdateTime, dbContext);
            mEFInterface.IncrementNumberOfSentSms(from, dbContext);
        }
    }
}
