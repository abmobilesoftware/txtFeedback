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
    
    public partial class User
    {
        public User()
        {
            this.WorkingPoints = new HashSet<WorkingPoint>();
            this.Roles = new HashSet<Role>();
        }
    
        public System.Guid ApplicationId { get; set; }
        public System.Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public System.DateTime LastActivityDate { get; set; }
        public string Company_Name { get; set; }
        public string XmppConnectionXmppUser { get; set; }
        public string TypeOfActivityReport { get; set; }
    
        public virtual ICollection<WorkingPoint> WorkingPoints { get; set; }
        public virtual Company Company { get; set; }
        public virtual Application Application { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual XmppConnection XmppConnection { get; set; }
    }
}
