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
        public string tooltip;

        public ReportResource(string iName, string iSource, ReportResourceOptions iOptions = null, string iTooltip = null) 
        {
            name = iName;
            source = iSource;
            options = iOptions;
            tooltip = iTooltip;
            if (options == null)
            {
                options = new ReportResourceOptions("area");
            }
            if (tooltip == null)
            {
                tooltip = "no tooltip";
            }
        }
    }
}