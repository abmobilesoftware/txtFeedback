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
      [DisplayName("Tax Id")]
      [Required]
      public string VATID { get; set; }
      [DisplayName("Registration Id")]
      public string RegistrationNumber { get; set; }
      [DisplayName("Postal Code")]
      public string PostalCode { get; set; }
      [Required]
      public string City { get; set; }
      [DisplayName("Bank")]
      [Required]
      public string Bank { get; set; }
      [DisplayName("Bank Account")]
      [Required]
      public string BankAccount { get; set; }
   }
   #endregion
}
