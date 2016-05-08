using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text;
using System.Threading;

namespace blqw.PT
{
    public sealed class TestResult : ITestResult
    {
        public int Completed
        {
            get
            {
                return Thread.VolatileRead(ref _Failed) + Thread.VolatileRead(ref _Succeeded);
            }
        }
        public TimeSpan Elapsed { get { return _Stopwatch?.Elapsed ?? TimeSpan.Zero; } }
        public int Failed { get { return _Failed; } }
        public int Succeeded { get { return _Succeeded; } }
        
        private int _Failed;
        private int _Succeeded;
        
        public event TestFailHandler OnFailed;
        public event TestSucceedHandler OnSucceeded;

        private ConcurrentQueue<Result> _ResultQueue = new ConcurrentQueue<Result>();

        struct Result
        {
            public Result(Exception exception)
            {
                Exception = exception;
                Elapsed = TimeSpan.Zero;
            }
            public Result(TimeSpan elapsed)
            {
                Exception = null;
                Elapsed = elapsed;
            }
            public Exception Exception;
            public TimeSpan Elapsed;
        }

        public void AddFail(Exception ex)
        {
            _ResultQueue.Enqueue(new Result(ex));
        }

        public void AddSucceed(TimeSpan elapsed)
        {
            _ResultQueue.Enqueue(new Result(elapsed));
        }

        Stopwatch _Stopwatch;
        Thread _Operater;
        public void Start()
        {
            _Stopwatch = Stopwatch.StartNew();
            _ResultQueue = new ConcurrentQueue<Result>();
            _Operater = new Thread(Operater);
            _Operater.Start();
        }

        private void Operater()
        {
            while (_Stopwatch?.IsRunning == true || _ResultQueue.Count > 0)
            {
                try
                {
                    Result r;
                    if (_ResultQueue.TryDequeue(out r) == false)
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                    if (r.Exception != null)
                    {
                        var failed = Interlocked.Increment(ref _Failed);
                        var temp = OnFailed;
                        if (temp != null)
                        {
                            var args = new TestFailEventArgs(_Succeeded, failed, Elapsed, r.Exception);
                            temp(args);
                        }
                    }
                    else
                    {
                        var succeeded = Interlocked.Increment(ref _Succeeded);
                        var temp = OnSucceeded;
                        if (temp != null)
                        {
                            var args = new TestSucceedEventArgs(succeeded, _Failed, r.Elapsed, Elapsed);
                            temp(args);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception)
                {

                }
            }
            Console.WriteLine();
        }

        public void Stop()
        {
            _Stopwatch?.Stop();
            for (int i = 0; i < 20; i++)
            {
                if (_Operater.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    return;
                }
                Thread.Sleep(500);
            }
            _Operater.Abort();
        }
    }
}
