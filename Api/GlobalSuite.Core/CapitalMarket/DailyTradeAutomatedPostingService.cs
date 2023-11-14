using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Admin.Business;
using BaseUtility.Business;
using CapitalMarket.Business;
using CustomerManagement.Business;
using GL.Business;
using GlobalSuite.Core.Helpers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GlobalSuite.Core.CapitalMarket
{
    public partial class TradingService
    {
        private const string BaseFolder = "GlobalSuiteFolder";
        public async Task<ResponseResult> DailyTradePosting(DateTime tradeDate, string filePath)
        {
          return  await Task.Run(() =>
            {
                var oBranch = new Branch();

                #region Check Time Is Within Market Period
                var oGlParamTradeBookAddGloSuite = new GLParam
                {
                    Type = Constants.ParamTable.TRADEBOOKADDTOGLOBALSUITE
                };
                var strTradeBookAddGloSuite = oGlParamTradeBookAddGloSuite.CheckParameter();
                var intTimeHour = GeneralFunc.GetTodayTimeHour();
                if (strTradeBookAddGloSuite.Trim() == "YES")
                {
                    if (intTimeHour >= 9 && intTimeHour < 15)
                        return ResponseResult.Error("Cannot Upload/Post Allotment Trades For The Day Yet! Time Within Market Trading Time of 9AM - 3PM");
                }
                #endregion

                #region Try Save
                try
                {
                    #region Declaration of Property
                    //IFormatProvider format = new CultureInfo("en-GB");
                    //Use this Instead of Cutting Date Returned From Database SubString(0,10)
                    var dtfi = new DateTimeFormatInfo
                    {
                        ShortDatePattern = "dd/MM/yyyy"
                    };
                    var strAllotmentNo = "";
                    var strJnumberNext = "";
                    decimal decAllotConsideration = 0;
                    var strUserName = GeneralFunc.UserName;
                    var oCompany = new Company();
                    var companyCode = oCompany.MemberCode;
                    var oFile = new FileHandler();
                    var strDefaultBranchCode = oBranch.DefaultBranch;
                    #endregion

                    //Read File
                    #region Loading from Trade File to Database
                    var blnFileTradeResult = oFile.ReadTextFile(filePath, out var oCustMissings, out var oStockMissings, out var oBrokerMissings);
                    #endregion

                    ResponseResult result;
                    #region Missing Customer,Stock,Broker
                    if (!blnFileTradeResult)
                    {
                        result = HandleCustomerMissing(companyCode, oCustMissings);
                        if (!result.IsSuccess) return result;
                        result = HandleStockMissing(companyCode, oStockMissings);
                        if (!result.IsSuccess) return result;
                        result = HandleBrokerMissing(companyCode, oBrokerMissings);
                        if (!result.IsSuccess) return result;
                    }
                    #endregion

                    #region Select BarginSlip/Trades Record And Start Checking After Loading From Trade File

                    result = HandleBarginSlip(tradeDate, strUserName, companyCode, out var datDiskTrans);
                    if (!result.IsSuccess) return result;
                    #endregion
                    // Start Processing

                    #region Declaration And Assisning Of Product Account,Customer,Fee and Allot Variable, Cap Market and pParam Parameter And Others
                    var oCust = new ProductAcct();
                    var oCustomer = new Customer();
                    var oCustCross = new ProductAcct();
                    var oCustomerCross = new Customer();
                    decimal decFeeSec = 0, decFeeStampDuty = 0, decFeeCommission = 0, decFeeVAT = 0, decFeeCSCS = 0, decSMSAlertCSCS = 0, decFeeNSE = 0;
                    decimal decFeeSecVat = 0, decFeeNseVat = 0, decFeeCscsVat = 0, decSMSAlertCSCSVAT = 0, decFeeTotalAmount = 0, decAllotConsiderationBuy = 0;

                    decimal decFeeSecSeller = 0, decFeeStampDutySeller = 0, decFeeCommissionSeller = 0, decFeeVATSeller = 0, decFeeCSCSSeller = 0, decSMSAlertCSCSSeller = 0, decFeeNSESeller = 0;
                    decimal decFeeSecVatSeller = 0, decFeeNseVatSeller = 0, decFeeCscsVatSeller = 0, decSMSAlertCSCSVATSeller = 0;

                    Decimal TotalFeeBuyer = 0;
                    Decimal TotalFeeSeller = 0;
                    Decimal TotalFeeSellerPost = 0;
                    var strAllotmentNo2 = "";

                    var oStkbPGenTable = new StkParam();
                    var oPGenTable = new PGenTable();
                    if (!oPGenTable.GetPGenTable())
                    {
                        return ResponseResult.Success();
                    }
                    if (!oStkbPGenTable.GetStkbPGenTable())
                    {
                        return ResponseResult.Success();
                    }

                    var strPostCommShared = "";
                    decimal decDeductComm;
                    decimal decDeductCommJobbing;
                    var strBuyerTrueId = "";
                    var strBranchBuySellCharge = "";
                    var strPostInvPLControlAcct = "";
                    var strCommissionSeperationInCustomerAccount = "";
                    var strStockProductAccount = oStkbPGenTable.Product;
                    var strInvestmentProductAccount = oStkbPGenTable.ProductInvestment;
                    var strInvestmentProductBondAccount = oStkbPGenTable.ProductInvestmentBond;
                    var strAgentProductAccount = oStkbPGenTable.ProductBrokPay;

                    //Variable to Check Prop Account
                    var oCustChkPropAccount = new ProductAcct();
                    var blnChkPropAccount = false;

                    //Commission To Share Or Not
                    var oGLParam = new GLParam();
                    oGLParam.Type = Constants.ParamTable.POSTCOMMSHARED;
                    strPostCommShared = oGLParam.CheckParameter();

                    //For Box Load To Calculate Profit/Loss On Sale
                    var oGLParamBoxSaleProfit = new GLParam();
                    oGLParamBoxSaleProfit.Type = Constants.ParamTable.CALCPROPGAINORLOSS;

                    //Branch Buy And Sell Charges
                    var oGLParamBranchBuySellCharge = new GLParam();
                    oGLParamBranchBuySellCharge.Type = Constants.ParamTable.BRANCHBUYSELLCHARGESDIFFERENT;
                    strBranchBuySellCharge = oGLParamBranchBuySellCharge.CheckParameter();

                    //Post Investment PL To Control Account
                    var oGLParamPostInvPLControlAcct = new GLParam();
                    oGLParamPostInvPLControlAcct.Type = Constants.ParamTable.POSTINVPLCTRLSECACCT;
                    strPostInvPLControlAcct = oGLParamPostInvPLControlAcct.CheckParameter();

                    //Commission Seperation In Customer Account
                    var oGLParamCommissionSeperationInCustomerAccount = new GLParam();
                    oGLParamCommissionSeperationInCustomerAccount.Type = Constants.ParamTable.COMMISSIONSEPERATED;
                    strCommissionSeperationInCustomerAccount = oGLParamCommissionSeperationInCustomerAccount.CheckParameter();

                    decimal decTotalBank = 0;
                    decimal decTotalBankAudit = 0;
                    decimal decTotalSEC = 0;
                    decimal decTotalCSCS = 0;
                    decimal decTotalNSE = 0;
                    decimal decTotalCommission = 0;
                    decimal decTotalCommissionBuy = 0;
                    decimal decTotalCommissionSell = 0;
                    decimal decTotalVAT = 0;
                    decimal decTotalStampDuty = 0;
                    decimal decTotalDeductCommission = 0;
                    decimal decTotalDeductCommissionBuy = 0;
                    decimal decTotalDeductCommissionSell = 0;
                    decimal decCrossDealTotalAmount = 0;
                    decimal decTotalDirectCashSettlement = 0;
                    decimal decTotalDirectCashSettlementDifference = 0;

                    decimal decTotalBankBond = 0;
                    decimal decTotalBankAuditBond = 0;
                    decimal decTotalSECBond = 0;
                    decimal decTotalCSCSBond = 0;
                    decimal decTotalNSEBond = 0;
                    decimal decTotalCommissionBond = 0;
                    decimal decTotalCommissionBuyBond = 0;
                    decimal decTotalCommissionSellBond = 0;
                    decimal decTotalVATBond = 0;
                    decimal decTotalStampDutyBond = 0;
                    decimal decTotalDeductCommissionBond = 0;
                    decimal decTotalDeductCommissionBuyBond = 0;
                    decimal decTotalDeductCommissionSellBond = 0;
                    decimal decTotalDirectCashSettlementBond = 0;
                    decimal decTotalDirectCashSettlementDifferenceBond = 0;

                    decimal decTotalBankProp = 0;
                    decimal decTotalBankAuditProp = 0;
                    decimal decTotalSECProp = 0;
                    decimal decTotalCSCSProp = 0;
                    decimal decTotalNSEProp = 0;
                    decimal decTotalCommissionProp = 0;
                    decimal decTotalCommissionBuyProp = 0;
                    decimal decTotalCommissionSellProp = 0;
                    decimal decTotalVATProp = 0;
                    decimal decTotalStampDutyProp = 0;
                    decimal decTotalDeductCommissionProp = 0;
                    decimal decTotalDeductCommissionBuyProp = 0;
                    decimal decTotalDeductCommissionSellProp = 0;
                    decimal decTotalDirectCashSettlementProp = 0;
                    decimal decTotalDirectCashSettlementDifferenceProp = 0;

                    #endregion

                    #region Update Error Trade Table

                    UpdateErrorTradeTable(tradeDate, datDiskTrans);

                    #endregion

                    #region Try Save to Database

                    var oCustomerExtraInformationDirectCash = new CustomerExtraInformation();
                    var oCustomerExtraInformationDoNotChargeStampDuty = new CustomerExtraInformation();
                    var tabDiskTrans = datDiskTrans.Tables[0];
                    var RecNumber = decimal.Parse(tabDiskTrans.Rows.Count.ToString());
                    var factory = new DatabaseProviderFactory();
                    var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            foreach (DataRow oRowView in tabDiskTrans.Rows)
                            {
                                #region Set Stock Instrument Equity Or Bond, Customer Is Prop And Initialise DeductComm,DeductCommJobbing,BuyerTrueId
                                SetStockInstrument(oRowView, oStkbPGenTable, oCustChkPropAccount, strStockProductAccount);
                                #endregion

                                #region Processing Allotments

                                var oAllot = ProcessAllotment(oRowView, oStkbPGenTable, blnChkPropAccount, oCustomerExtraInformationDoNotChargeStampDuty, strUserName, db, transaction, oCustChkPropAccount, strStockProductAccount, oCustomerExtraInformationDirectCash, ref decTotalBankProp, ref decTotalBankAuditProp, ref decTotalBank, ref decTotalBankAudit, ref decTotalBankBond, ref decTotalBankAuditBond, ref decTotalSECProp, ref decTotalSEC, ref decTotalSECBond, ref decTotalStampDutyProp, ref decTotalStampDuty, ref decTotalStampDutyBond, ref decTotalCommissionProp, ref decTotalCommissionBuyProp, ref decTotalCommission, ref decTotalCommissionBuy, ref decTotalCommissionBond, ref decTotalCommissionBuyBond, ref decTotalVATProp, ref decTotalVAT, ref decTotalVATBond, ref decTotalNSEProp, ref decTotalNSE, ref decTotalNSEBond, ref decTotalCSCSProp, ref decTotalCSCS, ref decTotalCSCSBond, ref strAllotmentNo, ref decTotalCommissionSellProp, ref decTotalCommissionSell, ref decTotalCommissionSellBond, ref decTotalDirectCashSettlementProp, ref decTotalDirectCashSettlement, ref decTotalDirectCashSettlementBond, ref decTotalDirectCashSettlementDifferenceProp, ref decTotalDirectCashSettlementDifference, ref decTotalDirectCashSettlementDifferenceBond, ref decFeeCscsVatSeller);

                                #endregion

                                #region Getting Next Junmber And Check Customer Product For Single And Cross Deal Exist.Resetting Variable for Portfolio Unit Cost

                                GetNextJNumber(db, transaction, oRowView, strBuyerTrueId, oCust, strStockProductAccount,
                                    oCustomer, oCustCross, oAllot, oCustomerCross);

                                #endregion

                                #region GL Posting For 1.Single Buy 2.Single Sell 3.Ordinary Cross Deal Buy 4.Norminal Cross Deal Buy 5.Buyer Cross Deal With Commission Buy 6.Seller Cross Deal With Commission Buy
                                var oGl = new AcctGL();
                                decTotalDeductCommissionBuy = GlPostingForBuy(oAllot, oRowView, oCust, oCustomer, oStkbPGenTable,
                                    strInvestmentProductAccount, strInvestmentProductBondAccount, strStockProductAccount, strCommissionSeperationInCustomerAccount, decFeeTotalAmount, decFeeCommission, strAllotmentNo, oGLParamBoxSaleProfit, strJnumberNext, strDefaultBranchCode, oPGenTable, db, transaction, strPostCommShared, strAgentProductAccount, oCustomerExtraInformationDirectCash, strPostInvPLControlAcct, TotalFeeBuyer, decTotalDeductCommissionBuy, oCustomerCross, strBranchBuySellCharge, ref oGl, ref decTotalDeductCommissionSell, ref decTotalDeductCommission, ref decTotalDeductCommissionBuyBond, ref decTotalDeductCommissionSellBond, ref decTotalDeductCommissionBond);
                                #endregion

                                //This does not solve Cross Type "NB" because it will post 2 transactions, one at the top for purchase and the second below
                                //for sale whereas it should only for one at the top or below, preferably at the top or bottom because all the parameter
                                //are there for bottom unless the total amount takes care of it at the top
                                #region GL Posting For 1.Ordinary Cross Deal Sell 2.Norminal Cross Deal Sell 3.NB Cross Deal 4.NS Cross Deal 2.Buy Cross Deal With Consideration Sell 4.Sell Cross Deal With Consideration Sell
                                decTotalDeductCommissionSell = GlPostingForSell(oAllot, oGl, oRowView, oCust, oCustomer, oStkbPGenTable, strInvestmentProductAccount, strInvestmentProductBondAccount, strStockProductAccount, oGLParamBoxSaleProfit, strCommissionSeperationInCustomerAccount, TotalFeeSeller, TotalFeeSellerPost, TotalFeeBuyer, decFeeCommissionSeller, oCustomerCross, strAllotmentNo2, strAllotmentNo, strJnumberNext, strDefaultBranchCode, oPGenTable, db, transaction, strPostCommShared, strAgentProductAccount, oCustomerExtraInformationDirectCash, strPostInvPLControlAcct, decTotalDeductCommission, ref decTotalDeductCommissionSell, ref decTotalDeductCommissionBond, ref decTotalDeductCommissionSellBond);
                                #endregion

                                #region Portfolio Processing
                                ProcessPortfolio(oRowView, oAllot, decCrossDealTotalAmount, strAllotmentNo, db, transaction, strAllotmentNo2);

                                #endregion

                                #region Job Order Processing

                                ProcessJobOrder(oRowView, oAllot, db, transaction, strAllotmentNo);

                                #endregion





                            }

                            GeneralFunc oGeneralFunc = new GeneralFunc();
                            string strSettlementDate = oGeneralFunc.AddBusinessDay(tradeDate, oGLParam.TradingClearingDay, Holiday.GetAllReturnList()).ToString("d");
                            AcctGL oGLTradBank = new AcctGL();
                            //IMPORTANT- Dont Put Any Value In Ref02 Column For
                            //TransType = TRADBANK Becacause That Is What Is
                            //Used To Differante NGX and NASD When you want To 
                            //Reverse Upload

                            #region Summation Of Fees And Control Account Posting

                            #region Credit Buy/Sell Income,VAT Account And Debit Trading Control 

                            #region Credit Income Buy And Debit Jobbing Control
                            if (decTotalCommissionBuy != 0)
                            {
                                CreditIncomeBuyDebitJobbingControl(tradeDate, oGLTradBank, oPGenTable, decTotalCommissionBuy, decTotalDeductCommissionBuy, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit Income Sell And Debit Jobbing Control
                            if (decTotalCommissionSell != 0)
                            {
                                CreditIncomeSellDebitJobbingControl(tradeDate, oGLTradBank, oPGenTable, decTotalCommissionSell, decTotalDeductCommissionSell, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit Stamp Duty And Debit Jobbing Control
                            //if (decTotalStampDuty != 0)
                            //{
                            //      oGLTradBank.EffectiveDate = tradeDate; 
                            //    oGLTradBank.MasterID = oPGenTable.Bstamp;
                            //    oGLTradBank.AccountID = "";
                            //    oGLTradBank.RecAcct = "";
                            //    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            //    oGLTradBank.Desciption = "Stamp Duty Total Charges For: " + GetFormattedDate(tradeDate);
                            //    oGLTradBank.Credit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                            //    oGLTradBank.Debit = 0;
                            //    oGLTradBank.Debcred = "C";
                            //    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                            //    oGLTradBank.TransType = "TRADBANK";
                            //    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                            //    oGLTradBank.Reverse = "N";
                            //    oGLTradBank.Jnumber = strJnumberNext;
                            //    oGLTradBank.Branch = strDefaultBranchCode;
                            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            //    oGLTradBank.FeeType = "TSTDC";
                            //    SqlCommand dbCommandStampTotalCredit = oGLTradBank.AddCommand();
                            //    db.ExecuteNonQuery(dbCommandStampTotalCredit, transaction);


                            //      oGLTradBank.EffectiveDate = tradeDate; 
                            //    oGLTradBank.MasterID = oPGenTable.ShInv;
                            //    oGLTradBank.AccountID = "";
                            //    oGLTradBank.RecAcct = "";
                            //    oGLTradBank.RecAcctMaster = oPGenTable.Bstamp;
                            //    oGLTradBank.Desciption = "Stamp Duty Total Charges For: " + GetFormattedDate(tradeDate);
                            //    oGLTradBank.Debit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                            //    oGLTradBank.Credit = 0;
                            //    oGLTradBank.Debcred = "D";
                            //    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                            //    oGLTradBank.TransType = "TRADBANK";
                            //    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                            //    oGLTradBank.Reverse = "N";
                            //    oGLTradBank.Jnumber = strJnumberNext;
                            //    oGLTradBank.Branch = strDefaultBranchCode;
                            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            //    oGLTradBank.FeeType = "TSTCS";
                            //    SqlCommand dbCommandStampTotalCreditJobbing = oGLTradBank.AddCommand();
                            //    db.ExecuteNonQuery(dbCommandStampTotalCreditJobbing, transaction);
                            //}
                            #endregion

                            #region Credit VAT and Debit Jobbing Control
                            if (decTotalVAT != 0)
                            {
                                CreditVatDebitJobbingControl(tradeDate, oGLTradBank, oPGenTable, decTotalVAT, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #endregion

                            #region Credit SEC,NSE,CSCS,Stamp Duty Account ONLY

                            #region Credit SEC Account
                            if (decTotalSEC != 0)
                            {
                                CreditSecAccount(tradeDate, oGLTradBank, oPGenTable, decTotalSEC, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit NSE Account
                            if (decTotalNSE != 0)
                            {
                                CreditNseAccount(tradeDate, oGLTradBank, oPGenTable, decTotalNSE, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit CSCS Account
                            if (decTotalCSCS != 0)
                            {
                                CreditCscs(tradeDate, oGLTradBank, oPGenTable, decTotalCSCS, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit Stamp Duty Account
                            if (decTotalStampDuty != 0)
                            {
                                CreditStamDutyAccount(tradeDate, oGLTradBank, oPGenTable, decTotalStampDuty, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion


                            #endregion

                            DebitMarketAndStampDutyAccount(tradeDate, decTotalBank, decTotalNSE, decTotalCSCS, decTotalSEC, decTotalStampDuty, decTotalDirectCashSettlement, oGLTradBank, oPGenTable, strJnumberNext, strDefaultBranchCode, db, transaction, strSettlementDate);

                            #region Posting Direct Cash Settlement Total Amount To Direct Cash Settlement Account 
                            if (decTotalDirectCashSettlement != 0)
                            {
                                PostTotalAmountToDirectCashSettlementAccount(tradeDate, oGLTradBank, oPGenTable, decTotalDirectCashSettlement, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Posting Direct Cash Settlement Difference Total Amount To Direct Cash Settlement Account 
                            if (decTotalDirectCashSettlementDifference != 0)
                            {
                                PostDifferenceAmountToDirectCashSettlementAccount(tradeDate, oGLTradBank, oPGenTable, decTotalDirectCashSettlementDifference, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #endregion

                            #region Summation Of Fees And Control Account Posting Prop

                            #region Credit Buy/Sell Income,VAT Account And Debit Trading Control Prop

                            #region Credit Income Buy And Debit Jobbing Control Prop
                            if (decTotalCommissionBuyProp != 0)
                            {
                                CreditIncomeBuyDebitJobbingProp(tradeDate, oGLTradBank, oPGenTable, decTotalCommissionBuyProp, decTotalDeductCommissionBuyProp, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit Income Sell And Debit Jobbing Control Prop
                            if (decTotalCommissionSellProp != 0)
                            {
                                CreditIncomeSellDebitjobbingProp(tradeDate, oGLTradBank, oPGenTable, decTotalCommissionSellProp, decTotalDeductCommissionSellProp, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit Stamp Duty And Debit Jobbing Control Prop
                            //if (decTotalStampDutyProp != 0)
                            //{
                            //      oGLTradBank.EffectiveDate = tradeDate; 
                            //    oGLTradBank.MasterID = oPGenTable.Bstamp;
                            //    oGLTradBank.AccountID = "";
                            //    oGLTradBank.RecAcct = "";
                            //    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            //    oGLTradBank.Desciption = "Stamp Duty Total Charges For Proprietary: " + GetFormattedDate(tradeDate);
                            //    oGLTradBank.Credit = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                            //    oGLTradBank.Debit = 0;
                            //    oGLTradBank.Debcred = "C";
                            //    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                            //    oGLTradBank.TransType = "TRADBANK";
                            //    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                            //    oGLTradBank.Reverse = "N";
                            //    oGLTradBank.Jnumber = strJnumberNext;
                            //    oGLTradBank.Branch = strDefaultBranchCode;
                            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            //    oGLTradBank.FeeType = "TSTDCPR";
                            //    SqlCommand dbCommandStampTotalCredit = oGLTradBank.AddCommand();
                            //    db.ExecuteNonQuery(dbCommandStampTotalCredit, transaction);


                            //      oGLTradBank.EffectiveDate = tradeDate; 
                            //    oGLTradBank.MasterID = oPGenTable.ShInv;
                            //    oGLTradBank.AccountID = "";
                            //    oGLTradBank.RecAcct = "";
                            //    oGLTradBank.RecAcctMaster = oPGenTable.Bstamp;
                            //    oGLTradBank.Desciption = "Stamp Duty Total Charges For Proprietary: " + GetFormattedDate(tradeDate);
                            //    oGLTradBank.Debit = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                            //    oGLTradBank.Credit = 0;
                            //    oGLTradBank.Debcred = "D";
                            //    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                            //    oGLTradBank.TransType = "TRADBANK";
                            //    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                            //    oGLTradBank.Reverse = "N";
                            //    oGLTradBank.Jnumber = strJnumberNext;
                            //    oGLTradBank.Branch = strDefaultBranchCode;
                            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            //    oGLTradBank.FeeType = "TSTCSPR";
                            //    SqlCommand dbCommandStampTotalCreditJobbing = oGLTradBank.AddCommand();
                            //    db.ExecuteNonQuery(dbCommandStampTotalCreditJobbing, transaction);
                            //}
                            #endregion

                            #region Credit VAT and Debit Jobbing Control Prop
                            if (decTotalVATProp != 0)
                            {
                                CreditVatDebitJobbingProp(tradeDate, oGLTradBank, oPGenTable, decTotalVATProp, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #endregion

                            #region Credit SEC,NSE,CSCS,Stamp Duty Account ONLY Prop

                            #region Credit SEC Account Prop
                            if (decTotalSECProp != 0)
                            {
                                CreditSecPropAccount(tradeDate, oGLTradBank, oPGenTable, decTotalSECProp, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit NSE Account Prop
                            if (decTotalNSEProp != 0)
                            {
                                CreditNsePropAccount(tradeDate, oGLTradBank, oPGenTable, decTotalNSE, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit CSCS Account Prop
                            if (decTotalCSCSProp != 0)
                            {
                                CreditCscsPropAccount(tradeDate, oGLTradBank, oPGenTable, decTotalCSCSProp, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion

                            #region Credit Stamp Duty Account Prop
                            if (decTotalStampDutyProp != 0)
                            {
                                CreditStampDutyPropAccount(tradeDate, oGLTradBank, oPGenTable, decTotalStampDutyProp, strJnumberNext, strDefaultBranchCode, db, transaction);
                            }
                            #endregion


                            #endregion

                            DebitMarketPropAccount(tradeDate, decTotalBankProp, decTotalNSEProp, decTotalCSCSProp, decTotalSECProp, decTotalStampDutyProp, decTotalDirectCashSettlementProp, oGLTradBank, oPGenTable, strJnumberNext, strDefaultBranchCode, db, transaction, strSettlementDate, decTotalDirectCashSettlementDifferenceProp);

                            #endregion

                            #region Summation Of Fees And Control Account Posting For Bond

                            #region Credit Buy/Sell Income,VAT Account And Debit Trading Control Bond

                            #region Credit Income Buy and Sell And Debit Jobbing Control Bond
                            if (decTotalCommissionBond != 0)
                            {
                                CreditIncomeBuyAndSellDebitJobbingControlBond(tradeDate, strJnumberNext, strDefaultBranchCode, oPGenTable, decTotalCommissionBond, decTotalCommissionBuyBond, decTotalCommissionSellBond, decTotalDeductCommissionBond, decTotalDeductCommissionBuyBond, decTotalDeductCommissionSellBond, db, transaction, oGLTradBank);
                            }
                            #endregion

                            #region Credit Stamp Duty And Debit Jobbing Control Bond
                            //if (decTotalStampDutyBond != 0)
                            //{
                            //      oGLTradBank.EffectiveDate = tradeDate; 
                            //    oGLTradBank.MasterID = oPGenTable.Bstamp;
                            //    oGLTradBank.AccountID = "";
                            //    oGLTradBank.RecAcct = "";
                            //    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            //    oGLTradBank.Desciption = "Stamp Duty Total Charges Bond For: " + GetFormattedDate(tradeDate);
                            //    oGLTradBank.Credit = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
                            //    oGLTradBank.Debit = 0;
                            //    oGLTradBank.Debcred = "C";
                            //    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                            //    oGLTradBank.TransType = "TRADBANK";
                            //    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                            //    oGLTradBank.Reverse = "N";
                            //    oGLTradBank.Jnumber = strJnumberNext;
                            //    oGLTradBank.Branch = strDefaultBranchCode;
                            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            //    oGLTradBank.FeeType = "TSTDCB";
                            //    SqlCommand dbCommandStampTotalCredit = oGLTradBank.AddCommand();
                            //    db.ExecuteNonQuery(dbCommandStampTotalCredit, transaction);


                            //      oGLTradBank.EffectiveDate = tradeDate; 
                            //    oGLTradBank.MasterID = oPGenTable.ShInv;
                            //    oGLTradBank.AccountID = "";
                            //    oGLTradBank.RecAcct = "";
                            //    oGLTradBank.RecAcctMaster = oPGenTable.Bstamp;
                            //    oGLTradBank.Desciption = "Stamp Duty Total Charges Bond For: " + GetFormattedDate(tradeDate);
                            //    oGLTradBank.Debit = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
                            //    oGLTradBank.Credit = 0;
                            //    oGLTradBank.Debcred = "D";
                            //    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                            //    oGLTradBank.TransType = "TRADBANK";
                            //    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                            //    oGLTradBank.Reverse = "N";
                            //    oGLTradBank.Jnumber = strJnumberNext;
                            //    oGLTradBank.Branch = strDefaultBranchCode;
                            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            //    oGLTradBank.FeeType = "TSTCSB";
                            //    SqlCommand dbCommandStampTotalCreditJobbing = oGLTradBank.AddCommand();
                            //    db.ExecuteNonQuery(dbCommandStampTotalCreditJobbing, transaction);
                            //}
                            #endregion

                            #region Credit VAT and Debit Jobbing Control Bond
                            if (decTotalVATBond != 0)
                            {
                                CreditVatDebitJobbingControlBond(tradeDate, strJnumberNext, strDefaultBranchCode, oPGenTable, decTotalVATBond, db, transaction, oGLTradBank);
                            }
                            #endregion

                            #endregion

                            #region Credit SEC,NSE,CSCS,Stamp Duty Account ONLY Bond

                            if (decTotalSECBond != 0)
                            {
                                CreditSecAccountBond(tradeDate, strJnumberNext, strDefaultBranchCode, oPGenTable, decTotalSECBond, db, transaction, oGLTradBank);
                            }

                            if (decTotalNSEBond != 0)
                            {
                                CreditNseAccountBond(tradeDate, strJnumberNext, strDefaultBranchCode, oPGenTable, decTotalNSEBond, db, transaction, oGLTradBank);
                            }

                            if (decTotalCSCSBond != 0)
                            {
                                CreditCscsAccountBond(tradeDate, strJnumberNext, strDefaultBranchCode, oPGenTable, decTotalCSCSBond, db, transaction, oGLTradBank);
                            }

                            if (decTotalStampDutyBond != 0)
                            {
                                CreditStampDutyAccountBond(tradeDate, strJnumberNext, strDefaultBranchCode, oPGenTable, decTotalStampDutyBond, db, transaction, oGLTradBank);
                            }
                            #endregion

                            DebitMarketAndStampDutyAccountBond(tradeDate, strJnumberNext, strDefaultBranchCode, oPGenTable, decTotalBankBond, decTotalSECBond, decTotalCSCSBond, decTotalNSEBond, decTotalStampDutyBond, decTotalDirectCashSettlementBond, decTotalDirectCashSettlementDifferenceBond, db, transaction, strSettlementDate, oGLTradBank);
                            #endregion

                            #region Different Branch Changes For City Code
                            if (strBranchBuySellCharge.Trim() == "YES")
                            {
                                BranchChangesForCityCode(tradeDate, strJnumberNext, strDefaultBranchCode, db, transaction, oGLTradBank);
                            }
                            #endregion

                            #region End Processing
                            AutoDate oAutoDate = new AutoDate
                            {
                                iAutoDate = tradeDate,
                                UserId = GeneralFunc.UserName
                            };
                            SqlCommand dbCommandAutoDate = oAutoDate.AddCommand();
                            db.ExecuteNonQuery(dbCommandAutoDate, transaction);

                            transaction.Commit();
                            return ResponseResult.Success("CsCs Allotment Postings Successfull");

                            #endregion


                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message, ex);
                            transaction?.Rollback();
                            return ResponseResult.Error("Error In Posting " + ex.Message.Trim());
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error("Error In Posting " + ex.Message.Trim());
                }
                #endregion
            });
        }

        private static void DebitMarketAndStampDutyAccountBond(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, decimal decTotalBankBond, decimal decTotalSECBond, decimal decTotalCSCSBond, decimal decTotalNSEBond, decimal decTotalStampDutyBond, decimal decTotalDirectCashSettlementBond, decimal decTotalDirectCashSettlementDifferenceBond, SqlDatabase db, SqlTransaction transaction, string strSettlementDate, AcctGL oGLTradBank)
        {
            if (decTotalBankBond != 0 || decTotalNSEBond != 0 || decTotalCSCSBond != 0 || decTotalSECBond != 0 || decTotalStampDutyBond != 0)
            {


                #region Debit SEC,NSE,CSCS,Stamp Duty Account Bond
                if (decTotalSECBond != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bsec;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "SEC Charges Payment Bond For: " + GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PSECDB";
                    SqlCommand dbCommandSecStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandSecStatControlDebit, transaction);
                }

                if (decTotalNSEBond != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bnse;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "NSE Charges Payment Bond For: " + GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PNSEDB";
                    SqlCommand dbCommandNseStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandNseStatControlDebit, transaction);
                }

                if (decTotalCSCSBond != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bcscs;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "CSCS Charges Payment Bond For: " + GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PCSCDB";
                    SqlCommand dbCommandCSCSStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandCSCSStatControlDebit, transaction);
                }

                if (decTotalStampDutyBond != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bstamp;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Stamp Duty Charges Payment Bond For: " + GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PSTDYDB";
                    SqlCommand dbCommandStampDutyStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandStampDutyStatControlDebit, transaction);
                }
                #endregion

                #region Bank Trading And Control Trading Account Posting Bond
                oGLTradBank.EffectiveDate = tradeDate;
                if (decTotalBankBond < 0)
                {
                    oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase Bond For: " + GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Abs(Math.Round(decTotalBankBond, 2, MidpointRounding.AwayFromZero)) + Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlementBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = Math.Abs(Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero));
                }
                else if (decTotalBankBond > 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.Desciption = "Net Sale Trading Bond For: " + GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Round(decTotalBankBond, 2, MidpointRounding.AwayFromZero) - (Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlementBond, 2, MidpointRounding.AwayFromZero));
                    oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                }
                else if (decTotalBankBond == 0)
                {
                    oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase Bond For: " + GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlementBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                }
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.Q; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = ""; oGLTradBank.ClearingDayForTradingTransaction = "Y";
                oGLTradBank.FeeType = "TSTDB";
                SqlCommand dbCommandStatControlCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlCredit, transaction);


                oGLTradBank.EffectiveDate = tradeDate;
                if (decTotalBankBond > 0)
                {
                    oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Sale Bond For: " + GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Round(decTotalBankBond, 2, MidpointRounding.AwayFromZero) - (Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlementBond, 2, MidpointRounding.AwayFromZero));
                    oGLTradBank.Debit = Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero);
                }
                else if (decTotalBankBond < 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.Desciption = "Net Purchase Trading Bond For: " + GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Abs(Math.Round(decTotalBankBond, 2, MidpointRounding.AwayFromZero)) + Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlementBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                }
                else if (decTotalBankBond == 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.Desciption = "Net Purchase Trading Bond For: " + GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlementBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                }
                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.Q; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = ""; oGLTradBank.ClearingDayForTradingTransaction = "Y";
                oGLTradBank.FeeType = "TSTDB";
                SqlCommand dbCommandStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlDebit, transaction);

                #region Posting For Direct Cash Settlement Bond
                if (decTotalDirectCashSettlementBond != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.DirectCashSettleAcct;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.Desciption = "Direct Settlement Net Trading Bond For: " + GetFormattedDate(tradeDate);
                    oGLTradBank.Debit = Math.Round(decTotalDirectCashSettlementBond, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "TSDCSB";
                    SqlCommand dbCommandDirectCashSettleCredit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandDirectCashSettleCredit, transaction);
                }
                #endregion

                #region Posting Direct Cash Settlement Difference Total Amount To Direct Cash Settlement Account 
                if (decTotalDirectCashSettlementDifferenceBond != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Direct Settlement Comm Pref Difference For: " + GetFormattedDate(tradeDate);
                    if (decTotalDirectCashSettlementDifferenceBond > 0)
                    {
                        oGLTradBank.Debit = 0;
                        oGLTradBank.Credit = Math.Round(decTotalDirectCashSettlementDifferenceBond, 2, MidpointRounding.AwayFromZero);
                        oGLTradBank.Debcred = "C";
                    }
                    else
                    {
                        oGLTradBank.Debit = Math.Round(Math.Abs(decTotalDirectCashSettlementDifferenceBond), 2, MidpointRounding.AwayFromZero);
                        oGLTradBank.Credit = 0;
                        oGLTradBank.Debcred = "D";
                    }
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "TSDCSBDF";
                    SqlCommand dbCommandDirectCashSettleCreditDifference = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandDirectCashSettleCreditDifference, transaction);
                }
                #endregion

                #endregion
            }
        }

        private static void CreditStampDutyAccountBond(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, decimal decTotalStampDutyBond, SqlDatabase db, SqlTransaction transaction, AcctGL oGLTradBank)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bstamp;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Stamp Duty Total Charges Bond For: " + GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTDYCB";
            SqlCommand dbCommandStampDutyTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandStampDutyTotalCredit, transaction);
        }

        private static void CreditCscsAccountBond(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, decimal decTotalCSCSBond, SqlDatabase db, SqlTransaction transaction, AcctGL oGLTradBank)
        {

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bcscs;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "CSCS Total Charges Bond For: " + GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalCSCSBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCSCCB";
            SqlCommand dbCommandCSCSTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCSCSTotalCredit, transaction);
        }

        private static void CreditNseAccountBond(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, decimal decTotalNSEBond, SqlDatabase db, SqlTransaction transaction, AcctGL oGLTradBank)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bnse;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "NSE Total Charges Bond For: " + GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalNSEBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TNSECB";
            SqlCommand dbCommandNseTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandNseTotalCredit, transaction);
        }

        private static void CreditSecAccountBond(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, decimal decTotalSECBond, SqlDatabase db, SqlTransaction transaction, AcctGL oGLTradBank)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bsec;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "SEC Total Charges Bond For: " + GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalSECBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSECCB";
            SqlCommand dbCommandSecTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandSecTotalCredit, transaction);
        }

        private static void CreditVatDebitJobbingControlBond(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, decimal decTotalVATBond, SqlDatabase db, SqlTransaction transaction, AcctGL oGLTradBank)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bvat;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "VAT Total Charges Bond For: " + GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalVATBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TVATCB";
            SqlCommand dbCommandVATTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandVATTotalCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Bvat;
            oGLTradBank.Desciption = "VAT Total Charges Bond For: " + GetFormattedDate(tradeDate);
            oGLTradBank.Debit = Math.Round(decTotalVATBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTVTB";
            SqlCommand dbCommandVATTotalCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandVATTotalCreditJobbing, transaction);
        }

        private static void CreditIncomeBuyAndSellDebitJobbingControlBond(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, decimal decTotalCommissionBond, decimal decTotalCommissionBuyBond, decimal decTotalCommissionSellBond, decimal decTotalDeductCommissionBond, decimal decTotalDeductCommissionBuyBond, decimal decTotalDeductCommissionSellBond, SqlDatabase db, SqlTransaction transaction, AcctGL oGLTradBank)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bcncomm;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Commission Total Buy Charges Bond For: " + GetFormattedDate(tradeDate);
            decimal decNetCommissionBuyBond = decTotalCommissionBuyBond - decTotalDeductCommissionBuyBond;
            oGLTradBank.Credit = Math.Round(decNetCommissionBuyBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCOMCBB";
            SqlCommand dbCommandCommissionTotalBuyCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalBuyCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Scncomm;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Commission Total Sell Charges Bond For: " + GetFormattedDate(tradeDate);
            decimal decNetCommissionSellBond = decTotalCommissionSellBond - decTotalDeductCommissionSellBond;
            oGLTradBank.Credit = Math.Round(decNetCommissionSellBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCOMCSB";
            SqlCommand dbCommandCommissionTotalSellCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalSellCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Bcncomm;
            oGLTradBank.Desciption = "Commission Total Charges Bond For: " + GetFormattedDate(tradeDate);
            decimal decNetCommissionBond = decTotalCommissionBond - decTotalDeductCommissionBond;
            oGLTradBank.Debit = Math.Round(decNetCommissionBond, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTCMB";
            SqlCommand dbCommandCommissionTotalCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalCreditJobbing, transaction);
        }

        private static void BranchChangesForCityCode(DateTime tradeDate, string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction, AcctGL oGLTradBank)
        {
            decimal decNetBuyAndSellUploadPerBranch = 0;
            Allotment oAllotmentBranchPosted = new Allotment();
            oAllotmentBranchPosted.DateAlloted = tradeDate;
            Branch oBranchPostNetTrade = new Branch();
            Branch oBranchSelectAccount = new Branch();
            foreach (DataRow oRowBranch in oBranchPostNetTrade.GetAllExcludeDefault().Tables[0].Rows)
            {
                decNetBuyAndSellUploadPerBranch = oAllotmentBranchPosted.GetNetPurchaseSaleAllotmentByDateAndBranchForUpload
                                                    (tradeDate, oRowBranch["Brancode"].ToString().Trim());
                if (decNetBuyAndSellUploadPerBranch != 0)
                {
                    oBranchSelectAccount.TransNo = oRowBranch["Brancode"].ToString().Trim();
                    oBranchSelectAccount.GetBranch();


                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oBranchSelectAccount.Trading;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oBranchSelectAccount.Commission;
                    if (decNetBuyAndSellUploadPerBranch < 0)
                    {
                        oGLTradBank.Credit = Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                        oGLTradBank.Debit = 0;
                        oGLTradBank.Debcred = "C";
                        oGLTradBank.Desciption = "Net Purchase For: " + GetFormattedDate(tradeDate);
                    }
                    else if (decNetBuyAndSellUploadPerBranch > 0)
                    {
                        oGLTradBank.Credit = 0;
                        oGLTradBank.Debit = Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                        oGLTradBank.Debcred = "D";
                        oGLTradBank.Desciption = "Net Sale For: " + GetFormattedDate(tradeDate);
                    }
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "BTRAB"; oGLTradBank.PostToOtherBranch = "Y";
                    SqlCommand dbCommandBranchNetTradeDefault = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandBranchNetTradeDefault, transaction);

                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oBranchSelectAccount.Commission;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oBranchSelectAccount.Trading;
                    if (decNetBuyAndSellUploadPerBranch < 0)
                    {
                        oGLTradBank.Credit = 0;
                        oGLTradBank.Debit = Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                        oGLTradBank.Debcred = "D";
                        oGLTradBank.Desciption = "Net Purchase For: " + GetFormattedDate(tradeDate);
                    }
                    else if (decNetBuyAndSellUploadPerBranch > 0)
                    {
                        oGLTradBank.Credit = Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                        oGLTradBank.Debit = 0;
                        oGLTradBank.Debcred = "C";
                        oGLTradBank.Desciption = "Net Sale For: " + GetFormattedDate(tradeDate);
                    }
                    oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "BTRAD";
                    SqlCommand dbCommandBranchNetTradeBranch = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandBranchNetTradeBranch, transaction);
                    oGLTradBank.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege
                }
            }
        }

        private static void DebitMarketPropAccount(DateTime tradeDate, decimal decTotalBankProp, decimal decTotalNSEProp, decimal decTotalCSCSProp,
            decimal decTotalSECProp, decimal decTotalStampDutyProp, decimal decTotalDirectCashSettlementProp,
            AcctGL oGLTradBank, PGenTable oPGenTable, string strJnumberNext, string strDefaultBranchCode, SqlDatabase db,
            SqlTransaction transaction, string strSettlementDate, decimal decTotalDirectCashSettlementDifferenceProp)
        {


            if (decTotalBankProp != 0 || decTotalNSEProp != 0 || decTotalCSCSProp != 0 || decTotalSECProp != 0 ||
                decTotalStampDutyProp != 0 || decTotalDirectCashSettlementProp != 0)
            {
                #region Debit SEC,NSE,CSCS,Stamp Duty Account ONLY Prop

                if (decTotalSECProp != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bsec;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "SEC Charges Payment For Proprietary: " +
                                             GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PSECDPR";
                    SqlCommand dbCommandSecStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandSecStatControlDebit, transaction);
                }

                if (decTotalNSEProp != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bnse;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "NSE Charges Payment For Proprietary: " +
                                             GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalNSEProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PNSEDPR";
                    SqlCommand dbCommandNseStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandNseStatControlDebit, transaction);
                }

                if (decTotalCSCSProp != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bcscs;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "CSCS Charges Payment For Proprietary: " + GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PCSCDPR";
                    SqlCommand dbCommandCSCSStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandCSCSStatControlDebit, transaction);
                }

                if (decTotalStampDutyProp != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bstamp;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Stamp Duty Charges Payment For Proprietary: " +
                                             GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PSTDYDPR";
                    SqlCommand dbCommandStampDutyStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandStampDutyStatControlDebit, transaction);
                }

                #endregion

                #region Bank Trading And Control Trading Account Posting Prop

                oGLTradBank.EffectiveDate = tradeDate;
                if (decTotalBankProp < 0)
                {
                    oGLTradBank.MasterID = oPGenTable.PropTradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase For Proprietary: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Abs(Math.Round(decTotalBankProp, 2, MidpointRounding.AwayFromZero)) +
                                         Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalNSEProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalDirectCashSettlementProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = Math.Abs(Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero));
                }
                else if (decTotalBankProp > 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.PropTradeBank;
                    oGLTradBank.Desciption = "Net Sale Trading For Proprietary: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Round(decTotalBankProp, 2, MidpointRounding.AwayFromZero) -
                                         (Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalNSEProp, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero));
                    oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                }
                else if (decTotalBankProp == 0)
                {
                    oGLTradBank.MasterID = oPGenTable.PropTradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase For Proprietary: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalNSEProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalDirectCashSettlementProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                }

                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     tradeDate.Day.ToString()
                                         .PadLeft(2, char.Parse("0")) +
                                     tradeDate.Month.ToString()
                                         .PadLeft(2, char.Parse("0")) + tradeDate
                                         .Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    tradeDate.Day.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Month.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.Q;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = "";
                oGLTradBank.Chqno = "";
                oGLTradBank.ClearingDayForTradingTransaction = "Y";
                oGLTradBank.FeeType = "TSTDPR";
                SqlCommand dbCommandStatControlCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlCredit, transaction);


                oGLTradBank.EffectiveDate = tradeDate;
                if (decTotalBankProp > 0)
                {
                    oGLTradBank.MasterID = oPGenTable.PropTradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Sale For Proprietary: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Round(decTotalBankProp, 2, MidpointRounding.AwayFromZero) -
                                        (Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalNSEProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalDirectCashSettlementProp, 2, MidpointRounding.AwayFromZero));
                    oGLTradBank.Debit = Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero);
                }
                else if (decTotalBankProp < 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.PropTradeBank;
                    oGLTradBank.Desciption = "Net Purchase Trading For Proprietary: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Abs(Math.Round(decTotalBankProp, 2, MidpointRounding.AwayFromZero)) +
                                        Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalNSEProp, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                }
                else if (decTotalBankProp == 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.PropTradeBank;
                    oGLTradBank.Desciption = "Net Purchase Trading For Proprietary: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalNSEProp, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                }

                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     tradeDate.Day.ToString()
                                         .PadLeft(2, char.Parse("0")) +
                                     tradeDate.Month.ToString()
                                         .PadLeft(2, char.Parse("0")) + tradeDate
                                         .Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    tradeDate.Day.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Month.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.Q;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = "";
                oGLTradBank.Chqno = "";
                oGLTradBank.ClearingDayForTradingTransaction = "Y";
                oGLTradBank.FeeType = "TSTDPR";
                SqlCommand dbCommandStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlDebit, transaction);

                #endregion
            }

            #region Posting Direct Cash Settlement Total Amount To Direct Cash Settlement Account Prop

            if (decTotalDirectCashSettlementProp != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate;
                oGLTradBank.MasterID = oPGenTable.DirectCashSettleAcct;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "Direct Settlement Net Trading For Proprietary: " +
                                         GetFormattedDate(tradeDate);
                oGLTradBank.Debit = Math.Round(decTotalDirectCashSettlementProp, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = "";
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSDCSPR";
                SqlCommand dbCommandDirectCashSettleCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandDirectCashSettleCredit, transaction);
            }

            #endregion

            #region Posting Direct Cash Settlement Difference Total Amount To Direct Cash Settlement Account Prop

            if (decTotalDirectCashSettlementDifferenceProp != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate;
                oGLTradBank.MasterID = oPGenTable.ShInv;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "Direct Settlement Comm Pref Difference For: " +
                                         GetFormattedDate(tradeDate);
                if (decTotalDirectCashSettlementDifferenceProp > 0)
                {
                    oGLTradBank.Debit = 0;
                    oGLTradBank.Credit =
                        Math.Round(decTotalDirectCashSettlementDifferenceProp, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "C";
                }
                else
                {
                    oGLTradBank.Debit = Math.Round(Math.Abs(decTotalDirectCashSettlementDifferenceProp), 2,
                        MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debcred = "D";
                }

                oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = "";
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSDCSDFPR";
                SqlCommand dbCommandDirectCashSettleCreditDifference = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandDirectCashSettleCreditDifference, transaction);
            }
        }

        private static void CreditStampDutyPropAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalStampDutyProp,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bstamp;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Stamp Duty Total Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTDYCPR";
            SqlCommand dbCommandStampDutyTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandStampDutyTotalCredit, transaction);
        }

        private static void CreditCscsPropAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalCSCSProp,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bcscs;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "CSCS Total Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalCSCSProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCSCCPR";
            SqlCommand dbCommandCSCSTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCSCSTotalCredit, transaction);
        }

        private static void CreditNsePropAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalNSE,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bnse;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "NSE Total Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TNSECPR";
            SqlCommand dbCommandNseTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandNseTotalCredit, transaction);
        }

        private static void CreditSecPropAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalSECProp,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bsec;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "SEC Total Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalSECProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSECCPR";
            SqlCommand dbCommandSecTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandSecTotalCredit, transaction);
        }

        private static void CreditVatDebitJobbingProp(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalVATProp,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bvat;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "VAT Total Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalVATProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TVATCPR";
            SqlCommand dbCommandVATTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandVATTotalCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Bvat;
            oGLTradBank.Desciption = "VAT Total Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Debit = Math.Round(decTotalVATProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTVTPR";
            SqlCommand dbCommandVATTotalCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandVATTotalCreditJobbing, transaction);
        }

        private static void CreditIncomeSellDebitjobbingProp(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable,
            decimal decTotalCommissionSellProp, decimal decTotalDeductCommissionSellProp, string strJnumberNext,
            string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Scncomm;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Commission Total Sell Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            decimal decNetCommissionSellProp = decTotalCommissionSellProp - decTotalDeductCommissionSellProp;
            oGLTradBank.Credit = Math.Round(decNetCommissionSellProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCOMCSPR";
            SqlCommand dbCommandCommissionTotalSellCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalSellCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Scncomm;
            oGLTradBank.Desciption = "Commission Total Sell Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            decNetCommissionSellProp = decTotalCommissionSellProp - decTotalDeductCommissionSellProp;
            oGLTradBank.Debit = Math.Round(decNetCommissionSellProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTSCMPR";
            SqlCommand dbCommandCommissionTotalSellCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalSellCreditJobbing, transaction);
        }

        private static string GetFormattedDate(DateTime tradeDate)
        {
            return $"{tradeDate:dd/MM/yyyy}";
        }

        private static void CreditIncomeBuyDebitJobbingProp(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable,
            decimal decTotalCommissionBuyProp, decimal decTotalDeductCommissionBuyProp, string strJnumberNext,
            string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bcncomm;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Commission Total Buy Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            decimal decNetCommissionBuyProp = decTotalCommissionBuyProp - decTotalDeductCommissionBuyProp;
            oGLTradBank.Credit = Math.Round(decNetCommissionBuyProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCOMCBPR";
            SqlCommand dbCommandCommissionTotalBuyCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalBuyCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Bcncomm;
            oGLTradBank.Desciption = "Commission Total Buy Charges For Proprietary: " +
                                     GetFormattedDate(tradeDate);
            decNetCommissionBuyProp = decTotalCommissionBuyProp - decTotalDeductCommissionBuyProp;
            oGLTradBank.Debit = Math.Round(decNetCommissionBuyProp, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTBCMPR";
            SqlCommand dbCommandCommissionTotalBuyCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalBuyCreditJobbing, transaction);
        }

        private static string GetUnformattedDate(DateTime tradeDate)
        {
            return $"{tradeDate:dd}{tradeDate:MM}{tradeDate:yyyy}";
        }

        private static void PostDifferenceAmountToDirectCashSettlementAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable,
            decimal decTotalDirectCashSettlementDifference, string strJnumberNext, string strDefaultBranchCode, SqlDatabase db,
            SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Direct Settlement Comm Pref Difference For: " +
                                     GetFormattedDate(tradeDate);
            if (decTotalDirectCashSettlementDifference > 0)
            {
                oGLTradBank.Debit = 0;
                oGLTradBank.Credit = Math.Round(decTotalDirectCashSettlementDifference, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debcred = "C";
            }
            else
            {
                oGLTradBank.Debit =
                    Math.Round(Math.Abs(decTotalDirectCashSettlementDifference), 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
            }

            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSDCSDF";
            SqlCommand dbCommandDirectCashSettleCreditDifference = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandDirectCashSettleCreditDifference, transaction);
        }

        private static void PostTotalAmountToDirectCashSettlementAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable,
            decimal decTotalDirectCashSettlement, string strJnumberNext, string strDefaultBranchCode, SqlDatabase db,
            SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.DirectCashSettleAcct;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Direct Settlement Net Trading For: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Debit = Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSDCS";
            SqlCommand dbCommandDirectCashSettleCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandDirectCashSettleCredit, transaction);
        }

        private static void DebitMarketAndStampDutyAccount(DateTime tradeDate, decimal decTotalBank, decimal decTotalNSE, decimal decTotalCSCS,
            decimal decTotalSEC, decimal decTotalStampDuty, decimal decTotalDirectCashSettlement, AcctGL oGLTradBank,
            PGenTable oPGenTable, string strJnumberNext, string strDefaultBranchCode, SqlDatabase db,
            SqlTransaction transaction, string strSettlementDate)
        {
            if (decTotalBank != 0 || decTotalNSE != 0 || decTotalCSCS != 0 || decTotalSEC != 0 || decTotalStampDuty != 0 ||
                decTotalDirectCashSettlement != 0)
            {
                #region Debit SEC,NSE,CSCS,Stamp Duty Account ONLY

                if (decTotalSEC != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bsec;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "SEC Charges Payment For: " +
                                             GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PSECD";
                    SqlCommand dbCommandSecStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandSecStatControlDebit, transaction);
                }

                if (decTotalNSE != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bnse;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "NSE Charges Payment For: " +
                                             GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PNSED";
                    SqlCommand dbCommandNseStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandNseStatControlDebit, transaction);
                }

                if (decTotalCSCS != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bcscs;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "CSCS Charges Payment For: " +
                                             GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PCSCD";
                    SqlCommand dbCommandCSCSStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandCSCSStatControlDebit, transaction);
                }

                if (decTotalStampDuty != 0)
                {
                    oGLTradBank.EffectiveDate = tradeDate;
                    oGLTradBank.MasterID = oPGenTable.Bstamp;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Stamp Duty Charges Payment For: " +
                                             GetFormattedDate(tradeDate);
                    oGLTradBank.Credit = 0;
                    oGLTradBank.Debit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debcred = "D";
                    oGLTradBank.SysRef = "TRB" + "-" +
                                         GetUnformattedDate(tradeDate);
                    oGLTradBank.TransType = "TRADBANK";
                    oGLTradBank.Ref01 =
                        GetUnformattedDate(tradeDate);
                    oGLTradBank.Reverse = "N";
                    oGLTradBank.Jnumber = strJnumberNext;
                    oGLTradBank.Branch = strDefaultBranchCode;
                    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGLTradBank.AcctRef = "";
                    oGLTradBank.Ref02 = "";
                    oGLTradBank.Chqno = "";
                    oGLTradBank.FeeType = "PSTDYD";
                    SqlCommand dbCommandStampDutyStatControlDebit = oGLTradBank.AddCommand();
                    db.ExecuteNonQuery(dbCommandStampDutyStatControlDebit, transaction);
                }

                #endregion

                #region Bank Trading And Control Trading Account Posting

                oGLTradBank.EffectiveDate = tradeDate;
                if (decTotalBank < 0)
                {
                    oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase For: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Abs(Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero)) +
                                         Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = Math.Abs(Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero));
                }
                else if (decTotalBank > 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.Desciption = "Net Sale Trading For: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero) -
                                         (Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero));
                    oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                }
                else if (decTotalBank == 0)
                {
                    oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase For: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Credit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                }

                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     tradeDate.Day.ToString()
                                         .PadLeft(2, char.Parse("0")) +
                                     tradeDate.Month.ToString()
                                         .PadLeft(2, char.Parse("0")) + tradeDate
                                         .Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    tradeDate.Day.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Month.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.Q;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = "";
                oGLTradBank.Chqno = "";
                oGLTradBank.ClearingDayForTradingTransaction = "Y";
                oGLTradBank.FeeType = "TSTD";
                SqlCommand dbCommandStatControlCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlCredit, transaction);


                oGLTradBank.EffectiveDate = tradeDate;
                if (decTotalBank > 0)
                {
                    oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Sale For: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero) -
                                        (Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) +
                                         Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero));
                    oGLTradBank.Debit = Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero);
                }
                else if (decTotalBank < 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.Desciption = "Net Purchase Trading For: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Abs(Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero)) +
                                        Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                }
                else if (decTotalBank == 0)
                {
                    oGLTradBank.MasterID = oPGenTable.ShInv;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.Desciption = "Net Purchase Trading For: " +
                                             GetFormattedDate(tradeDate) + " Trade Settlement Of " + strSettlementDate;
                    oGLTradBank.Debit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                }

                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     tradeDate.Day.ToString()
                                         .PadLeft(2, char.Parse("0")) +
                                     tradeDate.Month.ToString()
                                         .PadLeft(2, char.Parse("0")) + tradeDate
                                         .Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    tradeDate.Day.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Month.ToString().PadLeft(2, char.Parse("0")) +
                    tradeDate.Year.ToString().PadLeft(4, char.Parse("0"));
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.Q;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = "";
                oGLTradBank.Chqno = "";
                oGLTradBank.ClearingDayForTradingTransaction = "Y";
                oGLTradBank.FeeType = "TSTD";
                SqlCommand dbCommandStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlDebit, transaction);

                #endregion
            }
        }

        private static void CreditStamDutyAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalStampDuty,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bstamp;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Stamp Duty Total Charges For: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTDYC";
            SqlCommand dbCommandStampDutyTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandStampDutyTotalCredit, transaction);
        }

        private static void CreditCscs(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalCSCS, string strJnumberNext,
            string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bcscs;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "CSCS Total Charges For: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCSCC";
            SqlCommand dbCommandCSCSTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCSCSTotalCredit, transaction);
        }

        private static void CreditNseAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalNSE,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bnse;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "NSE Total Charges For: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TNSEC";
            SqlCommand dbCommandNseTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandNseTotalCredit, transaction);
        }

        private static void CreditSecAccount(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalSEC, string strJnumberNext,
            string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bsec;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "SEC Total Charges For: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSECC";
            SqlCommand dbCommandSecTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandSecTotalCredit, transaction);
        }

        private static void CreditVatDebitJobbingControl(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable, decimal decTotalVAT,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bvat;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "VAT Total Charges For: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Credit = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TVATC";
            SqlCommand dbCommandVATTotalCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandVATTotalCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Bvat;
            oGLTradBank.Desciption = "VAT Total Charges For: " +
                                     GetFormattedDate(tradeDate);
            oGLTradBank.Debit = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTVT";
            SqlCommand dbCommandVATTotalCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandVATTotalCreditJobbing, transaction);
        }

        private static void CreditIncomeSellDebitJobbingControl(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable,
            decimal decTotalCommissionSell, decimal decTotalDeductCommissionSell, string strJnumberNext,
            string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {
            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Scncomm;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Commission Total Sell Charges For: " +
                                     GetFormattedDate(tradeDate);
            decimal decNetCommissionSell = decTotalCommissionSell - decTotalDeductCommissionSell;
            oGLTradBank.Credit = Math.Round(decNetCommissionSell, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCOMCS";
            SqlCommand dbCommandCommissionTotalSellCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalSellCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Scncomm;
            oGLTradBank.Desciption = "Commission Total Sell Charges For: " +
                                     GetFormattedDate(tradeDate);
            decNetCommissionSell = decTotalCommissionSell - decTotalDeductCommissionSell;
            oGLTradBank.Debit = Math.Round(decNetCommissionSell, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" +
                                 tradeDate.Day.ToString()
                                     .PadLeft(2, char.Parse("0")) +
                                 tradeDate.Month.ToString()
                                     .PadLeft(2, char.Parse("0")) + tradeDate.Year
                                     .ToString().PadLeft(4, char.Parse("0"));
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 =
                GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTSCM";
            SqlCommand dbCommandCommissionTotalSellCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalSellCreditJobbing, transaction);
        }

        private static void CreditIncomeBuyDebitJobbingControl(DateTime tradeDate, AcctGL oGLTradBank, PGenTable oPGenTable,
            decimal decTotalCommissionBuy, decimal decTotalDeductCommissionBuy, string strJnumberNext,
            string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction)
        {


            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.Bcncomm;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            oGLTradBank.Desciption = "Commission Total Buy Charges For: " +
                                     GetFormattedDate(tradeDate);
            decimal decNetCommissionBuy = decTotalCommissionBuy - decTotalDeductCommissionBuy;
            oGLTradBank.Credit = Math.Round(decNetCommissionBuy, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Debit = 0;
            oGLTradBank.Debcred = "C";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TCOMCB";
            SqlCommand dbCommandCommissionTotalBuyCredit = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalBuyCredit, transaction);

            oGLTradBank.EffectiveDate = tradeDate;
            oGLTradBank.MasterID = oPGenTable.ShInv;
            oGLTradBank.AccountID = "";
            oGLTradBank.RecAcct = "";
            oGLTradBank.RecAcctMaster = oPGenTable.Bcncomm;
            oGLTradBank.Desciption = "Commission Total Buy Charges For: " +
                                     GetFormattedDate(tradeDate);
            decNetCommissionBuy = decTotalCommissionBuy - decTotalDeductCommissionBuy;
            oGLTradBank.Debit = Math.Round(decNetCommissionBuy, 2, MidpointRounding.AwayFromZero);
            oGLTradBank.Credit = 0;
            oGLTradBank.Debcred = "D";
            oGLTradBank.SysRef = "TRB" + "-" + GetUnformattedDate(tradeDate);
            oGLTradBank.TransType = "TRADBANK";
            oGLTradBank.Ref01 = GetUnformattedDate(tradeDate);
            oGLTradBank.Reverse = "N";
            oGLTradBank.Jnumber = strJnumberNext;
            oGLTradBank.Branch = strDefaultBranchCode;
            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGLTradBank.AcctRef = "";
            oGLTradBank.Ref02 = "";
            oGLTradBank.Chqno = "";
            oGLTradBank.FeeType = "TSTBCM";
            SqlCommand dbCommandCommissionTotalBuyCreditJobbing = oGLTradBank.AddCommand();
            db.ExecuteNonQuery(dbCommandCommissionTotalBuyCreditJobbing, transaction);
        }

        private static void ProcessJobOrder(DataRow oRowView, Allotment oAllot, SqlDatabase db, SqlTransaction transaction,
            string strAllotmentNo)
        {
            var oJob = new JobOrder();
            var oDsJob = new DataSet();
            oJob.CustNo = oRowView["CustNo"].ToString().Trim();
            oJob.StockCode = oRowView["StockCode"].ToString().Trim();
            oJob.CustNo_CD = oRowView["CrossCustNo"].ToString().Trim();
            if (oAllot.Cdeal == 'Y')
            {
                oDsJob = oRowView["CrossType"].ToString().Trim() == "NO" ? oJob.GetUnProcBuyGivenCustStock() : oJob.GetUnProcCrossGivenCustStock();
            }
            else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
            {
                oDsJob = oJob.GetUnProcSellGivenCustStock();
            }
            else
            {
                oDsJob = oJob.GetUnProcBuyGivenCustStock();
            }

            var thisTableJob = oDsJob.Tables[0];
            var iLeft = 0;
            foreach (DataRow oRowViewJob in thisTableJob.Rows)
            {
                if ((int.Parse(oRowView["Units"].ToString().Trim()) - iLeft) >
                    int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                {
                    iLeft = iLeft + int.Parse(oRowViewJob["Balance"].ToString().Trim());
                    var dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(),
                        int.Parse(oRowViewJob["Balance"].ToString().Trim()));
                    db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                    var dbCommandJobBalance =
                        oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(), strAllotmentNo);
                    db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                }
                else if ((int.Parse(oRowView["Units"].ToString().Trim()) - iLeft) ==
                         int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                {
                    iLeft = iLeft + int.Parse(oRowViewJob["Balance"].ToString().Trim());
                    var dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(),
                        int.Parse(oRowViewJob["Balance"].ToString().Trim()));
                    db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                    var dbCommandJobBalance =
                        oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(), strAllotmentNo);
                    db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                    break;
                    //Exit Do
                }
                else if ((int.Parse(oRowView["Units"].ToString().Trim()) - iLeft) <
                         int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                {
                    var dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(),
                        int.Parse(oRowView["Units"].ToString().Trim()) - iLeft);
                    db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                    var dbCommandJobBalance = oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(),
                        strAllotmentNo, int.Parse(oRowView["Units"].ToString().Trim()) - iLeft);
                    db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                    break;
                    //Exit Do
                }
            }

            if ((oAllot.Cdeal != 'Y') || (oRowView["CrossType"].ToString().Trim() != "NO")) return;
            {
                oJob.CustNo = oRowView["CrossCustNo"].ToString().Trim();
                oDsJob = oJob.GetUnProcSellGivenCustStock();
                thisTableJob = oDsJob.Tables[0];
                iLeft = 0;
                foreach (DataRow oRowViewJob in thisTableJob.Rows)
                {
                    if ((int.Parse(oRowView["Units"].ToString().Trim()) - iLeft) >
                        int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                    {
                        iLeft = iLeft + int.Parse(oRowViewJob["Balance"].ToString().Trim());
                        var dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(),
                            int.Parse(oRowViewJob["Balance"].ToString().Trim()));
                        db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                        var dbCommandJobBalance =
                            oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(), strAllotmentNo);
                        db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                    }
                    else if ((int.Parse(oRowView["Units"].ToString().Trim()) - iLeft) ==
                             int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                    {
                        iLeft = iLeft + int.Parse(oRowViewJob["Balance"].ToString().Trim());
                        var dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(),
                            int.Parse(oRowViewJob["Balance"].ToString().Trim()));
                        db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                        var dbCommandJobBalance =
                            oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(), strAllotmentNo);
                        db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                        break;
                        //Exit Do
                    }
                    else if ((int.Parse(oRowView["Units"].ToString().Trim()) - iLeft) <
                             int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                    {
                        var dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(),
                            int.Parse(oRowView["Units"].ToString().Trim()) - iLeft);
                        db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                        var dbCommandJobBalance = oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(),
                            strAllotmentNo, int.Parse(oRowView["Units"].ToString().Trim()) - iLeft);
                        db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                        break;
                        //Exit Do
                    }
                }
            }
        }

        private static void ProcessPortfolio(DataRow oRowView, Allotment oAllot, decimal decCrossDealTotalAmount,
            string strAllotmentNo, SqlDatabase db, SqlTransaction transaction, string strAllotmentNo2)
        {
            var oPortfolio = new Portfolio
            {
                PurchaseDate = oRowView["Date"].ToString().Trim().ToDate(),
                CustomerAcct = oRowView["CustNo"].ToString(),
                StockCode = oRowView["Stockcode"].ToString().Trim(),
                Units = long.Parse(oRowView["Units"].ToString().Trim()),
                UnitPrice = float.Parse(oAllot.UnitPrice.ToString().Trim())
            };
            if (oAllot.Cdeal == 'N')
            {
                oPortfolio.ActualUnitCost =
                    float.Parse(oAllot.TotalAmount.ToString()) / long.Parse(oRowView["Units"].ToString().Trim());
                oPortfolio.TotalCost = oAllot.TotalAmount;
            }
            else
            {
                oPortfolio.ActualUnitCost = float.Parse(decCrossDealTotalAmount.ToString()) /
                                       long.Parse(oRowView["Units"].ToString().Trim());
                oPortfolio.TotalCost = decCrossDealTotalAmount;
            }

            oPortfolio.Ref01 = strAllotmentNo;
            switch (oRowView["Buy_Sold_Ind"].ToString().Trim())
            {
                case "B":
                    {
                        oPortfolio.DebCred = "C";
                        if (oAllot.Cdeal == 'N')
                        {
                            oPortfolio.TransDesc = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                                   oRowView["Stockcode"].ToString().Trim() + " @ " +
                                                   decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim();
                        }
                        else
                        {
                            oPortfolio.TransDesc = "Cross Deal Pur: " + oRowView["Units"].ToString().Trim() + " " +
                                                   oRowView["Stockcode"].ToString().Trim() + " @ " +
                                                   decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim();
                        }

                        oPortfolio.TransType = "STKBSALE";
                        oPortfolio.SysRef = "STKB-" + strAllotmentNo;
                        break;
                    }
                case "S":
                    oPortfolio.DebCred = "D";
                    oPortfolio.TransDesc = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                           oRowView["Stockcode"].ToString().Trim() + " @ " +
                                           decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim();
                    oPortfolio.TransType = "COLTSALE";
                    oPortfolio.SysRef = "COLT-" + strAllotmentNo;
                    break;
            }

            oPortfolio.MarginCode = "";
            var dbCommandPort = oPortfolio.AddCommand();
            db.ExecuteNonQuery(dbCommandPort, transaction);

            if (oAllot.Cdeal != 'Y') return;
            oPortfolio.PurchaseDate = oRowView["Date"].ToString().Trim().ToDate();
            oPortfolio.CustomerAcct = oRowView["CrossCustNo"].ToString();
            oPortfolio.StockCode = oRowView["Stockcode"].ToString().Trim();
            oPortfolio.Units = long.Parse(oRowView["Units"].ToString().Trim());
            oPortfolio.UnitPrice = float.Parse(oAllot.UnitPrice.ToString().Trim());
            if (oAllot.Cdeal == 'N')
            {
                oPortfolio.ActualUnitCost = float.Parse(oAllot.TotalAmount.ToString()) /
                                            long.Parse(oRowView["Units"].ToString().Trim());
                oPortfolio.TotalCost = oAllot.TotalAmount;
            }
            else
            {
                oPortfolio.ActualUnitCost = float.Parse(decCrossDealTotalAmount.ToString()) /
                                            long.Parse(oRowView["Units"].ToString().Trim());
                oPortfolio.TotalCost = decCrossDealTotalAmount;
            }

            oPortfolio.Ref01 = strAllotmentNo2;
            oPortfolio.DebCred = "D";
            oPortfolio.TransDesc = "Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                   oRowView["Stockcode"].ToString().Trim() + " @ " +
                                   decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim();
            oPortfolio.TransType = "COLTSALE";
            oPortfolio.SysRef = "STKB-" + strAllotmentNo;
            oPortfolio.MarginCode = "";
            var dbCommandPort2 = oPortfolio.AddCommand();
            db.ExecuteNonQuery(dbCommandPort2, transaction);
        }

        private static decimal GlPostingForSell(Allotment oAllot, AcctGL oGL, DataRow oRowView, ProductAcct oCust,
            Customer oCustomer, StkParam oStkbPGenTable, string strInvestmentProductAccount,
            string strInvestmentProductBondAccount, string strStockProductAccount, GLParam oGLParamBoxSaleProfit,
            string strCommissionSeperationInCustomerAccount, decimal TotalFeeSeller, decimal TotalFeeSellerPost,
            decimal TotalFeeBuyer, decimal decFeeCommissionSeller, Customer oCustomerCross, string strAllotmentNo2,
            string strAllotmentNo, string strJnumberNext, string strDefaultBranchCode, PGenTable oPGenTable, SqlDatabase db,
            SqlTransaction transaction, string strPostCommShared, string strAgentProductAccount,
            CustomerExtraInformation oCustomerExtraInformationDirectCash, string strPostInvPLControlAcct,
            decimal decTotalDeductCommission, ref decimal decTotalDeductCommissionSell,
            ref decimal decTotalDeductCommissionBond, ref decimal decTotalDeductCommissionSellBond)
        {
            if (oAllot.Cdeal == 'Y')
            {
                #region Posting To Customer For Ordinary/Norminal Cross Seller,NB NS Cross Deal And Posting To Commission Separate

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                if ((oRowView["CrossType"].ToString().Trim() != "NB") && (oRowView["CrossType"].ToString().Trim() != "NA"))
                {
                    oGL.AccountID = oRowView["CrossCustNo"].ToString().Trim();
                    oCust.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                    oCustomer.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                }
                else
                {
                    oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                    oCust.CustAID = oRowView["CustNo"].ToString().Trim();
                    oCustomer.CustAID = oRowView["CustNo"].ToString().Trim();
                }

                var oProduct = new Product();
                if (oCust.GetBoxLoadStatus())
                {
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        oProduct.TransNo = strInvestmentProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentProductAccount;
                    }
                    else
                    {
                        oProduct.TransNo = strInvestmentProductBondAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentProductBondAccount;
                    }
                }
                else
                {
                    oProduct.TransNo = strStockProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strStockProductAccount;
                }

                if (!oCustomer.GetCustomerName(strStockProductAccount))
                {
                    throw new Exception("Missing Customer Account");
                }

                decimal decSellDiffTotal = 0;
                decimal decGetUnitCost = 0;
                var decActualSellPrice = oAllot.TotalAmount / decimal.Parse(oRowView["Units"].ToString().Trim());
                var decSellDiff = decActualSellPrice - decGetUnitCost;
                if ((oCust.GetBoxLoadStatus()) && (oGLParamBoxSaleProfit.CheckParameter() == "YES"))
                {
                    var oPortForSaleProfit = new Portfolio
                    {
                        PurchaseDate = oRowView["Date"].ToString().Trim().ToDate(),
                        StockCode = oRowView["Stockcode"].ToString().Trim(),
                        CustomerAcct = oRowView["CrossCustNo"].ToString().Trim()
                    };
                    decGetUnitCost = decimal.Parse(oPortForSaleProfit.GetUnitCost().ToString());
                    decGetUnitCost = Math.Round(decGetUnitCost, 2, MidpointRounding.AwayFromZero);
                    decActualSellPrice = Math.Round(decActualSellPrice, 2, MidpointRounding.AwayFromZero);
                    decSellDiff = Math.Round(decSellDiff, 2, MidpointRounding.AwayFromZero);
                    decSellDiffTotal = decSellDiff * decimal.Parse(oRowView["Units"].ToString().Trim());
                    decSellDiffTotal = Math.Round(decSellDiffTotal, 2, MidpointRounding.AwayFromZero);
                }

                if ((oRowView["CrossType"].ToString().Trim() == "NO") || (oRowView["CrossType"].ToString().Trim() == "NA")
                                                                      || (oRowView["CrossType"].ToString().Trim() == "NT"))
                {
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        oGL.Credit = oAllot.TotalAmount + oAllot.Commission;
                    }
                    else
                    {
                        oGL.Credit = oAllot.TotalAmount;
                    }

                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                }
                else if (oRowView["CrossType"].ToString().Trim() == "CD")
                {
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        oGL.Debit = TotalFeeSeller - oAllot.Commission;
                    }
                    else
                    {
                        oGL.Debit = TotalFeeSeller;
                    }

                    oGL.Credit = 0;
                    oGL.Debcred = "D";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                }
                else if (oRowView["CrossType"].ToString().Trim() == "NB" || oRowView["CrossType"].ToString().Trim() == "NS")
                {
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        oGL.Debit = (TotalFeeSellerPost + TotalFeeBuyer) - decFeeCommissionSeller;
                    }
                    else
                    {
                        oGL.Debit = TotalFeeSellerPost + TotalFeeBuyer;
                    }

                    oGL.Credit = 0;
                    oGL.Debcred = "D";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                }

                oGL.FeeType = "SCUM";
                if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                {
                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString());
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "CD"))
                {
                    oGL.Desciption = "Cross Deal Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString());
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "NB"))
                {
                    oGL.Desciption = "Stock Purchase (Cross:Buyer Bear All Charges): " + oRowView["Units"].ToString().Trim() +
                                     " " + oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString(CultureInfo.InvariantCulture)) + " - " + oCustomer.Surname.Trim() +
                                     " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "NA"))
                {
                    oGL.Desciption = "Stock Sale (Cross:Buyer Bear All Charges): " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString(CultureInfo.InvariantCulture)) + " - " +
                                     oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                     oCustomerCross.Othername.Trim();
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "NS") || (oRowView["CrossType"].ToString().Trim() == "NT"))
                {
                    oGL.Desciption = "Stock Sale (Cross:Seller Bear All Charges): " + oRowView["Units"].ToString().Trim() +
                                     " " + oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString(CultureInfo.InvariantCulture)) + " - " +
                                     oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                     oCustomerCross.Othername.Trim();
                }

                if ((oRowView["CrossType"].ToString().Trim() == "NO") || (oRowView["CrossType"].ToString().Trim() == "NA")
                                                                      || (oRowView["CrossType"].ToString().Trim() == "NT"))
                {
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                }
                else
                {
                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                }

                oGL.TransType = "STKBSALE";
                oGL.Ref01 = strAllotmentNo2;
                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.Ref02 = strAllotmentNo;
                oGL.Chqno = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                oGL.RecAcct = "";
                oGL.PostToOtherBranch = "Y";
                var dbCommandCustomer = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandCustomer, transaction);


                if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                {
                    oGL.Credit = 0;
                    if ((oRowView["CrossType"].ToString().Trim() == "NO") || (oRowView["CrossType"].ToString().Trim() == "CD")
                                                                          || (oRowView["CrossType"].ToString().Trim() ==
                                                                              "NA") ||
                                                                          (oRowView["CrossType"].ToString().Trim() == "NT"))
                    {
                        oGL.Debit = oAllot.Commission;
                    }
                    else
                    {
                        oGL.Debit = decFeeCommissionSeller;
                    }

                    oGL.Debcred = "D";
                    oGL.FeeType = oGL.FeeType + "SCM";
                    oGL.Desciption = "Commission Charge On " + oGL.Desciption;
                    var dbCommandCustomerSeperateCommission = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandCustomerSeperateCommission, transaction);
                }

                oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege    

                #endregion

                #region Second Leg Of Customer To Jobbing For Ordinary/Norminal Cross Seller,NB NS Cross Deal

                decimal decDeductCommJobbing = 0;
                if (strPostCommShared.Trim() == "NO")
                {
                    var oProductAcctAgentJobbing = new ProductAcct();
                    oProductAcctAgentJobbing.ProductCode = strStockProductAccount;
                    oProductAcctAgentJobbing.CustAID = oRowView["CustNo"].ToString().Trim();
                    if (oProductAcctAgentJobbing.GetCustomerAgent())
                    {
                        var oCustomerAgentJobbing = new Customer();
                        oProductAcctAgentJobbing.ProductCode = strAgentProductAccount;
                        oProductAcctAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                        oCustomerAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                        if (oProductAcctAgentJobbing.GetAgentCommission())
                        {
                            if (oCustomerAgentJobbing.GetCustomerName(strAgentProductAccount))
                            {
                                if ((oRowView["CrossType"].ToString().Trim() == "NO") ||
                                    (oRowView["CrossType"].ToString().Trim() == "CD"))
                                {
                                    decDeductCommJobbing = (oAllot.Commission * oProductAcctAgentJobbing.AgentComm) / 100;
                                }
                                else
                                {
                                    decDeductCommJobbing = (decFeeCommissionSeller * oProductAcctAgentJobbing.AgentComm) / 100;
                                }

                                decDeductCommJobbing = Math.Round((decimal)decDeductCommJobbing, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                }

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oGL.MasterID = oPGenTable.ShInv;
                oGL.AccountID = "";
                oGL.RecAcct = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                if ((oRowView["CrossType"].ToString().Trim() == "NO") || (oRowView["CrossType"].ToString().Trim() == "NA")
                                                                      || (oRowView["CrossType"].ToString().Trim() == "NT"))
                {
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        //oGL.Debit = oAllot.TotalAmount + decDeductCommJobbing;
                        oGL.Debit = oAllot.TotalAmount;
                    }
                    else
                    {
                        oGL.Debit = oAllot.TotalAmount;
                    }

                    oGL.Credit = 0;
                    oGL.Debcred = "D";
                }
                else if (oRowView["CrossType"].ToString().Trim() == "CD")
                {
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        //oGL.Credit = TotalFeeSeller - decDeductCommJobbing;
                        oGL.Credit = TotalFeeSeller;
                    }
                    else
                    {
                        oGL.Credit = TotalFeeSeller;
                    }

                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                }
                else if (oRowView["CrossType"].ToString().Trim() == "NB" || oRowView["CrossType"].ToString().Trim() == "NS")
                {
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        //oGL.Credit = (TotalFeeSellerPost + TotalFeeBuyer) - decDeductCommJobbing;
                        oGL.Credit = (TotalFeeSellerPost + TotalFeeBuyer);
                    }
                    else
                    {
                        oGL.Credit = (TotalFeeSellerPost + TotalFeeBuyer);
                    }

                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                }

                if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                {
                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString());
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "CD"))
                {
                    oGL.Desciption = "Cross Deal Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString());
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "NB"))
                {
                    oGL.Desciption = "Stock Purchase (Cross:Buyer Bear All Charges): " + oRowView["Units"].ToString().Trim() +
                                     " " + oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()) + " - " + oCustomer.Surname.Trim() +
                                     " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "NA"))
                {
                    oGL.Desciption = "Stock Sale (Cross:Buyer Bear All Charges): " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()) + " - " +
                                     oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                     oCustomerCross.Othername.Trim();
                }
                else if ((oRowView["CrossType"].ToString().Trim() == "NS") || (oRowView["CrossType"].ToString().Trim() == "NT"))
                {
                    oGL.Desciption = "Stock Sale (Cross:Seller Bear All Charges): " + oRowView["Units"].ToString().Trim() +
                                     " " + oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()) + " - " +
                                     oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                     oCustomerCross.Othername.Trim();
                }

                if ((oRowView["CrossType"].ToString().Trim() == "NO") || (oRowView["CrossType"].ToString().Trim() == "NA")
                                                                      || (oRowView["CrossType"].ToString().Trim() == "NT"))
                {
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                }
                else
                {
                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                }

                oGL.TransType = "STKBSALE";
                oGL.Ref01 = strAllotmentNo2;
                if ((oRowView["CrossType"].ToString().Trim() != "NB"))
                {
                    oGL.Ref02 = oRowView["CrossCustNo"].ToString().Trim();
                }
                else
                {
                    oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                }

                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGL.AcctRef = "";
                oGL.Chqno = "";
                oGL.FeeType = "CTRAD";
                var dbCommandJobbingCrossProper = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandJobbingCrossProper, transaction);

                #endregion

                #region Direct Cash Settlement

                oCustomerExtraInformationDirectCash.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                oCustomerExtraInformationDirectCash.WedAnniversaryDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                if (oCustomerExtraInformationDirectCash.DirectCashSettlementNASD &&
                    ((oRowView["CrossType"].ToString().Trim() == "NO")
                     || (oRowView["CrossType"].ToString().Trim() == "NA") || (oRowView["CrossType"].ToString().Trim() == "NT")))
                {
                    #region Posting To Customer For Ordinary Cross Seller Only Direct Cash Settlement

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.AccountID = oRowView["CrossCustNo"].ToString().Trim();
                    oCust.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                    oCustomer.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                    if (oCust.GetBoxLoadStatus())
                    {
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            oProduct.TransNo = strInvestmentProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductAccount;
                        }
                        else
                        {
                            oProduct.TransNo = strInvestmentProductBondAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductBondAccount;
                        }
                    }
                    else
                    {
                        oProduct.TransNo = strStockProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strStockProductAccount;
                    }

                    if (!oCustomer.GetCustomerName(strStockProductAccount))
                    {
                        throw new Exception("Missing Customer Account");
                    }

                    oGL.Debit = oAllot.TotalAmount;

                    oGL.Credit = 0;
                    oGL.Debcred = "D";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                    oGL.FeeType = "SCUMDCS";
                    oGL.Desciption = "Direct Cash Settlement For Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString());
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                    oGL.TransType = "STKBSALE";
                    oGL.Ref01 = strAllotmentNo2;
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.Ref02 = strAllotmentNo;
                    oGL.Chqno = "";
                    oGL.RecAcctMaster = oPGenTable.DirectCashSettleAcct;
                    oGL.RecAcct = "";
                    oGL.PostToOtherBranch = "Y";
                    var dbCommandCustomerDirectCashSettlement = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandCustomerDirectCashSettlement, transaction);

                    oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege    

                    #endregion

                    #region Second Leg To Direct Cash Settlement Account For Ordinary Cross Seller Only Direct Cash Settlement

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.MasterID = oPGenTable.DirectCashSettleAcct;
                    oGL.AccountID = "";
                    if (oCust.GetBoxLoadStatus())
                    {
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            oProduct.TransNo = strInvestmentProductAccount;
                            oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                            oGL.AcctRefSecond = strInvestmentProductAccount;
                        }
                        else
                        {
                            oProduct.TransNo = strInvestmentProductBondAccount;
                            oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                            oGL.AcctRefSecond = strInvestmentProductBondAccount;
                        }
                    }
                    else
                    {
                        oProduct.TransNo = strStockProductAccount;
                        oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                        oGL.AcctRefSecond = strStockProductAccount;
                    }

                    oGL.RecAcct = oRowView["CrossCustNo"].ToString().Trim();
                    oGL.Credit = oAllot.TotalAmount;
                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                    oGL.Desciption = "Direct Cash Settlement For Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()) + " By " + oCustomer.Surname.Trim() +
                                     " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                    oGL.TransType = "STKBSALE";
                    oGL.Ref01 = strAllotmentNo2;
                    oGL.Ref02 = oRowView["CrossCustNo"].ToString().Trim();
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGL.AcctRef = "";
                    oGL.Chqno = "";
                    oGL.FeeType = "CTRADDCS";
                    var dbCommandJobbingCrossProperDirectCashSettlement = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandJobbingCrossProperDirectCashSettlement, transaction);

                    #endregion
                }

                #endregion

                #region Posting To Selling Customer Capital Gain For All Except Nominal Buy Cross Deal

                if ((oRowView["CrossType"].ToString().Trim() != "NB"))
                {
                    if ((oCust.GetBoxLoadStatus()) && (decSellDiffTotal != 0) &&
                        (oGLParamBoxSaleProfit.CheckParameter() == "YES"))

                    {
                        #region Posting Gain Or Loss To Income Account

                        oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                        oGL.MasterID = oPGenTable.CapGain;
                        oGL.AccountID = "";
                        if (decSellDiffTotal < 0)
                        {
                            oGL.Credit = 0;
                            oGL.Debit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                            oGL.Debcred = "D";
                            oGL.Desciption = "Cap Loss Appr. from Sale of: " + oRowView["Units"].ToString().Trim() + " " +
                                             oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") +
                                             " SP @ " + decActualSellPrice.ToString("n").Trim() + " By " +
                                             oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                             oCustomer.Othername.Trim();
                        }
                        else
                        {
                            oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.Desciption = "Cap Gain Appr. from Sale of: " + oRowView["Units"].ToString().Trim() + " " +
                                             oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") +
                                             " SP @ " + decActualSellPrice.ToString("n").Trim() + " By " +
                                             oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                             oCustomer.Othername.Trim();
                        }

                        oGL.FeeType = "SGIV";
                        oGL.TransType = "STKBSALE";
                        oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                        oGL.Ref01 = strAllotmentNo2;
                        oGL.Ref02 = oCust.CustAID;
                        oGL.AcctRef = "";
                        oGL.Reverse = "N";
                        oGL.Jnumber = strJnumberNext;
                        oGL.Branch = strDefaultBranchCode;
                        oGL.Chqno = "";
                        oGL.RecAcctMaster = oPGenTable.ShInv;
                        oGL.RecAcct = "";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        var dbCommandSellProfitOrLoss = oGL.AddCommand();
                        db.ExecuteNonQuery(dbCommandSellProfitOrLoss, transaction);

                        #endregion

                        #region Posting Capital Gain Or Loss To Box Load Account Or P/L Control Account

                        oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                        oCust.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                        if (strPostInvPLControlAcct.Trim() != "YES")
                        {
                            oGL.AccountID = oRowView["CrossCustNo"].ToString().Trim();
                            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                            {
                                oProduct.TransNo = strInvestmentProductAccount;
                                oGL.MasterID = oProduct.GetProductGLAcct();
                                oGL.AcctRef = strInvestmentProductAccount;
                            }
                            else
                            {
                                oProduct.TransNo = strInvestmentProductBondAccount;
                                oGL.MasterID = oProduct.GetProductGLAcct();
                                oGL.AcctRef = strInvestmentProductBondAccount;
                            }
                        }
                        else
                        {
                            oGL.AccountID = "";
                            oGL.MasterID = oPGenTable.CapGainContra;
                            oGL.AcctRef = "";
                        }

                        oGL.FeeType = "SGIB";
                        oGL.Desciption = "Stock Sale P/L: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                        oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                        oGL.TransType = "STKBSALE";
                        oGL.Ref01 = strAllotmentNo2;
                        oGL.Ref02 = strAllotmentNo;
                        if (decSellDiffTotal < 0)
                        {
                            oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                        }
                        else
                        {
                            oGL.Credit = 0;
                            oGL.Debit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                            oGL.Debcred = "D";
                        }

                        oGL.Reverse = "N";
                        oGL.Jnumber = strJnumberNext;
                        oGL.Branch = strDefaultBranchCode;
                        oGL.Chqno = "";
                        oGL.RecAcctMaster = oPGenTable.ShInv;
                        oGL.RecAcct = "";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        var dbCommandBoxLoadProfitLoss = oGL.AddCommand();
                        db.ExecuteNonQuery(dbCommandBoxLoadProfitLoss, transaction);

                        #endregion
                    }
                }

                #endregion

                #region Agent Posting Seller

                var oCustomerAgentPro2 = new Customer();
                oCustomerAgentPro2.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                var oCustAgentPro2 = new ProductAcct();
                oCustAgentPro2.ProductCode = strStockProductAccount;
                oCustAgentPro2.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                decimal decDeductComm = 0;
                if (oCustAgentPro2.GetCustomerAgent())
                {
                    oCustAgentPro2.ProductCode = strAgentProductAccount;
                    oCustAgentPro2.CustAID = oCustAgentPro2.Agent.Trim();
                    oCustomerAgentPro2.CustAID = oCustAgentPro2.Agent.Trim();
                    if (oCustAgentPro2.GetAgentCommission())
                    {
                        if (oCustomerAgentPro2.GetCustomerName(strAgentProductAccount))
                        {
                            if (strPostCommShared.Trim() == "YES")
                            {
                                if ((oRowView["CrossType"].ToString().Trim() == "NO") ||
                                    (oRowView["CrossType"].ToString().Trim() == "CD"))
                                {
                                    decDeductComm = (oAllot.Commission * oCustAgentPro2.AgentComm) / 100;
                                }
                                else
                                {
                                    decDeductComm = (decFeeCommissionSeller * oCustAgentPro2.AgentComm) / 100;
                                }

                                decDeductComm = Math.Round((decimal)decDeductComm, 2, MidpointRounding.AwayFromZero);
                                if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                                {
                                    decTotalDeductCommission = decTotalDeductCommission + decDeductComm;
                                    decTotalDeductCommission =
                                        Math.Round(decTotalDeductCommission, 2, MidpointRounding.AwayFromZero);
                                    decTotalDeductCommissionSell = decTotalDeductCommissionSell + decDeductComm;
                                    decTotalDeductCommissionSell = Math.Round(decTotalDeductCommissionSell, 2,
                                        MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    decTotalDeductCommissionBond = decTotalDeductCommissionBond + decDeductComm;
                                    decTotalDeductCommissionBond = Math.Round(decTotalDeductCommissionBond, 2,
                                        MidpointRounding.AwayFromZero);
                                    decTotalDeductCommissionSellBond = decTotalDeductCommissionSellBond + decDeductComm;
                                    decTotalDeductCommissionSellBond = Math.Round(decTotalDeductCommissionSellBond, 2,
                                        MidpointRounding.AwayFromZero);
                                }
                            }

                            oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                            oGL.MasterID = strPostCommShared.Trim() == "YES" ? oPGenTable.ShInv : oPGenTable.AgComm;

                            oGL.AccountID = "";
                            if ((oRowView["CrossType"].ToString().Trim() == "NO") ||
                                (oRowView["CrossType"].ToString().Trim() == "CD"))
                            {
                                oGL.Debit = (oAllot.Commission * oCustAgentPro2.AgentComm) / 100;
                            }
                            else
                            {
                                oGL.Debit = (decFeeCommissionSeller * oCustAgentPro2.AgentComm) / 100;
                            }

                            oGL.Debit = Math.Round(oGL.Debit, 2, MidpointRounding.AwayFromZero);
                            oGL.Credit = 0;
                            oGL.Debcred = "D";
                            oGL.FeeType = "SECM";
                            if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                            {
                                oGL.Desciption = "Commission On Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                                oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                            }
                            else
                            {
                                oGL.Desciption = "Commission On Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                                oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                            }

                            oGL.TransType = "STKBSALE";
                            oGL.Ref01 = strAllotmentNo2;
                            oGL.AcctRef = "";
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = strDefaultBranchCode;
                            oGL.Ref02 = oRowView["CrossCustNo"].ToString().Trim();
                            oGL.Chqno = strAllotmentNo;
                            oGL.RecAcctMaster = oPGenTable.ShInv;
                            oGL.RecAcct = "";
                            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            var dbCommandAgentExpense2 = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandAgentExpense2, transaction);

                            oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                            oGL.AccountID = oCustAgentPro2.CustAID.Trim();
                            oProduct.TransNo = strAgentProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strAgentProductAccount;
                            if ((oRowView["CrossType"].ToString().Trim() == "NO") ||
                                (oRowView["CrossType"].ToString().Trim() == "CD"))
                            {
                                oGL.Credit = (oAllot.Commission * oCustAgentPro2.AgentComm) / 100;
                            }
                            else
                            {
                                oGL.Credit = (decFeeCommissionSeller * oCustAgentPro2.AgentComm) / 100;
                            }

                            oGL.Credit = Math.Round(oGL.Credit, 2, MidpointRounding.AwayFromZero);
                            oGL.Debit = 0;
                            oGL.Debcred = "C";
                            oGL.FeeType = "SBPY";
                            if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                            {
                                oGL.Desciption = "Commission On Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                                oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                            }
                            else
                            {
                                oGL.Desciption = "Commission On Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                                oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                            }

                            oGL.TransType = "STKBSALE";
                            oGL.Ref01 = strAllotmentNo2;
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = strDefaultBranchCode;
                            oGL.Ref02 = oRowView["CrossCustNo"].ToString().Trim();
                            oGL.Chqno = strAllotmentNo;
                            oGL.RecAcctMaster = oPGenTable.ShInv;
                            oGL.RecAcct = "";
                            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                            var dbCommandBrokPayable2 = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandBrokPayable2, transaction);
                        }
                    }
                }

                #endregion

                if ((oRowView["CrossType"].ToString().Trim() == "NB") || (oRowView["CrossType"].ToString().Trim() == "NS"))
                {
                    #region Posting Zero Transaction To Second Customer In 1.NB Cross Deal 2.NS Cross Deal

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    if ((oRowView["CrossType"].ToString().Trim() == "NB"))
                    {
                        oGL.AccountID = oRowView["CrossCustNo"].ToString().Trim();
                        oCust.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                        oCustomer.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                    }
                    else
                    {
                        oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                        oCust.CustAID = oRowView["CustNo"].ToString().Trim();
                        oCustomer.CustAID = oRowView["CustNo"].ToString().Trim();
                    }

                    if (oCust.GetBoxLoadStatus())
                    {
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            oProduct.TransNo = strInvestmentProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductAccount;
                        }
                        else
                        {
                            oProduct.TransNo = strInvestmentProductBondAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductBondAccount;
                        }
                    }
                    else
                    {
                        oProduct.TransNo = strStockProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strStockProductAccount;
                    }

                    if (!oCustomer.GetCustomerName(strStockProductAccount))
                    {
                        throw new Exception("Missing Customer Account");
                    }

                    decSellDiffTotal = 0;
                    if ((oCust.GetBoxLoadStatus()) && (oGLParamBoxSaleProfit.CheckParameter() == "YES") &&
                        (oRowView["CrossType"].ToString().Trim() == "NB"))
                    {
                        var oPortForSaleProfit = new Portfolio();
                        oPortForSaleProfit.PurchaseDate =
                            oRowView["Date"].ToString().Trim().ToDate();
                        oPortForSaleProfit.StockCode = oRowView["Stockcode"].ToString().Trim();
                        oPortForSaleProfit.CustomerAcct = oRowView["CrossCustNo"].ToString().Trim();
                        decGetUnitCost = decimal.Parse(oPortForSaleProfit.GetUnitCost().ToString(CultureInfo.InvariantCulture));
                        decGetUnitCost = Math.Round(decGetUnitCost, 2, MidpointRounding.AwayFromZero);
                        decActualSellPrice = oAllot.TotalAmount / decimal.Parse(oRowView["Units"].ToString().Trim());
                        decActualSellPrice = Math.Round(decActualSellPrice, 2, MidpointRounding.AwayFromZero);
                        decSellDiff = decActualSellPrice - decGetUnitCost;
                        decSellDiff = Math.Round(decSellDiff, 2, MidpointRounding.AwayFromZero);
                        decSellDiffTotal = decSellDiff * decimal.Parse(oRowView["Units"].ToString().Trim());
                        decSellDiffTotal = Math.Round(decSellDiffTotal, 2, MidpointRounding.AwayFromZero);
                    }

                    oGL.Credit = 0;
                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                    oGL.FeeType = "SCUM";
                    if ((oRowView["CrossType"].ToString().Trim() == "NB"))
                    {
                        oGL.Desciption = "Stock Sale (Cross:Buyer Bear All Charges): " + oRowView["Units"].ToString().Trim() +
                                         " " + oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oAllot.UnitPrice.ToString()) + " - " +
                                         oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                         oCustomerCross.Othername.Trim();
                    }
                    else if ((oRowView["CrossType"].ToString().Trim() == "NS"))
                    {
                        oGL.Desciption = "Stock Purchase (Cross:Seller Bear All Charges): " +
                                         oRowView["Units"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() +
                                         " @ " + decimal.Parse(oAllot.UnitPrice.ToString()) + " - " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }

                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                    oGL.TransType = "STKBSALE";
                    oGL.Ref01 = strAllotmentNo2;
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.Ref02 = strAllotmentNo;
                    oGL.Chqno = "PZR";
                    oGL.RecAcctMaster = oPGenTable.ShInv;
                    oGL.RecAcct = "";
                    oGL.PostToOtherBranch = "Y";
                    var dbCommandCustomerCross = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandCustomerCross, transaction);

                    oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege    

                    #endregion

                    if ((oRowView["CrossType"].ToString().Trim() == "NB"))
                    {
                        #region Posting To Selling Customer Capital Gain For Nominal Buy Cross Deal

                        if ((oRowView["CrossType"].ToString().Trim() != "NB"))
                        {
                            if ((oCust.GetBoxLoadStatus()) && (decSellDiffTotal != 0) &&
                                (oGLParamBoxSaleProfit.CheckParameter() == "YES"))
                            {
                                #region Posting Gain Or Loss To Income Account

                                oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                                oGL.MasterID = oPGenTable.CapGain;
                                oGL.AccountID = "";
                                if (decSellDiffTotal < 0)
                                {
                                    oGL.Credit = 0;
                                    oGL.Debit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                                    oGL.Debcred = "D";
                                    oGL.Desciption = "Cap Loss Appr. from Sale of: " + oRowView["Units"].ToString().Trim() +
                                                     " " + oRowView["StockCode"].ToString().Trim() + " CP @ " +
                                                     decGetUnitCost.ToString("n") + " SP @ " +
                                                     decActualSellPrice.ToString("n").Trim() + " By " +
                                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                     oCustomer.Othername.Trim();
                                }
                                else
                                {
                                    oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                                    oGL.Debit = 0;
                                    oGL.Debcred = "C";
                                    oGL.Desciption = "Cap Gain Appr. from Sale of: " + oRowView["Units"].ToString().Trim() +
                                                     " " + oRowView["StockCode"].ToString().Trim() + " CP @ " +
                                                     decGetUnitCost.ToString("n") + " SP @ " +
                                                     decActualSellPrice.ToString("n").Trim() + " By " +
                                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                     oCustomer.Othername.Trim();
                                }

                                oGL.FeeType = "SGIV";
                                oGL.TransType = "STKBSALE";
                                oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                                oGL.Ref01 = strAllotmentNo2;
                                oGL.Ref02 = oCust.CustAID;
                                oGL.AcctRef = "";
                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Branch = strDefaultBranchCode;
                                oGL.Chqno = "";
                                oGL.RecAcctMaster = oPGenTable.ShInv;
                                oGL.RecAcct = "";
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                var dbCommandSellProfitOrLoss = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandSellProfitOrLoss, transaction);

                                #endregion

                                #region Posting Capital Gain Or Loss To Box Load Account Or P/L Control Account

                                oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                                oCust.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                                if (strPostInvPLControlAcct.Trim() != "YES")
                                {
                                    oGL.AccountID = oRowView["CrossCustNo"].ToString().Trim();
                                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                                    {
                                        oProduct.TransNo = strInvestmentProductAccount;
                                        oGL.MasterID = oProduct.GetProductGLAcct();
                                        oGL.AcctRef = strInvestmentProductAccount;
                                    }
                                    else
                                    {
                                        oProduct.TransNo = strInvestmentProductBondAccount;
                                        oGL.MasterID = oProduct.GetProductGLAcct();
                                        oGL.AcctRef = strInvestmentProductBondAccount;
                                    }
                                }
                                else
                                {
                                    oGL.AccountID = "";
                                    oGL.MasterID = oPGenTable.CapGainContra;
                                    oGL.AcctRef = "";
                                }

                                oGL.FeeType = "SGIB";
                                oGL.Desciption = "Stock Sale P/L: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                                oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                                oGL.TransType = "STKBSALE";
                                oGL.Ref01 = strAllotmentNo2;
                                oGL.Ref02 = strAllotmentNo;
                                if (decSellDiffTotal < 0)
                                {
                                    oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                                    oGL.Debit = 0;
                                    oGL.Debcred = "C";
                                }
                                else
                                {
                                    oGL.Credit = 0;
                                    oGL.Debit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                                    oGL.Debcred = "D";
                                }

                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Branch = strDefaultBranchCode;
                                oGL.Chqno = "";
                                oGL.RecAcctMaster = oPGenTable.ShInv;
                                oGL.RecAcct = "";
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                var dbCommandBoxLoadProfitLoss = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandBoxLoadProfitLoss, transaction);

                                #endregion
                            }
                        }

                        #endregion
                    }
                }
            }

            return decTotalDeductCommission;
        }

        private static decimal GlPostingForBuy(Allotment oAllot, DataRow oRowView, ProductAcct oCust, Customer oCustomer,
            StkParam oStkbPGenTable, string strInvestmentProductAccount, string strInvestmentProductBondAccount,
            string strStockProductAccount, string strCommissionSeperationInCustomerAccount, decimal decFeeTotalAmount,
            decimal decFeeCommission, string strAllotmentNo, GLParam oGLParamBoxSaleProfit, string strJnumberNext,
            string strDefaultBranchCode, PGenTable oPGenTable, SqlDatabase db, SqlTransaction transaction,
            string strPostCommShared, string strAgentProductAccount,
            CustomerExtraInformation oCustomerExtraInformationDirectCash, string strPostInvPLControlAcct, decimal TotalFeeBuyer,
            decimal decTotalDeductCommissionBuy, Customer oCustomerCross, string strBranchBuySellCharge,
            ref AcctGL oGL, ref decimal decTotalDeductCommissionSell, ref decimal decTotalDeductCommission,
            ref decimal decTotalDeductCommissionBuyBond, ref decimal decTotalDeductCommissionSellBond,
            ref decimal decTotalDeductCommissionBond)
        {
            decimal decActualSellPrice = 0;
            decimal decDeductCommJobbing;
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO")
                                      || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                                      (oRowView["CrossType"].ToString().Trim() == "NT"))
            {
                #region Customer Posting For Single Buy/Sell,Ordinary Cross Buy,NA and NT Buy And Posting To Commission Separate

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();

                if ((oRowView["CrossType"].ToString().Trim() != "NT"))
                {
                    oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                    oCust.CustAID = oRowView["CustNo"].ToString().Trim();
                    oCustomer.CustAID = oRowView["CustNo"].ToString().Trim();
                }
                else
                {
                    oGL.AccountID = oRowView["CrossCustNo"].ToString().Trim();
                    oCust.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                    oCustomer.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                }

                var oProduct = new Product();
                if (oCust.GetBoxLoadStatus())
                {
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        oProduct.TransNo = strInvestmentProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentProductAccount;
                    }
                    else
                    {
                        oProduct.TransNo = strInvestmentProductBondAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentProductBondAccount;
                    }
                }
                else
                {
                    oProduct.TransNo = strStockProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strStockProductAccount;
                }

                decimal decSellDiffTotal = 0;
                decimal decGetUnitCost = 0;
                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                {
                    oGL.Credit = 0;
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        oGL.Debit = decFeeTotalAmount - decFeeCommission;
                    }
                    else
                    {
                        oGL.Debit = decFeeTotalAmount;
                    }

                    oGL.Debcred = "D";
                    oGL.FeeType = "BCUM";

                    if ((oRowView["CrossType"].ToString().Trim() == "NA"))
                    {
                        oGL.Desciption = "Stock Purchase (Cross:Buyer Bear All Charges): " +
                                         oRowView["Units"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() +
                                         " @ " + decimal.Parse(oAllot.UnitPrice.ToString()).ToString() + " - " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else if ((oRowView["CrossType"].ToString().Trim() == "NT"))
                    {
                        oGL.Desciption = "Stock Purchase (Cross:Seller Bear All Charges): " +
                                         oRowView["Units"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() +
                                         " @ " + decimal.Parse(oAllot.UnitPrice.ToString()).ToString() + " - " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n");
                    }

                    oGL.TransType = "STKBBUY";
                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGL.ClearingDayForTradingTransaction = "Y";
                }
                else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                {
                    if ((oCust.GetBoxLoadStatus()) && (oGLParamBoxSaleProfit.CheckParameter() == "YES"))
                    {
                        var oPortForSaleProfit = new Portfolio();
                        oPortForSaleProfit.PurchaseDate =
                            oRowView["Date"].ToString().Trim().ToDate();
                        oPortForSaleProfit.CustomerAcct = oRowView["CustNo"].ToString().Trim();
                        oPortForSaleProfit.StockCode = oRowView["Stockcode"].ToString().Trim();
                        decGetUnitCost = decimal.Parse(oPortForSaleProfit.GetUnitCost().ToString());
                        decGetUnitCost = Math.Round(decGetUnitCost, 2, MidpointRounding.AwayFromZero);
                        decActualSellPrice = oAllot.TotalAmount / decimal.Parse(oRowView["Units"].ToString().Trim());
                        decActualSellPrice = Math.Round(decActualSellPrice, 2);
                        var decSellDiff = decActualSellPrice - decGetUnitCost;
                        decSellDiff = Math.Round(decSellDiff, 2, MidpointRounding.AwayFromZero);
                        decSellDiffTotal = decSellDiff * decimal.Parse(oRowView["Units"].ToString().Trim());
                        decSellDiffTotal = Math.Round(decSellDiffTotal, 2, MidpointRounding.AwayFromZero);
                    }

                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        oGL.Credit = oAllot.TotalAmount + oAllot.Commission;
                    }
                    else
                    {
                        oGL.Credit = oAllot.TotalAmount;
                    }

                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                    oGL.FeeType = "SCUM";
                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n");
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                    oGL.ClearingDayForTradingTransaction = "Y";
                }

                oGL.Ref01 = strAllotmentNo;
                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.Ref02 = "";
                oGL.Chqno = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                oGL.RecAcct = "";
                oGL.PostToOtherBranch = "Y";
                var dbCommandCustomer = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandCustomer, transaction);

                #region Post Commission Seperately

                if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                {
                    oGL.Credit = 0;
                    if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                    {
                        oGL.Debit = decFeeCommission;
                    }
                    else
                    {
                        oGL.Debit = oAllot.Commission;
                    }

                    oGL.Debcred = "D";
                    oGL.FeeType = oGL.FeeType + "SCM";
                    oGL.Desciption = "Commission Charge On " + oGL.Desciption;
                    var dbCommandCustomerSeperateCommission = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandCustomerSeperateCommission, transaction);
                }

                #endregion

                oGL.PostToOtherBranch = "N";
                oGL.ClearingDayForTradingTransaction = "N"; // So that others will not inherit this powerful privelege

                #endregion

                #region Second Leg Of Customer To Jobbing For Single Buy/Sell,Ordinary Cross Buy

                decDeductCommJobbing = 0;
                if (strPostCommShared.Trim() == "NO")
                {
                    var oProductAcctAgentJobbing = new ProductAcct();
                    oProductAcctAgentJobbing.ProductCode = strStockProductAccount;
                    oProductAcctAgentJobbing.CustAID = oRowView["CustNo"].ToString().Trim();
                    if (oProductAcctAgentJobbing.GetCustomerAgent())
                    {
                        var oCustomerAgentJobbing = new Customer();
                        oProductAcctAgentJobbing.ProductCode = strAgentProductAccount;
                        oProductAcctAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                        oCustomerAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                        if (oProductAcctAgentJobbing.GetAgentCommission())
                        {
                            if (oCustomerAgentJobbing.GetCustomerName(strAgentProductAccount))
                            {
                                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                {
                                    decDeductCommJobbing = (decFeeCommission * oProductAcctAgentJobbing.AgentComm) / 100;
                                }
                                else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                                {
                                    decDeductCommJobbing = (oAllot.Commission * oProductAcctAgentJobbing.AgentComm) / 100;
                                }

                                decDeductCommJobbing = Math.Round((decimal)decDeductCommJobbing, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                }

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oGL.MasterID = oPGenTable.ShInv;
                oGL.AccountID = "";
                oGL.RecAcct = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                {
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        //oGL.Credit = decFeeTotalAmount - decDeductCommJobbing;
                        oGL.Credit = decFeeTotalAmount;
                    }
                    else
                    {
                        oGL.Credit = decFeeTotalAmount;
                    }

                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                    oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n") + " By " +
                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                     oCustomer.Othername.Trim();
                    oGL.TransType = "STKBBUY";
                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                }
                else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                {
                    oGL.Credit = 0;
                    if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                    {
                        //oGL.Debit = oAllot.TotalAmount + decDeductCommJobbing;
                        oGL.Debit = oAllot.TotalAmount;
                    }
                    else
                    {
                        oGL.Debit = oAllot.TotalAmount;
                    }

                    oGL.Debcred = "D";
                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n") + " By " +
                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                     oCustomer.Othername.Trim();
                    ;
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                }

                oGL.Ref01 = strAllotmentNo;
                oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGL.AcctRef = "";
                oGL.Chqno = "";
                oGL.FeeType = "CTRAD";
                var dbCommandJobbingNormal = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandJobbingNormal, transaction);

                #endregion

                #region Direct Cash Settlement

                oCustomerExtraInformationDirectCash.CustAID = oRowView["CustNo"].ToString().Trim();
                oCustomerExtraInformationDirectCash.WedAnniversaryDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                if (oCustomerExtraInformationDirectCash.DirectCashSettlement &&
                    (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S"))
                {
                    #region Customer Posting For Direct Cash Settlement

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                    oCust.CustAID = oRowView["CustNo"].ToString().Trim();
                    if (oCust.GetBoxLoadStatus())
                    {
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            oProduct.TransNo = strInvestmentProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductAccount;
                        }
                        else
                        {
                            oProduct.TransNo = strInvestmentProductBondAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductBondAccount;
                        }
                    }
                    else
                    {
                        oProduct.TransNo = strStockProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strStockProductAccount;
                    }

                    oGL.Debit = oAllot.TotalAmount;
                    oGL.Credit = 0;
                    oGL.Debcred = "D";
                    oGL.FeeType = "SCUMDCS";
                    oGL.Desciption = "Direct Cash Settlement For Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n");
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                    oGL.Ref01 = strAllotmentNo;
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.Ref02 = "";
                    oGL.Chqno = "";
                    oGL.RecAcctMaster = oPGenTable.DirectCashSettleAcct;
                    oGL.RecAcct = "";
                    oGL.PostToOtherBranch = "Y";
                    oGL.ClearingDayForTradingTransaction = "Y";
                    var dbCommandDirectCashSettlementCustomer = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandDirectCashSettlementCustomer, transaction);

                    oGL.PostToOtherBranch = "N";
                    oGL.ClearingDayForTradingTransaction = "N"; // So that others will not inherit this powerful privelege

                    #endregion

                    #region Second Leg To Direct Cash Settlement Account For Direct Cash Settlement

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.MasterID = oPGenTable.DirectCashSettleAcct;
                    oGL.AccountID = "";
                    if (oCust.GetBoxLoadStatus())
                    {
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            oProduct.TransNo = strInvestmentProductAccount;
                            oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                            oGL.AcctRefSecond = strInvestmentProductAccount;
                        }
                        else
                        {
                            oProduct.TransNo = strInvestmentProductBondAccount;
                            oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                            oGL.AcctRefSecond = strInvestmentProductBondAccount;
                        }
                    }
                    else
                    {
                        oProduct.TransNo = strStockProductAccount;
                        oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                        oGL.AcctRefSecond = strStockProductAccount;
                    }

                    oGL.RecAcct = oRowView["CustNo"].ToString().Trim();
                    oGL.Debit = 0;
                    oGL.Credit = oAllot.TotalAmount;
                    oGL.Debcred = "C";
                    oGL.Desciption = "Direct Cash Settlement For Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n") + " By " +
                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                     oCustomer.Othername.Trim();
                    ;
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                    oGL.Ref01 = strAllotmentNo;
                    oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGL.AcctRef = "";
                    oGL.Chqno = "";
                    oGL.FeeType = "CTRADDCS";
                    var dbCommandJobbingNormalDirectCashSettlement = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandJobbingNormalDirectCashSettlement, transaction);

                    #endregion
                }

                #endregion

                #region Posting Of Box Load Capital Gain Or Loss For Box Load

                if ((oRowView["Buy_Sold_Ind"].ToString().Trim() == "S") && (oCust.GetBoxLoadStatus()) &&
                    (decSellDiffTotal != 0) && (oGLParamBoxSaleProfit.CheckParameter() == "YES"))
                {
                    #region Posting Capital Gain Or Loss To Income Account

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.MasterID = oPGenTable.CapGain;
                    oGL.AccountID = "";
                    if (decSellDiffTotal < 0)
                    {
                        oGL.Credit = 0;
                        oGL.Debit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                        oGL.Debcred = "D";
                        oGL.Desciption = "Cap Loss Appr. from Sale of: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") +
                                         " SP @ " + decActualSellPrice.ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                        oGL.Debit = 0;
                        oGL.Debcred = "C";
                        oGL.Desciption = "Cap Gain Appr. from Sale of: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") +
                                         " SP @ " + decActualSellPrice.ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }

                    oGL.FeeType = "SGIV";
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                    oGL.Ref01 = strAllotmentNo;
                    oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                    oGL.AcctRef = "";
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.Chqno = "";
                    oGL.RecAcctMaster = oPGenTable.ShInv;
                    oGL.RecAcct = "";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    var dbCommandSellProfitOrLoss = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandSellProfitOrLoss, transaction);

                    #endregion

                    #region Posting Of Capital Gain Or Loss To Box Load Account Or P/L Control Account

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    if (strPostInvPLControlAcct.Trim() != "YES")
                    {
                        oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            oProduct.TransNo = strInvestmentProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductAccount;
                        }
                        else
                        {
                            oProduct.TransNo = strInvestmentProductBondAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentProductBondAccount;
                        }
                    }
                    else
                    {
                        oGL.AccountID = "";
                        oGL.MasterID = oPGenTable.CapGainContra;
                        oGL.AcctRef = "";
                    }

                    oGL.FeeType = "SGIB";
                    oGL.TransType = "STKBSALE";
                    oGL.Desciption = "Stock Sale P/L: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                     oCustomer.Othername.Trim();
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                    if (decSellDiffTotal < 0)
                    {
                        oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                        oGL.Debit = 0;
                        oGL.Debcred = "C";
                    }
                    else
                    {
                        oGL.Credit = 0;
                        oGL.Debit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                        oGL.Debcred = "D";
                    }

                    oGL.Ref01 = strAllotmentNo;
                    oGL.Ref02 = "";
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.Chqno = "";
                    oGL.RecAcctMaster = oPGenTable.ShInv;
                    oGL.RecAcct = "";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    var dbCommandBoxLoadProfitLoss = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandBoxLoadProfitLoss, transaction);

                    #endregion
                }

                #endregion
            }
            else if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                #region Customer Posting For Norminal Cross Buy And Posting To Commission Separate

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                oCust.CustAID = oRowView["CustNo"].ToString().Trim();
                oCustomer.CustAID = oRowView["CustNo"].ToString().Trim();
                var oProduct = new Product();
                if (oCust.GetBoxLoadStatus())
                {
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        oProduct.TransNo = strInvestmentProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentProductAccount;
                    }
                    else
                    {
                        oProduct.TransNo = strInvestmentProductBondAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentProductBondAccount;
                    }
                }
                else
                {
                    oProduct.TransNo = strStockProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strStockProductAccount;
                }

                if (!oCustomer.GetCustomerName(strStockProductAccount))
                {
                    throw new Exception("Missing Customer Account");
                }

                oGL.Credit = 0;
                if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                {
                    oGL.Debit = TotalFeeBuyer - decFeeCommission;
                }
                else
                {
                    oGL.Debit = TotalFeeBuyer;
                }

                oGL.Debcred = "D";
                oGL.FeeType = "BCUM";
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim();
                oGL.TransType = "STKBBUY";
                oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                oGL.Ref01 = strAllotmentNo;
                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.Ref02 = "";
                oGL.Chqno = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                oGL.RecAcct = "";
                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGL.PostToOtherBranch = "Y";
                var dbCommandCustomer = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandCustomer, transaction);

                if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                {
                    oGL.Credit = 0;
                    oGL.Debit = decFeeCommission;
                    oGL.Debcred = "D";
                    oGL.FeeType = oGL.FeeType + "SCM";
                    oGL.Desciption = "Commission Charge On " + oGL.Desciption;
                    var dbCommandCustomerSeperateCommission = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandCustomerSeperateCommission, transaction);
                }

                oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege

                #endregion

                #region Second Leg Of Customer To Jobbing For Norminal Cross Buy

                decDeductCommJobbing = 0;
                if (strPostCommShared.Trim() == "NO")
                {
                    var oProductAcctAgentJobbing = new ProductAcct();
                    oProductAcctAgentJobbing.ProductCode = strStockProductAccount;
                    oProductAcctAgentJobbing.CustAID = oRowView["CustNo"].ToString().Trim();
                    if (oProductAcctAgentJobbing.GetCustomerAgent())
                    {
                        var oCustomerAgentJobbing = new Customer();
                        oProductAcctAgentJobbing.ProductCode = strAgentProductAccount;
                        oProductAcctAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                        oCustomerAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                        if (oProductAcctAgentJobbing.GetAgentCommission())
                        {
                            if (oCustomerAgentJobbing.GetCustomerName(strAgentProductAccount))
                            {
                                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                {
                                    decDeductCommJobbing = (decFeeCommission * oProductAcctAgentJobbing.AgentComm) / 100;
                                }
                                else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                                {
                                    decDeductCommJobbing = (oAllot.Commission * oProductAcctAgentJobbing.AgentComm) / 100;
                                }

                                decDeductCommJobbing = Math.Round((decimal)decDeductCommJobbing, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                }

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oGL.MasterID = oPGenTable.ShInv;
                oGL.AccountID = "";
                oGL.RecAcct = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                oGL.Debit = 0;
                if (strCommissionSeperationInCustomerAccount.Trim() == "YES")
                {
                    //oGL.Credit = TotalFeeBuyer - decDeductCommJobbing;
                    oGL.Credit = TotalFeeBuyer;
                }
                else
                {
                    oGL.Credit = TotalFeeBuyer;
                }

                oGL.Debcred = "C";
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                oGL.TransType = "STKBBUY";
                oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                oGL.Ref01 = strAllotmentNo;
                oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGL.AcctRef = "";
                oGL.AcctRefSecond = "";
                oGL.Chqno = "";
                oGL.FeeType = "CTRAD";
                var dbCommandJobbingCrossOrdinary = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandJobbingCrossOrdinary, transaction);

                #endregion
            }

            #region Agent Posting Buyer

            var oProductAcctAgent = new ProductAcct();
            oProductAcctAgent.ProductCode = strStockProductAccount;
            oProductAcctAgent.CustAID = oRowView["CustNo"].ToString().Trim();
            if (oProductAcctAgent.GetCustomerAgent())
            {
                decimal decDeductComm = 0;
                var oCustomerAgent = new Customer();

                oProductAcctAgent.ProductCode = strAgentProductAccount;
                oProductAcctAgent.CustAID = oProductAcctAgent.Agent.Trim();
                oCustomerAgent.CustAID = oProductAcctAgent.Agent.Trim();
                if (oProductAcctAgent.GetAgentCommission())
                {
                    if (oCustomerAgent.GetCustomerName(strAgentProductAccount))
                    {
                        if (strPostCommShared.Trim() == "YES")
                        {
                            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                            {
                                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                {
                                    decDeductComm = (decFeeCommission * oProductAcctAgent.AgentComm) / 100;
                                    decTotalDeductCommissionBuy = decTotalDeductCommissionBuy + decDeductComm;
                                    decTotalDeductCommissionBuy = Math.Round(decTotalDeductCommissionBuy, 2,
                                        MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    decDeductComm = (oAllot.Commission * oProductAcctAgent.AgentComm) / 100;
                                    decTotalDeductCommissionSell = decTotalDeductCommissionSell + decDeductComm;
                                    decTotalDeductCommissionSell = Math.Round(decTotalDeductCommissionSell, 2,
                                        MidpointRounding.AwayFromZero);
                                }

                                decDeductComm = Math.Round((decimal)decDeductComm, 2, MidpointRounding.AwayFromZero);
                                decTotalDeductCommission = decTotalDeductCommission + decDeductComm;
                                decTotalDeductCommission =
                                    Math.Round(decTotalDeductCommission, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                {
                                    decDeductComm = (decFeeCommission * oProductAcctAgent.AgentComm) / 100;
                                    decTotalDeductCommissionBuyBond = decTotalDeductCommissionBuyBond + decDeductComm;
                                    decTotalDeductCommissionBuyBond = Math.Round(decTotalDeductCommissionBuyBond, 2,
                                        MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    decDeductComm = (oAllot.Commission * oProductAcctAgent.AgentComm) / 100;
                                    decTotalDeductCommissionSellBond = decTotalDeductCommissionSellBond + decDeductComm;
                                    decTotalDeductCommissionSellBond = Math.Round(decTotalDeductCommissionSellBond, 2,
                                        MidpointRounding.AwayFromZero);
                                }

                                decDeductComm = Math.Round((decimal)decDeductComm, 2, MidpointRounding.AwayFromZero);
                                decTotalDeductCommissionBond = decTotalDeductCommissionBond + decDeductComm;
                                decTotalDeductCommissionBond =
                                    Math.Round(decTotalDeductCommissionBond, 2, MidpointRounding.AwayFromZero);
                            }
                        }

                        oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                        oGL.MasterID = strPostCommShared.Trim() == "YES" ? oPGenTable.ShInv : oPGenTable.AgComm;

                        oGL.AccountID = "";
                        oGL.Credit = 0;
                        if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                        {
                            oGL.Debit = (decFeeCommission * oProductAcctAgent.AgentComm) / 100;
                        }
                        else
                        {
                            oGL.Debit = (oAllot.Commission * oProductAcctAgent.AgentComm) / 100;
                        }

                        oGL.Debit = Math.Round(oGL.Debit, 2, MidpointRounding.AwayFromZero);
                        oGL.Debcred = "D";
                        switch (oRowView["Buy_Sold_Ind"].ToString().Trim())
                        {
                            case "B":
                                {
                                    oGL.FeeType = "BECM";
                                    if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
                                    {
                                        oGL.Desciption = "Stk Purc.Comm For: " + oRowView["Units"].ToString().Trim() + " " +
                                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                                         decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                         oCustomer.Othername.Trim();
                                    }
                                    else
                                    {
                                        oGL.Desciption = "Cross Deal Purc.Comm.: " + oRowView["Units"].ToString().Trim() + " " +
                                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                                         decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                         oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                                         oCustomerCross.Othername.Trim();
                                    }

                                    oGL.TransType = "STKBBUY";
                                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                                    break;
                                }
                            case "S":
                                oGL.FeeType = "SECM";
                                oGL.TransType = "STKBSALE";
                                oGL.Desciption = "Stk Sale Comm: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                                oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                                break;
                        }

                        oGL.Ref01 = strAllotmentNo;
                        oGL.AcctRef = "";
                        oGL.Reverse = "N";
                        oGL.Jnumber = strJnumberNext;
                        oGL.Branch = strDefaultBranchCode;
                        oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                        oGL.Chqno = "";
                        oGL.RecAcctMaster = oPGenTable.ShInv;
                        oGL.RecAcct = "";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        var dbCommandAgentExpense = oGL.AddCommand();
                        db.ExecuteNonQuery(dbCommandAgentExpense, transaction);


                        oGL.EffectiveDate = oRowView["Date"].ToString().Trim().ToDate();
                        oGL.AccountID = oProductAcctAgent.CustAID;
                        var oProduct = new Product
                        {
                            TransNo = strAgentProductAccount
                        };
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strAgentProductAccount;
                        if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                        {
                            oGL.Credit = (decFeeCommission * oProductAcctAgent.AgentComm) / 100;
                        }
                        else
                        {
                            oGL.Credit = (oAllot.Commission * oProductAcctAgent.AgentComm) / 100;
                        }

                        oGL.Credit = Math.Round(oGL.Credit, 2, MidpointRounding.AwayFromZero);
                        oGL.Debit = 0;
                        oGL.Debcred = "C";
                        switch (oRowView["Buy_Sold_Ind"].ToString().Trim())
                        {
                            case "B":
                                {
                                    oGL.FeeType = "BBPY";
                                    if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
                                    {
                                        oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                                         decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                         oCustomer.Othername.Trim();
                                    }
                                    else
                                    {
                                        oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                                         decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                         oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                                         oCustomerCross.Othername.Trim();
                                    }

                                    oGL.TransType = "STKBBUY";
                                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                                    break;
                                }
                            case "S":
                                oGL.FeeType = "SBPY";
                                oGL.TransType = "STKBSALE";
                                oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                                oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                                break;
                        }

                        oGL.Ref01 = strAllotmentNo;
                        oGL.Reverse = "N";
                        oGL.Jnumber = strJnumberNext;
                        oGL.Branch = strDefaultBranchCode;
                        oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                        oGL.Chqno = "";
                        oGL.RecAcctMaster = oPGenTable.ShInv;
                        oGL.RecAcct = "";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        var dbCommandBrokPayable = oGL.AddCommand();
                        db.ExecuteNonQuery(dbCommandBrokPayable, transaction);
                    }
                }
            }

            #endregion

            #region Posting Buy Different Charges For City Code

            if (strBranchBuySellCharge.Trim() != "YES" || oRowView["Buy_Sold_Ind"].ToString().Trim() != "B")
                return decTotalDeductCommissionBuy;
            var oBranchBuySellCharge = new BranchBuySellCharge();
            var oCustomerBuySellCharge = new Customer();
            var oBranchBuySellChargeCustomer = new Branch();
            oCustomerBuySellCharge.CustAID = oRowView["CustNo"].ToString().Trim();
            oBranchBuySellChargeCustomer.TransNo = oCustomerBuySellCharge.GetBranchId();
            oBranchBuySellChargeCustomer.GetBranch();
            oBranchBuySellCharge.GetBranchBuySellCharges();
            if (oBranchBuySellChargeCustomer.TransNo.Trim() == "" ||
                oBranchBuySellChargeCustomer.TransNo.Trim() == oBranchBuySellChargeCustomer.DefaultBranch.Trim() ||
                oBranchBuySellChargeCustomer.JointHeadOffice != false) return decTotalDeductCommissionBuy;
            oGL.EffectiveDate =
                oRowView["Date"].ToString().Trim().ToDate();
            oGL.Credit = 0;
            oGL.Debcred = "D";
            if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                oGL.Debit = TotalFeeBuyer * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }
            else
            {
                oGL.Debit = decFeeTotalAmount * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }

            oGL.MasterID = oPGenTable.Bcncomm;
            oGL.AccountID = "";
            oGL.FeeType = "BCOMBRCHGD";
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
            {
                oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                 oCustomer.Othername.Trim();
            }
            else
            {
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                 oCustomerCross.Othername.Trim();
            }

            oGL.TransType = "STKBBUY";
            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
            oGL.Ref01 = strAllotmentNo;
            oGL.AcctRef = "";
            oGL.Reverse = "N";
            oGL.Jnumber = strJnumberNext;
            oGL.Branch = strDefaultBranchCode;
            oGL.Ref02 = "";
            oGL.Chqno = "";
            oGL.RecAcctMaster = oPGenTable.ShInv;
            oGL.RecAcct = "";
            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
            var dbCommandCommBranchBuyChargeDefault = oGL.AddCommand();
            db.ExecuteNonQuery(dbCommandCommBranchBuyChargeDefault, transaction);

            oGL.EffectiveDate =
                oRowView["Date"].ToString().Trim().ToDate();
            oGL.Debit = 0;
            oGL.Debcred = "C";
            if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                oGL.Credit = TotalFeeBuyer * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }
            else
            {
                oGL.Credit = decFeeTotalAmount * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }

            oGL.MasterID = oBranchBuySellChargeCustomer.Commission;
            oGL.AccountID = "";
            oGL.FeeType = "BCOMBRCHGB";
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
            {
                oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                 oCustomer.Othername.Trim();
            }
            else
            {
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                 oCustomerCross.Othername.Trim();
            }

            oGL.TransType = "STKBBUY";
            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
            oGL.Ref01 = strAllotmentNo;
            oGL.AcctRef = "";
            oGL.Reverse = "N";
            oGL.Jnumber = strJnumberNext;
            oGL.Branch = strDefaultBranchCode;
            oGL.Ref02 = "";
            oGL.Chqno = "";
            oGL.RecAcctMaster = oPGenTable.ShInv;
            oGL.RecAcct = "";
            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
            var dbCommandCommBranchBuyChargeBranch = oGL.AddCommand();
            db.ExecuteNonQuery(dbCommandCommBranchBuyChargeBranch, transaction);

            oGL.EffectiveDate =
                oRowView["Date"].ToString().Trim().ToDate();
            oGL.Credit = 0;
            oGL.Debcred = "D";
            if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                oGL.Debit = TotalFeeBuyer * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }
            else
            {
                oGL.Debit = decFeeTotalAmount * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }

            oGL.MasterID = oBranchBuySellChargeCustomer.Commission;
            oGL.AccountID = "";
            oGL.FeeType = "BCOMBRCHGC";
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
            {
                oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                 oCustomer.Othername.Trim();
            }
            else
            {
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                 oCustomerCross.Othername.Trim();
            }

            oGL.TransType = "STKBBUY";
            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
            oGL.Ref01 = strAllotmentNo;
            oGL.AcctRef = "";
            oGL.Reverse = "N";
            oGL.Jnumber = strJnumberNext;
            oGL.Branch = strDefaultBranchCode;
            oGL.Ref02 = "";
            oGL.Chqno = "";
            oGL.RecAcctMaster = oBranchBuySellChargeCustomer.Commission;
            oGL.RecAcct = "";
            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
            var dbCommandCommBranchBuyChargeCommission = oGL.AddCommand();
            db.ExecuteNonQuery(dbCommandCommBranchBuyChargeCommission, transaction);

            oGL.EffectiveDate =
                oRowView["Date"].ToString().Trim().ToDate();
            oGL.Debit = 0;
            oGL.Debcred = "C";
            if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                oGL.Credit = TotalFeeBuyer * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }
            else
            {
                oGL.Credit = decFeeTotalAmount * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }

            oGL.MasterID = oBranchBuySellChargeCustomer.Commission;
            oGL.AccountID = "";
            oGL.FeeType = "BCOMBRCHGCBC";
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
            {
                oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                 oCustomer.Othername.Trim();
            }
            else
            {
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                 oCustomerCross.Othername.Trim();
            }

            oGL.TransType = "TRADBANK";
            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
            oGL.Ref01 = strAllotmentNo;
            oGL.AcctRef = "";
            oGL.Reverse = "N";
            oGL.Jnumber = strJnumberNext;
            oGL.Branch = strDefaultBranchCode;
            oGL.Ref02 = "";
            oGL.Chqno = "";
            oGL.RecAcctMaster = oBranchBuySellChargeCustomer.Commission;
            oGL.RecAcct = "";
            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
            var dbCommandCommBranchBuyChargeCommissionBranchControl = oGL.AddCommand();
            db.ExecuteNonQuery(dbCommandCommBranchBuyChargeCommissionBranchControl, transaction);


            oGL.EffectiveDate =
                oRowView["Date"].ToString().Trim().ToDate();
            oGL.Debit = 0;
            oGL.Debcred = "C";
            if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                oGL.Credit = TotalFeeBuyer * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }
            else
            {
                oGL.Credit = decFeeTotalAmount * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }

            oGL.MasterID = oBranchBuySellChargeCustomer.CommissionIncome;
            oGL.AccountID = "";
            oGL.FeeType = "BCOMBRCHGI";
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
            {
                oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                 oCustomer.Othername.Trim();
            }
            else
            {
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                 oCustomerCross.Othername.Trim();
            }

            oGL.TransType = "TRADBANK";
            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
            oGL.Ref01 = strAllotmentNo;
            oGL.AcctRef = "";
            oGL.Reverse = "N";
            oGL.Jnumber = strJnumberNext;
            oGL.Branch = strDefaultBranchCode;
            oGL.Ref02 = "";
            oGL.Chqno = "";
            oGL.RecAcctMaster = oBranchBuySellChargeCustomer.Trading;
            oGL.RecAcct = "";
            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGL.PostToOtherBranch = "Y";
            var dbCommandCommBranchBuyChargeCommissionIncome = oGL.AddCommand();
            db.ExecuteNonQuery(dbCommandCommBranchBuyChargeCommissionIncome, transaction);
            oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege

            oGL.EffectiveDate =
                oRowView["Date"].ToString().Trim().ToDate();
            oGL.Credit = 0;
            oGL.Debcred = "D";
            if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                oGL.Debit = TotalFeeBuyer * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }
            else
            {
                oGL.Debit = decFeeTotalAmount * (oBranchBuySellCharge.TotalFeeBranchBuy / 100);
            }

            oGL.MasterID = oBranchBuySellChargeCustomer.Trading;
            oGL.AccountID = "";
            oGL.FeeType = "BCOMBRCHGIBC";
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
            {
                oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                 oCustomer.Othername.Trim();
            }
            else
            {
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oAllot.UnitPrice.ToString()).ToString("n").Trim() + " By " +
                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                 oCustomerCross.Othername.Trim();
            }

            oGL.TransType = "TRADBANK";
            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
            oGL.Ref01 = strAllotmentNo;
            oGL.AcctRef = "";
            oGL.Reverse = "N";
            oGL.Jnumber = strJnumberNext;
            oGL.Branch = strDefaultBranchCode;
            oGL.Ref02 = "";
            oGL.Chqno = "";
            oGL.RecAcctMaster = oBranchBuySellChargeCustomer.Commission;
            oGL.RecAcct = "";
            oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
            oGL.PostToOtherBranch = "Y";
            var dbCommandCommBranchBuyChargeCommissionIncomeBranchControl = oGL.AddCommand();
            db.ExecuteNonQuery(dbCommandCommBranchBuyChargeCommissionIncomeBranchControl, transaction);
            oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege

            #endregion

            return decTotalDeductCommissionBuy;
        }

        private static void GetNextJNumber(SqlDatabase db, SqlTransaction transaction, DataRow oRowView, string strBuyerTrueId,
            ProductAcct oCust, string strStockProductAccount, Customer oCustomer, ProductAcct oCustCross, Allotment oAllot,
            Customer oCustomerCross)
        {
            string strJnumberNext;
            SqlCommand oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
            db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(oCommandJnumber, transaction);
            strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

            if ((oRowView["CrossD"].ToString().Trim() == "Y") && (strBuyerTrueId.Trim() != "")
                                                              && ((oRowView["CrossType"].ToString().Trim() == "CD") ||
                                                                  (oRowView["CrossType"].ToString().Trim() == "NO")
                                                                  || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                                                                  (oRowView["CrossType"].ToString().Trim() == "NT")))
            {
                oCust.ProductCode = strStockProductAccount;
                oCust.CustAID = strBuyerTrueId;
                oCustomer.CustAID = strBuyerTrueId;
                oCustCross.ProductCode = strStockProductAccount;
                oCustCross.CustAID = oAllot.CustAID;
                oCustomerCross.CustAID = oAllot.CustAID;
            }
            else
            {
                oCust.ProductCode = strStockProductAccount;
                oCust.CustAID = oAllot.CustAID;
                oCustomer.CustAID = oAllot.CustAID;
                oCustCross.ProductCode = strStockProductAccount;
                oCustCross.CustAID = oAllot.OtherCust;
                oCustomerCross.CustAID = oAllot.OtherCust;
            }

            if (!oCust.GetCustomerByCustId())
            {
                throw new Exception("Missing Customer Stock Account");
            }

            if (!oCustomer.GetCustomerName(strStockProductAccount))
            {
                throw new Exception("Missing Customer Account");
            }

            if (oAllot.Cdeal == 'Y')
            {
                if (!oCustCross.GetCustomerByCustId())
                {
                    throw new Exception("Missing Customer Stock Account");
                }

                if (!oCustomerCross.GetCustomerName(strStockProductAccount))
                {
                    throw new Exception("Missing Customer Account");
                }
            }

            decimal decGetUnitCost = 0;
            decimal decActualSellPrice = 0;
            decimal decSellDiff = 0;
            decimal decSellDiffTotal = 0;
        }

        private static Allotment ProcessAllotment(DataRow oRowView, StkParam oStkbPGenTable, bool blnChkPropAccount,
            CustomerExtraInformation oCustomerExtraInformationDoNotChargeStampDuty, string strUserName, SqlDatabase db,
            SqlTransaction transaction, ProductAcct oCustChkPropAccount, string strStockProductAccount,
            CustomerExtraInformation oCustomerExtraInformationDirectCash, ref decimal decTotalBankProp,
            ref decimal decTotalBankAuditProp, ref decimal decTotalBank, ref decimal decTotalBankAudit,
            ref decimal decTotalBankBond, ref decimal decTotalBankAuditBond, ref decimal decTotalSecProp,
            ref decimal decTotalSec, ref decimal decTotalSecBond, ref decimal decTotalStampDutyProp,
            ref decimal decTotalStampDuty, ref decimal decTotalStampDutyBond, ref decimal decTotalCommissionProp,
            ref decimal decTotalCommissionBuyProp, ref decimal decTotalCommission, ref decimal decTotalCommissionBuy,
            ref decimal decTotalCommissionBond, ref decimal decTotalCommissionBuyBond, ref decimal decTotalVatProp,
            ref decimal decTotalVat, ref decimal decTotalVatBond, ref decimal decTotalNseProp, ref decimal decTotalNse,
            ref decimal decTotalNseBond, ref decimal decTotalCscsProp, ref decimal decTotalCscs, ref decimal decTotalCscsBond,
            ref string strAllotmentNo, ref decimal decTotalCommissionSellProp, ref decimal decTotalCommissionSell,
            ref decimal decTotalCommissionSellBond, ref decimal decTotalDirectCashSettlementProp,
            ref decimal decTotalDirectCashSettlement, ref decimal decTotalDirectCashSettlementBond,
            ref decimal decTotalDirectCashSettlementDifferenceProp, ref decimal decTotalDirectCashSettlementDifference,
            ref decimal decTotalDirectCashSettlementDifferenceBond, ref decimal decFeeCscsVatSeller)
        {
            decimal decFeeCscsSeller = 0;
            decimal decAllotConsiderationBuy = 0;
            decimal decReturnAmount = 0;
            var oAllot = new Allotment();
            switch (oRowView["Buy_Sold_Ind"].ToString().Trim())
            {
                case "B":
                    {
                        #region Save Buyer For 1.Single 2.Ordinary Cross 3.Norminal Cross 4. NA Buy Only With Consideration 5. NT Seller Only With Consideration

                        var decAllotConsideration = SaveBuyer(oRowView, oStkbPGenTable, blnChkPropAccount,
                            oCustomerExtraInformationDoNotChargeStampDuty, strUserName, db, transaction, ref decTotalBankProp,
                            ref decTotalBankAuditProp, ref decTotalBank, ref decTotalBankAudit, ref decTotalBankBond, ref decTotalBankAuditBond,
                            ref decTotalSecProp, ref decTotalSec, ref decTotalSecBond, ref decTotalStampDutyProp, oAllot, ref decTotalStampDuty,
                            ref decTotalStampDutyBond, ref decTotalCommissionProp, ref decTotalCommissionBuyProp, ref decTotalCommission,
                            ref decTotalCommissionBuy, ref decTotalCommissionBond, ref decTotalCommissionBuyBond, ref decTotalVatProp,
                            ref decTotalVat, ref decTotalVatBond, ref decTotalNseProp, ref decTotalNse, ref decTotalNseBond,
                            ref decTotalCscsProp, ref decTotalCscs, ref decTotalCscsBond, ref strAllotmentNo, out var totalFeeBuyer);

                        #endregion

                        #region Save Seller For 1.Ordinary Cross 2.Norminal Cross 3.Seller NS Cross Deal 4.Buyer NB Cross Deal 5.Seller NT Cross Deal With Consideration 5.Buyer NA Cross Deal With Consideration

                        blnChkPropAccount = SaveSeller(oRowView, oStkbPGenTable, blnChkPropAccount, oCustomerExtraInformationDoNotChargeStampDuty,
                            strUserName, db, transaction, oCustChkPropAccount, strStockProductAccount, oCustomerExtraInformationDirectCash,
                            ref decTotalBankProp, ref decTotalBankAuditProp, ref decTotalBank, ref decTotalBankAudit, ref decTotalBankBond,
                            ref decTotalBankAuditBond, ref decTotalSecProp, ref decTotalSec, ref decTotalSecBond, ref decTotalStampDutyProp,
                            oAllot, decAllotConsideration, ref decTotalStampDuty, totalFeeBuyer, ref decTotalStampDutyBond, ref decTotalCommissionProp,
                            ref decTotalCommission, ref decTotalCommissionBond, ref decTotalVatProp, ref decTotalVat, ref decTotalVatBond,
                            ref decTotalNseProp, ref decTotalNse, ref decTotalNseBond, ref decTotalCscsProp, ref decTotalCscs, ref decTotalCscsBond,
                            ref strAllotmentNo, ref decTotalCommissionSellProp, ref decTotalCommissionSell, ref decTotalCommissionSellBond,
                            ref decTotalDirectCashSettlementProp, ref decTotalDirectCashSettlement, ref decTotalDirectCashSettlementBond,
                            ref decTotalDirectCashSettlementDifferenceProp, ref decTotalDirectCashSettlementDifference,
                            ref decTotalDirectCashSettlementDifferenceBond, ref decFeeCscsVatSeller,
                            ref decAllotConsiderationBuy, ref decReturnAmount,
                            ref decFeeCscsSeller);

                        #endregion

                        break;
                    }
                case "S" when (oRowView["CrossD"].ToString().Trim() == "N"):
                    {
                        SaveSellForSingleOnly(oRowView, oStkbPGenTable, blnChkPropAccount, oCustomerExtraInformationDoNotChargeStampDuty,
                            strUserName, db, transaction, oCustomerExtraInformationDirectCash, ref decTotalBankProp, ref decTotalBankAuditProp,
                            ref decTotalBank, ref decTotalBankAudit, ref decTotalBankBond, ref decTotalBankAuditBond, ref decTotalSecProp,
                            ref decTotalSec, ref decTotalSecBond, ref decTotalStampDutyProp, oAllot, ref decTotalStampDuty,
                            ref decTotalStampDutyBond, ref decTotalCommissionProp, ref decTotalCommission, ref decTotalCommissionBond,
                            ref decTotalVatProp, ref decTotalVat, ref decTotalVatBond, ref decTotalNseProp, ref decTotalNse,
                            ref decTotalNseBond, ref decTotalCscsProp, ref decTotalCscs, ref decTotalCscsBond, out strAllotmentNo,
                            ref decTotalCommissionSellProp, ref decTotalCommissionSell, ref decTotalCommissionSellBond,
                            ref decTotalDirectCashSettlementProp, ref decTotalDirectCashSettlement, ref decTotalDirectCashSettlementBond,
                            ref decTotalDirectCashSettlementDifferenceProp, ref decTotalDirectCashSettlementDifference,
                            ref decTotalDirectCashSettlementDifferenceBond);
                        break;
                    }
            }

            return oAllot;
        }

        private static void SaveSellForSingleOnly(DataRow oRowView, StkParam oStkbPGenTable, bool blnChkPropAccount,
            CustomerExtraInformation oCustomerExtraInformationDoNotChargeStampDuty, string strUserName, SqlDatabase db,
            SqlTransaction transaction, CustomerExtraInformation oCustomerExtraInformationDirectCash,
            ref decimal decTotalBankProp, ref decimal decTotalBankAuditProp, ref decimal decTotalBank,
            ref decimal decTotalBankAudit, ref decimal decTotalBankBond, ref decimal decTotalBankAuditBond,
            ref decimal decTotalSecProp, ref decimal decTotalSec, ref decimal decTotalSecBond,
            ref decimal decTotalStampDutyProp, Allotment oAllot, ref decimal decTotalStampDuty,
            ref decimal decTotalStampDutyBond, ref decimal decTotalCommissionProp, ref decimal decTotalCommission,
            ref decimal decTotalCommissionBond, ref decimal decTotalVatProp, ref decimal decTotalVat,
            ref decimal decTotalVatBond, ref decimal decTotalNseProp, ref decimal decTotalNse, ref decimal decTotalNseBond,
            ref decimal decTotalCscsProp, ref decimal decTotalCscs, ref decimal decTotalCscsBond, out string strAllotmentNo,
            ref decimal decTotalCommissionSellProp, ref decimal decTotalCommissionSell, ref decimal decTotalCommissionSellBond,
            ref decimal decTotalDirectCashSettlementProp, ref decimal decTotalDirectCashSettlement,
            ref decimal decTotalDirectCashSettlementBond, ref decimal decTotalDirectCashSettlementDifferenceProp,
            ref decimal decTotalDirectCashSettlementDifference, ref decimal decTotalDirectCashSettlementDifferenceBond)
        {
            decimal decAllotConsideration;

            #region Save Sell For Single Only

            oAllot.CustAID = oRowView["CustNo"].ToString().Trim();
            oAllot.StockCode = oRowView["Stockcode"].ToString().Trim();
            oAllot.DateAlloted = oRowView["Date"].ToString().Trim().ToDate();
            oAllot.Qty = long.Parse(oRowView["Units"].ToString().Trim());
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                oAllot.UnitPrice = decimal.Parse(oRowView["UnitPrice"].ToString().Trim());
                decAllotConsideration =
                    Math.Round(
                        decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                        long.Parse(oRowView["Units"].ToString().Trim()), 2);
            }
            else
            {
                oAllot.UnitPrice = decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) * 10;
                decAllotConsideration =
                    Math.Round(
                        (decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) * 10) *
                        long.Parse(oRowView["Units"].ToString().Trim()), 2);
            }

            oAllot.CommissionType = "FIXED";
            decAllotConsideration = Math.Round(decAllotConsideration, 2, MidpointRounding.AwayFromZero);

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalBankProp += decAllotConsideration;
                    decTotalBankAuditProp += decAllotConsideration;
                    decTotalBankProp = Math.Round(decTotalBankProp, 2, MidpointRounding.AwayFromZero);
                    decTotalBankAuditProp = Math.Round(decTotalBankAuditProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalBank += decAllotConsideration;
                    decTotalBankAudit += decAllotConsideration;
                    decTotalBank = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero);
                    decTotalBankAudit = Math.Round(decTotalBankAudit, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalBankBond += decAllotConsideration;
                decTotalBankAuditBond += decAllotConsideration;
                decTotalBankBond = Math.Round(decTotalBankBond, 2, MidpointRounding.AwayFromZero);
                decTotalBankAuditBond = Math.Round(decTotalBankAuditBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.Consideration = decAllotConsideration;
            if (decAllotConsideration * (oStkbPGenTable.Ssec / 100) > oStkbPGenTable.MinSceS)
            {
                oAllot.SecFee = decAllotConsideration * (oStkbPGenTable.Ssec / 100);
            }
            else
            {
                oAllot.SecFee = oStkbPGenTable.MinSceS;
            }

            oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
            }

            oCustomerExtraInformationDoNotChargeStampDuty.CustAID = oRowView["CustNo"].ToString().Trim();
            if (!oCustomerExtraInformationDoNotChargeStampDuty.DoNotChargeStampDuty)
            {
                if (decAllotConsideration * (oStkbPGenTable.Sstamp / 100) > oStkbPGenTable.MinStampS)
                {
                    oAllot.StampDuty = decAllotConsideration * (oStkbPGenTable.Sstamp / 100);
                }
                else
                {
                    oAllot.StampDuty = oStkbPGenTable.MinStampS;
                }

                oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                oAllot.StampDuty = 0;
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero) +
                                            oAllot.StampDuty;
                    decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + oAllot.StampDuty;
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalStampDutyBond =
                    Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + oAllot.StampDuty;
                decTotalStampDutyBond = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.Commission =
                oAllot.ComputeComm(decAllotConsideration, "S", oStkbPGenTable.Scncomm, oStkbPGenTable.MinCommS);
            oAllot.Commission = Math.Round(oAllot.Commission, 2, MidpointRounding.AwayFromZero);

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero) +
                                             oAllot.Commission;
                    decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero);

                    decTotalCommissionSellProp = Math.Round(decTotalCommissionSellProp, 2, MidpointRounding.AwayFromZero) +
                                                 oAllot.Commission;
                    decTotalCommissionSellProp = Math.Round(decTotalCommissionSellProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCommission =
                        Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                    decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                    decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero) +
                                             oAllot.Commission;
                    decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCommissionBond =
                    Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                decTotalCommissionBond = Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero);

                decTotalCommissionSellBond = Math.Round(decTotalCommissionSellBond, 2, MidpointRounding.AwayFromZero) +
                                             oAllot.Commission;
                decTotalCommissionSellBond = Math.Round(decTotalCommissionSellBond, 2, MidpointRounding.AwayFromZero);
            }


            oAllot.VAT = oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "S", oStkbPGenTable.Svat,
                oStkbPGenTable.MinVatS, oStkbPGenTable.Scncomm, oRowView["Date"].ToString().Trim().ToDate());
            oAllot.VAT = Math.Round(oAllot.VAT, 2, MidpointRounding.AwayFromZero);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                    decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                    decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero);
            }

            if (decAllotConsideration * (oStkbPGenTable.Snse / 100) > oStkbPGenTable.MinSceS)
            {
                oAllot.NSEFee = decAllotConsideration * (oStkbPGenTable.Snse / 100);
            }
            else
            {
                oAllot.NSEFee = oStkbPGenTable.MinSceS;
            }

            oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2, MidpointRounding.AwayFromZero);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero);
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (decAllotConsideration * (oStkbPGenTable.Scscs / 100) > oStkbPGenTable.MinCscsS)
                {
                    oAllot.CSCSFee = decAllotConsideration * (oStkbPGenTable.Scscs / 100);
                }
                else
                {
                    oAllot.CSCSFee = oStkbPGenTable.MinCscsS;
                }

                oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                oAllot.CSCSFee = decAllotConsideration * (oStkbPGenTable.Scscs / 100);
                oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 0, MidpointRounding.AwayFromZero);
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.SMSAlertCSCS = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) * oStkbPGenTable.SMSAlertCSCSS;
            oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());

            if (oAllot.SecFee * (oStkbPGenTable.SsecVat / 100) > oStkbPGenTable.MinSecVatS)
            {
                oAllot.SecVat = oAllot.SecFee * (oStkbPGenTable.SsecVat / 100);
            }
            else
            {
                oAllot.SecVat = oStkbPGenTable.MinSecVatS;
            }

            oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
            }

            if (oAllot.NSEFee * (oStkbPGenTable.SnseVat / 100) > oStkbPGenTable.MinNseVatS)
            {
                oAllot.NseVat = oAllot.NSEFee * (oStkbPGenTable.SnseVat / 100);
            }
            else
            {
                oAllot.NseVat = oStkbPGenTable.MinNseVatS;
            }

            oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero);
            }

            if (oAllot.CSCSFee * (oStkbPGenTable.ScscsVat / 100) > oStkbPGenTable.MinCscsVatS)
            {
                oAllot.CscsVat = oAllot.CSCSFee * (oStkbPGenTable.ScscsVat / 100);
            }
            else
            {
                oAllot.CscsVat = oStkbPGenTable.MinCscsVatS;
            }

            oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.SMSAlertCSCSVAT = oAllot.SMSAlertCSCS * (oStkbPGenTable.SMSAlertCSCSVATS / 100);
            oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                       oAllot.SMSAlertCSCSVAT;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCSVAT;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCSVAT;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }


            oAllot.TotalAmount = decAllotConsideration - (oAllot.SecFee + oAllot.StampDuty
                                                                        + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee +
                                                                        oAllot.SMSAlertCSCS + oAllot.NSEFee
                                                                        + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat +
                                                                        oAllot.SMSAlertCSCSVAT);
            oAllot.TotalAmount = Math.Round(oAllot.TotalAmount, 2, MidpointRounding.AwayFromZero);

            oCustomerExtraInformationDirectCash.CustAID = oRowView["CustNo"].ToString().Trim();
            oCustomerExtraInformationDirectCash.WedAnniversaryDate = oRowView["Date"].ToString().Trim().ToDate();
            if (oCustomerExtraInformationDirectCash.DirectCashSettlement)
            {
                decimal decRealCommission = 0;
                if (decAllotConsideration * (oStkbPGenTable.Scncomm / 100) > oStkbPGenTable.MinCommS)
                {
                    decRealCommission = decAllotConsideration * (oStkbPGenTable.Scncomm / 100);
                }
                else
                {
                    decRealCommission = oStkbPGenTable.MinCommS;
                }

                decRealCommission = Math.Round(decRealCommission, 2, MidpointRounding.AwayFromZero);

                if (oAllot.Commission == decRealCommission)
                {
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalDirectCashSettlementProp += oAllot.TotalAmount;
                            decTotalDirectCashSettlementProp = Math.Round(decTotalDirectCashSettlementProp, 2,
                                MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalDirectCashSettlement += oAllot.TotalAmount;
                            decTotalDirectCashSettlement =
                                Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalDirectCashSettlementBond += oAllot.TotalAmount;
                        decTotalDirectCashSettlementBond = Math.Round(decTotalDirectCashSettlementBond, 2,
                            MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            if ((decRealCommission - oAllot.Commission) > 0)
                            {
                                decTotalDirectCashSettlementProp = decTotalDirectCashSettlementProp + oAllot.TotalAmount +
                                                                   (decRealCommission - oAllot.Commission);
                                decTotalDirectCashSettlementDifferenceProp =
                                    Math.Round(decTotalDirectCashSettlementDifferenceProp, 2,
                                        MidpointRounding.AwayFromZero) + (decRealCommission - oAllot.Commission);
                            }
                            else
                            {
                                decTotalDirectCashSettlementProp = decTotalDirectCashSettlementProp + oAllot.TotalAmount -
                                                                   (oAllot.Commission - decRealCommission);
                                decTotalDirectCashSettlementDifferenceProp =
                                    Math.Round(decTotalDirectCashSettlementDifferenceProp, 2,
                                        MidpointRounding.AwayFromZero) - (oAllot.Commission - decRealCommission);
                            }

                            decTotalDirectCashSettlementProp = Math.Round(decTotalDirectCashSettlementProp, 2,
                                MidpointRounding.AwayFromZero);
                            decTotalDirectCashSettlementDifferenceProp =
                                Math.Round(decTotalDirectCashSettlementDifferenceProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            if ((decRealCommission - oAllot.Commission) > 0)
                            {
                                decTotalDirectCashSettlement = decTotalDirectCashSettlement + oAllot.TotalAmount +
                                                               (decRealCommission - oAllot.Commission);
                                decTotalDirectCashSettlementDifference =
                                    Math.Round(decTotalDirectCashSettlementDifference, 2, MidpointRounding.AwayFromZero) +
                                    (decRealCommission - oAllot.Commission);
                            }
                            else
                            {
                                decTotalDirectCashSettlement = decTotalDirectCashSettlement + oAllot.TotalAmount -
                                                               (oAllot.Commission - decRealCommission);
                                decTotalDirectCashSettlementDifference =
                                    Math.Round(decTotalDirectCashSettlementDifference, 2, MidpointRounding.AwayFromZero) -
                                    (oAllot.Commission - decRealCommission);
                            }

                            decTotalDirectCashSettlement =
                                Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                            decTotalDirectCashSettlementDifference = Math.Round(decTotalDirectCashSettlementDifference, 2,
                                MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        if ((decRealCommission - oAllot.Commission) > 0)
                        {
                            decTotalDirectCashSettlementBond = decTotalDirectCashSettlementBond + oAllot.TotalAmount +
                                                               (decRealCommission - oAllot.Commission);
                            decTotalDirectCashSettlementDifferenceBond =
                                Math.Round(decTotalDirectCashSettlementDifferenceBond, 2, MidpointRounding.AwayFromZero) +
                                (decRealCommission - oAllot.Commission);
                        }
                        else
                        {
                            decTotalDirectCashSettlementBond = decTotalDirectCashSettlementBond + oAllot.TotalAmount -
                                                               (oAllot.Commission - decRealCommission);
                            decTotalDirectCashSettlementDifferenceBond =
                                Math.Round(decTotalDirectCashSettlementDifferenceBond, 2, MidpointRounding.AwayFromZero) -
                                (oAllot.Commission - decRealCommission);
                        }

                        decTotalDirectCashSettlementBond = Math.Round(decTotalDirectCashSettlementBond, 2,
                            MidpointRounding.AwayFromZero);
                        decTotalDirectCashSettlementDifferenceBond = Math.Round(decTotalDirectCashSettlementDifferenceBond,
                            2, MidpointRounding.AwayFromZero);
                    }
                }

                oAllot.TicketNO = "DCS";
            }
            else
            {
                oAllot.TicketNO = oRowView["BSlip#"].ToString();
            }

            oAllot.Posted = true;
            oAllot.Reversed = false;
            oAllot.UserId = strUserName;
            oAllot.Cdeal = char.Parse("N");
            oAllot.Autopost = char.Parse("Y");
            oAllot.SoldBy = oRowView["SoldBy"].ToString();
            oAllot.BoughtBy = oRowView["BoughtBy"].ToString();
            oAllot.Buy_sold_Ind = char.Parse("S");
            oAllot.CDSellTrans = "";
            oAllot.OtherCust = "";
            oAllot.MarginCode = "";
            oAllot.CrossType = "";
            oAllot.PrintFlag = 'N';
            var dbCommandAllot = oAllot.AddCommand();
            db.ExecuteNonQuery(dbCommandAllot, transaction);
            strAllotmentNo = db.GetParameterValue(dbCommandAllot, "Txn").ToString().Trim();

            #endregion
        }

        private static bool SaveSeller(DataRow oRowView, StkParam oStkbPGenTable, bool blnChkPropAccount,
            CustomerExtraInformation oCustomerExtraInformationDoNotChargeStampDuty, string strUserName, SqlDatabase db,
            SqlTransaction transaction, ProductAcct oCustChkPropAccount, string strStockProductAccount,
            CustomerExtraInformation oCustomerExtraInformationDirectCash, ref decimal decTotalBankProp,
            ref decimal decTotalBankAuditProp, ref decimal decTotalBank, ref decimal decTotalBankAudit,
            ref decimal decTotalBankBond, ref decimal decTotalBankAuditBond, ref decimal decTotalSecProp,
            ref decimal decTotalSec, ref decimal decTotalSecBond, ref decimal decTotalStampDutyProp, Allotment oAllot,
            decimal decAllotConsideration, ref decimal decTotalStampDuty, decimal totalFeeBuyer,
            ref decimal decTotalStampDutyBond, ref decimal decTotalCommissionProp, ref decimal decTotalCommission,
            ref decimal decTotalCommissionBond, ref decimal decTotalVatProp, ref decimal decTotalVat,
            ref decimal decTotalVatBond, ref decimal decTotalNseProp, ref decimal decTotalNse,
            ref decimal decTotalNseBond,
            ref decimal decTotalCscsProp, ref decimal decTotalCscs, ref decimal decTotalCscsBond,
            ref string strAllotmentNo,
            ref decimal decTotalCommissionSellProp, ref decimal decTotalCommissionSell,
            ref decimal decTotalCommissionSellBond,
            ref decimal decTotalDirectCashSettlementProp, ref decimal decTotalDirectCashSettlement,
            ref decimal decTotalDirectCashSettlementBond, ref decimal decTotalDirectCashSettlementDifferenceProp,
            ref decimal decTotalDirectCashSettlementDifference, ref decimal decTotalDirectCashSettlementDifferenceBond,
            ref decimal decFeeCscsVatSeller, ref decimal decAllotConsiderationBuy, ref decimal decReturnAmount,
            ref decimal decFeeCscsSeller)
        {
            if (oRowView["CrossD"].ToString().Trim() == "Y")
            {
                #region Save Seller For 1.Ordinary Cross 2.Norminal Cross 3.Buyer For NA Cross Deal 4.Seller For NT Cross Deal

                decimal totalFeeSeller;
                if ((oRowView["CrossType"].ToString().Trim() == "CD") || (oRowView["CrossType"].ToString().Trim() == "NO")
                                                                      || (oRowView["CrossType"].ToString().Trim() ==
                                                                          "NA") ||
                                                                      (oRowView["CrossType"].ToString().Trim() == "NT"))
                {
                    oCustChkPropAccount.ProductCode = strStockProductAccount;
                    oCustChkPropAccount.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                    blnChkPropAccount = oCustChkPropAccount.GetBoxLoadStatus();

                    //Can change the id to the buyer with consideration here if clients want for NA Cross deal
                    oAllot.CustAID = oRowView["CrossCustNo"].ToString();
                    oAllot.StockCode = oRowView["Stockcode"].ToString().Trim();
                    oAllot.DateAlloted = oRowView["Date"].ToString().Trim().ToDate();
                    oAllot.Qty = long.Parse(oRowView["Units"].ToString().Trim());
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        oAllot.UnitPrice = Decimal.Parse(oRowView["UnitPrice"].ToString().Trim());
                        decAllotConsideration =
                            Math.Round(
                                Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                                long.Parse(oRowView["Units"].ToString().Trim()), 2);
                        decAllotConsiderationBuy =
                            Math.Round(
                                Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                                long.Parse(oRowView["Units"].ToString().Trim()), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        oAllot.UnitPrice = Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) * 10;
                        decAllotConsideration =
                            Math.Round(
                                (Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) * 10) *
                                long.Parse(oRowView["Units"].ToString().Trim()), 2);
                        decAllotConsiderationBuy =
                            Math.Round(
                                (Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) * 10) *
                                long.Parse(oRowView["Units"].ToString().Trim()), 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.CommissionType = "FIXED";
                    decAllotConsideration = Math.Round(decAllotConsideration, 2, MidpointRounding.AwayFromZero);
                    decAllotConsiderationBuy = Math.Round((decimal)decAllotConsiderationBuy, 2, MidpointRounding.AwayFromZero);
                    if ((oRowView["CrossType"].ToString().Trim() == "NO")
                        || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                        (oRowView["CrossType"].ToString().Trim() == "NT"))
                    {
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            if (blnChkPropAccount)
                            {
                                decTotalBankProp = decTotalBankProp + decAllotConsideration;
                                decTotalBankAuditProp = decTotalBankAuditProp + decAllotConsideration;
                                decTotalBankProp = Math.Round(decTotalBankProp, 2, MidpointRounding.AwayFromZero);
                                decTotalBankAuditProp = Math.Round(decTotalBankAuditProp, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                decTotalBank = decTotalBank + decAllotConsideration;
                                decTotalBankAudit = decTotalBankAudit + decAllotConsideration;
                                decTotalBank = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero);
                                decTotalBankAudit = Math.Round(decTotalBankAudit, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        else
                        {
                            decTotalBankBond = decTotalBankBond + decAllotConsideration;
                            decTotalBankAuditBond = decTotalBankAuditBond + decAllotConsideration;
                            decTotalBankBond = Math.Round(decTotalBankBond, 2, MidpointRounding.AwayFromZero);
                            decTotalBankAuditBond = Math.Round(decTotalBankAuditBond, 2, MidpointRounding.AwayFromZero);
                        }
                    }

                    oAllot.Consideration = decAllotConsideration;
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (decAllotConsideration * (oStkbPGenTable.Ssec / 100) > oStkbPGenTable.MinSceS)
                        {
                            oAllot.SecFee = decAllotConsideration * (oStkbPGenTable.Ssec / 100);
                        }
                        else
                        {
                            oAllot.SecFee = oStkbPGenTable.MinSceS;
                        }

                        oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Ssec;
                        decReturnAmount = Math.Round((decimal)decReturnAmount, 2, MidpointRounding.AwayFromZero);
                        oAllot.SecFee = decReturnAmount;
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oCustomerExtraInformationDoNotChargeStampDuty.CustAID = oRowView["CrossCustNo"].ToString().Trim();
                    if (!oCustomerExtraInformationDoNotChargeStampDuty.DoNotChargeStampDuty)
                    {
                        if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                        {
                            if (decAllotConsideration * (oStkbPGenTable.Sstamp / 100) > oStkbPGenTable.MinStampS)
                            {
                                oAllot.StampDuty = decAllotConsideration * (oStkbPGenTable.Sstamp / 100);
                            }
                            else
                            {
                                oAllot.StampDuty = oStkbPGenTable.MinStampS;
                            }

                            oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Sstamp;
                            decReturnAmount = Math.Round((decimal)decReturnAmount, 2, MidpointRounding.AwayFromZero);
                            oAllot.StampDuty = decReturnAmount;
                        }
                    }
                    else
                    {
                        oAllot.StampDuty = 0;
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero) +
                                                    oAllot.StampDuty;
                            decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) +
                                                oAllot.StampDuty;
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalStampDutyBond = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) +
                                                oAllot.StampDuty;
                        decTotalStampDutyBond = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.Commission = oAllot.ComputeComm(decAllotConsideration, "S", oStkbPGenTable.Scncomm,
                        oStkbPGenTable.MinCommS);
                    oAllot.Commission = Math.Round(oAllot.Commission, 2);

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero) +
                                                     oAllot.Commission;
                            decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero);

                            decTotalCommissionSellProp =
                                Math.Round(decTotalCommissionSellProp, 2, MidpointRounding.AwayFromZero) +
                                oAllot.Commission;
                            decTotalCommissionSellProp =
                                Math.Round(decTotalCommissionSellProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) +
                                                 oAllot.Commission;
                            decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero) +
                                                     oAllot.Commission;
                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCommissionBond = Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero) +
                                                 oAllot.Commission;
                        decTotalCommissionBond = Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero);

                        decTotalCommissionSellBond =
                            Math.Round(decTotalCommissionSellBond, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                        decTotalCommissionSellBond =
                            Math.Round(decTotalCommissionSellBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.VAT = oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "S", oStkbPGenTable.Svat,
                        oStkbPGenTable.MinVatS, oStkbPGenTable.Scncomm, oRowView["Date"].ToString().Trim().ToDate());
                    oAllot.VAT = Math.Round(oAllot.VAT, 2);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                            decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                            decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                        decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero);
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (decAllotConsideration * (oStkbPGenTable.Snse / 100) > oStkbPGenTable.MinSceS)
                        {
                            oAllot.NSEFee = decAllotConsideration * (oStkbPGenTable.Snse / 100);
                        }
                        else
                        {
                            oAllot.NSEFee = oStkbPGenTable.MinSceS;
                        }

                        oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Snse;
                        decReturnAmount = Math.Round((decimal)decReturnAmount, 2, MidpointRounding.AwayFromZero);
                        oAllot.NSEFee = decReturnAmount;
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                            decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                            decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                        decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero);
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (decAllotConsideration * (oStkbPGenTable.Scscs / 100) > oStkbPGenTable.MinCscsS)
                        {
                            oAllot.CSCSFee = decAllotConsideration * (oStkbPGenTable.Scscs / 100);
                        }
                        else
                        {
                            oAllot.CSCSFee = oStkbPGenTable.MinCscsS;
                        }

                        oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Scscs;
                        decReturnAmount = Math.Round((decimal)decReturnAmount, 0, MidpointRounding.AwayFromZero);
                        oAllot.CSCSFee = decReturnAmount;
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               oAllot.CSCSFee;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.SMSAlertCSCS = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) *
                                          oStkbPGenTable.SMSAlertCSCSS;
                    oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               oAllot.SMSAlertCSCS;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) +
                                           oAllot.SMSAlertCSCS;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());

                    if (oAllot.SecFee * (oStkbPGenTable.SsecVat / 100) > oStkbPGenTable.MinSecVatS)
                    {
                        oAllot.SecVat = oAllot.SecFee * (oStkbPGenTable.SsecVat / 100);
                    }
                    else
                    {
                        oAllot.SecVat = oStkbPGenTable.MinSecVatS;
                    }

                    oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
                    }

                    if (oAllot.NSEFee * (oStkbPGenTable.SnseVat / 100) > oStkbPGenTable.MinNseVatS)
                    {
                        oAllot.NseVat = oAllot.NSEFee * (oStkbPGenTable.SnseVat / 100);
                    }
                    else
                    {
                        oAllot.NseVat = oStkbPGenTable.MinNseVatS;
                    }

                    oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                            decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                            decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                        decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero);
                    }

                    if (oAllot.CSCSFee * (oStkbPGenTable.ScscsVat / 100) > oStkbPGenTable.MinCscsVatS)
                    {
                        oAllot.CscsVat = oAllot.CSCSFee * (oStkbPGenTable.ScscsVat / 100);
                    }
                    else
                    {
                        oAllot.CscsVat = oStkbPGenTable.MinCscsVatS;
                    }

                    oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               oAllot.CscsVat;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.SMSAlertCSCSVAT = oAllot.SMSAlertCSCS * (oStkbPGenTable.SMSAlertCSCSVATS / 100);
                    oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               oAllot.SMSAlertCSCSVAT;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) +
                                           oAllot.SMSAlertCSCSVAT;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) +
                                           oAllot.SMSAlertCSCSVAT;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }


                    oAllot.TotalAmount = decAllotConsideration - (oAllot.SecFee + oAllot.StampDuty
                        + oAllot.Commission + oAllot.VAT +
                        oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                        oAllot.NSEFee
                        + oAllot.SecVat + oAllot.NseVat +
                        oAllot.CscsVat + oAllot.SMSAlertCSCSVAT);
                    oAllot.TotalAmount = Math.Round(oAllot.TotalAmount, 2, MidpointRounding.AwayFromZero);

                    totalFeeSeller = oAllot.SecFee + oAllot.StampDuty
                                                   + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                                                   oAllot.NSEFee
                                                   + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat +
                                                   oAllot.SMSAlertCSCSVAT;
                    totalFeeSeller = Math.Round(totalFeeSeller, 2, MidpointRounding.AwayFromZero);

                    if (oRowView["CrossType"].ToString().Trim() == "CD")
                    {
                        oAllot.TotalAmount = totalFeeSeller;
                    }

                    if (oRowView["CrossType"].ToString().Trim() == "NO" || oRowView["CrossType"].ToString().Trim() == "NA"
                                                                        || oRowView["CrossType"].ToString().Trim() == "NT")
                    {
                        oCustomerExtraInformationDirectCash.CustAID = oRowView["CrossCustNo"].ToString();
                        oCustomerExtraInformationDirectCash.WedAnniversaryDate = oRowView["Date"].ToString().Trim().ToDate();
                        if (oCustomerExtraInformationDirectCash.DirectCashSettlement)
                        {
                            decimal decRealCommission = 0;
                            if (decAllotConsideration * (oStkbPGenTable.Scncomm / 100) > oStkbPGenTable.MinCommS)
                            {
                                decRealCommission = decAllotConsideration * (oStkbPGenTable.Scncomm / 100);
                            }
                            else
                            {
                                decRealCommission = oStkbPGenTable.MinCommS;
                            }

                            decRealCommission = Math.Round(decRealCommission, 2, MidpointRounding.AwayFromZero);

                            if (oAllot.Commission == decRealCommission)
                            {
                                if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                                {
                                    if (blnChkPropAccount)
                                    {
                                        decTotalDirectCashSettlementProp =
                                            decTotalDirectCashSettlementProp + oAllot.TotalAmount;
                                        decTotalDirectCashSettlementProp = Math.Round(decTotalDirectCashSettlementProp, 2,
                                            MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        decTotalDirectCashSettlement = decTotalDirectCashSettlement + oAllot.TotalAmount;
                                        decTotalDirectCashSettlement = Math.Round(decTotalDirectCashSettlement, 2,
                                            MidpointRounding.AwayFromZero);
                                    }
                                }
                                else
                                {
                                    decTotalDirectCashSettlementBond =
                                        decTotalDirectCashSettlementBond + oAllot.TotalAmount;
                                    decTotalDirectCashSettlementBond = Math.Round(decTotalDirectCashSettlementBond, 2,
                                        MidpointRounding.AwayFromZero);
                                }
                            }
                            else
                            {
                                if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                                {
                                    if (blnChkPropAccount)
                                    {
                                        if ((decRealCommission - oAllot.Commission) > 0)
                                        {
                                            decTotalDirectCashSettlementProp = decTotalDirectCashSettlementProp +
                                                                               oAllot.TotalAmount +
                                                                               (decRealCommission - oAllot.Commission);
                                            decTotalDirectCashSettlementDifferenceProp =
                                                Math.Round(decTotalDirectCashSettlementDifferenceProp, 2,
                                                    MidpointRounding.AwayFromZero) +
                                                (decRealCommission - oAllot.Commission);
                                        }
                                        else
                                        {
                                            decTotalDirectCashSettlementProp = decTotalDirectCashSettlementProp +
                                                oAllot.TotalAmount - (oAllot.Commission - decRealCommission);
                                            decTotalDirectCashSettlementDifferenceProp =
                                                Math.Round(decTotalDirectCashSettlementDifferenceProp, 2,
                                                    MidpointRounding.AwayFromZero) -
                                                (oAllot.Commission - decRealCommission);
                                        }

                                        decTotalDirectCashSettlementProp = Math.Round(decTotalDirectCashSettlementProp, 2,
                                            MidpointRounding.AwayFromZero);
                                        decTotalDirectCashSettlementDifferenceProp = Math.Round(
                                            decTotalDirectCashSettlementDifferenceProp, 2, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        if ((decRealCommission - oAllot.Commission) > 0)
                                        {
                                            decTotalDirectCashSettlement = decTotalDirectCashSettlement +
                                                                           oAllot.TotalAmount +
                                                                           (decRealCommission - oAllot.Commission);
                                            decTotalDirectCashSettlementDifference =
                                                Math.Round(decTotalDirectCashSettlementDifference, 2,
                                                    MidpointRounding.AwayFromZero) +
                                                (decRealCommission - oAllot.Commission);
                                        }
                                        else
                                        {
                                            decTotalDirectCashSettlement = decTotalDirectCashSettlement +
                                                oAllot.TotalAmount - (oAllot.Commission - decRealCommission);
                                            decTotalDirectCashSettlementDifference =
                                                Math.Round(decTotalDirectCashSettlementDifference, 2,
                                                    MidpointRounding.AwayFromZero) -
                                                (oAllot.Commission - decRealCommission);
                                        }

                                        decTotalDirectCashSettlement = Math.Round(decTotalDirectCashSettlement, 2,
                                            MidpointRounding.AwayFromZero);
                                        decTotalDirectCashSettlementDifference = Math.Round(
                                            decTotalDirectCashSettlementDifference, 2, MidpointRounding.AwayFromZero);
                                    }
                                }
                                else
                                {
                                    if ((decRealCommission - oAllot.Commission) > 0)
                                    {
                                        decTotalDirectCashSettlementBond = decTotalDirectCashSettlementBond +
                                                                           oAllot.TotalAmount +
                                                                           (decRealCommission - oAllot.Commission);
                                        decTotalDirectCashSettlementDifferenceBond =
                                            Math.Round(decTotalDirectCashSettlementDifferenceBond, 2,
                                                MidpointRounding.AwayFromZero) + (decRealCommission - oAllot.Commission);
                                    }
                                    else
                                    {
                                        decTotalDirectCashSettlementBond = decTotalDirectCashSettlementBond +
                                            oAllot.TotalAmount - (oAllot.Commission - decRealCommission);
                                        decTotalDirectCashSettlementDifferenceBond =
                                            Math.Round(decTotalDirectCashSettlementDifferenceBond, 2,
                                                MidpointRounding.AwayFromZero) - (oAllot.Commission - decRealCommission);
                                    }

                                    decTotalDirectCashSettlementBond = Math.Round(decTotalDirectCashSettlementBond, 2,
                                        MidpointRounding.AwayFromZero);
                                    decTotalDirectCashSettlementDifferenceBond = Math.Round(
                                        decTotalDirectCashSettlementDifferenceBond, 2, MidpointRounding.AwayFromZero);
                                }
                            }

                            oAllot.TicketNO = "DCS";
                        }
                        else
                        {
                            oAllot.TicketNO = oRowView["BSlip#"].ToString();
                        }
                    }
                    else
                    {
                        oAllot.TicketNO = oRowView["BSlip#"].ToString();
                    }

                    oAllot.Posted = true;
                    oAllot.Reversed = false;
                    oAllot.UserId = strUserName;
                    oAllot.Cdeal = Char.Parse("Y");
                    oAllot.Autopost = Char.Parse("Y");

                    oAllot.SoldBy = oRowView["SoldBy"].ToString();
                    oAllot.BoughtBy = oRowView["BoughtBy"].ToString();
                    oAllot.Buy_sold_Ind = Char.Parse("S");
                    oAllot.CDSellTrans = strAllotmentNo;
                    oAllot.OtherCust = oRowView["Custno"].ToString();
                    ;
                    oAllot.MarginCode = "";
                    oAllot.CrossType = oRowView["CrossType"].ToString();
                    if (oRowView["CrossType"].ToString().Trim() == "CD")
                    {
                        oAllot.Consideration = 0;
                    }

                    oAllot.PrintFlag = 'N';
                    var dbCommandAllot = oAllot.AddCommand();
                    db.ExecuteNonQuery(dbCommandAllot, transaction);
                    db.GetParameterValue(dbCommandAllot, "Txn").ToString().Trim();
                }

                #endregion

                #region Save 1.Buyer For NB Cross Deal 2.Seller For NS Cross Deal

                else
                {
                    decimal decFeeSecSeller;
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (decAllotConsideration * (oStkbPGenTable.Ssec / 100) > oStkbPGenTable.MinSceS)
                        {
                            oAllot.SecFee = oAllot.SecFee +
                                            Math.Round((decAllotConsideration * (oStkbPGenTable.Ssec / 100)), 2,
                                                MidpointRounding.AwayFromZero);
                            decFeeSecSeller = decAllotConsideration * (oStkbPGenTable.Ssec / 100);
                        }
                        else
                        {
                            oAllot.SecFee = oAllot.SecFee + oStkbPGenTable.MinSceS;
                            decFeeSecSeller = oStkbPGenTable.MinSceS;
                        }

                        oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
                        decFeeSecSeller = Math.Round(decFeeSecSeller, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Ssec;
                        decReturnAmount = Math.Round((decimal)decReturnAmount, 2, MidpointRounding.AwayFromZero);
                        oAllot.SecFee = oAllot.SecFee + decReturnAmount;
                        decFeeSecSeller = decReturnAmount;
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) +
                                              decFeeSecSeller;
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + decFeeSecSeller;
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) + decFeeSecSeller;
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
                    }

                    decimal decFeeStampDutySeller;
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        oCustomerExtraInformationDoNotChargeStampDuty.CustAID = oRowView["CustNo"].ToString().Trim();
                        if (!oCustomerExtraInformationDoNotChargeStampDuty.DoNotChargeStampDuty)
                        {
                            if (decAllotConsideration * (oStkbPGenTable.Sstamp / 100) > oStkbPGenTable.MinStampS)
                            {
                                oAllot.StampDuty = oAllot.StampDuty +
                                                   Math.Round((decAllotConsideration * (oStkbPGenTable.Sstamp / 100)), 2);
                                decFeeStampDutySeller = (decAllotConsideration * (oStkbPGenTable.Sstamp / 100));
                            }
                            else
                            {
                                oAllot.StampDuty = oAllot.StampDuty + oStkbPGenTable.MinStampS;
                                decFeeStampDutySeller = oStkbPGenTable.MinStampS;
                            }
                        }
                        else
                        {
                            oAllot.StampDuty = oAllot.StampDuty + 0;
                            decFeeStampDutySeller = 0;
                        }

                        oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2);
                        decFeeStampDutySeller = Math.Round(decFeeStampDutySeller, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        oCustomerExtraInformationDoNotChargeStampDuty.CustAID = oRowView["CustNo"].ToString().Trim();
                        if (!oCustomerExtraInformationDoNotChargeStampDuty.DoNotChargeStampDuty)
                        {
                            decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Sstamp;
                            decReturnAmount = Math.Round((decimal)decReturnAmount, 2, MidpointRounding.AwayFromZero);
                            oAllot.StampDuty = oAllot.StampDuty + decReturnAmount;
                            decFeeStampDutySeller = decReturnAmount;
                        }
                        else
                        {
                            oAllot.StampDuty = oAllot.StampDuty + 0;
                            decFeeStampDutySeller = 0;
                        }
                    }


                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero) +
                                                    decFeeStampDutySeller;
                            decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) +
                                                decFeeStampDutySeller;
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalStampDutyBond = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) +
                                                decFeeStampDutySeller;
                        decTotalStampDutyBond = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.Commission = oAllot.Commission + oAllot.ComputeComm(decAllotConsideration, "S",
                        oStkbPGenTable.Scncomm, oStkbPGenTable.MinCommS);
                    oAllot.Commission = Math.Round(oAllot.Commission, 2);
                    var decFeeCommissionSeller = oAllot.ComputeComm(decAllotConsideration, "S", oStkbPGenTable.Scncomm,
                        oStkbPGenTable.MinCommS);
                    decFeeCommissionSeller = Math.Round(decFeeCommissionSeller, 2, MidpointRounding.AwayFromZero);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero) +
                                                     decFeeCommissionSeller;
                            decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero);

                            decTotalCommissionSellProp =
                                Math.Round(decTotalCommissionSellProp, 2, MidpointRounding.AwayFromZero) +
                                decFeeCommissionSeller;
                            decTotalCommissionSellProp =
                                Math.Round(decTotalCommissionSellProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) +
                                                 decFeeCommissionSeller;
                            decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero) +
                                                     decFeeCommissionSeller;
                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCommissionBond = Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero) +
                                                 decFeeCommissionSeller;
                        decTotalCommissionBond = Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero);

                        decTotalCommissionSellBond =
                            Math.Round(decTotalCommissionSellBond, 2, MidpointRounding.AwayFromZero) +
                            decFeeCommissionSeller;
                        decTotalCommissionSellBond =
                            Math.Round(decTotalCommissionSellBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.VAT = oAllot.VAT + oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "S",
                        oStkbPGenTable.Svat, oStkbPGenTable.MinVatS, oStkbPGenTable.Scncomm,
                        oRowView["Date"].ToString().Trim().ToDate());
                    oAllot.VAT = Math.Round(oAllot.VAT, 2);
                    var decFeeVatSeller = oAllot.ComputeVAT(decAllotConsideration, decFeeCommissionSeller, "S",
                        oStkbPGenTable.Svat, oStkbPGenTable.MinVatS, oStkbPGenTable.Scncomm,
                        oRowView["Date"].ToString().Trim().ToDate());
                    decFeeVatSeller = Math.Round(decFeeVatSeller, 2, MidpointRounding.AwayFromZero);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero) +
                                              decFeeVatSeller;
                            decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero) + decFeeVatSeller;
                            decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero) + decFeeVatSeller;
                        decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero);
                    }

                    decimal decFeeNseSeller;
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (decAllotConsideration * (oStkbPGenTable.Snse / 100) > oStkbPGenTable.MinSceS)
                        {
                            oAllot.NSEFee = oAllot.NSEFee +
                                            Math.Round((decAllotConsideration * (oStkbPGenTable.Snse / 100)), 2);
                            decFeeNseSeller = (decAllotConsideration * (oStkbPGenTable.Snse / 100));
                        }
                        else
                        {
                            oAllot.NSEFee = oAllot.NSEFee + oStkbPGenTable.MinSceS;
                            decFeeNseSeller = oStkbPGenTable.MinSceS;
                        }

                        oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2);
                        decFeeNseSeller = Math.Round(decFeeNseSeller, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Snse;
                        decReturnAmount = Math.Round((decimal)decReturnAmount, 2, MidpointRounding.AwayFromZero);
                        oAllot.NSEFee = oAllot.NSEFee + decReturnAmount;
                        decFeeNseSeller = decReturnAmount;
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero) +
                                              decFeeNseSeller;
                            decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero) + decFeeNseSeller;
                            decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero) + decFeeNseSeller;
                        decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero);
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (decAllotConsideration * (oStkbPGenTable.Scscs / 100) > oStkbPGenTable.MinCscsS)
                        {
                            oAllot.CSCSFee = oAllot.CSCSFee +
                                             Math.Round((decAllotConsideration * (oStkbPGenTable.Scscs / 100)), 2);
                            decFeeCscsSeller = (decAllotConsideration * (oStkbPGenTable.Scscs / 100));
                        }
                        else
                        {
                            oAllot.CSCSFee = oAllot.CSCSFee + oStkbPGenTable.MinCscsS;
                            decFeeCscsSeller = oStkbPGenTable.MinCscsS;
                        }

                        oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2);
                        decFeeCscsSeller = Math.Round((decimal)decFeeCscsSeller, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Scscs;
                        decReturnAmount = Math.Round((decimal)decReturnAmount, 0, MidpointRounding.AwayFromZero);
                        oAllot.CSCSFee = oAllot.CSCSFee + decReturnAmount;
                        decFeeCscsSeller = decReturnAmount;
                    }

                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               decFeeCscsSeller;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + decFeeCscsSeller;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) +
                                           decFeeCscsSeller;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.SMSAlertCSCS = oAllot.SMSAlertCSCS + (int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) *
                                                                 oStkbPGenTable.SMSAlertCSCSS);
                    oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
                    var decSmsAlertCscsSeller = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) *
                                                oStkbPGenTable.SMSAlertCSCSS;
                    decSmsAlertCscsSeller = Math.Round(decSmsAlertCscsSeller, 2);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               decSmsAlertCscsSeller;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) +
                                           decSmsAlertCscsSeller;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) +
                                           decSmsAlertCscsSeller;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());

                    if (decFeeSecSeller * (oStkbPGenTable.SsecVat / 100) > oStkbPGenTable.MinSecVatS)
                    {
                        oAllot.SecVat = oAllot.SecVat + Math.Round((decFeeSecSeller * (oStkbPGenTable.SsecVat / 100)), 2,
                            MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        oAllot.SecVat = oAllot.SecVat + oStkbPGenTable.MinSecVatS;
                    }

                    decimal decFeeSecVatSeller;
                    if (decFeeSecSeller * (oStkbPGenTable.SsecVat / 100) > oStkbPGenTable.MinSecVatS)
                    {
                        decFeeSecVatSeller = decFeeSecSeller * (oStkbPGenTable.SsecVat / 100);
                    }
                    else
                    {
                        decFeeSecVatSeller = oStkbPGenTable.MinSecVatS;
                    }

                    oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
                    decFeeSecVatSeller = Math.Round(decFeeSecVatSeller, 2, MidpointRounding.AwayFromZero);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) +
                                              decFeeSecVatSeller;
                            decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + decFeeSecVatSeller;
                            decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) +
                                          decFeeSecVatSeller;
                        decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
                    }

                    if (decFeeNseSeller * (oStkbPGenTable.SnseVat / 100) > oStkbPGenTable.MinNseVatS)
                    {
                        oAllot.NseVat = oAllot.NseVat + Math.Round((decFeeNseSeller * (oStkbPGenTable.SnseVat / 100)), 2);
                    }
                    else
                    {
                        oAllot.NseVat = oAllot.NseVat + oStkbPGenTable.MinNseVatS;
                    }

                    decimal decFeeNseVatSeller;
                    if (decFeeNseSeller * (oStkbPGenTable.SnseVat / 100) > oStkbPGenTable.MinNseVatS)
                    {
                        decFeeNseVatSeller = (decFeeNseSeller * (oStkbPGenTable.SnseVat / 100));
                    }
                    else
                    {
                        decFeeNseVatSeller = oStkbPGenTable.MinNseVatS;
                    }

                    oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
                    decFeeNseVatSeller = Math.Round(decFeeNseVatSeller, 2, MidpointRounding.AwayFromZero);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               decFeeCscsVatSeller;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + decFeeCscsVatSeller;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) +
                                           decFeeCscsVatSeller;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    if (decFeeCscsSeller * (oStkbPGenTable.ScscsVat / 100) > oStkbPGenTable.MinCscsVatS)
                    {
                        oAllot.CscsVat = oAllot.CscsVat +
                                         Math.Round((decFeeCscsSeller * (oStkbPGenTable.ScscsVat / 100)), 2);
                    }
                    else
                    {
                        oAllot.CscsVat = oAllot.CscsVat + oStkbPGenTable.MinCscsVatS;
                    }

                    if (decFeeCscsSeller * (oStkbPGenTable.ScscsVat / 100) > oStkbPGenTable.MinCscsVatS)
                    {
                        decFeeCscsVatSeller = (decFeeCscsSeller * (oStkbPGenTable.ScscsVat / 100));
                    }
                    else
                    {
                        decFeeCscsVatSeller = oStkbPGenTable.MinCscsVatS;
                    }

                    oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
                    decFeeCscsVatSeller = Math.Round(decFeeCscsVatSeller, 2, MidpointRounding.AwayFromZero);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               decFeeCscsVatSeller;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + decFeeCscsVatSeller;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) +
                                           decFeeCscsVatSeller;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.SMSAlertCSCSVAT = oAllot.SMSAlertCSCSVAT +
                                             Math.Round((decSmsAlertCscsSeller * (oStkbPGenTable.SMSAlertCSCSVATS / 100)),
                                                 2);
                    var decSmsAlertCscsvatSeller = (decSmsAlertCscsSeller * (oStkbPGenTable.SMSAlertCSCSVATS / 100));
                    oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2, MidpointRounding.AwayFromZero);
                    decSmsAlertCscsvatSeller = Math.Round(decSmsAlertCscsvatSeller, 2, MidpointRounding.AwayFromZero);
                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                    {
                        if (blnChkPropAccount)
                        {
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                               decSmsAlertCscsvatSeller;
                            decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) +
                                           decSmsAlertCscsvatSeller;
                            decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) +
                                           decSmsAlertCscsvatSeller;
                        decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
                    }

                    totalFeeSeller = oAllot.SecFee + oAllot.StampDuty
                                                   + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                                                   oAllot.NSEFee
                                                   + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat +
                                                   oAllot.SMSAlertCSCSVAT;
                    totalFeeSeller = Math.Round(totalFeeSeller, 2, MidpointRounding.AwayFromZero);

                    var totalFeeSellerPost = decFeeSecSeller + decFeeStampDutySeller
                                                             + decFeeCommissionSeller + decFeeVatSeller + decFeeCscsSeller +
                                                             decSmsAlertCscsSeller + decFeeNseSeller
                                                             + decFeeSecVatSeller + decFeeNseVatSeller + decFeeCscsVatSeller +
                                                             decSmsAlertCscsvatSeller;
                    totalFeeSellerPost = Math.Round(totalFeeSellerPost, 2, MidpointRounding.AwayFromZero);

                    //Change TotalFeeSeller to TotalFeeSellerPost because I suspect error
                    oAllot.TotalAmount = totalFeeBuyer + totalFeeSellerPost;

                    //Changes For One Transaction Only For NB and NS---- Change It Recently Because It did not reverse in GL and portfolio table
                    //if (oRowView["CrossType"].ToString().Trim() == "NS")
                    //{
                    //    oAllot.Buy_sold_Ind = Char.Parse("S");
                    //}

                    var dbCommandAllot = oAllot.AddCommand();
                    db.ExecuteNonQuery(dbCommandAllot, transaction);
                    strAllotmentNo = db.GetParameterValue(dbCommandAllot, "Txn").ToString().Trim();
                }

                #endregion
            }

            return blnChkPropAccount;
        }

        private static decimal SaveBuyer(DataRow oRowView, StkParam oStkbPGenTable, bool blnChkPropAccount,
            CustomerExtraInformation oCustomerExtraInformationDoNotChargeStampDuty, string strUserName, SqlDatabase db,
            SqlTransaction transaction, ref decimal decTotalBankProp, ref decimal decTotalBankAuditProp,
            ref decimal decTotalBank, ref decimal decTotalBankAudit, ref decimal decTotalBankBond,
            ref decimal decTotalBankAuditBond, ref decimal decTotalSecProp, ref decimal decTotalSec,
            ref decimal decTotalSecBond, ref decimal decTotalStampDutyProp, Allotment oAllot, ref decimal decTotalStampDuty,
            ref decimal decTotalStampDutyBond, ref decimal decTotalCommissionProp, ref decimal decTotalCommissionBuyProp,
            ref decimal decTotalCommission, ref decimal decTotalCommissionBuy, ref decimal decTotalCommissionBond,
            ref decimal decTotalCommissionBuyBond, ref decimal decTotalVatProp, ref decimal decTotalVat,
            ref decimal decTotalVatBond, ref decimal decTotalNseProp, ref decimal decTotalNse, ref decimal decTotalNseBond,
            ref decimal decTotalCscsProp, ref decimal decTotalCscs, ref decimal decTotalCscsBond, ref string strAllotmentNo,
            out decimal totalFeeBuyer)
        {
            decimal decAllotConsideration;
            oAllot.CustAID = oRowView["CustNo"].ToString();
            oAllot.StockCode = oRowView["Stockcode"].ToString();
            oAllot.DateAlloted =
                oRowView["Date"].ToString().Trim().ToDate();
            oAllot.Qty = long.Parse(oRowView["Units"].ToString());
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                oAllot.UnitPrice = decimal.Parse(oRowView["UnitPrice"].ToString());
            }
            else
            {
                oAllot.UnitPrice = decimal.Parse(oRowView["UnitPrice"].ToString()) * 10;
            }

            oAllot.CommissionType = "FIXED";
            decimal decAllotConsiderationBuy;
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                decAllotConsideration =
                    Math.Round(
                        decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                        long.Parse(oRowView["Units"].ToString().Trim()), 2);
                decAllotConsiderationBuy =
                    Math.Round(
                        decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                        long.Parse(oRowView["Units"].ToString().Trim()), 2);
            }
            else
            {
                decAllotConsideration =
                    Math.Round(
                        (decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) * 10) *
                        long.Parse(oRowView["Units"].ToString().Trim()), 2);
                decAllotConsiderationBuy =
                    Math.Round(
                        (decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) * 10) *
                        long.Parse(oRowView["Units"].ToString().Trim()), 2);
            }

            decAllotConsideration = Math.Round(decAllotConsideration, 2, MidpointRounding.AwayFromZero);
            decAllotConsiderationBuy = Math.Round(decAllotConsiderationBuy, 2, MidpointRounding.AwayFromZero);
            if ((oRowView["CrossD"].ToString().Trim() == "N") || (oRowView["CrossType"].ToString().Trim() == "NO")
                                                              || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                                                              (oRowView["CrossType"].ToString().Trim() == "NT"))
            {
                if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                {
                    if (blnChkPropAccount)
                    {
                        decTotalBankProp = decTotalBankProp - decAllotConsideration;
                        decTotalBankAuditProp = decTotalBankAuditProp + decAllotConsideration;
                        decTotalBankProp = Math.Round(decTotalBankProp, 2, MidpointRounding.AwayFromZero);
                        decTotalBankAuditProp = Math.Round(decTotalBankAuditProp, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        decTotalBank = decTotalBank - decAllotConsideration;
                        decTotalBankAudit = decTotalBankAudit + decAllotConsideration;
                        decTotalBank = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero);
                        decTotalBankAudit = Math.Round(decTotalBankAudit, 2, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    decTotalBankBond = decTotalBankBond - decAllotConsideration;
                    decTotalBankAuditBond = decTotalBankAuditBond + decAllotConsideration;
                    decTotalBankBond = Math.Round(decTotalBankBond, 2, MidpointRounding.AwayFromZero);
                    decTotalBankAudit = Math.Round(decTotalBankAudit, 2, MidpointRounding.AwayFromZero);
                    decTotalBankAuditBond = Math.Round(decTotalBankAuditBond, 2, MidpointRounding.AwayFromZero);
                }
            }

            oAllot.Consideration = decAllotConsideration;
            decimal decReturnAmount = 0;
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (decAllotConsideration * (oStkbPGenTable.Bsec / 100) > oStkbPGenTable.MinSecB)
                {
                    oAllot.SecFee = decAllotConsideration * (oStkbPGenTable.Bsec / 100);
                }
                else
                {
                    oAllot.SecFee = oStkbPGenTable.MinSecB;
                }

                oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Bsec;
                decReturnAmount = Math.Round(decReturnAmount, 2, MidpointRounding.AwayFromZero);
                oAllot.SecFee = decReturnAmount;
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
            }

            oCustomerExtraInformationDoNotChargeStampDuty.CustAID = oRowView["CustNo"].ToString().Trim();
            if (!oCustomerExtraInformationDoNotChargeStampDuty.DoNotChargeStampDuty)
            {
                if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                {
                    if (decAllotConsideration * (oStkbPGenTable.Bstamp / 100) > oStkbPGenTable.MinStampB)
                    {
                        oAllot.StampDuty = decAllotConsideration * (oStkbPGenTable.Bstamp / 100);
                    }
                    else
                    {
                        oAllot.StampDuty = oStkbPGenTable.MinStampB;
                    }

                    oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Bstamp;
                    decReturnAmount = Math.Round(decReturnAmount, 2, MidpointRounding.AwayFromZero);
                    oAllot.StampDuty = decReturnAmount;
                }
            }
            else
            {
                oAllot.StampDuty = 0;
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero) +
                                            oAllot.StampDuty;
                    decTotalStampDutyProp = Math.Round(decTotalStampDutyProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + oAllot.StampDuty;
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalStampDutyBond =
                    Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero) + oAllot.StampDuty;
                decTotalStampDutyBond = Math.Round(decTotalStampDutyBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.Commission =
                oAllot.ComputeComm(decAllotConsideration, "B", oStkbPGenTable.Bcncomm, oStkbPGenTable.MinCommB);
            oAllot.Commission = Math.Round(oAllot.Commission, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero) +
                                             oAllot.Commission;
                    decTotalCommissionProp = Math.Round(decTotalCommissionProp, 2, MidpointRounding.AwayFromZero);

                    decTotalCommissionBuyProp = Math.Round(decTotalCommissionBuyProp, 2, MidpointRounding.AwayFromZero) +
                                                oAllot.Commission;
                    decTotalCommissionBuyProp = Math.Round(decTotalCommissionBuyProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCommission =
                        Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                    decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                    decTotalCommissionBuy = Math.Round(decTotalCommissionBuy, 2, MidpointRounding.AwayFromZero) +
                                            oAllot.Commission;
                    decTotalCommissionBuy = Math.Round(decTotalCommissionBuy, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCommissionBond =
                    Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                decTotalCommissionBond = Math.Round(decTotalCommissionBond, 2, MidpointRounding.AwayFromZero);

                decTotalCommissionBuyBond = Math.Round(decTotalCommissionBuyBond, 2, MidpointRounding.AwayFromZero) +
                                            oAllot.Commission;
                decTotalCommissionBuyBond = Math.Round(decTotalCommissionBuyBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.VAT = oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "B", oStkbPGenTable.Bvat,
                oStkbPGenTable.MinVatB, oStkbPGenTable.Bcncomm,
                oRowView["Date"].ToString().Trim().ToDate());
            oAllot.VAT = Math.Round(oAllot.VAT, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                    decTotalVatProp = Math.Round(decTotalVatProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                    decTotalVat = Math.Round(decTotalVat, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                decTotalVatBond = Math.Round(decTotalVatBond, 2, MidpointRounding.AwayFromZero);
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (decAllotConsideration * (oStkbPGenTable.Bnse / 100) > oStkbPGenTable.MinNSceB)
                {
                    oAllot.NSEFee = decAllotConsideration * (oStkbPGenTable.Bnse / 100);
                }
                else
                {
                    oAllot.NSEFee = oStkbPGenTable.MinNSceB;
                }

                oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.Bnse;
                decReturnAmount = Math.Round(decReturnAmount, 2, MidpointRounding.AwayFromZero);
                oAllot.NSEFee = decReturnAmount;
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero);
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (decAllotConsideration * (oStkbPGenTable.BCsCs / 100) > oStkbPGenTable.MinCscsB)
                {
                    oAllot.CSCSFee = decAllotConsideration * (oStkbPGenTable.BCsCs / 100);
                }
                else
                {
                    oAllot.CSCSFee = oStkbPGenTable.MinCscsB;
                }

                oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                decReturnAmount = (decAllotConsideration / 100) * oStkbPGenTable.BCsCs;
                decReturnAmount = Math.Round(decReturnAmount, 0, MidpointRounding.AwayFromZero);
                oAllot.CSCSFee = decReturnAmount;
            }

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.SMSAlertCSCS = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) * oStkbPGenTable.SMSAlertCSCSB;
            oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());

            if (oAllot.SecFee * (oStkbPGenTable.BsecVat / 100) > oStkbPGenTable.MinSecVatB)
            {
                oAllot.SecVat = oAllot.SecFee * (oStkbPGenTable.BsecVat / 100);
            }
            else
            {
                oAllot.SecVat = oStkbPGenTable.MinSecVatB;
            }

            oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                    decTotalSecProp = Math.Round(decTotalSecProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                    decTotalSec = Math.Round(decTotalSec, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                decTotalSecBond = Math.Round(decTotalSecBond, 2, MidpointRounding.AwayFromZero);
            }

            if (oAllot.NSEFee * (oStkbPGenTable.BnseVat / 100) > oStkbPGenTable.MinNseVatB)
            {
                oAllot.NseVat = oAllot.NSEFee * (oStkbPGenTable.BnseVat / 100);
            }
            else
            {
                oAllot.NseVat = oStkbPGenTable.MinNseVatB;
            }

            oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                    decTotalNseProp = Math.Round(decTotalNseProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                    decTotalNse = Math.Round(decTotalNse, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                decTotalNseBond = Math.Round(decTotalNseBond, 2, MidpointRounding.AwayFromZero);
            }

            if (oAllot.CSCSFee * (oStkbPGenTable.BcscsVat / 100) > oStkbPGenTable.MinCscsVATB)
            {
                oAllot.CscsVat = oAllot.CSCSFee * (oStkbPGenTable.BcscsVat / 100);
            }
            else
            {
                oAllot.CscsVat = oStkbPGenTable.MinCscsVATB;
            }

            oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.SMSAlertCSCSVAT = oAllot.SMSAlertCSCS * (oStkbPGenTable.SMSAlertCSCSVATB / 100);
            oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2);

            if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
            {
                if (blnChkPropAccount)
                {
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero) +
                                       oAllot.SMSAlertCSCSVAT;
                    decTotalCscsProp = Math.Round(decTotalCscsProp, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCSVAT;
                    decTotalCscs = Math.Round(decTotalCscs, 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCSVAT;
                decTotalCscsBond = Math.Round(decTotalCscsBond, 2, MidpointRounding.AwayFromZero);
            }

            oAllot.TotalAmount = decAllotConsideration + oAllot.SecFee + oAllot.StampDuty
                                 + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS + oAllot.NSEFee
                                 + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat + oAllot.SMSAlertCSCSVAT;
            oAllot.TotalAmount = Math.Round(oAllot.TotalAmount, 2);

            //Populate decFeeTotalAmount Only at this stage for Buy Cross deal N or cross Type NO only
            totalFeeBuyer = oAllot.SecFee + oAllot.StampDuty
                                          + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                                          oAllot.NSEFee
                                          + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat + oAllot.SMSAlertCSCSVAT;
            totalFeeBuyer = Math.Round(totalFeeBuyer, 2, MidpointRounding.AwayFromZero);

            if (oRowView["CrossType"].ToString().Trim() == "CD")
            {
                oAllot.TotalAmount = totalFeeBuyer;
            }

            oAllot.Posted = true;
            oAllot.Reversed = false;
            oAllot.UserId = strUserName;
            oAllot.Cdeal = char.Parse(oRowView["CrossD"].ToString());
            oAllot.Autopost = char.Parse("Y");
            oAllot.TicketNO = oRowView["BSlip#"].ToString();
            oAllot.SoldBy = oRowView["SoldBy"].ToString();
            oAllot.BoughtBy = oRowView["BoughtBy"].ToString();
            oAllot.Buy_sold_Ind = char.Parse("B");
            oAllot.CDSellTrans = "";
            oAllot.OtherCust = oRowView["CrossCustNo"].ToString();
            ;
            oAllot.MarginCode = "";
            if (oAllot.Cdeal == 'N')
            {
                oAllot.CrossType = "";
            }
            else
            {
                oAllot.CrossType = oRowView["CrossType"].ToString();
                if (oRowView["CrossType"].ToString().Trim() == "CD")
                {
                    oAllot.Consideration = 0;
                }
            }

            oAllot.PrintFlag = 'N';
            if (oRowView["CrossD"].ToString().Trim() == "Y" && (oRowView["CrossType"].ToString().Trim() == "NB" ||
                                                                oRowView["CrossType"].ToString().Trim() == "NS"))
            {
            }
            else
            {
                var dbCommandAllot = oAllot.AddCommand();
                db.ExecuteNonQuery(dbCommandAllot, transaction);
                strAllotmentNo = db.GetParameterValue(dbCommandAllot, "Txn").ToString().Trim();
            }

            return decAllotConsideration;
        }

        private static void SetStockInstrument(DataRow oRowView, StkParam oStkbPGenTable, ProductAcct oCustChkPropAccount,
            string strStockProductAccount)
        {
            string strBuyerTrueId;
            Stock oStockInstrument = new Stock();
            oStockInstrument.SecCode = oRowView["Stockcode"].ToString();
            oStkbPGenTable.StockInstrument = oStockInstrument.GetInstrumentTypeUsingStockCode();

            oCustChkPropAccount.ProductCode = strStockProductAccount;
            oCustChkPropAccount.CustAID = oRowView["CustNo"].ToString().Trim();
            oCustChkPropAccount.GetBoxLoadStatus();

            strBuyerTrueId = "";
        }

        private void UpdateErrorTradeTable(DateTime tradeDate, DataSet datDiskTrans)
        {
            var tradeFileGroupByStockPrices = datDiskTrans.Tables[0].AsEnumerable().Select(
                oRow => new Allotment.TradeFileGroupByStockPrice
                {
                    CustomerId = Convert.ToString(oRow["CustNo"]),
                    StockCode = Convert.ToString(oRow["StockCode"]),
                    EffectiveDate = Convert.ToDateTime(oRow["Date"]),
                    Buy_sold_Ind = Convert.ToChar(oRow["Buy_Sold_Ind"]),
                    TradeUnit = Convert.ToInt64(oRow["Units"]),
                    TradePrice = Convert.ToDecimal(oRow["UnitPrice"]),
                }).ToList();

            var oJobOrder = new JobOrder
            {
                EffectiveDate = tradeDate
            };
            var lstJobOrder = oJobOrder.GetAllUnProcessedByDate().Tables[0].AsEnumerable().Select(
                oRow => new JobOrder
                {
                    CustNo = Convert.ToString(oRow["CustNo"]),
                    StockCode = Convert.ToString(oRow["StockCode"]),
                    TxnType = Convert.ToChar(oRow["TxnType"]),
                    Units = Convert.ToInt32(oRow["Units"]),
                    UnitPrice = Convert.ToDecimal(oRow["Price"] != null && oRow["Price"].ToString().Trim() != "" ? oRow["Price"] : 0),
                }).ToList();
        }

        private ResponseResult HandleBarginSlip(DateTime date, string userName, string companyCode, out DataSet datDiskTrans)
        {
            var oBargainSlipPre = new BargainSlip();
            var oDsPre = oBargainSlipPre.GetAll();

            //Where You Call The Unique Transaction To Use In The Posting
            datDiskTrans = oDsPre;

            var tabPre = oDsPre.Tables[0];
            var rowPre = tabPre.Select();

            if ($"{rowPre[0]["Date"]:dd/MM/yyyy}" != $"{date:dd/MM/yyyy}")
                return ResponseResult.Error("Posting Aborted! Date On The Trade To Upload Not The Same with Date Selected");

            var oAutoDatePre = new AutoDate
            {
                iAutoDate = date
            };
            if (oAutoDatePre.GetAutoDate())
                return ResponseResult.Error("Posting Aborted! Trade For The Date Selected Already Posted");

            if (tabPre.Rows.Cast<DataRow>().Any(oRow => $"{oRow["Date"]:dd/MM/yyyy}" != $"{date:dd/MM/yyyy}"))
                return ResponseResult.Error("Posting Aborted! Unequal Date(s) In The Trade To Upload/Post");

            var portNotEnough = oBargainSlipPre.ChkPortfolioStockEnoughToSell(userName, tabPre);
            switch (portNotEnough)
            {
                case 1:
                    break;
                case 2:
                    return ResponseResult.Error("Posting Aborted! Error Deleting Portfolio Processing Table");
                case 3:
                    return ResponseResult.Error("Posting Aborted! Error Inserting To Portfolio Processing Table");
                default:
                    {
                        var oPortNot = new PortNot();
                        var filePath = Path.Combine(BaseFolder, companyCode, "PortfolioNotEnough.txt");
                        using (var writerPortNot = File.CreateText(filePath))
                        {
                            writerPortNot.WriteLine("Stock Not Enough In Customer Portfolio, Please Correct Before Uploading Your Daily Trade");
                            foreach (DataRow drPortNot in oPortNot.GetAll().Tables[0].Rows)
                            {
                                var builderPortNot = new StringBuilder(1000); // None row is bigger than this
                                foreach (var objPortNot in drPortNot.ItemArray)
                                {
                                    builderPortNot.Append(objPortNot.ToString());
                                    builderPortNot.Append("  ;  ");
                                }
                                builderPortNot.Remove(builderPortNot.Length - 1, 1);
                                writerPortNot.WriteLine(builderPortNot.ToString());
                            }
                            writerPortNot.Close();
                            writerPortNot.Dispose();
                        }

                        return ResponseResult.Error("Posting Aborted! Not Enough Stocks In The Portfolio To Sell, Check PortfolioNotEnough Text File And Correct");
                    }
            }
            return ResponseResult.Success();
        }

        private static ResponseResult HandleBrokerMissing(string companyCode, List<string> oBrokerMissings)
        {
            if (oBrokerMissings == null || oBrokerMissings.Count <= 0) return ResponseResult.Success();
            var filePath = GetMissingFilePath("BrokerMissing", companyCode);
            using (var writerBrokerMissing = File.CreateText(filePath))
            {
                writerBrokerMissing.WriteLine("Missing Brokerage Firms In GlobalSuite Data,Please Correct");
                foreach (var strRow in oBrokerMissings)
                {
                    var builderBrokerMiss = new StringBuilder(500); // None row is bigger than this
                    builderBrokerMiss.Append(strRow + Environment.NewLine);
                    //builderBrokerMiss.Remove(builderBrokerMiss.Length - 1, 1);
                    writerBrokerMissing.WriteLine(builderBrokerMiss.ToString());
                }
            }
            return ResponseResult.Error("Posting Aborted! Some StockBroking Firms Are Missing, Please Check Broker Missing Text File");
        }

        private static ResponseResult HandleStockMissing(string companyCode, List<string> oStockMissing)
        {
            if (oStockMissing == null || oStockMissing.Count <= 0) return ResponseResult.Success();
            var filePath = GetMissingFilePath("StockMissing", companyCode);
            using (var writerStockMissing = File.CreateText(filePath))
            {
                writerStockMissing.WriteLine("Missing Stock In GlobalSuite Database,Please Correct");
                foreach (var strRow in oStockMissing)
                {
                    var builderStockMiss = new StringBuilder(500); // None row is bigger than this
                    builderStockMiss.Append(strRow + Environment.NewLine);
                    //builderStockMiss.Remove(builderStockMiss.Length - 1, 1);
                    writerStockMissing.WriteLine(builderStockMiss.ToString());
                }
            }
            return ResponseResult.Error("Posting Aborted! Some Stocks Are Missing, Please Check StockMissing Text File");
        }

        private static ResponseResult HandleCustomerMissing(string companyCode, List<string> oCustomerMissing)
        {
            if (oCustomerMissing == null || oCustomerMissing.Count <= 0) return ResponseResult.Success();
          
            var filePath = GetMissingFilePath("AccountMissing",companyCode);
         
            using (var writer = File.CreateText(filePath))
            {
                writer.WriteLine("Missing CsCs Account In GlobalSuite Database,Please Correct");
                foreach (var strRow in oCustomerMissing)
                {
                    var builder = new StringBuilder(500); // None row is bigger than this
                    builder.Append(strRow + Environment.NewLine);
                    //builder.Remove(builder.Length - 1, 1);
                    writer.WriteLine(builder.ToString());
                }
            }
            return ResponseResult.Error("Posting Aborted! CsCs Account Numbers Are Missing, Check AccountMissing Text File");
        }

        internal static string GetMissingFilePath(string fileName, string companyCode, string ext=".txt")
        {
            var dir = Path.Combine(HttpRuntime.AppDomainAppPath,BaseFolder, companyCode);
            var dirExist = Directory.Exists(dir);
            if (!dirExist)
                Directory.CreateDirectory(dir);
            var filePath=Path.Combine(HttpRuntime.AppDomainAppPath,BaseFolder, companyCode, $"{fileName}{ext}");
            File.Create(filePath).Close();
            return filePath;
        }
    }
}
#endregion