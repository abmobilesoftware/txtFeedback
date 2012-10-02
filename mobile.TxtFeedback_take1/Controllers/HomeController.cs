using mobile.TxtFeedback_take1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace mobile.TxtFeedback_take1.Controllers
{
   public class HomeController : Controller
   {
      public ActionResult Index()
      {
         ViewBag.Message = "Welcome to Lidl Republicii. How can we be of service?";         
         return View();
      }

      public ActionResult About()
      {
         ViewBag.Message = "Your app description page.";

         return View();
      }

      public ActionResult Contact()
      {
         ViewBag.Message = "Your contact page.";

         return View();
      }      

      public JsonResult GetUser(string location)
      {
         /*based on the location we:
          * create a conversationID 
          * display the appropriate welcome message
          */
         var guid = Guid.NewGuid().ToString();
         var clientTempPhoneNumber = Regex.Replace(guid, "[-]+", "", RegexOptions.Compiled);
         string convID= ConversationUtilities.BuildConversationIDFromFromAndTo(clientTempPhoneNumber,location);         
         string newPassword = "123456";
         return Json(new UserDetails() {
            Name = guid, 
            Password = newPassword,
            ConversationID = convID}, JsonRequestBehavior.AllowGet);
      }      
   }
}
