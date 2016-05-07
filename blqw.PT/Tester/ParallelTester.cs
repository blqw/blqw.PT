using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT
{
    /// <summary>
    /// 并行任务测试器
    /// </summary>
    public sealed class ParallelTester : TesterBase
    {
        protected override void OnTesting(int threadCount, ITestWorker worker)
        {
            Parallel.For(0, threadCount, i =>
             {
                 OnTesting(worker);
             });
        }
    }
}
