using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RestLib.Server
{
    public struct ResourceData
    {
        public readonly HttpListenerContext HttpListenerContext;
        public readonly string FirstRouteMatchOrEmpty;

        public ResourceData(HttpListenerContext context, string match)
        {
            this.HttpListenerContext = context;
            this.FirstRouteMatchOrEmpty = match;
        }
    }
}
