using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Prepayment
    {
        #region Declaration
        private string strTransNo;
        private DateTime datEffDate, datFirstDueDate;
        private decimal decAmount;
        private string strDescription, strBankAcct;
        private int intPrepaymentExpenseClassNumber;
        private string strPaymentFrequency;
        private int intNumberOfTimeExpensed;
        private string strChequeNo;
        private DataGeneral.GLInstrumentType enumInstrumentType;
        private bool blnPosted, blnReversed;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }

        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public DateTime FirstDueDate
        {
            set { datFirstDueDate = value; }
            get { return datFirstDueDate; }
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
        public string BankAcct
        {
            set { strBankAcct = value; }
            get { return strBankAcct; }
        }
        public int PrepaymentExpenseClassNumber
        {
            set { intPrepaymentExpenseClassNumber = value; }
            get { return intPrepaymentExpenseClassNumber; }
        }
        public string PaymentFrequency
        {
            set { strPaymentFrequency = value; }
            get { return strPaymentFrequency; }
        }
        public int NumberOfTimeExpensed
        {
            set { intNumberOfTimeExpensed = value; }
            get { return intNumberOfTimeExpensed; }
        }
        public string ChequeNo
        {
            set { strChequeNo = value; }
            get { return strChequeNo; }
        }
        public DataGeneral.GLInstrumentType InstrumentType
        {
            set { enumInstrumentType = value; }
            get { return enumInstrumentType; }
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
                        dbCommand = db.GetStoredProcCommand("PrepaymentAdd") as SqlCommand;
                        db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 8);
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("PrepaymentEdit") as SqlCommand;
                        db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    }
                    db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
                    db.AddInParameter(dbCommand, "FirstDueDate", SqlDbType.DateTime, datFirstDueDate);
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
                    db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
                    db.AddInParameter(dbCommand, "BankAcct", SqlDbType.VarChar, strBankAcct.Trim());
                    db.AddInParameter(dbCommand, "PrepaymentExpenseClassNumber", SqlDbType.Int, intPrepaymentExpenseClassNumber);
                    db.AddInParameter(dbCommand, "PaymentFrequency", SqlDbType.VarChar, strPaymentFrequency);
                    db.AddInParameter(dbCommand, "NumberOfTimeExpensed", SqlDbType.Int, intNumberOfTimeExpensed);
                    db.AddInParameter(dbCommand, "ChequeNo", SqlDbType.VarChar, strChequeNo.Trim());
                    db.AddInParameter(dbCommand, "InstrumentType", SqlDbType.VarChar, enumInstrumentType.ToString().Trim());
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (strSaveType == "ADDS")
                    {
                        strTransNo = db.GetParameterValue(dbCommand, "TransNo").ToString();
                    }
                    PrepaymentSchedule oPrepaymentSchedule = new PrepaymentSchedule();
                    oPrepaymentSchedule.PrepaymentNumber = long.Parse(strTransNo.Trim());
                    oPrepaymentSchedule.Save(ref transaction);

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

        #region Get
        public bool GetPrepayment(DataGeneral.PostStatus TranStatus)
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectUnPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim()); 
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datEffDate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                datFirstDueDate = DateTime.Parse(thisRow[0]["FirstDueDate"].ToString());
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strDescription = thisRow[0]["Description"].ToString();
                strBankAcct = thisRow[0]["BankAcct"].ToString();
                intPrepaymentExpenseClassNumber = int.Parse(thisRow[0]["PrepaymentExpenseClassNumber"].ToString());
                strPaymentFrequency = thisRow[0]["PaymentFrequency"].ToString();
                intNumberOfTimeExpensed = int.Parse(thisRow[0]["NumberOfTimeExpensed"].ToString());
                strChequeNo = thisRow[0]["ChequeNo"].ToString();
                enumInstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), thisRow[0]["InstrumentType"].ToString().Trim(), false);

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
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PrepaymentSelectAllReversed") as SqlCommand;
            }

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Edit Complete Payment And Return Command
        public SqlCommand EditCompletePaymentCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PrepaymentUpdateCompletePayment") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            return dbCommand;
        }
        #endregion
    }
}
