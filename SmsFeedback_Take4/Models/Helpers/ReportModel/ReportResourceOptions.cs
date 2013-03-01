using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_Take4.Utilities;

namespace SmsFeedback_Take4.Models.Helpers
{
    
    public class ReportResourceOptions
    {
        public string seriesType;
        public List<String> colors;
        public ReportResourceOptions(List<String> iColors = null, string iSeriesType = Constants.DEFAULT_CHART_STYLE)
        {
            if (iColors != null)
            {
                colors = iColors;
            }
            else
            {
                colors = new List<String> { "#3366cc", "#dc3912", "#f6b035", "#f635e2", "#3570f6", "#d8f635", "#f6b435", "#f63570", "#35d8f6", "#f65035" };
            }
            seriesType = iSeriesType;
        }
    }
}