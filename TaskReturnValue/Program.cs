using System;
using System.Threading.Tasks;
using System.Threading;

namespace TaskReturnValue
{
    class Program
    {
        static void Main(string[] args)
        {
            int total = 0;
            Task<int> task = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    total += i;
                    Console.WriteLine("Current total {0} i {1} #{2}", total, i, Thread.CurrentThread.ManagedThreadId);
                }
                return total;
            });
            Console.WriteLine("Total {0} #{1}.", task.Result, Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
