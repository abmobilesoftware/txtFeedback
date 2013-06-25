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

namespace Controllers
{
   public class RulesController : ApiController
   {
      private const string SHORT_ID = "shop20";
      smsfeedbackEntities dbContext = new smsfeedbackEntities();

      // GET api/rules/abmob1
      public MsgHandlers Get()
      {
         /* 
          * Handler == User. A user is connected on 0-* mobile devices 
          * and has 1 xmppId.
          */
         List<Agent> agents = new List<Agent>();
         var handlers = (from wp in dbContext.WorkingPoints
                         where wp.ShortID.Equals(SHORT_ID)
                         select
                            (from user in wp.Users
                             select
                                new
                                {
                                   devices = user.Devices,
                                   xmppDetails = user.XmppConnection
                                }
                            )).SelectMany(x => x);
         foreach (var handler in handlers)
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
         }
         return new MsgHandlers(agents);
      }
   }
}