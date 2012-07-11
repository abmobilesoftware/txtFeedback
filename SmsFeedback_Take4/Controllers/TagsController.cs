using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Models;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
   public class TagsController : Controller
   {
      #region "member variables and properties"
      private EFInteraction mEFInterface = new EFInteraction();
      private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      private AggregateSmsRepository mSmsRepository;
      private AggregateSmsRepository SMSRepository
      {
         get
         {
            mSmsRepository = AggregateSmsRepository.GetInstance(User.Identity.Name);
            return mSmsRepository;
         }
      }
      #endregion

      public JsonResult FindMatchingTags(string term)
      {
         smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
         var userId = User.Identity.Name;
         try
         {
            return Json(mEFInterface.FindMatchingTagsForUser(term, userId, lContextPerRequest), JsonRequestBehavior.AllowGet);
         }
         catch (Exception ex)
         {
            logger.Error("FindMatchingTags error", ex);
            return null;
         }
      }

      public JsonResult GetTagsForConversation(string conversationID)
      {
         if (conversationID == null)
         {
            logger.Error("conversationID was null");
            return null;
         }
         try
         {
            smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
            return Json(SMSRepository.GetTagsForConversation(conversationID, lContextPerRequest), JsonRequestBehavior.AllowGet);
         }
         catch (Exception ex)
         {
            logger.Error("Error occurred in Tags", ex);
            return null;
         }
      }

      public JsonResult AddTagToConversations(string tagName, string convID)
      {
         //check if the tag is already defined - if not, add it to our tag db. Then add the tag to this conversation
         var defaultTagDescription = "default description";
         var userId = User.Identity.Name;
         smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
         try
         {

            var tag = mEFInterface.AddTagToDB(tagName, defaultTagDescription, userId, lContextPerRequest);
            mEFInterface.AddTagToConversation(tag, convID, lContextPerRequest);
            return Json("Added successfully", JsonRequestBehavior.AllowGet);

         }
         catch (Exception ex)
         {
            logger.Error("AddTagToConversations error", ex);
            return null;
         }
      }

      public JsonResult RemoveTagFromConversation(string tagName, string convID)
      {
         smsfeedbackEntities lContextPerRequest = new smsfeedbackEntities();
         try
         {

            mEFInterface.RemoveTagFromConversation(tagName, convID, lContextPerRequest);
            return Json("Removed successfully", JsonRequestBehavior.AllowGet);

         }
         catch (Exception ex)
         {
            logger.Error("RemoveTagFromConversation error", ex);
            return null;
         }

      }
   }
}
