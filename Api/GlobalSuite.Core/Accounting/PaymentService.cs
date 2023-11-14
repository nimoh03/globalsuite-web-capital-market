using System;
using System.Collections.Generic;
using System.Data;
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

namespace GlobalSuite.Core.Accounting
{
    public partial class AccountingService
    {
        public async Task<List<PaymentResponse>> GetPayments(PaymentFilter filter)
        {
            return await Task.Run(() =>
            {
                var oPayment = new Payment();
                filter = filter ?? new PaymentFilter();
                Enum.TryParse(filter.Status, true, out DataGeneral.PostStatus postStatus);
                DataSet ds;
                if (filter.StartDate.HasValue && filter.EndDate.HasValue&& postStatus!=DataGeneral.PostStatus.All)
                {
                    oPayment.EDate = filter.StartDate.Value.ToExact();
                    oPayment.EDateTo = filter.EndDate.Value.ToExact();
                    ds = oPayment.GetAllGivenEntryDate(postStatus);
                }
                else
                {
                    ds=   oPayment.GetAll(postStatus);
                }
                var dt = ds.Tables[0].AsEnumerable().Skip(filter.Skip).Take(filter.PageSize);

                return dt.Select(row => FormatPayment(row.GetItem<PaymentResponse>())).ToList();
            });
        }
        public async Task<PaymentResponse> GetPayment(string code, DataGeneral.PostStatus postStatus)
        {
            return await Task.Run(() =>
            {
                var oPayment = new Payment
                {
                    Code = code.Trim()
                };
                var isSuccess = oPayment.GetPayment(postStatus);
                if (!isSuccess) return null;
                var p = Mapper.Map<PaymentResponse>(oPayment);
                p.PaymentNo = oPayment.Code;
               
                return FormatPayment(p);
            });
        }
        private PaymentResponse FormatPayment(PaymentResponse payment)
        {
            var oCustomer = new Customer{CustAID = payment.Custno};
            oCustomer.GetCustomer();
            payment.Customer = Mapper.Map<PublicCustomer>(oCustomer);
            if (!string.IsNullOrWhiteSpace(payment.AcctMasBank))
            {
                var oProduct = new Product
                {
                    TransNo = payment.AcctMasBank
                };
                oProduct.GetProduct();
                payment.AccoutMasBank = Mapper.Map<ProductDetailResponse>(oProduct);
            }

            if (!string.IsNullOrWhiteSpace(oCustomer.Branch))
            {
                var oBranch = new Branch
                {
                    TransNo = oCustomer.Branch
                };
                payment.Branch = oCustomer.Branch;
                oBranch.GetBranch();
                payment.BranchDetail = Mapper.Map<BranchResponse>(oBranch);
            }

            if (!string.IsNullOrWhiteSpace(payment.AcctSubBank))
            {
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
            }

            
            Enum.TryParse(payment.InstrumentType, true, out DataGeneral.GLInstrumentType instrumentType);
            payment.InstrumentType = EnumExtensions.GetDescription(instrumentType);
            payment.InstrumentTypeId = (int)instrumentType;
            return payment;
        }

        public async Task<ResponseResult> CreatePayment(Payment oPayment)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return AddOrEditPayment(oPayment, Constants.SaveType.ADDS);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error(ex.Message);
                }

            });
        }

        public async Task<ResponseResult> EditPayment(Payment oPayment)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return AddOrEditPayment(oPayment, Constants.SaveType.ADDS);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error(ex.Message);
                }

            });
        }

        private ResponseResult AddOrEditPayment(Payment oPayment, string saveMode)
        {
            var oBucket = new Bucket();
                    var oNumWord = new NumberToWords();
                    oPayment.Amtword = oNumWord.ConvertToWord($"{oPayment.Amount}");
                    if(saveMode==Constants.SaveType.ADDS)
                    oPayment.Code=oBucket.GetNextBucketNoNonIdentity(Constants.TableName.PAYMENT)
                        .PadLeft(9, char.Parse("0"));
                    var oUserProfile = new UserProfile
                    {
                        UserName = GeneralFunc.UserName
                    };
                    if (!oUserProfile.GetUserProfile()) return ResponseResult.Error("Cannot retrieve user profile.");
                    if (!oPayment.ChkCustomerAccountIsFunded(oPayment.AcctMasBank,oPayment.Custno, 
                            oPayment.Amount))
                    {
                        if (!oUserProfile.OverDrawAcct)
                            return ResponseResult.Error("Cannot Save Payment.Customer Account Does Not Have Enough Fund");
                        return ResponseResult.Error( "Cannot Save Payment.Customer Account Does Not Have Enough Fund! Do You Still Want To Effect This Payment");
                    }
                 
                    oPayment.Amtword = oNumWord.ConvertToWord($"{oPayment.Amount}");
                    oPayment.Posted = false;
                    oPayment.Reversed = false;
                    oPayment.TransNoRev = string.Empty;
                    oPayment.SaveType = saveMode;
                    var oSaveStatus = oPayment.Save();
                    if (oSaveStatus == DataGeneral.SaveStatus.Saved) return ResponseResult.Success(Mapper.Map<PaymentResponse>(oPayment));
                    if (oSaveStatus == DataGeneral.SaveStatus.NotExist)
                        return ResponseResult.Error("Cannot Edit Customer Payment Posting Does Not Exist");
                    if (oSaveStatus == DataGeneral.SaveStatus.DuplicateRef)
                        return ResponseResult.Error("Cannot Save,Duplicate Reference Number");
                    if (oSaveStatus == DataGeneral.SaveStatus.HeadOfficeExistEdit)
                        return ResponseResult.Error("Cannot Save,Duplicate Cheque Number");
                    if (oSaveStatus == DataGeneral.SaveStatus.NotSaved)
                        return ResponseResult.Error("Error In Saving");
                    return ResponseResult.Error();
        }

        public async Task<ResponseResult> PostPayment(string code)
        {
            return await Task.Run(() =>
            {
                 
                var oAccount = new Account();
                var oAcctGl = new AcctGL();
                var oPayment = new Payment
                {
                    Code = code
                };
                if (!oPayment.GetPayment(DataGeneral.PostStatus.UnPosted))
                    return ResponseResult.Error("Cannot Post! This Customer Payment Does Not Exist or Already Posted/Reversed,Try Saving");
                var oUserProfile = new UserProfile
                {
                    UserName = GeneralFunc.UserName
                };
                if (oUserProfile.GetUserProfile()) { }
                string strRealProduct;
                 
                if (!oPayment.ChkCustomerAccountIsFunded(oPayment.AcctMasBank, oPayment.Custno,oPayment.Amount))
                {
                    return ResponseResult.Error(!oUserProfile.OverDrawAcct ?
                        "Cannot Post Payment.Customer Account Does Not Have Enough Fund" :
                        "Cannot Post Payment.Customer Account Does Not Have Enough Fund! Do You Still Want To Post This Payment");
                }
                var oProduct = new Product
                {
                    TransNo = oPayment.AcctMasBank
                };
                oAcctGl.EffectiveDate = oPayment.RNDate;
                oAcctGl.MasterID = oPayment.AcctSubBank;
                oAcctGl.AccountID = "";
                oAcctGl.Credit = oPayment.Amount;
                var oCustomer = new Customer
                {
                    CustAID = oPayment.Custno.Trim()
                };
                oAcctGl.Desciption = oPayment.TransDesc.Trim() + " PVNo-A: " + oPayment.PVNo.Trim() + (oPayment.Ref.Trim() != "" ? " PVNo-M: " + oPayment.Ref.Trim() : "") + (oPayment.ChqueNo.Trim() != "" ? " Chq No: " + oPayment.ChqueNo.Trim() : "");       
                if (oCustomer.GetCustomerName(oProduct.TransNo))
                {
                    oAcctGl.Description2 = oPayment.TransDesc.Trim() + " PVNo-A: " + oPayment.PVNo.Trim() + (oPayment.Ref.Trim() != "" ? " PVNo-M: " + oPayment.Ref.Trim() : "") + (oPayment.ChqueNo.Trim() != "" ? " Chq No: " + oPayment.ChqueNo.Trim() : "") + " : " + oCustomer.CombineName().Trim();
                    oAcctGl.CustomerName = oCustomer.CombineName().Trim();
                }
                else
                {
                    oAcctGl.Description2 = oPayment.TransDesc.Trim() + " PVNo-A: " + oPayment.PVNo.Trim() + (oPayment.Ref.Trim() != "" ? " PVNo-M: " + oPayment.Ref.Trim() : "") + (oPayment.ChqueNo.Trim() != "" ? " Chq No: " + oPayment.ChqueNo.Trim() : "") ;
                    oAcctGl.CustomerName = "";
                }
                oAcctGl.TransType = "CUSTINV";
                oAcctGl.SysRef = "PVC" + "-" + oPayment.Code;
                oAcctGl.Ref01 = oPayment.Code;
                oAcctGl.AcctRef = "";
                oAcctGl.Chqno = oPayment.ChqueNo.Trim();
                oAcctGl.Ref02 = oPayment.Custno;
                oAcctGl.InstrumentType = oPayment.InstrumentType;
                oAcctGl.RecAcct = oPayment.Custno;
                oAcctGl.RecAcctMaster = oProduct.GetProductGLAcct();
                oAcctGl.AcctRefSecond = oPayment.AcctMasBank;
                oAcctGl.Reverse = "N";
                oAcctGl.FeeType = "";
                oAcctGl.Branch = oAccount.GetAccountBranch(oPayment.AcctSubBank);
                return oAcctGl.Post("PAYMENT") ? ResponseResult.Success( "Customer Payment Posted Successfully!") : ResponseResult.Error();
            });
        }
        
       
    }
}