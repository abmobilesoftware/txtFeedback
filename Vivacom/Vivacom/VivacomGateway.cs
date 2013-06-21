using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Vivacom
{
   public class VivacomGateway
   {
      private string resourcesBaseUri;
      private string inboxResourceUri;
      private string sendSMResourceUri;
      private string username;
      private string password;
      private RestClient restClient;
      private Encoding UTF8Encoding = UTF8Encoding.UTF8;

      public VivacomGateway()
      {
         resourcesBaseUri = "http://82.137.75.6:3699";
         inboxResourceUri = resourcesBaseUri + "/inbox/";
         sendSMResourceUri = resourcesBaseUri + "/send/";
         username = "txtfeedback";
         password = "txtf33dback";
         restClient = new RestClient();
      }

      public VivacomResponseCode SendSM(string from,
         string to,
         string msg,
         bool withDeliveryReport,
         bool isMessageInHex = false,
         int dcs = 0,
         int esm = 0,
         int pid = 0,
         int validity = 1440) 
       {
          LinkedList<KeyValuePair<string, string>> parameters = 
             new LinkedList<KeyValuePair<string,string>>();
          parameters.AddLast(new KeyValuePair<string, string>("uid", username));
          parameters.AddLast(new KeyValuePair<string, string>("pass", password));
          parameters.AddLast(new KeyValuePair<string, string>("from", from));
          parameters.AddLast(new KeyValuePair<string, string>("to", to));
          if (isMessageInHex) {
             parameters.AddLast(new KeyValuePair<string, string>("msghex", msg));
             parameters.AddLast(new KeyValuePair<string, string>("dcs", dcs.ToString()));
             parameters.AddLast(new KeyValuePair<string, string>("esm",esm.ToString()));
             parameters.AddLast(new KeyValuePair<string, string>("pid",pid.ToString()));
          } else {
             parameters.AddLast(new KeyValuePair<string, string>("msg",msg));
          }
         if (withDeliveryReport) {
            parameters.AddLast(new KeyValuePair<string, string>("dlr","1"));
         } else {
            parameters.AddLast(new KeyValuePair<string, string>("dlr","0"));
         }
         HttpWebResponse response = restClient.GETResource(sendSMResourceUri, parameters);
         if (response.StatusCode.Equals(HttpStatusCode.OK))
         {
            string responseBody = ConvertStreamToString(response.GetResponseStream());
            string[] responseBodySplitted = responseBody.Split(
               Environment.NewLine.ToCharArray());
            // TODO process the response and return the response code
            response.Close();
            return new VivacomResponseCode(360);
         }
         else
         {
            response.Close();
            return new VivacomResponseCode(900);
         }
       }

      public LinkedList<VivacomShortMessage> CheckInbox(string shortNumber)
      {
         // TODO: Retrieve the messages and mark them as retrieved
         LinkedList<KeyValuePair<string, string>> parameters = 
            new LinkedList<KeyValuePair<string,string>>();
         parameters.AddLast(new KeyValuePair<string, string>("uid", username));
         parameters.AddLast(new KeyValuePair<string, string>("pass", password));
         parameters.AddLast(new KeyValuePair<string, string>("to", shortNumber));
         HttpWebResponse response = restClient.GETResource(inboxResourceUri, parameters);
         // TODO: process response, return a list of messages
         return new LinkedList<VivacomShortMessage>();
      }

      private string ConvertStreamToString(Stream stream)
      {
         StreamReader responseStreamReader = new StreamReader(
               stream, UTF8Encoding);
         string responseString = responseStreamReader.ReadToEnd();
         responseStreamReader.Close();
         return responseString;
      }
   }
}
