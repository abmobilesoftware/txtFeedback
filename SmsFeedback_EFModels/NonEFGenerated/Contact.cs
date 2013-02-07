using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SmsFeedback_EFModels
{
   [MetadataType(typeof(Contact_Validation))]
   public partial class Contact
   {
   }

   #region Validation
   public class Contact_Validation
   {
      //TODO add error messages
      [Display(Name="contact_Name", ResourceType=typeof(Resources.EntitySpecific))]
      [Required(ErrorMessageResourceName="contact_NameRequired", ErrorMessageResourceType=typeof(Resources.EntitySpecific))]
      public string Name { get; set; }
      [Required(ErrorMessageResourceName = "contact_SurnameRequired", ErrorMessageResourceType = typeof(Resources.EntitySpecific))]
      [Display(Name = "contact_Surname", ResourceType = typeof(Resources.EntitySpecific))]
      public string Surname { get; set; }
      [Display(Name = "contact_Email", ResourceType = typeof(Resources.EntitySpecific))]
      [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessageResourceName = "contact_EmailFormatIncorrect", ErrorMessageResourceType=typeof(Resources.EntitySpecific))]
      [Required(ErrorMessageResourceName = "contact_EmailRequired", ErrorMessageResourceType = typeof(Resources.EntitySpecific))]
      public string Email { get; set; }
   }
   #endregion
}
