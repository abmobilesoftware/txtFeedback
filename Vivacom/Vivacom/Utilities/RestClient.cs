using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VivacomLib
{
   class RestClient
   {
      public HttpWebResponse GETResource(string baseUri, List<KeyValuePair<string, string>> parameters, ref string fullUrl)
      {
         string queryString  = String.Join("&", 
            parameters.Select(
            x=> String.Format(
               "{0}={1}", 
               HttpUtility.UrlEncode(x.Key), 
               HttpUtility.UrlEncode(x.Value))));
         string resourceUri = baseUri + "?" + queryString;
         fullUrl = resourceUri;
         HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resourceUri);
         request.Method = "GET";
         HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         return response;
      }
   }
}
