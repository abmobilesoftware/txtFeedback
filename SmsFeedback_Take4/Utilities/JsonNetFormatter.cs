﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;


namespace SmsFeedback_Take4.Utilities
{
   public class JsonNetFormatter : MediaTypeFormatter
 {
     private JsonSerializerSettings _jsonSerializerSettings;
  
     public JsonNetFormatter(JsonSerializerSettings jsonSerializerSettings)
     {
         _jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();
  
         // Fill out the mediatype and encoding we support
         SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
         Encoding = new UTF8Encoding(false, true);
     }
  
     protected override bool CanReadType(Type type)
     {
         if (type == typeof(IKeyValueModel))
         {
             return false;
         }
  
         return true;
     }
  
     protected override bool CanWriteType(Type type)
     {
         return true;
     }
  
     protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
     {
         // Create a serializer
         JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);
  
         // Create task reading the content
         return Task.Factory.StartNew(() =>
         {
             using (StreamReader streamReader = new StreamReader(stream, Encoding))
             {
                 using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                 {
                     return serializer.Deserialize(jsonTextReader, type);
                 }
             }
         });
     }
  
     protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
     {
         // Create a serializer
         JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);
  
         // Create task writing the serialized content
         return Task.Factory.StartNew(() =>
         {
             using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(stream, Encoding)) { CloseOutput = false })
             {
                 serializer.Serialize(jsonTextWriter, value);
                 jsonTextWriter.Flush();
             }
         });
     }
 }
}