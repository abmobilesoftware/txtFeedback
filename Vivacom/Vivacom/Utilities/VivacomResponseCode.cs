using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivacomLib
{
   class VivacomResponseCode
   {
      /* 
       * TODO: Error codes are expressed with numbers. Find a way to link
       * error numbers and error messages
       */
      /* Send errors as specified in http2sms_interface_v1.0.2.2.doc */
      public static KeyValuePair<int, string> AUTHENTICATION_FAILED =
         new KeyValuePair<int, string>(301, "Authentication failed");
      /* If from not belongs to this user */
      public static KeyValuePair<int, string> INCORRECT_FROM_FIELD =
         new KeyValuePair<int, string>(360, "Incorrect from field");
      /*	If validity is wrong value */
      public static KeyValuePair<int, string> INCORRECT_VALIDITY_PARAMETER =
         new KeyValuePair<int, string>(351, "Incorrect validity parameter");
      /* If client try to send more message per second then its limit  */
      public static KeyValuePair<int, string> OVERLOADED_MESSAGES_THROUGHPUT_PER_SECOND =
         new KeyValuePair<int, string>(310, "Overloaded messages throughput per second");
      /* If DCS has wrong value TODO: Clarify the typo in the document */
      public static KeyValuePair<int, string> DATA_CODING_SCHEME_PROBLEM =
         new KeyValuePair<int, string>(366, "Data Coding Scheme problem");
      /*	If PID has wrong value */
      public static KeyValuePair<int, string> PROTOCOL_ID_PROBLEM =
         new KeyValuePair<int, string>(364, "Protocol ID problem");
      /* If ESM has wrong value */
      public static KeyValuePair<int, string> ESM_CLASS_PROBLEM =
         new KeyValuePair<int, string>(365, "ESM Class problem");
      public static KeyValuePair<int, string> TEXT_MESSAGE_INVALID =
         new KeyValuePair<int, string>(327, "Text message is longer than allowed size or empty");
      public static KeyValuePair<int, string> HEX_MESSAGE_INVALID =
         new KeyValuePair<int, string>(328, "Hex message is longer than allowed size or empty");
      public static KeyValuePair<int, string> INCORRECT_DLR_PARAMETER_VALUE =
         new KeyValuePair<int, string>(341, "Incorrect dlr parameter value");
      /* If “to” field is missing */
      public static KeyValuePair<int, string> MISSING_RECIPIENT_PARAMETER =
         new KeyValuePair<int, string>(320, "Missing recipient parameter");
      /* If even one usmid not belongs to user */
      /* Status multiple messages */
      public static KeyValuePair<int, string> UNKNOW_MESSAGE_ID =
         new KeyValuePair<int, string>(342, "Unknown Message ID");
      /* If list of usmid are bigger than 20 records  */
      public static KeyValuePair<int, string> PROBLEM_WITH_SM_ID =
         new KeyValuePair<int, string>(362, "Problem with sm_id");
      /* If any other not defined error occurred */
      public static KeyValuePair<int, string> OTHER_ERROR =
         new KeyValuePair<int, string>(399, "Other error");
      public static KeyValuePair<int, string> OK =
         new KeyValuePair<int, string>(777, "OK");
      public static KeyValuePair<int, string> HTTP_REQUEST_ERROR =
         new KeyValuePair<int, string>(900, "Http request error");
      private string INVALID_RESPONSE_CODE = "Invalid response code";
      private KeyValuePair<int, string> responseCode;

      public VivacomResponseCode(int iResponseCode)
      {
         LinkedList<KeyValuePair<int, string>> responseCodesList =
            new LinkedList<KeyValuePair<int, string>>();
         responseCodesList.AddLast(AUTHENTICATION_FAILED);
         responseCodesList.AddLast(INCORRECT_FROM_FIELD);
         responseCodesList.AddLast(INCORRECT_VALIDITY_PARAMETER);
         responseCodesList.AddLast(OVERLOADED_MESSAGES_THROUGHPUT_PER_SECOND);
         responseCodesList.AddLast(DATA_CODING_SCHEME_PROBLEM);
         responseCodesList.AddLast(PROTOCOL_ID_PROBLEM);
         responseCodesList.AddLast(ESM_CLASS_PROBLEM);
         responseCodesList.AddLast(HEX_MESSAGE_INVALID);
         responseCodesList.AddLast(INCORRECT_DLR_PARAMETER_VALUE);
         responseCodesList.AddLast(MISSING_RECIPIENT_PARAMETER);
         responseCodesList.AddLast(UNKNOW_MESSAGE_ID);
         responseCodesList.AddLast(PROBLEM_WITH_SM_ID);
         responseCodesList.AddLast(OTHER_ERROR);
         responseCodesList.AddLast(OK);
         foreach (var rc in responseCodesList)
         {
            if (rc.Key == iResponseCode)
            {
               responseCode = rc;
            }
         }
      }

      public override string ToString()
      {
         if (responseCode.Key != null) {
            return responseCode.Value;
         } else {
            return INVALID_RESPONSE_CODE;
         }         
      }

      public override bool Equals(object iRc)
      {
         KeyValuePair<int, string> rc = (KeyValuePair<int, string>)iRc;
         return responseCode.Key == rc.Key ? true : false;
      }
   }
}
