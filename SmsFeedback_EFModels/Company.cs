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
    
    public partial class Company
    {
        public Company()
        {
            this.Tags = new HashSet<Tag>();
            this.Users = new HashSet<User>();
            this.Invoices = new HashSet<Invoice>();
        }
    
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string RegistrationNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Notes { get; set; }
        public string VATID { get; set; }
        public int Contact_Id { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
        public int SubscriptionDetails_Id { get; set; }
    
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual SubscriptionDetail SubscriptionDetail { get; set; }
    }
}
