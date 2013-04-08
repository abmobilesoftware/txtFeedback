using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Models;
using System.Web.Mvc;

namespace RestAbmob1.Controllers
{
   public class RulesController : ApiController
   {
      private const string SHORT_ID = "rise";

      // GET api/rules/abmob1
      public MsgHandlers Get(string from)
      {
         Agent agent1 = new Agent("rise@txtfeedback.net", 7);
         List<Agent> agents = new List<Agent> { agent1 };
         MsgHandlers listOfAgents = new MsgHandlers(agents);
         return listOfAgents;        
      }

      public List<Voucher> GetVouchersList()
      {
         return new List<Voucher>();
      }

      public void UseVoucher(String voucherCode)
      {
         throw new HttpResponseException(HttpStatusCode.NotImplemented);
      }
   
   }
}