using System;
using System.Collections;
using System.Collections.Generic;

namespace DataflowBatchBlock
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

       
        public int Insert(Employee em)
        {
            var sql = "INSERT INTO EMPLOYEE(FIRSTNAME, LASTNAME) VALUES(@FIRSTNAME, @LASTNAME);";
            return DBManager<Employee>.Execute(sql, em);
        }
        public int InsertEmployee(Employee[] emps)
        {
            var count = 0;
            foreach (var emp in emps)
            {
                count += Insert(emp);
            }
            return count;
        }
        public int Count()
        {
            var sql = "SELECT COUNT(*) FROM EMPLOYEE;";
            return (int) DBManager<Employee>.ExecuteScalar(sql);
        }

        public int GetEmployeeId(string Firstname, string Lastname)
        {
            var sql = "SELECT EMPLOYEEID FROM EMPLOYEE WHERE FIRSTNAME=@Firstname AND LASTNAME= @Lastname;";
            return (int)DBManager<Employee>.ExecuteScalar(sql, new { Firstname = Firstname, Lastname = Lastname });
        }

        public List<Employee> GetRandomEmployee()
        {
            var sql = "SELECT TOP 1 * FROM EMPLOYEE ORDER BY NEWID();";
            return DBManager<Employee>.ExecuteReader(sql);
        }

        public void Truncate()
        {
            DBManager<Employee>.Execute("TRUNCATE TABLE EMPLOYEE");
        }
    }
}
