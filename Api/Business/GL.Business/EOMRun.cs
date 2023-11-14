using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class EOMRun
    {
        #region Declarations
        private DateTime datRunDate;
        private string strRunType;
        #endregion

        #region Properties
        public DateTime RunDate
        {
            set { datRunDate = value; }
            get { return datRunDate; }
        }
        public string RunType
        {
            set { strRunType = value; }
            get { return strRunType; }
        }
        #endregion

        #region Add and Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EOMRunAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "RunDate", SqlDbType.DateTime, datRunDate);
            db.AddInParameter(dbCommand, "RunType", SqlDbType.Char, strRunType.Trim());
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Get All End Run
        public DataSet GetAllEndRun()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EOMRunSelectAllEndRun") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
