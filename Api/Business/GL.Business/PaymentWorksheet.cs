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
  public  class PaymentWorksheet
    {
        #region Properties
        public Int64 PaymentWorksheetId { get; set; }
        public Int64 PaymentWorksheetMasterId { get; set; }
        public Int64 AccountGLId { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {           
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("AcctPaymentWorksheetAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("AcctPaymentWorksheetEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "PaymentWorksheetId", SqlDbType.BigInt, PaymentWorksheetId);
            db.AddInParameter(oCommand, "PaymentWorksheetMasterId", SqlDbType.BigInt, PaymentWorksheetMasterId);
            db.AddInParameter(oCommand, "AccountGLId", SqlDbType.BigInt, AccountGLId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentWorksheetDelete") as SqlCommand;
            db.AddInParameter(oCommand, "PaymentWorksheetId", SqlDbType.BigInt, PaymentWorksheetId);
            return oCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentWorksheetSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetPaymentWorksheet(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, PaymentWorksheetId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                PaymentWorksheetId = Convert.ToInt64(thisRow[0]["PaymentWorksheetId"]);
                PaymentWorksheetMasterId = Convert.ToInt64(thisRow[0]["PaymentWorksheetMasterId"]);
                AccountGLId = Int64.Parse(thisRow[0]["AccountGLId"].ToString());
               
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
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, PaymentWorksheetId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);
            return true;
        }
        #endregion

        

        #region Convert To List
        public List<PaymentWorksheet> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new PaymentWorksheet()
                    {
                        PaymentWorksheetId = Convert.ToInt64(oRow["PaymentWorksheetId"]),
                        PaymentWorksheetMasterId = Convert.ToInt64(oRow["PaymentWorksheetMasterId"]),
                        AccountGLId = Convert.ToInt64(oRow["AccountGLId"].ToString()),
                       }).ToList();
        }
        #endregion

        #region Convert DataSet To List
        public List<PaymentWorksheet> ConvertDataSetToList(DataSet oDataSet)
        {
            List<PaymentWorksheet> lstPaymentWorksheet = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new PaymentWorksheet
                            {
                                PaymentWorksheetId = Convert.ToInt64(oRow["PaymentWorksheetId"]),
                                PaymentWorksheetMasterId = Convert.ToInt64(oRow["PaymentWorksheetMasterId"]),
                                AccountGLId = Convert.ToInt64(oRow["AccountGLId"].ToString()),
                            }).ToList();
            return lstPaymentWorksheet;
        }
        #endregion
    }
}
