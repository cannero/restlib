using System;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using RestLib.Server;
using RestLib.Utils;

namespace ServerExample
{
    public class MyResource
    {
        public readonly RestRoute FooRoute;
        public readonly RestRoute NotFoundRoute;
        public readonly RestRoute ExceptionRoute;
        public readonly RestRoute HtmlWithCssRoute;
        public readonly RestRoute MatchEverythingRoute;

        readonly ResponseWriter writer = new ResponseWriter();

        public MyResource()
        {
            FooRoute = new RestRoute("^/foo/(.*)(?=/\\?).*$", HttpMethod.GET);
            NotFoundRoute = new RestRoute("^/notFound.*$", HttpMethod.GET);
            ExceptionRoute = new RestRoute("^/ex/.*$", HttpMethod.GET);
            HtmlWithCssRoute = new RestRoute("^/css/.*$", HttpMethod.GET);
            MatchEverythingRoute = new RestRoute("^.*$", HttpMethod.GET);
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

            responseString += "<br><br>RestRoute:<br>";
            RestRoute route = data.MatchedRoute;
            responseString += "<br>matching string: " +
                HttpUtility.HtmlEncode(route.FirstMatchOrEmpty) + "<br>";
            responseString += "<br>query string:<br>";
            if(route.QueryString.Count > 0)
            {
                foreach (string key in route.QueryString.AllKeys)
                {
                    responseString += HttpUtility.HtmlEncode(key) + ":";
                    foreach (string value in route.QueryString.GetValues(key))
                    {
                        responseString += " " + HttpUtility.HtmlEncode(value);
                    }
                    responseString += "<br>";
                }
            }
            else
            {
                responseString += "no query string:<br>";
            }
            responseString += "</BODY></HTML>";
            WriteResponse(data.HttpListenerContext, responseString);
        }

        public void WriteRawUrl(ResourceData data)
        {
            HttpListenerContext context = data.HttpListenerContext;
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