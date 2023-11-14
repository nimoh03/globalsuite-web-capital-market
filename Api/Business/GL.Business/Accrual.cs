using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Accrual
    {
        #region Declaration
        private string strTransNo, strIncomeOrExpense, strIncomeExpenseAccount, strReceablePayableAccount;
        private DateTime datEffDate, datDueDate;
        private string strDescription,strRecPVNumber;
        private decimal decAmount;
        private bool blnPosted, blnReversed;
        private string strSaveType;
        #endregion


        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string IncomeOrExpense
        {
            set { strIncomeOrExpense = value; }
            get { return strIncomeOrExpense; }
        }
        public string IncomeExpenseAccount
        {
            set { strIncomeExpenseAccount = value; }
            get { return strIncomeExpenseAccount; }
        }
        public string ReceablePayableAccount
        {
            set { strReceablePayableAccount = value; }
            get { return strReceablePayableAccount; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public DateTime DueDate
        {
            set { datDueDate = value; }
            get { return datDueDate; }
        }
        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public string RecPVNumber
        {
            set { strRecPVNumber = value; }
            get { return strRecPVNumber; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
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
                    SqlCommand dbCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("AccrualAddNew") as SqlCommand;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("AccrualEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(dbCommand, "IncomeOrExpense", SqlDbType.VarChar, strIncomeOrExpense.Trim());
                    db.AddInParameter(dbCommand, "ReceablePayableAccount", SqlDbType.VarChar, strReceablePayableAccount.Trim());
                    db.AddInParameter(dbCommand, "IncomeExpenseAccount", SqlDbType.VarChar, strIncomeExpenseAccount.Trim());
                    db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
                    db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
                    db.AddInParameter(dbCommand, "RecPVNumber", SqlDbType.VarChar, strRecPVNumber.Trim());
                    db.AddInParameter(dbCommand, "DueDate", SqlDbType.DateTime, datDueDate);
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.VarChar, blnPosted);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.VarChar, blnReversed);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);

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
                    oCommand = db.GetStoredProcCommand("AccrualChkTransNoExistUnPosted") as SqlCommand;
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

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectAllReversed") as SqlCommand;
            }

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Due
        public DataSet GetAllDue()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AccrualSelectAllDue") as SqlCommand;
            db.AddInParameter(dbCommand, "DueDate", SqlDbType.DateTime, datDueDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetAccrual(DataGeneral.PostStatus TranStatus)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectUnPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("AccrualSelectReversed") as SqlCommand;
            } 
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datEffDate = DateTime.ParseExact(thisRow[0]["EffDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                strIncomeOrExpense = thisRow[0]["IncomeOrExpense"].ToString();
                datDueDate = DateTime.ParseExact(thisRow[0]["DueDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strDescription = thisRow[0]["Description"].ToString();
                strIncomeExpenseAccount = thisRow[0]["IncomeExpenseAccount"].ToString();
                strReceablePayableAccount = thisRow[0]["ReceablePayableAccount"].ToString();
                strRecPVNumber = thisRow[0]["RecPVNumber"].ToString();
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

        #region Delete
        public bool Delete()
        {
            return false;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccrualUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

    }
}
