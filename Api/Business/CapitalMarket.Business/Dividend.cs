using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class Dividend
    {
        #region Declarations
        private string strTransNo, strTransNoRev;
        private DateTime datDividendDate, datClosureDate,datQualificationDate,datPaymentDate;
        private decimal decDividendUnit;
        private string strStockCode, strComment;
        private string strPosted, strReversed;
        private string strSaveType;
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
        public decimal DividendUnit
        {
            set { decDividendUnit = value; }
            get { return decDividendUnit; }
        }
        public DateTime DividendDate
        {
            set { datDividendDate = value; }
            get { return datDividendDate; }
        }
        public DateTime ClosureDate
        {
            set { datClosureDate = value; }
            get { return datClosureDate; }
        }
        public DateTime QualificationDate
        {
            set { datQualificationDate = value; }
            get { return datQualificationDate; }
        }
        public DateTime PaymentDate
        {
            set { datPaymentDate = value; }
            get { return datPaymentDate; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }

        public string Comment
        {
            set { strComment = value; }
            get { return strComment; }
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

        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
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
                        oCommand = db.GetStoredProcCommand("DividendTxnsAdd") as SqlCommand;
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "DIVIDEND");


                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("DividendTxnsEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "DividendDate", SqlDbType.DateTime, datDividendDate);
                    db.AddInParameter(oCommand, "ClosureDate", SqlDbType.DateTime, datClosureDate);
                    db.AddInParameter(oCommand, "QualificationDate", SqlDbType.DateTime, datQualificationDate);
                    db.AddInParameter(oCommand, "PaymentDate", SqlDbType.DateTime, datPaymentDate);
                    db.AddInParameter(oCommand, "DividendUnit", SqlDbType.Money, decDividendUnit);
                    db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                    db.AddInParameter(oCommand, "Comment", SqlDbType.VarChar, strComment.Trim());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.VarChar, strPosted.Trim());
                    db.AddInParameter(oCommand, "Reverse", SqlDbType.VarChar, strReversed.Trim());
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
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

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("DividendTxnChkTransNoExist") as SqlCommand;
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

        #region Get Dividend Offer
        public bool GetDividend(DataGeneral.PostStatus TransStatus)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("DividendTxnSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DividendTxnSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("DividendTxnSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                datDividendDate = DateTime.Parse(thisRow[0]["Date"].ToString());
                datClosureDate = DateTime.Parse(thisRow[0]["ClosureDate"].ToString());
                datQualificationDate = DateTime.Parse(thisRow[0]["QualificationDate"].ToString()); 
                datPaymentDate = DateTime.Parse(thisRow[0]["PaymentDate"].ToString());
                strStockCode = thisRow[0]["StockCode"].ToString();
                strComment = thisRow[0]["Comments"].ToString();
                decDividendUnit = decimal.Parse(thisRow[0]["DividendPerShare"].ToString());
                strPosted = thisRow[0]["Posted"].ToString();
                strReversed = thisRow[0]["Reverse"].ToString();

                blnStatus = true;
            }
            else
            {
                blnStatus = false;
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
                dbCommand = db.GetStoredProcCommand("DividendTxnSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DividendTxnSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("DividendTxnSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Bonus Due
        public DataSet GetDividendDue()
        {
            StkParam oStkParam = new StkParam();
            DataSet oDS = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DividendTxnSelectDue") as SqlCommand;
            db.AddInParameter(dbCommand, "DividendUnit", SqlDbType.Real, decDividendUnit);
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product.Trim());
            db.AddInParameter(dbCommand, "ReportDate", SqlDbType.DateTime, datClosureDate);
            oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DividendTxnDelete") as SqlCommand;
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
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new Exception(err.Message);
                }
                connection.Close();
            }
            return blnStatus;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DividendTxnUpdateRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DividendTxnDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion
    }
}
