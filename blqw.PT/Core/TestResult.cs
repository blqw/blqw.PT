using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace blqw.PT
{
    public sealed class TestResult : ITestResult
    {
        public int Completed { get { return _Failed + _Succeeded; } }
        public TimeSpan Elapsed { get { return new TimeSpan(_Elapsed); } }
        public int Failed { get { return _Failed; } }
        public int Succeeded { get { return _Succeeded; } }

        private long _Elapsed;
        private int _Failed;
        private int _Succeeded;

        public event TestFailHandler OnFailed;
        public event TestSucceedHandler OnSucceeded;

        public void AddFail(Exception ex)
        {
            var failed = Interlocked.Increment(ref _Failed);
            var temp = OnFailed;
            if (temp != null)
            {
                var args = new TestFailEventArgs(_Succeeded, failed, Elapsed, ex);
                temp(args);
            }
            
        }
        public void AddSucceed(TimeSpan elapsed)
        {
            var totalelapsed = new TimeSpan(Interlocked.Add(ref _Elapsed, elapsed.Ticks));
            var succeeded = Interlocked.Increment(ref _Succeeded);
            var temp = OnSucceeded;
            if (temp != null)
            {
                var args = new TestSucceedEventArgs(succeeded, _Failed, elapsed, totalelapsed);
                temp(args);
            }
        }
    }
}
