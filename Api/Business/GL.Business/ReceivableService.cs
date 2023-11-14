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
    public class ReceivableService
    {
        #region Properties
        public Int64 ReceivableServiceId { get; set; }
        public Int64 ReceivableServiceMasterId { get; set; }
        public Int64 ServiceCustomerId { get; set; }
        public Decimal Amount { get; set; }
        public Decimal DiscountAmount { get; set; }
        public Decimal AmountReceivable { get; set; }
        public bool DiscountIsPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Narration { get; set; }
        public bool Posted { get; set; }
        public bool Reversed { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ReceivableServiceAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ReceivableServiceEdit") as SqlCommand;
            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "ReceivableServiceId", SqlDbType.BigInt, ReceivableServiceId);
            db.AddInParameter(oCommand, "ReceivableServiceMasterId", SqlDbType.BigInt, ReceivableServiceMasterId);
            db.AddInParameter(oCommand, "ServiceCustomerId", SqlDbType.BigInt, ServiceCustomerId);
            db.AddInParameter(oCommand, "Amount", SqlDbType.Decimal, Amount);
            db.AddInParameter(oCommand, "DiscountAmount", SqlDbType.Decimal, DiscountAmount);
            db.AddInParameter(oCommand, "AmountReceivable", SqlDbType.Decimal, AmountReceivable);
            db.AddInParameter(oCommand, "DiscountIsPercentage", SqlDbType.Bit, DiscountIsPercentage);
            db.AddInParameter(oCommand, "StartDate", SqlDbType.Date, StartDate);
            db.AddInParameter(oCommand, "EndDate", SqlDbType.Date, EndDate);
            db.AddInParameter(oCommand, "Narration", SqlDbType.NVarChar, Narration);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Save Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReceivableServiceDelete") as SqlCommand;
            db.AddInParameter(oCommand, "ReceivableServiceId", SqlDbType.BigInt, ReceivableServiceId);
            return oCommand;
        }
        #endregion



        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReceivableServiceDetailSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetReceivableService(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, ReceivableServiceId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ReceivableServiceId = Convert.ToInt64(thisRow[0]["ReceivableServiceId"]);
                ReceivableServiceMasterId = Convert.ToInt64(thisRow[0]["ReceivableServiceMasterId"]);
                ServiceCustomerId = Int64.Parse(thisRow[0]["ServiceCustomerId"].ToString());
                Amount = Convert.ToDecimal(thisRow[0]["Amount"]);
                DiscountAmount = Convert.ToDecimal(thisRow[0]["DiscountAmount"].ToString().Trim() != "" ? thisRow[0]["DiscountAmount"] : 0);
                AmountReceivable = Convert.ToDecimal(thisRow[0]["AmountReceivable"]);
                DiscountIsPercentage = Convert.ToBoolean(thisRow[0]["DiscountIsPercentage"]);
                StartDate = Convert.ToDateTime(thisRow[0]["StartDate"]);
                EndDate = Convert.ToDateTime(thisRow[0]["EndDate"]);
                Posted = Convert.ToBoolean(thisRow[0]["Posted"]);
                Reversed = Convert.ToBoolean(thisRow[0]["Reversed"]);
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
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, ReceivableServiceId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);
            return true;
        }
        #endregion

        #region Check Customer Service Id Exist
        public bool ChkServiceCustomerIdExist()
        {
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReceivableServiceChkServiceCustomerIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "ServiceCustomerId", SqlDbType.BigInt, ServiceCustomerId);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Convert To List
        public List<ReceivableService> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new ReceivableService()
                    {
                        ReceivableServiceId = Convert.ToInt64(oRow["ReceivableServiceId"]),
                        ReceivableServiceMasterId = Convert.ToInt64(oRow["ReceivableServiceMasterId"]),
                        ServiceCustomerId = Convert.ToInt64(oRow["ServiceCustomerId"].ToString()),
                        Amount = Convert.ToDecimal(oRow["Amount"]),
                        DiscountAmount = Convert.ToDecimal(oRow["DiscountAmount"]),
                        AmountReceivable = Convert.ToDecimal(oRow["AmountReceivable"]),
                        DiscountIsPercentage = Convert.ToBoolean(oRow["DiscountIsPercentage"]),
                    }).ToList();
        }
        #endregion

        #region Convert DataSet To List
        public List<ReceivableService> ConvertDataSetToList(DataSet oDataSet)
        {
            List<ReceivableService> lstReceivableService = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new ReceivableService
                            {
                                ReceivableServiceId = Convert.ToInt64(oRow["ReceivableServiceId"]),
                                ReceivableServiceMasterId = Convert.ToInt64(oRow["ReceivableServiceMasterId"]),
                                ServiceCustomerId = Convert.ToInt64(oRow["ServiceCustomerId"].ToString()),
                                Amount = Convert.ToDecimal(oRow["Amount"]),
                                DiscountAmount = Convert.ToDecimal(oRow["DiscountAmount"]),
                                AmountReceivable = Convert.ToDecimal(oRow["AmountReceivable"]),
                                DiscountIsPercentage = Convert.ToBoolean(oRow["DiscountIsPercentage"]),
                            }).ToList();
            return lstReceivableService;
        }
        #endregion
    }
}
