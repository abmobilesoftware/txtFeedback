using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivacom
{
   public class ShortMessage
   {
      public int Usmid { get; set; }
      public string To { get; set; }
      public string From { get; set; }
      public int Dcs { get; set; }
      public string Msghex {get; set;}
      public string Msg { get; set; }
      public DateTime ReceiveDate { get; set; }

      public ShortMessage(
         int iUsmid,
         string iTo,
         string iFrom,
         int iDcs,
         string iMsghex,
         DateTime iReceiveDate)
      {
         Usmid = iUsmid;
         To = iTo;
         From = iFrom;
         Dcs = iDcs;
         Msghex = iMsghex;
         Msg = Utilities.ConvertHexToString(iMsghex);
         ReceiveDate = iReceiveDate;
      }

   }
}
