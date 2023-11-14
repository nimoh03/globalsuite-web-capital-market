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
    public class PayableServicePayment
    {
        #region Properties
        public Int64 PayableServicePaymentId { get; set; }
        public Int64 PayableServicePaymentMasterId { get; set; }
        public Int64 ServiceCustomerId { get; set; }
        public Decimal AmountPaid { get; set; }
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
                oCommand = db.GetStoredProcCommand("PayableServicePaymentAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PayableServicePaymentEdit") as SqlCommand;
            }
            db.AddOutParameter(oCommand, "SaveErrMsg", SqlDbType.NVarChar, 300);
            db.AddInParameter(oCommand, "PayableServicePaymentId", SqlDbType.BigInt, PayableServicePaymentId);
            db.AddInParameter(oCommand, "PayableServicePaymentMasterId", SqlDbType.BigInt, PayableServicePaymentMasterId);
            db.AddInParameter(oCommand, "ServiceCustomerId", SqlDbType.BigInt, ServiceCustomerId);
            db.AddInParameter(oCommand, "AmountPaid", SqlDbType.Decimal, AmountPaid);
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, Posted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion



        #region Get
        public bool GetPayableServicePayment(string strTableName, string strColumnName)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenSelect") as SqlCommand;
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, PayableServicePaymentId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                PayableServicePaymentId = Convert.ToInt64(thisRow[0]["PayableServicePaymentId"]);
                PayableServicePaymentMasterId = Convert.ToInt64(thisRow[0]["PayableServicePaymentMasterId"]);
                ServiceCustomerId = Int64.Parse(thisRow[0]["ServiceCustomerId"].ToString());
                AmountPaid = Convert.ToDecimal(thisRow[0]["AmountPaid"]);
                Posted = Convert.ToBoolean(thisRow[0]["Posted"]);
                Reversed = Convert.ToBoolean(thisRow[0]["Reversed"]);
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Total Amount Paid By StudentId And TermHistoryId
        public decimal GetTotalAmountPaidByStudentAndTermHistory(Int64 intStudentId, Int64 intTermHistory)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServicePaymentSelectTotalAmountPaidByStudentIdAndTermHistoryId") as SqlCommand;
            db.AddInParameter(oCommand, "StudentId", SqlDbType.BigInt, intStudentId);
            db.AddInParameter(oCommand, "TermHistoryId", SqlDbType.BigInt, intTermHistory);
            var varTotalAmountPaid = db.ExecuteScalar(oCommand);
            return varTotalAmountPaid != null && varTotalAmountPaid.ToString().Trim() != ""
                ? Convert.ToDecimal(varTotalAmountPaid) : 0;
        }
        #endregion

        #region Delete
        public bool Delete(string strTableName, string strColumnName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GenDelete") as SqlCommand;
            db.AddInParameter(oCommand, "RecordValue", SqlDbType.BigInt, PayableServicePaymentId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);

            return true;
        }
        #endregion

        #region Reverse Return Command
        public SqlCommand ReverseCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PayableServicePaymentReverse") as SqlCommand;
            db.AddInParameter(oCommand, "PayableServicePaymentMasterId", SqlDbType.BigInt, PayableServicePaymentMasterId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Convert To List
        public List<PayableServicePayment> ConvertToList(DataRowCollection oDataRowCollection)
        {
            return (from DataRow oRow in oDataRowCollection
                    select new PayableServicePayment()
                    {
                        PayableServicePaymentId = Convert.ToInt64(oRow["PayableServicePaymentId"]),
                        PayableServicePaymentMasterId = Convert.ToInt64(oRow["PayableServicePaymentMasterId"]),
                        ServiceCustomerId = Convert.ToInt64(oRow["ServiceCustomerId"]),
                        AmountPaid = Convert.ToDecimal(oRow["AmountPaid"]),
                    }).ToList();
        }
        #endregion

        #region Convert DataSet To List
        public List<PayableServicePayment> ConvertDataSetToList(DataSet oDataSet)
        {
            List<PayableServicePayment> lstPayableServicePayment = oDataSet.Tables[0].AsEnumerable().Select(
                            oRow => new PayableServicePayment
                            {
                                PayableServicePaymentId = Convert.ToInt64(oRow["PayableServicePaymentId"]),
                                PayableServicePaymentMasterId = Convert.ToInt64(oRow["PayableServicePaymentMasterId"]),
                                ServiceCustomerId = Convert.ToInt64(oRow["ServiceCustomerId"].ToString()),
                                AmountPaid = Convert.ToDecimal(oRow["AmountPaid"]),
                            }).ToList();
            return lstPayableServicePayment;
        }
        #endregion
    }
}
