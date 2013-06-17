using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
   public class Device
   {
      public string id;
      public Device(string deviceId)
      {
         id = deviceId;
      }
   }
}