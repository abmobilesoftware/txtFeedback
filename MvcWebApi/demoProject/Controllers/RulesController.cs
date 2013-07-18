using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Models;
using System.Web.Mvc;
using System.Net.Http;
using SmsFeedback_EFModels;
using System.Linq;
using System.Collections.Generic;
using Helpers;

namespace Controllers
{
   public class RulesController : ApiController
   {
      smsfeedbackEntities dbContext = new smsfeedbackEntities();

      public MsgHandlers Get()
      {
         String shortID = Utilities.extractVirtualDirectoryName(Request.RequestUri.LocalPath);
         List<Agent> agents = new List<Agent>();
         Dictionary<string, IEnumerable<SmsFeedback_EFModels.Device>> handlersDictionary;
         var handlers = (from wp in dbContext.WorkingPoints
                         where wp.ShortID.Equals(shortID)
                         select
                            (from user in wp.Users
                             select
                                new
                                {
                                   devices = user.Devices,
                                   xmppDetails = user.XmppConnection
                                }
                            ));
         var handlersGroupped = from h in handlers.SelectMany(x => x)
                                group h by h.xmppDetails.XmppUser into handlerGroup
                                select new
                                {
                                   key = handlerGroup.Key,
                                   devices = handlerGroup
                                };

         foreach (var p in handlersGroupped)
         {
            var xmppId = p.key;
            List<Models.Device> devices = new List<Models.Device>();

            IEnumerable<SmsFeedback_EFModels.Device> userDevices = new List<SmsFeedback_EFModels.Device>();
            foreach (var item in p.devices)
            {
               userDevices = userDevices.Union(item.devices);               
            }
            foreach (var dev in userDevices)
            {
               Models.Device d = new Models.Device(dev.Id);
               devices.Add(d);
            }
            Agent agent = new Agent(xmppId, devices, 7);
            agents.Add(agent);
         }
         return new MsgHandlers(agents);


         /*foreach (var handler in handlers)
         {
            var xmppId = handler.xmppDetails.XmppUser;
            List<Models.Device> devices = new List<Models.Device>();
            foreach (var device in handler.devices)
            {
               Models.Device d = new Models.Device(device.Id);
               devices.Add(d);
            }
            Agent agent = new Agent(xmppId, devices, 7);
            agents.Add(agent);
         }*/
        
      }
   }
}