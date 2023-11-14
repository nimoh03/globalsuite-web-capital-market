using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Data.SqlTypes;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class PortOpenBal
    {
        #region Declaration

        private string strTransNo, strTransNoRev;
        private DateTime datEffDate, datEffDateTo, datTxnDate, datTxnDateTo;
        private string strProduct, strStockCode;
        private long intUnits;
        private float fltUnitprice;
        private string strCustNo;
        private int intBroker;
        private string strUserId;
        private Decimal decTotalAmount;
        private Decimal decTotalFee;
        private string strSaveType;
        private string strPosted;
        private string strReversed;
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }

        }
        public string TransNoRev
        {
            set { strTransNoRev = value; }
            get { return strTransNoRev; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public DateTime EffDateTo
        {
            set { datEffDateTo = value; }
            get { return datEffDateTo; }
        }
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }
        public DateTime TxnDateTo
        {
            set { datTxnDateTo = value; }
            get { return datTxnDateTo; }
        }

        public string Product
        {
            set { strProduct = value; }
            get { return strProduct; }

        }
        public string Stockcode
        {
            set { strStockCode = value; }
            get { return strStockCode; }

        }
        public long Units
        {
            set { intUnits = value; }
            get { return intUnits; }
        }
        public float UnitPrice
        {
            set { fltUnitprice = value; }
            get { return fltUnitprice; }
        }
        public String CustNo
        {
            set { strCustNo = value; }
            get { return strCustNo; }
        }
        public int Broker
        {
            set { intBroker = value; }
            get { return intBroker; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        public Decimal TotalAmount
        {
            set { decTotalAmount = value; }
            get { return decTotalAmount; }

        }
        public Decimal TotalFee
        {
            set { decTotalFee = value; }
            get { return decTotalFee; }
        }

        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        public string Posted
        {
            set { strPosted = value; }
            get { return strPosted; }
        }
        public string Reversed
        {
            set { strReversed = value; }
            get { return strReversed; }
        }

        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand oCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        oCommand = db.GetStoredProcCommand("PortOBalAdd") as SqlCommand;
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "STKBPORTOPENBAL");
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("PortOBalEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
                    db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
                    db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                    db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Float, fltUnitprice);
                    db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.VarChar, strPosted.Trim());
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
                    db.AddInParameter(oCommand, "TotalAmount", SqlDbType.Decimal, decTotalAmount);
                    db.AddInParameter(oCommand, "TotalFee", SqlDbType.Decimal, decTotalFee);
                    db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    db.ExecuteNonQuery(oCommand, transaction);
                    if (strTransNoRev.Trim() != "")
                    {
                        SqlCommand dbCommandDeleteReversal = DeleteReversalCommand();
                        db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                    }
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new Exception(err.Message);
                }
            }
            return enSaveStatus;
        }
        #endregion

        #region Save New Stock Holding Opening Balance and Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalAdd") as SqlCommand;
            db.AddOutParameter(oCommand, "TransNo", SqlDbType.VarChar, 8);
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Float, fltUnitprice);
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
            db.AddInParameter(oCommand, "Posted", SqlDbType.VarChar, strPosted.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
            db.AddInParameter(oCommand, "TotalAmount", SqlDbType.Decimal, decTotalAmount);
            db.AddInParameter(oCommand, "TotalFee", SqlDbType.Decimal, decTotalFee);
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "STKBPORTOPENBAL");
            return oCommand;
        }
        #endregion

        #region Get Stock Portfolio Opening Balance UnPosted
        public bool GetPortOpenBalUnPost()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalSelectByTransnoUnPosted") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datEffDate = DateTime.ParseExact(thisRow[0]["OpenDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
                strProduct = thisRow[0]["Product"] != null ? thisRow[0]["Product"].ToString() : "";
                strStockCode = thisRow[0]["StockCode"].ToString();
                intUnits = int.Parse(thisRow[0]["Units"].ToString());
                fltUnitprice = float.Parse(thisRow[0]["Unit Price"].ToString());
                decTotalAmount = decimal.Parse(thisRow[0]["Total Cost"].ToString());
                decTotalFee = decimal.Parse(thisRow[0]["TFees"].ToString());
                strCustNo = thisRow[0]["Customer Acct"].ToString();
                strUserId = thisRow[0]["UserId"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;

        }
        #endregion

        #region Get Stock Portfolio Opening Balance Posted
        public bool GetPortOpenBalPosted()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalSelectPosted") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datEffDate = DateTime.ParseExact(thisRow[0]["OpenDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
                strProduct = thisRow[0]["Product"] != null ? thisRow[0]["Product"].ToString() : "";
                strStockCode = thisRow[0]["StockCode"].ToString();
                intUnits = int.Parse(thisRow[0]["Units"].ToString());
                fltUnitprice = float.Parse(thisRow[0]["Unit Price"].ToString());
                decTotalAmount = decimal.Parse(thisRow[0]["Total Cost"].ToString());
                decTotalFee = decimal.Parse(thisRow[0]["TFees"].ToString());
                strCustNo = thisRow[0]["Customer Acct"].ToString();
                strUserId = thisRow[0]["UserId"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }


            return blnStatus;

        }
        #endregion

        #region Get Stock Portfolio Opening Balance Reversed
        public bool GetPortOpenBalReversed()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalSelectReversed") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datEffDate = DateTime.ParseExact(thisRow[0]["OpenDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
                strProduct = thisRow[0]["Product"] != null ? thisRow[0]["Product"].ToString() : "";
                strStockCode = thisRow[0]["StockCode"].ToString();
                intUnits = int.Parse(thisRow[0]["Units"].ToString());
                fltUnitprice = float.Parse(thisRow[0]["Unit Price"].ToString());
                decTotalAmount = decimal.Parse(thisRow[0]["Total Cost"].ToString());
                decTotalFee = decimal.Parse(thisRow[0]["TFees"].ToString());
                strCustNo = thisRow[0]["Customer Acct"].ToString();
                strUserId = thisRow[0]["UserId"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }


            return blnStatus;

        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("PortOBalChkTransNoExistUnPosted") as SqlCommand;
                }
                else if (ePostStatus == DataGeneral.PostStatus.Posted)
                {
                    oCommand = db.GetStoredProcCommand("PortOBalChkTransNoExistPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());


            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                    IFormatProvider format = new CultureInfo("en-GB");

                    db.ExecuteNonQuery(oCommand, transaction);
                    transaction.Commit();
                    blnStatus = true;

                }
                catch (Exception e)
                {
                    string your = e.Message;
                    transaction.Rollback();
                    blnStatus = false;
                    return blnStatus;

                }
                connection.Close();
            }
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllPostedAsc") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Update Post and Return A Command
        public SqlCommand UpDatePostCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalUpdateRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
            return oCommand;
        }
        #endregion

        #region Get All Portfolio Open Bal Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datTxnDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datTxnDateTo);
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Portfolio Open Bal Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datEffDateTo);
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Portfolio Open Bal Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo, string strCustomer)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PortOBalSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortOBalDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion


        public class PortClearedAppToPortfolio
        {
            public string TransNo { set; get; }

            #region Save Cleared App To Portfolio and Return Command
            public SqlCommand SaveClearedAppToPortfolioCommand()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("PortClearedAppToPortfolioAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "TransNo", SqlDbType.BigInt, 8);
                db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                return oCommand;
            }
            #endregion
        }
    }
}
