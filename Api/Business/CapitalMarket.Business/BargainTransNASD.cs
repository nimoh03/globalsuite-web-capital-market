using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;
using CustomerManagement.Business;
using GL.Business;

namespace CapitalMarket.Business
{
    public class BargainTransNASD
    {
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
        private string strNASDAcctNo, strCrossNASDAcctNo, strCustNo, strCrossCustNo, strCrossType;
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
        public string NASDAcctNo
        {
            set { strNASDAcctNo = value; }
            get { return strNASDAcctNo; }
        }
        public string CrossNASDAcctNo
        {
            set { strCrossNASDAcctNo = value; }
            get { return strCrossNASDAcctNo; }
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

        #region Add
        public void Add()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BargainTransNASDAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(oCommand, "Date", SqlDbType.DateTime, datiDate);
            db.AddInParameter(oCommand, "Bslip", SqlDbType.VarChar, strBslip.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.AddInParameter(oCommand, "Unitprice", SqlDbType.Money, decUnitPrice);
            db.AddInParameter(oCommand, "Consideration", SqlDbType.Money, decConsideration);
            db.AddInParameter(oCommand, "RConsideration", SqlDbType.Money, decRConsideration);
            db.AddInParameter(oCommand, "StockType", SqlDbType.VarChar, strStockType.Trim());
            db.AddInParameter(oCommand, "Buy_Sold_Ind", SqlDbType.Char, strBuy_Sold_Ind.Trim());
            db.AddInParameter(oCommand, "Allot_Ind", SqlDbType.Char, 'N');
            db.AddInParameter(oCommand, "Sing_Mult_Ind", SqlDbType.Char, strSing_Mult_Ind);
            db.AddInParameter(oCommand, "MultiAmt", SqlDbType.Int, intMultiAmt);
            db.AddInParameter(oCommand, "MultiNo", SqlDbType.Int, intMultiAmt);
            db.AddInParameter(oCommand, "Allot_Amt", SqlDbType.Int, intAllot_Amt);
            db.AddInParameter(oCommand, "Soldby", SqlDbType.VarChar, strSoldBy.Trim());
            db.AddInParameter(oCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
            db.AddInParameter(oCommand, "CrossD", SqlDbType.Char, strCrossD.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.AddInParameter(oCommand, "CrossDone", SqlDbType.Char, 'C');
            db.AddInParameter(oCommand, "CrossSell", SqlDbType.VarChar, strCrossSell);
            db.AddInParameter(oCommand, "CrossBuy", SqlDbType.VarChar, strCrossBuy);
            db.AddInParameter(oCommand, "RealCon", SqlDbType.Money, decRealCon);
            db.AddInParameter(oCommand, "CapGain", SqlDbType.Money, decCapGain);
            db.AddInParameter(oCommand, "LumpSum", SqlDbType.Int, blnLumbSum);
            db.AddInParameter(oCommand, "NASDAcctNo", SqlDbType.VarChar, strNASDAcctNo.Trim());
            db.AddInParameter(oCommand, "CrossNASDAcctNo", SqlDbType.VarChar, strCrossNASDAcctNo);
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "CrossCustNo", SqlDbType.VarChar, strCrossCustNo);
            db.AddInParameter(oCommand, "CrossType", SqlDbType.Char, strCrossType);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "BargainTransNASD");
            db.ExecuteNonQuery(oCommand);
        }
        #endregion


        #region Save To BargainSlip NASD
        public void SaveToBargainSlipNASD()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            foreach (DataRow oRow in GetUniqueTrans().Tables[0].Rows)
            {
                CustNo = oRow["CustNo"].ToString();
                StockCode = oRow["StockCode"].ToString();
                UnitPrice = decimal.Parse(oRow["UnitPrice"].ToString());
                Buy_Sold_Ind = oRow["Buy_Sold_Ind"].ToString();
                CrossD = oRow["CrossD"].ToString();
                GetBargainTransNASDGivenCustStockPriceTypeCD();
                oCommand = db.GetStoredProcCommand("BargainSlipNASDAddNew") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo);
                db.AddInParameter(oCommand, "Date", SqlDbType.DateTime, datiDate);
                db.AddInParameter(oCommand, "Bslip", SqlDbType.VarChar, strBslip.Trim());
                db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, oRow["StockCode"].ToString().Trim());
                db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, long.Parse(oRow["TotalUnit"].ToString()));
                db.AddInParameter(oCommand, "Unitprice", SqlDbType.Money, decimal.Parse(oRow["UnitPrice"].ToString()));
                db.AddInParameter(oCommand, "Consideration", SqlDbType.Money, decConsideration);
                db.AddInParameter(oCommand, "RConsideration", SqlDbType.Money, decRConsideration);
                db.AddInParameter(oCommand, "StockType", SqlDbType.VarChar, strStockType.Trim());
                db.AddInParameter(oCommand, "Buy_Sold_Ind", SqlDbType.Char, oRow["Buy_Sold_Ind"].ToString().Trim());
                db.AddInParameter(oCommand, "Allot_Ind", SqlDbType.Char, 'N');
                db.AddInParameter(oCommand, "Sing_Mult_Ind", SqlDbType.Char, strSing_Mult_Ind);
                db.AddInParameter(oCommand, "MultiAmt", SqlDbType.Int, intMultiAmt);
                db.AddInParameter(oCommand, "MultiNo", SqlDbType.Int, intMultiAmt);
                db.AddInParameter(oCommand, "Allot_Amt", SqlDbType.Int, intAllot_Amt);
                db.AddInParameter(oCommand, "Soldby", SqlDbType.VarChar, strSoldBy.Trim());
                db.AddInParameter(oCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
                db.AddInParameter(oCommand, "CrossD", SqlDbType.Char, oRow["CrossD"].ToString().Trim());
                db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
                db.AddInParameter(oCommand, "CrossDone", SqlDbType.Char, 'C');
                db.AddInParameter(oCommand, "CrossSell", SqlDbType.VarChar, strCrossSell);
                db.AddInParameter(oCommand, "CrossBuy", SqlDbType.VarChar, strCrossBuy);
                db.AddInParameter(oCommand, "RealCon", SqlDbType.Money, decRealCon);
                db.AddInParameter(oCommand, "CapGain", SqlDbType.Money, decCapGain);
                db.AddInParameter(oCommand, "LumpSum", SqlDbType.Int, blnLumbSum);
                db.AddInParameter(oCommand, "NASDAcctNo", SqlDbType.VarChar, strNASDAcctNo.Trim());
                db.AddInParameter(oCommand, "CrossNASDAcctNo", SqlDbType.VarChar, strCrossNASDAcctNo);
                db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, oRow["CustNo"].ToString().Trim());
                db.AddInParameter(oCommand, "CrossCustNo", SqlDbType.VarChar, strCrossCustNo);
                db.AddInParameter(oCommand, "CrossType", SqlDbType.Char, strCrossType);
                db.AddInParameter(oCommand, "NumberOfTrans", SqlDbType.Int, int.Parse(oRow["NumberOfTrans"].ToString()));
                db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "BARGAINSLIPNASD");
                db.ExecuteNonQuery(oCommand);
            }
        }
        #endregion

        #region Save To BargainSlip NASD Cross Deal
        public void SaveToBargainSlipNASDCrossDeal()
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CROSSTYPE";
            string strCrossDealType = oGLParam.CheckParameter();

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            foreach (DataRow oRow in GetAllBargainTransNASDCrossDeal().Tables[0].Rows)
            {
                oCommand = db.GetStoredProcCommand("BargainSlipNASDAddNew") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, oRow["TransNo"].ToString().Trim());
                db.AddInParameter(oCommand, "Date", SqlDbType.DateTime, DateTime.ParseExact(oRow["Date"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format));
                db.AddInParameter(oCommand, "Bslip", SqlDbType.VarChar, oRow["Bslip#"].ToString().Trim());
                db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, oRow["StockCode"].ToString().Trim());
                db.AddInParameter(oCommand, "Units", SqlDbType.Int, int.Parse(oRow["Units"].ToString()));
                db.AddInParameter(oCommand, "Unitprice", SqlDbType.Money, decimal.Parse(oRow["UnitPrice"].ToString()));
                db.AddInParameter(oCommand, "Consideration", SqlDbType.Money, decimal.Parse(oRow["Consideration"].ToString().Trim()));
                db.AddInParameter(oCommand, "RConsideration", SqlDbType.Money, decimal.Parse(oRow["RConsideration"].ToString().Trim()));
                db.AddInParameter(oCommand, "StockType", SqlDbType.VarChar, oRow["StockType"].ToString().Trim());
                db.AddInParameter(oCommand, "Buy_Sold_Ind", SqlDbType.Char, oRow["Buy_Sold_Ind"].ToString().Trim());
                db.AddInParameter(oCommand, "Allot_Ind", SqlDbType.Char, "N");
                db.AddInParameter(oCommand, "Sing_Mult_Ind", SqlDbType.Char, oRow["Sing_Mult_Ind"].ToString().Trim());
                db.AddInParameter(oCommand, "MultiAmt", SqlDbType.Int, 0);
                db.AddInParameter(oCommand, "MultiNo", SqlDbType.Int, 0);
                db.AddInParameter(oCommand, "Allot_Amt", SqlDbType.Int, int.Parse(oRow["Allot_Amt"].ToString().Trim()));
                db.AddInParameter(oCommand, "Soldby", SqlDbType.VarChar, oRow["Soldby"].ToString().Trim());
                db.AddInParameter(oCommand, "BoughtBy", SqlDbType.VarChar, oRow["BoughtBy"].ToString().Trim());
                db.AddInParameter(oCommand, "CrossD", SqlDbType.Char, char.Parse(oRow["CrossD"].ToString().Trim()));
                db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
                db.AddInParameter(oCommand, "CrossDone", SqlDbType.Char, 'C');
                db.AddInParameter(oCommand, "CrossSell", SqlDbType.VarChar, oRow["CrossSell"].ToString().Trim());
                db.AddInParameter(oCommand, "CrossBuy", SqlDbType.VarChar, oRow["CrossBuy"].ToString().Trim());
                db.AddInParameter(oCommand, "RealCon", SqlDbType.Money, 0);
                db.AddInParameter(oCommand, "CapGain", SqlDbType.Money, 0);
                db.AddInParameter(oCommand, "LumpSum", SqlDbType.Int, 0);
                db.AddInParameter(oCommand, "NASDAcctNo", SqlDbType.VarChar, oRow["NASDAcctNo"].ToString().Trim());
                db.AddInParameter(oCommand, "CrossNASDAcctNo", SqlDbType.VarChar, oRow["CrossNASDAcctNo"].ToString().Trim());
                db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, oRow["CustNo"].ToString().Trim());
                db.AddInParameter(oCommand, "CrossCustNo", SqlDbType.VarChar, oRow["CrossCustNo"].ToString().Trim());
                db.AddInParameter(oCommand, "CrossType", SqlDbType.Char, strCrossDealType.Trim());
                db.AddInParameter(oCommand, "NumberOfTrans", SqlDbType.Int, 1);
                db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "BARGAINSLIP");
                db.ExecuteNonQuery(oCommand);
            }
        }
        #endregion

        #region Save To BargainSlip NASD No Process
        public void SaveToBargainSlipNASDNoProcess()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;

            SqlCommand oCommand = db.GetStoredProcCommand("BargainSlipNASDAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(oCommand, "Date", SqlDbType.DateTime, datiDate);
            db.AddInParameter(oCommand, "Bslip", SqlDbType.VarChar, strBslip.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
            db.AddInParameter(oCommand, "Unitprice", SqlDbType.Money, decUnitPrice);
            db.AddInParameter(oCommand, "Consideration", SqlDbType.Money, decConsideration);
            db.AddInParameter(oCommand, "RConsideration", SqlDbType.Money, decRConsideration);
            db.AddInParameter(oCommand, "StockType", SqlDbType.VarChar, strStockType.Trim());
            db.AddInParameter(oCommand, "Buy_Sold_Ind", SqlDbType.Char, strBuy_Sold_Ind);
            db.AddInParameter(oCommand, "Allot_Ind", SqlDbType.Char, 'N');
            db.AddInParameter(oCommand, "Sing_Mult_Ind", SqlDbType.Char, strSing_Mult_Ind);
            db.AddInParameter(oCommand, "MultiAmt", SqlDbType.Int, intMultiAmt);
            db.AddInParameter(oCommand, "MultiNo", SqlDbType.Int, intMultiAmt);
            db.AddInParameter(oCommand, "Allot_Amt", SqlDbType.Int, intAllot_Amt);
            db.AddInParameter(oCommand, "Soldby", SqlDbType.VarChar, strSoldBy.Trim());
            db.AddInParameter(oCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
            db.AddInParameter(oCommand, "CrossD", SqlDbType.Char, strCrossD);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.AddInParameter(oCommand, "CrossDone", SqlDbType.Char, 'C');
            db.AddInParameter(oCommand, "CrossSell", SqlDbType.VarChar, strCrossSell);
            db.AddInParameter(oCommand, "CrossBuy", SqlDbType.VarChar, strCrossBuy);
            db.AddInParameter(oCommand, "RealCon", SqlDbType.Money, decRealCon);
            db.AddInParameter(oCommand, "CapGain", SqlDbType.Money, decCapGain);
            db.AddInParameter(oCommand, "LumpSum", SqlDbType.Int, blnLumbSum);
            db.AddInParameter(oCommand, "NASDAcctNo", SqlDbType.VarChar, strNASDAcctNo.Trim());
            db.AddInParameter(oCommand, "CrossNASDAcctNo", SqlDbType.VarChar, strCrossNASDAcctNo.Trim());
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "CrossCustNo", SqlDbType.VarChar, strCrossCustNo.Trim());
            db.AddInParameter(oCommand, "CrossType", SqlDbType.Char, strCrossType.Trim());
            db.AddInParameter(oCommand, "NumberOfTrans", SqlDbType.Int, 1);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "BARGAINSLIPNASD");
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Get Double Transaction of the Same Price,Date,Customer Number
        public DataSet GetUniqueTrans()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainTransNASDGroupByUniqueTransOrderByCustno") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Double Transaction of the Same Price,Date,Customer Number,Bank Margin Code
        public DataSet GetUniqueTransBankMargin()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainTransNASDGroupByUniqueTransOrderByCustnoBankMargin") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete All
        public void DeleteAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainTransNASDDeleteAll") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Get A Bargain Trans Info Given CustNo,Stock,Price,BuySell Type,Cross Deal
        public void GetBargainTransNASDGivenCustStockPriceTypeCD()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainTransNASDSelectByUniqueTransOrderByCustno") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Unitprice", SqlDbType.Decimal, decUnitPrice);
            db.AddInParameter(dbCommand, "Buy_Sold_Ind", SqlDbType.Char, strBuy_Sold_Ind.Trim());
            db.AddInParameter(dbCommand, "CrossD", SqlDbType.Char, strCrossD.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length >= 1)
            {
                strCustNo = thisRow[0]["CustNo"].ToString();
                strTransNo = thisRow[0]["TransNo"].ToString();
                datiDate = DateTime.ParseExact(thisRow[0]["Date"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                strBslip = thisRow[0]["Bslip#"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                intUnits = int.Parse(thisRow[0]["Units"].ToString());
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                decConsideration = decimal.Parse(thisRow[0]["Consideration"].ToString());
                decRConsideration = decimal.Parse(thisRow[0]["RConsideration"].ToString());
                strStockType = thisRow[0]["StockType"].ToString();
                strBuy_Sold_Ind = thisRow[0]["Buy_Sold_Ind"].ToString();
                strAllot_Ind = thisRow[0]["Allot_Ind"].ToString();
                strSing_Mult_Ind = thisRow[0]["Sing_Mult_Ind"].ToString();
                strSoldBy = thisRow[0]["SoldBy"].ToString();
                strBoughtBy = thisRow[0]["BoughtBy"].ToString();
                strNASDAcctNo = thisRow[0]["NASDAcctNo"].ToString();
            }
            else
            {
                throw new Exception();
            }
        }
        #endregion

        #region Get All Cross Deals For Bargain Trans
        public DataSet GetAllBargainTransNASDCrossDeal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BargainTransNASDSelectAllCrossDeal") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
