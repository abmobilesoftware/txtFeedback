using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class RepInfoBox
    {
        public double value;
        public string unit;

        public RepInfoBox(double iValue, string iUnit)
        {
            value = iValue;
            unit = iUnit;
        }
    }
}