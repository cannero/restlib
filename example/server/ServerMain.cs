using System;
using System.Threading;
using RestLib.Server;
using System.IO;
using RestLib.Utils;

namespace ServerExample
{
    public class ServerMain
    {
        public static void Main(string[] args)
        {
            var config = new ServerConfiguration
            {
                Host = "+",
                WebRoot = string.Format("..{0}..{0}server{0}files{0}", Path.DirectorySeparatorChar)
            };

            SetLoggingEvents();
            
            Server server = new Server(config);
            MyResource resources = new MyResource();
            server.AddResource(resources.FooRoute, resources.WriteMore);
            server.AddResource(resources.NotFoundRoute, resources.WriteNotFound);
            server.AddResource(resources.ExceptionRoute, (c) => { throw new Exception("exception happend");});
            server.AddResource(resources.HtmlWithCssRoute, resources.WriteHtmlWithCss);
//            server.AddResource(route.MatchEverythingRoute, route.WriteRawUrl);
            server.Start();

            while (server.IsListening)
            {
                Thread.Sleep(300);
            }
        }

        private static void SetLoggingEvents()
        {
            RestLogger.ExceptionEvent += (caller, ex) =>
            {
                Console.WriteLine("Server::" + caller + ": ");
                Console.WriteLine(ex);
            };
            RestLogger.WarningEvent += (warning) => Console.WriteLine(warning);
            RestLogger.InfoEvent += (info) => Console.WriteLine(info);
        }
    }
}