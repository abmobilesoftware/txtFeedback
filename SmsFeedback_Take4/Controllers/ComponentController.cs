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
using SmsFeedback_Take4.Mailers;
using Mvc.Mailer;
using System.Threading;
using System.Globalization;

namespace SmsFeedback_Take4.Controllers
{
   public class ComponentController : Controller
   {
      private EFInteraction mEFInterface = new EFInteraction();
      smsfeedbackEntities context = new smsfeedbackEntities();
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
                     SendMailNotification(convId, text, isSms);
                     return SaveIncommingMessage(from, to, convId, text, context);
                  }
               }
               else
               {
                  if (direction.Equals(Constants.DIRECTION_IN))
                  {
                     SendMailNotification(convId, text, isSms);
                  }
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
         SubscriptionSmsStatus messageStatus = mEFInterface.MarkMessageActivityInDB(from,
            to,
            convId,
            text,
            readStatus,
            DateTime.UtcNow,
            prevConvFrom,
            prevConvUpdateTime,
            false,
            xmppUserToBeSaved,
            null,
            null,
            direction,
            lContextPerRequest);
         return Json(messageStatus, JsonRequestBehavior.AllowGet);
      }

      private JsonResult SendSmsMessageAndUpdateDb(
         String from, String to, String convId, String text, String xmppUser, smsfeedbackEntities lContextPerRequest,
         String prevConvFrom, DateTime prevConvUpdateTime)
      {
         SubscriptionSmsStatus messageStatus = new SubscriptionSmsStatus(-1, false, false, false);
         //var messageCanBeSent = SMSRepository.SendMessage(from, to, text, lContextPerRequest, (msgResponse) =>
         var status = SMSRepository.SendMessage(from, to, text, lContextPerRequest);
         //status will tell you if message was sent or not

         //the rest should come later....
         if (status != null) // null stands for  - could not sent SMS - no credit
         {
            //the idea is that a message can be sent but somewhere on Nexmo/Twilio there was a problem....
            messageStatus = mEFInterface.MarkMessageActivityInDB(from,
                   to,
                   convId,
                   text,
                   true,
                   status.DateSent,
                   prevConvFrom,
                   prevConvUpdateTime,
                   true,
                   xmppUser,
                   status.Price,
                   status.ExternalID,
                   Constants.DIRECTION_OUT, lContextPerRequest);
            messageStatus.MessageSent = status.MessageSent != null ? true : false;
         }


         //messageStatus = mEFInterface.MarkMessageActivityInDB(from,
         //           to,
         //           convId,
         //           text,
         //           true,
         //           new DateTime(2013,1,1,10,10,10,100),
         //           prevConvFrom,
         //           prevConvUpdateTime,
         //           true,
         //           xmppUser,
         //           "10",
         //           "115",
         //           Constants.DIRECTION_OUT, lContextPerRequest);
         return Json(messageStatus, JsonRequestBehavior.AllowGet);
      }

      private JsonResult SaveIncommingMessage(
         String from, String to, String convId, String text, smsfeedbackEntities lContextPerRequest)
      {
         SubscriptionSmsStatus messageStatus = mEFInterface.MarkMessageActivityInDB(from, to, convId, text, false, DateTime.UtcNow, null, DateTime.UtcNow, true, Constants.DONT_ADD_XMPP_USER, null, null, Constants.DIRECTION_IN, lContextPerRequest);
         return Json(messageStatus, JsonRequestBehavior.AllowGet);
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

      public void SendMailNotification(string convId, string message, bool isSms)
      {
         smsfeedbackEntities dbContext = new smsfeedbackEntities();
         NotificationMailer mailer = new NotificationMailer();
         /**
          * WorkingPoint identifier is either the short ID or 
          * the phone number
          */
         string ACTIVITY_REPORT_EVERY_INCOMING = "Every incoming";
         string wpIdentifier = ConversationUtilities.
            GetFromAndToFromConversationID(convId)[1];
         string customerIdentifier = ConversationUtilities.
            GetFromAndToFromConversationID(convId)[0];
         var linqResults = from wp in dbContext.WorkingPoints
                           where wp.ShortID.Equals(wpIdentifier)
                           || wp.TelNumber.Equals(wpIdentifier)
                           select wp;
         SmsFeedback_EFModels.WorkingPoint workingPoint = null;
         if (linqResults.Count() > 0)
         {
            workingPoint = linqResults.First();
         }
         if (workingPoint != null)
         {
            var users = workingPoint.Users;
            foreach (var user in users)
            {
               if (user.TypeOfActivityReport.Equals(
                  ACTIVITY_REPORT_EVERY_INCOMING))
               {
                  Thread.CurrentThread.CurrentUICulture = 
                     new CultureInfo(workingPoint.Language);
                  System.Net.Mail.MailMessage notificationMail =
                     mailer.BuildNotificationMail(
                     user.Membership.Email,
                     customerIdentifier,
                     Resources.Global.mailTemplateSubject,
                     message,
                     workingPoint.Name,
                     workingPoint.Language,
                     isSms,
                     DateTime.UtcNow);
                  notificationMail.Send();
               }
            }
         }

      }

      protected override void Dispose(bool disposing)
      {
         context.Dispose();
         base.Dispose(disposing);
      }

   }

}
