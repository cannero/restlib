using System;
using System.Text.RegularExpressions;
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
                FirstMatchOrEmpty = match.Groups[1].Value;
            }
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

        public override string ToString()
        {
            return "Route: " + HttpMethod + " " + Path;
        }
    }
}