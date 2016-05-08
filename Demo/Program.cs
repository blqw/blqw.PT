using blqw.PT;
using blqw.PT.Profilers;
using blqw.PT.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var tester = new ThreadTester();
            var worker = new HttpWorker();
            worker.URL = "http://static.apitops.com/kk.js";
            tester.Worker = worker;
            var profiler = new TPSProfiler();
            tester.Profilers.Add(profiler);
            tester.Profilers.Add(new ResponseTimeProfiler());

            tester.Profilers.SetInterval(new TimeSpan(0, 0,0,0,500));
            profiler.ValueChanged += Profiler_ValueChanged;
            var w = worker.CreateNew();

            tester.Output = Console.Out;
            tester.TestingCount = 500;
            tester.TestingDelay = new TimeSpan(0, 0, 1);
            tester.ThreadCount = 50;
            tester.Equally = false;

            tester.Preheat(3);
            tester.Run();
            Console.Read();
        }

        private static void Profiler_ValueChanged(object sender, EventArgs e)
        {
            Console.Title = ((TPSProfiler)sender).Value;
        }
    }
}
