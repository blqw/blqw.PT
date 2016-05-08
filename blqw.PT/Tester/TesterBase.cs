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
        public virtual IProfilerCollection Profilers { get; set; } = new ProfilerCollection();
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
            if (count <= 0)
            {
                WriteLine("跳过预热阶段");
                return;
            }
            WriteLine($"正在进行{count}次预热");
            OnInitialize(worker);
            for (int i = 0; i < count; i++)
            {
                if (count > 1)
                {
                    WriteLine($"第{i+1}次预热");
                }
                var sw = Stopwatch.StartNew();
                var ex = worker.Testing();
                sw.Stop();
                if (ex != null)
                {
                    throw new TestingException("测试预热出现异常", ex);
                }
                WriteLine($"耗时: {sw.Elapsed.TotalMilliseconds.ToString("f2")} ms");
            }
            OnCleanup(worker);
            WriteLine("预热完成!");
        }

        public virtual ITestResult Run()
        {
            var worker = Worker;
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(Worker));
            }
            var v = Interlocked.CompareExchange(ref _isrunning, 1, 0);
            if (v != 0)
            {
                throw new TestingException("正在进行测试");
            }
            OnStart();
            var result = _Result = new TestResult();
            Profilers.Start(_Result);
            WriteLine($"分析器启动完成...");
            OnInitialize(worker);
            OnCleanup(worker);
            WriteLine("异常检查完成...");
            WriteLine("开始启动线程...");
            new Thread(() =>
            {
                Thread.Sleep(1);
                while (_activeThreads < _threadCount)
                {
                    WriteLine($"已启动线程:{_activeThreads}/{_threadCount}");
                    Thread.Sleep(500);
                }
                WriteLine($"已启动线程:{_activeThreads}/{_threadCount}");
                if (Interlocked.CompareExchange(ref _isrunning, 2, 1) == 1)
                {
                    WriteLine("线程启动完毕,开始测试...");
                    result.Start();
                }
                else
                {
                    WriteLine("测试被中断...");
                }
            }).Start();
            OnTesting(_threadCount, worker);
            OnEnd();
            return result;
        }
        // 0:未启动 1准备中 2正在运行
        int _isrunning;
        int _tastingCount;
        int _undone;
        int _threadCount;
        int _activeThreads;
        bool _iscancel;
        TimeSpan _delay;
        int _eachmax;
        TestResult _Result;

        public bool IsCompleted
        {
            get
            {
                if (_Result.Completed >= _tastingCount)
                {
                    return true;
                }
                else if (_iscancel && _activeThreads == 0)
                {
                    return true;
                }
                return false;
            }
        }

        public int ActiveThreads
        {
            get
            {
                return _activeThreads;
            }
        }

        private void OnStart()
        {
            _iscancel = false;
            _tastingCount = TestingCount;
            _undone = _tastingCount;
            _delay = TestingDelay;
            _threadCount = ThreadCount;
            _eachmax = _undone;
            if (Equally)
            {
                _eachmax = (_undone + (_threadCount - 1)) / _threadCount;
            }
            WriteLine("当前测试规则:");
            WriteLine($"线程数:{_threadCount}");
            WriteLine($"测试任务总数:{_undone}");
            WriteLine($"单线程最多执行任务数:{_eachmax}");
            WriteLine($"两次任务执行延迟:{_delay.TotalMilliseconds} ms");
            WriteLine($"测试提供程序:{this.ToString()}");
        }

        private void OnEnd()
        {
            while (IsCompleted == false)
            {
                Thread.Sleep(100);
            }
            _Result?.Stop();
            Interlocked.Exchange(ref _isrunning, 0);
            WriteLine("测试报告");
            foreach (var profiler in Profilers)
            {
                var s = profiler.Value;
                if (string.IsNullOrWhiteSpace(s) == false)
                {
                    Console.WriteLine($"{profiler.Name} : {s}");
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
            Interlocked.Increment(ref _activeThreads);
            while (true)
            {
                switch (Thread.VolatileRead(ref _isrunning))
                {
                    case 0:
                        return;
                    case 1:
                        Thread.Sleep(5);
                        continue;
                    case 2:
                        break;
                    default:
                        throw new NotSupportedException();
                }
                break;
            }
            worker = worker.CreateNew();
            OnInitialize(worker);
            var sw = new Stopwatch();
            for (int i = 0; i < _eachmax; i++)
            {
                if (_iscancel)
                {
                    break;
                }
                var work = Interlocked.Decrement(ref _undone);
                if (work < 0)
                {
                    break;
                }
                try
                {
                    sw.Restart();
                    var ex = worker.Testing();
                    sw.Stop();
                    _Result.AddSucceed(sw.Elapsed);
                }
                catch (Exception ex)
                {
                    _Result.AddFail(ex);
                }
                if (_delay > TimeSpan.Zero)
                {
                    Thread.Sleep(_delay);
                }
            }
            OnCleanup(worker);
            Interlocked.Decrement(ref _activeThreads);
        }

        public void Cancel(bool async = false)
        {
            _iscancel = true;
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
