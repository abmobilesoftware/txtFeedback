using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportData
    {
        public RepChartDataArray ChartDataArray;
        public RepInfoBoxArray InfoBoxArray;

        public ReportData(RepChartDataArray chartDataArray, RepInfoBoxArray infoBoxArray)
        {
            ChartDataArray = chartDataArray;
            InfoBoxArray = infoBoxArray;
        }
    }
}