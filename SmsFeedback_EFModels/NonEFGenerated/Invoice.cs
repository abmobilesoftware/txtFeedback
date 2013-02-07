using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SmsFeedback_EFModels
{
   [MetadataType(typeof(Invoice_Validation))]
   public partial class Invoice
   {
      #region Calculated fields
      public bool IsProposal
      {
         get
         {
            return this.InvoiceNumber == 0;
         }
      }

      public decimal VATAmount
      {
         get
         {
            return this.TotalWithVAT - this.NetTotal;
         }
      }

      /// <summary>
      /// Total before TAX
      /// </summary>
      public decimal NetTotal
      {
         get
         {
            if (InvoiceDetails == null)
               return 0;

            return InvoiceDetails.Sum(i => i.Total);
         }
      }

      public decimal AdvancePaymentTaxAmount
      {
         get
         {
            if (InvoiceDetails == null)
               return 0;
            if (AdvancePaymentTax.HasValue)
            {
               return NetTotal * (AdvancePaymentTax.Value / 100);
            }
            else return 0;
         }
      }

      /// <summary>
      /// Total with tax
      /// </summary>
      public decimal TotalWithVAT
      {
         get
         {
            if (InvoiceDetails == null)
               return 0;

            return InvoiceDetails.Sum(i => i.TotalPlusVAT);
         }
      }

      /// <summary>
      /// Total with VAT minus advanced tax payment 
      /// </summary>
      public decimal TotalToPay
      {
         get
         {
            return TotalWithVAT - AdvancePaymentTaxAmount;
         }
      }
      #endregion
      #region Currencies
      public IEnumerable<SelectListItem> Currencies { 
         get {
            SelectListItem[] currencies = new[] {
               new SelectListItem { Value = "RON", Text = "Romanian Leu" },
               new SelectListItem { Value = "EUR", Text = "Euro" },
               new SelectListItem { Value = "USD", Text = "US Dollar" }
            };
            return currencies;
         } }
      #endregion
   }
   #region Validation
   public class Invoice_Validation
   {
      [Required]
      public string CompanyName { get; set; }

      [DisplayName("Invoice Number")]
      public int InvoiceNumber { get; set; }

      [DisplayName("Invoice name")]
      [Required]
      public string Notes { get; set; }

      [DisplayName("Details")]
      [Required]
      public string ProposalDetails { get; set; }     

      [DisplayName("Created")]
      public DateTime DateCreated { get; set; }

      [DisplayName("Due Date")]
      public DateTime DueDate { get; set; }

      [Required]
      public string Currency { get; set; }

      [DisplayName("Advance Payment Tax")]
      [Range(0.00, 100.0, ErrorMessage = "Value must be a % between 0 and 100")]
      public decimal AdvancePaymentTax { get; set; } 
   }
   #endregion
}
