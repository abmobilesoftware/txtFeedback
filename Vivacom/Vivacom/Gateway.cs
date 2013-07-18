using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace VivacomLib
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
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
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
                md5sumString += deliveryReportString;
                parameters.Add(new KeyValuePair<string, string>("dlr", deliveryReportString));
                string md5sumValue = Utilities.CalculateMD5Hash(md5sumString);
                parameters.Add(new KeyValuePair<string, string>("md5sum", md5sumValue));
                string fullUri = "";
                response = restClient.GETResource(sendSMResourceUri, parameters, ref fullUri);
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    responseStream = response.GetResponseStream();
                    string responseBody = ConvertStreamToString(responseStream);
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
                if (response != null)
                {
                    response.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }
        }

        public string SendSMDetails(string from,
           string to,
           string msg,
           bool withDeliveryReport,
           bool isMessageInHex = false,
           int dcs = 0,
           int esm = 0,
           int pid = 0,
           int validity = 1440)
        {
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
            {
                StringBuilder responseSb = new StringBuilder();
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
                md5sumString += deliveryReportString;
                parameters.Add(new KeyValuePair<string, string>("dlr", deliveryReportString));
                string md5sumValue = Utilities.CalculateMD5Hash(md5sumString);
                parameters.Add(new KeyValuePair<string, string>("md5sum", md5sumValue));
                string fullUri = "";
                responseStream = response.GetResponseStream();
                response = restClient.GETResource(sendSMResourceUri, parameters, ref fullUri);
                responseSb.AppendFormat("HTTP status code {0}", response.StatusCode);
                responseSb.AppendFormat(" ## HTTP status description {0}", response.StatusDescription);
                responseSb.AppendFormat(" ## Response body {0}",
                   ConvertStreamToString(responseStream));
                responseSb.AppendFormat(" ## Request full Uri {0}", fullUri);
                return responseSb.ToString();
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }
        }

        public List<ShortMessage> CheckInbox(string shortNumber)
        {
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
            {
                List<KeyValuePair<string, string>> parameters =
                   new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("uid", username));
                parameters.Add(new KeyValuePair<string, string>("pass", password));
                parameters.Add(new KeyValuePair<string, string>("to", shortNumber));
                string fullUri = "";
                response = restClient.GETResource(inboxResourceUri, parameters, ref fullUri);
                responseStream = response.GetResponseStream();
                string responseBody = ConvertStreamToString(responseStream);
                InboxResponse inboxResponse = new InboxResponse(responseBody.Split(
                          Environment.NewLine.ToCharArray()).Where(x => !x.Equals("")).ToList());
                /*if (inboxResponse.ErrorCode.Equals(ResponseCode.INCORRECT_FROM_FIELD))
                {
                   Console.WriteLine("Number not added in account");
                }
                if (inboxResponse.Messages.Count == 0)
                {
                   Console.WriteLine("No messages");
                }*/
                /*ResponseCode deleteReponseCode = DeleteSM(inboxResponse.Messages);
                if (deleteReponseCode == ResponseCode.OTHER_ERROR)
                {
                   Console.WriteLine("Delete SM: Other error");
                };*/
                return inboxResponse.Messages;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }
        }

        public string CheckInboxDetails(string shortNumber)
        {
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
            {
                StringBuilder responseSb = new StringBuilder();
                List<KeyValuePair<string, string>> parameters =
                   new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("uid", username));
                parameters.Add(new KeyValuePair<string, string>("pass", password));
                parameters.Add(new KeyValuePair<string, string>("to", shortNumber));
                string fullUri = "";
                response = restClient.GETResource(inboxResourceUri, parameters, ref fullUri);
                responseStream = response.GetResponseStream();
                string responseBody = ConvertStreamToString(responseStream);
                List<string> content = responseBody.Split(
                          Environment.NewLine.ToCharArray()).Where(x => !x.Equals("")).ToList();
                InboxResponse inboxResponse = new InboxResponse(content);
                responseSb.AppendFormat("HTTP status code {0}", response.StatusCode);
                responseSb.AppendFormat(" ## HTTP status description {0}", response.StatusDescription);
                responseSb.AppendFormat(" ## Response body {0}", responseBody);
                responseSb.AppendFormat(" ## Url {0}", fullUri);
                /*foreach (var message in inboxResponse.Messages) 
                {
                   responseSb.AppendFormat("## Message text={0} ", message.Msg);
                }*/
                responseSb.AppendFormat("################");
                foreach (var item in content)
                {
                    responseSb.AppendFormat("%% {0} %%", item);
                }
                return responseSb.ToString();
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }
        }


        private ResponseCode DeleteSM(List<ShortMessage> messages)
        {
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
            {
                string usmidsList = String.Join(":", messages.Select(x => x.Usmid.ToString()));
                List<KeyValuePair<string, string>> parameters =
                   new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("uid", username));
                parameters.Add(new KeyValuePair<string, string>("pass", password));
                parameters.Add(new KeyValuePair<string, string>("usmids", usmidsList));
                string md5sum = Utilities.CalculateMD5Hash(username + password + usmidsList);
                parameters.Add(new KeyValuePair<string, string>("md5sum", md5sum));
                string fullUri = "";
                response = restClient.GETResource(deleteSMResourceUri, parameters, ref fullUri);
                responseStream = response.GetResponseStream();
                string responseBody = ConvertStreamToString(responseStream);
                DeleteResponse deleteResponse = new DeleteResponse(responseBody.Split(
                   Environment.NewLine.ToCharArray()).Where(x => !x.Equals("")).ToList());
                return deleteResponse.ErrorCode;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (responseStream != null)
                {
                    response.Close();
                }
            }
        }

        public string DeleteSMDetails(List<string> usmids)
        {
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
            {
                StringBuilder responseSb = new StringBuilder();
                string usmidsList = String.Join(":", usmids);
                List<KeyValuePair<string, string>> parameters =
                   new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("uid", username));
                parameters.Add(new KeyValuePair<string, string>("pass", password));
                parameters.Add(new KeyValuePair<string, string>("usmids", usmidsList));
                string md5sum = Utilities.CalculateMD5Hash(username + password + usmidsList);
                parameters.Add(new KeyValuePair<string, string>("md5sum", md5sum));
                string fullUri = "";
                response = restClient.GETResource(deleteSMResourceUri, parameters, ref fullUri);
                responseStream = response.GetResponseStream();
                responseSb.AppendFormat("HTTP status code {0}", response.StatusCode);
                responseSb.AppendFormat(" ## HTTP status description {0}", response.StatusDescription);
                responseSb.AppendFormat(" ## Response body {0}",
                   ConvertStreamToString(responseStream));
                responseSb.AppendFormat(" ## Url {0}", fullUri);
                return responseSb.ToString();
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
            }
        }

        private string ConvertStreamToString(Stream stream)
        {
            StreamReader responseStreamReader = null;
            try
            {
                responseStreamReader = new StreamReader(
                      stream, encodingUTF8);
                string responseString = responseStreamReader.ReadToEnd();
                return responseString;
            }
            finally
            {
                if (responseStreamReader != null)
                {
                    responseStreamReader.Close();
                }
            }
        }
    }
}
