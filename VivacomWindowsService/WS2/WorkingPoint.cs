using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2
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
