using System;
using System.Threading.Tasks;
using System.Threading;

namespace ParallelInvoke
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            Parallel.Invoke(
                () =>
                {
                    Console.WriteLine("Task 1 invoke {0} #{1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId);
                    for (int i = 0; i < 10; i++)
                    {
                        count++;
                        Console.WriteLine("Task 1 loop {0} #{1}", i, Thread.CurrentThread.ManagedThreadId);
                    }
                    Console.WriteLine("Task 1 count {0}", count);
                },
                 () =>
                 {
                     Console.WriteLine("Task 2 invoke {0} #{1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId);
                     for (int i = 0; i < 10; i++)
                     {
                         count++;
                         Console.WriteLine("Task 2 loop {0} #{1}", i, Thread.CurrentThread.ManagedThreadId);
                     }
                     Console.WriteLine("Task 3 count {0}", count);
                 },
             () =>
             {
                 Console.WriteLine("Task 3 invoke {0} #{1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId);
                 for (int i = 0; i < 10; i++)
                 {
                     count++;
                     Console.WriteLine("Task 3 loop {0} #{1}", i, Thread.CurrentThread.ManagedThreadId);
                 }
                 Console.WriteLine("Task 3 count {0}", count);
             }
                );
            Console.WriteLine("Total count {0}", count);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
