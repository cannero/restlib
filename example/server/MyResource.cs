using System;
using System.Net;
using System.Collections.Specialized;
using RestLib.Server;
using RestLib.Utils;

namespace ServerExample
{
    public class MyResource
    {
        public readonly Route FooRoute;
        public readonly Route NotFoundRoute;
        public readonly Route ExceptionRoute;
        public readonly Route HtmlWithCssRoute;
        public readonly Route MatchEverythingRoute;

        readonly ResponseWriter writer = new ResponseWriter();

        public MyResource()
        {
            FooRoute = new Route("^/foo/(.*)$", HttpMethod.GET);
            NotFoundRoute = new Route("^/notFound.*$", HttpMethod.GET);
            ExceptionRoute = new Route("^/ex/.*$", HttpMethod.GET);
            HtmlWithCssRoute = new Route("^/css/.*$", HttpMethod.GET);
            MatchEverythingRoute = new Route("^.*$", HttpMethod.GET);
        }

        public void WriteMore(ResourceData data)
        {
            HttpListenerRequest request = data.HttpListenerContext.Request;
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

            responseString += "<br>matching string: " + data.FirstRouteMatchOrEmpty + "<br>";
            
            responseString += "</BODY></HTML>";
            WriteResponse(data.HttpListenerContext, responseString);
        }

        public void WriteRawUrl(HttpListenerContext context)
        {
            Console.WriteLine("WriteRawUrl called with " + context.Request.RawUrl);
            HttpListenerRequest request = context.Request;
            string responseString = string.Format("<HTML><BODY>RawUrl {0} </BODY></HTML>", request.RawUrl);
            WriteResponse(context, responseString);
        }

        public void WriteHtmlWithCss(ResourceData data)
        {
            HttpListenerContext context = data.HttpListenerContext;
            Console.WriteLine("WriteHtmlWithCss called with " + context.Request.RawUrl);
            HttpListenerRequest request = context.Request;
            string responseString = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<link rel=\"stylesheet\" href=\"/styles.css\">" +
                "<script src=\"/setBackgroundColor.js\"></script>" +
                "</head>" +
                "<body>" +
                
                "<div id=divH1><h1>This is a heading</h1></div>" +
                "<p>This is a paragraph.</p>" +
                
                "</body>" +
                "</html>";

            WriteResponse(context, responseString);
        }

        public void WriteNotFound(ResourceData data)
        {
            HttpListenerResponse response = data.HttpListenerContext.Response;
            writer.SendNotFound(response);
        }

        void WriteResponse(HttpListenerContext context, string responseString)
        {
            HttpListenerResponse response = context.Response;
            writer.SendZippedResponse(response, new StringResponseData(responseString,
                                                                       ContentType.TextHtml));
        }
    }
}