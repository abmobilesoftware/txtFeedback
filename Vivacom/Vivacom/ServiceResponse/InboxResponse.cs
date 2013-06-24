using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vivacom
{
   class InboxResponse
   {
      public List<ShortMessage> _Messages;
      public ResponseCode ErrorCode { get; set; }
      public List<ShortMessage> Messages
      {
         get
         {
            if (_Messages == null)
            {
               return new List<ShortMessage>();
            }
            else
            {
               return _Messages;
            }
         }
         set
         {
            _Messages = value;
         }
      }
      public string md5sum { get; set; }
      private string dateFormat = "yyyy-MM-dd HH:mm:ff";

      public InboxResponse(List<string> response)
      {
         if ((ResponseCode)Convert.ToInt32(response[0]) == ResponseCode.OK)
         {
            for (int i = 1; i < response.Count - 1; i = i + 6)
            {
               Messages.Add(new ShortMessage(
                  Convert.ToInt32(response[i]),
                  response[i + 1],
                  response[i + 2],
                  Convert.ToInt32(response[i + 3]),
                  response[i + 4],
                  DateTime.ParseExact(response[i + 5],
                     dateFormat, CultureInfo.InvariantCulture)));

            }
         }
         else
         {
            ErrorCode = (ResponseCode)Convert.ToInt32(response[0]);
            md5sum = response[1];
         }
      }
   }
}
