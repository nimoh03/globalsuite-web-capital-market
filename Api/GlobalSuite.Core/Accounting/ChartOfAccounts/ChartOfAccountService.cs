using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<ResponseResult> ChartOfAccount(bool isInternal,Account oAccount)
        {
            return await Task.Run(() =>
            {
                oAccount.TransNo = GetTransNo(oAccount.AccountId);
                if (oAccount.AccountLevel > 0)
                {
                    var oAccountParent = new Account
                    {
                        AccountId = oAccount.ParentId
                    };
                    if (oAccountParent.GetAccount(oAccount.Branch))
                    {
                        oAccount.AccountType = oAccountParent.AccountType;
                        oAccount.Branch = oAccountParent.Branch;
                        if (oAccountParent.InternalAccount.Trim() == "Y")
                        {
                            oAccount.InternalAccount = "Y";
                            oAccount.IsParent = "N";
                        }
                        else
                        {
                            oAccount.InternalAccount = "N";
                            oAccount.IsParent = "Y";
                        }
                    }else
                    return ResponseResult.Error("Parent Account Does Not Exist");
                }
                else
                {
                    oAccount.ParentId = oAccount.AccountId;
                    if (isInternal)
                    {
                        oAccount.InternalAccount = "Y";
                        oAccount.IsParent = "N"; 
                    }
                    else
                    {
                        oAccount.InternalAccount = "N";
                        oAccount.IsParent = "Y";
                        var oBranch = new Branch();
                        oAccount.Branch = oBranch.DefaultBranchCustomer;
                    }
                }
                oAccount.PreviousYearCreditDebitAnnual = "N";
                oAccount.SaveType = Constants.SaveType.ADDS;
                var oSaveStatus = oAccount.Save();
                switch (oSaveStatus)
                {
                    case DataGeneral.SaveStatus.Saved:
                        return ResponseResult.Success(oAccount.SavedPartial.Trim() == "Y" ?
                            "Saved Successfully! But Cannot Overwrite Mandatory Details. GL Transactions Exist Or Account Is A Parent Account" :
                            "Account Created Successfully");
                    case DataGeneral.SaveStatus.NotExist:
                        return ResponseResult.Error( "Cannot Edit Account Does Not Exist");
                    case DataGeneral.SaveStatus.NameExistAdd:
                        return ResponseResult.Error("Error In Adding New Account,Account Name Already Exist.");
                    case DataGeneral.SaveStatus.NameExistEdit:
                        return ResponseResult.Error("Error In Editing Account,Account Name Already Exist.");
                    case DataGeneral.SaveStatus.AccountIdExistAdd:
                        return ResponseResult.Error( "Error In Adding New Account,Account Code Already Exist.");
                    case DataGeneral.SaveStatus.AccountIdExistEdit:
                        return ResponseResult.Error("Error In Editing Account,Account Code Already Exist.");
                    case DataGeneral.SaveStatus.IsCustomerAccount:
                        return ResponseResult.Error( "Error In Saving Account,Cannot Attach An Account To Customer Parent Account");
                    case DataGeneral.SaveStatus.DuplicateRef:
                        return ResponseResult.Error( "Error In Saving Account,Parent Account Have Existing GL Transactions");
                    default:
                        return ResponseResult.Error();
                }
            });
        }

        private string GetTransNo(string accountId)
        {
            if (accountId.IndexOf('-') > 0)
                return accountId.Split('-')[0];
            return $"{new Random().Next(12980, 100000000)}";
        }

        public async Task<List<IfrsAnnualResponse>> GetAllIncomeStateAnnual()
        {
            return await Task.Run( () =>
            {
                var ifsAnnual = new IFRSAnnual
                {
                    ReportType = "INCOME"
                };
                var ds = ifsAnnual.GetAll();
                return ds.Tables[0].ToList<IfrsAnnualResponse>();
            });
        }  
        public async Task<List<IfrsAnnualResponse>> GetAllSocf()
        {
            return await Task.Run( () =>
            {
                var ifsAnnual = new IFRSAnnual
                {
                    ReportType = "SOCF"
                };
                var ds = ifsAnnual.GetAll();
                return ds.Tables[0].ToList<IfrsAnnualResponse>();
            });
        }   
        public async Task<List<IfrsAnnualResponse>> GetAllSofp()
        {
            return await Task.Run( () =>
            {
                var ifsAnnual = new IFRSAnnual
                {
                    ReportType = "SOFP"
                };
                var ds = ifsAnnual.GetAll();
                return ds.Tables[0].ToList<IfrsAnnualResponse>();
            });
        }  
        public async Task<List<IfrsAnnualResponse>> GetAllSocie()
        {
            return await Task.Run( () =>
            {
                var ifsAnnual = new IFRSAnnual
                {
                    ReportType = "SOCIE"
                };
                var ds = ifsAnnual.GetAll();
                return ds.Tables[0].ToList<IfrsAnnualResponse>();
            });
        }  
        public async Task<List<AccountLevelResponse>> GetAllAccountLevels()
        {
            return await Task.Run( () =>
            {
                var oAccount = new Account();
                var ds = oAccount.GetAllAcctLevel();

                return ds.Tables[0].ToList<AccountLevelResponse>();
            });
        } 
        public async Task<List<AccountTypeResponse>> GetAllAccountTypes()
        {
            return await Task.Run( () =>
            {
                var oAccount = new Account();
                var ds = oAccount.GetAllAcctType();

                return ds.Tables[0].ToList<AccountTypeResponse>();
            });
        }

        public async Task<List<AccountResponse>> GetParentByLevel(int level, string branchCode = null)
        {
            return await Task.Run( async () =>
            {
                var branch = branchCode ?? GeneralFunc.UserBranchNumber;
                var oAccount = new Account
                {
                    AccountLevel = level,
                    Branch = branch
                };
                var ds = oAccount.GetInternalAccountGivenLevelBranch();
                var oList = new List<AccountResponse>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    oList.Add(await FormatAccount(row));
                }

                return oList;//ds.Tables[0].ToList<AccountResponse>();
            });
        }

        public async Task<List<AccountResponse>> GetChartOfAccounts()
        {
            return await Task.Run(async () =>
            {
                var oAccount = new Account();
                var ds=  oAccount.GetAll();
                var oList = new List<AccountResponse>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    oList.Add(await FormatAccount(row));
                }

                return oList;
            });
        }

        private async Task<AccountResponse> FormatAccount(DataRow row, bool showDetail=false)
        {
            var res = row.GetItem<AccountResponse>();
            res.TransNo = $"{row["TransNo"]}";
            res.AccountName = $"{row["Description"]}";
            res.AccountType = $"{row["AcctType"]}";
            res.ParentId = $"{row["ParentID"]}";
            res.BankAccount = $"{row["Bank"]}";
            res.PettyCashAccount = $"{row["PettyCash"]}";
            res.IsInternal = $"{row["InternalAccount"]}" == "Y";
            res.IsParent = $"{row["IsParent"]}" == "Y";
            if (!showDetail) return res;
            if(!string.IsNullOrEmpty(res.ParentId))
                res.Parent = await GetChartOfAccount(res.ParentId);
            if(!string.IsNullOrEmpty(res.BranchId))
            {
                var b = new Branch
                {
                    TransNo = res.BranchId
                };
                b.GetBranch();
                res.BranchDetail = Mapper.Map<BranchResponse>(b);
            }

            return res;
        }

        public async Task<List<ChartOfAccountResponse>> GetAllChildAccount(string branchCode)
        {
            return await Task.Run(() =>
            {
                var oAccount = new Account{Branch = branchCode};
                var ds=  oAccount.GetAllChildAccountByBranch();

                return (from DataRow row in ds.Tables[0].Rows 
                    select new ChartOfAccountResponse { AccountId = $"{row["AccountID"]}", AccountName = $"{row["AccountDetail"]}" }).ToList();
            });
        }
        public async Task<ResponseResult> DeleteChartOfAccount(string code, string accountId, string branchCode)
        {
          return  await Task.Run(() =>
          {
              var oAccount = new Account
              {
                  AccountId = accountId.Trim(),
                  TransNo = code.Trim(),
                  Branch = branchCode.Trim()
              };
              if (!oAccount.GetAccount(branchCode)) return ResponseResult.Error("Account cannot be deleted.");
              oAccount.SaveType = Constants.SaveType.DELETE;
              var oSaveStatus = oAccount.Delete();
              switch (oSaveStatus)
              {
                  case DataGeneral.SaveStatus.Saved:
                      return ResponseResult.Success();
                  case DataGeneral.SaveStatus.NotExist:
                      return ResponseResult.Error("Cannot Delete Account Does Not Exist");
                  case DataGeneral.SaveStatus.NotSaved:
                      return ResponseResult.Error(
                          "Error In Deleting Account,GL Transaction Exist Or Child Account(s) Exist For This Account");
                  default:
                      return ResponseResult.Error("Account cannot be deleted.");
              }
          });

        }
        public async Task<AccountResponse> GetChartOfAccount( string accountId)
        {
            return await Task.Run(() =>
            {
                var oAccount = new Account
                {
                    AccountId = accountId.Trim()
                };
                var isSuccess = oAccount.GetAccountWithoutBranch();
                return isSuccess ? Mapper.Map<AccountResponse>(oAccount) : null;
            });
        }
    }
}