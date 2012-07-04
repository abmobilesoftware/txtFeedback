using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmsFeedback_EFModels;

namespace SmsFeedback_Take4.Models.Repositories
{
   public static class EFContext
   {
      private static smsfeedbackEntities _instance = null;
      public static smsfeedbackEntities GetEFContext() {
         if (_instance == null)
         {
            _instance = new smsfeedbackEntities();
            _instance.Connection.Open();               
         }
         return _instance;
      }
   }
}