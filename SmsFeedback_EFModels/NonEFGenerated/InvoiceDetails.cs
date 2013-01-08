using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SmsFeedback_EFModels
{
    [MetadataType(typeof(InvoiceDetails_Validation))]
   public partial class InvoiceDetails
   {
      #region Calculated fields
      public decimal Total
      {
         get
         {
            return Quantity * Price;
         }
      }

      public decimal VATAmount
      {
         get
         {
            return TotalPlusVAT - Total;
         }
      }

      public decimal TotalPlusVAT
      {
         get
         {
            return Total * (1 + VAT / 100);
         }
      }
      #endregion
   }

   #region Validation
   public class InvoiceDetails_Validation
   {
      [Required]
      public string Article { get; set; }

      [Range(-100000, 100000, ErrorMessage = "Quantity must be between 1 and 100000")]
      public int Quantity { get; set; }

      [Range(0.01, 999999999, ErrorMessage = "Price must be between 0.01 and 999999999")]
      public decimal Price { get; set; }

      [DisplayName("Created")]
      public DateTime DateCreated { get; set; }
   }
   #endregion
}
