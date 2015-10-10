using System;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Web;

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
        
        //todo set also contentType
        public void WriteResponse(HttpListenerResponse response, string content)
        {
            byte[] buffer = Encoding.GetBytes(content);
            WriteAndFlushResponse(response, buffer);
        }

        public void WriteZippedResponse(HttpListenerResponse response, string content)
        {
            response.AddHeader("Content-Encoding", "gzip");
            byte[] buffer = Encoding.GetBytes(content);
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
            
            WriteResponse(response, "<HTML><BODY><h1>Not Found</h1></BODY></HTML>");
        }

        public void WriteInternalServerError(HttpListenerResponse response, string exceptionMessage)
        {
            response.StatusCode = 500;
            response.StatusDescription = "Internal Server Error";

            WriteResponse(response, "<HTML><BODY><h1>Internal ServerError</h1>" +
                          HttpUtility.HtmlEncode(exceptionMessage)
                          .Replace("\n", "<br>") +
                          "</BODY></HTML>");
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