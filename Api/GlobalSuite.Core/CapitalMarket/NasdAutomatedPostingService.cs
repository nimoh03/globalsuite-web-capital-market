using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<ResponseResult> NasdTradePosting(DateTime tradeDate, string filePath)
        {
            var oBranch = new Branch();
            return await Task.Run(() =>
            {
                #region Check Time Is Within For Today Posting
                var oGLParamTradeBookAddGloSuite = new GLParam
                {
                    Type = Constants.ParamTable.TRADEBOOKADDTOGLOBALSUITE
                };
                var strTradeBookAddGloSuite = oGLParamTradeBookAddGloSuite.CheckParameter();
                if (strTradeBookAddGloSuite.Trim() != "YES") return ResponseResult.Success();
                var intTimeHour = GeneralFunc.GetTodayTimeHour();
                if (intTimeHour >= 9 && intTimeHour < 15)
                    return ResponseResult.Error("Cannot Upload/Post Allotment Trades For The Day Yet! Time Within Market Trading Time of 9AM - 3PM");
                #endregion

                try
                {
                    #region Declaration of Property

                    var strAllotmentNo = "";
                    var strJnumberNext = "";
                    decimal decAllotConsideration = 0;
                    var strUserName = GeneralFunc.UserName;
                    var oCompany = new Company();
                    var companyCode = oCompany.MemberCode;
                    var oFile = new FileHandlerNASD();
                    var oCustMissings = new List<string>();
                    var oStockMissings = new List<string>();
                    var oBrokerMissings = new List<string>();
                    var strDefaultBranchCode = oBranch.DefaultBranch;

                    #endregion
                    ResponseResult result;
                    var oBargainSlipNasdPre = new BargainSlipNASD();
                    #region Loading from Trade File to Database

                    var blnFileTradeResult = oFile.ReadTextFile(filePath, out oCustMissings, out oStockMissings,
                        out oBrokerMissings);
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

                    #region Start Checking After Loading From Trade File
                     result=HandleBargainSlipNasd(oBargainSlipNasdPre, tradeDate, strUserName, companyCode);
                     if (!result.IsSuccess) return result;
                    #endregion

                    //Start Processing Trade
                    //Where You Call The Unique Transaction To Use In The Posting
                    var datDiskTrans = oBargainSlipNasdPre.GetAll();
                    #region Declaration And Assisning Of Product Account,Customer,Fee and Allot Variable, Cap Market and pParam Parameter And Others

                    GetHandleDeclarationAssignment(out var oCust, out var oCustomer,
                        out var oCustCross, out var oCustomerCross, out var decFeeSec,
                        out var decFeeStampDuty,
                        out var decFeeCommission, out var decFeeVAT, out var decFeeCSCS,
                        out var decSMSAlertCSCS,
                        out var decFeeNSE, out var decFeeSecVat, out var decFeeNseVat,
                        out var decFeeCscsVat,
                        out var decSMSAlertCSCSVAT, out var decFeeTotalAmount,
                        out var decAllotConsiderationBuy,
                        out var decFeeSecSeller, out var decFeeStampDutySeller,
                        out var decFeeCommissionSeller,
                        out var decFeeVATSeller, out var decFeeCSCSSeller, out var decSMSAlertCSCSSeller,
                        out var decFeeNSESeller, out var decFeeSecVatSeller, out var decFeeNseVatSeller,
                        out var decFeeCscsVatSeller, out var decSMSAlertCSCSVATSeller,
                        out var TotalFeeBuyer,
                        out var TotalFeeSeller, out var TotalFeeSellerPost, out var strAllotmentNo2,
                        out var oStkbPGenTable,
                        out var oPGenTable, out var decDeductComm, out var strBuyerTrueId,
                        out var strPostCommShared, out var strStockProductAccount,
                        out var strNASDStockProductAccount,
                        out var strInvestmentNASDProductAccount, out var strAgentProductAccount,
                        out var strBranchBuySellCharge, out var decTotalBank, out var decTotalBankAudit,
                        out var decTotalSEC,
                        out var decTotalCSCS, out var decTotalNSE, out var decTotalCommission,
                        out var decTotalCommissionBuy,
                        out var decTotalCommissionSell, out var decTotalVAT, out var decTotalStampDuty,
                        out var decTotalDeductCommission, out var decTotalDeductCommissionBuy,
                        out var decTotalDeductCommissionSell,
                        out var decCrossDealTotalAmount, out var decTotalDirectCashSettlement,
                        out var decTotalDirectCashSettlementBond, out var strMarginCodeForNASDIndicator,
                        out var oGLParamBoxSaleProfit, out var oGLParamBranchBuySellCharge);

                    #endregion

                    
                    var oCustomerExtraInformationDirectCash = new CustomerExtraInformation();
                    var tabDiskTrans = datDiskTrans.Tables[0];
                     var RecNumber = decimal.Parse(tabDiskTrans.Rows.Count.ToString());
                    var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                    using (var connection = db.CreateConnection() as SqlConnection)
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            foreach (DataRow oRowView in tabDiskTrans.Rows)
                            {
                                #region Processing Allotments

                                   ProcessAllotmentNasd(oRowView,oStkbPGenTable, strUserName, strMarginCodeForNASDIndicator, db, 
                                   transaction, oCustomerExtraInformationDirectCash, out var oAllot, out decAllotConsideration, ref decTotalBank, 
                                   out decAllotConsiderationBuy, ref decTotalBankAudit, out decFeeSec, ref decTotalSEC, out decFeeStampDuty, ref decTotalStampDuty, out decFeeCommission, ref decTotalCommission, ref decTotalCommissionBuy, out decFeeVAT, ref decTotalVAT, out decFeeNSE, ref decTotalNSE, out decFeeCSCS, ref decTotalCSCS, out decSMSAlertCSCS, out decFeeSecVat, out decFeeNseVat, out decFeeCscsVat, out decSMSAlertCSCSVAT, out decFeeTotalAmount, out decCrossDealTotalAmount, out TotalFeeBuyer, ref strAllotmentNo, out strBuyerTrueId, ref decTotalCommissionSell, out TotalFeeSeller, ref decTotalDirectCashSettlement, ref decTotalDirectCashSettlementBond, out strAllotmentNo2, out decFeeSecSeller, out decFeeStampDutySeller, out decFeeCommissionSeller, out decFeeVATSeller, out decFeeNSESeller, out decFeeCSCSSeller, out decSMSAlertCSCSSeller, out decFeeSecVatSeller, out decFeeNseVatSeller, out decFeeCscsVatSeller, out decSMSAlertCSCSVATSeller, out TotalFeeSellerPost);

                                #endregion
                                
                                #region GL Posting For Ordinary And Cross Deal Allotments
                                 PostGLForOrdinaryAndCrossBuyAllotmentNasd(db, transaction, oRowView, strBuyerTrueId, oCust, oGLParamBoxSaleProfit,
                                     strNASDStockProductAccount, oCustomer, oCustCross, oAllot, oCustomerCross, strInvestmentNASDProductAccount,
                                     decFeeTotalAmount, strAllotmentNo, strDefaultBranchCode, strMarginCodeForNASDIndicator, oPGenTable,
                                     oCustomerExtraInformationDirectCash, TotalFeeBuyer, strStockProductAccount, strAgentProductAccount,
                                     strPostCommShared, decFeeCommission, ref decTotalDeductCommissionBuy, strBranchBuySellCharge, out var oCommandJnumber,
                                     out strJnumberNext, out var decGetUnitCost, out var decActualSellPrice, out var decSellDiff, 
                                     out var decSellDiffTotal,out var oGL, out var oProductAcctAgent, out decDeductComm, 
                                     ref decTotalDeductCommissionSell, ref decTotalDeductCommission);
                                 #endregion
                                 
                                 //This does not solve Cross Type "NB" because it will post 2 transactions, one at the top for purchase and the second below
                                 //for sale whereas it should only for one at the top or below, preferably at the top or bottom because all the parameter
                                 //are there for bottom unless the total amount takes care of it at the top
                            #region GL Posting For 1.Ordinary Cross Deal Sell 2.Norminal Cross Deal Sell 3.NB Cross Deal 4.NS Cross Deal 5.Buy Cross Deal With Consideration Sell 6.Sell Cross Deal With Consideration Sell
                                GlPostingOrdinaryCrossDealSellNasd(oAllot, oGL, oRowView, oCust, oCustomer, strInvestmentNASDProductAccount, strNASDStockProductAccount, TotalFeeSeller, TotalFeeSellerPost, TotalFeeBuyer, oCustomerCross, strAllotmentNo2, strAllotmentNo, strJnumberNext, strDefaultBranchCode, strMarginCodeForNASDIndicator, oPGenTable, db, transaction, oCustomerExtraInformationDirectCash, strStockProductAccount, oGLParamBoxSaleProfit, strAgentProductAccount, strPostCommShared, decFeeCommissionSeller, ref decTotalDeductCommission, out decGetUnitCost, out decActualSellPrice, out decSellDiff, out decSellDiffTotal, out decDeductComm, ref decTotalDeductCommissionSell);
                                #endregion
 #region Portfolio Processing
                                PortfolioPostingNasd(oRowView, oAllot, decCrossDealTotalAmount, strAllotmentNo, strMarginCodeForNASDIndicator, db, transaction, strAllotmentNo2, out var oPort, out var dbCommandPort);

                                #endregion
 #region Job Order Processing
                                JobOrderProcessingNasd(oRowView, oAllot, db, transaction, strAllotmentNo, out var oJob, out var oDsJob, out var thisTableJob, out var iLeft);

                                #endregion

                               
                                
                            }
                            
                             #region Summation Of Fees And Control Account Posting

                             SummationOfFeesAndControlAccountPosting(tradeDate, decTotalCommission, oPGenTable, decTotalCommissionBuy, decTotalDeductCommissionBuy, strMarginCodeForNASDIndicator, strJnumberNext, strDefaultBranchCode, db, transaction, decTotalCommissionSell, decTotalDeductCommissionSell, decTotalDeductCommission, decTotalVAT, decTotalSEC, decTotalNSE, decTotalCSCS, decTotalStampDuty, decTotalBank, decTotalDirectCashSettlement, strBranchBuySellCharge, out var oGLTradBank);

                             #endregion
                             
                             #region End Processing
                             var oAutoDateNASD = new AutoDateNASD
                             {
                                 iAutoDate = tradeDate.ToExact()
                             };
                             var dbCommandAutoDate = oAutoDateNASD.AddCommand();
                             db.ExecuteNonQuery(dbCommandAutoDate, transaction);

                             transaction.Commit();
                             return ResponseResult.Success("NASD Allotment/Trade Postings Successfull");
                             #endregion
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message, ex);
                            transaction?.Rollback();
                            return ResponseResult.Error("Error In Posting " + ex.Message.Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error("Error In Posting " + ex.Message.Trim());
                }

                return ResponseResult.Success();
            });
        }

        private static void SummationOfFeesAndControlAccountPosting(DateTime tradeDate, decimal decTotalCommission, PGenTable oPGenTable,
            decimal decTotalCommissionBuy, decimal decTotalDeductCommissionBuy, string strMarginCodeForNASDIndicator,
            string strJnumberNext, string strDefaultBranchCode, SqlDatabase db, SqlTransaction transaction,
            decimal decTotalCommissionSell, decimal decTotalDeductCommissionSell, decimal decTotalDeductCommission,
            decimal decTotalVAT, decimal decTotalSEC, decimal decTotalNSE, decimal decTotalCSCS, decimal decTotalStampDuty,
            decimal decTotalBank, decimal decTotalDirectCashSettlement, string strBranchBuySellCharge, out AcctGL oGLTradBank)
        {
            oGLTradBank = new AcctGL();
            if (decTotalCommission != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBComm;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "Commission Total Buy Charges For: " +
                                         tradeDate.ToExact();
                var decNetCommissionBuy = decTotalCommissionBuy - decTotalDeductCommissionBuy;
                oGLTradBank.Credit = Math.Round(decNetCommissionBuy, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TCOMCB";
                var dbCommandCommissionTotalBuyCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandCommissionTotalBuyCredit, transaction);

                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDSComm;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "Commission Total Sell Charges For: " +
                                         tradeDate.ToExact();
                var decNetCommissionSell = decTotalCommissionSell - decTotalDeductCommissionSell;
                oGLTradBank.Credit = Math.Round(decNetCommissionSell, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TCOMCS";
                var dbCommandCommissionTotalSellCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandCommissionTotalSellCredit, transaction);

                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.ShInv;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.NASDBComm;
                oGLTradBank.Desciption = "Commission Total Charges For: " +
                                         tradeDate.ToExact();
                var decNetCommission = decTotalCommission - decTotalDeductCommission;
                oGLTradBank.Debit = Math.Round(decNetCommission, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSTCM";
                var dbCommandCommissionTotalCreditJobbing = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandCommissionTotalCreditJobbing, transaction);
            }

            //if (decTotalStampDuty != 0)
            //{
            //    oGLTradBank.EffectiveDate = tradeDate.ToExact();
            //    oGLTradBank.MasterID = oPGenTable.NASDBStamp;
            //    oGLTradBank.AccountID = "";
            //    oGLTradBank.RecAcct = "";
            //    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
            //    oGLTradBank.Desciption = "Stamp Duty Total Charges For: " + tradeDate.ToExact();
            //    oGLTradBank.Credit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
            //    oGLTradBank.Debit = 0;
            //    oGLTradBank.Debcred = "C";
            //    oGLTradBank.SysRef = "TRB" + "-" + tradeDate.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Year.ToString().PadLeft(4, char.Parse("0")) + strMarginCodeForNASDIndicator;
            //    oGLTradBank.TransType = "TRADBANK";
            //    oGLTradBank.Ref01 = tradeDate.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
            //    oGLTradBank.Reverse = "N";
            //    oGLTradBank.Jnumber = strJnumberNext;
            //    oGLTradBank.Branch = strDefaultBranchCode;
            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = strMarginCodeForNASDIndicator; oGLTradBank.Chqno = "";
            //    oGLTradBank.FeeType = "TSTDC";
            //    SqlCommand dbCommandStampTotalCredit = oGLTradBank.AddCommand();
            //    db.ExecuteNonQuery(dbCommandStampTotalCredit, transaction);


            //    oGLTradBank.EffectiveDate = tradeDate.ToExact();
            //    oGLTradBank.MasterID = oPGenTable.ShInv;
            //    oGLTradBank.AccountID = "";
            //    oGLTradBank.RecAcct = "";
            //    oGLTradBank.RecAcctMaster = oPGenTable.Bstamp;
            //    oGLTradBank.Desciption = "Stamp Duty Total Charges For: " + tradeDate.ToExact();
            //    oGLTradBank.Debit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
            //    oGLTradBank.Credit = 0;
            //    oGLTradBank.Debcred = "D";
            //    oGLTradBank.SysRef = "TRB" + "-" + tradeDate.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Year.ToString().PadLeft(4, char.Parse("0")) + strMarginCodeForNASDIndicator;
            //    oGLTradBank.TransType = "TRADBANK";
            //    oGLTradBank.Ref01 = tradeDate.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
            //    oGLTradBank.Reverse = "N";
            //    oGLTradBank.Jnumber = strJnumberNext;
            //    oGLTradBank.Branch = strDefaultBranchCode;
            //    oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = strMarginCodeForNASDIndicator; oGLTradBank.Chqno = "";
            //    oGLTradBank.FeeType = "TSTCS";
            //    SqlCommand dbCommandStampTotalCreditJobbing = oGLTradBank.AddCommand();
            //    db.ExecuteNonQuery(dbCommandStampTotalCreditJobbing, transaction);
            //}

            if (decTotalVAT != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBVAT;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "VAT Total Charges For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TVATC";
                var dbCommandVATTotalCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandVATTotalCredit, transaction);

                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.ShInv;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.NASDBVAT;
                oGLTradBank.Desciption = "VAT Total Charges For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Debit = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSTVT";
                var dbCommandVATTotalCreditJobbing = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandVATTotalCreditJobbing, transaction);
            }

            if (decTotalSEC != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBSec;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "SEC Total Charges For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSECC";
                var dbCommandSecTotalCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandSecTotalCredit, transaction);
            }

            if (decTotalNSE != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBNASD;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "NSE Total Charges For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TNSEC";
                var dbCommandNseTotalCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandNseTotalCredit, transaction);
            }

            if (decTotalCSCS != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBCSCS;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "CSCS Total Charges For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TCSCC";
                var dbCommandCSCSTotalCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandCSCSTotalCredit, transaction);
            }

            if (decTotalStampDuty != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBStamp;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "Stamp Duty Total Charges For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debit = 0;
                oGLTradBank.Debcred = "C";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSTDYC";
                var dbCommandStampDutyTotalCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStampDutyTotalCredit, transaction);
            }


            if (decTotalBank != 0 || decTotalNSE != 0 || decTotalCSCS != 0 || decTotalSEC != 0 || decTotalStampDuty != 0 ||
                decTotalDirectCashSettlement != 0)
            {
                //Debit And Credit SEC Account And Charges Control
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBSec;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "SEC Charges Payment For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = 0;
                oGLTradBank.Debit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "PSECD";
                var dbCommandSecStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandSecStatControlDebit, transaction);


                //Debit And Credit NSE Account And Charges Control
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBNASD;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "NSE Charges Payment For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = 0;
                oGLTradBank.Debit = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "PNSED";
                var dbCommandNseStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandNseStatControlDebit, transaction);

                //Debit And Credit CSCS Account And Charges Control
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBCSCS;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "CSCS Charges Payment For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = 0;
                oGLTradBank.Debit = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "PCSCD";
                var dbCommandCSCSStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandCSCSStatControlDebit, transaction);

                //Debit And Credit Stamp Duty Account And Charges Control
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.NASDBStamp;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "Stamp Duty Charges Payment For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Credit = 0;
                oGLTradBank.Debit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "PCSCD";
                var dbCommandStampDutyStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStampDutyStatControlDebit, transaction);


                //Trading And Control Account Posting
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                if (decTotalBank < 0)
                {
                    //oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.MasterID = oPGenTable.NASDTradingBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase For: " +$"{tradeDate.ToExact()}";
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
                    //oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.RecAcctMaster = oPGenTable.NASDTradingBank;
                    oGLTradBank.Desciption = "Net Sale Trading For: " +$"{tradeDate.ToExact()}";
                    oGLTradBank.Credit = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero) -
                                         (Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                          Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero));
                    oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                }
                else if (decTotalBank == 0)
                {
                    //oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.MasterID = oPGenTable.NASDTradingBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Purchase For: " +$"{tradeDate.ToExact()}";
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
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSTD";
                var dbCommandStatControlCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlCredit, transaction);

                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                if (decTotalBank > 0)
                {
                    //oGLTradBank.MasterID = oPGenTable.TradeBank;
                    oGLTradBank.MasterID = oPGenTable.NASDTradingBank;
                    oGLTradBank.AccountID = "";
                    oGLTradBank.RecAcct = "";
                    oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                    oGLTradBank.Desciption = "Net Sale For: " +
                                             $"{tradeDate.ToExact()}";
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
                    //oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.RecAcctMaster = oPGenTable.NASDTradingBank;
                    oGLTradBank.Desciption = "Net Purchase Trading For: " +
                                             $"{tradeDate.ToExact()}";
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
                    //oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                    oGLTradBank.RecAcctMaster = oPGenTable.NASDTradingBank;
                    oGLTradBank.Desciption = "Net Purchase Trading For: " +
                                             $"{tradeDate.ToExact()}";
                    oGLTradBank.Debit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) +
                                        Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                    oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                }

                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     GetUnformattedDate(tradeDate) + strMarginCodeForNASDIndicator;
                oGLTradBank.TransType = "TRADBANK";
                oGLTradBank.Ref01 =
                    GetUnformattedDate(tradeDate);
                oGLTradBank.Reverse = "N";
                oGLTradBank.Jnumber = strJnumberNext;
                oGLTradBank.Branch = strDefaultBranchCode;
                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGLTradBank.AcctRef = "";
                oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                oGLTradBank.Chqno = "";
                oGLTradBank.FeeType = "TSTD";
                var dbCommandStatControlDebit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandStatControlDebit, transaction);
            }

            #region Posting Direct Cash Settlement Total Amount To Direct Cash Settlement Account

            if (decTotalDirectCashSettlement != 0)
            {
                oGLTradBank.EffectiveDate = tradeDate.ToExact();
                oGLTradBank.MasterID = oPGenTable.DirectCashSettleAcct;
                oGLTradBank.AccountID = "";
                oGLTradBank.RecAcct = "";
                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                oGLTradBank.Desciption = "Direct Settlemet Net Trading For: " +
                                         tradeDate.ToExact();
                oGLTradBank.Debit = Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                oGLTradBank.Credit = 0;
                oGLTradBank.Debcred = "D";
                oGLTradBank.SysRef = "TRB" + "-" +
                                     tradeDate.ToExact().Day.ToString()
                                         .PadLeft(2, char.Parse("0")) +
                                     tradeDate.ToExact().Month.ToString()
                                         .PadLeft(2, char.Parse("0")) + tradeDate.ToExact()
                                         .Year.ToString().PadLeft(4, char.Parse("0"));
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
                var dbCommandDirectCashSettleCredit = oGLTradBank.AddCommand();
                db.ExecuteNonQuery(dbCommandDirectCashSettleCredit, transaction);
            }

            #endregion

            if (strBranchBuySellCharge.Trim() == "YES")
            {
                decimal decNetBuyAndSellUploadPerBranch = 0;
                var oAllotmentBranchPosted = new Allotment
                {
                    DateAlloted = tradeDate.ToExact()
                };
                var oBranchPostNetTrade = new Branch();
                var oBranchSelectAccount = new Branch();
                foreach (DataRow oRowBranch in oBranchPostNetTrade.GetAllExcludeDefault().Tables[0].Rows)
                {
                    decNetBuyAndSellUploadPerBranch = oAllotmentBranchPosted.GetNetPurchaseSaleAllotmentByDateAndBranchForUpload
                        (tradeDate.ToExact(), oRowBranch["Brancode"].ToString().Trim());
                    if (decNetBuyAndSellUploadPerBranch != 0)
                    {
                        oBranchSelectAccount.TransNo = oRowBranch["Brancode"].ToString().Trim();
                        oBranchSelectAccount.GetBranch();

                        oGLTradBank.EffectiveDate = tradeDate.ToExact();
                        oGLTradBank.MasterID = oBranchSelectAccount.Trading;
                        oGLTradBank.AccountID = "";
                        oGLTradBank.RecAcct = "";
                        oGLTradBank.RecAcctMaster = oBranchSelectAccount.Commission;
                        if (decNetBuyAndSellUploadPerBranch < 0)
                        {
                            oGLTradBank.Credit =
                                Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.Desciption = "Net Purchase For: " + $"{tradeDate.ToExact()}";
                        }
                        else if (decNetBuyAndSellUploadPerBranch > 0)
                        {
                            oGLTradBank.Credit = 0;
                            oGLTradBank.Debit =
                                Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                            oGLTradBank.Debcred = "D";
                            oGLTradBank.Desciption = "Net Sale For: " + $"{tradeDate.ToExact()}";
                        }

                        oGLTradBank.SysRef = "TRB" + "-" +
                                             tradeDate.ToExact().Day.ToString()
                                                 .PadLeft(2, char.Parse("0")) +
                                             tradeDate.ToExact().Month.ToString()
                                                 .PadLeft(2, char.Parse("0")) +
                                             tradeDate.ToExact().Year.ToString()
                                                 .PadLeft(4, char.Parse("0")) + strMarginCodeForNASDIndicator;
                        oGLTradBank.TransType = "TRADBANK";
                        oGLTradBank.Ref01 =
                            tradeDate.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) +
                            tradeDate.ToExact().Month.ToString()
                                .PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Year
                                .ToString().PadLeft(4, char.Parse("0"));
                        oGLTradBank.Reverse = "N";
                        oGLTradBank.Jnumber = strJnumberNext;
                        oGLTradBank.Branch = strDefaultBranchCode;
                        oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                        oGLTradBank.AcctRef = "";
                        oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                        oGLTradBank.Chqno = "";
                        oGLTradBank.FeeType = "BTRAB";
                        oGLTradBank.PostToOtherBranch = "Y";
                        var dbCommandBranchNetTradeDefault = oGLTradBank.AddCommand();
                        db.ExecuteNonQuery(dbCommandBranchNetTradeDefault, transaction);

                        oGLTradBank.EffectiveDate = tradeDate.ToExact();
                        oGLTradBank.MasterID = oBranchSelectAccount.Commission;
                        oGLTradBank.AccountID = "";
                        oGLTradBank.RecAcct = "";
                        oGLTradBank.RecAcctMaster = oBranchSelectAccount.Trading;
                        if (decNetBuyAndSellUploadPerBranch < 0)
                        {
                            oGLTradBank.Credit = 0;
                            oGLTradBank.Debit =
                                Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                            oGLTradBank.Debcred = "D";
                            oGLTradBank.Desciption = "Net Purchase For: " + $"{tradeDate.ToExact()}";
                        }
                        else if (decNetBuyAndSellUploadPerBranch > 0)
                        {
                            oGLTradBank.Credit =
                                Math.Abs(Math.Round(decNetBuyAndSellUploadPerBranch, 2, MidpointRounding.AwayFromZero));
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.Desciption = "Net Sale For: " + $"{tradeDate.ToExact()}";
                        }

                        oGLTradBank.SysRef = "TRB" + "-" +
                                             tradeDate.ToExact().Day.ToString()
                                                 .PadLeft(2, char.Parse("0")) +
                                             tradeDate.ToExact().Month.ToString()
                                                 .PadLeft(2, char.Parse("0")) +
                                             tradeDate.ToExact().Year.ToString()
                                                 .PadLeft(4, char.Parse("0")) + strMarginCodeForNASDIndicator;
                        oGLTradBank.TransType = "TRADBANK";
                        oGLTradBank.Ref01 =
                            tradeDate.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) +
                            tradeDate.ToExact().Month.ToString()
                                .PadLeft(2, char.Parse("0")) + tradeDate.ToExact().Year
                                .ToString().PadLeft(4, char.Parse("0"));
                        oGLTradBank.Reverse = "N";
                        oGLTradBank.Jnumber = strJnumberNext;
                        oGLTradBank.Branch = strDefaultBranchCode;
                        oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C;
                        oGLTradBank.AcctRef = "";
                        oGLTradBank.Ref02 = strMarginCodeForNASDIndicator;
                        oGLTradBank.Chqno = "";
                        oGLTradBank.FeeType = "BTRAD";
                        var dbCommandBranchNetTradeBranch = oGLTradBank.AddCommand();
                        db.ExecuteNonQuery(dbCommandBranchNetTradeBranch, transaction);
                        oGLTradBank.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege
                    }
                }
            }
        }

        private static void JobOrderProcessingNasd(DataRow oRowView, Allotment oAllot, SqlDatabase db,
            SqlTransaction transaction, string strAllotmentNo, out JobOrder oJob, out DataSet oDsJob,
            out DataTable thisTableJob, out int iLeft)
        {
            oJob = new JobOrder();
            oDsJob = new DataSet();
            oJob.CustNo = oRowView["CustNo"].ToString().Trim();
            oJob.StockCode = oRowView["StockCode"].ToString().Trim();
            oJob.CustNo_CD = oRowView["CrossCustNo"].ToString().Trim();
            if (oAllot.Cdeal == 'Y')
            {
                if (oRowView["CrossType"].ToString().Trim() == "NO")
                {
                    oDsJob = oJob.GetUnProcBuyGivenCustStock();
                }
                else
                {
                    oDsJob = oJob.GetUnProcCrossGivenCustStock();
                }
            }
            else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
            {
                oDsJob = oJob.GetUnProcSellGivenCustStock();
            }
            else
            {
                oDsJob = oJob.GetUnProcBuyGivenCustStock();
            }

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

            if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "NO"))
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

        private static void PortfolioPostingNasd(DataRow oRowView, Allotment oAllot, decimal decCrossDealTotalAmount,
            string strAllotmentNo, string strMarginCodeForNASDIndicator, SqlDatabase db, SqlTransaction transaction,
            string strAllotmentNo2, out Portfolio oPort, out SqlCommand dbCommandPort)
        {
            oPort = new Portfolio
            {
                PurchaseDate = oRowView["Date"].ToString().Trim().ToDate(),
                CustomerAcct = oRowView["CustNo"].ToString(),
                StockCode = oRowView["Stockcode"].ToString().Trim(),
                Units = long.Parse(oRowView["Units"].ToString().Trim()),
                UnitPrice = float.Parse(oRowView["UnitPrice"].ToString().Trim())
            };
            if (oAllot.Cdeal == 'N')
            {
                oPort.ActualUnitCost =
                    float.Parse(oAllot.TotalAmount.ToString()) / long.Parse(oRowView["Units"].ToString().Trim());
                oPort.TotalCost = oAllot.TotalAmount;
            }
            else
            {
                oPort.ActualUnitCost = float.Parse(decCrossDealTotalAmount.ToString()) /
                                       long.Parse(oRowView["Units"].ToString().Trim());
                oPort.TotalCost = decCrossDealTotalAmount;
            }

            oPort.Ref01 = strAllotmentNo;
            if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
            {
                oPort.DebCred = "C";
                if (oAllot.Cdeal == 'N')
                {
                    oPort.TransDesc = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                      oRowView["Stockcode"].ToString().Trim() + " @ " +
                                      decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim();
                }
                else
                {
                    oPort.TransDesc = "Cross Deal Pur: " + oRowView["Units"].ToString().Trim() + " " +
                                      oRowView["Stockcode"].ToString().Trim() + " @ " +
                                      decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim();
                }

                oPort.TransType = "STKBSALE";
                oPort.SysRef = "STKB-" + strAllotmentNo;
            }
            else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
            {
                oPort.DebCred = "D";
                oPort.TransDesc = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                  oRowView["Stockcode"].ToString().Trim() + " @ " +
                                  decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim();
                oPort.TransType = "COLTSALE";
                oPort.SysRef = "COLT-" + strAllotmentNo;
            }

            oPort.MarginCode = strMarginCodeForNASDIndicator;
            dbCommandPort = oPort.AddCommand();
            db.ExecuteNonQuery(dbCommandPort, transaction);

            if (oAllot.Cdeal == 'Y')
            {
                oPort.PurchaseDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oPort.CustomerAcct = oRowView["CrossCustNo"].ToString();
                oPort.StockCode = oRowView["Stockcode"].ToString().Trim();
                oPort.Units = long.Parse(oRowView["Units"].ToString().Trim());
                oPort.UnitPrice = float.Parse(oRowView["UnitPrice"].ToString().Trim());
                oPort.ActualUnitCost =
                    float.Parse(oAllot.TotalAmount.ToString()) / long.Parse(oRowView["Units"].ToString().Trim());
                oPort.TotalCost = oAllot.TotalAmount;
                oPort.Ref01 = strAllotmentNo2;
                oPort.DebCred = "D";
                oPort.TransDesc = "Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                  oRowView["Stockcode"].ToString().Trim() + " @ " +
                                  decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim();
                oPort.TransType = "COLTSALE";
                oPort.SysRef = "STKB-" + strAllotmentNo;
                oPort.MarginCode = strMarginCodeForNASDIndicator;
                var dbCommandPort2 = oPort.AddCommand();
                db.ExecuteNonQuery(dbCommandPort2, transaction);
            }
        }

        private static void GlPostingOrdinaryCrossDealSellNasd(Allotment oAllot, AcctGL oGL, DataRow oRowView, ProductAcct oCust, Customer oCustomer,
            string strInvestmentNASDProductAccount, string strNASDStockProductAccount, decimal TotalFeeSeller,
            decimal TotalFeeSellerPost, decimal TotalFeeBuyer, Customer oCustomerCross, string strAllotmentNo2,
            string strAllotmentNo, string strJnumberNext, string strDefaultBranchCode, string strMarginCodeForNASDIndicator,
            PGenTable oPGenTable, SqlDatabase db, SqlTransaction transaction,
            CustomerExtraInformation oCustomerExtraInformationDirectCash, string strStockProductAccount,
            GLParam oGLParamBoxSaleProfit, string strAgentProductAccount, string strPostCommShared,
            decimal decFeeCommissionSeller, ref decimal decTotalDeductCommission, out decimal decGetUnitCost,
            out decimal decActualSellPrice, out decimal decSellDiff, out decimal decSellDiffTotal, out decimal decDeductComm,
            ref decimal decTotalDeductCommissionSell)
        {
            decGetUnitCost = 0;
            decActualSellPrice = 0;
            decSellDiff = 0;
            decSellDiffTotal = 0;
            decDeductComm = 0;
            switch (oAllot.Cdeal)
            {
                case 'Y':
                {
                    #region Customer Posting Cross Deal

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
                        oProduct.TransNo = strInvestmentNASDProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentNASDProductAccount;
                    }
                    else
                    {
                        oProduct.TransNo = strNASDStockProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strNASDStockProductAccount;
                    }

                    if (!oCustomer.GetCustomerName(strNASDStockProductAccount))
                    {
                        throw new Exception("Missing Customer Account");
                    }

                    var oPortForSaleProfit = new Portfolio
                    {
                        PurchaseDate = oRowView["Date"].ToString().Trim().ToDate(),
                        StockCode = oRowView["Stockcode"].ToString().Trim(),
                        CustomerAcct = oRowView["CrossCustNo"].ToString().Trim()
                    };
                    decGetUnitCost = decimal.Parse(oPortForSaleProfit.GetUnitCost().ToString());
                    decGetUnitCost = Math.Round(decGetUnitCost, 2, MidpointRounding.AwayFromZero);
                    decActualSellPrice = oAllot.TotalAmount / decimal.Parse(oRowView["Units"].ToString().Trim());
                    decActualSellPrice = Math.Round(decActualSellPrice, 2, MidpointRounding.AwayFromZero);
                    decSellDiff = decActualSellPrice - decGetUnitCost;
                    decSellDiff = Math.Round(decSellDiff, 2, MidpointRounding.AwayFromZero);
                    decSellDiffTotal = decSellDiff * decimal.Parse(oRowView["Units"].ToString().Trim());
                    decSellDiffTotal = Math.Round(decSellDiffTotal, 2, MidpointRounding.AwayFromZero);
                    if ((oRowView["CrossType"].ToString().Trim() == "NO") || (oRowView["CrossType"].ToString().Trim() == "NA")
                                                                          || (oRowView["CrossType"].ToString().Trim() == "NT"))
                    {
                        oGL.Credit = oAllot.TotalAmount;
                        oGL.Debit = 0;
                        oGL.Debcred = "C";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                    }
                    else if (oRowView["CrossType"].ToString().Trim() == "CD")
                    {
                        oGL.Debit = TotalFeeSeller;
                        oGL.Credit = 0;
                        oGL.Debcred = "D";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    }
                    else if (oRowView["CrossType"].ToString().Trim() == "NB" || oRowView["CrossType"].ToString().Trim() == "NS")
                    {
                        oGL.Debit = TotalFeeSellerPost + TotalFeeBuyer;
                        oGL.Credit = 0;
                        oGL.Debcred = "D";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    }

                    oGL.FeeType = "SCUM";
                    if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                    {
                        oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString());
                    }
                    else if ((oRowView["CrossType"].ToString().Trim() == "CD"))
                    {
                        oGL.Desciption = "Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString());
                    }
                    else if ((oRowView["CrossType"].ToString().Trim() == "NB"))
                    {
                        oGL.Desciption = "Cross Buyer Only: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString());
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
                    oGL.Reverse = "N";
                    oGL.Jnumber = strJnumberNext;
                    oGL.Branch = strDefaultBranchCode;
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
                    oGL.Chqno = "";
                    oGL.RecAcctMaster = oPGenTable.ShInv;
                    oGL.RecAcct = "";
                    oGL.PostToOtherBranch = "Y";
                    var dbCommandCustomer = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandCustomer, transaction);

                    #endregion

                    oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege

                    #region Second Leg Of Customer To Jobbing

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.MasterID = oPGenTable.ShInv;
                    oGL.AccountID = "";
                    oGL.RecAcct = "";
                    oGL.RecAcctMaster = oPGenTable.ShInv;
                    if ((oRowView["CrossType"].ToString().Trim() == "NO") || (oRowView["CrossType"].ToString().Trim() == "NA")
                                                                          || (oRowView["CrossType"].ToString().Trim() == "NT"))
                    {
                        oGL.Debit = oAllot.TotalAmount;
                        oGL.Credit = 0;
                        oGL.Debcred = "D";
                    }
                    else if (oRowView["CrossType"].ToString().Trim() == "CD")
                    {
                        oGL.Credit = TotalFeeSeller;
                        oGL.Debit = 0;
                        oGL.Debcred = "C";
                    }
                    else if (oRowView["CrossType"].ToString().Trim() == "NB" || oRowView["CrossType"].ToString().Trim() == "NS")
                    {
                        oGL.Credit = TotalFeeSellerPost + TotalFeeBuyer;
                        oGL.Debit = 0;
                        oGL.Debcred = "C";
                    }

                    if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                    {
                        oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()) + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else if ((oRowView["CrossType"].ToString().Trim() == "CD"))
                    {
                        oGL.Desciption = "Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()) + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else if ((oRowView["CrossType"].ToString().Trim() == "NB"))
                    {
                        oGL.Desciption = "Cross Buyer Only: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()) + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
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
                        oGL.Ref02 = strMarginCodeForNASDIndicator;
                    }
                    else
                    {
                        oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                            oProduct.TransNo = strInvestmentNASDProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentNASDProductAccount;
                        }
                        else
                        {
                            oProduct.TransNo = strNASDStockProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strNASDStockProductAccount;
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
                        oGL.RecAcct = "";
                        oGL.RecAcctMaster = oPGenTable.ShInv;
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

                    if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                    {
                        #region Box Load Profit Loss For Sale

                        if ((oCust.GetBoxLoadStatus()) && (decSellDiffTotal != 0) &&
                            (oGLParamBoxSaleProfit.CheckParameter() == "YES"))
                        {
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
                                                 " SP @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() +
                                                 " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                            }
                            else
                            {
                                oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                                oGL.Debit = 0;
                                oGL.Debcred = "C";
                                oGL.Desciption = "Cap Gain Appr. from Sale of: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") +
                                                 " SP @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() +
                                                 " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                            }

                            oGL.FeeType = "SGIV";
                            oGL.TransType = "STKBSALE";
                            oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                            oGL.Ref01 = strAllotmentNo2;
                            oGL.Ref02 = strMarginCodeForNASDIndicator;
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

                            oGL.EffectiveDate =oRowView["Date"].ToString().Trim().ToDate();
                            oGL.AccountID = oRowView["CrossCustNo"].ToString().Trim();
                            oCust.CustAID = oRowView["CrossCustNo"].ToString().Trim();

                            oProduct.TransNo = strInvestmentNASDProductAccount;
                            oGL.MasterID = oProduct.GetProductGLAcct();
                            oGL.AcctRef = strInvestmentNASDProductAccount;
                            oGL.FeeType = "SGIB";
                            oGL.Desciption = "Stock Sale P/L: " + oRowView["Units"].ToString().Trim() + " " +
                                             oRowView["StockCode"].ToString().Trim() + " @ " +
                                             decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                             oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                             oCustomer.Othername.Trim();
                            oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                            oGL.TransType = "STKBSALE";
                            oGL.Ref01 = strAllotmentNo2;
                            oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                        }

                        #endregion
                    }

                    #region Agent Posting Seller

                    var oCustomerAgentPro2 = new Customer
                    {
                        CustAID = oRowView["CrossCustNo"].ToString().Trim()
                    };
                    var oCustAgentPro2 = new ProductAcct
                    {
                        ProductCode = strStockProductAccount,
                        CustAID = oRowView["CrossCustNo"].ToString().Trim()
                    };
                    decDeductComm = 0;
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

                                    decDeductComm = Math.Round(decDeductComm, 2, MidpointRounding.AwayFromZero);
                                    decTotalDeductCommission = decTotalDeductCommission + decDeductComm;
                                    decTotalDeductCommission =
                                        Math.Round(decTotalDeductCommission, 2, MidpointRounding.AwayFromZero);
                                    decTotalDeductCommissionSell = decTotalDeductCommissionSell + decDeductComm;
                                    decTotalDeductCommissionSell =
                                        Math.Round(decTotalDeductCommissionSell, 2, MidpointRounding.AwayFromZero);
                                }

                                oGL.EffectiveDate  =oRowView["Date"].ToString().Trim().ToDate();
                                if (strPostCommShared.Trim() == "YES")
                                {
                                    oGL.MasterID = oPGenTable.ShInv;
                                }
                                else
                                {
                                    oGL.MasterID = oPGenTable.AgComm;
                                }

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

                                oGL.Credit = 0;
                                oGL.Debcred = "D";
                                oGL.FeeType = "SECM";
                                if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                                {
                                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                     oCustomer.Othername.Trim();
                                    oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                                }
                                else
                                {
                                    oGL.Desciption = "Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                                oGL.Chqno = strMarginCodeForNASDIndicator;
                                oGL.RecAcctMaster = oPGenTable.ShInv;
                                oGL.RecAcct = "";
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                var dbCommandAgentExpense2 = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandAgentExpense2, transaction);

                                oGL.EffectiveDate =oRowView["Date"].ToString().Trim().ToDate();
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

                                oGL.Debit = 0;
                                oGL.Debcred = "C";
                                oGL.FeeType = "SBPY";
                                if ((oRowView["CrossType"].ToString().Trim() == "NO"))
                                {
                                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                     oCustomer.Othername.Trim();
                                    oGL.SysRef = "BSS" + "-" + strAllotmentNo2;
                                }
                                else
                                {
                                    oGL.Desciption = "Cross Deal Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                                oGL.Chqno = strMarginCodeForNASDIndicator;
                                oGL.RecAcctMaster = oPGenTable.ShInv;
                                oGL.RecAcct = "";
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                var dbCommandBrokPayable2 = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandBrokPayable2, transaction);
                            }
                        }
                    }

                    #endregion

                    break;
                }
            }

           
        }

        private static void PostGLForOrdinaryAndCrossBuyAllotmentNasd(SqlDatabase db, SqlTransaction transaction, DataRow oRowView,
            string strBuyerTrueId, ProductAcct oCust, GLParam oGLParamBoxSaleProfit, string strNASDStockProductAccount, Customer oCustomer,
            ProductAcct oCustCross, Allotment oAllot, Customer oCustomerCross, string strInvestmentNASDProductAccount,
            decimal decFeeTotalAmount, string strAllotmentNo, string strDefaultBranchCode, string strMarginCodeForNASDIndicator,
            PGenTable oPGenTable, CustomerExtraInformation oCustomerExtraInformationDirectCash, decimal TotalFeeBuyer,
            string strStockProductAccount, string strAgentProductAccount, string strPostCommShared, decimal decFeeCommission,
            ref decimal decTotalDeductCommissionBuy, string strBranchBuySellCharge, out SqlCommand oCommandJnumber,
            out string strJnumberNext, out decimal decGetUnitCost, out decimal decActualSellPrice, out decimal decSellDiff,
            out decimal decSellDiffTotal, out AcctGL oGL, out ProductAcct oProductAcctAgent, out decimal decDeductComm,
            ref decimal decTotalDeductCommissionSell, ref decimal decTotalDeductCommission)
        {
            oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
            db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(oCommandJnumber, transaction);
            strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();
            decDeductComm = 0;

            if ((oRowView["CrossD"].ToString().Trim() == "Y") && (strBuyerTrueId.Trim() != "")
                                                              && ((oRowView["CrossType"].ToString().Trim() == "CD") ||
                                                                  (oRowView["CrossType"].ToString().Trim() == "NO")
                                                                  || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                                                                  (oRowView["CrossType"].ToString().Trim() == "NT")))
            {
                oCust.ProductCode = strNASDStockProductAccount;
                oCust.CustAID = strBuyerTrueId;
                oCustomer.CustAID = strBuyerTrueId;
                oCustCross.ProductCode = strNASDStockProductAccount;
                oCustCross.CustAID = oAllot.CustAID;
                oCustomerCross.CustAID = oAllot.CustAID;
            }
            else
            {
                oCust.ProductCode = strNASDStockProductAccount;
                oCust.CustAID = oAllot.CustAID;
                oCustomer.CustAID = oAllot.CustAID;
                oCustCross.ProductCode = strNASDStockProductAccount;
                oCustCross.CustAID = oAllot.OtherCust;
                oCustomerCross.CustAID = oAllot.OtherCust;
            }

            if (!oCust.GetCustomerByCustId())
            {
                throw new Exception("Missing Customer Stock Account");
            }

            if (!oCustomer.GetCustomerName(strNASDStockProductAccount))
            {
                throw new Exception("Missing Customer Account");
            }

            if (oAllot.Cdeal == 'Y')
            {
                if (!oCustCross.GetCustomerByCustId())
                {
                    throw new Exception("Missing Customer Stock Account");
                }

                if (!oCustomerCross.GetCustomerName(strNASDStockProductAccount))
                {
                    throw new Exception("Missing Customer Account");
                }
            }

            decGetUnitCost = 0;
            decActualSellPrice = 0;
            decSellDiff = 0;
            decSellDiffTotal = 0;

            oGL = new AcctGL();
            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO")
                                      || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                                      (oRowView["CrossType"].ToString().Trim() == "NT"))
            {
                #region Customer Posting

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
                    oProduct.TransNo = strInvestmentNASDProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strInvestmentNASDProductAccount;
                }
                else
                {
                    oProduct.TransNo = strNASDStockProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strNASDStockProductAccount;
                }

                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                {
                    oGL.Credit = 0;
                    oGL.Debit = decFeeTotalAmount;
                    oGL.Debcred = "D";
                    oGL.FeeType = "BCUM";
                    if ((oRowView["CrossType"].ToString().Trim() == "NA"))
                    {
                        oGL.Desciption = "Stock Purchase (Cross:Buyer Bear All Charges): " +
                                         oRowView["Units"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() +
                                         " @ " + decimal.Parse(oAllot.UnitPrice.ToString()) + " - " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else if ((oRowView["CrossType"].ToString().Trim() == "NT"))
                    {
                        oGL.Desciption = "Stock Purchase (Cross:Seller Bear All Charges): " +
                                         oRowView["Units"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() +
                                         " @ " + decimal.Parse(oAllot.UnitPrice.ToString()) + " - " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n");
                    }

                    oGL.TransType = "STKBBUY";
                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGL.ClearingDayForTradingTransaction = "N";
                }
                else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                {
                    var oPortForSaleProfit = new Portfolio
                    {
                        PurchaseDate =oRowView["Date"].ToString().Trim().ToDate(),
                        CustomerAcct = oRowView["CustNo"].ToString().Trim(),
                        StockCode = oRowView["Stockcode"].ToString().Trim()
                    };
                    decGetUnitCost = decimal.Parse(oPortForSaleProfit.GetUnitCost().ToString());
                    decGetUnitCost = Math.Round(decGetUnitCost, 2, MidpointRounding.AwayFromZero);
                    decActualSellPrice = oAllot.TotalAmount / decimal.Parse(oRowView["Units"].ToString().Trim());
                    decActualSellPrice = Math.Round(decActualSellPrice, 2);
                    decSellDiff = decActualSellPrice - decGetUnitCost;
                    decSellDiff = Math.Round(decSellDiff, 2, MidpointRounding.AwayFromZero);
                    decSellDiffTotal = decSellDiff * decimal.Parse(oRowView["Units"].ToString().Trim());
                    decSellDiffTotal = Math.Round(decSellDiffTotal, 2, MidpointRounding.AwayFromZero);
                    oGL.Credit = oAllot.TotalAmount;
                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                    oGL.FeeType = "SCUM";
                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n");
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                    oGL.ClearingDayForTradingTransaction = "Y";
                }

                oGL.Ref01 = strAllotmentNo;
                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.Ref02 = strMarginCodeForNASDIndicator;
                oGL.Chqno = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                oGL.RecAcct = "";
                oGL.PostToOtherBranch = "Y";
                var dbCommandCustomer = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandCustomer, transaction);

                #endregion

                oGL.PostToOtherBranch = "N";
                oGL.ClearingDayForTradingTransaction = "N"; // So that others will not inherit this powerful privelege

                #region Second Leg Of Customer To Jobbing

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oGL.MasterID = oPGenTable.ShInv;
                oGL.AccountID = "";
                oGL.RecAcct = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                {
                    oGL.Credit = decFeeTotalAmount;
                    oGL.Debit = 0;
                    oGL.Debcred = "C";
                    oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n") + " By " +
                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                     oCustomer.Othername.Trim();
                    oGL.TransType = "STKBBUY";
                    oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                }
                else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                {
                    oGL.Credit = 0;
                    oGL.Debit = oAllot.TotalAmount;
                    oGL.Debcred = "D";
                    oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n") + " By " +
                                     oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                     oCustomer.Othername.Trim();
                    ;
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                }

                oGL.Ref01 = strAllotmentNo;
                oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                if (oCustomerExtraInformationDirectCash.DirectCashSettlementNASD &&
                    (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S"))
                {
                    #region Customer Posting For Direct Cash Settlement

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                    oCust.CustAID = oRowView["CustNo"].ToString().Trim();
                    if (oCust.GetBoxLoadStatus())
                    {
                        oProduct.TransNo = strInvestmentNASDProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strInvestmentNASDProductAccount;
                    }
                    else
                    {
                        oProduct.TransNo = strNASDStockProductAccount;
                        oGL.MasterID = oProduct.GetProductGLAcct();
                        oGL.AcctRef = strNASDStockProductAccount;
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
                    oGL.RecAcct = "";
                    oGL.RecAcctMaster = oPGenTable.ShInv;
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


                if ((oRowView["Buy_Sold_Ind"].ToString().Trim() == "S") && (oCust.GetBoxLoadStatus()) &&
                    (decSellDiffTotal != 0) && (oGLParamBoxSaleProfit.CheckParameter() == "YES"))
                {
                    #region Capital Gain Posting Of Profit

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
                                         " SP @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() +
                                         " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                        oGL.Debit = 0;
                        oGL.Debcred = "C";
                        oGL.Desciption = "Cap Gain Appr. from Sale of: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") +
                                         " SP @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() +
                                         " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }

                    oGL.FeeType = "SGIV";
                    oGL.TransType = "STKBSALE";
                    oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                    oGL.Ref01 = strAllotmentNo;
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
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

                    #region Box Load Posting Of Profit

                    oGL.EffectiveDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                    oProduct.TransNo = strInvestmentNASDProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strInvestmentNASDProductAccount;
                    oGL.FeeType = "SGIB";
                    oGL.TransType = "STKBSALE";
                    oGL.Desciption = "Stock Sale P/L: " + oRowView["Units"].ToString().Trim() + " " +
                                     oRowView["StockCode"].ToString().Trim() + " @ " +
                                     decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
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
            else if ((oAllot.Cdeal == 'Y') && (oRowView["CrossType"].ToString().Trim() == "CD"))
            {
                #region Customer Posting Cross Deal

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oGL.AccountID = oRowView["CustNo"].ToString().Trim();
                oCust.CustAID = oRowView["CustNo"].ToString().Trim();
                oCustomer.CustAID = oRowView["CustNo"].ToString().Trim();
                var oProduct = new Product();
                if (oCust.GetBoxLoadStatus())
                {
                    oProduct.TransNo = strInvestmentNASDProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strInvestmentNASDProductAccount;
                }
                else
                {
                    oProduct.TransNo = strNASDStockProductAccount;
                    oGL.MasterID = oProduct.GetProductGLAcct();
                    oGL.AcctRef = strNASDStockProductAccount;
                }

                if (!oCustomer.GetCustomerName(strNASDStockProductAccount))
                {
                    throw new Exception("Missing Customer Account");
                }

                oGL.Credit = 0;
                oGL.Debit = TotalFeeBuyer;
                oGL.Debcred = "D";
                oGL.FeeType = "BCUM";
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim();
                oGL.TransType = "STKBBUY";
                oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                oGL.Ref01 = strAllotmentNo;
                oGL.Reverse = "N";
                oGL.Jnumber = strJnumberNext;
                oGL.Branch = strDefaultBranchCode;
                oGL.Ref02 = strMarginCodeForNASDIndicator;
                oGL.Chqno = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                oGL.RecAcct = "";
                oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                oGL.PostToOtherBranch = "Y";
                var dbCommandCustomer = oGL.AddCommand();
                db.ExecuteNonQuery(dbCommandCustomer, transaction);

                #endregion

                oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege

                #region Second Leg Of Customer To Jobbing Cross Deal

                oGL.EffectiveDate =
                    oRowView["Date"].ToString().Trim().ToDate();
                oGL.MasterID = oPGenTable.ShInv;
                oGL.AccountID = "";
                oGL.RecAcct = "";
                oGL.RecAcctMaster = oPGenTable.ShInv;
                oGL.Debit = 0;
                oGL.Credit = TotalFeeBuyer;
                oGL.Debcred = "C";
                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                 decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                oGL.TransType = "STKBBUY";
                oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                oGL.Ref01 = strAllotmentNo;
                oGL.Ref02 = strMarginCodeForNASDIndicator;
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

            oProductAcctAgent = new ProductAcct
            {
                ProductCode = strStockProductAccount,
                CustAID = oRowView["CustNo"].ToString().Trim()
            };
            if (oProductAcctAgent.GetCustomerAgent())
            {
                decDeductComm = 0;
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
                            if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                            {
                                decDeductComm = (decFeeCommission * oProductAcctAgent.AgentComm) / 100;
                                decTotalDeductCommissionBuy = decTotalDeductCommissionBuy + decDeductComm;
                                decTotalDeductCommissionBuy =
                                    Math.Round(decTotalDeductCommissionBuy, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                decDeductComm = (oAllot.Commission * oProductAcctAgent.AgentComm) / 100;
                                decTotalDeductCommissionSell = decTotalDeductCommissionSell + decDeductComm;
                                decTotalDeductCommissionSell =
                                    Math.Round(decTotalDeductCommissionSell, 2, MidpointRounding.AwayFromZero);
                            }

                            decDeductComm = Math.Round(decDeductComm, 2, MidpointRounding.AwayFromZero);
                            decTotalDeductCommission = decTotalDeductCommission + decDeductComm;
                            decTotalDeductCommission = Math.Round(decTotalDeductCommission, 2, MidpointRounding.AwayFromZero);
                        }

                        oGL.EffectiveDate =oRowView["Date"].ToString().Trim().ToDate();
                        if (strPostCommShared.Trim() == "YES")
                        {
                            oGL.MasterID = oPGenTable.ShInv;
                        }
                        else
                        {
                            oGL.MasterID = oPGenTable.AgComm;
                        }

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
                        if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                        {
                            oGL.FeeType = "BECM";
                            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
                            {
                                oGL.Desciption = "Stk Purc.Comm For: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                            }
                            else
                            {
                                oGL.Desciption = "Cross Deal Purc.Comm.: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                                 oCustomerCross.Othername.Trim();
                            }

                            oGL.TransType = "STKBBUY";
                            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                        }
                        else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                        {
                            oGL.FeeType = "SECM";
                            oGL.TransType = "STKBSALE";
                            oGL.Desciption = "Stk Sale Comm: " + oRowView["Units"].ToString().Trim() + " " +
                                             oRowView["StockCode"].ToString().Trim() + " @ " +
                                             decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                             oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                             oCustomer.Othername.Trim();
                            oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                        }

                        oGL.Ref01 = strAllotmentNo;
                        oGL.AcctRef = "";
                        oGL.Reverse = "N";
                        oGL.Jnumber = strJnumberNext;
                        oGL.Branch = strDefaultBranchCode;
                        oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                        oGL.Chqno = strMarginCodeForNASDIndicator;
                        oGL.RecAcctMaster = oPGenTable.ShInv;
                        oGL.RecAcct = "";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        var dbCommandAgentExpense = oGL.AddCommand();
                        db.ExecuteNonQuery(dbCommandAgentExpense, transaction);


                        oGL.EffectiveDate =oRowView["Date"].ToString().Trim().ToDate();
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
                        if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                        {
                            oGL.FeeType = "BBPY";
                            if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
                            {
                                oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                                 oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                                 oCustomer.Othername.Trim();
                            }
                            else
                            {
                                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                                 oRowView["StockCode"].ToString().Trim() + " @ " +
                                                 decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                                 oCustomerCross.Surname.Trim() + " " + oCustomerCross.Firstname.Trim() + " " +
                                                 oCustomerCross.Othername.Trim();
                            }

                            oGL.TransType = "STKBBUY";
                            oGL.SysRef = "BSB" + "-" + strAllotmentNo;
                        }
                        else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                        {
                            oGL.FeeType = "SBPY";
                            oGL.TransType = "STKBSALE";
                            oGL.Desciption = "Stock Sale: " + oRowView["Units"].ToString().Trim() + " " +
                                             oRowView["StockCode"].ToString().Trim() + " @ " +
                                             decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                             oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                             oCustomer.Othername.Trim();
                            oGL.SysRef = "BSS" + "-" + strAllotmentNo;
                        }

                        oGL.Ref01 = strAllotmentNo;
                        oGL.Reverse = "N";
                        oGL.Jnumber = strJnumberNext;
                        oGL.Branch = strDefaultBranchCode;
                        oGL.Ref02 = oRowView["CustNo"].ToString().Trim();
                        oGL.Chqno = strMarginCodeForNASDIndicator;
                        oGL.RecAcctMaster = oPGenTable.ShInv;
                        oGL.RecAcct = "";
                        oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                        var dbCommandBrokPayable = oGL.AddCommand();
                        db.ExecuteNonQuery(dbCommandBrokPayable, transaction);
                    }
                }
            }

            #endregion

            #region Posting Buy Different Charges

            if (strBranchBuySellCharge.Trim() == "YES" && oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
            {
                var oBranchBuySellCharge = new BranchBuySellCharge();
                var oCustomerBuySellCharge = new Customer();
                var oBranchBuySellChargeCustomer = new Branch();
                oCustomerBuySellCharge.CustAID = oRowView["CustNo"].ToString().Trim();
                oBranchBuySellChargeCustomer.TransNo = oCustomerBuySellCharge.GetBranchId();
                oBranchBuySellChargeCustomer.GetBranch();
                oBranchBuySellCharge.GetBranchBuySellCharges();
                if (oBranchBuySellChargeCustomer.TransNo.Trim() != "" &&
                    oBranchBuySellChargeCustomer.TransNo.Trim() != oBranchBuySellChargeCustomer.DefaultBranch.Trim() &&
                    oBranchBuySellChargeCustomer.JointHeadOffice == false)
                {
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

                    oGL.MasterID = oPGenTable.NASDBComm;
                    oGL.AccountID = "";
                    oGL.FeeType = "BCOMBRCHGD";
                    if ((oAllot.Cdeal == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
                    {
                        oGL.Desciption = "Stock Purchase: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
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
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
                                         oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " +
                                         oCustomer.Othername.Trim();
                    }
                    else
                    {
                        oGL.Desciption = "Cross Deal Pur.: " + oRowView["Units"].ToString().Trim() + " " +
                                         oRowView["StockCode"].ToString().Trim() + " @ " +
                                         decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " +
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
                    oGL.Ref02 = strMarginCodeForNASDIndicator;
                    oGL.Chqno = "";
                    oGL.RecAcctMaster = oBranchBuySellChargeCustomer.Commission;
                    oGL.RecAcct = "";
                    oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                    oGL.PostToOtherBranch = "Y";
                    var dbCommandCommBranchBuyChargeCommissionIncomeBranchControl = oGL.AddCommand();
                    db.ExecuteNonQuery(dbCommandCommBranchBuyChargeCommissionIncomeBranchControl, transaction);
                    oGL.PostToOtherBranch = "N"; // So that others will not inherit this powerful privelege
                }
            }

            #endregion

        }

        private void ProcessAllotmentNasd(DataRow oRowView, StkParam oStkbPGenTable, string strUserName,
            string strMarginCodeForNASDIndicator, SqlDatabase db, SqlTransaction transaction,
            CustomerExtraInformation oCustomerExtraInformationDirectCash, out Allotment oAllot,
            out decimal decAllotConsideration,  ref decimal decTotalBank, out decimal decAllotConsiderationBuy, ref decimal decTotalBankAudit,
            out decimal decFeeSec, ref decimal decTotalSEC, out decimal decFeeStampDuty, ref decimal decTotalStampDuty,
            out decimal decFeeCommission, ref decimal decTotalCommission, ref decimal decTotalCommissionBuy, out decimal decFeeVAT,
            ref decimal decTotalVAT, out decimal decFeeNSE, ref decimal decTotalNSE, out decimal decFeeCSCS, ref decimal decTotalCSCS,
            out decimal decSMSAlertCSCS, out decimal decFeeSecVat, out decimal decFeeNseVat, out decimal decFeeCscsVat,
            out decimal decSMSAlertCSCSVAT, out decimal decFeeTotalAmount, out decimal decCrossDealTotalAmount,
            out decimal TotalFeeBuyer, ref string strAllotmentNo, out string strBuyerTrueId, ref decimal decTotalCommissionSell,
            out decimal TotalFeeSeller, ref decimal decTotalDirectCashSettlement, ref decimal decTotalDirectCashSettlementBond,
            out string strAllotmentNo2, out decimal decFeeSecSeller, out decimal decFeeStampDutySeller,
            out decimal decFeeCommissionSeller, out decimal decFeeVATSeller, out decimal decFeeNSESeller,
            out decimal decFeeCSCSSeller, out decimal decSMSAlertCSCSSeller, out decimal decFeeSecVatSeller,
            out decimal decFeeNseVatSeller, out decimal decFeeCscsVatSeller, out decimal decSMSAlertCSCSVATSeller,
            out decimal TotalFeeSellerPost)
        {
            decAllotConsideration = 0;
            decAllotConsiderationBuy = 0;
            decFeeSec = 0;
            decFeeStampDuty = 0;
            decFeeCommission = 0;
            decFeeVAT = 0;
            decFeeNSE = 0;
            decFeeCSCS = 0;
            decSMSAlertCSCS = 0;
            decFeeSecVat = 0;
            decFeeNseVat = 0;
            decFeeCscsVat = 0;
            decSMSAlertCSCSVAT = 0;
            decFeeTotalAmount = 0;
            decCrossDealTotalAmount = 0;
            TotalFeeBuyer = 0;
            strBuyerTrueId = null;
            TotalFeeSeller = 0;
            strAllotmentNo2 = null;
            decFeeSecSeller = 0;
            decFeeStampDutySeller = 0;
            decFeeCommissionSeller = 0;
            decFeeVATSeller = 0;
            decFeeNSESeller = 0;
            decFeeCSCSSeller = 0;
            decSMSAlertCSCSSeller = 0;
            decFeeSecVatSeller = 0;
            decFeeNseVatSeller = 0;
            decFeeCscsVatSeller = 0;
            decSMSAlertCSCSVATSeller = 0;
            TotalFeeSellerPost = 0;
            oAllot = new Allotment();
            switch (oRowView["Buy_Sold_Ind"].ToString().Trim())
            {
                case "B":
                {
                    #region Save Buyer For 1.Single 2.Ordinary Cross 3.Norminal Cross 4. NA Buy Only With Consideration 5. NT Seller Only With Consideration

                    oAllot.CustAID = oRowView["CustNo"].ToString();
                    oAllot.StockCode = oRowView["Stockcode"].ToString();
                    oAllot.DateAlloted =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oAllot.Qty = long.Parse(oRowView["Units"].ToString());
                    oAllot.UnitPrice = Decimal.Parse(oRowView["UnitPrice"].ToString());
                    oAllot.CommissionType = "FIXED";
                    decAllotConsideration =
                        Math.Round(
                            Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                            long.Parse(oRowView["Units"].ToString().Trim()), 2);
                    decAllotConsiderationBuy =
                        Math.Round(
                            Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                            long.Parse(oRowView["Units"].ToString().Trim()), 2);
                    decAllotConsideration = Math.Round(decAllotConsideration, 2, MidpointRounding.AwayFromZero);
                    decAllotConsiderationBuy = Math.Round(decAllotConsiderationBuy, 2, MidpointRounding.AwayFromZero);
                    if ((oRowView["CrossD"].ToString().Trim() == "N") || (oRowView["CrossType"].ToString().Trim() == "NO")
                                                                      || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                                                                      (oRowView["CrossType"].ToString().Trim() == "NT"))
                    {
                        decTotalBank = decTotalBank - decAllotConsideration;
                        decTotalBankAudit = decTotalBankAudit + decAllotConsideration;
                        decTotalBank = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero);
                        decTotalBankAudit = Math.Round(decTotalBankAudit, 2, MidpointRounding.AwayFromZero);
                    }

                    oAllot.Consideration = decAllotConsideration;

                    oAllot.SecFee = decAllotConsideration * (oStkbPGenTable.NASDBSec / 100);

                    oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
                    decFeeSec = oAllot.SecFee;
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);

                    oAllot.StampDuty = decAllotConsideration * (oStkbPGenTable.NASDBStamp / 100);
                    oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2);
                    decFeeStampDuty = oAllot.StampDuty;
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + oAllot.StampDuty;
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);

                    oAllot.Commission = oAllot.ComputeComm(decAllotConsideration, "B", oStkbPGenTable.NASDBComm,
                        oStkbPGenTable.NASDBCommMin);
                    oAllot.Commission = Math.Round(oAllot.Commission, 2);
                    decFeeCommission = oAllot.Commission;
                    decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                    decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                    decTotalCommissionBuy = Math.Round(decTotalCommissionBuy, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                    decTotalCommissionBuy = Math.Round(decTotalCommissionBuy, 2, MidpointRounding.AwayFromZero);

                    oAllot.VAT = oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "B", oStkbPGenTable.NASDBCommVAT,
                        oStkbPGenTable.MinVatB, oStkbPGenTable.NASDBComm,
                        oRowView["Date"].ToString().Trim().ToDate());
                    oAllot.VAT = Math.Round(oAllot.VAT, 2);
                    decFeeVAT = oAllot.VAT;
                    decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                    decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);


                    oAllot.NSEFee = decAllotConsideration * (oStkbPGenTable.NASDBNASD / 100);

                    oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2);
                    decFeeNSE = oAllot.NSEFee;
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);


                    oAllot.CSCSFee = decAllotConsideration * (oStkbPGenTable.NASDBCSCS / 100);
                    oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2);
                    decFeeCSCS = oAllot.CSCSFee;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
                    oAllot.SMSAlertCSCS = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) * oStkbPGenTable.NASDBSMSCharge;
                    oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
                    decSMSAlertCSCS = oAllot.SMSAlertCSCS;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                    oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());
                    oAllot.SecVat = oAllot.SecFee * (oStkbPGenTable.NASDBSECVAT / 100);
                    oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
                    decFeeSecVat = oAllot.SecVat;
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);

                    oAllot.NseVat = oAllot.NSEFee * (oStkbPGenTable.NASDBNASDVAT / 100);
                    oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
                    decFeeNseVat = oAllot.NseVat;
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);

                    oAllot.CscsVat = oAllot.CSCSFee * (oStkbPGenTable.NASDBCSCSVAT / 100);
                    oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
                    decFeeCscsVat = oAllot.CscsVat;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                    oAllot.SMSAlertCSCSVAT = oAllot.SMSAlertCSCS * (oStkbPGenTable.NASDBAlertVAT / 100);
                    oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2);
                    decSMSAlertCSCSVAT = oAllot.SMSAlertCSCSVAT;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCSVAT;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                    oAllot.TotalAmount = decAllotConsideration + oAllot.SecFee + oAllot.StampDuty
                                         + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS + oAllot.NSEFee
                                         + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat + oAllot.SMSAlertCSCSVAT;
                    oAllot.TotalAmount = Math.Round(oAllot.TotalAmount, 2);

                    //Populate decFeeTotalAmount Only at this stage for Buy Cross deal N or cross Type NO only
                    decFeeTotalAmount = oAllot.TotalAmount;
                    decCrossDealTotalAmount = oAllot.TotalAmount;

                    TotalFeeBuyer = oAllot.SecFee + oAllot.StampDuty
                                                  + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                                                  oAllot.NSEFee
                                                  + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat + oAllot.SMSAlertCSCSVAT;
                    TotalFeeBuyer = Math.Round(TotalFeeBuyer, 2, MidpointRounding.AwayFromZero);

                    if (oRowView["CrossType"].ToString().Trim() == "CD")
                    {
                        oAllot.TotalAmount = TotalFeeBuyer;
                    }

                    oAllot.Posted = true;
                    oAllot.Reversed = false;
                    oAllot.UserId = strUserName;
                    oAllot.Cdeal = Char.Parse(oRowView["CrossD"].ToString());
                    oAllot.Autopost = Char.Parse("Y");
                    oAllot.TicketNO = oRowView["BSlip#"].ToString();
                    oAllot.SoldBy = oRowView["SoldBy"].ToString();
                    oAllot.BoughtBy = oRowView["BoughtBy"].ToString();
                    oAllot.Buy_sold_Ind = Char.Parse("B");
                    oAllot.CDSellTrans = "";
                    oAllot.OtherCust = oRowView["CrossCustNo"].ToString();
                    oAllot.MarginCode = strMarginCodeForNASDIndicator;
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

                    #endregion

                    #region Save Seller For 1.Ordinary Cross 2.Norminal Cross 3.Seller NS Cross Deal 4.Buyer NB Cross Deal 5.Seller NT Cross Deal With Consideration 5.Buyer NA Cross Deal With Consideration

                    if (oRowView["CrossD"].ToString().Trim() == "Y")
                    {
                        #region Save Seller For 1.Ordinary Cross 2.Norminal Cross 3.Buyer For NA Cross Deal 4.Seller For NT Cross Deal

                        if ((oRowView["CrossType"].ToString().Trim() == "CD") || (oRowView["CrossType"].ToString().Trim() == "NO")
                                                                              || (oRowView["CrossType"].ToString().Trim() ==
                                                                                  "NA") ||
                                                                              (oRowView["CrossType"].ToString().Trim() == "NT"))
                        {
                            strBuyerTrueId = oAllot.CustAID;
                            oAllot.CustAID = oRowView["CrossCustNo"].ToString();
                            oAllot.StockCode = oRowView["Stockcode"].ToString().Trim();
                            oAllot.DateAlloted  =oRowView["Date"].ToString().Trim().ToDate();
                            oAllot.Qty = long.Parse(oRowView["Units"].ToString().Trim());
                            oAllot.UnitPrice = Decimal.Parse(oRowView["UnitPrice"].ToString().Trim());
                            oAllot.CommissionType = "FIXED";
                            decAllotConsideration =
                                Math.Round(
                                    Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                                    long.Parse(oRowView["Units"].ToString().Trim()), 2);
                            decAllotConsiderationBuy =
                                Math.Round(
                                    Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                                    long.Parse(oRowView["Units"].ToString().Trim()), 2, MidpointRounding.AwayFromZero);
                            decAllotConsideration = Math.Round(decAllotConsideration, 2, MidpointRounding.AwayFromZero);
                            decAllotConsiderationBuy = Math.Round(decAllotConsiderationBuy, 2, MidpointRounding.AwayFromZero);
                            if ((oRowView["CrossType"].ToString().Trim() == "NO")
                                || (oRowView["CrossType"].ToString().Trim() == "NA") ||
                                (oRowView["CrossType"].ToString().Trim() == "NT"))
                            {
                                decTotalBank = decTotalBank + decAllotConsideration;
                                decTotalBankAudit = decTotalBankAudit + decAllotConsideration;
                                decTotalBank = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero);
                                decTotalBankAudit = Math.Round(decTotalBankAudit, 2, MidpointRounding.AwayFromZero);
                            }

                            oAllot.Consideration = decAllotConsideration;

                            oAllot.SecFee = decAllotConsideration * (oStkbPGenTable.NASDSSec / 100);

                            oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);


                            oAllot.StampDuty = decAllotConsideration * (oStkbPGenTable.NASDSStamp / 100);

                            oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2);
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + oAllot.StampDuty;
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);

                            oAllot.Commission = oAllot.ComputeComm(decAllotConsideration, "S", oStkbPGenTable.NASDSComm,
                                oStkbPGenTable.NASDSCommMin);
                            oAllot.Commission = Math.Round(oAllot.Commission, 2);
                            decTotalCommission =
                                Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                            decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero) +
                                                     oAllot.Commission;
                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero);

                            oAllot.VAT = oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "S",
                                oStkbPGenTable.NASDSCommVAT, oStkbPGenTable.MinVatS, oStkbPGenTable.NASDSComm,
                                oRowView["Date"].ToString().Trim().ToDate());
                            oAllot.VAT = Math.Round(oAllot.VAT, 2);
                            decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                            decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);


                            oAllot.NSEFee = decAllotConsideration * (oStkbPGenTable.NASDSNASD / 100);

                            oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2);
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);


                            oAllot.CSCSFee = decAllotConsideration * (oStkbPGenTable.NASDSCSCS / 100);

                            oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            oAllot.SMSAlertCSCS = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) *
                                                  oStkbPGenTable.NASDSSMSCharge;
                            oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());


                            oAllot.SecVat = oAllot.SecFee * (oStkbPGenTable.NASDSSECVAT / 100);

                            oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);


                            oAllot.NseVat = oAllot.NSEFee * (oStkbPGenTable.NASDSNASDVAT / 100);

                            oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);


                            oAllot.CscsVat = oAllot.CSCSFee * (oStkbPGenTable.NASDSCSCSVAT / 100);

                            oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            oAllot.SMSAlertCSCSVAT = oAllot.SMSAlertCSCS * (oStkbPGenTable.NASDSAlertVAT / 100);
                            oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCSVAT;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);


                            oAllot.TotalAmount = decAllotConsideration - (oAllot.SecFee + oAllot.StampDuty
                                + oAllot.Commission + oAllot.VAT +
                                oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                                oAllot.NSEFee
                                + oAllot.SecVat + oAllot.NseVat +
                                oAllot.CscsVat + oAllot.SMSAlertCSCSVAT);
                            oAllot.TotalAmount = Math.Round(oAllot.TotalAmount, 2, MidpointRounding.AwayFromZero);

                            decCrossDealTotalAmount = oAllot.TotalAmount;

                            TotalFeeSeller = oAllot.SecFee + oAllot.StampDuty
                                                           + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                                                           oAllot.NSEFee
                                                           + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat +
                                                           oAllot.SMSAlertCSCSVAT;
                            TotalFeeSeller = Math.Round(TotalFeeSeller, 2, MidpointRounding.AwayFromZero);

                            if (oRowView["CrossType"].ToString().Trim() == "CD")
                            {
                                oAllot.TotalAmount = TotalFeeSeller;
                            }

                            oAllot.Posted = true;
                            oAllot.Reversed = false;
                            oAllot.UserId = strUserName;
                            oAllot.Cdeal = Char.Parse("Y");
                            oAllot.Autopost = Char.Parse("Y");
                            //oAllot.TicketNO = oRowView["BSlip#"].ToString();
                            if (oRowView["CrossType"].ToString().Trim() == "NO" || oRowView["CrossType"].ToString().Trim() == "NA"
                                || oRowView["CrossType"].ToString().Trim() == "NT")
                            {
                                oCustomerExtraInformationDirectCash.CustAID = oRowView["CrossCustNo"].ToString();
                                oCustomerExtraInformationDirectCash.WedAnniversaryDate =
                                    oRowView["Date"].ToString().Trim().ToDate();
                                if (oCustomerExtraInformationDirectCash.DirectCashSettlementNASD)
                                {
                                    if (oStkbPGenTable.StockInstrument != DataGeneral.StockInstrumentType.BOND)
                                    {
                                        decTotalDirectCashSettlement = decTotalDirectCashSettlement + oAllot.TotalAmount;
                                        decTotalDirectCashSettlement = Math.Round(decTotalDirectCashSettlement, 2,
                                            MidpointRounding.AwayFromZero);
                                        oAllot.TicketNO = "DCS";
                                    }
                                    else
                                    {
                                        decTotalDirectCashSettlementBond = decTotalDirectCashSettlementBond + oAllot.TotalAmount;
                                        decTotalDirectCashSettlementBond = Math.Round(decTotalDirectCashSettlementBond, 2,
                                            MidpointRounding.AwayFromZero);
                                        oAllot.TicketNO = "DCS";
                                    }
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

                            oAllot.SoldBy = oRowView["SoldBy"].ToString();
                            oAllot.BoughtBy = oRowView["BoughtBy"].ToString();
                            oAllot.Buy_sold_Ind = Char.Parse("S");
                            oAllot.CDSellTrans = strAllotmentNo;
                            oAllot.OtherCust = oRowView["Custno"].ToString();
                            oAllot.MarginCode = strMarginCodeForNASDIndicator;
                            oAllot.CrossType = oRowView["CrossType"].ToString();
                            //if (oRowView["CrossType"].ToString().Trim() != "NO" &&
                            //    oRowView["CrossType"].ToString().Trim() != "NA" && oRowView["CrossType"].ToString().Trim() != "NT")
                            //{
                            //    oAllot.Consideration = 0;
                            //}
                            if (oRowView["CrossType"].ToString().Trim() == "CD")
                            {
                                oAllot.Consideration = 0;
                            }

                            oAllot.PrintFlag = 'N';
                            SqlCommand dbCommandAllot;
                            dbCommandAllot = oAllot.AddCommand();
                            db.ExecuteNonQuery(dbCommandAllot, transaction);
                            strAllotmentNo2 = db.GetParameterValue(dbCommandAllot, "Txn").ToString().Trim();
                        }

                        #endregion

                        #region Save 1.Buyer For NB Cross Deal 2.Seller For NS Cross Deal

                        else
                        {
                            oAllot.SecFee = oAllot.SecFee + Math.Round((decAllotConsideration * (oStkbPGenTable.NASDSSec / 100)), 2,
                                MidpointRounding.AwayFromZero);
                            decFeeSecSeller = decAllotConsideration * (oStkbPGenTable.NASDSSec / 100);

                            oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
                            decFeeSecSeller = Math.Round(decFeeSecSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + decFeeSecSeller;
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);


                            oAllot.StampDuty = oAllot.StampDuty +
                                               Math.Round((decAllotConsideration * (oStkbPGenTable.NASDSStamp / 100)), 2,
                                                   MidpointRounding.AwayFromZero);
                            ;
                            decFeeStampDutySeller = decAllotConsideration * (oStkbPGenTable.NASDSStamp / 100);

                            oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2);
                            decFeeStampDutySeller = Math.Round(decFeeStampDutySeller, 2, MidpointRounding.AwayFromZero);
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) +
                                                decFeeStampDutySeller;
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);

                            oAllot.Commission = oAllot.Commission + oAllot.ComputeComm(decAllotConsideration, "S",
                                oStkbPGenTable.NASDSComm, oStkbPGenTable.NASDSCommMin);
                            oAllot.Commission = Math.Round(oAllot.Commission, 2);
                            decFeeCommissionSeller = oAllot.ComputeComm(decAllotConsideration, "S", oStkbPGenTable.NASDSComm,
                                oStkbPGenTable.NASDSCommMin);
                            decFeeCommissionSeller = Math.Round(decFeeCommissionSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) +
                                                 decFeeCommissionSeller;
                            decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero) +
                                                     decFeeCommissionSeller;
                            decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero);

                            oAllot.VAT = oAllot.VAT + oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "S",
                                oStkbPGenTable.NASDSCommVAT, oStkbPGenTable.MinVatS, oStkbPGenTable.NASDSComm,
                                oRowView["Date"].ToString().Trim().ToDate());
                            oAllot.VAT = Math.Round(oAllot.VAT, 2);
                            decFeeVATSeller = oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "S",
                                oStkbPGenTable.NASDSCommVAT, oStkbPGenTable.MinVatS, oStkbPGenTable.NASDSComm,
                                oRowView["Date"].ToString().Trim().ToDate());
                            decFeeVATSeller = Math.Round(decFeeVATSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero) + decFeeVATSeller;
                            decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);


                            oAllot.NSEFee = oAllot.NSEFee +
                                            Math.Round((decAllotConsideration * (oStkbPGenTable.NASDSNASD / 100)), 2);
                            decFeeNSESeller = (decAllotConsideration * (oStkbPGenTable.NASDSNASD / 100));

                            oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2);
                            decFeeNSESeller = Math.Round(decFeeNSESeller, 2, MidpointRounding.AwayFromZero);
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + decFeeNSESeller;
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);


                            oAllot.CSCSFee = oAllot.CSCSFee +
                                             Math.Round((decAllotConsideration * (oStkbPGenTable.NASDSCSCS / 100)), 2);
                            decFeeCSCSSeller = (decAllotConsideration * (oStkbPGenTable.NASDSCSCS / 100));

                            oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2);
                            decFeeCSCSSeller = Math.Round(decFeeCSCSSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + decFeeCSCSSeller;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            oAllot.SMSAlertCSCS = oAllot.SMSAlertCSCS + (int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) *
                                                                         oStkbPGenTable.NASDSSMSCharge);
                            oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
                            decSMSAlertCSCSSeller = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) *
                                                    oStkbPGenTable.NASDSSMSCharge;
                            decSMSAlertCSCSSeller = Math.Round(decSMSAlertCSCSSeller, 2);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + decSMSAlertCSCSSeller;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());


                            oAllot.SecVat = oAllot.SecVat + oAllot.SecFee * (oStkbPGenTable.NASDSSECVAT / 100);


                            decFeeSecVatSeller = oAllot.SecFee * (oStkbPGenTable.NASDSSECVAT / 100);

                            oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
                            decFeeSecVatSeller = Math.Round(decFeeSecVatSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + decFeeSecVatSeller;
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);


                            oAllot.NseVat = oAllot.NseVat + oAllot.NSEFee * (oStkbPGenTable.NASDSNASDVAT / 100);


                            decFeeNseVatSeller = oAllot.NSEFee * (oStkbPGenTable.NASDSNASDVAT / 100);

                            oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
                            decFeeNseVatSeller = Math.Round(decFeeNseVatSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + decFeeNseVatSeller;
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);


                            oAllot.CscsVat = oAllot.CscsVat + oAllot.CSCSFee * (oStkbPGenTable.NASDSCSCSVAT / 100);


                            decFeeCscsVatSeller = oAllot.CSCSFee * (oStkbPGenTable.NASDSCSCSVAT / 100);

                            oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
                            decFeeCscsVatSeller = Math.Round(decFeeCscsVatSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + decFeeCscsVatSeller;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            oAllot.SMSAlertCSCSVAT =
                                oAllot.SMSAlertCSCSVAT + oAllot.SMSAlertCSCS * (oStkbPGenTable.NASDSAlertVAT / 100);
                            decSMSAlertCSCSVATSeller = oAllot.SMSAlertCSCS * (oStkbPGenTable.NASDSAlertVAT / 100);
                            oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2);
                            decSMSAlertCSCSVATSeller = Math.Round(decSMSAlertCSCSVATSeller, 2, MidpointRounding.AwayFromZero);
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + decSMSAlertCSCSVATSeller;
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            TotalFeeSeller = oAllot.SecFee + oAllot.StampDuty
                                                           + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee + oAllot.SMSAlertCSCS +
                                                           oAllot.NSEFee
                                                           + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat +
                                                           oAllot.SMSAlertCSCSVAT;
                            TotalFeeSeller = Math.Round(TotalFeeSeller, 2, MidpointRounding.AwayFromZero);

                            TotalFeeSellerPost = decFeeSecSeller + decFeeStampDutySeller
                                                                 + decFeeCommissionSeller + decFeeVATSeller + decFeeCSCSSeller +
                                                                 decSMSAlertCSCSSeller + decFeeNSESeller
                                                                 + decFeeSecVatSeller + decFeeNseVatSeller + decFeeCscsVatSeller +
                                                                 decSMSAlertCSCSVATSeller;
                            TotalFeeSellerPost = Math.Round(TotalFeeSellerPost, 2, MidpointRounding.AwayFromZero);

                            //Change TotalFeeSeller to TotalFeeSellerPost because I suspect error
                            oAllot.TotalAmount = TotalFeeBuyer + TotalFeeSellerPost;

                            //Changes For One Transaction Only For NB and NS
                            if (oRowView["CrossType"].ToString().Trim() == "NS")
                            {
                                oAllot.Buy_sold_Ind = Char.Parse("S");
                            }

                            var dbCommandAllot = oAllot.AddCommand();
                            db.ExecuteNonQuery(dbCommandAllot, transaction);
                            strAllotmentNo = db.GetParameterValue(dbCommandAllot, "Txn").ToString().Trim();
                            strAllotmentNo2 = strAllotmentNo;
                        }

                        #endregion
                    }

                    #endregion

                    break;
                }
                case "S" when (oRowView["CrossD"].ToString().Trim() == "N"):
                {
                    #region Save Sell For Single Only

                    oAllot.CustAID = oRowView["CustNo"].ToString().Trim();
                    oAllot.StockCode = oRowView["Stockcode"].ToString().Trim();
                    oAllot.DateAlloted =
                        oRowView["Date"].ToString().Trim().ToDate();
                    oAllot.Qty = long.Parse(oRowView["Units"].ToString().Trim());
                    oAllot.UnitPrice = Decimal.Parse(oRowView["UnitPrice"].ToString().Trim());
                    oAllot.CommissionType = "FIXED";
                    decAllotConsideration =
                        Math.Round(
                            Decimal.Parse(oRowView["UnitPrice"].ToString().Trim()) *
                            long.Parse(oRowView["Units"].ToString().Trim()), 2);
                    decAllotConsideration = Math.Round(decAllotConsideration, 2, MidpointRounding.AwayFromZero);
                    decTotalBank = decTotalBank + decAllotConsideration;
                    decTotalBankAudit = decTotalBankAudit + decAllotConsideration;
                    decTotalBank = Math.Round(decTotalBank, 2, MidpointRounding.AwayFromZero);
                    decTotalBankAudit = Math.Round(decTotalBankAudit, 2, MidpointRounding.AwayFromZero);
                    oAllot.Consideration = decAllotConsideration;

                    oAllot.SecFee = decAllotConsideration * (oStkbPGenTable.NASDSSec / 100);

                    oAllot.SecFee = Math.Round(oAllot.SecFee, 2, MidpointRounding.AwayFromZero);
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + oAllot.SecFee;
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);


                    oAllot.StampDuty = decAllotConsideration * (oStkbPGenTable.NASDSStamp / 100);

                    oAllot.StampDuty = Math.Round(oAllot.StampDuty, 2);
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + oAllot.StampDuty;
                    decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);

                    oAllot.Commission = oAllot.ComputeComm(decAllotConsideration, "S", oStkbPGenTable.NASDSComm,
                        oStkbPGenTable.NASDSCommMin);
                    oAllot.Commission = Math.Round(oAllot.Commission, 2);
                    decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                    decTotalCommission = Math.Round(decTotalCommission, 2, MidpointRounding.AwayFromZero);

                    decTotalCommissionSell =
                        Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero) + oAllot.Commission;
                    decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero);


                    oAllot.VAT = oAllot.ComputeVAT(decAllotConsideration, oAllot.Commission, "S", oStkbPGenTable.NASDSCommVAT,
                        oStkbPGenTable.MinVatS, oStkbPGenTable.NASDSComm,
                        oRowView["Date"].ToString().Trim().ToDate());
                    oAllot.VAT = Math.Round(oAllot.VAT, 2);
                    decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero) + oAllot.VAT;
                    decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);


                    oAllot.NSEFee = decAllotConsideration * (oStkbPGenTable.NASDSNASD / 100);

                    oAllot.NSEFee = Math.Round(oAllot.NSEFee, 2);
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + oAllot.NSEFee;
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);


                    oAllot.CSCSFee = decAllotConsideration * (oStkbPGenTable.NASDSCSCS / 100);

                    oAllot.CSCSFee = Math.Round(oAllot.CSCSFee, 2);
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.CSCSFee;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                    oAllot.SMSAlertCSCS = int.Parse(oRowView["NumberOfTrans"].ToString().Trim()) * oStkbPGenTable.NASDSSMSCharge;
                    oAllot.SMSAlertCSCS = Math.Round(oAllot.SMSAlertCSCS, 2);
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCS;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                    oAllot.NumberOfTrans = int.Parse(oRowView["NumberOfTrans"].ToString().Trim());


                    oAllot.SecVat = oAllot.SecFee * (oStkbPGenTable.NASDSSECVAT / 100);
                    oAllot.SecVat = Math.Round(oAllot.SecVat, 2, MidpointRounding.AwayFromZero);
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + oAllot.SecVat;
                    decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);


                    oAllot.NseVat = oAllot.NSEFee * (oStkbPGenTable.NASDSNASDVAT / 100);
                    oAllot.NseVat = Math.Round(oAllot.NseVat, 2);
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + oAllot.NseVat;
                    decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);


                    oAllot.CscsVat = oAllot.CSCSFee * (oStkbPGenTable.NASDSCSCSVAT / 100);
                    oAllot.CscsVat = Math.Round(oAllot.CscsVat, 2);
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.CscsVat;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                    oAllot.SMSAlertCSCSVAT = oAllot.SMSAlertCSCS * (oStkbPGenTable.NASDSAlertVAT / 100);
                    oAllot.SMSAlertCSCSVAT = Math.Round(oAllot.SMSAlertCSCSVAT, 2);
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + oAllot.SMSAlertCSCSVAT;
                    decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                    oAllot.TotalAmount = decAllotConsideration - (oAllot.SecFee + oAllot.StampDuty
                        + oAllot.Commission + oAllot.VAT + oAllot.CSCSFee +
                        oAllot.SMSAlertCSCS + oAllot.NSEFee
                        + oAllot.SecVat + oAllot.NseVat + oAllot.CscsVat +
                        oAllot.SMSAlertCSCSVAT);
                    oAllot.TotalAmount = Math.Round(oAllot.TotalAmount, 2, MidpointRounding.AwayFromZero);
                    oAllot.Posted = true;
                    oAllot.Reversed = false;
                    oAllot.UserId = strUserName;
                    oAllot.Cdeal = Char.Parse("N");
                    oAllot.Autopost = Char.Parse("Y");
                    //oAllot.TicketNO = oRowView["BSlip#"].ToString();
                    oCustomerExtraInformationDirectCash.CustAID = oRowView["CustNo"].ToString().Trim();
                    oCustomerExtraInformationDirectCash.WedAnniversaryDate =
                        oRowView["Date"].ToString().Trim().ToDate();
                    if (oCustomerExtraInformationDirectCash.DirectCashSettlementNASD)
                    {
                        decTotalDirectCashSettlement = decTotalDirectCashSettlement + oAllot.TotalAmount;
                        decTotalDirectCashSettlement = Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                        oAllot.TicketNO = "DCS";
                    }
                    else
                    {
                        oAllot.TicketNO = oRowView["BSlip#"].ToString();
                    }

                    oAllot.SoldBy = oRowView["SoldBy"].ToString();
                    oAllot.BoughtBy = oRowView["BoughtBy"].ToString();
                    oAllot.Buy_sold_Ind = Char.Parse("S");
                    oAllot.CDSellTrans = "";
                    oAllot.OtherCust = "";
                    oAllot.MarginCode = strMarginCodeForNASDIndicator;
                    oAllot.CrossType = "";
                    oAllot.PrintFlag = 'N';
                    var dbCommandAllot = oAllot.AddCommand();
                    db.ExecuteNonQuery(dbCommandAllot, transaction);
                    strAllotmentNo = db.GetParameterValue(dbCommandAllot, "Txn").ToString().Trim();

                    #endregion

                    break;
                }
            }

           
        }


        private static void GetHandleDeclarationAssignment(out ProductAcct oCust, out Customer oCustomer,
            out ProductAcct oCustCross, out Customer oCustomerCross, out decimal decFeeSec, out decimal decFeeStampDuty,
            out decimal decFeeCommission, out decimal decFeeVAT, out decimal decFeeCSCS, out decimal decSMSAlertCSCS,
            out decimal decFeeNSE, out decimal decFeeSecVat, out decimal decFeeNseVat, out decimal decFeeCscsVat,
            out decimal decSMSAlertCSCSVAT, out decimal decFeeTotalAmount, out decimal decAllotConsiderationBuy,
            out decimal decFeeSecSeller, out decimal decFeeStampDutySeller, out decimal decFeeCommissionSeller,
            out decimal decFeeVATSeller, out decimal decFeeCSCSSeller, out decimal decSMSAlertCSCSSeller,
            out decimal decFeeNSESeller, out decimal decFeeSecVatSeller, out decimal decFeeNseVatSeller,
            out decimal decFeeCscsVatSeller, out decimal decSMSAlertCSCSVATSeller, out decimal TotalFeeBuyer,
            out decimal TotalFeeSeller, out decimal TotalFeeSellerPost, out string strAllotmentNo2,
            out StkParam oStkbPGenTable,
            out PGenTable oPGenTable, out decimal decDeductComm, out string strBuyerTrueId,
            out string strPostCommShared, out string strStockProductAccount, out string strNASDStockProductAccount,
            out string strInvestmentNASDProductAccount, out string strAgentProductAccount,
            out string strBranchBuySellCharge, out decimal decTotalBank, out decimal decTotalBankAudit,
            out decimal decTotalSEC,
            out decimal decTotalCSCS, out decimal decTotalNSE, out decimal decTotalCommission,
            out decimal decTotalCommissionBuy,
            out decimal decTotalCommissionSell, out decimal decTotalVAT, out decimal decTotalStampDuty,
            out decimal decTotalDeductCommission, out decimal decTotalDeductCommissionBuy,
            out decimal decTotalDeductCommissionSell,
            out decimal decCrossDealTotalAmount, out decimal decTotalDirectCashSettlement,
            out decimal decTotalDirectCashSettlementBond, out string strMarginCodeForNASDIndicator,
            out GLParam oGLParamBoxSaleProfit, out GLParam oGLParamBranchBuySellCharge)
        {
            oCust = new ProductAcct();
            oCustomer = new Customer();
            oCustCross = new ProductAcct();
            oCustomerCross = new Customer();
            decFeeSec = 0;
            decFeeStampDuty = 0;
            decFeeCommission = 0;
            decFeeVAT = 0;
            decFeeCSCS = 0;
            decSMSAlertCSCS = 0;
            decFeeNSE = 0;
            decFeeSecVat = 0;
            decFeeNseVat = 0;
            decFeeCscsVat = 0;
            decSMSAlertCSCSVAT = 0;
            decFeeTotalAmount = 0;
            decAllotConsiderationBuy = 0;

            decFeeSecSeller = 0;
            decFeeStampDutySeller = 0;
            decFeeCommissionSeller = 0;
            decFeeVATSeller = 0;
            decFeeCSCSSeller = 0;
            decSMSAlertCSCSSeller = 0;
            decFeeNSESeller = 0;
            decFeeSecVatSeller = 0;
            decFeeNseVatSeller = 0;
            decFeeCscsVatSeller = 0;
            decSMSAlertCSCSVATSeller = 0;

            TotalFeeBuyer = 0;
            TotalFeeSeller = 0;
            TotalFeeSellerPost = 0;
            strAllotmentNo2 = "";

            oStkbPGenTable = new StkParam();
            oPGenTable = new PGenTable();
            if (!oPGenTable.GetPGenTable())
            {
            }

            if (!oStkbPGenTable.GetStkbPGenTable())
            {
            }

            decDeductComm=0;
             strBuyerTrueId = "";
              strStockProductAccount = oStkbPGenTable.Product;
              strNASDStockProductAccount = oStkbPGenTable.ProductNASDAccount;
              strInvestmentNASDProductAccount = oStkbPGenTable.ProductInvestmentNASD;
              strAgentProductAccount = oStkbPGenTable.ProductBrokPay;


             //Commission To Share Or Not
             var oGLParam = new GLParam
            {
                Type = "POSTCOMMSHARED"
            };
             strPostCommShared = oGLParam.CheckParameter();

            //For Box Load To Calculate Profit/Loss On Sale
             oGLParamBoxSaleProfit = new GLParam
            {
                Type = "CALCPROPGAINORLOSS"
            };

            //Branch Buy And Sell Charges
             oGLParamBranchBuySellCharge = new GLParam
            {
                Type = "BRANCHBUYSELLCHARGESDIFFERENT"
            };
            strBranchBuySellCharge = oGLParamBranchBuySellCharge.CheckParameter();

             decTotalBank = 0;
             decTotalBankAudit = 0;
             decTotalSEC = 0;
             decTotalCSCS = 0;
             decTotalNSE = 0;
             decTotalCommission = 0;
             decTotalCommissionBuy = 0;
             decTotalCommissionSell = 0;
             decTotalVAT = 0;
             decTotalStampDuty = 0;
             decTotalDeductCommission = 0;
             decTotalDeductCommissionBuy = 0;
             decTotalDeductCommissionSell = 0;
             decCrossDealTotalAmount = 0;
             decTotalDirectCashSettlement = 0;
             decTotalDirectCashSettlementBond = 0;
             strMarginCodeForNASDIndicator = "NASDOTC";
        }

        private  ResponseResult HandleBargainSlipNasd(BargainSlipNASD oBargainSlipNasdPre, DateTime tradeDate,  string strUserName, 
            string companyCode)
        {
            var oDsPre = oBargainSlipNasdPre.GetAll();
            var tabPre = oDsPre.Tables[0];
            var rowPre = tabPre.Select();
            if (rowPre[0]["Date"].ToString().ToDate() != tradeDate.ToExact())
                return ResponseResult.Error("Posting Aborted! Date On The Trade To Upload Not The Same with Date Selected");

            var oAutoDateNasdPre = new AutoDateNASD
            {
                iAutoDate = tradeDate.ToExact()
            };
            if (oAutoDateNasdPre.GetAutoDateNASD())
                return ResponseResult.Error("Posting Aborted! Trade For The Date Selected Already Posted");

            foreach (DataRow oRow in tabPre.Rows)
            {
                if (oRow["Date"].ToString().ToDate() !=
                    tradeDate.ToExact())
                    return ResponseResult.Error("Posting Aborted! Unequal Date(s) In The Trade To Upload/Post");
            }

            var portNotEnough = oBargainSlipNasdPre.ChkPortfolioStockEnoughToSell(strUserName.Trim(), tabPre);
            if (portNotEnough == 1)
            {
            }
            else if (portNotEnough == 2)
                return ResponseResult.Error("Posting Aborted! Error Deleting Portfolio Processing Table");
            else if (portNotEnough == 3)
                return ResponseResult.Error( "Posting Aborted! Error Inserting To Portfolio Processing Table");
            else
            {
                var oPortNot = new PortNot();
                var filePath = GetMissingFilePath("PortfolioNotEnough", companyCode);
                using (var writerPortNot = File.CreateText(filePath))
                {
                    writerPortNot.WriteLine(
                        "Stock Not Enough In Customer Portfolio, Please Correct Before Uploading Your Daily Trade");
                    foreach (DataRow drPortNot in oPortNot.GetAll().Tables[0].Rows)
                    {
                        var builderPortNot = new StringBuilder(1000); // None row is bigger than this
                        foreach (var objPortNot in drPortNot.ItemArray)
                        {
                            builderPortNot.Append(objPortNot);
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

            return ResponseResult.Success();
        }
    }
}
               
 