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
            SmsFeedback_Take4.Models.WorkingPoint wp = new SmsFeedback_Take4.Models.WorkingPoint("07897951452", "hei", "nexmo","defaultShortID");
            wp.NrOfSentSmsThisMonth = 10;
            wp.MaxNrOfSmsToSendPerMonth = 100;
            return Json(wp, JsonRequestBehavior.AllowGet);
        }

         private void UpdateDb(String from, String to, String conversationId, String text, Boolean readStatus,
                                                     DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime, bool isSmsBased, String XmppUser, smsfeedbackEntities dbContext)
        {
            string convID = mEFInterface.UpdateAddConversation(from, to, conversationId, text, readStatus, updateTime, dbContext);
            mEFInterface.AddMessage(from, to, conversationId, text, readStatus, updateTime, prevConvFrom, prevConvUpdateTime, isSmsBased, XmppUser, dbContext);
            mEFInterface.IncrementNumberOfSentSms(from, dbContext);
        }

        public JsonResult SaveMessage(String from, String to, String convId, String text, String xmppUser, bool isSms)
        {

            logger.InfoFormat("SendMessage - from: [{0}], to: [{1}], convId: [{2}] text: [{3}]", from, to, convId, text);
            
            try
            {
                /* 
                 * get the previous conversation from and time.
                 */ 
                smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                var previousConv = mEFInterface.GetLatestConversationDetails(convId, lContextPerRequest);
                String prevConvFrom = "";
                DateTime prevConvUpdateTime = DateTime.MinValue;

                if (previousConv != null)
                {
                    prevConvFrom = previousConv.From;
                    prevConvUpdateTime = previousConv.TimeUpdated;
                }
                /* Compute message direction */
                var splitSymbolsArray = new [] {'-'};
                String[] fromTo = convId.Split(splitSymbolsArray);
                var direction = Constants.DIRECTION_OUT;
                if (!from.Equals(fromTo.First())) direction = Constants.DIRECTION_IN;

                /*
                 * treat the cases SMS - in, out
                 *                 IM - in, out
                 */                 
                if (isSms)
                {
                    if (direction.Equals(Constants.DIRECTION_OUT)) {
                        SMSRepository.SendMessage(from, to, text, lContextPerRequest, (msgResponse) =>
                        {
                            //TODO add check if message was sent successfully 
                            UpdateDb(from, to, convId, text, false, msgResponse.DateSent, prevConvFrom, prevConvUpdateTime, true, xmppUser, lContextPerRequest);
                        });
                        //we should wait for the call to finish
                        //I should return the sent time (if successful)              
                        String response = "success"; //TODO should be a class
                        return Json(response, JsonRequestBehavior.AllowGet);
                    } else {
                        // the message should already be stored in DB by the php script
                        
                        // UpdateDb(from, to, convId, text, false, msgResponse.DateSent, prevConvFrom, prevConvUpdateTime, true, xmppUser, lContextPerRequest);
                        String response = "success"; //TODO should be a class
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                } else {
                    UpdateDb(from, to, convId, text, false, DateTime.Now, prevConvFrom, prevConvUpdateTime, true, xmppUser, lContextPerRequest);
                    String response = "success"; //TODO should be a class
                    return Json(response, JsonRequestBehavior.AllowGet);
                }                    
            }
            catch (Exception ex)
            {
                logger.Error("SendMessage error", ex);
            }
            return Json("error", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParametersTest(String id, String from, String to)
        {
            return Json("hei", JsonRequestBehavior.AllowGet);
        }
    }
}
