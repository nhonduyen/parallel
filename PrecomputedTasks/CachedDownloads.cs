using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Net;
using System;

namespace PrecomputedTasks
{
    public class CachedDownloads
    {
        static ConcurrentDictionary<string, string> cachedDownload = new ConcurrentDictionary<string, string>();
        public static Task<string> DownloadStringAsync(string url)
        {
            string content;
            if (cachedDownload.TryGetValue(url, out content))
                return Task.FromResult<string>(content);
            return Task.Run(async () =>
                {
                    Console.WriteLine("Visiting {0}.", url);
                    content = await new WebClient().DownloadStringTaskAsync(url);
                    cachedDownload.TryAdd(url, content);
                    return content;
                });
        }
    }
}
