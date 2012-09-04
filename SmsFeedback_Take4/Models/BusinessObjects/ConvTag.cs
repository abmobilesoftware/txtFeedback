using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models
{
   [Serializable]
   public class ConvTag
   {
      public string CompanyName { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string TagType { get; set; }
      public bool IsDefault { get; set; }
   }
}