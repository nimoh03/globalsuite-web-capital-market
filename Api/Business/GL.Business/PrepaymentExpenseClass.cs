using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class PrepaymentExpenseClass
    {
        #region Declaration

        private string strTransNo;
        private string strPrepaymentExpenseName;
        private string strPrepaidAssetAccount, strExpenseAccount,strSaveType;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string PrepaymentExpenseName
        {
            set { strPrepaymentExpenseName = value; }
            get { return strPrepaymentExpenseName; }
        }
        public string PrepaidAssetAccount
        {
            set { strPrepaidAssetAccount = value; }
            get { return strPrepaidAssetAccount; }
        }
        public string ExpenseAccount
        {
            set { strExpenseAccount = value; }
            get { return strExpenseAccount; }
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
                        dbCommand = db.GetStoredProcCommand("PrepaymentExpenseClassAddNew") as SqlCommand;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("PrepaymentExpenseClassEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(dbCommand, "PrepaymentExpenseName", SqlDbType.VarChar, strPrepaymentExpenseName.ToUpper());
                    db.AddInParameter(dbCommand, "PrepaidAssetAccount", SqlDbType.VarChar, strPrepaidAssetAccount);
                    db.AddInParameter(dbCommand, "ExpenseAccount", SqlDbType.VarChar, strExpenseAccount);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
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
        public bool GetPrepaymentExpenseClass()
        {
            bool blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PrepaymentExpenseClassSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strPrepaymentExpenseName = thisRow[0]["PrepaymentExpenseName"].ToString();
                strPrepaidAssetAccount = thisRow[0]["PrepaidAssetAccount"].ToString();
                strExpenseAccount = thisRow[0]["ExpenseAccount"].ToString();
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
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("PrepaymentExpenseClassSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
