using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Helpers;
using System.Collections.ObjectModel;

namespace App_Start
{
    public class HandlerConfig
    {
        public static void RegisterHandlers(Collection<DelegatingHandler> handlers)
        {
            handlers.Add(new CorsMessageHandler());
        }
    }
}