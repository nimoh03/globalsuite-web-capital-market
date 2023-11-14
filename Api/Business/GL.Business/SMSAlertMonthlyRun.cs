using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class SMSAlertMonthlyRun
    {
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
            SqlCommand dbCommand = db.GetStoredProcCommand("SMSAlertMonthlyRunAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "RunDate", SqlDbType.DateTime, datRunDate);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, strUserID.Trim());
            return dbCommand;
        }
        #endregion

        #region Get Monthly Run
        public bool GetSMSAlertMonthlyRun()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SMSAlertMonthlyRunSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "RunDate", SqlDbType.VarChar, datRunDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["RunDate"].ToString() == "" || thisRow[0]["RunDate"].ToString() == null)
                {
                    datRunDate = DateTime.MinValue;
                }
                else
                {
                    datRunDate = DateTime.ParseExact(thisRow[0]["RunDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                strUserID = thisRow[0]["UserID"].ToString();
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
            SqlCommand dbCommand = db.GetStoredProcCommand("SMSAlertMonthlyRunSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SMSAlertMonthlyRunDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RunDate", SqlDbType.VarChar, datRunDate);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Check Monthly Run Exist
        public bool ChkMonthlyRunExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("SMSAlertMonthlyRunChkMonthYearExist") as SqlCommand;
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
        public DateTime GetSMSAlertNextMonthlyRun()
        {
            string strResult;
            DateTime datNewDate;
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SMSAlertMonthlyRunSelectLast") as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                datNewDate = DateTime.ParseExact(db.ExecuteScalar(dbCommand).ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).AddMonths(1);
                strResult = GeneralFunc.GetMonthLastDay(DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Month, DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Year) + "/" +
                    DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                        DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Year.ToString();
            }
            else
            {
                datNewDate = GeneralFunc.GetTodayDate().AddMonths(-1);
                strResult = GeneralFunc.GetMonthLastDay(DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Month, DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Year) + "/" +
                    DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                        DateTime.ParseExact(datNewDate.ToString().Substring(0, 10), "dd/MM/yyyy", format).Year.ToString();
            }
            return DateTime.ParseExact(strResult, "dd/MM/yyyy", format);
        }
        #endregion

    }
}
