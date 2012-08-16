using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    
    public class ReportResourceOptions
    {
        public string seriesType;
        public ReportResourceOptions(string iSeriesType)
        {
            seriesType = iSeriesType;
        }
    }
}