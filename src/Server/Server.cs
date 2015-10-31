using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using RestLib.Utils;

namespace RestLib.Server
{
    //todo give route reg ex to callback, how is it done in sinatra?
    public class Server
    {
        private readonly ServerConfiguration config;
        private readonly HttpListener listener = new HttpListener();
        private readonly Thread listenerThread;
        private ProducerConsumerQueue<HttpListenerContext> contextQueue;
        readonly Dictionary<Route, Action<ResourceData>> resources =
             new Dictionary<Route, Action<ResourceData>>();
        ResponseWriter responseWriter = new ResponseWriter();
        FileResponder fileResponder;

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
            this.fileResponder = new FileResponder(config.WebRoot, responseWriter);
            listenerThread = new Thread(HandleRequest);
        }

        public void AddResource(Route route, Action<ResourceData> action)
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
                RestLogger.LogInfo("Server starting, {0}", config.BaseUrl);
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
            string url = context.Request.RawUrl;
            Route route = resources.Keys.Where(rt => rt.Matches(context.Request.HttpMethod,
                                                                url))
                                        .FirstOrDefault();
            try
            {
                if (route != null)
                {
                    RestLogger.LogInfo("Server::ProcessRequest: " + route + " found");
                    resources[route](new ResourceData(context, route.FirstMatchOrEmpty));
                }
                else if (fileResponder.FileExists(url))
                {
                    RestLogger.LogInfo("Server::ProcessRequest: file {0} found", url);
                    fileResponder.SendFileResponse(context);
                }
                else
                {
                    RestLogger.LogWarning("Server::ProcessRequest: url {0} not found", url);
                    responseWriter.SendNotFound(context.Response);
                }
            }
            catch(Exception ex)
            {
                LogException("ProcessRequest", ex);
                responseWriter.SendInternalServerError(context.Response,
                                                       ex.ToString());
            }
        }

        private void LogException(string callingFunction, Exception ex)
        {
            RestLogger.Log("Server::" + callingFunction, ex);
        }
    }
}
