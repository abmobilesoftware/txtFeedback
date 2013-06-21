using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivacom
{
   class VivacomShortMessage
   {
      public int usmid { get; set; }
      public string to { get; set; }
      public string from { get; set; }
      public int dcs { get; set; }
      public string msghex {get; set;}
      public string msg { get; set; }
      public DateTime receiveDate { get; set; }

      public VivacomShortMessage(int iUsmid,
         string iTo,
         string iFrom,
         int iDcs,
         string iMsghex,
         DateTime iReceiveDate)
      {
         usmid = iUsmid;
         to = iTo;
         from = iFrom;
         dcs = iDcs;
         msghex = iMsghex;
         msg = VivacomUtilities.ConvertHexToString(iMsghex);
         receiveDate = iReceiveDate;
      }

   }
}
