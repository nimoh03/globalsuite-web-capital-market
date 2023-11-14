using System;
using System.Data;
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
        public async Task<ResponseResult> NasdPullOut(DateTime tradeDate)
        {
            return await Task.Run(() =>
              {
                  try
                  {
                      System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                    
                      var strUserName = GeneralFunc.UserName.Trim();
                      //decimal decTotalBank = 0;
                      var strMarginCodeForNasdIndicator = "NASDOTC";
                      var oGl = new AcctGL();
                      var oPort = new Portfolio();
                      var oJob = new JobOrder();
                      var oAllot = new Allotment();
                      var tabAllot = oAllot.GetAllotmentByDateForUploadBank(tradeDate, strMarginCodeForNasdIndicator).Tables[0];


                      //decTotalBank = oGL.GetTotalAmountPostingByDateForUpload(DateTime.ParseExact(txtDate.Text.Trim(), "dd/MM/yyyy", format));
                      var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                      using (var connection = db.CreateConnection() as SqlConnection)
                      {
                          connection.Open();
                          var transaction = connection.BeginTransaction();
                          try
                          {
                              foreach (DataRow oRowView in tabAllot.Rows)
                            {
                                switch (oRowView["Buy_Sold_Ind"].ToString().Trim())
                                {
                                    case "B":
                                    {
                                        oGl.EffectiveDate = tradeDate.ToExact();
                                        oGl.SysRef = "BSB" + "-" + oRowView["Txn#"].ToString().Trim();
                                        var dbCommandDelSysRefBuy = oGl.DeleteGLBySysRefCommand();
                                        db.ExecuteNonQuery(dbCommandDelSysRefBuy, transaction);

                                        oPort.SysRef = "STKB" + "-" + oRowView["Txn#"].ToString().Trim();
                                        var dbCommandDeleteUploadPortBuy = oPort.DeleteUploadByTransNoCommand();
                                        db.ExecuteNonQuery(dbCommandDeleteUploadPortBuy, transaction);

                                        var oJobReverse = new JobOrder();
                                        var oDsJobReverse = oJobReverse.GetJobbingAllotmentEffect(oRowView["Txn#"].ToString().Trim());
                                        var thisTableJobReverse = oDsJobReverse.Tables[0];
                                        foreach (DataRow oAllotmentJob in thisTableJobReverse.Rows)
                                        {
                                            var dbCommandJobReverse = oJobReverse.UpdateBalanceProcessedCommand(oAllotmentJob["JobOrderNo"].ToString().Trim(), oAllotmentJob["AllotmentNo"].ToString().Trim(), int.Parse(oAllotmentJob["Quantity"].ToString().Trim()));
                                            db.ExecuteNonQuery(dbCommandJobReverse, transaction);
                                        }

                                        break;
                                    }
                                    case "S":
                                    {
                                        oGl.EffectiveDate = tradeDate.ToExact();
                                        oGl.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
                                        var dbCommandDelSysRefSell = oGl.DeleteGLBySysRefCommand();
                                        db.ExecuteNonQuery(dbCommandDelSysRefSell, transaction);

                                        oPort.SysRef = "COLT" + "-" + oRowView["Txn#"].ToString().Trim();
                                        var dbCommandDeleteUploadPortSell = oPort.DeleteUploadByTransNoCommand();
                                        db.ExecuteNonQuery(dbCommandDeleteUploadPortSell, transaction);

                                        var oJobReverse = new JobOrder();
                                        var oDsJobReverse = oJobReverse.GetJobbingAllotmentEffect(oRowView["Txn#"].ToString().Trim());
                                        var thisTableJobReverse = oDsJobReverse.Tables[0];
                                        foreach (DataRow oAllotmentJob in thisTableJobReverse.Rows)
                                        {
                                            var dbCommandJobReverse = oJobReverse.UpdateBalanceProcessedCommand(oAllotmentJob["JobOrderNo"].ToString().Trim(), oAllotmentJob["AllotmentNo"].ToString().Trim(), int.Parse(oAllotmentJob["Quantity"].ToString().Trim()));
                                            db.ExecuteNonQuery(dbCommandJobReverse, transaction);
                                        }

                                        break;
                                    }
                                }

                                oAllot.Txn = oRowView["Txn#"].ToString().Trim();
                                var dbCommandDeleteBuySellUpload = oAllot.DeleteBuySellUploadByTransNoCommand();
                                db.ExecuteNonQuery(dbCommandDeleteBuySellUpload, transaction);

                            }
                            
                            var oGlDelete = new AcctGL
                            {
                                EffectiveDate = tradeDate.ToExact() 
                            };
                            var dbCommandDeleteGlUpload = oGlDelete.DeleteTradBankUploadByDateNASDCommand(strMarginCodeForNasdIndicator);
                            db.ExecuteNonQuery(dbCommandDeleteGlUpload, transaction);

                            var oAutoDateNasd = new AutoDateNASD
                            {
                                iAutoDate = tradeDate.ToExact()
                            };
                            var dbCommandDeleteAutoDate = oAutoDateNasd.DeleteCommand();
                            db.ExecuteNonQuery(dbCommandDeleteAutoDate, transaction);


                          

                            transaction.Commit();
                            return  ResponseResult.Success("Allotment Upload PullOut/Reversal Successful");
                          }
                          catch (Exception ex)
                          {
                              Logger.Error(ex.Message, ex);
                              transaction?.Rollback();
                              return ResponseResult.Error("Error In Reversing/PullOut " + ex.Message.Trim());

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
