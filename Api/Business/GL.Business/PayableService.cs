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
    public class PayableService
    {
        #region Properties
        public Int64 PayableServiceId { get; set; }
        public Int64 PayableServiceMasterId { get; set; }
        public Int64 ServiceCustomerId { get; set; }
        public Decimal Amount { get; set; }
        public Decimal DiscountAmount { get; set; }
        public Decimal AmountPayable { get; set; }
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
                oCommand = db.GetStoredProcCommand("PayableServiceAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PayableServiceEdit") as SqlCommand; 
            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "PayableServiceId", SqlDbType.BigInt, PayableServiceId);
            db.AddInParameter(oCommand, "PayableServiceMasterId", SqlDbType.BigInt, PayableServiceMasterId);
            db.AddInParameter(oCommand, "ServiceCustomerId", SqlDbType.BigInt, ServiceCustomerId);
            db.AddInParameter(oCommand, "Amount", SqlDbType.Decimal, Amount);
            db.AddInParameter(oCommand, "DiscountAmount", SqlDbType.Decimal,DiscountAmount);
            db.AddInParameter(oCommand, "AmountPayable", SqlDbType.Decimal, AmountPayable);
            db.AddInParameter(oCommand, "DiscountIsPercentage", SqlDbType.Bit,DiscountIsPercentage);
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
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServiceDelete") as SqlCommand;
            db.AddInParameter(oCommand, "PayableServiceId", SqlDbType.BigInt, PayableServiceId);
            return oCommand;
        }
        #endregion



        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServiceDetailSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetPayableService(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, PayableServiceId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                PayableServiceId = Convert.ToInt64(thisRow[0]["PayableServiceId"]);
                PayableServiceMasterId = Convert.ToInt64(thisRow[0]["PayableServiceMasterId"]);
                ServiceCustomerId = Int64.Parse(thisRow[0]["ServiceCustomerId"].ToString());
                Amount = Convert.ToDecimal(thisRow[0]["Amount"]);
                DiscountAmount = Convert.ToDecimal(thisRow[0]["DiscountAmount"].ToString().Trim() != "" ? thisRow[0]["DiscountAmount"] : 0);
                AmountPayable = Convert.ToDecimal(thisRow[0]["AmountPayable"]);
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
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, PayableServiceId);
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
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServiceChkServiceCustomerIdExist") as SqlCommand;
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
        public List<PayableService> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new PayableService()
                    {
                        PayableServiceId = Convert.ToInt64(oRow["PayableServiceId"]),
                        PayableServiceMasterId = Convert.ToInt64(oRow["PayableServiceMasterId"]),
                        ServiceCustomerId = Convert.ToInt64(oRow["ServiceCustomerId"].ToString()),
                        Amount = Convert.ToDecimal(oRow["Amount"]),
                        DiscountAmount = Convert.ToDecimal(oRow["DiscountAmount"]),
                        AmountPayable = Convert.ToDecimal(oRow["AmountPayable"]),
                        DiscountIsPercentage = Convert.ToBoolean(oRow["DiscountIsPercentage"]),
                    }).ToList();
        }
        #endregion

        #region Convert DataSet To List
        public List<PayableService> ConvertDataSetToList(DataSet oDataSet)
        {
            List<PayableService> lstPayableService = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new PayableService
                            {
                                PayableServiceId = Convert.ToInt64(oRow["PayableServiceId"]),
                                PayableServiceMasterId = Convert.ToInt64(oRow["PayableServiceMasterId"]),
                                ServiceCustomerId = Convert.ToInt64(oRow["ServiceCustomerId"].ToString()),
                                Amount = Convert.ToDecimal(oRow["Amount"]),
                                DiscountAmount = Convert.ToDecimal(oRow["DiscountAmount"]),
                                AmountPayable = Convert.ToDecimal(oRow["AmountPayable"]),
                                DiscountIsPercentage = Convert.ToBoolean(oRow["DiscountIsPercentage"]),
                            }).ToList();
            return lstPayableService;
        }
        #endregion
    }
}
