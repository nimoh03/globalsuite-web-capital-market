using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Admin.Business
{
    public class SchoolBank
    {

        #region Properties
        public Int64 SchoolBankId { get; set; }
        public Int32 SchoolId { get; set; }
        public string BankId { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("SchoolBankAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                if (SchoolBankId != 0)
                {
                    oCommand = db.GetStoredProcCommand("SchoolBankEdit") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("SchoolBankAdd") as SqlCommand;
                }
            }
            db.AddInParameter(oCommand, "SchoolBankId", SqlDbType.BigInt, SchoolBankId);
            db.AddInParameter(oCommand, "SchoolId", SqlDbType.BigInt, SchoolId);
            db.AddInParameter(oCommand, "BankId", SqlDbType.VarChar, BankId.Trim());
            db.AddInParameter(oCommand, "AccountNo", SqlDbType.VarChar, AccountNo.Trim());
            db.AddInParameter(oCommand, "AccountName", SqlDbType.VarChar, AccountName.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Get
        public bool GetSchoolBank(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, SchoolBankId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                SchoolBankId = Convert.ToInt64(thisRow[0]["SchoolBankId"]);
                SchoolId = Convert.ToInt32(thisRow[0]["SchoolId"]);
                BankId = thisRow[0]["BankId"].ToString();
                AccountNo = thisRow[0]["AccountNo"].ToString();
                AccountName = thisRow[0]["AccountName"].ToString();
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check SchoolBankId Exist
        public bool ChkSchoolBankIdExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("SchoolBankChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "Transno", SqlDbType.Int, SchoolBankId);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            else if (SaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Get
        public DataSet GetSchoolBankBySchoolId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SchoolBankSelectBySchoolId") as SqlCommand;
            db.AddInParameter(dbCommand, "SchoolId", SqlDbType.Int, SchoolId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete(string strTableName, string strColumnName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, SchoolBankId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion

        #region Get First School Bank Id
        public Int64 GetFirstSchoolBankId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SchoolBankSelectFirstId") as SqlCommand;
            var varFirstId = db.ExecuteScalar(oCommand);
            return varFirstId != null && varFirstId.ToString().Trim() != "" ? Convert.ToInt64(varFirstId) : 0;
        }
        #endregion

        #region Get Second School Bank Id
        public Int64 GetSecondSchoolBankId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SchoolBankSelectSecondId") as SqlCommand;
            var varFirstId = db.ExecuteScalar(oCommand);
            return varFirstId != null && varFirstId.ToString().Trim() != "" ? Convert.ToInt64(varFirstId) : 0;
        }
        #endregion
    }
}









    