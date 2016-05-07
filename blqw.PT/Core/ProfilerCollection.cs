using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT
{
    public sealed class ProfilerCollection :  IProfilerCollection
    {
        List<ITestProfiler> _list = new List<ITestProfiler>();

        public void SetInterval(TimeSpan value)
        {
            lock (this)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].Interval = value;
                }
            }
        }

        public void Start(ITestResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            lock (this)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].TestResult = result;
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].TestResult = null;
                }
            }
        }

        public int Count { get { return _list.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(ITestProfiler item)
        {
            lock (this)
            {
                _list.Add(item);
            }
        }

        public void Clear()
        {
            lock (this)
            {
                _list.Clear();
            }
        }

        public bool Contains(ITestProfiler item)
        {
            lock (this)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(ITestProfiler[] array, int arrayIndex)
        {
            lock (this)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<ITestProfiler> GetEnumerator()
        {
            lock (this)
            {
                return new List<ITestProfiler>(_list).GetEnumerator();
            }
        }

        public bool Remove(ITestProfiler item)
        {
            lock (this)
            {
                return _list.Contains(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
