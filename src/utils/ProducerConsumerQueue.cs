using System;
using System.Threading;
using System.Collections.Generic;

namespace RestLib.Utils
{
    public class ProducerConsumerQueue<T> : IDisposable where T : class
    {
        EventWaitHandle workHandle = new ManualResetEvent(false);
        EventWaitHandle stopHandle = new ManualResetEvent(false);
//todo: array of worker threads, constructor argument for number of workers
        Thread[] workers;
        readonly object locker = new object();
        Queue<T> tasks = new Queue<T>();
        Action<T> workCallback;

        public ProducerConsumerQueue(Action<T> workCallback)
            :this(workCallback, 1)
        {
            
        }

        public ProducerConsumerQueue(Action<T> workCallback, int numberOfWorkers)
        {
            if (workCallback == null)
            {
                throw new ArgumentNullException("workCallback");
            }
            this.workCallback = workCallback;

            if(numberOfWorkers < 1)
            {
                throw new ArgumentException("has to be greater or equal 1", "numberOfWorkers");
            }
            workers = new Thread[numberOfWorkers];
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i] = new Thread(Work);
                workers[i].Start();
            }
        }
        
        public void EnqueueTask(T task)
        {
            if(task != null)
            {
                lock(locker)
                {
                    tasks.Enqueue(task);
                }
            }
            workHandle.Set();
        }

        public void Dispose()
        {
            stopHandle.Set();
            foreach(Thread worker in workers)
            {
                worker.Join();
            }
            workHandle.Close();
            stopHandle.Close();
        }

        void Work()
        {
            const int positionWorkHandle = 0;
            const int positionStopHandle = 1;
            WaitHandle[] handles = new WaitHandle[2];
            handles[positionWorkHandle] = workHandle;
            handles[positionStopHandle] = stopHandle;

            while (WaitHandle.WaitAny(handles) == positionWorkHandle)
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
            Console.WriteLine("PC returning");
        }
    }
}