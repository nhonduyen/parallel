using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace CancelTaskChildren
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            // Store references to the tasks so that we can wait on them and  
            // observe their status after cancellation. 
            Task t;
            Task t1;
            var tasks = new ConcurrentBag<Task>();
            Console.WriteLine("Press any key to start");
            Console.ReadKey(true);
            Console.WriteLine("Press c to stop");
            Console.WriteLine();

            t = Task.Run(() => DoSomeWork(1, token), token);
            Console.WriteLine("Task {0} executing", t.Id);
            tasks.Add(t);

            t1 = Task.Run(() => DoSomeWork(2, token), token);
            Console.WriteLine("Task {0} executing", t1.Id);
            tasks.Add(t1);
            var key = Console.ReadKey().KeyChar;
            if (char.ToLower(key) == 'c')
            {
                tokenSource.Cancel();
                Console.WriteLine("Task cancel {0}", DateTime.Now);
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("Error occurs");
                foreach (var err in ae.InnerExceptions)
                {
                    if (err is TaskCanceledException)
                        Console.WriteLine("Task cancel exception {0}", ((TaskCanceledException)err).Task.Id);
                    else
                        Console.WriteLine("Exception {0}", err.GetType().Name);
                }
            }
            finally
            {
                tokenSource.Dispose();
            }
            foreach (var task in tasks)
            {
                Console.WriteLine("Task {0} status is {1}", task.Id, task.Status);
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public static void DoSomeWork(int tasknum, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Task {0} was cancel before it got started", tasknum);
                return;
            }
            var maxIterations = 100;
            for (int i = 0; i <= maxIterations; i++)
            {
                Console.WriteLine("Task {0} {1}.", tasknum, i);
                var sw = new SpinWait();
                for (int j = 0; j <= maxIterations; j++)
                {
                    sw.SpinOnce();
                }
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Task {0} cancelled", tasknum);
                }
            }
        }
    }
}
