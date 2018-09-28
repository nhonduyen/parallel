using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CancelForEachLoop
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Processor count {0}", Environment.ProcessorCount);
            int[] nums = Enumerable.Range(0, 1000).ToArray();
            var cts = new CancellationTokenSource();
            // Use ParallelOptions instance to store the CancellationToken
            var po = new ParallelOptions();
            po.CancellationToken = cts.Token;
            po.MaxDegreeOfParallelism = Environment.ProcessorCount;
            Console.WriteLine("Press any key to start. Press c to cancel.");
            Console.ReadKey();
            Task.Run(() => {
                Console.WriteLine("Listen {0} at {1}", Thread.CurrentThread.ManagedThreadId, DateTime.Now);
                if (Console.ReadKey().KeyChar == 'c')
                {
                    cts.Cancel();
                }
                Console.WriteLine("Press any key to exit.");
            });

            try
            {
                Parallel.ForEach(nums, po, (num) => {
                    if (po.CancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Cancel thread {0}", Thread.CurrentThread.ManagedThreadId);
                        
                    }
                    var d = Math.Sqrt(num);
                    Console.WriteLine("{0} on {1}",d, Thread.CurrentThread.ManagedThreadId);
                    //po.CancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                cts.Dispose();
            }
            Console.WriteLine("end");
            Console.ReadKey();
        }
    }
}
