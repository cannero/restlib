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
        public readonly Route MatchedRoute;

        public ResourceData(HttpListenerContext context, Route route)
        {
            this.HttpListenerContext = context;
            this.MatchedRoute = route;
        }
    }
}
