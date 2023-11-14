using System;
using System.Threading.Tasks;
using BaseUtility.Business;
using CapitalMarket.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService 
    {
        public async Task<ResponseResult> CreateJobOrder(string customerId,JobOrder oJobOrder)
        {
            var oStkbPGenTable = new StkParam();
            var oAcctGl = new AcctGL();
            var oProductAcct = new ProductAcct
            {
                CustAID = customerId,
                ProductCode = oStkbPGenTable.Product
            };

            oJobOrder.CustNo = customerId;
            oAcctGl.AcctRef = "ALL";
            oAcctGl.AccountID = customerId;
            oJobOrder.CustBalance = oAcctGl.GetAccountBalanceByCustomer();
            oJobOrder.CustCreditLimit = 0;
            oJobOrder.Posted = "N";
            oJobOrder.Reversed = "N";
            oJobOrder.JB_ID = "0";
            oJobOrder.InputFrom = "";
          return  await Task.Run(() =>
          {
              if (oProductAcct.AcctDeactivation.Trim() == "Y")
              {
                  return ResponseResult.Error("Cannot Save! Customer Account Deactivated");
              }

              if (string.IsNullOrEmpty(oJobOrder.Code))
                  return ResponseResult.Error("Job order code not set.");
              oJobOrder.SaveType = Constants.SaveType.ADDS;
             var oSaveStatus = oJobOrder.Save(); 
             
             if (oSaveStatus == JobOrder.SaveStatus.NotExist)
                return ResponseResult.Error("Cannot Edit, Job Order Does Not Exist");
             if (oSaveStatus == JobOrder.SaveStatus.PurchaseEmptyTotalAmount)
                return ResponseResult.Error("Cannot Job Purchase Order Unit Price Must Be Greater Than Zero To Calculate Cost Of Purchase");
             if (oSaveStatus == JobOrder.SaveStatus.PurchaseNotEnoughFund)
                 return ResponseResult.Error( "Cannot Job Purchase Order Customer Does Not Have Enough Funds For This Purchase Order");
             if (oSaveStatus == JobOrder.SaveStatus.SaleNotEnoughStock)
                 return ResponseResult.Error(oJobOrder.OutMessage.Trim());
             if (oSaveStatus == JobOrder.SaveStatus.KYCMissingDocument)
                 return ResponseResult.Error( oJobOrder.KYCMissingMessage.Trim());
             if (oSaveStatus == JobOrder.SaveStatus.ProductStockBrokingOrNASD)
                 return ResponseResult.Error("Cannot Job Order Customer Account Must Be StockBroking NSE Or NASD Account");
              if (oSaveStatus == JobOrder.SaveStatus.StockNotForNSEProduct)
                  return ResponseResult.Error("Cannot Job Order Stocks Not On Equity(NSE) Board");
             if (oSaveStatus == JobOrder.SaveStatus.StockNotForNASDProduct)
                  return ResponseResult.Error("Cannot Job Order Stocks Not On OTC/NASD Board");
              if (oSaveStatus == JobOrder.SaveStatus.SaleCannotModifyCertJobOrder)
                  return ResponseResult.Error("Cannot Edit Sale Order, It is a Certificate Verification Order Try Reversing The Certificate Verification");
              if (oSaveStatus == JobOrder.SaveStatus.TransactionSavedExist)
                  return ResponseResult.Error("Cannot Save Job Order, The Same Transaction Details Exist In The Saved Transaction Number "
                                              + oJobOrder.TransNoReturn.Trim());
             if (oSaveStatus == JobOrder.SaveStatus.TransactionPostedExist)
                 return ResponseResult.Error( "Cannot Save Job Order, The Same Transaction Details Exist In The Posted Transaction Number " 
                                              + oJobOrder.TransNoReturn.Trim());
              return ResponseResult.Success();
          });
        }

        public async Task<JobOrder> GetJobOrder()
        {
            var oJobOrder = new JobOrder();
           return  await Task.Run(() =>
             {
                 var available= oJobOrder.GetJobOrder(DataGeneral.PostStatus.UnPosted);
                 return available ? oJobOrder : null;
             });
             
        }

        public async Task<ResponseResult> PostJobOrder(string code, DateTime effectiveDate)
        {
            var oJobOrder = new JobOrder
            {
                Code = code, EffectiveDate = effectiveDate
            };
            return await Task.Run(() => oJobOrder.Post()
                ? ResponseResult.Success()
                : ResponseResult.Error("Job Order Does Not Exist Or Reversed"));
        }
    }
}