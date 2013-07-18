using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helpers
{
   public class Utilities
   {
      public static string extractVirtualDirectoryName(string localPath)
      {
         string[] separator = new string[] { "/" };
         return localPath.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries)[0];
      }
   }
}