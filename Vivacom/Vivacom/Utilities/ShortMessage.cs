using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivacomLib
{
   public class ShortMessage
   {
      /// <summary>
      /// Universal short message id
      /// </summary>
      public long Usmid { get; set; }
      public string To { get; set; }
      public string From { get; set; }
      /// <summary>
      /// 
      /// </summary>
      public int Dcs { get; set; }
      public string MsgAsHex {get; set;}
      public string Msg { get; set; }
      public DateTime DateReceived { get; set; }

      public ShortMessage(
         long iUsmid,
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
         MsgAsHex = iMsghex;
         Msg = Utilities.ConvertHexToString(iMsghex);
         DateReceived = iReceiveDate;
      }

   }
}
