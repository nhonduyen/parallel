using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System;

namespace DataflowBlockRecieveData
{
    public class DataflowExecutionBlock
    {
        public static async Task<int> CountBytesAsync(string path)
        {
            var buffer = new byte[1024];
            var totalZeroBytes = 0;

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, true))
            {
                var byteRead = 0;
                do
                {
                    Console.WriteLine("Read File {0} #{1}", path, Thread.CurrentThread.ManagedThreadId);
                    byteRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                    totalZeroBytes += buffer.Count(b => b == 0);
                }
                while (byteRead > 0);
            }
            return totalZeroBytes;
        }

        public static async Task WriteFileAsync(string path)
        {

            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Write, 0x1000, true))
            {
                var ran = new System.Random();
                var buffer = new byte[1024];
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("Write File {0} #{1}", buffer.Length, Thread.CurrentThread.ManagedThreadId);
                    ran.NextBytes(buffer);
                    await fileStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
