using System;
using System.Net;
using System.Threading;
using RestLib.Utils;

namespace RestLib.Server
{
    public class Server
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly Thread listenerThread;
        private readonly ProducerConsumerQueue<HttpListenerContext> contextQueue;

        //todo remove
        int timesCalled;

        public bool IsListening
        {
            get;
            private set;
        }

        public Server()
        {
            listenerThread = new Thread(HandleRequest);
        }

        public void Start()
        {
            listener.Prefixes.Add("http://+:1234/");
            listener.Start();
            listenerThread.Start();
            IsListening = true;
        }

        public void Stop()
        {
            try
            {
                listener.Stop();
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
                    WriteOutput(context);
                    
                    timesCalled++;
                    if(timesCalled > 5)
                    {
                        Stop();
                        return;
                    }
                }
                catch(Exception ex)
                {
                    LogException("HandleRequest", ex);
                    Stop();
                    return;
                }
            }
        }

        private void WriteOutput(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = string.Format("<HTML><BODY> Hello world{0}!</BODY></HTML>", timesCalled);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer,0,buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        private void LogException(string caller, Exception ex)
        {
            Console.WriteLine(caller + ": ");
            Console.WriteLine(ex);
        }
    }
}
