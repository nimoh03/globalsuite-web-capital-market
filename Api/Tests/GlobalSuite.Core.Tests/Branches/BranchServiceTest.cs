using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using GlobalSuite.Core.Admin;

namespace GlobalSuite.Core.Tests.Branches
{
    [TestClass]
    public class BranchServiceTest
    {
        [TestMethod]
        public void Should_Not_Be_Null()
        {
            var service=new AdminService();
            Assert.IsNotNull(service);
        }
        [TestMethod]
        public async Task Should_Return_More_Than_One_Branch()
        {
            var service = new TestBranchService();
            Assert.IsNotNull(service);
            var branches = await service.GetBranches();
            Assert.IsNotNull(branches);
            Assert.AreNotEqual(1, branches.Count);
        }
    }
}
