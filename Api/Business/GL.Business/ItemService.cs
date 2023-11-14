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
    public class ItemService
    {
        #region Properties
        public Int64 ItemServiceId { get; set; }
        public string ItemServiceName { get; set; }
        public string IncomeAccount { get; set; }
        public string ExpenseAccount { get; set; }
        public bool IsItem { get; set; }
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
                oCommand = db.GetStoredProcCommand("ItemServiceAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ItemServiceEdit") as SqlCommand;

            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "ItemServiceId", SqlDbType.BigInt, ItemServiceId);
            db.AddInParameter(oCommand, "ItemServiceName", SqlDbType.NVarChar, ItemServiceName.Trim().ToUpper());
            db.AddInParameter(oCommand, "IncomeAccount", SqlDbType.NVarChar, IncomeAccount.Trim());
            db.AddInParameter(oCommand, "ExpenseAccount", SqlDbType.NVarChar, ExpenseAccount.Trim());
            db.AddInParameter(oCommand, "IsItem", SqlDbType.Bit, IsItem);
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
            SqlCommand dbCommand = db.GetStoredProcCommand("ItemServiceSelectAllWithServiceAndCustomerName") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion





        #region Get
        public bool GetItemService(string strTableName, string strColumnName)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, ItemServiceId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ItemServiceName = thisRow[0]["ItemServiceName"].ToString();
                IncomeAccount = thisRow[0]["IncomeAccount"].ToString();
                ExpenseAccount = thisRow[0]["ExpenseAccount"].ToString();
                IsItem = Convert.ToBoolean(thisRow[0]["IsItem"]);

                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
    }
}
