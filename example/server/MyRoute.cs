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
        public readonly Route AllMatchingRoute;
        public MyRoute()
        {
            FooRoute = new Route("^/foo/.*$", HttpMethod.GET);
            AllMatchingRoute = new Route("^.*$", HttpMethod.GET);
        }

        public void WriteMore(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string responseString = string.Format("<HTML><BODY>RawUrl: {0}<br>Local Endpoint {1} </BODY></HTML>", request.RawUrl, request.LocalEndPoint);

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
                        responseString += value + "<br>";
                    }
                }
            }
            WriteResponse(context, responseString);
        }

        public void WriteRawUrl(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            string responseString = string.Format("<HTML><BODY>RawUrl {0} </BODY></HTML>", request.RawUrl);
            WriteResponse(context, responseString);
        }

        void WriteResponse(HttpListenerContext context, string responseString)
        {
            HttpListenerResponse response = context.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer,0,buffer.Length);
            output.Close();
        }
    }
}