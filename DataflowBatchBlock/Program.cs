using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Diagnostics;
using System.IO;

namespace DataflowBatchBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            int insertCount = 256;
            int batchSize = 96;

            Employee emp = new Employee();
            Console.WriteLine("Table employee size: {0}", emp.Count());
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Insert");
            AddEmployees(insertCount);

            stopwatch.Stop();
            Console.WriteLine("New size of table {0}, insert time {1} ms", emp.Count(), stopwatch.ElapsedMilliseconds);
            emp.Truncate();
            Console.WriteLine("Clean table");

            stopwatch.Start();
            Console.WriteLine("Insert with batch");
            AddEmployeesBatch(batchSize, insertCount);

            stopwatch.Stop();
            Console.WriteLine("New size of table {0}, insert time {1} ms", emp.Count(), stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Batch join database select operation");
            GetRandomEmployee(batchSize, insertCount);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        public static void PostRandomEmployees(ITargetBlock<Employee> target, int count)
        {
            Console.WriteLine("Adding {0} employees.", count);
            for (int i = 0; i < count; i++)
            {
                target.Post(RandomEmp());
            }
        }
        public static void AddEmployees(int count)
        {
            Employee em = new Employee();
            var insertEmployee = new ActionBlock<Employee>(e => em.InsertEmployee(new Employee[] { e }));
            PostRandomEmployees(insertEmployee, count);
            insertEmployee.Complete();
            insertEmployee.Completion.Wait();
        }

        public static void AddEmployeesBatch(int batchSize, int count)
        {
            Employee em = new Employee();
            var batchEmployees = new BatchBlock<Employee>(batchSize);

            var insertEmployee = new ActionBlock<Employee[]>(e => em.InsertEmployee(e));
            batchEmployees.LinkTo(insertEmployee);

            batchEmployees.Completion.ContinueWith(delegate { insertEmployee.Complete(); });
            PostRandomEmployees(batchEmployees, count);
            batchEmployees.Complete();
            batchEmployees.Completion.Wait();
        }
        public static void GetRandomEmployee(int batchSize, int count)
        {
            Employee em = new Employee();
            var selectEmployees = new BatchedJoinBlock<Employee, Exception>(batchSize);

            int totalException = 0;

            var printEmployees = new ActionBlock<Tuple<IList<Employee>, IList<Exception>>>(data =>
            {
                Console.WriteLine("Recieve a batch");
                foreach (var emp in data.Item1)
                {
                    Console.WriteLine("{0} {1} ID={2}", emp.FirstName, emp.LastName, emp.EmployeeId);
                }
                Console.WriteLine("There was {0} errors in this batch", data.Item2.Count);
                totalException += data.Item2.Count;
            });
            selectEmployees.LinkTo(printEmployees);
            selectEmployees.Completion.ContinueWith(delegate { printEmployees.Complete(); });
            Console.WriteLine("Select random employee");
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Employee ranEmp = em.GetRandomEmployee().FirstOrDefault();
                    selectEmployees.Target1.Post(ranEmp);
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine("Null Exception {0}", ex.InnerException);
                    selectEmployees.Target2.Post(ex);
                }
            }
            selectEmployees.Complete();
            printEmployees.Completion.Wait();
            Console.WriteLine("Finished get employee. There was {0} error", totalException);
        }

        public static Employee RandomEmp()
        {
            return new Employee { 
            EmployeeId=-1,
            FirstName = Path.GetRandomFileName().Replace(".", ""),
            LastName = Path.GetRandomFileName().Replace(".", "")
            };
        }
    }
}
