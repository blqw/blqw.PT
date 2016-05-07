using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT
{
    /// <summary>
    /// 测试成功事件参数
    /// </summary>
    public struct TestSucceedEventArgs
    {
        public TestSucceedEventArgs(int succeeded, int failed, TimeSpan onceElapsed, TimeSpan totalElapsed)
        {
            Succeeded = succeeded;
            Failed = failed;
            OnceElapsed = onceElapsed;
            TotalElapsed = totalElapsed;
            Completed = failed + succeeded;
        }
        /// <summary>
        /// 测试已完成数
        /// </summary>
        public readonly int Completed;
        /// <summary>
        /// 测试总耗时
        /// </summary>
        public readonly TimeSpan TotalElapsed;
        /// <summary>
        /// 单次测试耗时
        /// </summary>
        public readonly TimeSpan OnceElapsed;
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
    /// 测试成功事件
    /// </summary>
    /// <param name="args"></param>
    public delegate void TestSucceedHandler(TestSucceedEventArgs args);
}
