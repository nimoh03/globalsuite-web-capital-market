using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalSuite.Core.Tests.Employees
{
    [TestClass]
    public class EmployeeServiceTest
    {
        [TestMethod]
        public async Task Should_Return_3_Employees()
        {
            var service = new TestEmployeeService();
            var employees = await service.GetEmployees();

            Assert.IsNotNull(employees);
            Assert.AreEqual(employees.Count, 3);
        }
        
        [TestMethod]
        public async Task Should_Return_Employees()
        {
            var service = new TestEmployeeService();
            var employees = await service.GetEmployees();
            Assert.IsNotNull(employees);
            var first=employees.FirstOrDefault(x=>x.FirstName=="Molo");
            Assert.AreEqual(first.FirstName,"Molo");
        }
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public async Task Should_Throw_Not_Implemented_Exception()
        {
            var service = new TestEmployeeService();
             await service.Create(new HR.Business.Employee());
            
        }
    }
}
