using System;
using System.Threading.Tasks;

namespace ChainingTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            var task1 = Task.Run(() => {
                Console.WriteLine("Time {0}", DateTime.Now);
                return DateTime.Now;
            });

            var task2 = task1.ContinueWith(now => {
                if (now.Result.IsDaylightSavingTime())
                {
                    Console.WriteLine("{0} is daylight", now.Result);
                }
                else
                {
                    Console.WriteLine("{0} is night", now.Result);
                }
            });
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
