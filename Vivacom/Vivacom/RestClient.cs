using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vivacom
{
   class RestClient
   {
      public HttpWebResponse GETResource(string baseUri, LinkedList<KeyValuePair<string, string>> parameters)
      {
         StringBuilder uriSb = new StringBuilder(baseUri);
         for (int i = 0; i < parameters.Count; ++i)
         {
            if (i == 0)
            {
               uriSb.AppendFormat("?{0}={1}",
                  parameters.ElementAt(i).Key,
                  parameters.ElementAt(i).Value);
            }
            else
            {
               uriSb.AppendFormat("&{0}={1}",
                  parameters.ElementAt(i).Key,
                  parameters.ElementAt(i).Value);
            }
         }
         HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriSb.ToString());
         request.Method = "GET";
         HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         return response;
      }
   }
}
