using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataflowBlockRecieveData
{
    class Program
    {
        static void Main(string[] args)

        {
            var tempFile = Path.GetTempFileName();

            Task.Run(() => DataflowExecutionBlock.WriteFileAsync(tempFile)).Wait();
          
            // Create an ActionBlock<int> object that prints to the console 
            // the number of bytes read.
            var printResult = new ActionBlock<Task<int>>(zeroBytesRead => {
                Console.WriteLine("{0} Contains {1} zero bytes #{2} total thread {3}", Path.GetFileName(tempFile), zeroBytesRead.Result,
                     Thread.CurrentThread.ManagedThreadId, System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
            });
            // Create a TransformBlock<string, int> object that calls the 
            // CountBytes function and returns its result.
            var countBytes = new TransformBlock<string, Task<int>>((s) => DataflowExecutionBlock.CountBytesAsync(s));
            // Link the TransformBlock<string, int> object to the 
            // ActionBlock<int> object.
            countBytes.LinkTo(printResult);
            // Create a continuation task that completes the ActionBlock<int>
            // object when the TransformBlock<string, int> finishes.
            countBytes.Completion.ContinueWith(delegate { printResult.Complete(); });
            // Post the path to the temporary file to the 
            // TransformBlock<string, int> object.
            countBytes.Post(tempFile);
            // Requests completion of the TransformBlock<string, int> object.
            countBytes.Complete();
            // Wait for the ActionBlock<int> object to print the message.
            printResult.Completion.Wait();
            File.Delete(tempFile);
          
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
