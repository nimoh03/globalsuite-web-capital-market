using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class EODRun
    {
        #region Declarations
        private DateTime datEODRunDateLast, datEODRunDateNew;
        private string strRunType;
        #endregion

        #region Properties
        public DateTime EODRunDateLast
        {
            set { datEODRunDateLast = value; }
            get { return datEODRunDateLast; }
        }
        public DateTime EODRunDateNew
        {
            set { datEODRunDateNew = value; }
            get { return datEODRunDateNew; }
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
            SqlCommand dbCommand = db.GetStoredProcCommand("EODRunAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "EODRunDateLast", SqlDbType.DateTime, datEODRunDateLast);
            db.AddInParameter(dbCommand, "EODRunDateNew", SqlDbType.DateTime, datEODRunDateNew);
            db.AddInParameter(dbCommand, "RunType", SqlDbType.Char, strRunType.Trim());
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion
    }
}
