using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class PaymentWorksheetMaster
    {
        #region Properties
        public Int64 PaymentWorksheetMasterId { get; set; }
        public string CustomerId { get; set; }
        public DateTime TransDate { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }
        public char IsSubDealer { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("AcctPaymentWorksheetMasterAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("AcctPaymentWorksheetMasterEdit") as SqlCommand;
            }
            db.AddOutParameter(oCommand, "PaymentWorksheetMasterId", SqlDbType.BigInt, 8);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId);
            db.AddInParameter(oCommand, "TransDate", SqlDbType.DateTime, TransDate);
            db.AddInParameter(oCommand, "DateFrom", SqlDbType.DateTime, DateFrom);
            db.AddInParameter(oCommand, "DateTo", SqlDbType.DateTime, DateTo);
            db.AddInParameter(oCommand, "IsSubDealer", SqlDbType.Char, IsSubDealer);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentWorksheetMasterDelete") as SqlCommand;
            db.AddInParameter(oCommand, "PaymentWorksheetId", SqlDbType.BigInt, PaymentWorksheetMasterId);
            return oCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentWorksheetMasterSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All
        public DataSet GetByCustomerDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentWorksheetMasterSelectCustomerDate") as SqlCommand;
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId.Trim());
            db.AddInParameter(oCommand, "DateFrom", SqlDbType.DateTime, DateFrom);
            db.AddInParameter(oCommand, "DateTo", SqlDbType.DateTime, DateTo);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetPaymentWorksheetMaster(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, PaymentWorksheetMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                PaymentWorksheetMasterId = Convert.ToInt64(thisRow[0]["PaymentWorksheetMasterId"]);
                CustomerId = thisRow[0]["CustomerId"].ToString();
                TransDate = Convert.ToDateTime(thisRow[0]["TransDate"]);
                DateFrom = Convert.ToDateTime(thisRow[0]["DateFrom"]);
                DateTo = Convert.ToDateTime(thisRow[0]["DateTo"]);
                IsSubDealer = Convert.ToChar(thisRow[0]["IsSubDealer"]);
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete(string strTableName, string strColumnName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, PaymentWorksheetMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);
            return true;
        }
        #endregion

        #region Convert To List
        public List<PaymentWorksheetMaster> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new PaymentWorksheetMaster()
                    {
                        PaymentWorksheetMasterId = Convert.ToInt64(oRow["PaymentWorksheetMasterId"]),
                        CustomerId = oRow["CustomerId"].ToString(),
                        TransDate = Convert.ToDateTime(oRow["TransDate"]),
                        DateFrom = Convert.ToDateTime(oRow["DateFrom"]),
                        DateTo = Convert.ToDateTime(oRow["DateTo"]),
                        IsSubDealer = Convert.ToChar(oRow["IsSubDealer"]),
                    }).ToList();
        }
        #endregion

        #region Convert DataSet To List
        public List<PaymentWorksheetMaster> ConvertDataSetToList(DataSet oDataSet)
        {
            List<PaymentWorksheetMaster> lstPaymentWorksheetMaster = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new PaymentWorksheetMaster
                            {
                                PaymentWorksheetMasterId = Convert.ToInt64(oRow["PaymentWorksheetMasterId"]),
                                CustomerId = oRow["CustomerId"].ToString(),
                                TransDate = Convert.ToDateTime(oRow["TransDate"]),
                                DateFrom = Convert.ToDateTime(oRow["DateFrom"]),
                                DateTo = Convert.ToDateTime(oRow["DateTo"]),
                                IsSubDealer = Convert.ToChar(oRow["IsSubDealer"]),
                            }).ToList();
            return lstPaymentWorksheetMaster;
        }
        #endregion
    }
}
