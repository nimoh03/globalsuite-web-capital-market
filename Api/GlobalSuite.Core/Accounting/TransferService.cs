using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public async Task<ResponseResult> CreateTransfer(CustomerTransfer oCustomerTransfer)
        {
            return await Task.Run(() =>
            {
                var oUserProfile = new UserProfile
                {
                    UserName = GeneralFunc.UserName
                };
                if (!oUserProfile.GetUserProfile()) return ResponseResult.Error("Cannot retrieve user profile.");
                if (!oCustomerTransfer.ChkCustomerAccountIsFunded(oCustomerTransfer.RProduct,oCustomerTransfer.RCustAID, 
                        oCustomerTransfer.Amount))
                {
                    if (!oUserProfile.OverDrawAcct)
                       return ResponseResult.Error("Cannot Save CustomerTransfer.Customer Account Does Not Have Enough Fund");
                    return ResponseResult.Error( "Cannot Save CustomerTransfer.Customer Account Does Not Have Enough Fund! Do You Still Want To Effect This Payment");
                }
                // var oAccount = new Account
                // {
                //     AccountId = oCustomerTransfer.RCustAID
                // };
                oCustomerTransfer.EffDate=DateTime.Now;
                oCustomerTransfer.Posted = false;
                oCustomerTransfer.Reversed = false;
                oCustomerTransfer.SaveType = Constants.SaveType.ADDS;
                var oSaveStatus = oCustomerTransfer.Save();
                if (oSaveStatus == DataGeneral.SaveStatus.Saved) return ResponseResult.Success();
                if (oSaveStatus == DataGeneral.SaveStatus.NotExist)
                    return ResponseResult.Error("CCannot Edit Opening Balance Posting Does Not Exist");
                if (oSaveStatus == DataGeneral.SaveStatus.DuplicateRef)
                    return ResponseResult.Error("Cannot Save,Duplicate Reference Number");
                if (oSaveStatus == DataGeneral.SaveStatus.NotSaved)
                    return ResponseResult.Error("Error In Saving");
                return ResponseResult.Error();

            });
        }

        public async Task<ResponseResult> PostTransfer(string code)
        {
            return await Task.Run(() =>
            {
                var oCustomer = new Customer();
                var oAcctGL = new AcctGL();
                var oCustomerTransfer = new CustomerTransfer
                {
                    TransNo = code
                };

                if (!oCustomerTransfer.GetCustomerTransfer(DataGeneral.PostStatus.UnPosted))
                    return ResponseResult.Error("Cannot Post! This Customer Transfer Does Not Exist or Already Posted/Reversed,Try Saving");
                var oUserProfile = new UserProfile
                {
                    UserName = GeneralFunc.UserName
                };
                if (oUserProfile.GetUserProfile()) { }
                if (!oCustomerTransfer.ChkCustomerAccountIsFunded(oCustomerTransfer.RProduct, oCustomerTransfer.RCustAID,oCustomerTransfer.Amount))
                {
                    return ResponseResult.Error(!oUserProfile.OverDrawAcct ?
                        "Cannot Post CustomerTransfer.Customer Account Does Not Have Enough Fund" :
                        "Cannot Post CustomerTransfer.Customer Account Does Not Have Enough Fund! Do You Still Want To Post This CustomerTransfer");
                }
                var oProduct = new Product
                {
                    TransNo = oCustomerTransfer.TProduct
                };
                var oProductFrom = new Product
                {
                    TransNo = oCustomerTransfer.RProduct
                };

                oAcctGL.EffectiveDate = oCustomerTransfer.EffDate.ToExact();
                oAcctGL.MasterID = oProduct.GetProductGLAcct();
                oAcctGL.AccountID = oCustomerTransfer.TCustAID.Trim();
                oAcctGL.AcctRef = oCustomerTransfer.TProduct;
                oAcctGL.Credit = oCustomerTransfer.Amount;
                oAcctGL.Desciption = oCustomerTransfer.Description.Trim();
                oAcctGL.Description2 = oCustomerTransfer.Description.Trim();
                oAcctGL.TransType = "CUSTTRF";
                oAcctGL.SysRef = "CUTRF" + "-" + oCustomerTransfer.TransNo.ToString().Trim();
                oAcctGL.Ref01 = oCustomerTransfer.TransNo;
                oAcctGL.Ref02 = oCustomerTransfer.TCustAID;
                oAcctGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                
                oAcctGL.RecAcct = oCustomerTransfer.RCustAID;
                oAcctGL.RecAcctMaster = oProductFrom.GetProductGLAcct();
                oAcctGL.AcctRefSecond = oCustomerTransfer.RProduct;
                oCustomer.CustAID = oCustomerTransfer.TCustAID;
                oCustomer.GetCustomerName(oCustomerTransfer.TProduct);
                oAcctGL.Branch = oCustomer.Branch;
                oAcctGL.Reverse = "N";
                oAcctGL.FeeType = "";
                oAcctGL.Chqno = "";
                return oAcctGL.Post("CUSTTRANSFER") ? ResponseResult.Success( "Customer Transfer Posted Successfully!") : ResponseResult.Error();
            });
        }
        
        public async Task<ResponseResult> ReverseTransfer(string code)
        {

            return await Task.Run(() =>
            {
                try
                {
                    string strJnumberNext = "";
                CustomerTransfer oCustomerTransfer = new CustomerTransfer();
                AcctGL oGL = new AcctGL();
                AcctGL oGLGetRec = new AcctGL();
                oCustomerTransfer.TransNo = code.Trim();
                if (oCustomerTransfer.GetCustomerTransfer(DataGeneral.PostStatus.Posted))
                {
                    DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();

                        try
                        {
                            oGLGetRec.SysRef = "CUTRF" + "-" + code.Trim();

                            if (oGLGetRec.GetGLBySysRefNonRev().Tables[0].Rows.Count >= 1)
                            {
                                SqlCommand oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                                db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                                db.ExecuteNonQuery(oCommandJnumber, transaction);
                                strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                                foreach (DataRow oRowView in oGLGetRec.GetGLBySysRefNonRev().Tables[0].Rows)
                                {
                                    oGL.EffectiveDate = oRowView["EffDate"].ToString().ToDate();
                                    oGL.AccountID = oRowView["AccountID"].ToString();
                                    oGL.MasterID = oRowView["MasterID"].ToString();

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
                                    
                                    oGL.InstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), oRowView["InstrumentType"].ToString().Trim(), false); 
                                    
                                    oGL.Chqno = oRowView["Chqno"].ToString().Trim();
                                    
                                    
                                    oGL.Branch = oRowView["Branch"].ToString().Trim();
                                    
                                    oGL.FeeType = "";
                                    SqlCommand dbCommandGLTrans = oGL.AddCommand();
                                    db.ExecuteNonQuery(dbCommandGLTrans, transaction);
                                }
                                SqlCommand dbCommandGLTransUpdate = oGLGetRec.UpdateGLBySysRefReversalCommand();
                                db.ExecuteNonQuery(dbCommandGLTransUpdate, transaction);

                                oCustomerTransfer.Reversed = true;
                                SqlCommand dbCommandCustomerTransfer = oCustomerTransfer.UpDateRevCommand();
                                db.ExecuteNonQuery(dbCommandCustomerTransfer, transaction);
                                transaction.Commit();
                                return ResponseResult.Success();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message,ex);
                            transaction.Rollback();
                           return ResponseResult.Error("Error Reversing Customer CustomerTransfer Posting " + ex.Message);
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

        public async Task<List<TransferResponse>> GetTransfers(TransferFilter filter)
        {
            return await Task.Run(() =>
            {
                var oCustomerTransfer = new CustomerTransfer();
                filter = filter ?? new TransferFilter();
                if (filter.Status.ToLower() == "all") filter.Status = DataGeneral.PostStatus.Posted.ToString();
                Enum.TryParse(filter.Status, true, out DataGeneral.PostStatus postStatus);
                DataSet ds;
                if (filter.StartDate.HasValue && filter.EndDate.HasValue&& postStatus!=DataGeneral.PostStatus.All)
                {
                    oCustomerTransfer.EffDate = filter.StartDate.Value.ToExact();
                    oCustomerTransfer.EffDateTo = filter.EndDate.Value.ToExact();
                    ds = oCustomerTransfer.GetAllGivenEntryDate(postStatus);
                }
                else
                {
                    ds=   oCustomerTransfer.GetAll(postStatus);
                }
                var dt = ds.Tables[0].AsEnumerable().Skip(filter.Skip).Take(filter.PageSize);

                return dt.Select(row => FormatTransfer(row.GetItem<TransferResponse>())).ToList();
            });
        }

        public async Task<TransferResponse> GetTransfer(string code, DataGeneral.PostStatus status)
        {
            return await Task.Run(() =>
            {
                var oCustomerTransfer = new CustomerTransfer()
                {
                    TransNo = code.Trim()
                };
                var isSuccess = oCustomerTransfer.GetCustomerTransfer(status);
                if (!isSuccess) return null;
                var p = Mapper.Map<TransferResponse>(oCustomerTransfer);
                 
               
                return FormatTransfer(p);
            });
        }
        private TransferResponse FormatTransfer(TransferResponse transfer)
        {
            var rCustomer = new Customer(){CustAID = transfer.RCustAID};
            rCustomer.GetCustomer();
            transfer.RCustomer = Mapper.Map<PublicCustomer>(rCustomer); 
            var tCustomer = new Customer(){CustAID = transfer.TCustAID};
            tCustomer.GetCustomer();
            transfer.TCustomer = Mapper.Map<PublicCustomer>(tCustomer);
            var rProduct = new Product
            {
                TransNo = transfer.RProduct
            };
            rProduct.GetProduct();
            transfer.FromProductDetail = Mapper.Map<ProductDetailResponse>(rProduct);
            var tProduct = new Product
            {
                TransNo = transfer.TProduct
            };
            tProduct.GetProduct();
            transfer.ToProductDetail = Mapper.Map<ProductDetailResponse>(tProduct);
            
            var oBranch = new Branch
            {
                TransNo = rCustomer.Branch
            };
            oBranch.GetBranch();
            transfer.BranchDetail = Mapper.Map<BranchResponse>(oBranch);
            
            return transfer;
        }
    }
}