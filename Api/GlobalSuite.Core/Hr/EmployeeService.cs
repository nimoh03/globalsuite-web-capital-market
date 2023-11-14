using System.Collections.Generic;
using System.Threading.Tasks;
using BaseUtility.Business;
using GlobalSuite.Core.Helpers;
using HR.Business;

namespace GlobalSuite.Core.Hr
{
    public partial class HrService 
    {
        public async Task<List<Employee>> GetEmployees()
        {
            var emp = new Employee();

            var employees = await Task.Run(() =>
            {
                var dataSet = emp.GetAll("jdj");

                return dataSet.Tables[0].ToList<Employee>();
            });
            return employees;
        }

        public async Task<ResponseResult> CreateEmployee(Employee employee)
        {
            var result = Validate(employee);
            if (!result.IsSuccess) return result;

          return  await Task.Run(() =>
            {
                var oBucket = new Bucket();
                employee.TransNo = oBucket.GetNextBucketNoNonIdentity("EMPLOYEE").PadLeft(5, char.Parse("0"));
                employee.SaveType = Constants.SaveType.ADDS;
                var saveStatus = employee.Save();
                if (saveStatus == DataGeneral.SaveStatus.ProductNotExist)
                    return ResponseResult.Error("Error Saving Employee,Pay Item Rate Not Set For This Job Grade/Step");
                return saveStatus==DataGeneral.SaveStatus.Saved ? ResponseResult.Success() : ResponseResult.Error(saveStatus.ToString());
            });
        }

        private ResponseResult Validate(Employee employee)
        {
            if (employee.Occupation > 0 && string.IsNullOrEmpty(employee.OccupLevel))
                return ResponseResult.Error("Both Employee Grade And Step Must Be Selected");
            return ResponseResult.Success();
        }
    }
}
