using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace TxtFeedback_tests.EF
{
    [TestFixture]
    class RESTServicesTest
    {
        [TestFixtureSetUp]
        public void Initialise()
        {
          
        }

        [Test]
        public void callAllRestServicesInTheProject_EveryServiceShouldReplyWith200()
        {
            DirectoryInfo di = new DirectoryInfo(@"D:\Work\Txtfeedback\Repository Git\txtFeedback\MvcWebApi");
            DirectoryInfo[] dirs = di.GetDirectories("*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < dirs.Length; ++i)
            {
                
                
                if (!(dirs[i].Name.ToLower().Equals("testrestservices") || dirs[i].Name.ToLower().Equals("packages")))
                {
                    string url = "http://rest.txtfeedback.net/ccpr/api/rules/?from=abcd";
                    StringBuilder urlBuilder = new StringBuilder("http://rest.txtfeedback.net/");
                    urlBuilder.Append(dirs[i].Name.ToLower());
                    urlBuilder.Append("/api/rules/?from=abcd");
                    Console.WriteLine("Apel la " + urlBuilder.ToString());
                    var request = (HttpWebRequest)WebRequest.Create(urlBuilder.ToString());
                    try
                    {
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Console.WriteLine("Raspuns = " + response.StatusCode);
                        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode); 
                        response.Dispose();
                        request.Abort();
                    }
                    catch (WebException tEx)
                    {
                        Console.WriteLine("A esuat " + urlBuilder.ToString());
                        request.Abort();
                    }
                }
            }
            
        }               
    }
}
