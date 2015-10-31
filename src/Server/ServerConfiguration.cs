using System;

namespace RestLib.Server
{

    public class ServerConfiguration
    {
        public string Protocol { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int NumWorkerThreads { get; set; }
        public string WebRoot { get; set; }

        public ServerConfiguration()
        {
            Protocol = "http";
            Host = "localhost";
            Port = 1234;
            NumWorkerThreads = 1;
            WebRoot = ".";
        }

        public string BaseUrl
        {
            get
            {
                return string.Format("{0}://{1}:{2}/",
                                     Protocol, Host, Port);
            }
        }
    }
}