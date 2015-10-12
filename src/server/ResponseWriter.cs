using System;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Web;
using RestLib.Utils;

namespace RestLib.Server
{
    public class ResponseWriter
    {
        public Encoding Encoding
        {
            get;
            set;
        }

        public ResponseWriter()
        {
            Encoding = Encoding.UTF8;
        }

        public void AddLastModifiedAndExpires(HttpListenerResponse response, string lastModified, string expires)
        {
            response.AddHeader("Last-Modified", lastModified);
            response.AddHeader("Expires", expires);
        }
        
        public void WriteResponse(HttpListenerResponse response, ResponseData data)
        {
            response.ContentType = data.ContentType.GetValue();
            byte[] buffer = Encoding.GetBytes(data.Content);
            WriteAndFlushResponse(response, buffer);
        }

        public void WriteZippedResponse(HttpListenerResponse response, ResponseData data)
        {
            response.AddHeader("Content-Encoding", "gzip");
            response.ContentType = data.ContentType.GetValue();
            byte[] buffer = Encoding.GetBytes(data.Content);
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }
                buffer = ms.ToArray();
            }
            
            WriteAndFlushResponse(response, buffer);
        }

        public void WriteNotFound(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            response.StatusDescription = "Not Found";
            
            WriteResponse(response, new ResponseData("<HTML><BODY><h1>Not Found</h1></BODY></HTML>",
                                                     ContentType.TextHtml));
        }

        public void WriteInternalServerError(HttpListenerResponse response, string exceptionMessage)
        {
            response.StatusCode = 500;
            response.StatusDescription = "Internal Server Error";

            WriteResponse(response, new ResponseData("<HTML><BODY><h1>Internal ServerError</h1>" +
                                                     HttpUtility.HtmlEncode(exceptionMessage)
                                                     .Replace("\n", "<br>") +
                                                     "</BODY></HTML>",
                                                     ContentType.TextHtml));
        }

        public void WriteNotModified(HttpListenerResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.NotModified;
            response.Close();
        }

        void WriteAndFlushResponse(HttpListenerResponse response, byte[] buffer)
        {
            response.ContentLength64 = buffer.Length;
            using(Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
            response.Close();
        }
    }
}