using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using System.Data.SqlTypes;
using BaseUtility.Business;


namespace CapitalMarket.Business
{
    public class Portfolio
    {
        #region Declaration
        private DateTime datPurchaseDate;
        private string strCustomerAcct;
        private string strStockCode;
        private Int64 intUnits;
        private float fltUnitPrice;
        private float fltActualUnitCost;
        private Decimal decTotalCost;
        private string strSysRef;
        private string strTransType;
        private string strRef01;
        private string strTransDesc;
        private string strDebCred;
        private string strMarginCode,strCustodianCode;
        public Int64 TotalUnitForUnitCost { get; set; }
        #endregion

        #region Properties
        public DateTime PurchaseDate
        {
            set { datPurchaseDate = value; }
            get { return datPurchaseDate; }
        }
        public string CustomerAcct
        {
            set { strCustomerAcct = value; }
            get { return strCustomerAcct; }
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
        public float UnitPrice
        {
            set { fltUnitPrice = value; }
            get { return fltUnitPrice; }
        }
        public float ActualUnitCost
        {
            set { fltActualUnitCost = value; }
            get { return fltActualUnitCost; }
        }
        
        public Decimal TotalCost
        {
            set { decTotalCost = value; }
            get { return decTotalCost; }
        }
       
        public string SysRef
        {
            set { strSysRef = value; }
            get { return strSysRef; }
        }

        public string TransType
        {
            set { strTransType = value; }
            get { return strTransType; }
        }
        public string Ref01
        {
            set { strRef01 = value; }
            get { return strRef01; }
        }
        
        public string TransDesc
        {
            set { strTransDesc = value; }
            get { return strTransDesc; }
        }
       
        public string DebCred
        {
            set { strDebCred = value; }
            get { return strDebCred; }
        }
       
        public string MarginCode
        {
            set { strMarginCode = value; }
            get { return strMarginCode; }
        }
        public string CustodianCode
        {
            set { strCustodianCode = value; }
            get { return strCustodianCode; }
        }
       
        #endregion

        #region Add New Portfolio and Return A Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioAdd") as SqlCommand;
            db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(oCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Float, fltUnitPrice);
            db.AddInParameter(oCommand, "ActualUnitCost", SqlDbType.Float, fltActualUnitCost);
            db.AddInParameter(oCommand, "TotalCost", SqlDbType.Decimal, decTotalCost);
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
            db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, strRef01.Trim());
            db.AddInParameter(oCommand, "TransDesc", SqlDbType.VarChar, strTransDesc.Trim());
            db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, strDebCred.Trim());
            db.AddInParameter(oCommand, "MarginCode", SqlDbType.VarChar,strMarginCode.Trim());
            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar,strCustodianCode);
            return oCommand;
        }
        #endregion

        #region Add New Portfolio and Return A Command FIX
        public SqlCommand AddCommandFIX(string strUserId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioAdd") as SqlCommand;
            db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(oCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Float, fltUnitPrice);
            db.AddInParameter(oCommand, "ActualUnitCost", SqlDbType.Float, fltActualUnitCost);
            db.AddInParameter(oCommand, "TotalCost", SqlDbType.Decimal, decTotalCost);
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar,strUserId.Trim());
            db.AddInParameter(oCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
            db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, strRef01.Trim());
            db.AddInParameter(oCommand, "TransDesc", SqlDbType.VarChar, strTransDesc.Trim());
            db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, strDebCred.Trim());
            db.AddInParameter(oCommand, "MarginCode", SqlDbType.VarChar, strMarginCode.Trim());
            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode);
            return oCommand;
        }
        #endregion

        #region Check If CsCs Details is Closed
        public bool CheckCloseCsCsUpload()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioChkCloseCsCsUpload") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 0)
            {
                blnStatus = true;
            }
            else if (thisRow[0]["CustAcctUpLoadDone"] == null)
            {
                blnStatus = true;
            }
            else if (thisRow[0]["CustAcctUpLoadDone"].ToString().Trim() == "N")
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

        #region Delete Portfolio Transaction Upload Selected By TransNo  And Return SqlCommand
        public SqlCommand DeleteUploadByTransNoCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioDeleteUploadByTransNo") as SqlCommand;
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef);
            return oCommand;
        }
        #endregion

        #region Delete Portfolio Transaction Upload FIX And Return SqlCommand
        public SqlCommand DeleteUploadFIXCommand(DateTime datPostDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioDeleteUploadFIX") as SqlCommand;
            db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, datPostDate);
            return oCommand;
        }
        #endregion

        
        #region Delete Portfolio Selected By Sysref And Return SqlCommand
        public SqlCommand DeleteBySysrefCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioReverseBySysref") as SqlCommand;
            db.AddInParameter(oCommand, "Sysref", SqlDbType.VarChar, strSysRef.Trim());
            if (datPurchaseDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            }
            else
            {
                db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, strDebCred.Trim());

            return oCommand;
        }
        #endregion

        #region Delete Portfolio Selected By Customer And Stock And Return SqlCommand
        public SqlCommand DeleteByCustomerStockCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioReverseByCustomerStock") as SqlCommand;
            db.AddInParameter(oCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Portfolio Balance Only Selected By Sysref And Return SqlCommand
        public SqlCommand DeleteBySysrefBalanceOnlyCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioReverseBySysrefBalanceOnly") as SqlCommand;
            db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(oCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, strDebCred.Trim());

            return oCommand;
        }
        #endregion


        #region Delete Portfolio Given Cert Number
        public SqlCommand DeleteCertNumberCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioReverseCertNumber") as SqlCommand;
            db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, strRef01.Trim());
            db.AddInParameter(oCommand, "@Debcred", SqlDbType.VarChar, strDebCred.Trim());
            db.AddInParameter(oCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(oCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region  Get Stock Position
        public long GetNetHolding()
        {
            long intNetHolding = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioNetHolding") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddOutParameter(dbCommand, "NetHolding", SqlDbType.BigInt, 8);
            try
            {
                db.ExecuteNonQuery(dbCommand);
                var varHolding = db.GetParameterValue(dbCommand, "NetHolding");
                intNetHolding = varHolding != null && varHolding.ToString().Trim() != "" ? long.Parse(varHolding.ToString()) : 0;
            }
            catch
            {
                intNetHolding = 0;
            }

            return intNetHolding;
        }
        #endregion

        #region  Get Stock Position Date Specified
        public long GetNetHolding(DateTime datDateSpecified)
        {
            long intNetHolding = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioNetHolding") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datDateSpecified);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddOutParameter(dbCommand, "NetHolding", SqlDbType.BigInt, 8);
            try
            {
                db.ExecuteNonQuery(dbCommand);
                var varHolding = db.GetParameterValue(dbCommand, "NetHolding");
                intNetHolding = varHolding != null && varHolding.ToString().Trim() != "" ? long.Parse(varHolding.ToString()) : 0;
            }
            catch (Exception err)
            {
                string you = err.Message;
                intNetHolding = 0;
            }

            return intNetHolding;
        }
        #endregion

        #region  Get All Customer By Stock For Investment
        public DataSet GetAllCustomerByStockForInvestment(string strInvestmentType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectAllCustomerByStockForInvestment") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode);
            db.AddInParameter(dbCommand, "InvestmentType", SqlDbType.VarChar, strInvestmentType);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get All Customer Stock Position
        public DataSet GetAllCustomerNetHolding()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioAllCustomerNetHolding") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get Stock Position By Previous Date
        public long GetNetHoldingByPreviousDate()
        {
            long intNetHolding = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioNetHoldingByPreviousDate") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddOutParameter(dbCommand, "NetHolding", SqlDbType.BigInt, 8);
            try
            {
                db.ExecuteNonQuery(dbCommand);
                var varHolding = db.GetParameterValue(dbCommand, "NetHolding");
                intNetHolding = varHolding != null && varHolding.ToString().Trim() != "" ? long.Parse(varHolding.ToString()) : 0;
            }
            catch
            {
                intNetHolding = 0;
            }

            return intNetHolding;
        }
        #endregion

        #region  Get Stock Holding List By Previous Date
        public DataSet GetStockHoldingListByPrevDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioStockHoldingListByPreviousDate") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get Portfolio
        public bool GetPortfolio()
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datPurchaseDate = DateTime.ParseExact(thisRow[0]["Purchase Date"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                strCustomerAcct = thisRow[0]["Customer Acct"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                intUnits = int.Parse(thisRow[0]["Units"].ToString());
                fltUnitPrice = thisRow[0]["Unit Price"] != null &&
                             thisRow[0]["Unit Price"].ToString().Trim() != "" ?
                             float.Parse(thisRow[0]["Unit Price"].ToString()) : 0;
                fltActualUnitCost = thisRow[0]["Actual Unit Cost"] != null &&
                             thisRow[0]["Actual Unit Cost"].ToString().Trim() != "" ?
                             float.Parse(thisRow[0]["Actual Unit Cost"].ToString()) : 0;
                decTotalCost = thisRow[0]["Total Cost"] != null &&
                             thisRow[0]["Total Cost"].ToString().Trim() != "" ?
                             decimal.Parse(thisRow[0]["Total Cost"].ToString()) : 0;
                strSysRef = thisRow[0]["SysRef"].ToString();
                strTransType = thisRow[0]["TransType"].ToString();
                strRef01 = thisRow[0]["Ref01"] != null ?
                             thisRow[0]["Ref01"].ToString() : "";
                strTransDesc = thisRow[0]["TransDesc"].ToString();
                strDebCred = thisRow[0]["DebCred"].ToString();
                strMarginCode = thisRow[0]["MarginCode"] != null ?
                             thisRow[0]["MarginCode"].ToString() : "";
                strCustodianCode = thisRow[0]["CustodianCode"] != null ?
                             thisRow[0]["CustodianCode"].ToString(): "";
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion


        #region  Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get By Sysref
        public DataSet GetBySysref()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectShowName") as SqlCommand;
            db.AddInParameter(dbCommand, "Sysref", SqlDbType.NVarChar, strSysRef.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get All By Customer And Stock
        public DataSet GetByCustomerStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectByCustStock") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get All Group By Customer And Stock
        public DataSet GetAllGroupByCustomerStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectAllGroupByCustStock") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get All Group By Stock For Investment
        public DataSet GetAllGroupByStockForInvestment(string strInvestmentType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectAllGroupByStockForInvestment") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "InvestmentType", SqlDbType.VarChar, strInvestmentType);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get All Group By Customer
        public DataSet GetAllGroupByCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectAllGroupByCust") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Check If Customer Stock Is Not Enough In Portfolio To Make A Sale
        public bool ChkQuantityStockNotEnough(DataTable dtAllotment)
        {
            bool blnStatus = true;
            long intNetHolding = 0;
            long intAllotBuy = 0;
            if (dtAllotment != null)
            {
                foreach (DataRow oRow in dtAllotment.Rows)
                {
                    if (oRow["Buy_Sold_Ind"].ToString().Trim() == "B"
                        && oRow["CustNo"].ToString().Trim() == strCustomerAcct.Trim()
                        && oRow["StockCode"].ToString().Trim() == strStockCode.Trim())
                    {
                        intAllotBuy = intAllotBuy + int.Parse(oRow["Units"].ToString());
                    }
                }
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioNetHolding") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddOutParameter(dbCommand, "NetHolding", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NetHolding").ToString() == "" || db.GetParameterValue(dbCommand, "NetHolding") == null)
            {
                if (intAllotBuy < intUnits)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            else
            {
                intNetHolding = long.Parse(db.GetParameterValue(dbCommand, "NetHolding").ToString());
                if ((intNetHolding + intAllotBuy) < intUnits)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            

            return blnStatus;
        }
        #endregion

        #region Calculate Unit Cost
        public double GetUnitCost()
        {
            double decRealUnitCost = 0;
            long intHolding = 0;
            TotalUnitForUnitCost = 0;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioTransactionHolding") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataTable thisTable = db.ExecuteDataSet(dbCommand).Tables[0];
            DataRow[] thisRow = thisTable.Select();

            if (intHolding < 0)
            {
                intHolding = 0;
            }
            if (thisRow.Length >= 1)
            {
                foreach (DataRow oRow in thisTable.Rows)
                {
                    if (oRow["DebCred"].ToString().Trim() == "C")
                    {
                        if (intHolding < 0)
                        {
                            intHolding = 0;
                        }
                        decRealUnitCost = (((intHolding * Math.Round(decRealUnitCost, 2))
                        + (long.Parse(oRow["Units"].ToString().Trim()) * Math.Round(double.Parse(oRow["Actual Unit Cost"].ToString().Trim()), 2)))
                        / (intHolding + long.Parse(oRow["Units"].ToString().Trim())));
                        decRealUnitCost = Math.Round(decRealUnitCost, 2);
                        if (oRow["TransType"].ToString().Trim() != "MARKTOMKT")
                        {
                            intHolding = intHolding + long.Parse(oRow["Units"].ToString().Trim());
                        }
                    }
                    else if (oRow["DebCred"].ToString().Trim() == "D")
                    {
                        intHolding = intHolding - long.Parse(oRow["Units"].ToString().Trim());
                        if (intHolding < 0)
                        {
                            intHolding = 0;
                        }
                    }
                }
            }
            TotalUnitForUnitCost = intHolding;
            return decRealUnitCost;
        }
        #endregion

        #region Calculate Unit Cost For Investment Account Only Exclude Later Transaction
        public double GetUnitCostExcludeLaterTransaction(long lngTransactionNumber)
        {
            long lngCurrentNumber;

            double decRealUnitCost = 0;
            long intHolding = 0;



            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioTransactionHolding") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataTable thisTable = db.ExecuteDataSet(dbCommand).Tables[0];
            DataRow[] thisRow = thisTable.Select();

            if (intHolding < 0)
            {
                intHolding = 0;
            }
            if (thisRow.Length >= 1)
            {
                foreach (DataRow oRow in thisTable.Rows)
                {
                    lngCurrentNumber = long.Parse(oRow["Ref01"] != null && oRow["Ref01"].ToString().Trim() != "" ? oRow["Ref01"].ToString().Trim() : "0");
                    if (lngCurrentNumber < lngTransactionNumber)
                    {
                        if (oRow["DebCred"].ToString().Trim() == "C")
                        {
                            decRealUnitCost = (((intHolding * decRealUnitCost)
                            + (long.Parse(oRow["Units"].ToString().Trim()) * float.Parse(oRow["Actual Unit Cost"].ToString().Trim())))
                            / (intHolding + long.Parse(oRow["Units"].ToString().Trim())));
                            decRealUnitCost = Math.Round(decRealUnitCost, 2);
                            if (oRow["TransType"].ToString().Trim() != "MARKTOMKT")
                            {
                                intHolding = intHolding + long.Parse(oRow["Units"].ToString().Trim());
                            }
                        }
                        else if (oRow["DebCred"].ToString().Trim() == "D")
                        {
                            intHolding = intHolding - long.Parse(oRow["Units"].ToString().Trim());
                            if (intHolding < 0)
                            {
                                intHolding = 0;
                            }
                        }
                    }
                }

            }
            return decRealUnitCost;
        }
        #endregion

        #region Get All Transactions By Non Upload Online
        public DataSet GetAllByNonUploadOnline()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectByNonUploadOnline") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Update Unit Cost and Return A Command
        public SqlCommand UpdateUnitCostCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioUpdateUnitCostOnly") as SqlCommand;
            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Float, fltUnitPrice);
            db.AddInParameter(oCommand, "ActualUnitCost", SqlDbType.Float, fltActualUnitCost);
            db.AddInParameter(oCommand, "TotalCost", SqlDbType.Decimal, decTotalCost);
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion


        #region  Get Holding Of Box Load
        public DataSet GetHoldingBoxLoad()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioHoldingBoxLoad") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "InvestmentProductCode", SqlDbType.VarChar, oStkParam.ProductInvestment);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get Holding  Per Customer
        public DataSet GetHoldingPerCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioHoldingPerCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get Holding Of Box Load Per Customer And StockCode
        public Int64 GetHoldingPerCustomerStockcode()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioHoldingPerCustomerStockCode") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct);
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode);
            var varHolding = db.ExecuteScalar(dbCommand);
            return varHolding != null && varHolding.ToString().Trim() != "" ? long.Parse(varHolding.ToString()) : 0;

        }
        #endregion

        #region  Get Holding Of Box Load Per Customer NSE Only
        public DataSet GetHoldingPerCustomerNSEOnly()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioHoldingPerCustomerNSEOnly") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region  Get Holding Of Box Load Per Customer NASD Only
        public DataSet GetHoldingPerCustomerNASDOnly()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioHoldingPerCustomerNASDOnly") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get All Fix Portfolio Distinct Date
        public DataSet GetAllFixDistinctDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("PortfolioSelectAllFixDistinctDatePosted") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FIX Portfolio Not In GL
        public DataSet GetFIXPortfolioNotInGL(DateTime datTransDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectFIXPortfolioNotInGL") as SqlCommand;
            db.AddInParameter(dbCommand, "TransDate", SqlDbType.DateTime, datTransDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FIX Portfolio Not In Allotment
        public DataSet GetFIXPortfolioNotInAllotment(DateTime datTransDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectFIXPortfolioNotInAllotment") as SqlCommand;
            db.AddInParameter(dbCommand, "TransDate", SqlDbType.DateTime, datTransDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Change Unit Cost For Date,Customer And Stock
        public DataSet GetChangeUnitCostForDateCustomerAndStock()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioChangeUnitCostForDateCustomerAndStock") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get Last Change Unit Cost Date
        public DateTime GetLastChangeUnitCostDate()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioChangeUnitCostMaxDate") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datPurchaseDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            var varLastDate = db.ExecuteScalar(dbCommand);
            return varLastDate != null && varLastDate.ToString().Trim() != "" ? DateTime.ParseExact(varLastDate.ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion

        #region Get Price For Last Quote Date For A Particular Stock For A Date
        public decimal GetPriceForChangeUnitCostForADateCustomerAndStock(DateTime datSpecifiedDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioChangeUnitCostPriceForDateCustomerAndStock") as SqlCommand;
            db.AddInParameter(dbCommand, "PurchaseDate", SqlDbType.DateTime, datSpecifiedDate);
            db.AddInParameter(dbCommand, "CustomerAcct", SqlDbType.VarChar, strCustomerAcct.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            var varUnitCost = db.ExecuteScalar(dbCommand);
            return varUnitCost != null || varUnitCost.ToString().Trim() != "" ? Convert.ToDecimal(varUnitCost) : 0;
        }
        #endregion

        public class StockHolding
        {
            public string SecurityCode { get; set; }
            public Int64 Quantity { get; set; }
            public string SeccodeQuantity { get; set; }
        }

        //--------- Transfer From Old Database

        #region Get Old Portfolio Transactions Given Old Transaction Number
        public DataSet GetPortTransGivenAllotNo(string strOldTransNo)
        {
            Allotment oAllot = new Allotment();
            SqlDatabase db = DatabaseFactory.CreateDatabase("GlobalSuitedbOld") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioSelectBySysrefOld") as SqlCommand;
            string strResult = oAllot.GetAllotType(strOldTransNo);
            if (strResult.Trim() == "B")
            {

                db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, "STKB-" + strOldTransNo.Trim());
            }
            else if (strResult.Trim() == "S")
            {

                db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, "COLT-" + strOldTransNo.Trim());
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        //---------------end of Transfer

    }
}
