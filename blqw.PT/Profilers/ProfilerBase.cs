using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace blqw.PT.Profilers
{
    public abstract class ProfilerBase : ITestProfiler
    {

        protected abstract bool NeedTimer { get; }

        public virtual TimeSpan Interval { get; set; }

        public virtual ITestResult TestResult { get; private set; }

        public virtual string Value
        {
            get
            {
                return _value;
            }
            protected set
            {
                if (_value != value)
                {
                    _value = value;
                    _ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public abstract string Name { get; }

        EventHandler _ValueChanged;
        Timer _timer;
        private string _value;

        public event EventHandler ValueChanged
        {
            add
            {
                _ValueChanged -= value;
                _ValueChanged += value;
                if (_timer == null && NeedTimer)
                {
                    _timer = new Timer();
                    _timer.Elapsed += _timer_Elapsed;
                    _timer.Interval = Interval.TotalMilliseconds;
                    _timer.AutoReset = true;
                    _timer.Start();
                }
            }
            remove
            {
                _ValueChanged -= value;
                if (_ValueChanged == null)
                {
                    _timer?.Stop();
                }
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (TestResult == null)
            {
                return;
            }
            TimerElapsed();
        }

        protected virtual void TimerElapsed()
        {

        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public virtual void Start(ITestResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            TestResult = result;
        }

        public virtual void Stop()
        {
            TestResult = null;
        }
    }
}
