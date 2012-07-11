using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
          var userId = User.Identity.Name;
          return Json(mEFInterface.FindMatchingTagsForUser(term, userId), JsonRequestBehavior.AllowGet);
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
             System.Threading.Thread.Sleep(2000);
             return Json(SMSRepository.GetTagsForConversation(conversationID), JsonRequestBehavior.AllowGet);
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
          var tag = mEFInterface.AddTagToDB(tagName, defaultTagDescription, userId);
          mEFInterface.AddTagToConversation(tag, convID);
          return Json("Added successfully", JsonRequestBehavior.AllowGet);
       }

       public JsonResult RemoveTagFromConversation(string tagName, string convID)
       {
          mEFInterface.RemoveTagFromConversation(tagName, convID);
          return Json("Removed successfully", JsonRequestBehavior.AllowGet);
       }
    }
}
