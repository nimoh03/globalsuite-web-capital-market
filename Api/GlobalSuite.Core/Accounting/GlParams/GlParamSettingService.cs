using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseUtility.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<ResponseResult> EditGlParam(GLParam oGlParam)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var isSuccess = oGlParam.Add();
                    return isSuccess ? ResponseResult.Success() : ResponseResult.Error("Error In Saving");
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message, ex);
                   return ResponseResult.Error("Error Saving. Reason: Invalid Inputs.");
                }
               
            });
        }
        
        
        public async Task<GlParamResponse> GetGlParam()
        {
            return await Task.Run(() =>
            {
                var oGlParam = new GLParam();
                var isSuccess = oGlParam.GetGLParam();
                if (isSuccess)
                {
                    var res = Mapper.Map<GlParamResponse>(oGlParam);

                    if (!string.IsNullOrEmpty(oGlParam.CustomerProduct))
                    {
                        var oProduct = new Product
                        {
                            TransNo = oGlParam.CustomerProduct
                        };
                      var isProduct=  oProduct.GetProduct();
                      if (isProduct)
                          res.CustomerProductDetail = Mapper.Map<ProductResponse>(oProduct);
                    
                    }
                    if (!string.IsNullOrEmpty(oGlParam.VendorProduct))
                    {
                        var oProduct = new Product
                        {
                            TransNo = oGlParam.VendorProduct
                        };
                      var isProduct=  oProduct.GetProduct();
                      if (isProduct)
                          res.VendorProductDetail = Mapper.Map<ProductResponse>(oProduct);
                    
                    }
                    if (!string.IsNullOrEmpty(oGlParam.PettyCashAcct))
                        res.PettyCashAccount=SetAccount(oGlParam.PettyCashAcct);
                     if (!string.IsNullOrEmpty(oGlParam.PayableAcct))
                        res.PayableAccount=SetAccount(oGlParam.PayableAcct);
                     if (!string.IsNullOrEmpty(oGlParam.SalesAcct))
                        res.SalesAccount=SetAccount(oGlParam.SalesAcct); 
                     if (!string.IsNullOrEmpty(oGlParam.ReserveAcct))
                        res.ReserveAccount=SetAccount(oGlParam.ReserveAcct);
                     if (!string.IsNullOrEmpty(oGlParam.CustOpenAcct))
                        res.CustOpenAccount=SetAccount(oGlParam.CustOpenAcct);
                     if (!string.IsNullOrEmpty(oGlParam.FXAssetControlAcct))
                        res.FXAssetControlAccount=SetAccount(oGlParam.FXAssetControlAcct);
                    if (!string.IsNullOrEmpty(oGlParam.GLOpenAcct))
                        res.GLOpenAccount=SetAccount(oGlParam.GLOpenAcct);
                     if (!string.IsNullOrEmpty(oGlParam.COTAcct))
                        res.COTAccount=SetAccount(oGlParam.COTAcct);
                     if (!string.IsNullOrEmpty(oGlParam.VATAcct))
                        res.VATAccount=SetAccount(oGlParam.VATAcct);
                    if (!string.IsNullOrEmpty(oGlParam.SMSChargeAcct))
                        res.SMSChargeAccount=SetAccount(oGlParam.SMSChargeAcct);
                     if (!string.IsNullOrEmpty(oGlParam.SMSAlertIncomeAcct))
                        res.SMSAlertIncomeAccount=SetAccount(oGlParam.SMSAlertIncomeAcct);
                     if (!string.IsNullOrEmpty(oGlParam.RevaluationreserveAcct))
                        res.RevaluationReserveAccount=SetAccount(oGlParam.RevaluationreserveAcct);
                      if (!string.IsNullOrEmpty(oGlParam.IncomeAcctForBankStampDuty))
                        res.IncomeAccountForBankStampDuty=SetAccount(oGlParam.IncomeAcctForBankStampDuty);
                     if (!string.IsNullOrEmpty(oGlParam.SalesAcctIncome))
                        res.SalesAccountIncome=SetAccount(oGlParam.SalesAcctIncome);
                     if (!string.IsNullOrEmpty(oGlParam.PurchaseAcct))
                        res.PurchaseAccount=SetAccount(oGlParam.PurchaseAcct);
                     if (!string.IsNullOrEmpty(oGlParam.PurchaseAcctIncome))
                        res.PurchaseAccountIncome=SetAccount(oGlParam.PurchaseAcctIncome);
                    
                    return res;
                }

                return null;
            });
        }

        private ChartOfAccountResponse SetAccount(string accountId)
        {
            var oAccount = new Account
            {
                AccountId = accountId
            };
            oAccount.GetAccount(GeneralFunc.UserBranchNumber);
            return new ChartOfAccountResponse
            {
                AccountId = oAccount.AccountId,
                AccountName = oAccount.AccountName
            };
        }
    }
}