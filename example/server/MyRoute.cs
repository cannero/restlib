using System;
using System.Net;
using System.Collections.Specialized;
using RestLib.Server;
using RestLib.Utils;

namespace ServerExample
{
    public class MyRoute
    {
        public readonly Route FooRoute;
        public readonly Route MatchEverythingRoute;
        public readonly Route NotFoundRoute;
        public readonly Route ExceptionRoute;

        readonly ResponseWriter writer = new ResponseWriter();

        public MyRoute()
        {
            FooRoute = new Route("^/foo/.*$", HttpMethod.GET);
            NotFoundRoute = new Route("^/notFound.*$", HttpMethod.GET);
            ExceptionRoute = new Route("^/ex/.*$", HttpMethod.GET);
            MatchEverythingRoute = new Route("^.*$", HttpMethod.GET);
        }

        public void WriteMore(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string responseString = string.Format("<HTML><BODY>RawUrl: {0}<br>Local Endpoint {1}<br>", request.RawUrl, request.LocalEndPoint);

            NameValueCollection headers = request.Headers;
            // Get each header and display each value.
            foreach (string key in headers.AllKeys)
            {
                string[] values = headers.GetValues(key);
                if(values.Length > 0)
                {
                    responseString += string.Format("The values of the {0} header are:<br>", key);
                    foreach (string value in values)
                    {
                        responseString += "&nbsp;&nbsp;&nbsp;" + value + "<br>";
                    }
                }
            }
            responseString += "</BODY></HTML>";
            WriteResponse(context, responseString);
        }

        public void WriteRawUrl(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string responseString = string.Format("<HTML><BODY>RawUrl {0} </BODY></HTML>", request.RawUrl);
            WriteResponse(context, responseString);
        }

        public void WriteNotFound(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            writer.WriteNotFound(response);
        }

        void WriteResponse(HttpListenerContext context, string responseString)
        {
            HttpListenerResponse response = context.Response;
            writer.WriteZippedResponse(response, new ResponseData(responseString,
                                                                  ContentType.TextHtml));
        }
    }
}