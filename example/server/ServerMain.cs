using System;
using System.Threading;
using RestLib.Server;

namespace ServerExample
{
    public class ServerMain
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            MyRoute route = new MyRoute();
            server.AddListenerMethod(route.FooRoute, route.WriteMore);
            server.AddListenerMethod(route.AllMatchingRoute, route.WriteRawUrl);
            server.Start();

            while (server.IsListening)
            {
                Thread.Sleep(300);
            }

        }
    }
}