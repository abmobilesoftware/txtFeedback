using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmsFeedback_EFModels
{
   //we extend the generated Tag class with our own methods
   public partial class Tag
   {
      public Tag()
      {

      }

      #region "Overriding GetHashCode"
      //this was based on http://nhforge.org/blogs/nhibernate/archive/2008/09/06/identity-field-equality-and-hash-code.aspx
      private int? _oldHashCode;
      public override int GetHashCode()
      {
         // Once we have a hash code we'll never change it
         if (_oldHashCode.HasValue)
            return _oldHashCode.Value;

         bool thisIsTransient = Equals(Name, Guid.Empty);

         // When this instance is transient, we use the base GetHashCode()
         // and remember it, so an instance can NEVER change its hash code.
         if (thisIsTransient)
         {
            _oldHashCode = base.GetHashCode();
            return _oldHashCode.Value;
         }
         return Name.GetHashCode();
      }

      #endregion

      #region "Overriding Equals"
      public override bool Equals(object obj)
      {
         var that = obj as Tag;
         return this.Company.Equals(that.Company) && this.Name.Equals(that.Name);         
      }
      #endregion


   }
}
