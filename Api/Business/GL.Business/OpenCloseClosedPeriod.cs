using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class OpenCloseClosedPeriod
    {
        #region Declarations
        private string strTransNo,strOpenOrClosePeriod,strPastOrCurrentYear;
        private DateTime datYearStartDate,datYearEndDate,datMonthDate;
        private DateTime datCurrentYearStartDate, datCurrentYearEndDate, datCurrentMonthDate;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string OpenOrClosePeriod
        {
            set { strOpenOrClosePeriod = value; }
            get { return strOpenOrClosePeriod; }
        }
        public string PastOrCurrentYear
        {
            set { strPastOrCurrentYear = value; }
            get { return strPastOrCurrentYear; }
        }
        public DateTime YearStartDate
        {
            set { datYearStartDate = value; }
            get { return datYearStartDate; }
        }
        public DateTime YearEndDate
        {
            set { datYearEndDate = value; }
            get { return datYearEndDate; }
        }
        public DateTime MonthDate
        {
            set { datMonthDate = value; }
            get { return datMonthDate; }
        }
        public DateTime CurrentYearStartDate
        {
            set { datCurrentYearStartDate = value; }
            get { return datCurrentYearStartDate; }
        }
        public DateTime CurrentYearEndDate
        {
            set { datCurrentYearEndDate = value; }
            get { return datCurrentYearEndDate; }
        }
        public DateTime CurrentMonthDate
        {
            set { datCurrentMonthDate = value; }
            get { return datCurrentMonthDate; }
        }
        #endregion

        #region Add and Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OpenCloseClosedPeriodAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "OpenOrClosePeriod", SqlDbType.VarChar, strOpenOrClosePeriod.Trim());
            db.AddInParameter(dbCommand, "PastOrCurrentYear", SqlDbType.VarChar, strPastOrCurrentYear.Trim());
            db.AddInParameter(dbCommand, "YearStartDate", SqlDbType.DateTime, datYearStartDate);
            db.AddInParameter(dbCommand, "YearEndDate", SqlDbType.DateTime, datYearEndDate);
            db.AddInParameter(dbCommand, "MonthDate", SqlDbType.DateTime, datMonthDate != DateTime.MinValue ? datMonthDate : SqlDateTime.Null);
            db.AddInParameter(dbCommand, "CurrentYearStartDate", SqlDbType.DateTime, datCurrentYearStartDate);
            db.AddInParameter(dbCommand, "CurrentYearEndDate", SqlDbType.DateTime, datCurrentYearEndDate);
            db.AddInParameter(dbCommand, "CurrentMonthDate", SqlDbType.DateTime, datCurrentMonthDate != DateTime.MinValue ? datCurrentMonthDate : SqlDateTime.Null);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Get Last Transaction Number
        public long GetLastTransactionNumber()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OpenCloseClosedPeriodSelectLastNumber") as SqlCommand;
            var varLastNumber = db.ExecuteScalar(dbCommand);
            return varLastNumber != null && varLastNumber.ToString().Trim() != "" ? long.Parse(varLastNumber.ToString()) : 0;
        }
        #endregion

        #region Get Open Close Closed Period
        public bool GetOpenCloseClosedPeriod()
        {
            bool blnStatus = false;
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("OpenCloseClosedPeriodSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strOpenOrClosePeriod = thisRow[0]["OpenOrClosePeriod"].ToString();
                strPastOrCurrentYear = thisRow[0]["PastOrCurrentYear"].ToString();
                datYearStartDate = thisRow[0]["YearStartDate"] != null && thisRow[0]["YearStartDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["YearStartDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                datYearEndDate = thisRow[0]["YearEndDate"] != null && thisRow[0]["YearEndDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["YearEndDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                datMonthDate = thisRow[0]["MonthDate"] != null && thisRow[0]["MonthDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["MonthDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                datCurrentYearStartDate = thisRow[0]["CurrentYearStartDate"] != null && thisRow[0]["CurrentYearStartDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["CurrentYearStartDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                datCurrentYearEndDate = thisRow[0]["CurrentYearEndDate"] != null && thisRow[0]["CurrentYearEndDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["CurrentYearEndDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                datCurrentMonthDate = thisRow[0]["CurrentMonthDate"] != null && thisRow[0]["CurrentMonthDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["CurrentMonthDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;

                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
    }
}
