using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using SmsFeedback_Take4.MvcLocalization.Models;

namespace SmsFeedback_Take4.Models
{

   public class ChangePasswordModel
   {
      [Required]
      [DataType(DataType.Password)]
      [Display(Name = "Current password")]
      public string OldPassword { get; set; }

      [Required]
      [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
      [DataType(DataType.Password)]
      [Display(Name = "New password")]
      public string NewPassword { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm new password")]
      [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
      public string ConfirmPassword { get; set; }
   }

   public class LogOnModel
   {
      [Required(ErrorMessageResourceName = "LogOnModel_UserName_Required",
                  ErrorMessageResourceType = typeof(Resources.Global))]      
      [LocalizationDisplayName("LogOnModel_UserName_DisplayName",
                                typeof(Resources.Global))]
      public string UserName { get; set; }

      [Required(ErrorMessageResourceName = "LogOnModel_Password_Required",
                  ErrorMessageResourceType = typeof(Resources.Global))]  
      [DataType(DataType.Password)]
      [LocalizationDisplayName("LogOnModel_Password_DisplayName",
                               typeof(Resources.Global))]
      public string Password { get; set; }

      [LocalizationDisplayName("LogOnModel_RememberMe_DisplayName",
                               typeof(Resources.Global))]
      public bool RememberMe { get; set; }
   }

   public class RegisterModel
   {
      [Required]
      [Display(Name = "User name")]
      public string UserName { get; set; }

      [Required]
      [DataType(DataType.EmailAddress)]
      [Display(Name = "Email address")]
      public string Email { get; set; }

      [Required]
      [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      public string Password { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm password")]
      [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
      public string ConfirmPassword { get; set; }
   }
}
