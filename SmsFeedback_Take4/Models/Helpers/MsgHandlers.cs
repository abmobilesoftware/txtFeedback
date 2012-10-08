using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
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