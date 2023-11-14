using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Web.Controllers.Accounting.Dto;

namespace GlobalSuite.Web.Controllers.Accounting
{
    public partial class AccountingController:BaseController
    {
        /// <summary>
        /// Get All Chart of Accounts
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount)]
        [ResponseType(typeof(List<AccountResponse>))]
        public async Task<IHttpActionResult> GetAll()
        {
                var account = await _accountingService.GetChartOfAccounts();
                var res = _mapper.Map<List<AccountResponse>>(account);
                return Ok(res);
        }   
        /// <summary>
        /// Get Chart of Accounts
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/{accountId}")]
        [ResponseType(typeof(AccountResponse))]
        public async Task<IHttpActionResult> Get(string accountId)
        {
                var account = await _accountingService.GetChartOfAccount(accountId);
                if (account == null) return NotFound();
                var res = _mapper.Map<AccountResponse>(account);
                return Ok(res);
        }  
        /// <summary>
        /// Get All Children Chart of Accounts
        /// </summary>
        /// <paramref name="branchCode"/>
        /// <returns></returns>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/children")]
        [ResponseType(typeof(List<ChartOfAccountResponse>))]
        public async Task<IHttpActionResult> GetChildren(string branchCode)
        {
                var accounts = await _accountingService.GetAllChildAccount(branchCode);
                
                return Ok(accounts);
        } 
        /// <summary>
        /// Get All Chart of Accounts
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/parent/{level}/{branchCode?}")]
        [ResponseType(typeof(List<AccountResponse>))]
        public async Task<IHttpActionResult> GetParents(int level=0, string branchCode=null)
        {
                var accounts = await _accountingService.GetParentByLevel(level);
                var res = _mapper.Map<List<AccountResponse>>(accounts);
                return Ok(res);
        }   
        
        /// <summary>
        /// Get All Account Levels
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/levels")]
        [ResponseType(typeof(List<AccountLevelResponse>))]
        public async Task<IHttpActionResult> GetAccountLevels()
        {
                var accounts = await _accountingService.GetAllAccountLevels();
                
                return Ok(accounts);
        }   
        /// <summary>
        /// Get All Account Types
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/types")]
        [ResponseType(typeof(List<AccountTypeResponse>))]
        public async Task<IHttpActionResult> GetAccountTypes()
        {
                var accounts = await _accountingService.GetAllAccountTypes();
                
                return Ok(accounts);
        }  
        /// <summary>
        /// Delete Chart of Accounts
        /// </summary>
        /// <param name="code"></param>
        /// <param name="accountId"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpDelete, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount)]
        public async Task<IHttpActionResult> Remove(string code, string accountId, string branchCode)
        {
                var result = await _accountingService.DeleteChartOfAccount(code, accountId, branchCode);
                if (!result.IsSuccess) return BadRequest(result.ToString());
                return Ok();
        } 
        /// <summary>
        /// Create Chart of Accounts
        /// </summary>
        [HttpPost, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount)]
        public async Task<IHttpActionResult> CreateChartOfAccount([FromBody] AccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var oAccount = _mapper.Map<Account>(request);
            var result = await _accountingService.ChartOfAccount(request.IsInternal, oAccount);
            if (!result.IsSuccess) return BadRequest(result.ToString());
            return Ok();
        } 
         
        
        /// <summary>
        /// Get All IFRS Income State Annual
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/ifrs/income-statement")]
        [ResponseType(typeof(List<IfrsAnnualResponse>))]
        public async Task<IHttpActionResult> GetIfrsIncome()=>Ok(await _accountingService.GetAllIncomeStateAnnual());
  
        
        /// <summary>
        /// Get All IFRS SOCF
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/ifrs/socf")]
        [ResponseType(typeof(List<IfrsAnnualResponse>))]
        public async Task<IHttpActionResult> GetIfrsSocf()=>Ok(await _accountingService.GetAllSocf());
       
        /// <summary>
        /// Get All IFRS SOFP
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/ifrs/sofp")]
        [ResponseType(typeof(List<IfrsAnnualResponse>))]
        public async Task<IHttpActionResult> GetIfrsSofp()=>Ok(await _accountingService.GetAllSofp());
    
        /// <summary>
        /// Get All IFRS SOCIE
        /// </summary>
        [HttpGet, Route(GlobalSuiteRoutes.AccountingRoutes.MaintainRoutes.ChartOfAccount+"/ifrs/socie")]
        [ResponseType(typeof(List<IfrsAnnualResponse>))]
        public async Task<IHttpActionResult> GetIfrsSocie()=>Ok(await _accountingService.GetAllSocie());
      
    }
}