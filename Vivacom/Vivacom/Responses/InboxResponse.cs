using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivacomLib
{
   class InboxResponse
   {
      private List<ShortMessage> _Messages;
      public ResponseCode ErrorCode { get; set; }
      public List<ShortMessage> Messages
      {
         get
         {
            if (_Messages == null)
            {
                _Messages = new List<ShortMessage>();
                return _Messages;
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
      public string Md5sum { get; set; }
      private string DateFormat = "yyyy-MM-dd HH:mm:ff";

      public InboxResponse(List<string> response)
      {
         ErrorCode = (ResponseCode)Convert.ToInt32(response[0]);
         if ((ResponseCode)Convert.ToInt32(response[0]) == ResponseCode.OK)
         {
            Md5sum = response[response.Count - 1];
            for (int i = 1; i < response.Count - 1; i = i + 6)
            {
               Messages.Add(new ShortMessage(
                  Convert.ToInt64(response[i]),
                  response[i + 1],
                  response[i + 2],
                  Convert.ToInt32(response[i + 3]),
                  response[i + 4],
                  DateTime.ParseExact(response[i + 5],
                     DateFormat, CultureInfo.InvariantCulture)));

            }
         }
         else
         {
            Md5sum = response[1];
         }
      }
   }
}
