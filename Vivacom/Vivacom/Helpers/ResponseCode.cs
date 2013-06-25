using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivacomLib
{
   public enum ResponseCode
   {
      INVALID_RESPONSE_CODE = 0,
      AUTHENTICATION_FAILED = 301,
      /* If client try to send more message per second then its limit  */
      OVERLOADED_MESSAGES_THROUGHPUT_PER_SECOND = 310,
      /* If “to” field is missing */
      MISSING_RECIPIENT_PARAMETER = 320,
      TEXT_MESSAGE_INVALID = 327,
      HEX_MESSAGE_INVALID = 328,
      INCORRECT_DLR_PARAMETER_VALUE = 341,
      UNKNOW_MESSAGE_ID = 342,
      /*	If validity is wrong value */
      INCORRECT_VALIDITY_PARAMETER = 351,
      INCORRECT_FROM_FIELD = 360,
      /* If list of usmid are bigger than 20 records  */
      PROBLEM_WITH_SM_ID = 362,
      /*	If PID has wrong value */
      PROTOCOL_ID_PROBLEM = 364,
      /* If ESM has wrong value */
      ESM_CLASS_PROBLEM = 365,
      /* If DCS has wrong value TODO: Clarify the typo in the document */
      DATA_CODING_SCHEME_PROBLEM = 366,
      OTHER_ERROR = 399,
      OK = 777,
      HTTP_REQUEST_ERROR = 900,
   }
}
