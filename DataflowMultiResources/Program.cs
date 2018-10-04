using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace DataflowMultiResources
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create three BufferBlock<T> objects. Each object holds a different
            // type of resource.
            var networkResources = new BufferBlock<NetworkResource>();
            var fileResources = new BufferBlock<FileResource>();
            var memoryResource = new BufferBlock<MemoryResource>();

            // Create two non-greedy JoinBlock<T1, T2> objects. 
            // The first join works with network and memory resources; 
            // the second pool works with file and memory resources.
            var joinNetworkAndMemoryResources = new JoinBlock<NetworkResource, MemoryResource>(
                new GroupingDataflowBlockOptions { Greedy = false });
            // A non-greedy join block postpones all incoming messages until one is available from each source, for efficient memory resource and prevent deadlock
            var joinFileAndMemoryResources = new JoinBlock<FileResource, MemoryResource>(
               new GroupingDataflowBlockOptions { Greedy = false });

            // Create two ActionBlock<T> objects. 
            // The first block acts on a network resource and a memory resource.
            // The second block acts on a file resource and a memory resource.
            var networkMemoryAction = new ActionBlock<Tuple<NetworkResource, MemoryResource>>(
                data =>
                {
                    Console.WriteLine("Networker using resource....");
                    Thread.Sleep(new Random().Next(500, 2000));
                    Console.WriteLine("Networker finish using resource.");
                    // Release the resources back to their respective pools.
                    networkResources.Post(data.Item1);
                    memoryResource.Post(data.Item2);
                }
                );
            var fileMemoryAction = new ActionBlock<Tuple<FileResource, MemoryResource>>(
               data =>
               {
                   Console.WriteLine("Fileworker using resource....");
                   Thread.Sleep(new Random().Next(500, 2000));
                   Console.WriteLine("Fileworker finish using resource.");
                   // Release the resources back to their respective pools.
                   fileResources.Post(data.Item1);
                   memoryResource.Post(data.Item2);
               }
               );

            // Link the resource pools to the JoinBlock<T1, T2> objects.
            // Because these join blocks operate in non-greedy mode, they do not
            // take the resource from a pool until all resources are available from
            // all pools.
            networkResources.LinkTo(joinNetworkAndMemoryResources.Target1);
            memoryResource.LinkTo(joinNetworkAndMemoryResources.Target2);

            fileResources.LinkTo(joinFileAndMemoryResources.Target1);
            memoryResource.LinkTo(joinFileAndMemoryResources.Target2);

            // Link the JoinBlock<T1, T2> objects to the ActionBlock<T> objects.
            joinNetworkAndMemoryResources.LinkTo(networkMemoryAction);
            joinFileAndMemoryResources.LinkTo(fileMemoryAction);

            // Populate the resource pools. In this example, network and 
            // file resources are more abundant than memory resources.
            networkResources.Post(new NetworkResource());
            networkResources.Post(new NetworkResource());
            networkResources.Post(new NetworkResource());

            memoryResource.Post(new MemoryResource());
            memoryResource.Post(new MemoryResource());

            fileResources.Post(new FileResource());
            fileResources.Post(new FileResource());
            fileResources.Post(new FileResource());

            Thread.Sleep(10000);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
