using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models
{
   public class WorkingPoint
   {
      public WorkingPoint()
      {

      }

      public WorkingPoint(string telNumber,string name,string descr)
      {
         TelNumber = telNumber;
         Name = name;
         Description = descr;
      }

      public string TelNumber { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
   }
}