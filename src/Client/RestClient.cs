using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using RestLib.Utils;
using System.IO;

namespace RestLib.Client
{
    public class RestClient
    {
        string root;
        NetworkCredential credentialsOrNull;

        public CookieContainer Cookies
        {
            get;
            private set;
        }
        
        public RestClient(string url, NetworkCredential credentialsOrNull)
        {
            this.Cookies = new CookieContainer();
            url = SanitizeUrl(url);
            this.root = url;
            RestLogger.LogInfo("Client root {0}", this.root);
            this.credentialsOrNull = credentialsOrNull;
        }

        private static string SanitizeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            url = Uri.EscapeUriString(url);
            
            return url;
        }
        
        /// <summary>
        /// todo all exceptions except WebException have to be handled by user
        /// </summary>
        public RestResponse SendRequest(RestRequest request)
        {
            string url = root + request.GetResourceAndQuery();
            HttpWebRequest httpRequest = CreateHttpRequest(request, url);
            SetData(request, httpRequest);

            RestResponse response;
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                response = new RestResponse(httpResponse);
                this.Cookies.Add(httpResponse.Cookies);
            }
            catch (WebException ex)
            {
                response = new RestResponse(ex);
            }
            return response;
        }

        private HttpWebRequest CreateHttpRequest(RestRequest restRequest, string url)
        {
            //cast to HttpWebRequest is needed for the CookieContainer and Credentials
            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpRequest.Timeout = restRequest.Timeout;
            httpRequest.Method = restRequest.Method.ToString();
            httpRequest.Headers = restRequest.Headers;
            httpRequest.ContentType = restRequest.ContentType.GetValue();
            httpRequest.CookieContainer = Cookies;
            if (credentialsOrNull != null)
            {
                httpRequest.Credentials = credentialsOrNull;
            }
            return httpRequest;
        }

        private static void SetData(RestRequest restRequest, HttpWebRequest httpRequest)
        {
            byte[] data = restRequest.GetData();
            httpRequest.ContentLength = data.Length;
            if (data.Length > 0)
            {
                using (Stream httpRequestStream = httpRequest.GetRequestStream())
                {
                    httpRequestStream.Write(data, 0, data.Length);
                }
            }
        }
    }
}
