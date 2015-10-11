using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using RestLib.Utils;

namespace RestLib.Server
{
    //todo add class which checks if gzip can be used
    //maybe give route reg ex to callback, how is it done in sinatra?
    public class Server
    {
        private readonly ServerConfiguration config;
        private readonly HttpListener listener = new HttpListener();
        private readonly Thread listenerThread;
        private ProducerConsumerQueue<HttpListenerContext> contextQueue;
        readonly Dictionary<Route, Action<HttpListenerContext>> resources =
             new Dictionary<Route, Action<HttpListenerContext>>();
        ResponseWriter responseWriter = new ResponseWriter();

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
            try
            {
                contextQueue = new ProducerConsumerQueue
                    <HttpListenerContext>(ProcessRequest, config.NumWorkerThreads);
                listener.Prefixes.Add(config.BaseUrl);
                IsListening = true;
                listener.Start();
                listenerThread.Start();
            }
            catch(Exception ex)
            {
                IsListening = false;
                LogException("Start", ex);
                throw;
            }
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
            Route route = resources.Keys.Where(rt => rt.Matches(context.Request.HttpMethod,
                                                                context.Request.RawUrl))
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
                    responseWriter.WriteInternalServerError(context.Response,
                                                            ex.ToString());
                }
            }
            else
            {
                responseWriter.WriteNotFound(context.Response);
            }
        }

        private void LogException(string caller, Exception ex)
        {
            Console.WriteLine("Server::" + caller + ": ");
            Console.WriteLine(ex);
        }
    }
}
