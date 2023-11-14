using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using GlobalSuite.Core.Exceptions;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using DataTableExtensions = GlobalSuite.Core.Helpers.DataTableExtensions;

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<List<DepositResponse>> GetDeposits(DepositFilter filter)
        {
            try
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        var oDeposit = new Deposit();
                        filter = filter ?? new DepositFilter(); 
                        if (filter.Status.ToLower() == "all") filter.Status = DataGeneral.PostStatus.Posted.ToString();
                        Enum.TryParse(filter.Status, true, out DataGeneral.PostStatus postStatus);
                        DataSet ds;
                        if (filter.StartDate.HasValue && filter.EndDate.HasValue&& postStatus!=DataGeneral.PostStatus.All)
                        {
                            oDeposit.EDate = filter.StartDate.Value.ToExact();
                            oDeposit.EDateTo = filter.EndDate.Value.ToExact();
                            ds = oDeposit.GetAllGivenEntryDate(postStatus);
                        }
                        else
                        {
                         ds=   oDeposit.GetAll(postStatus);
                        }

                        var dt = ds.Tables[0].AsEnumerable().Skip(filter.Skip).Take(filter.PageSize);
                        
                        return dt.Select(row => FormatDeposit(row.GetItem<DepositResponse>())).ToList();
                       
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        throw new AppException(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            return new List<DepositResponse>();
        }

        public async Task<ResponseResult> DeleteDeposit(string code)
        {
            await Task.Run(() =>
            {
                try
                {
                    var oDeposit = new Deposit
                    {
                        Code = code
                    };
                    if (!oDeposit.GetDeposit(DataGeneral.PostStatus.UnPosted))
                        return ResponseResult.Error("Customer Deposit Does Not Exist");
                    return oDeposit.Delete() ? ResponseResult.Success() : ResponseResult.Error("Error In Deleting Customer Deposit");
                }
                catch (Exception ex)
                {
                   Logger.Error(ex.Message, ex);
                   return ResponseResult.Error(ex.Message);
                }
            });
            return ResponseResult.Error();
        }

        public async Task<ResponseResult> EditDeposit(Deposit oDeposit)
        {
            try
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        return AddOrEditDeposit(oDeposit, Constants.SaveType.EDIT);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        return ResponseResult.Error("Deposit cannot be saved.");
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseResult.Error(ex.Message);
            }
        }

        public async Task<ResponseResult> Deposit(Deposit oDeposit)
        {
            try
            {
            return await Task.Run(() =>
            {
                try
                {
                    return AddOrEditDeposit(oDeposit, Constants.SaveType.ADDS);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error("Deposit cannot be saved.");
                }
            });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseResult.Error(ex.Message);
            }
        }

        private ResponseResult AddOrEditDeposit(Deposit oDeposit, string saveMode)
        {
            var oBucket = new Bucket();
                    var oAcctGL = new AcctGL();
                var oNumWord = new NumberToWords();
                oDeposit.Amtword = oNumWord.ConvertToWord($"{oDeposit.Amount}");
                if (saveMode == Constants.SaveType.ADDS)
                {
                                    oDeposit.Code=oBucket.GetNextBucketNoNonIdentity(Constants.TableName.DEPOSIT)
                                        .PadLeft(9, char.Parse("0"));
                oDeposit.RecNo = oAcctGL.GetNextReceiptNo().ToString();

                }
                oDeposit.Posted = false;
                oDeposit.Reversed = false;
                oDeposit.TransNoRev = string.Empty;
                oDeposit.SaveType = saveMode;
                var oSaveStatus = oDeposit.Save();
                switch (oSaveStatus)
                {
                    case DataGeneral.SaveStatus.Saved:
                        return ResponseResult.Success(Mapper.Map<DepositResponse>(oDeposit));
                    case DataGeneral.SaveStatus.NotExist:
                        return ResponseResult.Error( "Cannot Edit Customer Deposit Posting Does Not Exist");
                    case DataGeneral.SaveStatus.DuplicateRef:
                    {
                        string strExistTransactionDetail;
                        var oDepositExist = new Deposit
                        {
                            Code = oDeposit.ExistingCodeNumber
                        };
                        if (oDepositExist.GetDeposit(DataGeneral.PostStatus.All))
                        {
                            strExistTransactionDetail = "Trans. No.: " + oDepositExist.Code.Trim() +
                                                        " Date: " + oDepositExist.RNDate.ToString("d").Trim() +
                                                        " Amount: " + oDepositExist.Amount.ToString("n").Trim() +
                                                        " Description: " + oDepositExist.TransDesc.Trim();
                        }
                        else
                        {
                            strExistTransactionDetail = "";
                        }
                        return ResponseResult.Error("Cannot Save,Duplicate Reference Number with details " + strExistTransactionDetail);
                    }
                    case DataGeneral.SaveStatus.HeadOfficeExistEdit:
                        return ResponseResult.Error("Cannot Save,Duplicate Cheque Number");
                    default:
                        return oSaveStatus == DataGeneral.SaveStatus.NotSaved ? ResponseResult.Error( "Error In Saving") : ResponseResult.Error();
                }
        }

        public async Task<ResponseResult> PostDeposit(string code)
        {
            return await Task.Run(() =>
            {
                
                var oAccount = new Account();
                var oAcctGl = new AcctGL();
                var oDep = new Deposit
                {
                    Code = code.Trim()
                };
                 if (!oDep.GetDeposit(DataGeneral.PostStatus.UnPosted))
                    return ResponseResult.Error("Cannot Post! This Customer Deposit Does Not Exist or Already Posted/Reversed,Try Saving");
           
                var oProduct = new Product
                {
                    TransNo = oDep.AcctMasBank
                };

                oAcctGl.EffectiveDate = oDep.RNDate.ToExact();
                oAcctGl.MasterID = oProduct.GetProductGLAcct();
                oAcctGl.AccountID = oDep.Custno.Trim();
                oAcctGl.Credit = oDep.Amount;
                var oCustomer = new Customer
                {
                    CustAID = oDep.Custno.Trim()
                };

                oAcctGl.Desciption = oDep.TransDesc.Trim() + " RcNo-A: " + oDep.RecNo.Trim() + (oDep.Ref.Trim() != "" ? " RcNo-M: " + oDep.Ref.Trim() : "") + (oDep.ChqueNo.Trim() != "" ? " Chq No: " + oDep.ChqueNo.Trim() : "");
                if (oCustomer.GetCustomerName(oProduct.TransNo))
                {
                    oAcctGl.Description2 = oDep.TransDesc.Trim() + " RcNo-A: " + oDep.RecNo.Trim() + (oDep.Ref.Trim() != "" ? " RcNo-M: " + oDep.Ref.Trim() : "") + (oDep.ChqueNo.Trim() != "" ? " Chq No: " + oDep.ChqueNo.Trim() : "") + " : " + oCustomer.CombineName().Trim();
                    oAcctGl.CustomerName = oCustomer.CombineName().Trim();
                }
                else
                {
                    oAcctGl.Description2 = oDep.TransDesc.Trim() + " RcNo-A: " + oDep.RecNo.Trim() + (oDep.Ref.Trim() != "" ? " RcNo-M: " + oDep.Ref.Trim() : "") + (oDep.ChqueNo.Trim() != "" ? " Chq No: " + oDep.ChqueNo.Trim() : "");
                    oAcctGl.CustomerName = "";
                }
                oAcctGl.TransType = "CUSTREC";
                oAcctGl.SysRef = "RCD" + "-" + oDep.Code.ToString().Trim();
                oAcctGl.Ref01 = oDep.Code;
                oAcctGl.AcctRef = oDep.AcctMasBank;
                oAcctGl.Chqno = oDep.ChqueNo.Trim();
                oAcctGl.Ref02 = oDep.Custno;
                oAcctGl.InstrumentType = oDep.InstrumentType;
                oAcctGl.RecAcct = "";
                oAcctGl.RecAcctMaster = oDep.AcctSubBank;
                oAcctGl.AcctRefSecond = "";
                oAcctGl.Branch = oAccount.GetAccountBranch(oDep.AcctSubBank);
                oAcctGl.Reverse = "N";
                oAcctGl.FeeType = "";
                var isPosted = oAcctGl.Post("DEPOSIT");
                return isPosted ? ResponseResult.Success("Customer Deposit cannot be posted.") : ResponseResult.Success();
                
            });
        } 
        public async Task<ResponseResult> ReverseDeposit(string code)
        {
            return await Task.Run(() =>
            {
                
                var strJnumberNext = "";
                var oDep = new Deposit
                {
                    Code = code
                };
                var oGl = new AcctGL();
                var oGlGetRec = new AcctGL();
                if (!oDep.GetDeposit(DataGeneral.PostStatus.Posted))
                    return ResponseResult.Success("Customer Deposit cannot be posted.");
                var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                using (var connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    try
                    {
                        oGlGetRec.SysRef = "RCD" + "-" + code;

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
                                switch (oRowView["Debcred"].ToString().Trim())
                                {
                                    case "D":
                                        oGl.Credit = decimal.Parse(oRowView["Debit"].ToString().Trim());
                                        oGl.Debit = 0;
                                        oGl.Debcred = "C";
                                        break;
                                    case "C":
                                        oGl.Credit = 0;
                                        oGl.Debit = decimal.Parse(oRowView["Credit"].ToString().Trim());
                                        oGl.Debcred = "D";
                                        break;
                                }
                                oGl.Desciption = "REVERSAL Of " + oRowView["Description"].ToString().Trim();
                                oGl.TransType = oRowView["TransType"].ToString().Trim();
                                oGl.SysRef = oRowView["SysRef"].ToString().Trim();
                                oGl.Ref01 = oRowView["Ref01"].ToString().Trim();
                                oGl.Ref02 = oRowView["Ref02"].ToString().Trim();
                                oGl.AcctRef = oRowView["AcctRef"].ToString().Trim();
                                oGl.AcctRefSecond = oRowView["AcctRefSecond"].ToString().Trim();
                                oGl.RecAcctMaster = oRowView["RecAcctMas"].ToString().Trim();
                                oGl.RecAcct = oRowView["RecAcctSub"].ToString().Trim();
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
                            var dbCommandDeposit = oDep.UpDateRevCommand();
                            db.ExecuteNonQuery(dbCommandDeposit, transaction);
                            transaction.Commit();
                            return ResponseResult.Success();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        transaction.Rollback();
                        return ResponseResult.Error("Error Reversing Customer Deposit Posting " + ex.Message);
                    }
                    connection.Close();
                }

                return ResponseResult.Success("Customer Deposit cannot be posted.");
            });
        }
        
        public async Task<DepositResponse> GetDeposit(string code)
        {
            return await Task.Run(() =>
            {
                var oDeposit = new Deposit
                {
                    Code = code.Trim()
                };
               
                var isSuccess = oDeposit.GetDeposit(DataGeneral.PostStatus.All);


                var response = FormatDeposit(Mapper.Map<DepositResponse>(oDeposit));
                response.ReceiptNo = oDeposit.Code;
                response.TranDesc = oDeposit.TransDesc;
                
                return isSuccess ? response : null;
            });
        }

        private DepositResponse FormatDeposit(DepositResponse oDeposit)
        {
            var response = Mapper.Map<DepositResponse>(oDeposit);
             
                var oCustomer = new Customer{CustAID = oDeposit.Custno};
                     oCustomer.GetCustomer();
                     response.Customer = Mapper.Map<PublicCustomer>(oCustomer);
            
            var oAcctGl = new AcctGL
            {
                AccountID = oDeposit.Custno,
                AcctRef = oDeposit.AcctMasBank
            };
            response.Balance=   oAcctGl.GetAccountBalanceByCustomer();
            Enum.TryParse(oDeposit.InstrumentType, true, out DataGeneral.GLInstrumentType instrumentType);
            response.InstrumentType = EnumExtensions.GetDescription(instrumentType);
            response.InstrumentTypeId = (int)instrumentType;
            var oProduct = new Product
            {
                TransNo = oDeposit.AcctMasBank
            };
            oProduct.GetProduct();
            response.ProductDetail = Mapper.Map<ProductDetailResponse>(oProduct);
            var oBranch = new Branch
            {
                TransNo = oCustomer.Branch
            };
            oBranch.GetBranch();
            response.Branch = Mapper.Map<BranchResponse>(oBranch);
            var oAcct = new Account
            {
                AccountId = oDeposit.AcctSubBank
            };
            oAcct.GetAccount(oCustomer.Branch);
            response.SubBank = new ChartOfAccountResponse
            {
                AccountId = oAcct.AccountId,
                AccountName = oAcct.AccountName
            };

            return response;
        }
    }
    
}