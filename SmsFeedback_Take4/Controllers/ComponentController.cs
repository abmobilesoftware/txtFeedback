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
        public JsonResult GetHandlerForMessage(String wp, String convId, bool isSms)
        {
            if (isSms)
            {
                // No conversion required for wp
                if (wp.Equals("abmob1@moderator.txtfeedback.net", StringComparison.InvariantCultureIgnoreCase))
                {
                    Agent agent1 = new Agent("support_dev_de@txtfeedback.net", 7);
                    Agent agent2 = new Agent("dragos@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1, agent2 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Agent agent1 = new Agent("support_dev_de@txtfeedback.net", 7);
                    Agent agent2 = new Agent("dragos@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1, agent2 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (wp.Equals("abmobando@moderator.txtfeedback.net", StringComparison.InvariantCultureIgnoreCase))
                {
                    Agent agent1 = new Agent("testDA@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1};
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Conversion required from xmpp address to wp number. It's stored in wp param
                    Agent agent1 = new Agent("support_dev_de@txtfeedback.net", 7);
                    Agent agent2 = new Agent("dragos@txtfeedback.net", 7);
                    List<Agent> agents = new List<Agent> { agent1, agent2 };
                    MsgHandlers listOfAgents = new MsgHandlers(agents);
                    return Json(listOfAgents, JsonRequestBehavior.AllowGet);
                }
            }
        }

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
            SmsFeedback_Take4.Models.WorkingPoint wp = new SmsFeedback_Take4.Models.WorkingPoint("07897951452", "hei", "nexmo","defaultShortID", "@moderator.txtfeedback.net");
            wp.NrOfSentSmsThisMonth = 10;
            wp.MaxNrOfSmsToSendPerMonth = 100;
            return Json(wp, JsonRequestBehavior.AllowGet);
        }

         private void UpdateDb(String from, String to, String conversationId, String text, Boolean readStatus,
                                                     DateTime updateTime, String prevConvFrom, DateTime prevConvUpdateTime, bool isSmsBased, String XmppUser, smsfeedbackEntities dbContext)
        {
            string convID = mEFInterface.UpdateAddConversation(from, to, conversationId, text, readStatus, updateTime, isSmsBased, dbContext);
            mEFInterface.AddMessage(from, to, conversationId, text, readStatus, updateTime, prevConvFrom, prevConvUpdateTime, isSmsBased, XmppUser, dbContext);
            mEFInterface.IncrementNumberOfSentSms(from, dbContext);
        }

        public JsonResult SaveMessage(String from, String to, String convId, String text, String xmppUser, bool isSms)
        {
            logger.InfoFormat("SendMessage - from: [{0}], to: [{1}], convId: [{2}] text: [{3}], xmppUser: [{4}], isSms: [{5}]", from, to, convId, text, xmppUser, isSms.ToString());
            try
            {
                /* 
                 * get the previous conversation from and time.
                 */
                smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
                var previousConv = mEFInterface.GetLatestConversationDetails(convId, lContextPerRequest);
                String prevConvFrom = Constants.NO_LAST_FROM;
                DateTime prevConvUpdateTime = DateTime.MaxValue;
                if (previousConv != null)
                {
                    prevConvFrom = previousConv.From;
                    prevConvUpdateTime = previousConv.TimeUpdated;
                }
                // Compute direction
                var direction = Constants.DIRECTION_OUT;
                string[] fromTo = ConversationUtilities.GetFromAndToFromConversationID(convId);
                if (isSms)
                {
                    if (fromTo[0].Equals(from)) direction = Constants.DIRECTION_IN;
                }
                else
                {
                    if (fromTo[0].Equals(ConversationUtilities.ExtractUserFromAddress(from))) direction = Constants.DIRECTION_IN;
                }

                // decode text from UTF-8 and make conversationId lower case
                var textUnescaped = Server.UrlDecode(text);
                var conversationId = convId.ToLower();
                if (isSms)
                {
                    if (direction.Equals(Constants.DIRECTION_OUT))
                    {
                        SMSRepository.SendMessage(from, to, textUnescaped, lContextPerRequest, (msgResponse) =>
                        {
                            //TODO add check if message was sent successfully 
                            UpdateDb(from, to, conversationId, textUnescaped, true, msgResponse.DateSent, prevConvFrom, prevConvUpdateTime, true, xmppUser, lContextPerRequest);
                        });
                        //we should wait for the call to finish
                        //I should return the sent time (if successful)              
                        String response = "success"; //TODO should be a class
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        String response = "success"; //TODO should be a class
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    // save xmppUser in db just when the message direction is from customer to client
                    string xmppUserToBeSaved = xmppUser;
                    bool readStatus = true;
                    if (direction.Equals(Constants.DIRECTION_IN))
                    {
                        xmppUserToBeSaved = Constants.DONT_ADD_XMPP_USER;
                        readStatus = false;
                    }
                    UpdateDb(from, to, conversationId, textUnescaped, readStatus, DateTime.UtcNow, prevConvFrom, prevConvUpdateTime, false, xmppUserToBeSaved, lContextPerRequest);
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
      
    }
}
