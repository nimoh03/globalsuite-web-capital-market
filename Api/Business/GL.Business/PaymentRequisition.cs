using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class PaymentRequisition
    {
        public Int64 TransNo { set; get; }
        public DateTime TransDate { set; get; }
        public string ProductId { set; get; }
        public string CustomerId { set; get; }
        public string Narration { set; get; }
        public decimal Amount { set; get; }
        public List<string> ListAllotmentId { set; get; }
        public string SaveType { set; get; }



        public List<string> Save()
        {
            List<string> oErrMsg = new List<string>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (SaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("PaymentRequisitionAdd") as SqlCommand;
            }
            else if (SaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("PaymentRequisitionEdit") as SqlCommand;
            }

            db.AddOutParameter(oCommand, "@SaveErrMsg", SqlDbType.VarChar, 600);
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "TransDate", SqlDbType.DateTime, TransDate);
            db.AddInParameter(oCommand, "ProductId", SqlDbType.Char, ProductId.Trim());
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId.Trim());
            db.AddInParameter(oCommand, "Narration", SqlDbType.NVarChar, Narration.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, Amount);
            DataTable oDtAllotmentId = new DataTable();
            oDtAllotmentId.Columns.Add("Item", typeof(string));
            if (ListAllotmentId != null)
            {
                foreach (string oStr in ListAllotmentId)
                {
                    var row = oDtAllotmentId.NewRow();
                    row["Item"] = oStr;
                    oDtAllotmentId.Rows.Add(row);
                }
            }
            db.AddInParameter(oCommand, "ListAllotmentId", SqlDbType.Structured, oDtAllotmentId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.NVarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);

            var varErrMsg = db.GetParameterValue(oCommand, "SaveErrMsg").ToString();
            if (varErrMsg.Trim() != "")
            {
                oErrMsg.Add(varErrMsg);
            }
            return oErrMsg;
        }

        #region Get
        public bool GetPaymentRequisition()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PaymentRequisitionSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, TransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                TransDate = Convert.ToDateTime(thisRow[0]["TransDate"]);
                ProductId =  Convert.ToString(thisRow[0]["ProductId"]);
                CustomerId =  Convert.ToString(thisRow[0]["CustomerId"]);
                Narration = Convert.ToString(thisRow[0]["Narration"]);
                Amount =  Convert.ToDecimal(thisRow[0]["Amount"]);
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentRequisitionDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, TransNo);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PaymentRequisitionSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        public class PaymentRequisitionAllotId
        {
            public Int64 PaymentRequisitionId { set; get; }
            #region Get All By PaymentRequisition
            public DataSet GetAllByPaymentRequisition()
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("PaymentRequisitionAllotIdSelectAllPaymentRequisition") as SqlCommand;
                db.AddInParameter(dbCommand, "PaymentRequisitionId", SqlDbType.BigInt, PaymentRequisitionId);
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                return oDS;
            }
            #endregion
        }

    }
}

