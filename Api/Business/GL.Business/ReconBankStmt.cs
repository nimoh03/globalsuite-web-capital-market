using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ReconBankStmt
    {
        #region Declaration
        private string strTransNo, strDebCred, strBankAcctCode, strDescription;
        private DateTime datTransDate, datValueDate;
        private decimal decCredit, decDebit;
        private string strReportType, strCounterMaster, strCounterAccount, strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string BankAcctCode
        {
            set { strBankAcctCode = value; }
            get { return strBankAcctCode; }
        }

        public DateTime TransDate
        {
            set { datTransDate = value; }
            get { return datTransDate; }
        }
        public DateTime ValueDate
        {
            set { datValueDate = value; }
            get { return datValueDate; }
        }
        public string  Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }

        public decimal Credit
        {
            set { decCredit = value; }
            get { return decCredit; }
        }
        public decimal Debit
        {
            set { decDebit = value; }
            get { return decDebit; }
        }
        public string  DebCred
        {
            set { strDebCred = value; }
            get { return strDebCred; }
        }
        public string ReportType
        {
            set { strReportType = value; }
            get { return strReportType; }
        }
        public string CounterMaster
        {
            set { strCounterMaster = value; }
            get { return strCounterMaster; }
        }
        public string CounterAccount
        {
            set { strCounterAccount = value; }
            get { return strCounterAccount; }
        }
       
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        #endregion

        #region Add 
        public void AddNew()
        {
            SqlCommand oCommand = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            oCommand = db.GetStoredProcCommand("ReconBankStmtAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "BankAcctCode", SqlDbType.VarChar, strBankAcctCode.Trim());
            if (datTransDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "TransDate", SqlDbType.DateTime, datTransDate);

            }
            else
            {
                db.AddInParameter(oCommand, "TransDate", SqlDbType.DateTime, System.Data.SqlTypes.SqlDateTime.Null);
            }
            if (datValueDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "ValueDate", SqlDbType.DateTime, datValueDate);

            }
            else
            {
                db.AddInParameter(oCommand, "ValueDate", SqlDbType.DateTime, System.Data.SqlTypes.SqlDateTime.Null);
            }
            db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
            db.AddInParameter(oCommand, "Credit", SqlDbType.Decimal, decCredit);
            db.AddInParameter(oCommand, "Debit", SqlDbType.Decimal, decDebit);
            db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, strDebCred.Trim());
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar,strReportType.Trim());
            db.AddInParameter(oCommand, "CounterMaster", SqlDbType.VarChar, strCounterMaster.Trim());
            db.AddInParameter(oCommand, "CounterAccount", SqlDbType.VarChar, strCounterAccount.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            
        }
        #endregion

        
        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete All
        public bool DeleteAll()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankStmtDeleteAll") as SqlCommand;
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion
    }
}
