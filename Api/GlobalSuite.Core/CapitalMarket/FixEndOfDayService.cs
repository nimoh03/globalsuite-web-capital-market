using System;
using System.Data;
using System.Data.SqlClient;
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
        public async Task<ResponseResult> RunEndOfDayForFix(DateTime unPostedDate)
        {
             
            return await Task.Run(() =>
            {
                try
                {
                     #region Check That All Reconciliation Is Done
                    FIXReconcile oFIXReconcile = new FIXReconcile();
                    if (!oFIXReconcile.ChkGLReconcile(unPostedDate.ToExact()))
                        return ResponseResult.Error("Reconciliation Aborted! GL Reconciliation Not Yet Done");
                    if (!oFIXReconcile.ChkAllotmentReconcile(unPostedDate.ToExact()))
                        return ResponseResult.Error( "Reconciliation Aborted! Trade Reconciliation Not Yet Done");
                    if (!oFIXReconcile.ChkPortfolioReconcile(unPostedDate.ToExact()))
                        return ResponseResult.Error("Reconciliation Aborted! Portfolio Reconciliation Not Yet Done");
                    if (!oFIXReconcile.ChkTradeFileReconcile(unPostedDate.ToExact()))
                        return ResponseResult.Error( "Reconciliation Aborted! Trade File From NSE Reconciliation Not Yet Done");
                    #endregion

                    
                    #region Check Date Already Posted
                    AutoDate oAutoDatePre = new AutoDate();
                    oAutoDatePre.iAutoDate = unPostedDate.ToExact();
                    if (oAutoDatePre.GetAutoDate())
                        return ResponseResult.Error( "Posting Aborted! Trade For The Date Selected Already Posted");
                    #endregion

                    #region Check Time Is Within Market Period
                    GLParam oGLParamTradeBookAddGloSuite = new GLParam();
                    oGLParamTradeBookAddGloSuite.Type = "TRADEBOOKADDTOGLOBALSUITE";
                    string strTradeBookAddGloSuite = oGLParamTradeBookAddGloSuite.CheckParameter();
                    if (strTradeBookAddGloSuite.Trim() == "YES")
                    {
                        int intTimeHour = GeneralFunc.GetTodayTimeHour();
                        if (intTimeHour >= 9 && intTimeHour < 15)
                            return ResponseResult.Error( "Cannot Run Reconcillation Of Trades Yet! Time Within Market Trading Time of 9AM - 3PM");
                    }
                    #endregion
                    
                 return   ProcessEndOfTradingFix(unPostedDate);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return ResponseResult.Error("Error In Posting " + ex.Message.Trim());
                }
                return ResponseResult.Success();
            });
        }

        private ResponseResult ProcessEndOfTradingFix(DateTime date)
        {
             #region Declaration of Property
            Allotment oAllotment = new Allotment();
            Branch oBranch = new Branch();
            DataTable tabFIXAllotments;
            string strJnumberNext = "";
            string strPostCommShared = "";
            decimal decDeductComm;
            decimal decDeductCommJobbing;

            decimal decTotalAllotConsiderationBuy = 0;
            decimal decTotalAllotConsiderationSell = 0;
            decimal decTotalSEC = 0;
            decimal decTotalCSCS = 0;
            decimal decTotalNSE = 0;
            decimal decTotalCommissionBuy = 0;
            decimal decTotalCommissionSell = 0;
            decimal decTotalVAT = 0;
            decimal decTotalStampDuty = 0;
            decimal decTotalDeductCommission = 0;
            decimal decTotalDeductCommissionBuy = 0;
            decimal decTotalDeductCommissionSell = 0;
            decimal decTotalDirectCashSettlement = 0;
            decimal decTotalNetConsiderationForSellAndBuy = 0;
            string strDefaultBranchCode = oBranch.DefaultBranch;


            //Commission To Share Or Not
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "POSTCOMMSHARED";
            strPostCommShared = oGLParam.CheckParameter();

            GLParam oGLParamBoxSaleProfit = new GLParam();
            oGLParamBoxSaleProfit.Type = "CALCPROPGAINORLOSS";
            string strBoxLoadSaleProfit = oGLParamBoxSaleProfit.CheckParameter();

            GLParam oGLParamPostInvPLControlAcct = new GLParam();
            oGLParamPostInvPLControlAcct.Type = "POSTINVPLCTRLSECACCT";
            string strPostInvPLControlAcct = oGLParamPostInvPLControlAcct.CheckParameter();

            StkParam oStkbPGenTable = new StkParam();
            PGenTable oPGenTable = new PGenTable();
            if (!oPGenTable.GetPGenTable())
            {
                throw new Exception("Account Parameter Setup Not Available");
            }
            if (!oStkbPGenTable.GetStkbPGenTable())
            {
                throw new Exception("Fees Parameter Setup Not Available");
            }

            string strStockProductAccount = oStkbPGenTable.Product;
            string strInvestmentProductAccount = oStkbPGenTable.ProductInvestment;
            string strAgentProductAccount = oStkbPGenTable.ProductBrokPay;

            #endregion


            tabFIXAllotments = oAllotment.GetAllFIXBuySell(date.ToExact()).Tables[0];
            if (tabFIXAllotments.Rows.Count > 0)
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        AcctGL oGL = new AcctGL();
                        CustomerExtraInformation oCustomerExtraInformationDirectCash = new CustomerExtraInformation();
                        ProductAcct oProductAcct = new ProductAcct();
                        Product oProduct = new Product();
                        Customer oCustomer = new Customer();
                        SqlCommand oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                        db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                        db.ExecuteNonQuery(oCommandJnumber, transaction);
                        strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();
                        foreach (DataRow oRowView in tabFIXAllotments.Rows)
                        {
                            #region Check Customer Exist
                            oCustomer.CustAID = oRowView["CustAID"].ToString().Trim();
                            if (!oCustomer.GetCustomerNameWithoutProduct())
                            {
                                throw new Exception("Missing Customer Account");
                            }
                            #endregion

                            #region Calculate Box Load Capital Gain
                            if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                            {
                                oProductAcct.ProductCode = strStockProductAccount;
                                oProductAcct.CustAID = oRowView["CustAID"].ToString().Trim();
                                if (oProductAcct.GetBoxLoadStatus())
                                {
                                    decimal decGetUnitCost = 0;
                                    decimal decActualSellPrice = 0;
                                    decimal decSellDiff = 0;
                                    decimal decSellDiffTotal = 0;

                                    Portfolio oPortForSaleProfit = new Portfolio();
                                    oPortForSaleProfit.PurchaseDate = oRowView["DateAlloted"].ToString().Trim().ToDate();
                                    oPortForSaleProfit.CustomerAcct = oRowView["CustAID"].ToString().Trim();
                                    oPortForSaleProfit.StockCode = oRowView["Stockcode"].ToString().Trim();
                                    decGetUnitCost = decimal.Parse(oPortForSaleProfit.GetUnitCostExcludeLaterTransaction(long.Parse(oRowView["Txn#"].ToString().Trim())).ToString());
                                    decGetUnitCost = Math.Round(decGetUnitCost, 2, MidpointRounding.AwayFromZero);
                                    decActualSellPrice = decimal.Parse(oRowView["TotalAmt"].ToString().Trim()) / decimal.Parse(oRowView["Qty"].ToString().Trim());
                                    decActualSellPrice = Math.Round(decActualSellPrice, 2);
                                    decSellDiff = decActualSellPrice - decGetUnitCost;
                                    decSellDiff = Math.Round(decSellDiff, 2, MidpointRounding.AwayFromZero);
                                    decSellDiffTotal = decSellDiff * decimal.Parse(oRowView["Qty"].ToString().Trim());
                                    decSellDiffTotal = Math.Round(decSellDiffTotal, 2, MidpointRounding.AwayFromZero);

                                    if ((decSellDiffTotal != 0) && (strBoxLoadSaleProfit.Trim() == "YES"))
                                    {
                                        #region Capital Gain Posting Of Income Account
                                        oGL.EffectiveDate =oRowView["DateAlloted"].ToString().Trim().ToDate();
                                        oGL.MasterID = oPGenTable.CapGain;
                                        oGL.AccountID = "";
                                        if (decSellDiffTotal < 0)
                                        {
                                            oGL.Credit = 0;
                                            oGL.Debit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                                            oGL.Debcred = "D";
                                            oGL.Desciption = "Cap Loss Appr. from Sale of: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") + " SP @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                        }
                                        else
                                        {
                                            oGL.Credit = Math.Round(Math.Abs((decimal)decSellDiffTotal), 2);
                                            oGL.Debit = 0;
                                            oGL.Debcred = "C";
                                            oGL.Desciption = "Cap Gain Appr. from Sale of: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " CP @ " + decGetUnitCost.ToString("n") + " SP @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                        }
                                        oGL.FeeType = "SGIV";
                                        oGL.TransType = "STKBSALE";
                                        oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
                                        oGL.Ref01 = oRowView["Txn#"].ToString().Trim();
                                        oGL.Ref02 = oRowView["CustAID"].ToString().Trim();
                                        oGL.AcctRef = "";
                                        oGL.Reverse = "N";
                                        oGL.Jnumber = strJnumberNext;
                                        oGL.Branch = strDefaultBranchCode;
                                        oGL.Chqno = ""; oGL.RecAcctMaster = oPGenTable.ShInv; oGL.RecAcct = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                        SqlCommand dbCommandSellProfitOrLoss = oGL.AddCommandFIX("FIX");
                                        db.ExecuteNonQuery(dbCommandSellProfitOrLoss, transaction);
                                        #endregion

                                        #region Capital Gain Posting To Investment Account Or Gain Contra Account
                                        oGL.EffectiveDate =oRowView["DateAlloted"].ToString().Trim().ToDate();
                                        if (strPostInvPLControlAcct.Trim() != "YES")
                                        {
                                            oGL.AccountID = oRowView["CustAID"].ToString().Trim();
                                            oProduct.TransNo = strInvestmentProductAccount;
                                            oGL.MasterID = oProduct.GetProductGLAcct();
                                            oGL.AcctRef = strInvestmentProductAccount;
                                        }
                                        else
                                        {
                                            oGL.AccountID = "";
                                            oGL.MasterID = oPGenTable.CapGainContra;
                                            oGL.AcctRef = "";
                                        }
                                        oGL.FeeType = "SGIB";
                                        oGL.TransType = "STKBSALE";
                                        oGL.Desciption = "Stock Sale P/L: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                        oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
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
                                        oGL.Ref01 = oRowView["Txn#"].ToString().Trim();
                                        oGL.Ref02 = "";
                                        oGL.Reverse = "N";
                                        oGL.Jnumber = strJnumberNext;
                                        oGL.Branch = strDefaultBranchCode;
                                        oGL.Chqno = ""; oGL.RecAcctMaster = oPGenTable.ShInv; oGL.RecAcct = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                        SqlCommand dbCommandBoxLoadProfitLoss = oGL.AddCommandFIX("FIX");
                                        db.ExecuteNonQuery(dbCommandBoxLoadProfitLoss, transaction);
                                        #endregion
                                    }
                                }
                            }
                            #endregion

                            #region Posting To Trade Upload Control Account For PostShared Is NO
                            decDeductCommJobbing = 0;
                            if (strPostCommShared.Trim() == "NO")
                            {
                                ProductAcct oProductAcctAgentJobbing = new ProductAcct();
                                oProductAcctAgentJobbing.ProductCode = strStockProductAccount;
                                oProductAcctAgentJobbing.CustAID = oRowView["CustAID"].ToString().Trim();
                                if (oProductAcctAgentJobbing.GetCustomerAgent())
                                {
                                    Customer oCustomerAgentJobbing = new Customer();
                                    oProductAcctAgentJobbing.ProductCode = strAgentProductAccount;
                                    oProductAcctAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                                    oCustomerAgentJobbing.CustAID = oProductAcctAgentJobbing.Agent.Trim();
                                    if (oProductAcctAgentJobbing.GetAgentCommission())
                                    {
                                        if (oCustomerAgentJobbing.GetCustomerName(strAgentProductAccount))
                                        {
                                            if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                            {
                                                decDeductCommJobbing = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgentJobbing.AgentComm) / 100;
                                            }
                                            else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                                            {
                                                decDeductCommJobbing = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgentJobbing.AgentComm) / 100;
                                            }
                                            decDeductCommJobbing = Math.Round(decDeductCommJobbing, 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                }
                            }
                            oGL.EffectiveDate =oRowView["DateAlloted"].ToString().Trim().ToDate();
                            oGL.MasterID = oPGenTable.ShInv;
                            oGL.AccountID = "";
                            oGL.RecAcct = "";
                            oGL.RecAcctMaster = oPGenTable.ShInv;
                            if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                            {
                                oGL.Debit = decDeductCommJobbing;
                                oGL.Credit = 0;
                                oGL.Debcred = "D";
                                oGL.Desciption = "Stock Purchase: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n") + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                oGL.TransType = "STKBBUY";
                                oGL.SysRef = "BSB" + "-" + oRowView["Txn#"].ToString();
                            }
                            else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                            {
                                oGL.Credit = 0;
                                oGL.Debit = decDeductCommJobbing;
                                oGL.Debcred = "D";
                                oGL.Desciption = "Stock Sale: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n") + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                oGL.TransType = "STKBSALE";
                                oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString();
                            }
                            oGL.Ref01 = oRowView["Txn#"].ToString(); 
                            oGL.Ref02 = oRowView["CustAID"].ToString().Trim();
                            oGL.Reverse = "N";
                            oGL.Jnumber = strJnumberNext;
                            oGL.Branch = strDefaultBranchCode;
                            oGL.InstrumentType = DataGeneral.GLInstrumentType.C; oGL.AcctRef = ""; oGL.Chqno = "";
                            oGL.FeeType = "CTRAD";
                            SqlCommand dbCommandJobbingNormal = oGL.AddCommand();
                            db.ExecuteNonQuery(dbCommandJobbingNormal, transaction);
                            #endregion

                            #region Agent Posting Buyer
                            ProductAcct oProductAcctAgent = new ProductAcct();
                            oProductAcctAgent.ProductCode = strStockProductAccount;
                            oProductAcctAgent.CustAID = oRowView["CustAID"].ToString().Trim();
                            if (oProductAcctAgent.GetCustomerAgent())
                            {
                                decDeductComm = 0;
                                Customer oCustomerAgent = new Customer();

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
                                                decDeductComm = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgent.AgentComm) / 100;
                                                decTotalDeductCommissionBuy = decTotalDeductCommissionBuy + decDeductComm;
                                                decTotalDeductCommissionBuy = Math.Round(decTotalDeductCommissionBuy, 2, MidpointRounding.AwayFromZero);
                                            }
                                            else
                                            {
                                                decDeductComm = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgent.AgentComm) / 100;
                                                decTotalDeductCommissionSell = decTotalDeductCommissionSell + decDeductComm;
                                                decTotalDeductCommissionSell = Math.Round(decTotalDeductCommissionSell, 2, MidpointRounding.AwayFromZero);
                                            }
                                            decDeductComm = Math.Round(decDeductComm, 2, MidpointRounding.AwayFromZero);
                                            decTotalDeductCommission = decTotalDeductCommission + decDeductComm;
                                            decTotalDeductCommission = Math.Round(decTotalDeductCommission, 2, MidpointRounding.AwayFromZero);
                                        }

                                        oGL.EffectiveDate =oRowView["DateAlloted"].ToString().Trim().ToDate();
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
                                            oGL.Debit = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgent.AgentComm) / 100;
                                        }
                                        else
                                        {
                                            oGL.Debit = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgent.AgentComm) / 100;
                                        }
                                        oGL.Debit = Math.Round(oGL.Debit, 2, MidpointRounding.AwayFromZero);
                                        oGL.Debcred = "D";
                                        if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                        {
                                            oGL.FeeType = "BECM";
                                            if ((Char.Parse(oRowView["CDeal"].ToString().Trim()) == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
                                            {
                                                oGL.Desciption = "Stk Purc.Comm For: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                            }
                                            else
                                            {
                                                //This is where you changed customer cross to customer because you were to lazy to do cross deals
                                                oGL.Desciption = "Cross Deal Purc.Comm.: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                            }
                                            oGL.TransType = "STKBBUY";
                                            oGL.SysRef = "BSB" + "-" + oRowView["Txn#"].ToString().Trim();
                                        }
                                        else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                                        {
                                            oGL.FeeType = "SECM";
                                            oGL.TransType = "STKBSALE";
                                            oGL.Desciption = "Stk Sale Comm: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                            oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
                                        }
                                        oGL.Ref01 = oRowView["Txn#"].ToString().Trim();
                                        oGL.AcctRef = "";
                                        oGL.Reverse = "N";
                                        oGL.Jnumber = strJnumberNext;
                                        oGL.Branch = strDefaultBranchCode;
                                        oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oPGenTable.ShInv; oGL.RecAcct = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                        SqlCommand dbCommandAgentExpense = oGL.AddCommand();
                                        db.ExecuteNonQuery(dbCommandAgentExpense, transaction);


                                        oGL.EffectiveDate =oRowView["DateAlloted"].ToString().Trim().ToDate();
                                        oGL.AccountID = oProductAcctAgent.CustAID;
                                        oProduct.TransNo = strAgentProductAccount;
                                        oGL.MasterID = oProduct.GetProductGLAcct();
                                        oGL.AcctRef = strAgentProductAccount;
                                        if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                        {
                                            oGL.Credit = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgent.AgentComm) / 100;
                                        }
                                        else
                                        {
                                            oGL.Credit = (Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0") * oProductAcctAgent.AgentComm) / 100;
                                        }
                                        oGL.Credit = Math.Round(oGL.Credit, 2, MidpointRounding.AwayFromZero);
                                        oGL.Debit = 0;
                                        oGL.Debcred = "C";
                                        if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                                        {
                                            oGL.FeeType = "BBPY";
                                            if ((Char.Parse(oRowView["CDeal"].ToString().Trim()) == 'N') || (oRowView["CrossType"].ToString().Trim() == "NO"))
                                            {
                                                oGL.Desciption = "Stock Purchase: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                            }
                                            else
                                            {
                                                //This is where you changed customer cross to customer because you were to lazy to do cross deals
                                                oGL.Desciption = "Cross Deal Pur.: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                            }
                                            oGL.TransType = "STKBBUY";
                                            oGL.SysRef = "BSB" + "-" + oRowView["Txn#"].ToString().Trim();
                                        }
                                        else if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                                        {
                                            oGL.FeeType = "SBPY";
                                            oGL.TransType = "STKBSALE";
                                            oGL.Desciption = "Stock Sale: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString()).ToString("n").Trim() + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                            oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
                                        }
                                        oGL.Ref01 = oRowView["Txn#"].ToString().Trim();
                                        oGL.Reverse = "N";
                                        oGL.Jnumber = strJnumberNext;
                                        oGL.Branch = strDefaultBranchCode;
                                        oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oPGenTable.ShInv; oGL.RecAcct = ""; oGL.InstrumentType = DataGeneral.GLInstrumentType.C;
                                        SqlCommand dbCommandBrokPayable = oGL.AddCommand();
                                        db.ExecuteNonQuery(dbCommandBrokPayable, transaction);
                                    }
                                }
                            }
                            #endregion

                            #region Procesing Total Consideration and Fees
                            if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "B")
                            {
                                decTotalAllotConsiderationBuy = Math.Round(decTotalAllotConsiderationBuy, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["Consideration"].ToString().Trim());
                                decTotalAllotConsiderationBuy = Math.Round(decTotalAllotConsiderationBuy, 2, MidpointRounding.AwayFromZero);

                                decTotalCommissionBuy = Math.Round(decTotalCommissionBuy, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0");
                                decTotalCommissionBuy = Math.Round(decTotalCommissionBuy, 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                decTotalAllotConsiderationSell = Math.Round(decTotalAllotConsiderationSell, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["Consideration"].ToString().Trim());
                                decTotalAllotConsiderationSell = Math.Round(decTotalAllotConsiderationSell, 2, MidpointRounding.AwayFromZero);

                                decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["Commission"] != null && oRowView["Commission"].ToString().Trim() != "" ? oRowView["Commission"].ToString().Trim() : "0");
                                decTotalCommissionSell = Math.Round(decTotalCommissionSell, 2, MidpointRounding.AwayFromZero);
                            }
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["SecFee"] != null && oRowView["SecFee"].ToString().Trim() != "" ? oRowView["SecFee"].ToString().Trim() : "0");
                            decTotalSEC = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);

                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["StampDuty"] != null && oRowView["StampDuty"].ToString().Trim() != "" ? oRowView["StampDuty"].ToString().Trim() : "0");
                            decTotalStampDuty = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);

                            decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["VAT"] != null && oRowView["VAT"].ToString().Trim() != "" ? oRowView["VAT"].ToString().Trim() : "0");
                            decTotalVAT = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);

                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["NSEFee"] != null && oRowView["NSEFee"].ToString().Trim() != "" ? oRowView["NSEFee"].ToString().Trim() : "0");
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);

                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["CSCSFee"] != null && oRowView["CSCSFee"].ToString().Trim() != "" ? oRowView["CSCSFee"].ToString().Trim() : "0");
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["NSEVat"] != null && oRowView["NSEVat"].ToString().Trim() != "" ? oRowView["NSEVat"].ToString().Trim() : "0");
                            decTotalNSE = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);

                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["CSCSVat"] != null && oRowView["CSCSVat"].ToString().Trim() != "" ? oRowView["CSCSVat"].ToString().Trim() : "0");
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["SMSAlertCSCS"] != null && oRowView["SMSAlertCSCS"].ToString().Trim() != "" ? oRowView["SMSAlertCSCS"].ToString().Trim() : "0");
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);

                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Decimal.Parse(oRowView["SMSAlertCSCSVAT"] != null && oRowView["SMSAlertCSCSVAT"].ToString().Trim() != "" ? oRowView["SMSAlertCSCSVAT"].ToString().Trim() : "0");
                            decTotalCSCS = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
                            #endregion

                            #region Direct Cash Settlement
                            oCustomerExtraInformationDirectCash.CustAID = oRowView["CustAID"].ToString().Trim();
                            oCustomerExtraInformationDirectCash.WedAnniversaryDate = oRowView["Date"].ToString().Trim().ToDate();
                            if (oCustomerExtraInformationDirectCash.DirectCashSettlement && (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S"))
                            {
                                #region Customer Posting For Direct Cash Settlement
                                oGL.EffectiveDate =oRowView["DateAlloted"].ToString().Trim().ToDate();
                                oGL.AccountID = oRowView["CustAID"].ToString().Trim();
                                oProductAcct.CustAID = oRowView["CustAID"].ToString().Trim();
                                if (oProductAcct.GetBoxLoadStatus())
                                {
                                    oProduct.TransNo = strInvestmentProductAccount;
                                    oGL.MasterID = oProduct.GetProductGLAcct();
                                    oGL.AcctRef = strInvestmentProductAccount;
                                }
                                else
                                {
                                    oProduct.TransNo = strStockProductAccount;
                                    oGL.MasterID = oProduct.GetProductGLAcct();
                                    oGL.AcctRef = strStockProductAccount;
                                }
                                oGL.Debit = Decimal.Parse(oRowView["TotalAmt"].ToString().Trim());
                                oGL.Credit = 0;
                                oGL.Debcred = "D";
                                oGL.FeeType = "SCUMDCS";
                                oGL.Desciption = "Direct Cash Settlement For Sale: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString().Trim()).ToString("n");
                                oGL.TransType = "STKBSALE";
                                oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.Q;
                                oGL.Ref01 = oRowView["Txn#"].ToString().Trim();
                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Branch = strDefaultBranchCode;
                                oGL.Ref02 = ""; oGL.Chqno = ""; oGL.RecAcctMaster = oPGenTable.DirectCashSettleAcct; oGL.RecAcct = ""; oGL.PostToOtherBranch = "Y"; oGL.ClearingDayForTradingTransaction = "Y";
                                SqlCommand dbCommandDirectCashSettlementCustomer = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandDirectCashSettlementCustomer, transaction);

                                oGL.PostToOtherBranch = "N"; oGL.ClearingDayForTradingTransaction = "N"; // So that others will not inherit this powerful privelege
                                #endregion

                                #region Second Leg To Direct Cash Settlement Account For Direct Cash Settlement
                                oGL.EffectiveDate =oRowView["DateAlloted"].ToString().Trim().ToDate();
                                oGL.MasterID = oPGenTable.DirectCashSettleAcct;
                                oGL.AccountID = "";
                                if (oProductAcct.GetBoxLoadStatus())
                                {
                                    oProduct.TransNo = strInvestmentProductAccount;
                                    oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                                    oGL.AcctRefSecond = strInvestmentProductAccount;
                                }
                                else
                                {
                                    oProduct.TransNo = strStockProductAccount;
                                    oGL.RecAcctMaster = oProduct.GetProductGLAcct();
                                    oGL.AcctRefSecond = strStockProductAccount;
                                }
                                oGL.RecAcct = oRowView["CustAID"].ToString().Trim();
                                oGL.Debit = 0;
                                oGL.Credit = Decimal.Parse(oRowView["TotalAmt"].ToString().Trim());
                                oGL.Debcred = "C";
                                oGL.Desciption = "Direct Cash Settlement For Sale: " + oRowView["Qty"].ToString().Trim() + " " + oRowView["StockCode"].ToString().Trim() + " @ " + decimal.Parse(oRowView["UnitPrice"].ToString().Trim()).ToString("n") + " By " + oCustomer.Surname.Trim() + " " + oCustomer.Firstname.Trim() + " " + oCustomer.Othername.Trim();
                                oGL.TransType = "STKBSALE";
                                oGL.SysRef = "BSS" + "-" + oRowView["Txn#"].ToString().Trim();
                                oGL.Ref01 = oRowView["Txn#"].ToString().Trim();
                                oGL.Ref02 = oRowView["CustAID"].ToString().Trim();
                                oGL.Reverse = "N";
                                oGL.Jnumber = strJnumberNext;
                                oGL.Branch = strDefaultBranchCode;
                                oGL.InstrumentType = DataGeneral.GLInstrumentType.C; oGL.AcctRef = ""; oGL.Chqno = "";
                                oGL.FeeType = "CTRADDCS";
                                SqlCommand dbCommandJobbingNormalDirectCashSettlement = oGL.AddCommand();
                                db.ExecuteNonQuery(dbCommandJobbingNormalDirectCashSettlement, transaction);
                                #endregion

                                decTotalDirectCashSettlement = decTotalDirectCashSettlement + Decimal.Parse(oRowView["TotalAmt"].ToString().Trim());
                                decTotalDirectCashSettlement = Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);

                            }
                            #endregion

                            #region Job Order Processing
                            JobOrder oJob = new JobOrder();
                            DataSet oDsJob = new DataSet();
                            oJob.CustNo = oRowView["CustAID"].ToString().Trim();
                            oJob.StockCode = oRowView["StockCode"].ToString().Trim();
                            if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                            {
                                oDsJob = oJob.GetUnProcSellGivenCustStock();
                            }
                            else
                            {
                                oDsJob = oJob.GetUnProcBuyGivenCustStock();
                            }

                            DataTable thisTableJob = oDsJob.Tables[0];
                            int iLeft = 0;
                            foreach (DataRow oRowViewJob in thisTableJob.Rows)
                            {
                                if ((int.Parse(oRowView["Qty"].ToString().Trim()) - iLeft) > int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                                {
                                    iLeft = iLeft + int.Parse(oRowViewJob["Balance"].ToString().Trim());
                                    SqlCommand dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(), int.Parse(oRowViewJob["Balance"].ToString().Trim()));
                                    db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                                    SqlCommand dbCommandJobBalance = oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(), oRowView["Txn#"].ToString().Trim());
                                    db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                                }
                                else if ((int.Parse(oRowView["Qty"].ToString().Trim()) - iLeft) == int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                                {
                                    iLeft = iLeft + int.Parse(oRowViewJob["Balance"].ToString().Trim());
                                    SqlCommand dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(), int.Parse(oRowViewJob["Balance"].ToString().Trim()));
                                    db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                                    SqlCommand dbCommandJobBalance = oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(), oRowView["Txn#"].ToString().Trim());
                                    db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                                    break;
                                    //Exit Do
                                }
                                else if ((int.Parse(oRowView["Qty"].ToString().Trim()) - iLeft) < int.Parse(oRowViewJob["Balance"].ToString().Trim()))
                                {
                                    SqlCommand dbCommandJobProcessed = oJob.UpdateProcessedCommand(oRowViewJob["Transno"].ToString().Trim(), int.Parse(oRowView["Qty"].ToString().Trim()) - iLeft);
                                    db.ExecuteNonQuery(dbCommandJobProcessed, transaction);
                                    SqlCommand dbCommandJobBalance = oJob.UpdateBalanceCommand(oRowViewJob["Transno"].ToString().Trim(), oRowView["Txn#"].ToString().Trim(), int.Parse(oRowView["Qty"].ToString().Trim()) - iLeft);
                                    db.ExecuteNonQuery(dbCommandJobBalance, transaction);
                                    break;
                                    //Exit Do
                                }
                            }
                            #endregion

                        }

                        decTotalNetConsiderationForSellAndBuy = decTotalAllotConsiderationSell - decTotalAllotConsiderationBuy;

                        GeneralFunc oGeneralFunc = new GeneralFunc();
                        string strSettlementDate = oGeneralFunc.AddBusinessDay(date.ToExact(), oGLParam.TradingClearingDay, Holiday.GetAllReturnList()).ToString("d");

                        #region Summation Of Fees And Control Account Posting
                        AcctGL oGLTradBank = new AcctGL();

                        #region Credit Buy/ Sell Income,VAT Account And Debit Trading Control

                        #region Credit Income Buy And Debit Jobbing Control
                        if (decTotalCommissionBuy != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.Bcncomm;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            oGLTradBank.Desciption = "Commission Total Buy Charges For: " + date.ToExact();
                            decimal decNetCommissionBuy = decTotalCommissionBuy - decTotalDeductCommissionBuy;
                            oGLTradBank.Credit = Math.Round(decNetCommissionBuy, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TCOMCB";
                            SqlCommand dbCommandCommissionTotalBuyCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandCommissionTotalBuyCredit, transaction);


                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.ShInv;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.Bcncomm;
                            oGLTradBank.Desciption = "Commission Total Buy Charges For: " + date.ToExact();
                            decNetCommissionBuy = decTotalCommissionBuy - decTotalDeductCommissionBuy;
                            oGLTradBank.Debit = Math.Round(decNetCommissionBuy, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Credit = 0;
                            oGLTradBank.Debcred = "D";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSTCM";
                            SqlCommand dbCommandCommissionTotalCreditJobbingBuy = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandCommissionTotalCreditJobbingBuy, transaction);
                        }
                        #endregion

                        #region Credit Income Sell And Debit Jobbing Control
                        if (decTotalCommissionSell != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.Scncomm;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            oGLTradBank.Desciption = "Commission Total Sell Charges For: " + date.ToExact();
                            decimal decNetCommissionSell = decTotalCommissionSell - decTotalDeductCommissionSell;
                            oGLTradBank.Credit = Math.Round(decNetCommissionSell, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TCOMCS";
                            SqlCommand dbCommandCommissionTotalSellCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandCommissionTotalSellCredit, transaction);

                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.ShInv;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.Scncomm;
                            oGLTradBank.Desciption = "Commission Total Sell Charges For: " + date.ToExact();
                            decNetCommissionSell = decTotalCommissionSell - decTotalDeductCommissionSell;
                            oGLTradBank.Debit = Math.Round(decNetCommissionSell, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Credit = 0;
                            oGLTradBank.Debcred = "D";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSTCM";
                            SqlCommand dbCommandCommissionTotalCreditJobbing = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandCommissionTotalCreditJobbing, transaction);
                        }
                        #endregion

                        #region Credit Stamp Duty And Debit Jobbing Control
                        //if (decTotalStampDuty != 0)
                        //{
                        //oGLTradBank.EffectiveDate = date.ToExact();
                        //oGLTradBank.MasterID = oPGenTable.Bstamp;
                        //oGLTradBank.AccountID = "";
                        //oGLTradBank.RecAcct = "";
                        //oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                        //oGLTradBank.Desciption = "Stamp Duty Total Charges For: " + date.ToExact();
                        //oGLTradBank.Credit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                        //oGLTradBank.Debit = 0;
                        //oGLTradBank.Debcred = "C";
                        //oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                        //oGLTradBank.TransType = "TRADBANK";
                        //oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                        //oGLTradBank.Reverse = "N";
                        //oGLTradBank.Jnumber = strJnumberNext;
                        //oGLTradBank.Branch = strDefaultBranchCode;
                        //oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                        //oGLTradBank.FeeType = "TSTDC";
                        //SqlCommand dbCommandStampTotalCredit = oGLTradBank.AddCommandFIX("FIX");
                        //db.ExecuteNonQuery(dbCommandStampTotalCredit, transaction);


                        //oGLTradBank.EffectiveDate = date.ToExact();
                        //oGLTradBank.MasterID = oPGenTable.ShInv;
                        //oGLTradBank.AccountID = "";
                        //oGLTradBank.RecAcct = "";
                        //oGLTradBank.RecAcctMaster = oPGenTable.Bstamp;
                        //oGLTradBank.Desciption = "Stamp Duty Total Charges For: " + date.ToExact();
                        //oGLTradBank.Debit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                        //oGLTradBank.Credit = 0;
                        //oGLTradBank.Debcred = "D";
                        //oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                        //oGLTradBank.TransType = "TRADBANK";
                        //oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                        //oGLTradBank.Reverse = "N";
                        //oGLTradBank.Jnumber = strJnumberNext;
                        //oGLTradBank.Branch = strDefaultBranchCode;
                        //oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                        //oGLTradBank.FeeType = "TSTCS";
                        //SqlCommand dbCommandStampTotalCreditJobbing = oGLTradBank.AddCommandFIX("FIX");
                        //db.ExecuteNonQuery(dbCommandStampTotalCreditJobbing, transaction);
                        //}
                        #endregion

                        #region Credit VAT and Debit Jobbing Control
                        if (decTotalVAT != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.Bvat;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            oGLTradBank.Desciption = "VAT Total Charges For: " + date.ToExact();
                            oGLTradBank.Credit = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TVATC";
                            SqlCommand dbCommandVATTotalCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandVATTotalCredit, transaction);

                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.ShInv;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.Bvat;
                            oGLTradBank.Desciption = "VAT Total Charges For: " + date.ToExact();
                            oGLTradBank.Debit = Math.Round(decTotalVAT, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Credit = 0;
                            oGLTradBank.Debcred = "D";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSTVT";
                            SqlCommand dbCommandVATTotalCreditJobbing = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandVATTotalCreditJobbing, transaction);
                        }
                        #endregion

                        #endregion

                        #endregion

                        #region Credit SEC,NSE,CSCS,Stamp Duty Account ONLY
                        if (decTotalSEC != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.Bsec;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            oGLTradBank.Desciption = "SEC Total Charges For: " + date.ToExact();
                            oGLTradBank.Credit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSECC";
                            SqlCommand dbCommandSecTotalCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandSecTotalCredit, transaction);
                        }

                        if (decTotalNSE != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.Bnse;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            oGLTradBank.Desciption = "NSE Total Charges For: " + date.ToExact();
                            oGLTradBank.Credit = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TNSEC";
                            SqlCommand dbCommandNseTotalCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandNseTotalCredit, transaction);
                        }

                        if (decTotalCSCS != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.Bcscs;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            oGLTradBank.Desciption = "CSCS Total Charges For: " + date.ToExact();
                            oGLTradBank.Credit = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TCSCC";
                            SqlCommand dbCommandCSCSTotalCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandCSCSTotalCredit, transaction);
                        }

                        if (decTotalStampDuty != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.Bstamp;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                            oGLTradBank.Desciption = "Stamp Duty Total Charges For: " + date.ToExact();
                            oGLTradBank.Credit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSTDYC";
                            SqlCommand dbCommandStampDutyTotalCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandStampDutyTotalCredit, transaction);
                        }
                        #endregion

                        if (decTotalNetConsiderationForSellAndBuy != 0 || decTotalNSE != 0 || decTotalCSCS != 0 || decTotalSEC != 0 || decTotalStampDuty != 0 || decTotalDirectCashSettlement != 0)
                        {
                            #region Debit SEC,NSE,CSCS,Stamp Duty Account ONLY

                            #region Debit SEC Account
                            if (decTotalSEC != 0)
                            {
                                oGLTradBank.EffectiveDate = date.ToExact();
                                oGLTradBank.MasterID = oPGenTable.Bsec;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                                oGLTradBank.Desciption = "SEC Charges Payment For: " + date.ToExact();
                                oGLTradBank.Credit = 0;
                                oGLTradBank.Debit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero);
                                oGLTradBank.Debcred = "D";
                                oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.TransType = "TRADBANK";
                                oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.Reverse = "N";
                                oGLTradBank.Jnumber = strJnumberNext;
                                oGLTradBank.Branch = strDefaultBranchCode;
                                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                                oGLTradBank.FeeType = "PSECD";
                                SqlCommand dbCommandSecStatControlDebit = oGLTradBank.AddCommandFIX("FIX");
                                db.ExecuteNonQuery(dbCommandSecStatControlDebit, transaction);
                            }
                            #endregion

                            #region Debit NSE Account
                            if (decTotalNSE != 0)
                            {
                                oGLTradBank.EffectiveDate = date.ToExact();
                                oGLTradBank.MasterID = oPGenTable.Bnse;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                                oGLTradBank.Desciption = "NSE Charges Payment For: " + date.ToExact();
                                oGLTradBank.Credit = 0;
                                oGLTradBank.Debit = Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero);
                                oGLTradBank.Debcred = "D";
                                oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.TransType = "TRADBANK";
                                oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.Reverse = "N";
                                oGLTradBank.Jnumber = strJnumberNext;
                                oGLTradBank.Branch = strDefaultBranchCode;
                                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                                oGLTradBank.FeeType = "PNSED";
                                SqlCommand dbCommandNseStatControlDebit = oGLTradBank.AddCommandFIX("FIX");
                                db.ExecuteNonQuery(dbCommandNseStatControlDebit, transaction);
                            }
                            #endregion

                            #region Debit CSCS Account
                            if (decTotalCSCS != 0)
                            {
                                oGLTradBank.EffectiveDate = date.ToExact();
                                oGLTradBank.MasterID = oPGenTable.Bcscs;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                                oGLTradBank.Desciption = "CSCS Charges Payment For: " + date.ToExact();
                                oGLTradBank.Credit = 0;
                                oGLTradBank.Debit = Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero);
                                oGLTradBank.Debcred = "D";
                                oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.TransType = "TRADBANK";
                                oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.Reverse = "N";
                                oGLTradBank.Jnumber = strJnumberNext;
                                oGLTradBank.Branch = strDefaultBranchCode;
                                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                                oGLTradBank.FeeType = "PCSCD";
                                SqlCommand dbCommandCSCSStatControlDebit = oGLTradBank.AddCommandFIX("FIX");
                                db.ExecuteNonQuery(dbCommandCSCSStatControlDebit, transaction);
                            }
                            #endregion

                            #region Debit Stamp Duty Account
                            if (decTotalStampDuty != 0)
                            {
                                oGLTradBank.EffectiveDate = date.ToExact();
                                oGLTradBank.MasterID = oPGenTable.Bstamp;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                                oGLTradBank.Desciption = "Stamp Duty Charges Payment For: " + date.ToExact();
                                oGLTradBank.Credit = 0;
                                oGLTradBank.Debit = Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                                oGLTradBank.Debcred = "D";
                                oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.TransType = "TRADBANK";
                                oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                                oGLTradBank.Reverse = "N";
                                oGLTradBank.Jnumber = strJnumberNext;
                                oGLTradBank.Branch = strDefaultBranchCode;
                                oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                                oGLTradBank.FeeType = "PSTDYD";
                                SqlCommand dbCommandCSCSStatControlDebit = oGLTradBank.AddCommandFIX("FIX");
                                db.ExecuteNonQuery(dbCommandCSCSStatControlDebit, transaction);
                            }
                            #endregion

                            #endregion

                            #region Bank Trading And Control Trading Account Posting
                            oGLTradBank.EffectiveDate = date.ToExact();
                            if (decTotalNetConsiderationForSellAndBuy < 0)
                            {
                                oGLTradBank.MasterID = oPGenTable.TradeBank;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                                oGLTradBank.Desciption = "Net Purchase For: " + date.ToExact() + " Trade Settlement Of " + strSettlementDate;
                                oGLTradBank.Credit = Math.Abs(Math.Round(decTotalNetConsiderationForSellAndBuy, 2, MidpointRounding.AwayFromZero)) + Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                                oGLTradBank.Credit = Math.Abs(Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero));
                            }
                            else if (decTotalNetConsiderationForSellAndBuy > 0)
                            {
                                oGLTradBank.MasterID = oPGenTable.ShInv;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                                oGLTradBank.Desciption = "Net Sale Trading For: " + date.ToExact() + " Trade Settlement Of " + strSettlementDate;
                                oGLTradBank.Credit = Math.Round(decTotalNetConsiderationForSellAndBuy, 2, MidpointRounding.AwayFromZero) - (Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero));
                                oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                            }
                            else if (decTotalNetConsiderationForSellAndBuy == 0)
                            {
                                oGLTradBank.MasterID = oPGenTable.TradeBank;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                                oGLTradBank.Desciption = "Net Purchase For: " + date.ToExact() + " Trade Settlement Of " + strSettlementDate;
                                oGLTradBank.Credit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                                oGLTradBank.Credit = Math.Round(oGLTradBank.Credit, 2, MidpointRounding.AwayFromZero);
                            }
                            oGLTradBank.Debit = 0;
                            oGLTradBank.Debcred = "C";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSTD";
                            SqlCommand dbCommandStatControlCredit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandStatControlCredit, transaction);

                            oGLTradBank.EffectiveDate = date.ToExact();
                            if (decTotalNetConsiderationForSellAndBuy > 0)
                            {
                                oGLTradBank.MasterID = oPGenTable.TradeBank;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.ShInv;
                                oGLTradBank.Desciption = "Net Sale For: " + date.ToExact() + " Trade Settlement Of " + strSettlementDate;
                                oGLTradBank.Debit = Math.Round(decTotalNetConsiderationForSellAndBuy, 2, MidpointRounding.AwayFromZero) - (Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero));
                                oGLTradBank.Debit = Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero);
                            }
                            else if (decTotalNetConsiderationForSellAndBuy < 0)
                            {
                                oGLTradBank.MasterID = oPGenTable.ShInv;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                                oGLTradBank.Desciption = "Net Purchase Trading For: " + date.ToExact() + " Trade Settlement Of " + strSettlementDate;
                                oGLTradBank.Debit = Math.Abs(Math.Round(decTotalNetConsiderationForSellAndBuy, 2, MidpointRounding.AwayFromZero)) + Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                                oGLTradBank.Debit = Math.Abs(Math.Round(oGLTradBank.Debit, 2, MidpointRounding.AwayFromZero));
                            }
                            else if (decTotalNetConsiderationForSellAndBuy == 0)
                            {
                                oGLTradBank.MasterID = oPGenTable.ShInv;
                                oGLTradBank.AccountID = "";
                                oGLTradBank.RecAcct = "";
                                oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                                oGLTradBank.Desciption = "Net Purchase Trading For: " + date.ToExact() + " Trade Settlement Of " + strSettlementDate;
                                oGLTradBank.Debit = Math.Round(decTotalSEC, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalNSE, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalCSCS, 2, MidpointRounding.AwayFromZero) + Math.Round(decTotalStampDuty, 2, MidpointRounding.AwayFromZero);
                            }
                            oGLTradBank.Credit = 0;
                            oGLTradBank.Debcred = "D";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSTD";
                            SqlCommand dbCommandStatControlDebit = oGLTradBank.AddCommandFIX("FIX");
                            db.ExecuteNonQuery(dbCommandStatControlDebit, transaction);
                            #endregion

                        }

                        #region Posting Direct Cash Settlement Total Amount To Direct Cash Settlement Account 
                        if (decTotalDirectCashSettlement != 0)
                        {
                            oGLTradBank.EffectiveDate = date.ToExact();
                            oGLTradBank.MasterID = oPGenTable.DirectCashSettleAcct;
                            oGLTradBank.AccountID = "";
                            oGLTradBank.RecAcct = "";
                            oGLTradBank.RecAcctMaster = oPGenTable.TradeBank;
                            oGLTradBank.Desciption = "Direct Settlemet Net Trading For: " + date.ToExact();
                            oGLTradBank.Debit = Math.Round(decTotalDirectCashSettlement, 2, MidpointRounding.AwayFromZero);
                            oGLTradBank.Credit = 0;
                            oGLTradBank.Debcred = "D";
                            oGLTradBank.SysRef = "TRB" + "-" + date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.TransType = "TRADBANK";
                            oGLTradBank.Ref01 = date.ToExact().Day.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Month.ToString().PadLeft(2, char.Parse("0")) + date.ToExact().Year.ToString().PadLeft(4, char.Parse("0"));
                            oGLTradBank.Reverse = "N";
                            oGLTradBank.Jnumber = strJnumberNext;
                            oGLTradBank.Branch = strDefaultBranchCode;
                            oGLTradBank.InstrumentType = DataGeneral.GLInstrumentType.C; oGLTradBank.AcctRef = ""; oGLTradBank.Ref02 = ""; oGLTradBank.Chqno = "";
                            oGLTradBank.FeeType = "TSDCS";
                            SqlCommand dbCommandDirectCashSettleCredit = oGLTradBank.AddCommand();
                            db.ExecuteNonQuery(dbCommandDirectCashSettleCredit, transaction);
                        }
                        #endregion

                        #region End Processing
                        AutoDate oAutoDate = new AutoDate();
                        oAutoDate.iAutoDate = date.ToExact();
                        oAutoDate.UserId = "FIX";
                        SqlCommand dbCommandAutoDate = oAutoDate.AddCommand();
                        db.ExecuteNonQuery(dbCommandAutoDate, transaction);

                        transaction.Commit();
                     return ResponseResult.Success("End Of Day Trading Postings Successful");

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
            else
            {
               return ResponseResult.Error("Cannot Post,No Transaction Exist For This Date");
            }
        }
    }
}