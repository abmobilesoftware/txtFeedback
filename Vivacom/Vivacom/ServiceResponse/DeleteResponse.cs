using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivacomLib
{
   class DeleteResponse
   {
      public ResponseCode ErrorCode;
      public DeleteResponse(List<string> response) 
      {
         ErrorCode = (ResponseCode)Convert.ToInt32(response[0]);
      }
   }
}
