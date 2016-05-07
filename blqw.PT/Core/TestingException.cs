using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT
{
    /// <summary>
    /// 测试中出现的异常
    /// </summary>
    public sealed class TestingException : Exception
    {
        public TestingException(string message, Exception innerError = null)
            : base(message, innerError)
        {
            HelpLink = "https://github.com/blqw/blqw.PT";
            Source = "blqw.PT";
        }

        public TestingException(int code, string message, Exception innerError = null)
           : this(message, innerError)
        {
            HResult = code;
        }
    }
}
