using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Admin.Business;
using BaseUtility.Business;
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
        public async Task<ResponseResult> OpeningBalance(CustOBal oCustomerBalance)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return AddOrEditOpeningBalance(oCustomerBalance, Constants.SaveType.ADDS);
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message, ex);
                }
               
                return  ResponseResult.Error("Cannot Save Opening Balance.");
            });
        }

         public async Task<ResponseResult> EditOpeningBalance(CustOBal oCustomerBalance)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return AddOrEditOpeningBalance(oCustomerBalance, Constants.SaveType.EDIT);
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message, ex);
                }
               
                return  ResponseResult.Error("Cannot Save Opening Balance.");
            });
        }

        private ResponseResult AddOrEditOpeningBalance(CustOBal oCustomerBalance, string saveMode)
        {
            oCustomerBalance.Posted = false;
            oCustomerBalance.Reversed = false;
            oCustomerBalance.TransNoRev = string.Empty;
            oCustomerBalance.SaveType = saveMode;
            var oSaveStatus = oCustomerBalance.Save();
            return oSaveStatus==DataGeneral.SaveStatus.Saved ? ResponseResult.Success() : ResponseResult.Error("Error saving.");
        }

        public async Task<ResponseResult> PostOpeningBalance(string code)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var oGl = new AcctGL();
                    var oCustOBal = new CustOBal
                    {
                        TransNo = code.Trim()
                    };

                    if (!oCustOBal.GetCustOBal(DataGeneral.PostStatus.UnPosted))
                        return ResponseResult.Error("Cannot Post! Customer Account Opening Balance Does Not Exist Or Reversed");
                    var oProduct = new Product
                    {
                        TransNo = oCustOBal.Ref
                    };

                        oGl.EffectiveDate = oCustOBal.RNDate == DateTime.MinValue ? DateTime.MinValue : oCustOBal.RNDate;
                         var oGlParam = new GLParam();
                         if (oGlParam.CustOpenAcct.Trim() == "")
                             return ResponseResult.Error("Cannot Post! GL Parameter Table Not Setup Or Missing Opening Balance Control A/C");
                         oGl.Credit = oCustOBal.Amount;
                         var oCustomer = new Customer
                         {
                             CustAID = oCustOBal.CustNo
                         };
                         oGl.Desciption = oCustOBal.Trandesc;
                         if (oCustomer.GetCustomerName(oCustOBal.Ref))
                             oGl.Description2 = oCustOBal.Trandesc.Trim() + " : " + oCustomer.CombineName().Trim();
                         else
                             oGl.Description2 = oCustOBal.Trandesc.Trim();
                         oGl.Branch = oCustomer.Branch;

                         oGl.TransType = "OPCOB";
                         oGl.SysRef = "COB" + "-" + oCustOBal.TransNo;
                         oGl.Ref01 = oCustOBal.TransNo.Trim();
                         if (oCustOBal.DebCred == "C")
                         {
                             oGl.MasterID = oProduct.GetProductGLAcct();
                             oGl.AccountID = oCustOBal.CustNo;
                             oGl.AcctRef = oCustOBal.Ref;
                             oGl.RecAcct = "";
                             oGl.RecAcctMaster = oGlParam.CustOpenAcct.Trim();
                             oGl.AcctRefSecond = "";

                         }
                         else if (oCustOBal.DebCred == "D")
                         {
                             oGl.RecAcctMaster = oProduct.GetProductGLAcct();
                             oGl.RecAcct = oCustOBal.CustNo;
                             oGl.AcctRefSecond = oCustOBal.Ref;
                             oGl.AccountID = "";
                             oGl.MasterID = oGlParam.CustOpenAcct;
                             oGl.AcctRef = "";

                         }
                         oGl.Reverse = "N";
                         
                         oGl.FeeType = "";
                         
                         
                         oGl.InstrumentType = DataGeneral.GLInstrumentType.C;
                         
                         oGl.Chqno = "";
                         oGl.Ref02 = oCustOBal.CustNo.Trim();
                         return oGl.Post("CUSTOBAL")
                             ? ResponseResult.Success("Customer Credit Note Posted Successfully!")
                             : ResponseResult.Error("Cannot Post Customer Balance");
                }
                catch (Exception exception)
                {
                    Logger.Error(exception.Message, exception);
                    return ResponseResult.Error("Error In Posting " + exception.Message);
                }
            });
        }
        public async Task<List<OpeningBalanceResponse>> GetOpeningBalances(StatusFilter filter)
        {
            return await Task.Run(() =>
            {
                var acctOBal = new CustOBal();
                filter = filter ?? new StatusFilter();
                if (filter.Status.ToLower() == "all") filter.Status = DataGeneral.PostStatus.UnPosted.ToString();
                Enum.TryParse(filter.Status, true, out DataGeneral.PostStatus postStatus);
                DataSet ds;
                if (filter.StartDate.HasValue && filter.EndDate.HasValue&& postStatus!=DataGeneral.PostStatus.All)
                {
                    acctOBal.EDate = filter.StartDate.Value.ToExact();
                    acctOBal.EDateTo = filter.EndDate.Value.ToExact();
                    ds = acctOBal.GetAllGivenEntryDate(postStatus);
                }
                else
                {
                    ds=   acctOBal.GetAll(postStatus);
                }
                var dt = ds.Tables[0].AsEnumerable().Skip(filter.Skip).Take(filter.PageSize);
                var oList = new List<OpeningBalanceResponse>();
                foreach (var row in dt)
                {
                    var item = FormatOpeningBalance(row.GetItem<OpeningBalanceResponse>());
                    item.Source = $"{row["UserId"]}";
                    oList.Add(item);
                }

                return oList;// dt.Select(row => FormatOpeningBalance(row.GetItem<OpeningBalanceResponse>())).ToList();
            });
        }
        public async Task<OpeningBalanceResponse> GetOpeningBalance(string code, DataGeneral.PostStatus status)
        {
            if (!Enum.IsDefined(typeof(DataGeneral.PostStatus), status))
                throw new InvalidEnumArgumentException(nameof(status), (int)status, typeof(DataGeneral.PostStatus));
            return await Task.Run(() =>
            {
                var acctOBal = new CustOBal
                {
                    TransNo = code.Trim()
                };
                var isSuccess = acctOBal.GetCustOBal(status);
                if (!isSuccess) return null;
                var res = Mapper.Map<OpeningBalanceResponse>(acctOBal);
                 return FormatOpeningBalance(res);
            });
        }
        
        private OpeningBalanceResponse FormatOpeningBalance(OpeningBalanceResponse balance)
        {
            var oCustomer = new Customer{CustAID = balance.CustNo};
            oCustomer.GetCustomer();
            balance.Customer = Mapper.Map<PublicCustomer>(oCustomer);
            balance.BranchId = GeneralFunc.UserBranchNumber;
            var branch = new Branch
            {
                TransNo = GeneralFunc.UserBranchNumber
            };
            branch.GetBranch();
            balance.BranchDetail = Mapper.Map<BranchResponse>(branch);
            
            return balance;
        }
          public async Task<ResponseResult> ReverseOpeningBalance(string code)
        {

            return await Task.Run(() =>
            {
                try
                {
                   
                  var strJnumberNext = "";
                 var oCustOBal = new CustOBal();
                 var oGL = new AcctGL();
                 var oGLGetRec = new AcctGL();
                 oCustOBal.TransNo = code.Trim();
                 if (oCustOBal.GetCustOBal(DataGeneral.PostStatus.Posted))
                 {
                     System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                     IFormatProvider format = new CultureInfo("en-GB");

                     var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                     using (var connection = db.CreateConnection() as SqlConnection)
                     {
                         connection.Open();
                         var transaction = connection.BeginTransaction();
                         try
                         {
                             oGLGetRec.SysRef = "COB" + "-" + code.Trim();
                             if (oGLGetRec.GetGLBySysRefNonRev().Tables[0].Rows.Count >= 1)
                             {
                                 var oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                                 db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                                 db.ExecuteNonQuery(oCommandJnumber, transaction);
                                 strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                                 foreach (DataRow oRowView in oGLGetRec.GetGLBySysRefNonRev().Tables[0].Rows)
                                 {
                                     oGL.EffectiveDate =oRowView["EffDate"].ToString().ToDate();
                                     oGL.AccountID = oRowView["AccountID"].ToString().Trim();
                                     oGL.MasterID = oRowView["MasterID"].ToString().Trim();
                                     if (oRowView["Debcred"].ToString().Trim() == "D")
                                     {
                                         oGL.Credit = decimal.Parse(oRowView["Debit"].ToString().Trim());
                                         oGL.Debit = 0;
                                         oGL.Debcred = "C";
                                     }
                                     else if (oRowView["Debcred"].ToString().Trim() == "C")
                                     {
                                         oGL.Credit = 0;
                                         oGL.Debit = decimal.Parse(oRowView["Credit"].ToString().Trim());
                                         oGL.Debcred = "D";
                                     }
                                     oGL.FeeType = oRowView["FeeType"].ToString().Trim();
                                     oGL.Desciption = "REVERSAL Of " + oRowView["Description"].ToString().Trim();
                                     oGL.TransType = oRowView["TransType"].ToString().Trim();
                                     oGL.SysRef = oRowView["SysRef"].ToString().Trim();
                                     oGL.Ref01 = oRowView["Ref01"].ToString().Trim();
                                     oGL.Ref02 = oRowView["Ref02"].ToString().Trim();
                                     oGL.AcctRef = oRowView["AcctRef"].ToString().Trim();
                                     oGL.AcctRefSecond = oRowView["AcctRefSecond"].ToString().Trim();
                                     oGL.RecAcctMaster = oRowView["RecAcctMas"].ToString().Trim();
                                     oGL.RecAcct = oRowView["RecAcctSub"].ToString().Trim();
                                     oGL.Reverse = "Y";
                                     oGL.Jnumber = strJnumberNext;
                                     oGL.Chqno = oRowView["Chqno"].ToString().Trim();
                                     oGL.InstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), oRowView["InstrumentType"].ToString().Trim(), false); 
                                     oGL.Branch = oRowView["Branch"].ToString().Trim();
                                     oGL.FeeType = "";
                                     var dbCommandGLTrans = oGL.AddCommand();
                                     db.ExecuteNonQuery(dbCommandGLTrans, transaction);
                                 }
                                 var dbCommandGLTransUpdate = oGLGetRec.UpdateGLBySysRefReversalCommand();
                                 db.ExecuteNonQuery(dbCommandGLTransUpdate, transaction);

                                 oCustOBal.Reversed = true;
                                 var dbCommandCustOBal = oCustOBal.UpDateRevCommand();
                                 db.ExecuteNonQuery(dbCommandCustOBal, transaction);
                                 transaction.Commit();
                             }
                         }
                         catch (Exception errmsg)
                         {
                             transaction.Rollback();
                             return ResponseResult.Error("Error Reversing Customer Account Opening Balance Posting");
                         }
                         connection.Close();
                     }
                 }
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message,ex);
                }
                return ResponseResult.Error("Opening Balance cannot be reversed.");
            });
        }

    }
}