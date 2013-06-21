using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encoding;

namespace Vivacom
{
   class VivacomUtilities
   {
      public static string ConvertStringToHex(String input)
      {

         Byte[] stringBytes = UTF8Encoding.UTF8.GetBytes(input);
         StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
         foreach (byte b in stringBytes) 
         {
            sbBytes.AppendFormat("{0:X2}", b);
         }
         return sbBytes.ToString();
      }

      public static string ConvertHexToString(String hexInput)
      {
         int hexInputLength = hexInput.Length;
         byte[] bytes = new byte[hexInputLength / 2];
         for (int i = 0; i < hexInputLength; i += 2)
         {
            bytes[i/2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
         }
         return UTF8Encoding.UTF8.GetString(bytes);
      }
   }
}
