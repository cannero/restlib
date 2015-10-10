using System;
using System.Threading;
using RestLib.Server;

namespace ServerExample
{
    public class ServerMain
    {
        public static void Main(string[] args)
        {
            var config = new ServerConfiguration
            {
                Host = "+"
            };

            Server server = new Server(config);
            MyRoute route = new MyRoute();
            server.AddResource(route.FooRoute, route.WriteMore);
            server.AddResource(route.NotFoundRoute, route.WriteNotFound);
            server.AddResource(route.MatchEverythingRoute, route.WriteRawUrl);
            server.Start();

            while (server.IsListening)
            {
                Thread.Sleep(300);
            }

        }
    }
}