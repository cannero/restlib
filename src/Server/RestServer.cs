using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using RestLib.Utils;

namespace RestLib.Server
{
    public class RestServer
    {
        private readonly RestServerConfiguration config;
        private readonly HttpListener listener = new HttpListener();
        private readonly Thread listenerThread;
        private ProducerConsumerQueue<HttpListenerContext> contextQueue;
        readonly Dictionary<RestRoute, Action<ResourceData>> resources =
             new Dictionary<RestRoute, Action<ResourceData>>();
        ResponseWriter responseWriter = new ResponseWriter();
        FileResponder fileResponder;

        public bool IsListening
        {
            get;
            private set;
        }

        public RestServer(RestServerConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            this.config = config;
            this.fileResponder = new FileResponder(config.WebRoot, responseWriter);
            listenerThread = new Thread(HandleRequest);
        }

        public void AddResource(RestRoute route, Action<ResourceData> action)
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
                RestLogger.LogInfo("RestServer starting, {0}", config.BaseUrl);
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
            RestRoute route = resources.Keys.Where(rt => rt.Matches(context.Request.HttpMethod,
                                                                url))
                                        .FirstOrDefault();
            try
            {
                if (route != null)
                {
                    RestLogger.LogInfo("RestServer::ProcessRequest: " + route + " found");
                    resources[route](new ResourceData(context, route));
                }
                else if (fileResponder.FileExists(url))
                {
                    RestLogger.LogInfo("RestServer::ProcessRequest: file {0} found", url);
                    fileResponder.SendFileResponse(context);
                }
                else
                {
                    RestLogger.LogWarning("RestServer::ProcessRequest: url {0} not found", url);
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
            RestLogger.Log("RestServer::" + callingFunction, ex);
        }
    }
}