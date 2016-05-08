using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.PT.Workers
{
    public abstract class WorkerBase : NameObjectCollectionBase, ITestWorker
    {
        public WorkerBase() { }

        public string this[string name]
        {
            get
            {
                return (string)BaseGet(name);
            }
            set
            {
                BaseSet(name, value);
            }
        }

        public virtual void Cleanup()
        {
        }

        public virtual ITestWorker CreateNew()
        {
            return (WorkerBase)MemberwiseClone();
        }

        List<WorkerProperty> _Properties;

        protected void AddProperty(string name, string desc)
        {
            if (_Properties == null)
            {
                _Properties = new List<WorkerProperty>();
            }
            _Properties.Add(new WorkerProperty(name, desc));
        }

        public virtual IEnumerable<WorkerProperty> GetProperties()
        {
            return _Properties;
        }

        public virtual void Initialize()
        {
        }

        public abstract Exception Testing();
    }
}
