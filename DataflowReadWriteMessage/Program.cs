using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataflowReadWriteMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>();
            
            var post1 = Task.Run(() =>
            {
                Console.WriteLine("Post12");
                bufferBlock.Post(0);
                bufferBlock.Post(1);
            });
            var recieve = Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine("recieve " + bufferBlock.Receive());
                }
            });
            var post2 = Task.Run(() =>
            {
                Console.WriteLine("Post12");
                bufferBlock.Post(2);
            });

            Task.WaitAll(post1, recieve, post2);
            AsyncSendReceive(bufferBlock).Wait(); 
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        static async Task AsyncSendReceive(BufferBlock<int> bufferBlock)
        {
            for (int i = 0; i < 3; i++)
            {
                await bufferBlock.SendAsync(i);
            }
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(await bufferBlock.ReceiveAsync());
            }
        }
    }
}
