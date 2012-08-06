using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class RepDataRow
    {
        /* 
            * the reason of using an IEnumerable structure and not a pair 
            * is because the json generated is a general json which fits 
            * a large number of charts, one of them with multiple dimensions. 
            * Also this structure is intended primarly to be displayed in tables with many columns.
         */
        public IEnumerable<RepDataRowCell> c;
               
        public RepDataRow(IEnumerable<RepDataRowCell> iC)
        {
            c = iC;
        }
        
    }
}
