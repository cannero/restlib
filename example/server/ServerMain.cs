using System;
using System.Threading;
using RestLib;

namespace ServerExample
{
    public class ServerMain
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            while (server.IsListening)
            {
                Thread.Sleep(300);
            }

        }
    }
}