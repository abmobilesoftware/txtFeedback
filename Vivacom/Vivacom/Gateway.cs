using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Vivacom
{
   public class Gateway
   {
      private string resourcesBaseUri;
      private string inboxResourceUri;
      private string sendSMResourceUri;
      private string deleteSMResourceUri;
      private string username;
      private string password;
      private RestClient restClient;
      private Encoding encodingUTF8;

      public Gateway()
      {
         resourcesBaseUri = "http://82.137.75.6:3699";
         inboxResourceUri = resourcesBaseUri + "/inbox/";
         sendSMResourceUri = resourcesBaseUri + "/send/";
         deleteSMResourceUri = resourcesBaseUri + "/delete/";
         username = "txtfeedback";
         password = "txtf33dback";
         restClient = new RestClient();
         encodingUTF8 = UTF8Encoding.UTF8;
      }

      public ResponseCode SendSM(string from,
         string to,
         string msg,
         bool withDeliveryReport,
         bool isMessageInHex = false,
         int dcs = 0,
         int esm = 0,
         int pid = 0,
         int validity = 1440)
      {
         List<KeyValuePair<string, string>> parameters =
            new List<KeyValuePair<string, string>>();
         string md5sumString;
         parameters.Add(new KeyValuePair<string, string>("uid", username));
         parameters.Add(new KeyValuePair<string, string>("pass", password));
         parameters.Add(new KeyValuePair<string, string>("from", from));
         parameters.Add(new KeyValuePair<string, string>("to", to));
         md5sumString = username + password + from + to;
         if (isMessageInHex)
         {
            parameters.Add(new KeyValuePair<string, string>("msghex", msg));
            parameters.Add(new KeyValuePair<string, string>("dcs", dcs.ToString()));
            parameters.Add(new KeyValuePair<string, string>("esm", esm.ToString()));
            parameters.Add(new KeyValuePair<string, string>("pid", pid.ToString()));
            md5sumString += msg + dcs.ToString() + esm.ToString() + pid.ToString();
         }
         else
         {
            parameters.Add(new KeyValuePair<string, string>("msg", msg));
            md5sumString += msg;
         }
         string deliveryReportString = withDeliveryReport ? "1" : "0";
         parameters.Add(new KeyValuePair<string, string>("dlr", deliveryReportString));
         string md5sumValue = Utilities.CalculateMD5Hash(md5sumString);
         parameters.Add(new KeyValuePair<string,string>("md5sum", md5sumValue));
         
         HttpWebResponse response = null;
         try
         {
            response = restClient.GETResource(sendSMResourceUri, parameters);
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
               string responseBody = ConvertStreamToString(response.GetResponseStream());
               SendResponse sendResponse = new SendResponse(responseBody.Split(
                  Environment.NewLine.ToCharArray()).Where(x => !x.Equals("")).ToList());
               return sendResponse.ErrorCode;
            }
            else
            {
               return ResponseCode.HTTP_REQUEST_ERROR;
            }
         }
         finally
         {
            response.Close();
         }

      }

      public List<ShortMessage> CheckInbox(string shortNumber)
      {
         // TODO: Move this in the project
         List<KeyValuePair<string, string>> parameters =
            new List<KeyValuePair<string, string>>();
         parameters.Add(new KeyValuePair<string, string>("uid", username));
         parameters.Add(new KeyValuePair<string, string>("pass", password));
         parameters.Add(new KeyValuePair<string, string>("to", shortNumber));
         HttpWebResponse response = restClient.GETResource(inboxResourceUri, parameters);
         string responseBody = ConvertStreamToString(response.GetResponseStream());
         InboxResponse inboxResponse = new InboxResponse(responseBody.Split(
                   Environment.NewLine.ToCharArray()).Where(x => !x.Equals("")).ToList());
         if (inboxResponse.ErrorCode.Equals(ResponseCode.INCORRECT_FROM_FIELD))
         {
            Console.WriteLine("Number not added in account");
         }
         if (inboxResponse.Messages.Count == 0)
         {
            Console.WriteLine("No messages");
         }
         if (ResponseCode.OTHER_ERROR == DeleteSM(inboxResponse.Messages))
         {
            Console.WriteLine("Delete SM: Other error");
         };
         return inboxResponse.Messages;
      }

      private ResponseCode DeleteSM(List<ShortMessage> messages)
      {
         string usmidsList = String.Join(":", messages.Select(x => x.Usmid.ToString()));
         List<KeyValuePair<string, string>> parameters =
            new List<KeyValuePair<string, string>>();
         parameters.Add(new KeyValuePair<string, string>("uid", username));
         parameters.Add(new KeyValuePair<string, string>("pass", password));
         parameters.Add(new KeyValuePair<string, string>("usmids", usmidsList));
         string md5sum = Utilities.CalculateMD5Hash(username + password + usmidsList);
         parameters.Add(new KeyValuePair<string, string>("md5sum", md5sum));
         HttpWebResponse response = restClient.GETResource(deleteSMResourceUri, parameters);
         string responseBody = ConvertStreamToString(response.GetResponseStream());
         DeleteResponse deleteResponse = new DeleteResponse(responseBody.Split(
            Environment.NewLine.ToCharArray()).Where(x => !x.Equals("")).ToList());
         return deleteResponse.ErrorCode;
      }

      private string ConvertStreamToString(Stream stream)
      {
         StreamReader responseStreamReader = new StreamReader(
               stream, encodingUTF8);
         string responseString = responseStreamReader.ReadToEnd();
         responseStreamReader.Close();
         return responseString;
      }
   }
}
