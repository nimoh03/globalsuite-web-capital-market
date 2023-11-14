using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using BaseUtility.Business;
using CapitalMarket.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        public async Task<ResponseResult> FixPullOut(DateTime tradeSummaryDate,DateTime accountStatementDate,
            DateTime portfolioDate)
        {
            return await Task.Run(() =>
            {
                var oAutoDatePre = new AutoDate();
                try
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                    var strUserName = GeneralFunc.UserName.Trim();
                    var oGl = new AcctGL();
                    var oPort = new Portfolio();
                    var oAllot = new Allotment();
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            if (tradeSummaryDate!=default)
                            {
                                oAutoDatePre.iAutoDate = tradeSummaryDate.ToExact();
                                if (oAutoDatePre.GetAutoDate())
                                    return ResponseResult.Error("Cannot Reverse For Trade Summary! End Of Day Already Processed For This FIX Trade Date");
                                oGl.EffectiveDate = tradeSummaryDate.ToExact();
                                var dbCommandDeleteGlFix = oGl.DeleteGLFIXCommand(tradeSummaryDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeleteGlFix, transaction);

                                var dbCommandDeletePortfolioFix = oPort.DeleteUploadFIXCommand(tradeSummaryDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeletePortfolioFix, transaction);

                                var dbCommandDeleteAllotmentFix = oAllot.DeleteBuySellUploadFIXCommand(tradeSummaryDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeleteAllotmentFix, transaction);
                                
                            }
                            if (accountStatementDate!=default)
                            {
                                oAutoDatePre.iAutoDate = accountStatementDate.ToExact();
                                if (oAutoDatePre.GetAutoDate())
                                    return ResponseResult.Error( "Cannot Reverse For Account Statement! End Of Day Already Processed For This FIX Trade Date");
                                oGl.EffectiveDate = accountStatementDate.ToExact();
                                var dbCommandDeleteGlfix = oGl.DeleteGLFIXCommand(accountStatementDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeleteGlfix, transaction);

                                var dbCommandDeletePortfolioFix = oPort.DeleteUploadFIXCommand(accountStatementDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeletePortfolioFix, transaction);

                                var dbCommandDeleteAllotmentFix = oAllot.DeleteBuySellUploadFIXCommand(accountStatementDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeleteAllotmentFix, transaction); }

                            if (portfolioDate==default)
                            {
                                oAutoDatePre.iAutoDate = portfolioDate.ToExact();
                                if (oAutoDatePre.GetAutoDate())
                                    return ResponseResult.Error( "Cannot Reverse For Portfolio! End Of Day Already Processed For This FIX Trade Date");
                                oGl.EffectiveDate = portfolioDate.ToExact();
                                var dbCommandDeleteGlfix = oGl.DeleteGLFIXCommand(portfolioDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeleteGlfix, transaction);

                                var dbCommandDeletePortfolioFix = oPort.DeleteUploadFIXCommand(portfolioDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeletePortfolioFix, transaction);

                                var dbCommandDeleteAllotmentFix = oAllot.DeleteBuySellUploadFIXCommand(portfolioDate.ToExact());
                                db.ExecuteNonQuery(dbCommandDeleteAllotmentFix, transaction);   }
                             
                            transaction.Commit();
                         return   ResponseResult.Success("FIX Trades Upload PullOut/Reversal Successful");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message, ex);
                            transaction?.Rollback();
                            return ResponseResult.Error("Error In Reversing/PullOut FIX Trades " + ex.Message.Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error("Error In Reversing/PullOut " + ex.Message.Trim());
                }

            });
        }
    }
}