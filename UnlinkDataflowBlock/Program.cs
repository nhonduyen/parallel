using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace UnlinkDataflowBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Func<int, int> action = n => TrySolution(n, cts.Token);

            var trySolution1 = new TransformBlock<int, int>(action);
            var trySolution2 = new TransformBlock<int, int>(action);
            var trySolution3 = new TransformBlock<int, int>(action);

            trySolution1.Post(11);
            trySolution2.Post(65);
            trySolution3.Post(45);

            int result = RecieveFromAny(trySolution1, trySolution2, trySolution3);
            cts.Cancel();
            Console.WriteLine("The solution is {0}.", result);

            cts.Dispose();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public static T RecieveFromAny<T>(params ISourceBlock<T>[] sources)
        {
            var writeOnceBlock = new WriteOnceBlock<T>(e => e);
            foreach (var source in sources)
            {
                source.LinkTo(writeOnceBlock, new DataflowLinkOptions { MaxMessages = 1 });
            }
            return writeOnceBlock.Receive();
        }
        public static int TrySolution(int n, CancellationToken ct)
        {
            SpinWait.SpinUntil(() => ct.IsCancellationRequested,
                new Random().Next(3000));
            return n + 42;
        }
    }
}
