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
        public async Task<ResponseResult> DailyTradePullOut(DateTime tradeDate)
        {
            return await Task.Run(() =>
              {
                  
                  if (tradeDate ==null)
                      return ResponseResult.Error("Date Cannot Be Empty");
                  try
                  {
                      System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                      IFormatProvider format = new CultureInfo("en-GB");
                      string strUserName = GeneralFunc.UserName.Trim();
                      //decimal decTotalBank = 0;
                      DataTable tabAllot;
                      AcctGL oGL = new AcctGL();
                      Portfolio oPort = new Portfolio();
                      JobOrder oJob = new JobOrder();
                      Allotment oAllot = new Allotment();
                      tabAllot = oAllot.GetAllotmentByDateForUpload(tradeDate).Tables[0];

                      //decTotalBank = oGL.GetTotalAmountPostingByDateForUpload(DateTime.ParseExact(txtDate.Text.Trim(), "dd/MM/yyyy", format));
                      DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                      using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                      {
                          connection.Open();
                          SqlTransaction transaction = connection.BeginTransaction();
                          try
                          {
                              foreach (DataRow oRowView in tabAllot.Rows)
                              {
                                  if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                  {
                                      oGL.EffectiveDate = tradeDate;
                                      oGL.SysRef = "BSB" + "-" + oRowView["Txn#"].ToString().Trim();
                                      SqlCommand dbCommandDelSysRefBuy = oGL.DeleteGLBySysRefCommand();
                                      db.ExecuteNonQuery(dbCommandDelSysRefBuy, transaction);

                                      oPort.SysRef = "STKB" + "-" + oRowView["Txn#"].ToString().Trim();
                                      SqlCommand dbCommandDeleteUploadPortBuy = oPort.DeleteUploadByTransNoCommand();
                                      db.ExecuteNonQuery(dbCommandDeleteUploadPortBuy, transaction);

                                      JobOrder oJobReverse = new JobOrder();
                                      DataSet oDsJobReverse = new DataSet();
                                      oDsJobReverse = oJobReverse.GetJobbingAllotmentEffect(oRowView["Txn#"].ToString().Trim());
                                      DataTable thisTableJobReverse = oDsJobReverse.Tables[0];
                                      foreach (DataRow oAllomentJob in thisTableJobReverse.Rows)
                                      {
                                          SqlCommand dbCommandJobReverse = oJobReverse.UpdateBalanceProcessedCommand(oAllomentJob["JobOrderNo"].ToString().Trim(), oAllomentJob["AllotmentNo"].ToString().Trim(), int.Parse(oAllomentJob["Quantity"].ToString().Trim()));
                                          db.ExecuteNonQuery(dbCommandJobReverse, transaction);
                                      }

                                  }
                                  else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                                  {
                                      oGL.EffectiveDate = tradeDate;
                                      oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
                                      SqlCommand dbCommandDelSysRefSell = oGL.DeleteGLBySysRefCommand();
                                      db.ExecuteNonQuery(dbCommandDelSysRefSell, transaction);

                                      oPort.SysRef = "COLT" + "-" + oRowView["Txn#"].ToString().Trim();
                                      SqlCommand dbCommandDeleteUploadPortSell = oPort.DeleteUploadByTransNoCommand();
                                      db.ExecuteNonQuery(dbCommandDeleteUploadPortSell, transaction);

                                      JobOrder oJobReverse = new JobOrder();
                                      DataSet oDsJobReverse = new DataSet();
                                      oDsJobReverse = oJobReverse.GetJobbingAllotmentEffect(oRowView["Txn#"].ToString().Trim());
                                      DataTable thisTableJobReverse = oDsJobReverse.Tables[0];
                                      foreach (DataRow oAllomentJob in thisTableJobReverse.Rows)
                                      {
                                          SqlCommand dbCommandJobReverse = oJobReverse.UpdateBalanceProcessedCommand(oAllomentJob["JobOrderNo"].ToString().Trim(), oAllomentJob["AllotmentNo"].ToString().Trim(), int.Parse(oAllomentJob["Quantity"].ToString().Trim()));
                                          db.ExecuteNonQuery(dbCommandJobReverse, transaction);
                                      }

                                  }

                                  oAllot.Txn = oRowView["Txn#"].ToString().Trim();
                                  SqlCommand dbCommandDeleteBuySellUpload = oAllot.DeleteBuySellUploadByTransNoCommand();
                                  db.ExecuteNonQuery(dbCommandDeleteBuySellUpload, transaction);

                              }

                              AcctGL oGLDelete = new AcctGL();
                              oGLDelete.EffectiveDate = tradeDate;
                              SqlCommand dbCommandDeleteGLUpload = oGLDelete.DeleteTradBankUploadByDateCommand();
                              db.ExecuteNonQuery(dbCommandDeleteGLUpload, transaction);

                              AutoDate oAutoDate = new AutoDate();
                              oAutoDate.iAutoDate = tradeDate;
                              SqlCommand dbCommandDeleteAutoDate = oAutoDate.DeleteCommand();
                              db.ExecuteNonQuery(dbCommandDeleteAutoDate, transaction);

                              transaction.Commit();

                            return  ResponseResult.Success("Allotment Upload PullOut/Reversal Successfull");
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

                  return ResponseResult.Success();
              });
        }
    }
}
