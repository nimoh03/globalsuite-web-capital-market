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
    public class PayableServiceMaster
    {
        #region Properties
        public Int64 PayableServiceMasterId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Int64 VendorId { get; set; }
        public bool Posted { get; set; }
        public bool Reversed { get; set; }
        public string SaveType { get; set; }
        #endregion

       

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            if (!ChkPayableServiceMasterIdExist())
            {
                throw new Exception("Cannot Save Payable Service Master Does Not Exist");
            }
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("PayableServiceMasterAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "PayableServiceMasterId", SqlDbType.BigInt, 8);
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PayableServiceMasterEdit") as SqlCommand;
                db.AddInParameter(oCommand, "PayableServiceMasterId", SqlDbType.BigInt, PayableServiceMasterId);
            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.Date, EffectiveDate);
            db.AddInParameter(oCommand, "VendorId", SqlDbType.BigInt, VendorId);
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, Posted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;

        }
        #endregion

        #region Check Term Fee MasterId Exist
        public bool ChkPayableServiceMasterIdExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("PayableServiceMasterChkStudentPayableServiceMasterIdExist") as SqlCommand;
                db.AddInParameter(oCommand, "PayableServiceMasterId", SqlDbType.BigInt, PayableServiceMasterId);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
            }
            else if (SaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion



        #region Get Payable Service With Payments
        public DataSet GetPayableServiceWithPayment()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServiceSelectByPayableServiceMasterWithPayment") as SqlCommand;
            db.AddInParameter(oCommand, "PayableServiceMasterId", SqlDbType.BigInt, PayableServiceMasterId);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetPayableServiceMaster(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, PayableServiceMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                PayableServiceMasterId = Convert.ToInt64(thisRow[0]["PayableServiceMasterId"]);
                VendorId = Convert.ToInt64(thisRow[0]["VendorId"]);
                EffectiveDate = Convert.ToDateTime(thisRow[0]["EffectiveDate"]);
                Posted = Convert.ToBoolean(thisRow[0]["Posted"]);
                Reversed = Convert.ToBoolean(thisRow[0]["Reversed"]);
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

       
        #region Get Payable Service Master By VendorId Not Fully Paid
        public DataSet GetPayableServiceMasterByVendorIdNotFullyPaid()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServiceMasterSelectByVendorIdNotFullyPaid") as SqlCommand;
            db.AddInParameter(oCommand, "VendorId", SqlDbType.BigInt, VendorId);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion



        #region Delete
        public bool Delete(string strTableName, string strColumnName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, PayableServiceMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion

       

        #region Convert To List
        public List<PayableServiceMaster> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new PayableServiceMaster()
                    {
                        PayableServiceMasterId = Convert.ToInt64(oRow["PayableServiceMasterId"]),
                        VendorId = Convert.ToInt64(oRow["VendorId"]),
                        EffectiveDate = Convert.ToDateTime(oRow["EffectiveDate"]),
                        Posted = Convert.ToBoolean(oRow["Posted"]),
                        Reversed = Convert.ToBoolean(oRow["Reversed"]),

                    }).ToList();
        }
        #endregion 

        #region Convert DataSet To List
        public List<PayableServiceMaster> ConvertDataSetToList(DataSet oDataSet)
        {
            List<PayableServiceMaster> lstPayableServiceMaster = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new PayableServiceMaster
                            {
                                PayableServiceMasterId = Convert.ToInt64(oRow["PayableServiceMasterId"]),
                                VendorId = Convert.ToInt64(oRow["VendorId"]),
                                EffectiveDate = Convert.ToDateTime(oRow["EffectiveDate"]),
                                Posted = Convert.ToBoolean(oRow["Posted"]),
                                Reversed = Convert.ToBoolean(oRow["Reversed"]),
                            }).ToList();
            return lstPayableServiceMaster;
        }
        #endregion
    }
}
