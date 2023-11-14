using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ServiceCustomer
    {
        #region Properties
        public Int64 ServiceCustomerId { get; set; }
        public string CustomerId { get; set; }
        public Int64 ItemServiceId { get; set; }
        public DateTime NextDueDate { get; set; }
        public decimal PeriodAmount { get; set; }
        public int PeriodMonths { get; set; }
        public Int64 VendorId { get; set; }
        public DateTime VendorNextDueDate { get; set; }
        public decimal VendorPeriodAmount { get; set; }
        public int VendorPeriodMonths { get; set; }
        public string SaveType { get; set; }
        #endregion

        #region Save
        public List<string> Save()
        {
            List<string> oErrMsg = new List<string>();

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ServiceCustomerAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ServiceCustomerEdit") as SqlCommand;

            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "ServiceCustomerId", SqlDbType.BigInt, ServiceCustomerId);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.NVarChar, CustomerId.Trim());
            db.AddInParameter(oCommand, "ItemServiceId", SqlDbType.BigInt, ItemServiceId);
            db.AddInParameter(oCommand, "PeriodAmount", SqlDbType.Decimal, PeriodAmount);
            db.AddInParameter(oCommand, "PeriodMonths", SqlDbType.Int, PeriodMonths);
            db.AddInParameter(oCommand, "NextDueDate", SqlDbType.Date, NextDueDate);
            db.AddInParameter(oCommand, "VendorId", SqlDbType.BigInt, VendorId);
            db.AddInParameter(oCommand, "VendorPeriodAmount", SqlDbType.Decimal, VendorPeriodAmount);
            db.AddInParameter(oCommand, "VendorPeriodMonths", SqlDbType.Int, VendorPeriodMonths);
            db.AddInParameter(oCommand, "VendorNextDueDate", SqlDbType.Date, VendorNextDueDate);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            var varErrMsg = db.GetParameterValue(oCommand, "SaveErrMsg").ToString();
            if (varErrMsg.Trim() != "")
            {
                oErrMsg.Add(varErrMsg);
            }
            return oErrMsg;

        }
        #endregion

        #region Get All With Customer And Service Name
        public DataSet GetAllWithCustomerAndServiceName()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ServiceCustomerSelectAllWithServiceAndCustomerName") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetServiceCustomer(string strTableName, string strColumnName)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, ServiceCustomerId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                CustomerId = thisRow[0]["CustomerId"].ToString();
                ItemServiceId = Convert.ToInt64(thisRow[0]["ItemServiceId"].ToString());
                NextDueDate = DateTime.ParseExact(thisRow[0]["NextDueDate"].ToString().Substring(0,10),"dd/MM/yyyy", format);
                PeriodAmount = Convert.ToDecimal(thisRow[0]["PeriodAmount"]);
                PeriodMonths = Convert.ToInt32(thisRow[0]["PeriodMonths"]);
                VendorId = Convert.ToInt32(thisRow[0]["VendorId"]);
                VendorNextDueDate = DateTime.ParseExact(thisRow[0]["VendorNextDueDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
                VendorPeriodAmount = Convert.ToDecimal(thisRow[0]["VendorPeriodAmount"]);
                VendorPeriodMonths = Convert.ToInt32(thisRow[0]["VendorPeriodMonths"]);
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Update Next Due Date
        public SqlCommand UpdateNextDueDate(DateTime datNextDueDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ServiceCustomerUpdateNextDueDate") as SqlCommand;
            db.AddInParameter(oCommand, "ServiceCustomerId", SqlDbType.Int, ServiceCustomerId);
            db.AddInParameter(oCommand, "NextDueDate", SqlDbType.Date, datNextDueDate);
            return oCommand;
        }
        #endregion

        #region Update Next Due Date
        public SqlCommand UpdateVendorNextDueDate(DateTime datNextDueDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ServiceCustomerUpdateVendorNextDueDate") as SqlCommand;
            db.AddInParameter(oCommand, "ServiceCustomerId", SqlDbType.Int, ServiceCustomerId);
            db.AddInParameter(oCommand, "NextDueDate", SqlDbType.Date, datNextDueDate);
            return oCommand;
        }
        #endregion

    }
}
