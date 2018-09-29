using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace SpeedUpSmallLoopBodies
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = Enumerable.Range(0, 1000).ToArray();
            // Partition the entire source array
            var rangePartitioner = Partitioner.Create(0, source.Length);
            var results = new double[source.Length];

            // Loop over the partitions in parallel.
            Parallel.ForEach(rangePartitioner, (range, loopstate) =>
            {
                Console.WriteLine("Range {0}", range);
                // Loop over each range element without a delegate invocation.
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    results[i] = source[i] * Math.PI;
                    Console.WriteLine("From {0} To {1} partition {2}  result {3} thread #{4}", i, range.Item2, range, results[i], Thread.CurrentThread.ManagedThreadId);
                }
            });
            Console.WriteLine("Complete. Print result? Y/N");
            var input = Console.ReadKey().KeyChar;
            if (char.ToLower(input) == 'y')
            {
                foreach (var d in results)
                {
                    Console.WriteLine("{0} ", d);
                }
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
