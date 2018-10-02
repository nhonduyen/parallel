using System;
using System.Threading.Tasks.Dataflow;

namespace ProducerConsumerDataflowPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<byte[]>();
            var consumer = DataflowProducerConsumer.ConsumeAsync(bufferBlock);
            DataflowProducerConsumer.Produce(bufferBlock);
            consumer.Wait();
            Console.WriteLine("Process {0} bytes.", consumer.Result);
        
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
