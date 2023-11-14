using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.BatchPosting;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<ResponseResult> CreateBatchPosting(DateTime effectiveDate, List<BatchSpreadSheet> spreadSheets)
        {
            return await Task.Run(async () =>
            {
                var master = new BatchSpreadSheetMaster
                {
                    EffectiveDate = effectiveDate,
                    InputBy = GeneralFunc.UserName,
                    ApprovedBy = string.Empty,
                    ReversedBy = string.Empty,
                    SaveType = Constants.SaveType.ADDS
                };
                 var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                 using (var connection = db.CreateConnection() as SqlConnection)
                 {
                     connection.Open();
                     var transaction = connection.BeginTransaction();
                     try
                     {
                         var dbCommandBatchSpreadSheetMaster = master.SaveCommand();
                         db.ExecuteNonQuery(dbCommandBatchSpreadSheetMaster, transaction);

                         master.BatchSpreadSheetMasterId = Convert.ToInt64(
                             db.GetParameterValue(dbCommandBatchSpreadSheetMaster, "BatchSpreadSheetMasterId"));
                         foreach (var oBatchSpreadSheetObj in spreadSheets)
                         {
                             try
                             {
                                 var account =await GetChartOfAccount(oBatchSpreadSheetObj.AccountId);
                                 if (account == null) return ResponseResult.Error($"Account with the ID: '{oBatchSpreadSheetObj.AccountId}' not found.");
                                 oBatchSpreadSheetObj.BatchSpreadSheetMasterId = master.BatchSpreadSheetMasterId;
                                 oBatchSpreadSheetObj.EffectiveDate = effectiveDate;
                                 var dbCommandoBatchSpreadSheetObj = oBatchSpreadSheetObj.AddCommand();
                                 db.ExecuteNonQuery(dbCommandoBatchSpreadSheetObj, transaction);
                             }
                             catch (Exception ex) 
                             {
                                Logger.Error(ex.Message, ex);
                             }
                         }
                         transaction.Commit();
                         return ResponseResult.Success();
                     }
                     catch (Exception ex)
                     {
                         Logger.Error(ex.Message, ex);
                         transaction.Rollback();
                         return ResponseResult.Error("Error In Saving Batch Posting  " + ex.Message.Trim());
                     }
                 }
            });
        }
        public async Task<ResponseResult> PostBatchPosting(long batchId)
        {
            return await Task.Run(() =>
            {
            var strJnumberNext = "";
            var oGl = new AcctGL();
            var oBatchSpreadSheetMaster = new BatchSpreadSheetMaster();
            oBatchSpreadSheetMaster.SaveType = "EDIT";
            var oBatchSpreadSheet = new BatchSpreadSheet();
            oBatchSpreadSheet.BatchSpreadSheetMasterId = batchId;
            oBatchSpreadSheetMaster.BatchSpreadSheetMasterId = batchId;
            if (!oBatchSpreadSheetMaster.ChkBatchSpreadSheetMasterExist(DataGeneral.PostStatus.UnPosted))
                return ResponseResult.Error("Cannot Post! Batch Number Does Not Exist Or Is Reversed");
            if (!oBatchSpreadSheetMaster.EqualDebitCredit())
                return ResponseResult.Error( "Cannot Post! Credit And Debit Amount Is Not Equal For This Batch Number");
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (var connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                    db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                    db.ExecuteNonQuery(oCommandJnumber, transaction);
                    strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                    foreach (DataRow thisRow in oBatchSpreadSheet.GetAll().Tables[0].Rows)
                    {
                        oGl.EffectiveDate = thisRow["EffectiveDate"].ToString().Trim().ToDate();
                        oGl.MasterID = thisRow["AccountId"].ToString();
                        oGl.AccountID = "";
                        oGl.AcctRef = "";
                        if (Convert.ToDecimal(thisRow["Credit"]) != 0 && Convert.ToDecimal(thisRow["Debit"]) == 0)
                        {
                            oGl.Credit = Convert.ToDecimal(thisRow["Credit"]);
                            oGl.Debit = 0;
                            oGl.Debcred = "C";
                        }
                        else if (Convert.ToDecimal(thisRow["Credit"]) == 0 && Convert.ToDecimal(thisRow["Debit"]) != 0)
                        {
                            oGl.Debit = Convert.ToDecimal(thisRow["Debit"]);
                            oGl.Credit = 0;
                            oGl.Debcred = "D";
                        }
                        else
                        {
                            throw new Exception("Debit Or Credit Value Is Invalid");
                        }

                        oGl.Desciption = thisRow["Description"].ToString();
                        oGl.TransType = "GLBATCHSHEET";
                        oGl.SysRef = "GBATSPSH" + "-" + thisRow["BatchSpreadSheetMasterId"].ToString().Trim();
                        oGl.Ref01 = thisRow["BatchSpreadSheetId"].ToString().Trim();
                        oGl.Chqno = "";
                        oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                        oGl.RecAcctMaster = oGl.MasterID;
                        oGl.RecAcct = oGl.AccountID;
                        oGl.AcctRefSecond = oGl.AcctRef;
                        oGl.Reverse = "N";
                        oGl.Jnumber = strJnumberNext;
                        oGl.Branch = thisRow["Branch"].ToString();
                        oGl.Reverse = "N";
                        oGl.FeeType = "";
                        oGl.Ref02 = "";
                        var dbCommandTrans = oGl.AddCommand();
                        db.ExecuteNonQuery(dbCommandTrans, transaction);
                    }

                    var oCommandBatchSpreadSheetMaster = new SqlCommand();
                    oCommandBatchSpreadSheetMaster =
                        db.GetStoredProcCommand("BatchSpreadSheetMasterUpdatePost") as SqlCommand;
                    db.AddInParameter(oCommandBatchSpreadSheetMaster, "Code", SqlDbType.BigInt,
                        oBatchSpreadSheetMaster.BatchSpreadSheetMasterId);
                    db.AddInParameter(oCommandBatchSpreadSheetMaster, "UserID", SqlDbType.VarChar,
                        GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(oCommandBatchSpreadSheetMaster, transaction);
                    transaction.Commit();
                    return ResponseResult.Success();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ResponseResult.Error("Error In Posting Batch " + ex.Message.Trim());
                }
            }

            });
        }

        public async Task<ResponseResult> ReverseBatchPosting(string batchNo)
        {
            return await Task.Run(() =>
            {
                 var strJnumberNext = "";
                 var oBatchOwner = new BatchOwner();
                 var oGl = new AcctGL();
                 var oGlGetRec = new AcctGL();
                 oBatchOwner.Batchno = batchNo.Trim();
                 if (oBatchOwner.GetBatchowner(DataGeneral.PostStatus.Posted))
                 {
                      
                     var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                     using (var connection = db.CreateConnection() as SqlConnection)
                     {
                         connection.Open();
                         var transaction = connection.BeginTransaction();

                         try
                         {
                             oGlGetRec.SysRef = "GBAT" + "-" + batchNo.Trim();
                             if (oGlGetRec.GetGLBySysRefNonRev().Tables[0].Rows.Count >= 1)
                             {
                                 var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                                 db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                                 db.ExecuteNonQuery(oCommandJnumber, transaction);
                                 strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                                 foreach (DataRow oRowView in oGlGetRec.GetGLBySysRefNonRev().Tables[0].Rows)
                                 {
                                     oGl.EffectiveDate = oRowView["EffDate"].ToString().Trim().ToDate();
                                     oGl.AccountID = oRowView["AccountID"].ToString().Trim();
                                     oGl.MasterID = oRowView["MasterID"].ToString().Trim();
                                     if (oRowView["Debcred"].ToString().Trim() == "D")
                                     {
                                         oGl.Credit = decimal.Parse(oRowView["Debit"].ToString().Trim());
                                         oGl.Debit = 0;
                                         oGl.Debcred = "C";
                                     }
                                     else if (oRowView["Debcred"].ToString().Trim() == "C")
                                     {
                                         oGl.Credit = 0;
                                         oGl.Debit = decimal.Parse(oRowView["Credit"].ToString().Trim());
                                         oGl.Debcred = "D";
                                     }
                                     oGl.FeeType = oRowView["FeeType"].ToString().Trim();
                                     oGl.Desciption = "REVERSAL Of " + oRowView["Description"].ToString().Trim();
                                     oGl.TransType = oRowView["TransType"].ToString().Trim();
                                     oGl.SysRef = oRowView["SysRef"].ToString().Trim();
                                     oGl.Ref01 = oRowView["Ref01"].ToString().Trim();
                                     oGl.AcctRef = oRowView["AcctRef"].ToString().Trim();
                                     oGl.AcctRefSecond = oRowView["AcctRefSecond"].ToString().Trim();
                                     oGl.RecAcctMaster = oRowView["RecAcctMas"].ToString().Trim();
                                     oGl.RecAcct = oRowView["RecAcctSub"].ToString().Trim();
                                     oGl.Reverse = "Y";
                                     oGl.Jnumber = strJnumberNext;
                                     oGl.Chqno = oRowView["Chqno"].ToString().Trim();
                                     oGl.InstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), oRowView["InstrumentType"].ToString().Trim(), false); 
                                     oGl.Branch = oRowView["Branch"].ToString().Trim();
                                     oGl.Ref02 = oRowView["Ref02"].ToString().Trim();
                                     var dbCommandGlTrans = oGl.AddCommand();
                                     db.ExecuteNonQuery(dbCommandGlTrans, transaction);
                                 }

                                 var dbCommandGlTransUpdate = oGlGetRec.UpdateGLBySysRefReversalCommand();
                                 db.ExecuteNonQuery(dbCommandGlTransUpdate, transaction);

                                 oBatchOwner.Reversed = "Y";
                                 oBatchOwner.Reversedby = GeneralFunc.UserName;
                                 var dbCommandBatchPostRev = oBatchOwner.UpDateRevCommand();
                                 db.ExecuteNonQuery(dbCommandBatchPostRev, transaction);
                                 transaction.Commit();
                                 return ResponseResult.Success();
                             }
                         }
                         catch (Exception ex)
                         {
                             Logger.Error(ex.Message, ex);
                             transaction.Rollback();
                             return ResponseResult.Error("Error Reversing Batch Posting " + ex.Message);
                         }

                     }
                 }
                 return ResponseResult.Error("Error Reversing Batch Posting");
            });
        }
        public async Task<ResponseResult> DeleteBatchPosting(long batchNo)
        {
            return await Task.Run(() =>
            {
                var oBatch = new BatchSpreadSheetMaster()
                {
                    BatchSpreadSheetMasterId = batchNo,
                };
                    return oBatch.Delete() ? ResponseResult.Success() : ResponseResult.Error("Error In Deleting Batch Transaction");
            });
        }
        
        public async Task<BatchPostingDetail> GetBatchPosting(long batchNo, DataGeneral.PostStatus status)
        {
            return await Task.Run(async () =>
            {
                var master = new BatchSpreadSheetMaster()
                {
                    BatchSpreadSheetMasterId = batchNo,
                };
                var oBatch = new BatchSpreadSheet()
                {
                    BatchSpreadSheetMasterId = batchNo,
                };
                master.GetBatchSpreadSheetMaster("Acct_BatchSpreadSheetMaster","BatchSpreadSheetMasterId", status);
                var detail = Mapper.Map<BatchPostingDetail>(master);
                var ds=  oBatch.GetAll();
               var transactions= ds.Tables[0].ToList<BatchTransactionResponse>();
                foreach (var transaction in transactions)
                {
                    transaction.Account = await GetChartOfAccount(transaction.AccountId);
                    detail.Transactions.Add(transaction);
                }
                return detail;
            });
        }
        public async Task<List<BatchPostingResponse>> GetBatchPostings(BatchPostingFilter filter)
        {
            return await Task.Run(() =>
            {
                var oBatch = new BatchSpreadSheetMaster();
                filter = filter ?? new BatchPostingFilter();
                if (filter.Status.ToLower() == "all") filter.Status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(filter.Status, true, out DataGeneral.PostStatus postStatus);

                DataSet ds;

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    oBatch.EffectiveDate = filter.StartDate.Value;
                    ds = oBatch.GetAllGivenEffDate(postStatus);
                }

               else if (!string.IsNullOrEmpty(filter.TxnFrom) && !string.IsNullOrEmpty(filter.TxnTo))
                {
                    ds = oBatch.GetAllGivenTxnNo(postStatus, filter.TxnFrom, filter.TxnTo);
                }
                else
                {
                    var now = DateTime.Now;
                    filter.StartDate = now.FirstDayOfMonth();
                    filter.EndDate = now.LastDayOfMonth();
                    oBatch.EffectiveDate = now.FirstDayOfMonth();
                   ds= GeneralFunc.GetAllPosting("Acct_BatchSpreadSheetMaster", postStatus);
                }
                var dt = ds.Tables[0].AsEnumerable().Skip(filter.Skip).Take(filter.PageSize);

                return dt.Select(row => row.GetItem<BatchPostingResponse>()).ToList();
            });
        }

        
        private BatchPostingResponse FormatBatchPosting(BatchPostingResponse batchPosting)
        {
            // var oProduct = new Product
            // {
            //     TransNo = batchPosting.AccountID
            // };
            // oProduct.GetProduct();
            // batchPosting.Account = Mapper.Map<ProductResponse>(oProduct);
            // if (string.IsNullOrEmpty(batchPosting.Branch)) return batchPosting;
            // var oBranch = new Branch
            // {
            //     TransNo = batchPosting.Branch
            // };
            // oBranch.GetBranch();
            // batchPosting.BranchDetail = Mapper.Map<BranchResponse>(oBranch);
            return batchPosting;
        }
    }
}