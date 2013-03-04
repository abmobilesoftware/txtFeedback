using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ChartValue
    {
        public int value {get; set;}
        public string description {get; set;}
        public bool changed { get; set; }

        public ChartValue(int iValue, string iDescription, bool iChanged = false)
        {
            value = iValue;
            description = iDescription;
            changed = iChanged;
        }
    }
}