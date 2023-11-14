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
    public class PayableServicePaymentMaster
    {
        #region Properties
        public Int64 PayableServicePaymentMasterId { get; set; }
        public Int64 PayableServiceMasterId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Int64 VendorId { get; set; }
        public decimal AmountPaid { get; set; }
        public string AccountId { get; set; }
        public DataGeneral.GLInstrumentType PaymentMode { get; set; }
        public string PaymentModeType { get; set; }
        public bool Posted { get; set; }
        public bool Reversed { get; set; }
        public string SaveType { get; set; }

        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            if (!ChkPayableServicePaymentMasterIdExist())
            {
                throw new Exception("Cannot Save Term Fee Master Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("PayableServicePaymentMasterAdd") as SqlCommand;
                db.AddOutParameter(oCommand, "PayableServicePaymentMasterId", SqlDbType.BigInt, 8);
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PayableServicePaymentMasterEdit") as SqlCommand;
                db.AddInParameter(oCommand, "PayableServicePaymentMasterId", SqlDbType.BigInt, PayableServicePaymentMasterId);
            }
            db.AddInParameter(oCommand, "PayableServiceMasterId", SqlDbType.BigInt, PayableServiceMasterId);
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.Date, EffectiveDate);
            db.AddInParameter(oCommand, "VendorId", SqlDbType.BigInt, VendorId);
            db.AddInParameter(oCommand, "AmountPaid", SqlDbType.Decimal, AmountPaid);
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, AccountId);
            db.AddInParameter(oCommand, "PaymentMode", SqlDbType.VarChar, PaymentMode);
            db.AddInParameter(oCommand, "PaymentModeType", SqlDbType.Char, PaymentModeType);
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, Posted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;

        }
        #endregion

        #region Reverse And Return Command
        public SqlCommand ReverseCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServicePaymentMasterReverse") as SqlCommand;
            db.AddInParameter(oCommand, "PayableServicePaymentMasterId", SqlDbType.BigInt, PayableServicePaymentMasterId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;

        }
        #endregion



        #region Check Term Fee MasterId Exist
        public bool ChkPayableServicePaymentMasterIdExist()
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("PayableServicePaymentMasterChkStudentPayableServicePaymentMasterIdExist") as SqlCommand;
                db.AddInParameter(oCommand, "PayableServicePaymentMasterId", SqlDbType.BigInt, PayableServicePaymentMasterId);
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

        #region Check TermHistoryId And VendorId Exist
        public bool ChkVendorIdExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServicePaymentMasterChkVendorIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "VendorId", SqlDbType.BigInt, VendorId);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion


        #region Get
        public bool GetPayableServicePaymentMaster(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, PayableServicePaymentMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                PayableServicePaymentMasterId = Convert.ToInt64(thisRow[0]["PayableServicePaymentMasterId"]);
                PayableServiceMasterId = Convert.ToInt64(thisRow[0]["PayableServiceMasterId"]);
                VendorId = Convert.ToInt64(thisRow[0]["VendorId"]);
                EffectiveDate = Convert.ToDateTime(thisRow[0]["EffectiveDate"]);
                AmountPaid = Convert.ToDecimal(thisRow[0]["AmountPaid"]);
                AccountId = Convert.ToString(thisRow[0]["AccountId"]);
                PaymentMode = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), thisRow[0]["PaymentMode"].ToString().Trim(), false);
                PaymentModeType = Convert.ToString(thisRow[0]["PaymentModeType"]);
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
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, PayableServicePaymentMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion

        #region Get Term Fee Payment
        public DataSet GetPayableServicePayment()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServicePaymentMasterSelectPayableServicePaymentByPayableServicePaymentMaster") as SqlCommand;
            db.AddInParameter(oCommand, "PayableServicePaymentMasterId", SqlDbType.BigInt, PayableServicePaymentMasterId);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Get All Posting
        public DataSet GetAllPosting(DataGeneral.PostStatus TransStatus, string strStudentName, int intLevel)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PayableServicePaymentMasterSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PayableServicePaymentMasterSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PayableServicePaymentMasterSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PayableServicePaymentMasterSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PayableServicePaymentMasterSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("PayableServicePaymentMasterSelectAllAll") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "StudentName", SqlDbType.VarChar, strStudentName.Trim());
            db.AddInParameter(dbCommand, "Level", SqlDbType.Int, intLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Convert To List
        public List<PayableServicePaymentMaster> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new PayableServicePaymentMaster()
                    {
                        PayableServicePaymentMasterId = Convert.ToInt64(oRow["PayableServicePaymentMasterId"]),
                        PayableServiceMasterId = Convert.ToInt64(oRow["PayableServiceMasterId"]),
                        VendorId = Convert.ToInt64(oRow["VendorId"]),
                        EffectiveDate = Convert.ToDateTime(oRow["EffectiveDate"]),
                        AmountPaid = Convert.ToDecimal(oRow["AmountPaid"]),
                        AccountId = Convert.ToString(oRow["AccountId"]),
                        PaymentModeType = Convert.ToString(oRow["PaymentModeType"]),
                    }).ToList();
        }
        #endregion 

        #region Convert DataSet To List
        public List<PayableServicePaymentMaster> ConvertDataSetToList(DataSet oDataSet)
        {
            List<PayableServicePaymentMaster> lstPayableServicePaymentMaster = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new PayableServicePaymentMaster
                            {
                                PayableServicePaymentMasterId = Convert.ToInt64(oRow["PayableServicePaymentMasterId"]),
                                PayableServiceMasterId = Convert.ToInt64(oRow["PayableServiceMasterId"]),
                                VendorId = Convert.ToInt64(oRow["VendorId"]),
                                EffectiveDate = Convert.ToDateTime(oRow["EffectiveDate"]),
                                AmountPaid = Convert.ToDecimal(oRow["AmountPaid"]),
                                AccountId = Convert.ToString(oRow["AccountId"]),
                                PaymentModeType = Convert.ToString(oRow["PaymentModeType"]),
                            }).ToList();
            return lstPayableServicePaymentMaster;
        }
        #endregion
    }
}
