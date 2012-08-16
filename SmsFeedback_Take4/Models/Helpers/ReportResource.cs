using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportResource
    {
        public string name;
        public string source;
        public ReportResourceOptions options;

        public ReportResource(string iName, string iSource, ReportResourceOptions iOptions = null) 
        {
            name = iName;
            source = iSource;
            options = iOptions;
            if (options == null)
            {
                options = new ReportResourceOptions("area");
            }
        }
    }
}