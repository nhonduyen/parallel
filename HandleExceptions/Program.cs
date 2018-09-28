using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;


namespace HandleExceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create some random data to process in parallel.
            // There is a good probability this data will cause some exceptions to be thrown.
            byte[] data = new byte[500];
            var r = new Random();
            r.NextBytes(data);

            try
            {
                ProcessDataParallel(data);
            }
            catch (AggregateException ae)
            {
                var ignoredException = new List<Exception>();
                foreach (var ex in ae.Flatten().InnerExceptions)
                {
                    if (ex is ArgumentException)
                        Console.WriteLine(ex.Message);
                    else
                        ignoredException.Add(ex);
                }
                if (ignoredException.Count > 0)
                    throw new AggregateException(ignoredException);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        private static void ProcessDataParallel(byte[] data)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            Parallel.ForEach(data, d => {
                try
                {
                    if (d < 3)
                        throw new ArgumentException("Value is {d} value must > 3");
                    else
                        Console.Write(d + " ");
                }
                catch(Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });
            Console.WriteLine();
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }
    }
}
