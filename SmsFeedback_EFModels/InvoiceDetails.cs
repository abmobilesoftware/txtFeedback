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
    
    public partial class InvoiceDetails
    {
        public int InvoiceDetailsID { get; set; }
        public string Article { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int InvoiceInvoiceId { get; set; }
        public decimal VAT { get; set; }
    
        public virtual Invoice Invoice { get; set; }
    }
}
