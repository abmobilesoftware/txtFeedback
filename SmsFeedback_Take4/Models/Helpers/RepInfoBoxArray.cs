using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class RepInfoBoxArray
    {
        public LinkedList<RepInfoBox> InfoBoxArray;
        public RepInfoBoxArray(LinkedList<RepInfoBox> infoBoxArray) {
            InfoBoxArray = infoBoxArray;
        }
    }
}