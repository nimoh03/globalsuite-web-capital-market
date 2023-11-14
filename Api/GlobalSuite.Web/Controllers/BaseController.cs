using GlobalSuite.Core.Exceptions;
using Ninject;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Admin.Business;
using BaseUtility.Business;
// using CrystalDecisions.CrystalReports.Engine;
using GL.Business;
using GlobalSuite.Core.Admin;

using GlobalSuite.Web.Filters;
using Serilog;

namespace GlobalSuite.Web.Controllers
{
    [System.Web.Http.Authorize]
    [ApiExceptionFilter]
    public class BaseController:ApiController
    {

        private IAdminService _adminService;

        protected ILogger Logger = Log.Logger;
        public BaseController()
        {
          _adminService=  DependencyResolver.Current.GetService<IAdminService>();
        }
        [Inject]
        public IAdminService AdminService { get => _adminService; set => _adminService = value; }

        protected internal async Task<User> CurrentUser()
        {
            try
            {
                if (!User.Identity.IsAuthenticated) throw new AppUnAuthorizedException();

                var claimIdentity = User.Identity as ClaimsIdentity;
                var nameId = claimIdentity.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId)?.Value;
                if (string.IsNullOrEmpty(nameId)) throw new AppUnAuthorizedException();
                var user = await AdminService.GetUser(nameId);
                GeneralFunc.UserName = nameId;
                return user ?? throw new AppUnAuthorizedException();
            }
            catch (Exception)
            {

                throw new AppUnAuthorizedException();
            }
           
        }

        // internal static FormulaFieldDefinitions SetBaseFormulaForReport(ReportDocument rd, string custId)
        // {
        //     var formFields = rd.DataDefinition.FormulaFields;
        //     var ffLicensee = formFields["Licensee"];
        //     var oAppRead = new AppSettingsReader();
        //     ffLicensee.Text = "'" + oAppRead.GetValue("Licensee", typeof(string)) + "'";
        //
        //     var ffAddress = formFields["Address"];
        //     var oCompany = new Company();
        //     oCompany.GetCompany(GeneralFunc.CompanyNumber);
        //     var strFullAddress = oCompany.Address1 + " " + oCompany.Address2 + " " + oCompany.Town;
        //     ffAddress.Text = "'" + strFullAddress + "'";
        //
        //     var ffImageLocation = formFields["ImageLocation"];
        //     ffImageLocation.Text = "'" + oAppRead.GetValue("LogoLocation", typeof(string)) + "'";
        //
        //     if (string.IsNullOrEmpty(custId)) return formFields;
        //     var ffCustNo = formFields["CustID"];
        //     ffCustNo.Text = "'" + custId + "'";
        //     
        //     var FFBranchName = formFields["BranchName"];
        //     FFBranchName.Text = "'" + GeneralFunc.UserBranchNumber + "'";
        //     
        //     
        //
        //     return formFields;
        // }
    }
}