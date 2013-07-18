using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxtPushService
{
   class Utilities
   {
      private static string RFC2822format = "ddd, dd MMM yyyy hh:mm:ff zzz";
      public static string buildXmlString(
         string from,
         string to,
         string body,
         bool staff,
         bool sms,
         DateTime dateSent)
      {
         StringBuilder sb = new StringBuilder("<msg>");
         sb.AppendFormat("<from>{0}</from>", from);
         sb.AppendFormat("<to>{0}</to>", to);
         sb.AppendFormat("<convID>{0}-{1}</convID>", from, to);
         sb.AppendFormat("<body>{0}</body>", body);
         sb.AppendFormat("<staff>{0}</staff>", staff.ToString());
         sb.AppendFormat("<sms>{0}</sms>", sms.ToString());
         sb.AppendFormat("<datesent>{0}</datesent>", dateSent.ToString(RFC2822format));
         sb.Append("</msg>");
         return sb.ToString();
      }
   }
}
