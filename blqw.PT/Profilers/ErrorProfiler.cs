using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT.Profilers
{
    /// <summary>
    /// 异常分析器
    /// </summary>
    public sealed class ErrorProfiler : ProfilerBase
    {
        public string FilePath { get; set; } = "error.log";
        public override string Value
        {
            get
            {
                if (_count == 0)
                {
                    return "没有错误";
                }
                return "错误日志已经写入文件:" + FilePath;
            }
            protected set
            {
                throw new NotSupportedException();
            }
        }
        public override string Name
        {
            get
            {
                return "异常分析器";
            }
        }

        protected override bool NeedTimer
        {
            get
            {
                return false;
            }
        }

        public override void Start(ITestResult result)
        {
            base.Start(result);
            _count = 0;
            result.OnFailed += TestResult_OnFailed;
            File.Delete(FilePath);
        }

        public override void Stop()
        {
            if (base.TestResult != null)
            {
                base.TestResult.OnFailed -= TestResult_OnFailed;
            }
            base.Stop();
        }
        int _count;
        private void TestResult_OnFailed(TestFailEventArgs args)
        {
            _count++;
            File.AppendAllText(FilePath, args.Error.ToString() + Environment.NewLine);
        }
    }
}
