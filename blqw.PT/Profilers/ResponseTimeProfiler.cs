using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blqw.PT.Profilers
{
    /// <summary>
    /// 响应时间分析器
    /// </summary>
    public sealed class ResponseTimeProfiler : ProfilerBase
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
                return "响应时间分析器";
            }
        }

        private void TestResult_OnSucceeded(TestSucceedEventArgs args)
        {
            _ticks += args.OnceElapsed.Ticks;
            _max = Math.Max(_max, args.OnceElapsed.Ticks);
            _min = Math.Min(_min, args.OnceElapsed.Ticks);
            var elapsed = new TimeSpan(_ticks);
            Value = $"成功请求数: {args.Succeeded}; 总响应时间: {(int)new TimeSpan(_ticks).TotalMilliseconds} ms; 平均响应时间: {elapsed.TotalMilliseconds / args.Succeeded} ms; 最大响应时间 {(int)new TimeSpan(_max).TotalMilliseconds} ms; 最小响应时间 {(int)new TimeSpan(_min).TotalMilliseconds}ms";
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
