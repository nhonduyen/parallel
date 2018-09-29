using System;
using System.Threading.Tasks;
using System.Threading;

namespace TaskCancelations
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancel = new CancellationTokenSource();
            var cancelToken = cancel.Token;
            Task.Run(() =>
            {
                Console.WriteLine("Listen {0} at {1}", Thread.CurrentThread.ManagedThreadId, DateTime.Now);
                if (Console.ReadKey().KeyChar == 'c')
                {
                    cancel.Cancel();
                }
            });
            var task = Task.Run(() =>
            {

                cancelToken.ThrowIfCancellationRequested();
                var moreToDo = true;
                while (moreToDo)
                {
                    Console.WriteLine("Time {0}", DateTime.Now);
                    Task.Delay(500000);
                    if (cancelToken.IsCancellationRequested)
                    {
                        moreToDo = false;
                        //cancelToken.ThrowIfCancellationRequested();
                        Console.WriteLine("Task canceled at {0}", DateTime.Now);
                    }
                }
            }, cancel.Token);

            try
            {
                task.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
            finally
            {
                cancel.Dispose();
                task.Dispose();
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
