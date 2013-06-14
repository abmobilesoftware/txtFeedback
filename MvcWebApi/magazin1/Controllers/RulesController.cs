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
      private const string SHORT_ID = "magazin1";

      // GET api/rules/abmob1
      public MsgHandlers Get(string from)
      {
         Agent agent1 = new Agent("magazindemo1@txtfeedback.net", 7);
         List<Agent> agents = new List<Agent> { agent1 };
         MsgHandlers listOfAgents = new MsgHandlers(agents);
         return listOfAgents;       
      }

      public List<Voucher> GetVouchersList()
      {
         List<Voucher> vouchers = new List<Voucher>();
         vouchers.Add(new Voucher("123456789", "5% discount"));
         return vouchers;
      }

      public void UseVoucher(String voucherCode)
      {
         throw new HttpResponseException(HttpStatusCode.NotImplemented);
      }
   }
}