using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class RepDataRowCell 
    {
        public object v;
        public string f;
        
        public RepDataRowCell(object iV, string iF = "")
        {
            v = iV;
            f = iF;
        }

    }
}
