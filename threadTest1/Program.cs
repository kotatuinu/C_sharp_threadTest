using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace threadTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadTest_OK ttOK = new ThreadTest_OK();
            Task<int> result = ttOK.testMethod1();
            result.Wait();
            System.Console.Write("★OK{0}\n", result.Result);

            ThreadTest_NG ttNG = new ThreadTest_NG();
            result = ttNG.testMethod1();
            result.Wait();
            System.Console.Write("★NG{0}\n", result.Result);
        }
    }

    class ThreadTest_OK
    {
        public async Task<int> testMethod1()
        {
            System.Random r = new System.Random();

            int resultCount = 0;
            var tasks = new List<Task<int>>();

            for (int i=0; 10 > i; i++)
            {
                //Thread.Sleep(r.Next(10,30));
                SC.WriteLine("OK 1:{0}", i);
                testTask task = new testTask(i);
                task.wait = r.Next(100, 300);
                tasks.Add(Task.Run<int>(() => {return task.inc(); }));
            }
            var result = await Task.WhenAll(tasks);
            foreach (Task<int> task in tasks)
            {
                SC.WriteLine("OK 2:{0}", task.Result);
                resultCount += task.Result;
            }

            return resultCount;
        }
    }
    class testTask
    {
        private int val_;
        public int wait { get; set; }

        public testTask(int val)
        {
            val_ = val;
            wait = 1;
        }

        public int inc()
        {
            Thread.Sleep(wait);
            SC.WriteLine("OK 3:{0} wait={1}", val_, wait);
            return val_ + 1;
        }
    }

    class ThreadTest_NG
    {
        public async Task<int> testMethod1()
        {
            System.Random r = new System.Random();

            int resultCount = 0;
            var tasks = new List<Task<int>>();

            for (int i = 0; 10 > i; i++)
            {
                //Thread.Sleep(10);
                SC.WriteLine("NG 1:{0}", i);
                tasks.Add(Task.Run(() => { return inc(i, r.Next(100, 300)); }));
            }
            var result = await Task.WhenAll(tasks);
            foreach (Task<int> task in tasks)
            {
                SC.WriteLine("NG 2:{0}", task.Result);
                resultCount += task.Result;
            }

            return resultCount;
        }

        private int inc(int val, int wait)
        {
            Thread.Sleep(wait);
            SC.WriteLine("NG 3:{0} wait={1}", val, wait);
            return val + 1;
        }
    }

    class SC
    {
        static public void WriteLine(string format, params object[] arg)
        {
#if DEBUG
            System.Console.WriteLine(format, arg);
#else
        return;
#endif
        }
    }

}

