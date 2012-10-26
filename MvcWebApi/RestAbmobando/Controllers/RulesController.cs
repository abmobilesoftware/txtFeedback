using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Models;

namespace RestAbmobando.Controllers
{
    public class RulesController : ApiController
    {
        // GET api/<controller>/5
        public MsgHandlers Get(string from)
        {
            Agent agent1 = new Agent("testDA@txtfeedback.net", 7);
            Agent agent2 = new Agent("testdragos1@txtfeedback.net", 7);
            List<Agent> agents = new List<Agent> { agent1, agent2 };
            MsgHandlers listOfAgents = new MsgHandlers(agents);
            return listOfAgents;
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}