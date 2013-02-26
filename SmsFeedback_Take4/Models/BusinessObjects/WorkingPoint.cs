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
      public WorkingPoint(SmsFeedback_EFModels.WorkingPoint wp)
      {
         TelNumber = wp.TelNumber;
         Name = wp.Name;
         Description = wp.Description;
         ShortID = wp.ShortID;
         XMPPsuffix = wp.XMPPsuffix;
         WelcomeMessage = wp.WelcomeMessage;
         BusyMessage = wp.BusyMessage;
         BusyMessageTimer = wp.BusyMessageTimer;
      }

      public WorkingPoint(
         string telNumber, string name, string descr,
         string shortID, string xmppSuffix, string welcomeMessage,
         string busyMessage)
      {
         TelNumber = telNumber;
         Name = name;
         Description = descr;
         ShortID = shortID;
         XMPPsuffix = xmppSuffix;
         WelcomeMessage = welcomeMessage;
         BusyMessage = busyMessage;
      }
      [Required(ErrorMessageResourceName = "settingsConfigureWpTelNumberError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(50, 8, "settingsErrorWpTelNumberLength", typeof(Resources.Global))]
      [Display(Name="settingsTelNoHeader",ResourceType=typeof(Resources.Global))]
      public string TelNumber { get; set; }
      
      [Required(ErrorMessageResourceName = "settingsConfigureWpNameError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(40, 6, "settingsErrorWpNameLength", typeof(Resources.Global))]
      [Display(Name="settingsWpNameHeader",ResourceType=typeof(Resources.Global))]
      public string Name { get; set; }
      
      [Required(ErrorMessageResourceName = "settingsConfigureWpDescriptionError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(160, 0, "settingsErrorWpDescriptionLength", typeof(Resources.Global))]
      [Display(Name="settingsWpDescriptionHeader",ResourceType=typeof(Resources.Global))]
      public string Description { get; set; }

      public int NrOfSentSmsThisMonth { get; set; }
      public int MaxNrOfSmsToSendPerMonth { get; set; }
      
      [Required]
      [Display(Name="settingsShortIDHeader",ResourceType=typeof(Resources.Global))]
      public string ShortID { get; set; }
      
      [Required]
      public string XMPPsuffix { get; set; }

      [Required(ErrorMessageResourceName = "settingsConfigureWpWelcomeMsgError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(160, 10, "settingsErrorWpWelcomeLength", typeof(Resources.Global))]
      [Display(Name="settingsWpWelcomeMessageHeader",ResourceType=typeof(Resources.Global))]
      public string WelcomeMessage { get; set; }

      [Required(ErrorMessageResourceName = "settingsConfigureWpBusyMessageMsgError",
                ErrorMessageResourceType = typeof(Resources.Global))]
      [LocalizationStringLength(160, 10, "settingsErrorWpBusyMessageLength", typeof(Resources.Global))]
      [Display(Name = "settingsWpBusyMessageHeader", ResourceType = typeof(Resources.Global))]
      public string BusyMessage { get; set; }

      [Required(ErrorMessageResourceName = "settingsConfigureWpBusyMessageIntervalMsgError", ErrorMessageResourceType = typeof(Resources.Global))]
      [RegularExpression(@"\b([1-9][0-9]{3}|[1-5][0-9]{4}|60000)\b", ErrorMessageResourceName="settingsErrorBusyMessageNotInInterval", ErrorMessageResourceType=typeof(Resources.Global))]
      [Display(Name="settingsConfigureBusyMessageTimerHeader",ResourceType=typeof(Resources.Global))]
      public int BusyMessageTimer { get; set; }
   }
}