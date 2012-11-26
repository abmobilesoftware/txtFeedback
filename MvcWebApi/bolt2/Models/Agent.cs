using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
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