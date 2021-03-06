//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmsFeedback_EFModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class SubscriptionDetail
    {
        public SubscriptionDetail()
        {
            this.ExtraAddedCreditThisMonth = 0m;
            this.Companies = new HashSet<Company>();
        }
    
        public int Id { get; set; }
        public int BillingDay { get; set; }
        public int SubscriptionSMS { get; set; }
        public int RemainingSMS { get; set; }
        public decimal SpendingLimit { get; set; }
        public decimal WarningLimit { get; set; }
        public decimal SpentThisMonth { get; set; }
        public int PrimaryContact_Id { get; set; }
        public int SecondaryContact_Id { get; set; }
        public string DefaultCurrency { get; set; }
        public Nullable<int> MonthlySubscriptionTemplate_InvoiceDetailsTemplateID { get; set; }
        public Nullable<int> MonthlyExtraSMSCharge_InvoiceDetailsTemplateID { get; set; }
        public decimal ExtraAddedCreditThisMonth { get; set; }
        public decimal RemainingCreditFromPreviousMonth { get; set; }
        public byte[] TimeStamp { get; set; }
    
        public virtual ICollection<Company> Companies { get; set; }
        public virtual Contact PrimaryContact { get; set; }
        public virtual Contact SecondaryContact { get; set; }
        public virtual InvoiceDetailsTemplate MonthlySubscriptionTemplate { get; set; }
        public virtual InvoiceDetailsTemplate ExtraSMSCostsDetails { get; set; }
    }
}
