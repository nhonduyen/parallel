using System;
using System.Threading.Tasks;
using System.Threading;

namespace ParallelDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/
            Console.WriteLine("Enter a string:");
            string srcString = Console.ReadLine();
            string revert = string.Empty;
            Parallel.ForEach(srcString, (str) =>
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                revert += char.ToUpper(str);
            });
            Console.Write(revert);
            Console.ReadKey();
        }
    }
}
