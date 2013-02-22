using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class RepChartData
    {
       public IEnumerable<RepDataColumn> cols;
       public IEnumerable<RepDataRow> rows;
       
        public RepChartData(IEnumerable<RepDataColumn> iCols, IEnumerable<RepDataRow> iRows)
        {
            cols = iCols;
            rows = iRows;
        }

    }
}
