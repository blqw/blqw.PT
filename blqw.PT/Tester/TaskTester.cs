using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT
{
    public sealed class TaskTester : TesterBase
    {
        protected override void OnTesting(int threadCount, ITestWorker worker)
        {
            var tasks = new Task[threadCount];
            var factory = new TaskFactory();
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = factory.StartNew(o => OnTesting((ITestWorker)o), worker);
            }
            Task.WaitAll(tasks);
        }
    }
}
