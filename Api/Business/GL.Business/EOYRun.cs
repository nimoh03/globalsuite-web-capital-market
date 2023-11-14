using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class EOYRun
    {
        #region Declarations
        private DateTime datRunStartDate;
        private DateTime datRunEndDate;
        private string strRunType;
        #endregion

        #region Properties
        public DateTime RunStartDate
        {
            set { datRunStartDate = value; }
            get { return datRunStartDate; }
        }
        public DateTime RunEndDate
        {
            set { datRunEndDate = value; }
            get { return datRunEndDate; }
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
            SqlCommand dbCommand = db.GetStoredProcCommand("EOYRunAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "RunStartDate", SqlDbType.DateTime, datRunStartDate);
            db.AddInParameter(dbCommand, "RunEndDate", SqlDbType.DateTime, datRunEndDate);
            db.AddInParameter(dbCommand, "RunType", SqlDbType.Char, strRunType.Trim());
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        
        #region Get All End Run
        public DataSet GetAllEndRun()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EOYRunSelectAllEndRun") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Year End Date
        public DateTime GetYearEndDate()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EOYRunSelectAllYearEndDate") as SqlCommand;
            db.AddInParameter(dbCommand, "RunStartDate", SqlDbType.DateTime, datRunStartDate);
            return DateTime.ParseExact(db.ExecuteScalar(dbCommand).ToString().Trim().Substring(0,10),"dd/MM/yyyy",format);
        }
        #endregion

        #region Check Dates In EOY Run
        public bool CheckDateInEOYRun()
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EOYRunSelectDateInEOY") as SqlCommand;
            db.AddInParameter(dbCommand, "RunStartDate", SqlDbType.DateTime, datRunStartDate);
            db.AddInParameter(dbCommand, "RunEndDate", SqlDbType.DateTime, datRunEndDate);
            db.AddInParameter(dbCommand, "RunType", SqlDbType.Char, strRunType.Trim());
            if(db.ExecuteDataSet(dbCommand).Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

    }
}
