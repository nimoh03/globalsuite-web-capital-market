using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Period
    {
        #region Properties
        public Int64 PeriodId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Opened { get; set; }
        public bool CurrentPeriod { get; set; }
        public string UserId { get; set; }
        public Int64 LocationId { get; set; }
        public Int32 CompanyId { get; set; }
        #endregion

        #region Save
        public List<string> Save()
        {
            List<string> oErrMsg = new List<string>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;

            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("PeriodAdd") as SqlCommand;
            db.AddInParameter(oCommand, "PeriodId", SqlDbType.BigInt, PeriodId);
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "StartDate", SqlDbType.Date, StartDate);
            db.AddInParameter(oCommand, "EndDate", SqlDbType.Date, EndDate);
            db.AddInParameter(oCommand, "Opened", SqlDbType.Bit, Opened);
            db.AddInParameter(oCommand, "CurrentPeriod", SqlDbType.Bit, CurrentPeriod);
            db.AddInParameter(oCommand, "UserId", SqlDbType.Int, UserId.Trim());
            db.AddInParameter(oCommand, "CompanyId", SqlDbType.BigInt, CompanyId);
            db.AddInParameter(oCommand, "LocationId", SqlDbType.BigInt, LocationId);
            db.ExecuteNonQuery(oCommand);
            var varErrMsg = db.GetParameterValue(oCommand, "SaveErrMsg").ToString();
            if (varErrMsg.Trim() != "")
            {
                oErrMsg.Add(varErrMsg);
            }
            return oErrMsg;
        }
        #endregion

        #region Get
        public bool GetPeriod(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.Int, PeriodId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                PeriodId =Convert.ToInt64( thisRow[0]["PeriodId"]);
                StartDate = Convert.ToDateTime(thisRow[0]["StartDate"]);
                EndDate = Convert.ToDateTime(thisRow[0]["EndDate"]);
                Opened = Convert.ToBoolean(thisRow[0]["Opened"]);
                CurrentPeriod = Convert.ToBoolean(thisRow[0]["CurrentPeriod"]);
                UserId = thisRow[0]["UserId"].ToString();
                CompanyId = Convert.ToInt32(thisRow[0]["CompanyId"]);
            }
            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete(string strTableName, string strColumnName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, PeriodId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion

        #region Convert DataSet To List
        public List<Period> ConvertDataSetToList(DataSet oDataSet)
        {
            List<Period> lstPeriod = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new Period
                            {
                                PeriodId = Convert.ToInt64(oRow["PeriodId"]),
                                StartDate = Convert.ToDateTime(oRow["StartDate"]),
                                EndDate = Convert.ToDateTime(oRow["EndDate"]),
                                Opened = Convert.ToBoolean(oRow["Opened"]),
                                CurrentPeriod = Convert.ToBoolean(oRow["CurrentPeriod"]),
                                UserId = oRow["UserId"].ToString(),
                                CompanyId = Convert.ToInt32(oRow["CompanyId"]),
                                LocationId = Convert.ToInt64(oRow["LocationId"]),
                            }).ToList();
            return lstPeriod;
        }
        #endregion
       
    }
}
