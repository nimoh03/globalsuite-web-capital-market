using System.Collections.Generic;
using System.Threading.Tasks;
using GlobalSuite.Core.Helpers;
using HR.Business;

namespace GlobalSuite.Core.Hr
{
    public interface IHrService
    {
        Task<List<Employee>> GetEmployees();
        Task<ResponseResult> CreateEmployee(Employee employee);
    }

   public partial class HrService : IHrService
    {
    }
}