﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.XMPP;
using System.Net;
using VivacomLib;
using System.Xml;

namespace TxtPushService
{
   class Logic
   {
      private List<WorkingPoint> workingPoints =
         new List<WorkingPoint>();
      private Gateway vivacomGateway;
      private XmppClientWrapper xmppClient;
      private int index;
      
      public Logic()
      {
         vivacomGateway = new Gateway();
         xmppClient = new XmppClientWrapper();
         index = 0;
         initializeWorkingPointsPool();         
         xmppClient.connect();         
      }      

      private void initializeWorkingPointsPool()
      {
         workingPoints.Add(new WorkingPoint("15300", "store1min"));
         workingPoints.Add(new WorkingPoint("15301", "store1plus"));
         workingPoints.Add(new WorkingPoint("15302", "store2min"));
         workingPoints.Add(new WorkingPoint("15303", "store2plus"));
         workingPoints.Add(new WorkingPoint("15304", "store3min"));
         workingPoints.Add(new WorkingPoint("15305", "store3plus"));
         workingPoints.Add(new WorkingPoint("15306", "store4min"));
         workingPoints.Add(new WorkingPoint("15307", "store4plus"));
         workingPoints.Add(new WorkingPoint("15308", "store5min"));
         workingPoints.Add(new WorkingPoint("15309", "store5plus"));
      }

      public void checkInbox(object state, bool timeOut)
      {
         if (xmppClient.IsConnected)
         {
            WorkingPoint workingPoint = workingPoints[index % 10];
            List<ShortMessage> messages = vivacomGateway.CheckInbox(workingPoint.PhoneNumber);
            string componentAddress = workingPoint.ShortID + "@moderator.txtfeedback.net";
            foreach (var message in messages)
            {
               xmppClient.sendXmppMessage(
                  Utilities.buildXmlString(message.From,
                  message.To,
                  message.Msg,
                  true,
                  true,
                  message.DateReceived),
                  "mihai.batista@txtfeedback.net");
            }
            ++index;
         }        
      }
      
      public void dispose()
      {
         xmppClient.disconnect();
      }
   }
}
