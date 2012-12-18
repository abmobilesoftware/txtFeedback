using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SmsFeedback_EFModels
{
   [MetadataType(typeof(Company_Validation))]
   public partial class Company
   {

   }

   #region Validation
   public class Company_Validation
   {
      //TODO add error messages
      [Required]
      public string Name { get; set; }
      [Required]
      public string Description { get; set; }
      [Required]
      public string Address { get; set; }      
      [DisplayName("TaxID")]
      [Required]
      public string VATID { get; set; }
      [DisplayName("RegistrationID")]
      public string RegistrationNumber { get; set; }
      [Required]
      public string PostalCode { get; set; }
      [Required]
      public string City { get; set; }      
      
   }
   #endregion
}
