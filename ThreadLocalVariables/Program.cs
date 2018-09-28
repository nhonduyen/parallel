using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadLocalVariables
{
    class Program
    {
        //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-parallel-for-loop-with-thread-local-variables
        static void Main(string[] args)
        {
            int[] nums = Enumerable.Range(0, 1000).ToArray();
            long total = 0;

            Parallel.For<long>(0, nums.Length, () => 0, (j, loop, subtotal) =>
                {
                    subtotal += nums[j];
                    Console.WriteLine(string.Format("{0} {1} {2}", nums[j], loop, subtotal));
                    return subtotal;
                },
                (x) => Interlocked.Add(ref total, x)
                );
            Console.WriteLine("Result {0}", total);
            Console.ReadKey();
        }
    }
}
