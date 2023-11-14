using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using BaseUtility.Business.Extensions;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<ResponseResult> CreateSelfBalance(SelfBal oSelfBal)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return AddOrEditSelfBalance(oSelfBal, Constants.SaveType.ADDS);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
                return ResponseResult.Error("Error In Saving");
            });
        }

         public async Task<ResponseResult> EditSelfBalance(SelfBal oSelfBal)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return AddOrEditSelfBalance(oSelfBal, Constants.SaveType.EDIT);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
                return ResponseResult.Error("Error In Saving");
            });
        }

        private ResponseResult AddOrEditSelfBalance(SelfBal oSelfBal, string saveMode)
        {
            oSelfBal.Posted = "N";
            oSelfBal.Reversed = "N";
            oSelfBal.MainSub = string.Empty;
            oSelfBal.ConSub = string.Empty;
            oSelfBal.TransNoRev = string.Empty;
            oSelfBal.MainTransType = string.Empty;
            oSelfBal.ChqNo = string.Empty;
            oSelfBal.VDate=DateTime.Now;
            oSelfBal.SaveType = saveMode;
            var oSaveStatus = oSelfBal.Save();
            switch (oSaveStatus)
            {
                case DataGeneral.SaveStatus.Saved:
                    return ResponseResult.Success();
                case DataGeneral.SaveStatus.HeadOfficeExistEdit:
                    return ResponseResult.Error("Cannot Save,Duplicate Cheque Number");
                default:
                    return ResponseResult.Error("Error saving.");
            }
        }

        public async Task<ResponseResult> DeleteSelfBalance(string code)
        {
          return  await Task.Run(() =>
            {
                var oSelfBal = new SelfBal
                {
                    TransNo = code
                };
                if (oSelfBal.GetSelfBal(DataGeneral.PostStatus.UnPosted))
                {

                    if (!oSelfBal.Delete())
                        return ResponseResult.Error( "Error In Deleting Self Balancing Posting");
                }
                else
                    return ResponseResult.Error("Self Balancing Posting Does Not Exist");
                return ResponseResult.Success();
            });

        }

        public async Task<ResponseResult> PostSelfBalance( string code)
        {
            return await Task.Run(() =>
            {
                var oGl = new AcctGL();
                var oSelfBal = new SelfBal
                {
                    TransNo = code
                };
                if (!oSelfBal.GetSelfBal(DataGeneral.PostStatus.UnPosted))
                    return ResponseResult.Error("Cannot Post! This Customer Payment Does Not Exist or Already Posted/Reversed,Try Saving");
                oGl.EffectiveDate = oSelfBal.VDate;

                var oAccount = new Account();
                if (oSelfBal.MainTransType == "CMA")
                {
                    if (!oAccount.ChkAccountIsCustomerAcct(oSelfBal.ConAccountID))
                    {
                        oGl.AccountID = "";
                        oGl.MasterID = oSelfBal.ConAccountID;
                        oGl.AcctRef = "";
                    }
                    else
                    {
                        oGl.AccountID = oSelfBal.ConSub;
                        oGl.MasterID = oSelfBal.ConAccountID;
                    }
                    if (!oAccount.ChkAccountIsCustomerAcct(oSelfBal.MainAcctID))
                    {
                        oGl.RecAcct = "";
                        oGl.RecAcctMaster = oSelfBal.MainAcctID;
                        oGl.AcctRefSecond = "";
                    }
                    else
                    {
                        oGl.RecAcct = oSelfBal.MainSub;
                        oGl.RecAcctMaster = oSelfBal.MainAcctID;
                        oGl.AcctRefSecond = "";
                    }
                }
                else
                {
                    if (!oAccount.ChkAccountIsCustomerAcct(oSelfBal.MainAcctID))
                    {
                        oGl.AccountID = "";
                        oGl.MasterID = oSelfBal.MainAcctID;
                        oGl.AcctRef = "";
                    }
                    else
                    {
                        oGl.AccountID = oSelfBal.MainSub;
                        oGl.MasterID = oSelfBal.MainAcctID;
                        oGl.AcctRef = "";
                    }

                    if (!oAccount.ChkAccountIsCustomerAcct(oSelfBal.ConAccountID))
                    {
                        oGl.RecAcct = "";
                        oGl.RecAcctMaster = oSelfBal.ConAccountID;
                        oGl.AcctRefSecond = "";
                    }
                    else
                    {
                        oGl.RecAcct = oSelfBal.ConSub;
                        oGl.RecAcctMaster = oSelfBal.ConAccountID;
                        oGl.AcctRefSecond = "";
                    }
                }
                oGl.Credit = oSelfBal.Amount;
                if (oSelfBal.Ref.Trim() != "")
                {
                    oGl.Desciption = oSelfBal.Description + " PVNo: " + oSelfBal.Ref.Trim();
                }
                else
                {
                    oGl.Desciption = oSelfBal.Description;
                }
                oGl.TransType = "GLSELF";
                oGl.SysRef = "SBL" + "-" + oSelfBal.TransNo;
                oGl.Ref01 = oSelfBal.TransNo;
                oGl.Chqno = oSelfBal.Ref;
                oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGl.Branch = oAccount.GetAccountBranch(oSelfBal.MainAcctID);
                oGl.Reverse = "N";
                oGl.FeeType = ""; oGl.Ref02 = "";
                return oGl.Post("SELFBAL") ? ResponseResult.Success("Customer Payment Posted Successfully!") : ResponseResult.Error();
            });
        }

        public async Task<List<SelfBalanceResponse>> GetSelfBalances(StatusFilter filter)
        {
            return await Task.Run(() =>
            {
                var oSelfBal = new SelfBal();
                filter = filter ?? new StatusFilter();
                if (filter.Status.ToLower() == "all") filter.Status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(filter.Status, true, out DataGeneral.PostStatus postStatus);
                DataSet ds;
                if (filter.StartDate.HasValue && filter.EndDate.HasValue&& postStatus!=DataGeneral.PostStatus.All)
                {
                    oSelfBal.VDate = filter.StartDate.Value.ToExact();
                    oSelfBal.VDate = filter.EndDate.Value.ToExact();
                    ds = oSelfBal.GetAllGivenEntryDate(postStatus);
                }
                else
                {
                    ds=   oSelfBal.GetAll(postStatus);
                }
                var dt = ds.Tables[0].AsEnumerable().Skip(filter.Skip).Take(filter.PageSize);
                var oList = new List<SelfBalanceResponse>();
                foreach (var row in dt)
                {
                    var self = row.GetItem<SelfBalanceResponse>();
                    self.Posted = $"{row["Posted"]}" == "Y";
                    self.Reversed = $"{row["Reversed"]}" == "Y";
                    oList.Add(FormatSelfBalance(self));
                }

                return oList;// dt.Select(row => FormatSelfBalance(Mapper.Map<SelfBalanceResponse>(row.GetItem<SelfBal>()))).ToList();
            });
        }

        public async Task<SelfBalanceResponse> GetSelfBalance(string code, DataGeneral.PostStatus status)
        {
            return await Task.Run(() =>
            {
                var oSelfBal = new SelfBal
                {
                    TransNo = code.Trim()
                };
                var isSuccess = oSelfBal.GetSelfBal(status);
                if (!isSuccess) return null;
                var s = Mapper.Map<SelfBalanceResponse>(oSelfBal);
                return FormatSelfBalance(s);
            });
        }
        
        
        private SelfBalanceResponse FormatSelfBalance(SelfBalanceResponse selfBalance)
        {
            var branchNo = !string.IsNullOrEmpty(selfBalance.Branch) ? selfBalance.Branch : GeneralFunc.UserBranchNumber;
            var oProduct = new Account()
            {
                AccountId = selfBalance.MainAcctID
            };
            oProduct.GetAccount(branchNo);
            selfBalance.MainAcct = Mapper.Map<ChartOfAccountResponse>(oProduct);
            var oConAcct = new Account
            {
                AccountId = selfBalance.ConAcctID,
            };
            oConAcct.GetAccount(branchNo);
            selfBalance.ConAcct = Mapper.Map<ChartOfAccountResponse>(oConAcct);
            if (!string.IsNullOrEmpty(selfBalance.Branch))
            {
                var oBranch = new Branch
                            {
                                TransNo = branchNo
                            };
                            oBranch.GetBranch();
                            selfBalance.BranchDetail = Mapper.Map<BranchResponse>(oBranch);
            }
            
            Enum.TryParse(selfBalance.InstrumentType, true, out DataGeneral.GLInstrumentType instrumentType);
            selfBalance.InstrumentType = EnumExtensions.GetDescription(instrumentType);
            return selfBalance;
        }
    }
}