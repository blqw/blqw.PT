using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blqw.PT
{
    public sealed class ThreadPoolTester : TesterBase
    {
        protected override void OnTesting(int threadCount, ITestWorker worker)
        {
            var max = Math.Min(threadCount * 2, threadCount + 10000);
            ThreadPool.SetMaxThreads(max, max);
            ThreadPool.SetMinThreads(threadCount, threadCount);
            for (int i = 0; i < threadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(o => OnTesting((ITestWorker)o), worker);
            }
        }
    }
}
