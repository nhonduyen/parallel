using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Net.Http;
using System.Threading;

namespace DataflowPipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            // Downloads the requested resource as a string.
            var downloadString = new TransformBlock<string, string>(async uri =>
            {
                Console.WriteLine("Downloading {0}...{1} #{2}", uri, DateTime.Now.ToString("HH:mm:ss fff"), Thread.CurrentThread.ManagedThreadId);
                return await new HttpClient().GetStringAsync(uri);
            });
            // Separates the specified text into an array of words.
            var createWordlist = new TransformBlock<string, string[]>(text =>
            {
                Console.WriteLine("Create word list...{0} #{1}", DateTime.Now.ToString("HH:mm:ss fff"), Thread.CurrentThread.ManagedThreadId);
                // Remove common punctuation by replacing all non-letter characters 
                // with a space character.
                char[] token = text.Select(c => char.IsLetter(c) ? c : ' ').ToArray();
                text = new string(token);
                // Separate the text into an array of words.
                return text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            });
            // Removes short words and duplicates.
            var filterWordlist = new TransformBlock<string[], string[]>(words =>
            {
                Console.WriteLine("Filter word list...{0} #{1}", DateTime.Now.ToString("HH:mm:ss fff"), Thread.CurrentThread.ManagedThreadId);
                return words.Where(word => word.Length > 3)
                    .Distinct().ToArray();
            });
            // Finds all words in the specified collection whose reverse also 
            // exists in the collection.
            var findReverseWordlist = new TransformManyBlock<string[], string>(words =>
            {
                Console.WriteLine("Find reversed word list...{0} {1}", DateTime.Now.ToString("HH:mm:ss fff"), Thread.CurrentThread.ManagedThreadId);
                var wordset = new HashSet<string>(words);
                return from word in words.AsParallel()
                       let reversed = new string(word.Reverse().ToArray())
                       where word != reversed && wordset.Contains(reversed)
                       select word;
            });
            // Prints the provided reversed words to the console. 
            var printReversedWords = new ActionBlock<string>(reversed =>
            {
                Console.WriteLine("Found reversed word: {0}/{1} #{2} {3}", reversed, new string(reversed.Reverse().ToArray()),
                    Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToString("HH:mm:ss fff"));
            });
            //
            // Connect the dataflow blocks to form a pipeline.
            //
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            downloadString.LinkTo(createWordlist, linkOptions);
            createWordlist.LinkTo(filterWordlist, linkOptions);
            filterWordlist.LinkTo(findReverseWordlist, linkOptions);
            findReverseWordlist.LinkTo(printReversedWords, linkOptions);

            // Process "The Iliad of Homer" by Homer.
            downloadString.Post("http://www.gutenberg.org/files/6130/6130-0.txt");
            // Mark the head of the pipeline as complete.
            downloadString.Complete();
            // Wait for the last block in the pipeline to process all messages.
            printReversedWords.Completion.Wait();

            Console.WriteLine("Total thread {0}.", System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
