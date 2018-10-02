using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PrecomputedTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] urls = new string[] { 
            "http://msdn.microsoft.com",
            "http://www.contoso.com",
            "http://www.microsoft.com"
            };
            Task[] tasks = new Task[urls.Length];
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var download = from url in urls
                           select CachedDownloads.DownloadStringAsync(url);
            Action myaction = () => CachedDownloads.DownloadStringAsync(urls[0]);
            Task.WhenAll(download).ContinueWith(results =>
            {
                stopwatch.Stop();
                Console.WriteLine("Retrieve {0} character in {1} ms.",
                    results.Result.Sum(result => result.Length), stopwatch.ElapsedMilliseconds);
            }).Wait();
            stopwatch.Restart();
            var download1 = from url in urls
                       select CachedDownloads.DownloadStringAsync(url);
            Task.WhenAll(download1).ContinueWith(results =>
            {
                stopwatch.Stop();
                Console.WriteLine("Retrieve from cache {0} character in {1} ms.",
                    results.Result.Sum(result => result.Length), stopwatch.ElapsedMilliseconds);
            }).Wait();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
