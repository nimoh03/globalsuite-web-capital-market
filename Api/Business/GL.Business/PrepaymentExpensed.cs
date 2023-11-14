using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class PrepaymentExpensed
    {
        #region Declaration

        private string strTransNo;
        private long lngPrepaymentNumber;
        private long lngPrepaymentScheduleNumber;
        private DateTime datEffDate;
        private decimal decAmount;
        private string strDescription;
        private bool blnPosted, blnReversed;
        private string strSaveType;
        private DateTime datTxnDate;
        private DateTime datEffDateTo, datTxnDateTo;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public long PrepaymentNumber
        {
            set { lngPrepaymentNumber = value; }
            get { return lngPrepaymentNumber; }
        }
        public long PrepaymentScheduleNumber
        {
            set { lngPrepaymentScheduleNumber = value; }
            get { return lngPrepaymentScheduleNumber; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
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
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }
        public DateTime EffDateTo
        {
            set { datEffDateTo = value; }
            get { return datEffDateTo; }
        }
        public DateTime TxnDateTo
        {
            set { datTxnDateTo = value; }
            get { return datTxnDateTo; }
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlTransaction transaction;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand dbCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("PrepaymentExpensedAdd") as SqlCommand;
                        db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 8);
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("PrepaymentExpensedEdit") as SqlCommand;
                        db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    }
                    db.AddInParameter(dbCommand, "PrepaymentNumber", SqlDbType.BigInt, lngPrepaymentNumber);
                    db.AddInParameter(dbCommand, "PrepaymentScheduleNumber", SqlDbType.BigInt, lngPrepaymentScheduleNumber);
                    db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
                    db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (strSaveType == "ADDS")
                    {
                        strTransNo = db.GetParameterValue(dbCommand, "TransNo").ToString();
                    }
                    PrepaymentSchedule oPrepaymentSchedule = new PrepaymentSchedule();
                    oPrepaymentSchedule.TransNo = lngPrepaymentScheduleNumber.ToString();
                    oPrepaymentSchedule.ExpensedNumber =  long.Parse(strTransNo.Trim());
                    if (strSaveType == "EDIT")
                    {
                        oPrepaymentSchedule.UpdateExpensedNumberToZero(ref transaction);
                    }
                    oPrepaymentSchedule.UpdateExpensedNumber(ref transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllReversed") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Prepayment Expensed Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datTxnDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datTxnDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Prepayment Expensed Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datEffDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Prepayment Expensed Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetPrepaymentExpensed(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                lngPrepaymentNumber = long.Parse(thisRow[0]["PrepaymentNumber"].ToString());
                lngPrepaymentScheduleNumber = long.Parse(thisRow[0]["PrepaymentScheduleNumber"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                datEffDate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                strDescription = thisRow[0]["Description"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Number Of Expensed By Prepayment Posted
        public int GetNumberOfExpensedByPrepaymentPosted()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PrepaymentExpensedSelectNumberByPrepaymentPosted") as SqlCommand;
            db.AddInParameter(oCommand, "PrepaymentNumber", SqlDbType.BigInt, lngPrepaymentNumber);
            return int.Parse(db.ExecuteScalar(oCommand).ToString());
        }
        #endregion

        #region Update Post and Return A Command
        public SqlCommand UpDatePostCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PrepaymentExpensedUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

    }
}
