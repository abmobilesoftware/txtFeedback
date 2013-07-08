using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmsFeedback_EFModels;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Controllers
{
   [CustomAuthorizeAtribute]
    public class DevicesController : BaseController
    {
      private smsfeedbackEntities context = new smsfeedbackEntities();

      [HttpPost]
      public JsonResult AddDevice(string deviceId)
      {
         try
         {
            var user = (from u in context.Users
                        where u.UserName.ToLower().Equals(HttpContext.User.Identity.Name.ToLower())
                        select u).First();
            var userDevice = from d in context.Devices where d.Id.Equals(deviceId) select d;
            var deviceIsNotInDb = (userDevice.Count() == 0) ? true : false;
            Device device;
            if (deviceIsNotInDb)
            {
               device = new Device()
               {
                  Id = deviceId
               };
               context.Devices.Add(device);
               context.SaveChanges();
            }
            else
            {
               device = userDevice.First();
            }
            user.Devices.Add(device);
            context.SaveChanges();
            return Json("addDevice success", JsonRequestBehavior.AllowGet);
         }
         catch (Exception e)
         {
            return Json("addDevice fail " + e.Message, JsonRequestBehavior.AllowGet);
         }
      }

      [HttpPost]
      public JsonResult RemoveDevice(string deviceId)
      {
         try
         {
            var user = (from u in context.Users
                        where u.UserName.ToLower().Equals(HttpContext.User.Identity.Name.ToLower())
                        select u).First();
            var userDevice = from d in context.Devices where d.Id.Equals(deviceId) select d;
            if (userDevice.Count() > 0)
            {
               user.Devices.Remove(userDevice.First());
               context.SaveChanges();
            }
            return Json("removeDevice success", JsonRequestBehavior.AllowGet);
         }
         catch (Exception e)
         {
            return Json("removeDevice fail " + e.Message, JsonRequestBehavior.AllowGet);
         }
      }
    }
}
