using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivacomLib
{
   class SendResponse
   {
      public ResponseCode ErrorCode { get; set; }
      public SendResponse(List<string> response)
      {
         ErrorCode = (ResponseCode)Convert.ToInt32(response[0]);
      }
   }
}
