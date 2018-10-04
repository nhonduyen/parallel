using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Diagnostics;

namespace DataflowDegreeParallism
{
    class Program
    {
        static void Main(string[] args)
        {
            var processorCount = Environment.ProcessorCount;
            int messageCount = processorCount;

            Console.WriteLine("Processor count {0}.", processorCount);
            TimeSpan elapsed;

            elapsed = TimeDataflowComputation(2, messageCount);
            Console.WriteLine("Degree of parallism {0} message count {1} time {2}.", 2, messageCount, elapsed.TotalMilliseconds);

            elapsed = TimeDataflowComputation(processorCount, messageCount);
            Console.WriteLine("Degree of parallism {0} message count {1} time {2}.", processorCount, messageCount, elapsed.TotalMilliseconds);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

       public static TimeSpan TimeDataflowComputation(int maxDegreeOfParallism, int messageCount)
        {
            var workerBlock = new ActionBlock<int>(
                millisecondTimeout => Thread.Sleep(millisecondTimeout),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallism }
                );
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < messageCount; i++)
            {
                workerBlock.Post(i);
            }
            workerBlock.Complete();
            workerBlock.Completion.Wait();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
