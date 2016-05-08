using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace blqw.PT.Profilers
{
    /// <summary>
    /// QPS分析器
    /// </summary>
    public sealed class TPSProfiler : ProfilerBase
    {
        public override string Name
        {
            get
            {
                return "每秒事务(TPS)分析器";
            }
        }

        public override string Value
        {
            get
            {
                var result = TestResult;
                if (result == null)
                {
                    return "分析器尚未运行";
                }
                if (result.Elapsed == TimeSpan.Zero)
                {
                    return $"成功执行次数: {result.Succeeded}; 耗时:{result.Elapsed.TotalMilliseconds.ToString("f2")} ms; TPS: 0; 失败: {result.Failed}";
                }
                return $"成功执行次数: {result.Succeeded}; 耗时:{result.Elapsed.TotalSeconds.ToString("f3")} ms; TPS: { (int)(result.Succeeded / result.Elapsed.TotalSeconds)}; 失败: {result.Failed}";
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }

        protected override bool NeedTimer
        {
            get
            {
                return true;
            }
        }

        protected override void TimerElapsed()
        {
            base.Value = Value;
        }
    }
}
