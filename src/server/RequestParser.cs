using System;
using System.Net;
using System.Linq;

namespace RestLib.Server
{
    public class RequestParser
    {
        public static bool GzipCanBeUsed(HttpListenerRequest request)
        {
            if (request.Headers.AllKeys.Contains("Accept-Encoding"))
            {
                return GzipCanBeUsed(request.Headers.GetValues("Accept-Encoding"));
            }
            return false;
        }

        public static bool GzipCanBeUsed(string[] values)
        {
            string gzipVal = values.Where(val => val.StartsWith("gzip")).FirstOrDefault();
            if (!string.IsNullOrEmpty(gzipVal))
            {
                return gzipVal != "gzip;q=0";
            }

            string allEncodings = values.Where(val => val.StartsWith("*")).FirstOrDefault();
            if(!string.IsNullOrEmpty(allEncodings))
            {
                return allEncodings != "*;q=0";
            }

            return false;
        }
    }
}