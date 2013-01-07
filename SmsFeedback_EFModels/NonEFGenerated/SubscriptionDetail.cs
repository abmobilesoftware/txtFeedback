using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SmsFeedback_EFModels
{
   [MetadataType(typeof(SubscriptionDetail_Validation))]
   public partial class SubscriptionDetail
   {
      #region Calculated properties
      public Decimal RemainingAmount
      {
         get
         {
            var totalToBeSpent = SpendingLimit + ExtraAddedCreditThisMonth + RemainingCreditFromPreviousMonth ;
            return totalToBeSpent - SpentThisMonth;
         }         
      } 

      public Decimal RemainingExtraCreditForNextMonth
      {
         get
         {
            var remainingCredit = Credit - SpentThisMonth;
            if (remainingCredit <= 0)
            {
               //we spent more than our credit -> so no more credit
               return 0;
            }
            else
            {
               return remainingCredit;
            }
         }
      }

      public Decimal Credit
      {
         get {
            return ExtraAddedCreditThisMonth + RemainingCreditFromPreviousMonth;
         }
      }

      public Decimal AmountToBeBilledFor
      {
         get 
         {
            //any extra credit you purchased will be billed
            decimal total = 0;
            //you get billed for extra sms if you went over your purchased credit + what was left from the previous month
            var remainingCredit = Credit - SpentThisMonth;
            if (remainingCredit < 0)
            {
               //this means that we spent more than our credit -> bill me
               total += Math.Abs(remainingCredit);
            }
            return total;
         }
      }
      #endregion
      #region Currencies
      public static IEnumerable<SelectListItem> DefaultCurrencies
      {
         get
         {
            SelectListItem[] currencies = new[] {
               new SelectListItem { Value = "RON", Text = "Romanian Leu" },
               new SelectListItem { Value = "EUR", Text = "Euro" },
               new SelectListItem { Value = "USD", Text = "US Dollar" }
            };
            return currencies;
         }
      }
      #endregion
   }
   #region Validation
   public class SubscriptionDetail_Validation
   {
      [DisplayName("Billing day")]
      [Required]
      [RegularExpression(@"\b([1-9]|1[0-9]|2[0-5])\b", ErrorMessage = "Invalid day: must be between 1 and 25")]
      public int BillingDay { get; set; }
      [DisplayName("SMS' included")]
      [Required]
      public int SubscriptionSMS { get; set; }
      [DisplayName("Spending Limit")]      
      [Required]
      public int SpendingLimit { get; set; }
      [DisplayName("Warning Limit")]
      [Required]
      public int WarningLimit { get; set; }
      [DisplayName("Default currency")]
      [Required]
      public string DefaultCurrency { get; set; }
   }
   #endregion
}
