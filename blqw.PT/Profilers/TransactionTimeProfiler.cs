using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blqw.PT.Profilers
{
    /// <summary>
    /// 事务时间分析器
    /// </summary>
    public sealed class TransactionTimeProfiler : ProfilerBase
    {
        long _ticks;
        long _max;
        long _min = long.MaxValue;
        protected override bool NeedTimer
        {
            get
            {
                return false;
            }
        }

        public override string Name
        {
            get
            {
                return "事务时间分析器";
            }
        }

        private void TestResult_OnSucceeded(TestSucceedEventArgs args)
        {
            _ticks += args.OnceElapsed.Ticks;
            _max = Math.Max(_max, args.OnceElapsed.Ticks);
            _min = Math.Min(_min, args.OnceElapsed.Ticks);
            var elapsed = new TimeSpan(_ticks);
            Value = $"成功事务数: {args.Succeeded}; 总事务时间: {(int)new TimeSpan(_ticks).TotalMilliseconds} ms; 平均事务时间: {elapsed.TotalMilliseconds / args.Succeeded} ms; 最大事务时间 {(int)new TimeSpan(_max).TotalMilliseconds} ms; 最小事务时间 {(int)new TimeSpan(_min).TotalMilliseconds}ms";
        }

        public override void Start(ITestResult result)
        {
            base.Start(result);
            _ticks = 0;
            _max = 0;
            _min = long.MaxValue;
            result.OnSucceeded += TestResult_OnSucceeded;
        }

        public override void Stop()
        {
            if (base.TestResult != null)
            {
                base.TestResult.OnSucceeded -= TestResult_OnSucceeded;
            }
            base.Stop();
        }
    }
}
