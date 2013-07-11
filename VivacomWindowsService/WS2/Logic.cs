using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.XMPP;
using System.Net;
using VivacomLib;
using System.Xml;
using System.Diagnostics;

namespace WS2
{
   class Logic
   {
      private List<WorkingPoint> workingPoints =
         new List<WorkingPoint>();
      private Gateway vivacomGateway;
      private XmppClientWrapper xmppClient;
      private int index;
      private EventLog eventLog;
      
      public Logic(EventLog pEventLog)
      {
         eventLog = pEventLog;
         vivacomGateway = new Gateway();
         xmppClient = new XmppClientWrapper();
         index = 0;
         initializeWorkingPointsPool();         
         xmppClient.connect();         
      }      

      private void initializeWorkingPointsPool()
      {
         workingPoints.Add(new WorkingPoint("123001", "store1min"));
         workingPoints.Add(new WorkingPoint("123002", "store1plus"));
         workingPoints.Add(new WorkingPoint("123003", "store2min"));
         workingPoints.Add(new WorkingPoint("123004", "store2plus"));
         workingPoints.Add(new WorkingPoint("123005", "store3min"));
         workingPoints.Add(new WorkingPoint("123006", "store3plus"));
         workingPoints.Add(new WorkingPoint("123007", "store4min"));
         workingPoints.Add(new WorkingPoint("123008", "store4plus"));
         workingPoints.Add(new WorkingPoint("123009", "store5min"));
         workingPoints.Add(new WorkingPoint("123010", "store5plus"));
      }

      public void checkInbox(object state, bool timeOut)
      {
         
         if (xmppClient.IsConnected)
         {
            try
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
                     componentAddress);
               }
               eventLog.WriteEntry("Checked inbox for " + workingPoint.PhoneNumber +
                  ". No of messages " + messages.Count);
               ++index;               
            }
            catch (Exception e)
            {
               eventLog.WriteEntry("Check inbox exception " + e.Message);
               ++index;
            }
         }        
      }
      
      public void dispose()
      {
         xmppClient.disconnect();
      }
   }
}
