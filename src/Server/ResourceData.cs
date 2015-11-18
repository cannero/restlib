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
        public readonly RestRoute MatchedRoute;

        public ResourceData(HttpListenerContext context, RestRoute route)
        {
            this.HttpListenerContext = context;
            this.MatchedRoute = route;
        }
    }
}
