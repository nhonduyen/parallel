using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ProducerConsumerDataflowPattern
{
    public class DataflowProducerConsumer
    {
        public static void Produce(ITargetBlock<byte[]> target)
        {
            var ran = new Random();
            for (int i = 0; i < 10; i++)
            {
                var buffer = new byte[1024];
                ran.NextBytes(buffer);
                target.Post(buffer);
                Console.WriteLine("Post data {0} index {1}", buffer.Length, i);
            }
            target.Complete();
            Console.WriteLine("Post data complete");
        }
        public static async Task<int> ConsumeAsync(IReceivableSourceBlock<byte[]> source)
        {
            var byteProcessed = 0;
            while (await source.OutputAvailableAsync())
            {
                byte[] data;
                while (source.TryReceive(out data))
                {
                    byteProcessed += data.Length;
                    Console.WriteLine("Processing {0} bytes", byteProcessed);
                }
            }
            return byteProcessed;
        }
    }
}
