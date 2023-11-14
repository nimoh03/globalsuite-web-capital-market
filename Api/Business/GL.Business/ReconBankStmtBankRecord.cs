using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ReconBankRecord
    {
        #region Declaration
        private Int64 intTransNo, intReconBankStmtTransaction;
        private string strDescription,strDebCred;
        private DateTime datTransDate, datValueDate;
        private decimal decCredit, decDebit;
        #endregion

        #region Properties
        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public Int64 ReconBankStmtTransaction
        {
            set { intReconBankStmtTransaction = value; }
            get { return intReconBankStmtTransaction; }
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
        public string Description
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
        
        public string DebCred
        {
            set { strDebCred = value; }
            get { return strDebCred; }
        }
        

        #endregion

        #region Add Return Command
        public SqlCommand AddCommand()
        {
            SqlCommand oCommand = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            oCommand = db.GetStoredProcCommand("ReconBankStmtBankRecordAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
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
            db.AddInParameter(oCommand, "DebCred", SqlDbType.VarChar, strDebCred.Trim());
            db.AddInParameter(oCommand, "Credit", SqlDbType.Decimal, decCredit);
            db.AddInParameter(oCommand, "Debit", SqlDbType.Decimal, decDebit);
            return oCommand;

        }
        #endregion


        #region Get All By ReconBankStmtTran
        public DataSet GetAllByReconBankStmtTran()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtBankRecordSelectAllByReconBankStmtTran") as SqlCommand;
            db.AddInParameter(dbCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankStmtBankRecordDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get All UnMatched By ReconBankStmtTranBank
        public DataSet GetAllUnMatchedByReconBankStmtTranBank()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtBankRecordSelectAllUnMatchedByReconBankStmtTran") as SqlCommand;
            db.AddInParameter(dbCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Balance By ReconBankStmtTran
        public decimal GetBalanceByReconBankStmtTran()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtBankRecordSelectBalanceByReconBankStmtTran") as SqlCommand;
            db.AddInParameter(dbCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            var varBalance =  db.ExecuteScalar(dbCommand);
            return varBalance != null ? decimal.Parse(varBalance.ToString()) : 0;
        }
        #endregion

        #region Update Exclude Transaction
        public void UpdateExcludeTransaction()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ReconBankStmtBankRecordUpdateExcludeTransaction") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Reset Exclude Transaction
        public void ResetExcludeTransaction(long lngReconcilationNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ReconBankStmtBankRecordResetExcludeTransaction") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngReconcilationNumber);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion
    }
}
