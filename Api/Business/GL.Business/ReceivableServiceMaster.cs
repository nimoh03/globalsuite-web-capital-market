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
    public class ReceivableServiceMaster
    {
        #region Properties
        public Int64 ReceivableServiceMasterId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Int64 CustomerId { get; set; }
        public bool Posted { get; set; }
        public bool Reversed { get; set; }
        public string SaveType { get; set; }
        #endregion



        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            if (!ChkReceivableServiceMasterIdExist())
            {
                throw new Exception("Cannot Save Receivable Service Master Does Not Exist");
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ReceivableServiceMasterAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "ReceivableServiceMasterId", SqlDbType.BigInt, 8);
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ReceivableServiceMasterEdit") as SqlCommand;
                db.AddInParameter(oCommand, "ReceivableServiceMasterId", SqlDbType.BigInt, ReceivableServiceMasterId);
            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.Date, EffectiveDate);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.BigInt, CustomerId);
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, Posted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;

        }
        #endregion

        #region Check Term Fee MasterId Exist
        public bool ChkReceivableServiceMasterIdExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("ReceivableServiceMasterChkStudentReceivableServiceMasterIdExist") as SqlCommand;
                db.AddInParameter(oCommand, "ReceivableServiceMasterId", SqlDbType.BigInt, ReceivableServiceMasterId);
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



        #region Get Receivable Service With Payments
        public DataSet GetReceivableServiceWithPayment()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReceivableServiceSelectByReceivableServiceMasterWithPayment") as SqlCommand;
            db.AddInParameter(oCommand, "ReceivableServiceMasterId", SqlDbType.BigInt, ReceivableServiceMasterId);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get
        public bool GetReceivableServiceMaster(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, ReceivableServiceMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ReceivableServiceMasterId = Convert.ToInt64(thisRow[0]["ReceivableServiceMasterId"]);
                CustomerId = Convert.ToInt64(thisRow[0]["CustomerId"]);
                EffectiveDate = Convert.ToDateTime(thisRow[0]["EffectiveDate"]);
                Posted = Convert.ToBoolean(thisRow[0]["Posted"]);
                Reversed = Convert.ToBoolean(thisRow[0]["Reversed"]);
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion


        #region Get Receivable Service Master By CustomerId Not Fully Paid
        public DataSet GetReceivableServiceMasterByCustomerIdNotFullyPaid()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReceivableServiceMasterSelectByCustomerIdNotFullyPaid") as SqlCommand;
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.BigInt, CustomerId);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion



        #region Delete
        public bool Delete(string strTableName, string strColumnName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, ReceivableServiceMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion



        #region Convert To List
        public List<ReceivableServiceMaster> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new ReceivableServiceMaster()
                    {
                        ReceivableServiceMasterId = Convert.ToInt64(oRow["ReceivableServiceMasterId"]),
                        CustomerId = Convert.ToInt64(oRow["CustomerId"]),
                        EffectiveDate = Convert.ToDateTime(oRow["EffectiveDate"]),
                        Posted = Convert.ToBoolean(oRow["Posted"]),
                        Reversed = Convert.ToBoolean(oRow["Reversed"]),

                    }).ToList();
        }
        #endregion 

        #region Convert DataSet To List
        public List<ReceivableServiceMaster> ConvertDataSetToList(DataSet oDataSet)
        {
            List<ReceivableServiceMaster> lstReceivableServiceMaster = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new ReceivableServiceMaster
                            {
                                ReceivableServiceMasterId = Convert.ToInt64(oRow["ReceivableServiceMasterId"]),
                                CustomerId = Convert.ToInt64(oRow["CustomerId"]),
                                EffectiveDate = Convert.ToDateTime(oRow["EffectiveDate"]),
                                Posted = Convert.ToBoolean(oRow["Posted"]),
                                Reversed = Convert.ToBoolean(oRow["Reversed"]),
                            }).ToList();
            return lstReceivableServiceMaster;
        }
        #endregion
    }
}
