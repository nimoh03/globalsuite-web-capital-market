using GlobalSuite.Core.Employees;
using GlobalSuite.Core.Helpers;
using HR.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSuite.Core.Tests.Employees
{
    internal class TestEmployeeService 
    {
        public Task<ResponseResult> Create(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<List<Employee>> GetEmployees()
        {
            return Task.FromResult(new List<Employee>
            {
                new Employee{FirstName="Molo"},
                new Employee{FirstName="Ayodeji"},
                new Employee{FirstName="Stephen"},
            });
        }
    }
}
