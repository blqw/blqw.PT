using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT
{
    /// <summary>
    /// 测试任务属性
    /// </summary>
    public struct WorkerProperty
    {
        public WorkerProperty(string name,string desc)
            :this()
        {
            Name = name;
            Description = desc;
        }
        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
