using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.Admin
{
    public interface IAdminService
    {
        Task<List<BranchResponse>> GetBranches();
        Task ChangeFullName(string fullName, string emailAddress, string branchId);
        Task<User> FindUser(string username, string password);
        Task<UserProfile> GetProfile();
        Task<User> GetUser(string username);
        Task<List<UserLevel>> GetUserLevels();
        Task<bool> ResetPassword(string username, string password);
        Task<ResponseResult> ChangePassword(string oldPassword, string password);
        Task<bool> Suspend(string username);
        Task<bool> Unlock(string username);
        Task<List<CompanyResponse>> GetAllCompanies();
        Task<DataGeneral.SaveStatus> SaveCompany(Company company);

        /// <summary>
        /// Run End of Month
        /// </summary>
        /// <param name="runDate"></param>
        /// <returns></returns>
        Task<ResponseResult> RunEoM(DateTime runDate);

        /// <summary>
        /// Run Start of Month
        /// </summary>
        /// <param name="runDate"></param>
        /// <returns></returns>
        Task<ResponseResult> RunSoM(DateTime runDate);

        Task<ResponseResult> RunEoY();
        Task<ResponseResult> RunSoY();
        Task<ResponseResult> OpenClosedPeriod(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate);
        Task<ResponseResult> CloseClosedPeriod(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate);
        Task<ResponseResult> OpenClosedPeriodCurrentYear(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate);
        Task<ResponseResult> CloseClosedPeriodCurrentYear(DateTime monthDate, DateTime yearStartDate, DateTime yearEndDate);
        double GetUnitCost(DateTime datPurchaseDate, string strCustomerAcct, string strStockCode);
        string GetProductInvestment(SqlDatabase db);
        Task<List<Country>> GetAllCountries();
        Task<List<Country>> GetAllCountriesOrderByName();
        Task<DataGeneral.SaveStatus> CreateLga(string name, int stateId);
        Task<List<LGA>> GetAllLgas();
        Task<List<LGA>> GetLgaByStateId(int stateId);
        Task<List<Param>> GetAllTitles();
        Task<ResponseResult> CreateTitle(string title);
        Task<ResponseResult> CreateUser(User oUser);
        Task<ResponseResult> CreateBank(string name);
        Task<List<Param>> GetAllBanks();
        Task<ResponseResult> CreateReligion(string name);
        Task<List<Param>> GetAllReligions();
        Task<ResponseResult> CreateOccupation(string name);
        Task<List<Param>> GetAllOccupations();
        Task<ResponseResult> CreateGeoZones(string name);
        Task<List<Param>> GetAllGeoZones();
        Task<List<User>> GetAllUsers(UserFilter filter);
    }
}