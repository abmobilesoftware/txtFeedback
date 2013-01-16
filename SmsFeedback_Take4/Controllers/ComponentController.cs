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
using System.Text;

namespace SmsFeedback_Take4.Controllers
{
    public class ComponentController : Controller
    {
        private EFInteraction mEFInterface = new EFInteraction();
        smsfeedbackEntities context= new smsfeedbackEntities();
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private AggregateSmsRepository mSmsRepository;
        private AggregateSmsRepository SMSRepository
        {
           get
           {
              if (mSmsRepository == null)
                 /*if (User.Identity.Name.Length > 0) 
                     mSmsRepository = AggregateSmsRepository.GetInstance(User.Identity.Name);
                 else*/
                 mSmsRepository = AggregateSmsRepository.GetInstance("ando"); // for testing
              return mSmsRepository;
           }
        }

        /*
         * Just for testing 
         */ 
        public JsonResult GetHandlerForMessage1(String wp, String convId, bool isSms)
        {
            if (isSms)
            {
                if (wp.Equals("shop1@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m13@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop2@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m14@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop3@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m15@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Agent agent1 = new Agent("m16@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                // TODO: Xmpp address to WP number
                if (wp.Equals("shop1@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m13@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop2@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m14@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop3@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m15@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop4@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m16@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                } else if (wp.Equals("shop5@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m17@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                } else if (wp.Equals("shop6@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m18@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop7@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m19txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop8@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m20@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop9@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m21@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop10@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m22@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop11@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m23@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop12@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m24@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop13@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m25@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop14@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m26@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop15@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m27@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop16@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m28@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop17@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m29@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop18@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m30@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop19@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m31@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else if (wp.Equals("shop20@compdev.txtfeedback.net"))
                {
                    Agent agent1 = new Agent("m32@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Agent agent1 = new Agent("m32@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult GetWorkingPointForCertainAddress(String iIMAddress)
        {
            SmsFeedback_Take4.Models.WorkingPoint wp = new SmsFeedback_Take4.Models.WorkingPoint("07897951452", "hei", "nexmo","defaultShortID", "@moderator.txtfeedback.net", "Hello");
            wp.NrOfSentSmsThisMonth = 10;
            wp.MaxNrOfSmsToSendPerMonth = 100;
            return Json(wp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveMessage(String from, String to, String convId, String text, String xmppUser, bool isSms)
        {
            logger.InfoFormat("SaveMessage - from: [{0}], to: [{1}], convId: [{2}] text: [{3}], xmppUser: [{4}], isSms: [{5}]", from, to, convId, text, xmppUser, isSms.ToString());
            try
            {
                // get the previous from message to compute the response time

               var previousConv = mEFInterface.GetLatestConversationDetails(convId, context);
                String prevConvFrom = Constants.NO_LAST_FROM;
                DateTime prevConvUpdateTime = DateTime.MaxValue;
                if (previousConv != null)
                {
                    prevConvFrom = previousConv.From;
                    prevConvUpdateTime = previousConv.TimeUpdated;
                }
                string direction = ComputeDirection(from, convId, isSms);
                bool validDirection = !direction.Equals(Constants.DIRECTION_INVALID);
                if (validDirection)
                {
                    text = HttpUtility.UrlDecode(text);
                    convId = convId.ToLower();
                    if (isSms)
                    {
                        if (direction.Equals(Constants.DIRECTION_OUT))
                        {
                           return SendSmsMessageAndUpdateDb(from, to, convId,
                                text, xmppUser, context,
                                prevConvFrom, prevConvUpdateTime);
                        }
                        else
                        {
                           return SaveIncommingMessage(from, to, convId, text, context);
                        }
                    }
                    else
                    {
                        return SaveImMessageInDb(from, to, convId,
                            text, xmppUser, context,
                            prevConvFrom, prevConvUpdateTime, direction);
                    }
                }
                else
                {
                    return Json(JsonReturnMessages.INVALID_DIRECTION, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("SendMessage error", ex);
                Console.WriteLine("SaveMessage Error = " + ex.Message);
                Console.WriteLine("SaveMessage Error stack = " + ex.StackTrace + "||| & source " + ex.Source);
            }
            return Json("error", JsonRequestBehavior.AllowGet);            
        }
        
       /* returns the id of the new inserted message */
        private JsonResult SaveImMessageInDb(
           String from, String to, String convId, String text, String xmppUser, smsfeedbackEntities lContextPerRequest,
           String prevConvFrom, DateTime prevConvUpdateTime, string direction)
        {
            // save xmppUser in db just when the message direction is from staff to client
            string xmppUserToBeSaved = xmppUser;
            bool readStatus = true;
            if (direction.Equals(Constants.DIRECTION_IN))
            {
                xmppUserToBeSaved = Constants.DONT_ADD_XMPP_USER;
                readStatus = false;
            }
            //maybe delegate the result to the UpdateDB function
            //or interpret the result and return an appropriate message
            int result = mEFInterface.UpdateDb(from, to, convId, text, readStatus, DateTime.UtcNow, prevConvFrom, prevConvUpdateTime, false, xmppUserToBeSaved, null, null, direction, lContextPerRequest);
            return Json(result.ToString(), JsonRequestBehavior.AllowGet);
        }

        private JsonResult SendSmsMessageAndUpdateDb(
           String from, String to, String convId, String text, String xmppUser, smsfeedbackEntities lContextPerRequest, 
           String prevConvFrom, DateTime prevConvUpdateTime)
        {
            SMSRepository.SendMessage(from, to, text, lContextPerRequest, (msgResponse) =>
            {
                mEFInterface.UpdateDb(from, to, convId, text, true, msgResponse.DateSent, prevConvFrom, prevConvUpdateTime, true, xmppUser, msgResponse.Price, msgResponse.ExternalID, Constants.DIRECTION_OUT, lContextPerRequest);
            });
            return Json(JsonReturnMessages.OP_SUCCESSFUL, JsonRequestBehavior.AllowGet);
        }

        private JsonResult SaveIncommingMessage(
           String from, String to, String convId, String text, smsfeedbackEntities lContextPerRequest)
        {
           int result = mEFInterface.UpdateDb(from, to, convId, text, false, DateTime.UtcNow, null, DateTime.UtcNow, true, Constants.DONT_ADD_XMPP_USER, null, null, Constants.DIRECTION_IN, lContextPerRequest);
           return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static string ComputeDirection(String from, String convId, bool isSms)
        {
            /*
             * Example: 
             *      IN:  convId = xy123-shop10 (client-WP) 
             *           from = xy123
             *           isSms = false
             *      OUT: Direction_IN 
             *      A message has the "IN" direction if it comes from the client and "OUT" otherwise.
             *      ConvId it's fixed between messages of the same conversation and has the format "client-WP". [part1]-[part2] 
             *      From indicates the sender of the message. 
             *      To compute the direction I test the from against the first part of the convId.
             *          
             */
            var direction = Constants.DIRECTION_OUT;
            string[] fromTo = ConversationUtilities.GetFromAndToFromConversationID(convId);
            if (isSms)
            {
                if (fromTo[0].Equals(from)) direction = Constants.DIRECTION_IN;
                else if (fromTo[1].Equals(from)) direction = Constants.DIRECTION_OUT;
                else direction = Constants.DIRECTION_INVALID;
            }
            else
            {
                var userId = ConversationUtilities.ExtractUserFromAddress(from);
                if (fromTo[0].Equals(userId)) direction = Constants.DIRECTION_IN;
                else if (fromTo[1].Equals(userId)) direction = Constants.DIRECTION_OUT;
                else direction = Constants.DIRECTION_INVALID;            
            }
            return direction;
        }

        public JsonResult UpdateMessageClientAcknowledgeField(int msgID, bool clientAcknowledge)
        {
           try
           {
              smsfeedbackEntities dbContext = new smsfeedbackEntities();
              mEFInterface.updateMsgClientAckField(msgID, clientAcknowledge, dbContext);
              return Json(JsonReturnMessages.OP_SUCCESSFUL, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
              return Json(JsonReturnMessages.OP_FAILED, JsonRequestBehavior.AllowGet);
           }
        }
      
       protected override void Dispose(bool disposing)
        {
           context.Dispose();
           base.Dispose(disposing);
        }
      
    }

}
