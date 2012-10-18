using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SmsFeedback_Take4.MvcLocalization.Models;

namespace SmsFeedback_Take4.Models
{
   [Serializable]
   public class WorkingPoint
   {
      public WorkingPoint()
      {

      }

      public WorkingPoint(string telNumber, string name, string descr, string shortID, string xmppSuffix)
      {
         TelNumber = telNumber;
         Name = name;
         Description = descr;
         ShortID = shortID;
         XMPPsuffix = xmppSuffix;
      }
      [Required(ErrorMessageResourceName = "settingsConfigureWpTelNumberError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(50, 8, "settingsErrorWpTelNumberLength", typeof(Resources.Global))]
      public string TelNumber { get; set; }
      
      [Required(ErrorMessageResourceName = "settingsConfigureWpNameError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(40, 6, "settingsErrorWpNameLength", typeof(Resources.Global))]
      public string Name { get; set; }
      
      [Required(ErrorMessageResourceName = "settingsConfigureWpDescriptionError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(160, 0, "settingsErrorWpDescriptionLength", typeof(Resources.Global))]
      public string Description { get; set; }

      public int NrOfSentSmsThisMonth { get; set; }
      public int MaxNrOfSmsToSendPerMonth { get; set; }
      //TODO DA required
      public string ShortID { get; set; }
      //TODO DA required
      public string XMPPsuffix { get; set; }
   }
}