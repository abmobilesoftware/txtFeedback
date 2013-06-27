using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxtPushService
{
   class WorkingPoint
   {
      public string PhoneNumber { get; set; }
      public string ShortID { get; set; }

      public WorkingPoint(string iPhoneNumber, string iShortID)
      {
         PhoneNumber = iPhoneNumber;
         ShortID = iShortID;
      }
   }
}
