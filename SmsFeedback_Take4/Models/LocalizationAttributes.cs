using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

    public class LocalizationCompareAttribute : CompareAttribute
    {
       private DisplayAttribute display;

       public LocalizationCompareAttribute(string otherProperty, string resourceName, Type resourceType): base(otherProperty)
       {
          this.display = new DisplayAttribute()
          {
             ResourceType = resourceType,
             Name = resourceName
          };
          base.ErrorMessage = display.GetName();

       }
    }


    public class LocalizationStringLengthAttribute : StringLengthAttribute
    {
       private DisplayAttribute display;
       public LocalizationStringLengthAttribute(int maxLength, int minLength, string resourceName, Type resourceType)
          : base(maxLength)
       {
          this.display = new DisplayAttribute()
          {
             ResourceType = resourceType,
             Name = resourceName
          };

          this.MinimumLength = minLength;
          this.ErrorMessage = display.GetName();
       }
    }
}
