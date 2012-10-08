using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class Agent
    {
        public String user;
        public int priority;
        
        public Agent(String iUser, int iPriority)
        {
            user = iUser;
            priority = iPriority;
        }
    }
}