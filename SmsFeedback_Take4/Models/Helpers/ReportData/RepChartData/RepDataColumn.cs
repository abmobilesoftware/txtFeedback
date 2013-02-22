using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class RepDataColumn  {
       public string id;
       public string label;
       public string type;

       public RepDataColumn(string iId, string iType, string iLabel = "")
        {
            id = iId;
            type = iType;
            label = iLabel;
        }

    }

    
}
