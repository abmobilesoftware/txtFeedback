using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SmsFeedback_Take4.MvcLocalization.Models
{
    public class LocalizationDisplayNameAttribute : DisplayNameAttribute
    {
        private DisplayAttribute display;

        public LocalizationDisplayNameAttribute(string resourceName, Type resourceType)
        {
            this.display = new DisplayAttribute()
            {
                ResourceType = resourceType,
                Name = resourceName
            };
        }

        public override string DisplayName
        {
            get
            {
                return display.GetName();
            }
        }
    }
}
