using GlobalSuite.Core.Employees;
using GlobalSuite.Core.Helpers;
using HR.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalSuite.Application.Employees
{
    internal class EmployeeService : BaseService, IEmployeeService
    {
        public async Task<List<EmployeePoco>> GetEmployees()
        {
            var emp = new Employee();

            var employees = await Task.Run(() =>
            {
                var dataSet = emp.GetAll("jdj");

                return dataSet.Tables[0].ToList<EmployeePoco>();
            });
            return employees;
        }
    }
}
