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
    public class BonusDividend
    {
        #region Declarations
        private string strTransNo, strTransNoRev;
        private DateTime datEffDate;
        private string strCustomer;
        private string strStockcode;
        private string strReturnType;
        private decimal decDividendUnit;
        private decimal decDividendAmt;
        private int    intBonusUnit;
        private int    intShareNo;
        private int intBonusAmt;
        private DateTime datClosureDate, datQualificationDate, datPaymentDate;
        private string strDescription;
        private string strUserID;
        private bool blnPosted;
        private bool blnReversed;
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
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public string Customer
        {
            set { strCustomer = value; }
            get { return strCustomer; }
        }
        public string Stockcode
        {
            set { strStockcode = value; }
            get { return strStockcode; }
        }
        public string ReturnType
        {
            set { strReturnType = value; }
            get { return strReturnType; }
        }
        public decimal DividendUnit
        {
            set { decDividendUnit = value; }
            get { return decDividendUnit; }
        }
        public decimal DividendAmt
        {
            set { decDividendAmt = value; }
            get { return decDividendAmt; }
        }
        public int BonusUnit
        {
            set { intBonusUnit = value; }
            get { return intBonusUnit; }
        }
        public int ShareNo
        {
            set { intShareNo = value; }
            get { return intShareNo; }
        }
        public int BonusAmt
        {
            set { intBonusAmt = value; }
            get { return intBonusAmt; }
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
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }

        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
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
                        oCommand = db.GetStoredProcCommand("BonusDividendAddNew") as SqlCommand;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("BonusDividendEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
                    db.AddInParameter(oCommand, "Stockcode", SqlDbType.VarChar, strStockcode.Trim());
                    db.AddInParameter(oCommand, "ReturnType", SqlDbType.VarChar, strReturnType.Trim());
                    db.AddInParameter(oCommand, "DividendUnit", SqlDbType.Money, decDividendUnit);
                    db.AddInParameter(oCommand, "DividendAmt", SqlDbType.Money, decDividendAmt);
                    db.AddInParameter(oCommand, "BonusUnit", SqlDbType.BigInt, intBonusUnit);
                    db.AddInParameter(oCommand, "ShareNo", SqlDbType.Int, intShareNo);
                    db.AddInParameter(oCommand, "BonusAmt", SqlDbType.Int, intBonusAmt);
                    db.AddInParameter(oCommand, "ClosureDate", SqlDbType.DateTime, datClosureDate);
                    db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
                    db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserID.Trim());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
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
                SqlCommand oCommand = db.GetStoredProcCommand("BonusDividendChkTransNoExistUnPosted") as SqlCommand;
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

        #region Get Bonus Offer
        public bool GetBonusDividend(DataGeneral.PostStatus TransStatus,DataGeneral.AllotmentType ReturnType)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ReturnType == DataGeneral.AllotmentType.Buy)
            {
                if (TransStatus == DataGeneral.PostStatus.UnPosted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendBuySelectUnPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Posted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendBuySelectPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Reversed)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendBuySelectReversed") as SqlCommand;
                }
            }
            else if (ReturnType == DataGeneral.AllotmentType.Sell)
            {
                if (TransStatus == DataGeneral.PostStatus.UnPosted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendSellSelectUnPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Posted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendSellSelectPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Reversed)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendSellSelectReversed") as SqlCommand;
                }
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                datEffDate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                datClosureDate = DateTime.Parse(thisRow[0]["ClosureDate"].ToString());
                strStockcode = thisRow[0]["StockCode"].ToString();
                strDescription = thisRow[0]["Description"].ToString();
                intBonusUnit = int.Parse(thisRow[0]["BonusUnit"].ToString());
                intShareNo = int.Parse(thisRow[0]["ShareNo"].ToString());
                if (thisRow[0]["BonusAmt"] != null && thisRow[0]["BonusAmt"].ToString().Trim() != "")
                {
                    intBonusAmt = int.Parse(thisRow[0]["BonusAmt"].ToString());
                }
                else
                {
                    intBonusAmt = 0;
                }
                if (thisRow[0]["DividendUnit"] != null && thisRow[0]["DividendUnit"].ToString().Trim() != "")
                {
                    decDividendUnit = decimal.Parse(thisRow[0]["DividendUnit"].ToString());
                }
                else
                {
                    decDividendUnit = 0;
                }
                if (thisRow[0]["DividendAmt"] != null && thisRow[0]["DividendAmt"].ToString().Trim() != "")
                {
                    decDividendAmt = decimal.Parse(thisRow[0]["DividendAmt"].ToString());
                }
                else
                {
                    decDividendAmt = 0;
                }
                strCustomer = thisRow[0]["Customer"].ToString();
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
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
        public DataSet GetAll(DataGeneral.PostStatus TransStatus,DataGeneral.AllotmentType ReturnType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ReturnType == DataGeneral.AllotmentType.Buy)
            {
                if (TransStatus == DataGeneral.PostStatus.UnPosted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendBuySelectAllUnPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Posted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendBuySelectAllPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Reversed)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendBuySelectAllReversed") as SqlCommand;
                }
            }
            else if (ReturnType == DataGeneral.AllotmentType.Sell)
            {
                if (TransStatus == DataGeneral.PostStatus.UnPosted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendSellSelectAllUnPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Posted)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendSellSelectAllPosted") as SqlCommand;
                }
                else if (TransStatus == DataGeneral.PostStatus.Reversed)
                {
                    dbCommand = db.GetStoredProcCommand("BonusDividendSellSelectAllReversed") as SqlCommand;
                }
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete 
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BonusDividendDelete") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("BonusDividendUpdateRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BonusDividendTxnDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion
    }
}
