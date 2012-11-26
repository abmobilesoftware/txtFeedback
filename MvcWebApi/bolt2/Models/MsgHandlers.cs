using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class MsgHandlers
    {
        public List<Agent> agents;

        public MsgHandlers(List<Agent> iAgents)
        {
            agents = iAgents;
        }
    }
}