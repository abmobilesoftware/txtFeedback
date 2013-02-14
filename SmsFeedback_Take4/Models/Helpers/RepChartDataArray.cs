using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class RepChartDataArray
    {
        public LinkedList<RepChartData> ChartDataArray;
        public RepChartDataArray(LinkedList<RepChartData> chartDataArray)
        {
            ChartDataArray = chartDataArray;
        }
    }
}