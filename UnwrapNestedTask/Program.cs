using System;
using System.Threading.Tasks;

namespace UnwrapNestedTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var task1 = Task.Run(() => DoSomeWork());
            var task2 = task1.ContinueWith((s) => DoMoreWork(s.Result));
            Console.WriteLine(task2.Result);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        private static string DoSomeWork()
        {
            return string.Format("Do some work {0}", DateTime.Now);
        }
        private static string DoMoreWork(string s)
        {
            return string.Format("Do some work {0} {1}", DateTime.Now.AddHours(1), s);
        }
    }
}
