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

        public byte[] Encode(string text)
        {
            return Encoding.GetBytes(text);
        }

        public void AddLastModifiedAndExpires(HttpListenerResponse response, string lastModified, string expires)
        {
            response.AddHeader("Last-Modified", lastModified);
            response.AddHeader("Expires", expires);
        }
        
        public void SendResponse(HttpListenerResponse response, StringResponseData data)
        {
            SendResponse(response, new ByteResponseData(Encoding.GetBytes(data.Content),
                                                        data.ContentType));
 
        }

        public void SendResponse(HttpListenerResponse response, ByteResponseData data)
        {
            response.ContentType = data.ContentType.GetValue();
            byte[] buffer = data.Content;
            WriteAndFlushResponse(response, buffer);
        }

        public void SendZippedResponse(HttpListenerResponse response, StringResponseData data)
        {
            SendZippedResponse(response, new ByteResponseData(Encoding.GetBytes(data.Content),
                                                              data.ContentType));
        }

        public void SendZippedResponse(HttpListenerResponse response, ByteResponseData data)
        {
            response.AddHeader("Content-Encoding", "gzip");
            response.ContentType = data.ContentType.GetValue();
            byte[] buffer = data.Content;
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

        public void SendNotFound(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            response.StatusDescription = "Not Found";
            
            SendResponse(response, new StringResponseData("<HTML><BODY><h1>Not Found</h1></BODY></HTML>",
                                                           ContentType.TextHtml));
        }

        public void SendInternalServerError(HttpListenerResponse response, string exceptionMessage)
        {
            response.StatusCode = 500;
            response.StatusDescription = "Internal Server Error";

            SendResponse(response, new StringResponseData("<HTML><BODY><h1>Internal ServerError</h1>" +
                                                          HttpUtility.HtmlEncode(exceptionMessage)
                                                          .Replace("\n", "<br>") +
                                                          "</BODY></HTML>",
                                                          ContentType.TextHtml));
        }

        public void SendNotModified(HttpListenerResponse response)
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