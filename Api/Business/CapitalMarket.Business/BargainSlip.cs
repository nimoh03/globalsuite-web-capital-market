using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using CustomerManagement.Business;
using GL.Business;

namespace CapitalMarket.Business
{
    public class BargainSlip
    {
        IFormatProvider format = new CultureInfo("en-GB");

        #region Declaration
        private DateTime datiDate;
        private string strBslip, strTransNo, strStockCode;
        private Int64 intUnits;
        private decimal decUnitPrice, decConsideration, decRConsideration;
        private string strStockType, strBuy_Sold_Ind, strAllot_Ind, strSing_Mult_Ind;
        private Int32 intMultiAmt;
        private string strMultiNo;
        private Int32 intAllot_Amt;
        private string strSoldBy, strBoughtBy, strCrossD, strUserID;
        private DateTime datTxnTime;
        private string strCrossDone, strCrossSell, strCrossBuy;
        private decimal decRealCon, decCapGain;
        private bool blnLumbSum;
        private string strAcctNo, strCrossAcctNo, strCustNo, strCrossCustNo, strCrossType;
        #endregion

        #region Properties
        public DateTime iDate
        {
            set { datiDate = value; }
            get { return datiDate; }
        }
        public string Bslip
        {
            set { strBslip = value; }
            get { return strBslip; }
        }
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public Int64 Units
        {
            set { intUnits = value; }
            get { return intUnits; }
        }
        public decimal UnitPrice
        {
            set { decUnitPrice = value; }
            get { return decUnitPrice; }
        }
        public decimal Consideration
        {
            set { decConsideration = value; }
            get { return decConsideration; }
        }
        public decimal RConsideration
        {
            set { decRConsideration = value; }
            get { return decRConsideration; }
        }
        public string StockType
        {
            set { strStockType = value; }
            get { return strStockType; }
        }
        public string Buy_Sold_Ind
        {
            set { strBuy_Sold_Ind = value; }
            get { return strBuy_Sold_Ind; }
        }
        public string Allot_Ind
        {
            set { strAllot_Ind = value; }
            get { return strAllot_Ind; }
        }
        public string Sing_Mult_Ind
        {
            set { strSing_Mult_Ind = value; }
            get { return strSing_Mult_Ind; }
        }
        public Int32 MultiAmt
        {
            set { intMultiAmt = value; }
            get { return intMultiAmt; }
        }
        public string MultiNo
        {
            set { strMultiNo = value; }
            get { return strMultiNo; }
        }
        public Int32 Allot_Amt
        {
            set { intAllot_Amt = value; }
            get { return intAllot_Amt; }
        }
        public string SoldBy
        {
            set { strSoldBy = value; }
            get { return strSoldBy; }
        }
        public string BoughtBy
        {
            set { strBoughtBy = value; }
            get { return strBoughtBy; }
        }
        public string CrossD
        {
            set { strCrossD = value; }
            get { return strCrossD; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public DateTime TxnTime
        {
            set { datTxnTime = value; }
            get { return datTxnTime; }
        }
        public string CrossDone
        {
            set { strCrossDone = value; }
            get { return strCrossDone; }
        }
        public string CrossSell
        {
            set { strCrossSell = value; }
            get { return strCrossSell; }
        }
        public string CrossBuy
        {
            set { strCrossBuy = value; }
            get { return strCrossBuy; }
        }
        public decimal RealCon
        {
            set { decRealCon = value; }
            get { return decRealCon; }
        }
        public decimal CapGain
        {
            set { decCapGain = value; }
            get { return decCapGain; }
        }
        public bool LumbSum
        {
            set { blnLumbSum = value; }
            get { return blnLumbSum; }
        }
        public string AcctNo
        {
            set { strAcctNo = value; }
            get { return strAcctNo; }
        }
        public string CrossAcctNo
        {
            set { strCrossAcctNo = value; }
            get { return strCrossAcctNo; }
        }
        public string CustNo
        {
            set { strCustNo = value; }
            get { return strCustNo; }
        }
        public string CrossCustNo
        {
            set { strCrossCustNo = value; }
            get { return strCrossCustNo; }
        }
        public string CrossType
        {
            set { strCrossType = value; }
            get { return strCrossType; }
        }
        
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete All
        public void DeleteAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipDeleteAll") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);

        }
        #endregion

        #region Update Cross Customer No and CsCs Account
        public bool UpdateCrossCustNoCsCsAcct()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BargainSlipUpdateCrossCustNoCsCsAcct") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "CrossAcctNo", SqlDbType.VarChar, strCrossAcctNo.Trim());
            db.AddInParameter(oCommand, "CrossCustNo", SqlDbType.VarChar, strCrossCustNo.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;

            return blnStatus;

        }
        #endregion

        #region Update Unit
        public bool UpdateUnit()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BargainSlipUpdateUnit") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;

            return blnStatus;

        }
        #endregion

        #region Delete A Transaction
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Process Cross Deals
        public void ProcessCrossDeal()
        {
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandSell = null;
            SqlCommand dbCommandBuy = db.GetStoredProcCommand("BargainSlipSelectAllBuyCrossDeal") as SqlCommand;
            DataSet oDSBuy = db.ExecuteDataSet(dbCommandBuy);
            foreach (DataRow oRowBuy in oDSBuy.Tables[0].Rows)
            {
                dbCommandSell = db.GetStoredProcCommand("BargainSlipSelectSellCrossDealForBuy") as SqlCommand;
                db.AddInParameter(dbCommandSell, "StockCode", SqlDbType.VarChar, oRowBuy["StockCode"].ToString());
                db.AddInParameter(dbCommandSell, "UnitPrice", SqlDbType.Decimal, decimal.Parse(oRowBuy["UnitPrice"].ToString()));
                db.AddInParameter(dbCommandSell, "Units", SqlDbType.BigInt, long.Parse(oRowBuy["Units"].ToString()));
                DataSet oDSSell = db.ExecuteDataSet(dbCommandSell);
                DataTable thisTableSell = oDSSell.Tables[0];
                DataRow[] thisRowSell = thisTableSell.Select();
                if (thisRowSell.Length >= 1)
                {
                    strTransNo = oRowBuy["TransNo"].ToString();
                    strCrossCustNo = thisRowSell[0]["CustNo"].ToString();
                    strCrossAcctNo = thisRowSell[0]["AcctNo"].ToString();
                    UpdateCrossCustNoCsCsAcct();
                    strTransNo = thisRowSell[0]["TransNo"].ToString();
                    Delete();
                }
            }
        }
        #endregion

        #region Process UnEqual Cross Deals
        public void ProcessUnEqualCrossDeal()
        {
            BargainTrans oBargainTrans = new BargainTrans();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandSell = null;
            SqlCommand dbCommandBuy = db.GetStoredProcCommand("BargainSlipSelectNoCrossCustBuyCrossDeal") as SqlCommand;
            DataSet oDSBuy = db.ExecuteDataSet(dbCommandBuy);
            foreach (DataRow oRowBuy in oDSBuy.Tables[0].Rows)
            {
                int intBuyDeducted = 0;
                dbCommandSell = db.GetStoredProcCommand("BargainSlipSelectUnEqualSellCrossDealForBuy") as SqlCommand;
                db.AddInParameter(dbCommandSell, "StockCode", SqlDbType.VarChar, oRowBuy["StockCode"].ToString());
                db.AddInParameter(dbCommandSell, "UnitPrice", SqlDbType.Decimal, decimal.Parse(oRowBuy["UnitPrice"].ToString()));
                DataSet oDSSell = db.ExecuteDataSet(dbCommandSell);
                foreach (DataRow oRowSell in oDSSell.Tables[0].Rows)
                {
                    if ((int.Parse(oRowBuy["Units"].ToString()) - intBuyDeducted) >= int.Parse(oRowSell["Units"].ToString()))
                    {
                        oBargainTrans.Bslip = oRowBuy["BSlip#"].ToString();
                        oBargainTrans.iDate = DateTime.ParseExact(oRowBuy["Date"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                        oBargainTrans.StockCode = oRowBuy["StockCode"].ToString();
                        oBargainTrans.Units = long.Parse(oRowSell["Units"].ToString());
                        oBargainTrans.UnitPrice = decimal.Parse(oRowSell["UnitPrice"].ToString());
                        oBargainTrans.Consideration = decimal.Parse(oRowSell["Consideration"].ToString());
                        oBargainTrans.StockType = oRowBuy["StockType"].ToString();
                        oBargainTrans.SoldBy = oRowBuy["SoldBy"].ToString();
                        oBargainTrans.Buy_Sold_Ind = oRowBuy["Buy_Sold_Ind"].ToString();
                        oBargainTrans.BoughtBy = oRowBuy["BoughtBy"].ToString();
                        oBargainTrans.CrossD = oRowBuy["CrossD"].ToString();
                        oBargainTrans.AcctNo = oRowBuy["AcctNo"].ToString();
                        oBargainTrans.CustNo = oRowBuy["CustNo"].ToString();
                        oBargainTrans.CrossAcctNo = oRowSell["AcctNo"].ToString();
                        oBargainTrans.CrossCustNo = oRowSell["CustNo"].ToString();
                        oBargainTrans.CrossType = "";
                        oBargainTrans.SaveToBargainSlipNoProcess();
                        strTransNo = oRowBuy["TransNo"].ToString();
                        intUnits = int.Parse(oRowSell["Units"].ToString());
                        UpdateUnit();
                        intBuyDeducted = intBuyDeducted + int.Parse(oRowSell["Units"].ToString());
                        strTransNo = oRowSell["TransNo"].ToString();
                        Delete();
                    }
                }
            }
            ProcessUnEqualSellCrossDeal();
            DeleteZeroCrossDeal();
        }
        #endregion

        #region Process UnEqual Sell Cross Deals
        public void ProcessUnEqualSellCrossDeal()
        {
            BargainTrans oBargainTrans = new BargainTrans();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandBuy  = null;
            SqlCommand dbCommandSell = db.GetStoredProcCommand("BargainSlipSelectAllSellCrossDeal") as SqlCommand;
            DataSet oDSSell = db.ExecuteDataSet(dbCommandSell);
            foreach (DataRow oRowSell in oDSSell.Tables[0].Rows)
            {
                int intSellDeducted = 0;
                dbCommandBuy = db.GetStoredProcCommand("BargainSlipSelectUnEqualNoCrossCustBuyCrossDealForSell") as SqlCommand;
                db.AddInParameter(dbCommandBuy, "StockCode", SqlDbType.VarChar, oRowSell["StockCode"].ToString());
                db.AddInParameter(dbCommandBuy, "UnitPrice", SqlDbType.Decimal, decimal.Parse(oRowSell["UnitPrice"].ToString()));
                DataSet oDSBuy = db.ExecuteDataSet(dbCommandBuy);
                foreach (DataRow oRowBuy in oDSBuy.Tables[0].Rows)
                {
                    if ((int.Parse(oRowSell["Units"].ToString()) - intSellDeducted) > int.Parse(oRowBuy["Units"].ToString()))
                    {
                        strTransNo = oRowBuy["TransNo"].ToString();
                        strCrossCustNo = oRowSell["CustNo"].ToString();
                        strCrossAcctNo = oRowSell["AcctNo"].ToString();
                        UpdateCrossCustNoCsCsAcct();
                        strTransNo = oRowSell["TransNo"].ToString();
                        intUnits = int.Parse(oRowBuy["Units"].ToString());
                        UpdateUnit();
                        intSellDeducted = intSellDeducted + int.Parse(oRowBuy["Units"].ToString());
                    }
                }
            }
        }
        #endregion

        #region Update Cross Deal Type
        public void UpdateCrossType()
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CROSSTYPE";
            string strCrossDealType = oGLParam.CheckParameter();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipUpdateCrossType") as SqlCommand;
            db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar, strCrossDealType.Trim());
            db.ExecuteNonQuery(dbCommand);

        }
        #endregion

        #region Delete Zero Cross Deals
        public void DeleteZeroCrossDeal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandBuy = db.GetStoredProcCommand("BargainSlipDeleteZeroUnitCrossDeal") as SqlCommand;
            db.ExecuteNonQuery(dbCommandBuy);
            
        }
        #endregion

        #region Check If CapMarketCustomer Has Enough In Portfolio To Make Sale in Transaction Upload
        public int ChkPortfolioStockEnoughToSell(string strUserName,DataTable dtAllot)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            int intReturn = 0;
            StkParam oStkParam = new StkParam();
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CHKSTOCKJOB";
            if (oGLParam.CheckParameter() == "YES")
            {
                PortNot oPortNot = new PortNot();
                if (!oPortNot.DeleteAll())
                {
                    intReturn = 2;
                    return intReturn;
                }
                foreach (DataRow oRowView in GetAll().Tables[0].Rows)
                {
                    if (oRowView["Buy_Sold_Ind"].ToString().Trim() == "S")
                    {
                        Portfolio oPortfolio = new Portfolio();
                        oPortfolio.CustomerAcct = oRowView["CustNo"].ToString().Trim();
                        oPortfolio.StockCode = oRowView["StockCode"].ToString().Trim();
                        oPortfolio.Units = long.Parse(oRowView["Units"].ToString().Trim());
                        oPortfolio.PurchaseDate = DateTime.ParseExact(oRowView["Date"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                        if (oPortfolio.ChkQuantityStockNotEnough(dtAllot))
                        {
                            Verification oVerification = new Verification();
                            oVerification.CustNo = oRowView["CustNo"].ToString().Trim();
                            oVerification.Stockcode = oRowView["StockCode"].ToString().Trim();

                            Customer oCustomer = new Customer();
                            oCustomer.CustAID = oRowView["CustNo"].ToString();
                            ProductAcct oProductAcct = new ProductAcct();
                            oProductAcct.CustAID = oRowView["CustNo"].ToString();
                            oProductAcct.ProductCode = oStkParam.Product;
                            
                            if (oCustomer.GetCustomerName(oStkParam.Product) && oProductAcct.GetCSCSNo())
                            { }
                            oPortNot.PortDesc = "Stock Quantity Not Enough:  Sold "
                                + oRowView["Units"].ToString().Trim() + " Of " +
                                oRowView["StockCode"].ToString().Trim() + " But Has Only " +
                                oPortfolio.GetNetHolding().ToString().Trim() + " In Portfolio";
                            DataSet oDsUnVerifyCert = oVerification.GetUnPostedGivenCustStock();
                            if (oDsUnVerifyCert.Tables[0].Rows.Count >= 1)
                            {
                                oPortNot.PortDesc = oPortNot.PortDesc + "\r\n" + "\t" +
                                       "-----Please Verify This UnVerified Certificates-------";
                                oPortNot.PortDesc = oPortNot.PortDesc + "\r\n" + "\t" +
                                        " PLEASE MAKE SURE RETURNED DATE IS SAME OR EARLIER THAN DATE OF TRANSACTION POSTING";

                                foreach (DataRow oRowUnVerifyCert in oDsUnVerifyCert.Tables[0].Rows)
                                {
                                    oPortNot.PortDesc = oPortNot.PortDesc + "\r\n" + "\t" +
                                        "TransNo: " + oRowUnVerifyCert["Transno"].ToString().Trim() +
                                        " CertNo: " + oRowUnVerifyCert["Certno"].ToString().Trim() +
                                        " Units: " + oRowUnVerifyCert["Units"].ToString().Trim() +
                                        " StockCode: " + oRowUnVerifyCert["StockCode"].ToString().Trim() +
                                        " Date Lodged: " + oRowUnVerifyCert["EffDate"].ToString().Trim().Substring(0, 10);
                                }
                            }
                            oPortNot.CsCsAcct = oProductAcct.CsCsAcct.Trim();
                            oPortNot.CustId = oRowView["CustNo"].ToString().Trim(); ;
                            oPortNot.CustName = oCustomer.Surname.Trim() + "  " + oCustomer.Firstname.Trim()
                                + "  " + oCustomer.Othername.Trim();
                            oPortNot.StockCode = oRowView["StockCode"].ToString().Trim();
                            oPortNot.EffDate = DateTime.ParseExact(oRowView["Date"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                            oPortNot.UserId = strUserName.Trim();
                            if (!oPortNot.Add())
                            {
                                intReturn = 3;
                                return intReturn;
                            }
                        }
                    }
                }
                DataSet dataChkPortNot = oPortNot.GetAll();
                System.Data.DataTable thisTablePortNot = dataChkPortNot.Tables[0];
                System.Data.DataRow[] thisRowPortNot = thisTablePortNot.Select();
                if (thisRowPortNot.Length == 0)
                {
                    intReturn = 1;
                    return intReturn;
                }
                else
                {
                    intReturn = 4;
                    return intReturn;
                }
            }
            else
            {
                intReturn = 1;
                return intReturn;
            }
        }
        #endregion

        #region Get Cross Deal With No Cross Customer
        public DataSet GetAllCrossDealNoCrossCust()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipSelectAllCrossDealWithNoCrossCust") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Cross Deal With Negative Units
        public DataSet GetAllCrossDealNegativeUnit()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipSelectAllCrossDealWithNegativeUnits") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Do Cross Deal Checks
        public bool CheckCrossDealProcess()
        {
            bool blnStatus = false;
            if ((GetAllCrossDealNegativeUnit().Tables[0].Rows.Count > 0) || (GetAllCrossDealNoCrossCust().Tables[0].Rows.Count > 0))
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get All Cross Deals
        public DataSet GetAllCrossDeal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipSelectAllCrossDeal") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Asset Deposit Transactions
        public DataSet GetAllAssetDepositTrans(string strProductCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipSelectAllAssetDepositTran") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
        

        #region Get Cross Type
        public string GetCrossType()
        {
            string strResult="";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipSelectCrossType") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            if (db.ExecuteScalar(dbCommand) != null)
            {
                strResult = db.ExecuteScalar(dbCommand).ToString();
            }
            return strResult;
        }
        #endregion

        #region Update Cross Type By TransNo
        public string UpdateCrossTypeByTransNo()
        {
            string strResult = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipUpdateCrossTypeByTransNo") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar,strCrossType.Trim());
            if (db.ExecuteScalar(dbCommand) != null)
            {
                strResult = db.ExecuteScalar(dbCommand).ToString();
            }
            return strResult;
        }
        #endregion

        #region Update Treat As AssetDeposit By TransNo
        public void UpdateTreatAsAssetDepositByTransNo()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipUpdateTreatAsAssetDepositByTransNo") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.ExecuteNonQuery(dbCommand).ToString();
        }
        #endregion

        #region Update Change To Normal Buy
        public void UpdateChangeToNormalBuy()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainSlipUpdateChangeToNormalBuyByTransNo") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.ExecuteNonQuery(dbCommand).ToString();
        }
        #endregion

        

        
        
    }
}
