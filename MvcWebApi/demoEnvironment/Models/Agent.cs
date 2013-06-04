using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
   public class Agent
   {
      public String xmppId;
      public List<Device> devices;
      public int priority;

      public Agent(String iXmppId, List<Device> iDevices, int iPriority)
      {
         xmppId = iXmppId;
         devices = iDevices;
         priority = iPriority;
      }
   }
}