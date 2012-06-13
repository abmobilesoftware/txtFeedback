using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmsFeedback_Take4.Utilities
{
   public class CustomJsonResult : JsonResult
   {
      public CustomJsonResult()
      {
         JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      }
      public override void ExecuteResult(ControllerContext context)
      {
         if (context == null)
         {
            throw new ArgumentNullException("context");
         }
         //if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
         //    String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
         //{
         //   throw new InvalidOperationException();
         //}

         HttpResponseBase response = context.HttpContext.Response;

         if (!String.IsNullOrEmpty(ContentType))
         {
            response.ContentType = ContentType;
         }
         else
         {
            response.ContentType = "application/json";
         }
         if (ContentEncoding != null)
         {
            response.ContentEncoding = ContentEncoding;
         }
         if (Data != null)
         {
            JsonNetFormatter serializer = new JsonNetFormatter(new Newtonsoft.Json.JsonSerializerSettings());
            response.Write(serializer.Serialize(Data));
         }
      }
   }
}