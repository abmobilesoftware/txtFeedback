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
    
    public partial class Conversation
    {
        public Conversation()
        {
            this.IsSmsBased = false;
            this.Messages = new HashSet<Message>();
            this.ConversationTags = new HashSet<ConversationTag>();
            this.WorkingPoints = new HashSet<WorkingPoint>();
            this.ConversationEvents = new HashSet<ConversationHistory>();
        }
    
        public string ConvId { get; set; }
        public string Text { get; set; }
        public bool Read { get; set; }
        public System.DateTime TimeUpdated { get; set; }
        public string From { get; set; }
        public bool Starred { get; set; }
        public System.DateTime StartTime { get; set; }
        public string WorkingPoint_TelNumber { get; set; }
        public int LastSequence { get; set; }
        public bool IsSmsBased { get; set; }
    
        public virtual ICollection<Message> Messages { get; set; }
        public virtual WorkingPoint WorkingPoint { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<ConversationTag> ConversationTags { get; set; }
        public virtual ICollection<WorkingPoint> WorkingPoints { get; set; }
        public virtual ICollection<ConversationHistory> ConversationEvents { get; set; }
    }
}