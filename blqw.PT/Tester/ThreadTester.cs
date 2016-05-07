using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blqw.PT
{
    public sealed class ThreadTester : TesterBase
    {
        protected override void OnTesting(int threadCount, ITestWorker worker)
        {
            for (int i = 0; i < threadCount; i++)
            {
                new Thread(o => OnTesting((ITestWorker)o)).Start(worker);
            }
        }
    }
}
