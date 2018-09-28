using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace PartitionLocalvariables
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Processor count {0}", Environment.ProcessorCount);
            int[] nums = Enumerable.Range(0, 1000).ToArray();
            long total = 0;
            // First type parameter is the type of the source elements
            // Second type parameter is the type of the thread-local variable (partition subtotal)
            Parallel.ForEach<int, long>(nums, // source collection
                () => 0, // method to initialize the local variable
                (j, loop, subtotal) =>// method invoked by the loop on each iteration
                {
                    subtotal += j;//modify local variable
                    Console.WriteLine(string.Format("{0} {1} {2}", nums[j], loop, subtotal));
                    return subtotal;// value to be passed to next iteration
                },
                // Method to be executed when each partition has completed.
                // finalResult is the final value of subtotal for a particular partition.
                (finalResult) => Interlocked.Add(ref total, finalResult)
                );
            Console.WriteLine("Result {0}", total);
            Console.ReadKey();
        }
    }
}
