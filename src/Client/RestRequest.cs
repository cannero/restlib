using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RestLib.Utils;
using System.Net;
using System.Collections.Specialized;
using System.Web;

namespace RestLib.Client
{
    public class RestRequest
    {
        //the resource string is not encoded and the parameters are not replaced
        string resource;
        string data;
        NameValueCollection queryString = new NameValueCollection();
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        //the value has to be a valid url, no escaping is done
        //only the parameters replaced are escaped
        public string Resource
        {
            get
            {
                string completeResource = resource;
                foreach (KeyValuePair<string, string> kvp in parameters)
                {
                    completeResource = completeResource.Replace("{" + kvp.Key + "}",
                                                                Uri.EscapeDataString(kvp.Value));
                }
                return completeResource;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Resource");
                }
                value = Regex.Replace(value, "^/", "");
                this.resource = value;
            }
        }

        public int Timeout
        {
            get;
            set;
        }

        public string Data
        {
            set
            {
                this.data = value ?? string.Empty;  
            }
        }

        public ContentType ContentType
        {
            get;
            set;
        }

        public HttpMethod Method
        {
            get;
            set;
        }

        public Encoding Encoding
        {
            get;
            set;
        }

        public WebHeaderCollection Headers
        {
            get;
            private set;
        }

        public RestRequest()
        {
            Headers = new WebHeaderCollection();
            Headers.Add(HttpRequestHeader.CacheControl, "no-store, must-revalidate");
            Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
            Encoding = Encoding.UTF8;
            Resource = string.Empty;
            Timeout = 2000;
            Data = string.Empty;
            ContentType = ContentType.TextPlain;
            Method = HttpMethod.GET;
        }

        public string GetResourceAndQuery()
        {
            string path = Resource;

            if (queryString.Count > 0)
            {
                path += "?";

                List<string> listOfQuerys = new List<string>();

                foreach (string key in queryString.AllKeys)
                {
                    string value = queryString.Get(key);//Get returns a comma separated string of all values
                    listOfQuerys.Add(Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(value));
                }

                path += string.Join("&", listOfQuerys.ToArray());
            }
            return path;
        }

        public void AddQuery(string name, string value)
        {
            this.queryString.Add(name, value);
        }

        public void ClearQuery()
        {
            this.queryString.Clear();
        }

        public byte[] GetData()
        {
            return Encoding.GetBytes(data);
        }

        public void AddParameter(string key, string data)
        {
            parameters[key] = data;
        }

        public void ClearParameters()
        {
            parameters.Clear();
        }
    }
}
