using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models
{
   [Serializable]
   public class WorkingPoint
   {
      public WorkingPoint()
      {

      }

      public WorkingPoint(string telNumber,string name,string descr)
      {
         TelNumber = telNumber;
         Name = name;
         Description = descr;
      }
      
      public string TelNumber { get; set; }
      
      [Required(ErrorMessageResourceName = "settingsConfigureWpName",
                ErrorMessageResourceType = typeof(Resources.Global))]
      public string Name { get; set; }
      
      [Required(ErrorMessageResourceName = "settingsConfigureWpDescription",
                ErrorMessageResourceType = typeof(Resources.Global))]
      public string Description { get; set; }

      public int NrOfSentSmsThisMonth { get; set; }
      public int MaxNrOfSmsToSendPerMonth { get; set; }
   }
}