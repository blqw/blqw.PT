using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace blqw.PT
{
    public abstract class TesterBase : ITester
    {
        private void WriteLine(string message)
        {
            Output?.WriteLine(message);
        }

        public virtual bool Equally { get; set; }
        public virtual IProfilerCollection Profilers { get; set; }
        public virtual int TestingCount { get; set; }
        public virtual TimeSpan TestingDelay { get; set; }
        public virtual int ThreadCount { get; set; }
        public virtual ITestWorker Worker { get; set; }

        public TextWriter Output { get; set; }

        public virtual void Preheat(int count = 1)
        {
            var worker = Worker;
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(Worker));
            }
            OnInitialize(worker);
            for (int i = 0; i < count; i++)
            {
                var ex = worker.Testing();
                if (ex != null)
                {
                    throw new TestingException("测试预热出现异常", ex);
                }
            }
            OnCleanup(worker);
        }

        public virtual ITestResult Run()
        {
            var worker = Worker;
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(Worker));
            }
            OnStart();
            var result = Result = new TestResult();
            OnInitialize(worker);
            OnCleanup(worker);
            OnTesting(threadCount, worker);
            OnEnd();
            return result;
        }

        int isrunning;
        int tastingCount;
        int undone;
        int threadCount;
        int activeTesting;
        bool iscancel;
        TimeSpan delay;
        int eachmax;
        TestResult Result;

        public bool IsCompleted
        {
            get
            {
                if (Result.Completed >= tastingCount)
                {
                    return true;
                }
                else if (iscancel && activeTesting == 0)
                {
                    return true;
                }
                return false;
            }
        }

        private void OnStart()
        {
            var v = Interlocked.CompareExchange(ref isrunning, 1, 0);
            if (v == 1)
            {
                throw new TestingException("正在进行测试");
            }
            iscancel = false;
            tastingCount = TestingCount;
            undone = tastingCount;
            delay = TestingDelay;
            threadCount = ThreadCount;
            eachmax = undone;
            if (Equally)
            {
                eachmax = (undone + (threadCount - 1)) / threadCount;
            }
            WriteLine("当前测试规则:");
            WriteLine($"线程数:{threadCount}");
            WriteLine($"测试任务总数:{undone}");
            WriteLine($"单线程最多执行任务数:{eachmax}");
            WriteLine($"两次任务执行延迟:{delay.TotalMilliseconds} ms");
            WriteLine($"测试提供程序:{this.ToString()}");
            Profilers.Start(Result);
        }

        private void OnEnd()
        {
            while (IsCompleted == false)
            {
                Thread.Sleep(100);
            }
            isrunning = 0;
            WriteLine("测试报告");
            foreach (var profiler in Profilers)
            {
                var s = profiler.Value;
                if (string.IsNullOrWhiteSpace(s) == false)
                {
                    Console.WriteLine(s);
                }
            }
            Profilers.Stop();
        }

        protected abstract void OnTesting(int threadCount, ITestWorker worker);

        protected static void OnCleanup(ITestWorker worker)
        {
            try
            {
                worker.Cleanup();
            }
            catch (Exception ex)
            {
                throw new TestingException("测试任务 Cleanup 方法异常", ex);
            }
        }

        protected static void OnInitialize(ITestWorker worker)
        {
            try
            {
                worker.Initialize();
            }
            catch (Exception ex)
            {
                throw new TestingException("测试任务 Initialize 方法异常", ex);
            }
        }

        protected void OnTesting(ITestWorker worker)
        {
            Interlocked.Increment(ref activeTesting);
            worker = worker.CreateNew();
            OnInitialize(worker);
            var sw = new Stopwatch();
            for (int i = 0; i < eachmax; i++)
            {
                if (iscancel)
                {
                    break;
                }
                var work = Interlocked.Decrement(ref undone);
                if (work <= 0)
                {
                    break;
                }
                try
                {
                    sw.Start();
                    var ex = worker.Testing();
                    sw.Stop();
                    Result.AddSucceed(sw.Elapsed);
                }
                catch (Exception ex)
                {
                    Result.AddFail(ex);
                }
            }
            OnCleanup(worker);
            Interlocked.Decrement(ref activeTesting);
        }

        public void Cancel(bool async = false)
        {
            iscancel = true;
            if (async == false)
            {
                while (IsCompleted)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
