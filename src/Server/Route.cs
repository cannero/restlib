using System;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using RestLib.Utils;

namespace RestLib.Server
{
    public class Route : IEquatable<Route>
    {
        public readonly string Path;
        public readonly HttpMethod HttpMethod;

        public Route(string path, HttpMethod method)
        {
            if(string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("cannot be empty", "path");
            }
            this.Path = path;
            this.HttpMethod = method;
            this.QueryString = new NameValueCollection();
        }

        public bool Equals(Route other)
        {
            if(other == null)
            {
                return false;
            }
            if(this == other)
            {
                return true;
            }

            return this.HttpMethod == other.HttpMethod &&
                   this.Path == other.Path;
        }

        public bool Matches(string httpMethod, string rawUrl)
        {
            this.FirstMatchOrEmpty = string.Empty;
            this.QueryString.Clear();

            if (httpMethod.ToUpper() != this.HttpMethod.ToString())
            {
                return false;
            }

            Match match = Regex.Match(rawUrl, this.Path, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return false;
            }

            if (match.Groups.Count > 1)
            {
                FirstMatchOrEmpty = Uri.UnescapeDataString(match.Groups[1].Value);
            }

            GetQueryString(rawUrl);
            return true;
        }

        /// <summary>
        /// filled after Matches with url is called
        /// </summary>
        public string FirstMatchOrEmpty
        {
            get;
            private set;
        }

        /// <summary>
        /// filled after Matches with url is called
        /// </summary>
        public NameValueCollection QueryString
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return "Route: " + HttpMethod + " " + Path;
        }

        private void GetQueryString(string url)
        {
            string[] splittedUrl = url.IndexOf("?") > -1 ?
                url.Split('?') : url.Split('#');
            if (splittedUrl.Length <= 1)
            {
                return;
            }

            string allQueries = splittedUrl[1];
            string[] queries = allQueries.IndexOf("&") > -1 ?
                allQueries.Split('&') : allQueries.Split(';');
            foreach (string query in queries)
            {
                string[] keyAndValues = query.Split('=');
                if (keyAndValues.Length != 2)
                {
                    RestLogger.LogWarning("Route::GetQueryString: {0} does not contain an equal sign", query);
                    continue;
                }

                string key = Uri.UnescapeDataString(keyAndValues[0]);
                string[] values = keyAndValues[1].Split(',');
                foreach (string value in values)
                {
                    this.QueryString.Add(key, Uri.UnescapeDataString(value));
                }
            }
        }
    }
}