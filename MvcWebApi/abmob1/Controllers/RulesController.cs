﻿using System;
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
        private const string SHORT_ID = "abmob1";

        // GET api/rules/abmob1
        public MsgHandlers Get(string from)
        {
            Agent agent1 = new Agent("support_dev_de@txtfeedback.net", 7);
            Agent agent2 = new Agent("dragos@txtfeedback.net", 7);
            List<Agent> agents = new List<Agent> { agent1 , agent2};
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