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
      private const string SHORT_ID = "magazin2";

      // GET api/rules/abmob1
      public MsgHandlers Get(string from)
      {
         Agent agent1 = new Agent("magazindemo1@txtfeedback.net", 7);
         Agent agent2 = new Agent("magazindemo2@txtfeedback.net", 7);
         List<Agent> agents = new List<Agent> { agent1, agent2 };
         MsgHandlers listOfAgents = new MsgHandlers(agents);
         return listOfAgents;         
      }

      public List<Voucher> GetVouchersList()
      {
         List<Voucher> vouchers = new List<Voucher>();
         vouchers.Add(new Voucher("1234567810", "10% discount"));
         return vouchers;
      }

      public void UseVoucher(String voucherCode)
      {
         throw new HttpResponseException(HttpStatusCode.NotImplemented);
      }
     
   }
}