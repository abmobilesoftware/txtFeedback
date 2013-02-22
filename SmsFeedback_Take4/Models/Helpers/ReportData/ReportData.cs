using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportData
    {
        public List<RepChartData> charts;
        public List<RepInfoBox> infoBoxes;

        public ReportData(List<RepChartData> iCharts, List<RepInfoBox> iInfoBoxes)
        {
            charts = iCharts;
            infoBoxes = iInfoBoxes;
        }
    }
}