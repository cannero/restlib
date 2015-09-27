using System;
using System.Threading;
using System.Collections.Generic;

namespace RestLib.Utils
{
    public class ProducerConsumerQueue<T> : IDisposable where T : class
    {
        EventWaitHandle workHandle = new ManualResetEvent(false);
        EventWaitHandle stopHandle = new ManualResetEvent(false);
        Thread worker;
        readonly object locker = new object();
        Queue<T> tasks = new Queue<T>();
        Action<T> workCallback;

        public ProducerConsumerQueue(Action<T> workCallback)
        {
            if (workCallback == null)
            {
                throw new ArgumentNullException("workCallback");
            }
            this.workCallback = workCallback;
            worker = new Thread(Work);
            worker.Start();
        }
        
        public void EnqueueTask(T task)
        {
            lock(locker)
            {
                tasks.Enqueue(task);
            }
            workHandle.Set();
        }

        public void Dispose()
        {
            stopHandle.Set();
            worker.Join();
            workHandle.Close();
            stopHandle.Close();
        }

        void Work()
        {
            int positionWorkHandle = 0;
            int positionStopHandle = 1;
            WaitHandle[] handles = new[] { workHandle, stopHandle };
            
            while (true)
            {
                T task = null;
                lock (locker)
                {
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                    }
                }
                if (task != null)
                {
                    workCallback(task);
                }
                else
                {
                    workHandle.Reset();
                }
            }
        }
    }
}