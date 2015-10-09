using System;
using System.Net;
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

        public bool Matches(HttpListenerRequest request)
        {
            return request.HttpMethod.ToUpper() == this.HttpMethod.ToString() &&
                Regex.IsMatch(request.RawUrl, this.Path, RegexOptions.IgnoreCase);
        }
    }
}