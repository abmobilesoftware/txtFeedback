using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_EFModels;

namespace SmsFeedback_Take4.Models.Helpers
{
   public class WpAndConversations
   {
      public SmsFeedback_EFModels.WorkingPoint workingPoint { get; set; }
      public IEnumerable<Conversation> conversations { get; set; }
      public SmsFeedback_EFModels.User user { get; set; }
   }
}