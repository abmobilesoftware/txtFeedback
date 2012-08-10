using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportResource
    {
        public string name;
        public string source;

        public ReportResource(string iName, string iSource) 
        {
            name = iName;
            source = iSource;
        }
    }
}