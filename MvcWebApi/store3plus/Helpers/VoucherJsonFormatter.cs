using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using Models;

namespace Helpers
{
   public class VoucherJsonFormatter : BufferedMediaTypeFormatter
   {
      private String callbackFunction;

      public VoucherJsonFormatter(String iCallbackFunction)
      {
         callbackFunction = iCallbackFunction;
         SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/javascript"));
      }

      public override bool CanWriteType(Type type)
      {
         if (type == typeof(Voucher))
         {
            return true;
         }
         else
         {
            Type enumerableType = typeof(IEnumerable<Voucher>);
            return enumerableType.IsAssignableFrom(type);
         }
      }

      public override bool CanReadType(Type type)
      {
         return false;
      }

      public override void WriteToStream(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content)
      {
         using (var writer = new StreamWriter(writeStream))
         {
            var vouchers = value as IEnumerable<Voucher>;
            if (vouchers != null)
            {
               writer.Write(callbackFunction + "(");
               writer.Write(JsonConvert.SerializeObject(vouchers));
               writer.Write(")");
            }
            else
            {
               Voucher voucher = value as Voucher;
               if (voucher == null)
               {
                  throw new InvalidOperationException("Cannot serialize the object");
               }
               writer.Write(callbackFunction + "(");
               writer.Write(JsonConvert.SerializeObject(voucher));
               writer.Write(")");
            }
         }
         writeStream.Close();
      }

   }
}