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
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Accounting.Models;
using GlobalSuite.Core.Admin.Models;
using GlobalSuite.Core.Customers.Models;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<ResponseResult> CreditNote(CNote oCNote)
        {
            return await Task.Run(() =>
            {
                try
                {
                 return   AddOrEditCreditNote(oCNote,Constants.SaveType.ADDS);
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message, ex);
                   return ResponseResult.Error(ex.Message);
                }
            });
        }
        public async Task<ResponseResult> EditCreditNote(CNote oCNote)
        {
            return await Task.Run(() =>
            {
                try
                {
                 return   AddOrEditCreditNote(oCNote,Constants.SaveType.EDIT);
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message, ex);
                   return ResponseResult.Error(ex.Message);
                }
            });
        }

        private ResponseResult AddOrEditCreditNote(CNote oCNote, string saveMode)
        {
            var oNumWord = new NumberToWords();
            oCNote.AmtWord = oNumWord.ConvertToWord($"{oCNote.Amount}");
            oCNote.Posted = false;
            oCNote.Reversed = false;
            oCNote.TransNoRev = string.Empty;
            oCNote.SaveType = saveMode;
            var oSaveStatus = oCNote.Save();
            if (oSaveStatus == DataGeneral.SaveStatus.Saved) return ResponseResult.Success(Mapper.Map<CreditNoteResponse>(oCNote));
            if (oSaveStatus == DataGeneral.SaveStatus.NotExist)
                return ResponseResult.Error( "Cannot Edit Opening Balance Posting Does Not Exist");
            if (oSaveStatus == DataGeneral.SaveStatus.DuplicateRef)
                return ResponseResult.Error( "Cannot Save,Duplicate Reference Number");
            if (oSaveStatus == DataGeneral.SaveStatus.NotSaved)
                return ResponseResult.Error("Error In Saving");
            return ResponseResult.Error();
        }

        public async Task<ResponseResult> PostCreditNote(string code)
        {
            return await Task.Run(() =>
            {
                var oAccount = new Account();
                var oAcctGl = new AcctGL();
                var oDep = new CNote
                {
                    CreditNo = code.Trim()
                };

                if (!oDep.GetCrNote(DataGeneral.PostStatus.UnPosted))
                    return ResponseResult.Error(
                        "Cannot Post! This Customer CNote Does Not Exist or Already Posted/Reversed,Try Saving");

                var oProduct = new Product
                {
                    TransNo = oDep.AcctMasBank
                };

                oAcctGl.EffectiveDate = oDep.RNDate;
                oAcctGl.MasterID = oProduct.GetProductGLAcct();
                oAcctGl.AccountID = oDep.CustNo.Trim();
                oAcctGl.AcctRef = oDep.AcctMasBank;
                oAcctGl.Credit = oDep.Amount;
                var oCustomer = new Customer
                {
                    CustAID = oDep.CustNo
                };
                oAcctGl.Desciption = oDep.Trandesc;
                if (oCustomer.GetCustomerName(oDep.AcctMasBank))
                {
                    oAcctGl.Description2 = oDep.Trandesc.Trim() + " : " + oCustomer.CombineName().Trim();
                    oAcctGl.CustomerName = oCustomer.CombineName().Trim();
                }
                else
                {
                    oAcctGl.Description2 = oDep.Trandesc.Trim();
                    oAcctGl.CustomerName = "";
                }

                oAcctGl.TransType = "CRENOTE";
                oAcctGl.SysRef = "CRN" + "-" + oDep.CreditNo.Trim();
                oAcctGl.Ref01 = oDep.CreditNo;
                oAcctGl.Ref02 = oDep.CustNo;
                oAcctGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                oAcctGl.RecAcct = "";
                oAcctGl.RecAcctMaster = oDep.AcctSubBank;
                oAcctGl.AcctRefSecond = "";
                oAcctGl.Branch = oAccount.GetAccountBranch(oDep.AcctSubBank);
                oAcctGl.Reverse = "N";
                oAcctGl.FeeType = "";
                oAcctGl.Chqno = "";


                return oAcctGl.Post(Constants.TableName.CRNOTE)
                    ? ResponseResult.Success("Customer Credit Note Posted Successfully!")
                    : ResponseResult.Error();
            });
        }

        public async Task<ResponseResult> ReverseCreditNote(string code)
        {

            return await Task.Run(() =>
            {
                try
                {
                    var strJnumberNext = "";
                    var oDep = new CNote();
                    var oGl = new AcctGL();
                    var oGlGetRec = new AcctGL();
                    oDep.CreditNo = code.Trim();
                    if (oDep.GetCrNote(DataGeneral.PostStatus.Posted))
                    {
                        var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                        using (var connection = db.CreateConnection() as SqlConnection)
                        {
                            connection.Open();
                            var transaction = connection.BeginTransaction();

                            try
                            {
                                oGlGetRec.SysRef = "CRN" + "-" + code.Trim();

                                if (oGlGetRec.GetGLBySysRefNonRev().Tables[0].Rows.Count >= 1)
                                {
                                    var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                                    db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                                    db.ExecuteNonQuery(oCommandJnumber, transaction);
                                    strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                                    foreach (DataRow oRowView in oGlGetRec.GetGLBySysRefNonRev().Tables[0].Rows)
                                    {
                                        oGl.EffectiveDate = oRowView["EffDate"].ToString().ToDate();
                                        oGl.AccountID = oRowView["AccountID"].ToString();
                                        oGl.MasterID = oRowView["MasterID"].ToString();

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
                                        oGl.Ref02 = oRowView["Ref02"].ToString().Trim();
                                        oGl.AcctRef = oRowView["AcctRef"].ToString().Trim();
                                        oGl.RecAcctMaster = oRowView["RecAcctMas"].ToString().Trim();
                                        oGl.RecAcct = oRowView["RecAcctSub"].ToString().Trim();
                                        oGl.AcctRefSecond = oRowView["AcctRefSecond"].ToString().Trim();
                                    
                                    
                                        oGl.Reverse = "Y";
                                        oGl.Jnumber = strJnumberNext;
                                    
                                        oGl.InstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), oRowView["InstrumentType"].ToString().Trim(), false); 
                                    
                                        oGl.Chqno = oRowView["Chqno"].ToString().Trim();
                                    
                                    
                                        oGl.Branch = oRowView["Branch"].ToString().Trim();
                                    
                                        oGl.FeeType = "";
                                        var dbCommandGlTrans = oGl.AddCommand();
                                        db.ExecuteNonQuery(dbCommandGlTrans, transaction);
                                    }
                                    var dbCommandGlTransUpdate = oGlGetRec.UpdateGLBySysRefReversalCommand();
                                    db.ExecuteNonQuery(dbCommandGlTransUpdate, transaction);

                                    oDep.Reversed = true;
                                    var dbCommandCNote = oDep.UpDateRevCommand();
                                    db.ExecuteNonQuery(dbCommandCNote, transaction);
                                    transaction.Commit();
                                    return ResponseResult.Success();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message,ex);
                                transaction.Rollback();
                                return ResponseResult.Error("Error Reversing Customer CNote Posting " + ex.Message);
                            }
                            connection.Close();
                        }
                    }
            
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message,ex);
                }
                return ResponseResult.Error("Credit Note cannot be reversed.");
            });
        }

        public async Task<List<CreditNoteResponse>> GetCreditNotes(CreditNoteFilter filter)
        {
            return await Task.Run(() =>
            {
                var oCNote = new CNote();
                filter = filter ?? new CreditNoteFilter();
                if (filter.Status.ToLower() == "all") filter.Status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(filter.Status, true, out DataGeneral.PostStatus postStatus);
                DataSet ds;
                if (filter.StartDate.HasValue && filter.EndDate.HasValue&& postStatus!=DataGeneral.PostStatus.All)
                {
                    oCNote.EDate = filter.StartDate.Value.ToExact();
                    oCNote.EDateTo = filter.EndDate.Value.ToExact();
                    ds = oCNote.GetAllGivenEntryDate(postStatus);
                }
                else
                {
                    ds=   oCNote.GetAll(postStatus);
                }
                var dt = ds.Tables[0].AsEnumerable().Skip(filter.Skip).Take(filter.PageSize);

                return dt.Select(row => FormatCreditNote(row.GetItem<CreditNoteResponse>())).ToList();
            });
        }

        public async Task<CreditNoteResponse> GetCreditNote(string code, DataGeneral.PostStatus postStatus)
        {
            return await Task.Run(() =>
            {
                var oCNote = new CNote
                {
                    CreditNo = code.Trim()
                };
                var isSuccess = oCNote.GetCrNote(postStatus);
                if (!isSuccess) return null;
                var p = Mapper.Map<CreditNoteResponse>(oCNote);
                 
               
                return FormatCreditNote(p);
            });
        }
        private CreditNoteResponse FormatCreditNote(CreditNoteResponse payment)
        {
            var oCustomer = new Customer{CustAID = payment.Custno};
            oCustomer.GetCustomer();
            payment.Customer = Mapper.Map<PublicCustomer>(oCustomer);
            var oProduct = new Product
            {
                TransNo = payment.AcctMasBank
            };
            oProduct.GetProduct();
            payment.ProductDetail = Mapper.Map<ProductDetailResponse>(oProduct);
            var oBranch = new Branch
            {
                TransNo = oCustomer.Branch
            };
            oBranch.GetBranch();
            payment.Branch = Mapper.Map<BranchResponse>(oBranch);
            var oAcct = new Account
            {
                AccountId = payment.AcctSubBank
            };
            oAcct.GetAccount(oCustomer.Branch);
            payment.SubBank = new ChartOfAccountResponse
            {
                AccountId = oAcct.AccountId,
                AccountName = oAcct.AccountName
            };
            
            return payment;
        }
    }
}