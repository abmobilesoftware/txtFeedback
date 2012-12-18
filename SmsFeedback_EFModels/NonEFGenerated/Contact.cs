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
      [Required]
      public string Name { get; set; }
      [Required]
      public string Surname { get; set; }            
      [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Wrong email format")]
      public string Email { get; set; }
   }
   #endregion
}
