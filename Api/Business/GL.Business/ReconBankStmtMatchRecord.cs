using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ReconBankStmtMatchRecord
    {
        #region Declaration
        private Int64 intTransNo, intReconBankStmtTransaction, intReconBankStmtBankRecord,intNextNo;
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

        public Int64 ReconBankStmtBankRecord
        {
            set { intReconBankStmtBankRecord = value; }
            get { return intReconBankStmtBankRecord; }
        }

        public Int64 NextNo
        {
            set { intNextNo = value; }
            get { return intNextNo; }
        }

        #endregion

        #region Add Return Command
        public SqlCommand AddCommand()
        {
            SqlCommand oCommand = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            oCommand = db.GetStoredProcCommand("ReconBankStmtMatchRecordAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            db.AddInParameter(oCommand, "ReconBankStmtBankRecord", SqlDbType.BigInt, intReconBankStmtBankRecord);
            db.AddInParameter(oCommand, "NextNo", SqlDbType.BigInt, intNextNo);
            return oCommand;
        }
        #endregion

        #region Save
        public void Save()
        {
            SqlCommand oCommand = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            oCommand = db.GetStoredProcCommand("ReconBankStmtMatchRecordAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            db.AddInParameter(oCommand, "ReconBankStmtBankRecord", SqlDbType.BigInt, intReconBankStmtBankRecord);
            db.AddInParameter(oCommand, "NextNo", SqlDbType.BigInt, intNextNo);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtMatchRecordSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankStmtMatchRecordDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Delete With Other Field
        public void DeleteWithOtherField()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReconBankStmtMatchRecordDeleteWithOtherField") as SqlCommand;
            db.AddInParameter(oCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            db.AddInParameter(oCommand, "ReconBankStmtBankRecord", SqlDbType.BigInt, intReconBankStmtBankRecord);
            db.AddInParameter(oCommand, "NextNo", SqlDbType.BigInt, intNextNo);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        

        #region Get All By ReconBankStmtTran GL
        public DataSet GetAllByReconBankStmtTranGL()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtMatchRecordSelectAllByReconBankStmtTranGL") as SqlCommand;
            db.AddInParameter(dbCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By ReconBankStmtTran Bank
        public DataSet GetAllByReconBankStmtTranBank()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ReconBankStmtMatchRecordSelectAllByReconBankStmtTranBank") as SqlCommand;
            db.AddInParameter(dbCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconBankStmtTransaction);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
