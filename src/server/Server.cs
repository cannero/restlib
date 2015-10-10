using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using RestLib.Utils;

namespace RestLib.Server
{
    public class Server
    {
        private readonly ServerConfiguration config;
        private readonly HttpListener listener = new HttpListener();
        private readonly Thread listenerThread;
        private ProducerConsumerQueue<HttpListenerContext> contextQueue;
        readonly Dictionary<Route, Action<HttpListenerContext>> resources =
             new Dictionary<Route, Action<HttpListenerContext>>();

        public bool IsListening
        {
            get;
            private set;
        }

        public Server(ServerConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.config = config;
            listenerThread = new Thread(HandleRequest);
            contextQueue = new ProducerConsumerQueue<HttpListenerContext>(ProcessRequest, config.NumWorkerThreads);
        }

        public void AddResource(Route route, Action<HttpListenerContext> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            resources.Add(route, action);
        }

        public void Start()
        {
            listener.Prefixes.Add(config.BaseUrl);
            listener.Start();
            listenerThread.Start();
            IsListening = true;
        }

        public void Stop()
        {
            try
            {
                listener.Stop();
                contextQueue.Dispose();
            }
            catch(Exception ex)
            {
                LogException("Stop", ex);
            }
            IsListening = false;
        }

        private void HandleRequest()
        {
            while (IsListening)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    contextQueue.EnqueueTask(context);
                }
                catch(Exception ex)
                {
                    LogException("HandleRequest", ex);
                    Stop();
                    return;
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            Route route = resources.Keys.Where(rt => rt.Matches(context.Request))
                                        .FirstOrDefault();
            if (route != null)
            {
                try
                {
                    resources[route](context);
                }
                catch(Exception ex)
                {
                    LogException("ProcessRequest", ex);
                }
            }
        }

        private void LogException(string caller, Exception ex)
        {
            Console.WriteLine("Server::" + caller + ": ");
            Console.WriteLine(ex);
        }
    }
}
