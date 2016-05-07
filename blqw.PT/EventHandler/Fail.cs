using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT
{
    /// <summary>
    /// 测试失败事件参数
    /// </summary>
    public struct TestFailEventArgs
    {
        public TestFailEventArgs(int succeeded, int failed, TimeSpan totalElapsed, Exception ex)
        {
            Succeeded = succeeded;
            Failed = failed;
            TotalElapsed = totalElapsed;
            Error = ex;
            Completed = failed + succeeded;
        }
        /// <summary>
        /// 测试执行异常
        /// </summary>
        public readonly Exception Error;
        /// <summary>
        /// 测试已完成数
        /// </summary>
        public readonly int Completed;
        /// <summary>
        /// 测试总耗时
        /// </summary>
        public readonly TimeSpan TotalElapsed;
        /// <summary>
        /// 测试失败数量
        /// </summary>
        public readonly int Failed;
        /// <summary>
        /// 测试成功数量
        /// </summary>
        public readonly int Succeeded;

    }
    /// <summary>
    /// 测试失败事件
    /// </summary>
    /// <param name="args"></param>
    public delegate void TestFailHandler(TestFailEventArgs args);
}
