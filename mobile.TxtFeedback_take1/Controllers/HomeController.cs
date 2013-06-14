using mobile.TxtFeedback_take1.Models;
using SmsFeedback_EFModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Utilities;

namespace mobile.TxtFeedback_take1.Controllers
{
   public class HomeController : Controller
   {
      private const String cStoreKey = "store";
      private String mStore = "";      
      protected override void OnActionExecuting(ActionExecutingContext filterContext)
      {
         if (RouteData.Values[cStoreKey] != null &&
           !string.IsNullOrWhiteSpace(RouteData.Values[cStoreKey].ToString()))
         {
            mStore = Utilities.ConversationUtilities.CleanUpPhoneNumber(RouteData.Values[cStoreKey].ToString()).ToLowerInvariant();
         }         
      }

      public ActionResult Index()
      {         
         //based on store ID we should retrieve the "long description"
         smsfeedbackEntities ent = new smsfeedbackEntities();                  
         var wps = from w in ent.WorkingPoints where w.ShortID == mStore select w;
         if (wps.Count() == 1)
         {
            var wp = wps.First();
            String language = wp.Language;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(language);
            var desc = wp.Name;
            ViewBag.Store = desc;
            ViewBag.Message = wp.WelcomeMessage;
            ViewBag.BusyMessage = wp.BusyMessage;
            ViewBag.BusyTimerValue = wp.BusyMessageTimer;
            //the format of the db language should be en-US or ro-RO (general-LOCAL)
            ViewBag.Language = language.Split('-')[0];
            ViewBag.ComponentLocation = mStore;
            return View();
         }
         //else we should return an error page
         return RedirectToAction("StoreNotFound");
      }

      public ActionResult StoreNotFound()
      {
         ViewBag.Store = mStore;
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
         string convID= ConversationUtilities.BuildConversationIDFromFromAndTo(clientTempPhoneNumber,location.ToLowerInvariant());         
         string newPassword = "123456";
         return Json(new UserDetails() {
            Name = clientTempPhoneNumber, 
            Password = newPassword,
            ConversationID = convID}, JsonRequestBehavior.AllowGet);
      }      
   }
}
