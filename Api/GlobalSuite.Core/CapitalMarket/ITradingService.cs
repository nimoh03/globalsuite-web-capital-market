using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CapitalMarket.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;

namespace GlobalSuite.Core.CapitalMarket
{
    public interface ITradingService
    {
        Task<ResponseResult> CreateJobOrder(string customerId,JobOrder oJobOrder);
        Task<JobOrder> GetJobOrder();
        Task<ResponseResult> PostJobOrder(string code, DateTime effectiveDate);
        Task<ResponseResult> AllotmentBuy(Allotment oAllotment);
        Task<ResponseResult> AllotmentSell(Allotment oAllotment);
        Task<ResponseResult> AllotmentCrossDeal(Allotment oAllotmentBuy, Allotment oAllotmentSale);
        Task<ResponseResult> RunEndOfDayForFix(DateTime unPostedDate);
        Task<ResponseResult> AddCustomerStock(ProductAcct oProductAcct);
        Task<ResponseResult> DeleteCustomerStock(string code);
        Task<ResponseResult> PortfolioCreateInternalUpdate(PortfolioInternalUpdate oPortfolioInternalUpdate);
        Task<ResponseResult> PortfolioUpdateInternalUpdate(PortfolioInternalUpdate oPortfolioInternalUpdate);
        Task<ResponseResult> PostPortfolioInternalUpdate(string code);
        Task<ResponseResult> PostCsCsStockPositionFoxPro(DateTime date);
        Task<ResponseResult> SendPortfolioToEmailCsCs(SendPortfolioToEmail oSendPortfolioToEmail);
        Task<ResponseResult> PortfolioHoldingBalance(PortOpenBal oPortOpenBal);
        Task<ResponseResult> PostPortfolioHoldingBalance(string code);
        Task<ResponseResult> AddStock(Stock oStock);
        Task<Stock> GetStock(string code);
        Task<ResponseResult> EditStock(Stock oStock);
        Task<ResponseResult> DeleteStock(string code);
        Task<ResponseResult> NasdTradePosting(DateTime tradeDate, string filePath);
        Task<ResponseResult> AddCapitalMarketParam( string memberCode, StkParam oStkbPGenTable, PGenTable oPGenTable);
        Task<ResponseResult> AddBroker(Broker oBroker);
        Task<Broker> GetBroker(string code);
        Task<ResponseResult> EditBroker(Broker oBroker);
        Task<ResponseResult> DeleteBroker(string code);

        Task<ResponseResult> FixPullOut(DateTime tradeSummaryDate,DateTime accountStatementDate,
            DateTime portfolioDate);

        Task<ResponseResult> DailyTradePosting(DateTime tradeDate, string filePath);
        Task<ResponseResult> NasdPullOut(DateTime tradeDate);
        Task<ResponseResult> MergeCustomer(MergeCustomer oMergeCustomer, List<string> subCustomers);
        Task<ResponseResult> DailyTradePullOut(DateTime tradeDate);
        Task<ResponseResult> TradeValuation(Valuation oValuation);
    }
    
    
  
}  
