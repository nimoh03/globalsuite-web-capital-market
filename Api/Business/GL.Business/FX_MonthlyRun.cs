using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class FX_MonthlyRun
    {
        IFormatProvider format = new CultureInfo("en-GB");
        #region Declarations
        private DateTime datRunDate;

        private string strUserID;
        private string strSaveType;
        #endregion

        #region Properties
        public DateTime RunDate
        {
            set { datRunDate = value; }
            get { return datRunDate; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Add and Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand    dbCommand = db.GetStoredProcCommand("FX_MonthlyRunAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "RunDate", SqlDbType.DateTime, datRunDate);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, strUserID.Trim());
            return dbCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("FX_MonthlyRunSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("FX_MonthlyRunSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("FX_MonthlyRunSelectAllPostedAsc") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FX Monthly Run
        public bool GetFXMonthlyRun(DataGeneral.PostStatus TransStatus)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("FX_MonthlyRunSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("FX_MonthlyRunSelectReversed") as SqlCommand;
            }
            
            db.AddInParameter(dbCommand, "RunDate", SqlDbType.DateTime, datRunDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                datRunDate = DateTime.ParseExact(thisRow[0]["RunDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
               blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Update Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FX_MonthlyRunUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "RunDate", SqlDbType.DateTime, datRunDate);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Check Monthly Run Exist
        public bool ChkMonthlyRunExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FX_MonthlyRunChkMonthYearExist") as SqlCommand;
            db.AddInParameter(oCommand, "RunMonth", SqlDbType.Int, datRunDate.Month);
            db.AddInParameter(oCommand, "RunYear", SqlDbType.Int, datRunDate.Year);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
           

            return blnStatus;
        }
        #endregion

        #region Get Fixed Asset Next Monthly Run
        public DateTime GetFixedAssetNextMonthlyRun()
        {
            string strResult;
            DateTime datNewDate;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_MonthlyRunSelectLast") as SqlCommand;
            var varLastDate = db.ExecuteScalar(dbCommand);
            datNewDate = varLastDate != null && varLastDate.ToString().Trim() != "" ? DateTime.ParseExact(varLastDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddMonths(1) : DateTime.MinValue;
            if (datNewDate != DateTime.MinValue)
            {
                strResult = GeneralFunc.GetMonthLastDay(DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Month, DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Year) + "/" +
                DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Year.ToString();
            }
            else
            {
                throw new Exception("Fixed Asset Monthly Run Table Is Empty Or None Posted.Please Add Entry To The Fixed Asset Montly Run Table");
            }
            return DateTime.ParseExact(strResult, "dd/MM/yyyy", format);
        }
        #endregion

        #region Get Last Fixed Asset Monthly Run
        public DateTime GetLastFixedAssetMonthlyRun()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_MonthlyRunSelectLast") as SqlCommand;
            var varLastDate = db.ExecuteScalar(dbCommand);
            return varLastDate != null && varLastDate.ToString().Trim() != "" ? DateTime.ParseExact(varLastDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion

    }
}