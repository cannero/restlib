using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using RestLib.Utils;

namespace RestLib.Client
{
    public class RestResponse
    {
        public HttpStatusCode StatusCode
        {
            get;
            private set;
        }

        public WebExceptionStatus ErrorStatus
        {
            get;
            private set;
        }

        public string CompleteErrorMessage
        {
            get;
            private set;
        }

        public string Content
        {
            get;
            private set;
        }

        public bool CallSuccessful
        {
            get
            {
                return ErrorStatus == WebExceptionStatus.Success;
            }
        }

        public RestResponse(HttpWebResponse httpResponse)
        {
            this.ErrorStatus = WebExceptionStatus.Success;
            this.StatusCode = httpResponse.StatusCode;
            this.CompleteErrorMessage = string.Empty;
            this.Content = GetContent(httpResponse);
        }

        public RestResponse(WebException ex)
        {
            this.ErrorStatus = ex.Status;
            this.CompleteErrorMessage = ex.ToString();
            using (HttpWebResponse resp = (HttpWebResponse)ex.Response)
            {
                if (resp != null)//is null if no connection or timeout etc.
                {
                    this.StatusCode = resp.StatusCode;
                    this.Content = GetContent(resp);
                }
            }
        }

        private string GetContent(HttpWebResponse httpResponse)
        {
            try
            {
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    Stream dataStream = stream;
                    if (httpResponse.Headers.AllKeys.Contains("Content-Encoding") &&
                        httpResponse.Headers["Content-Encoding"].Contains("gzip"))
                    {
                        dataStream = new GZipStream(stream, CompressionMode.Decompress);
                    }
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        string content = reader.ReadToEnd();
                        return content;
                    }
                }
            }
            catch (Exception ex)
            {
                RestLogger.Log("Response::GetContent", ex);
            }
            return string.Empty;
        }
    }
}
