using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class BatchSpreadSheetMaster
    {
        #region Properties
        public Int64 BatchSpreadSheetMasterId { set; get; }
        public Int64 BatchSpreadSheetMasterIdRev { set; get; }
        public DateTime EffectiveDate { set; get; }
        public string InputBy { set; get; }
        public string ApprovedBy { set; get; }
        public string ReversedBy { set; get; }
        public bool Posted { set; get; }
        public bool Reversed { set; get; }
        public string SaveType { set; get; }
        #endregion

        #region Save Return Command
        public SqlCommand SaveCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            if (SaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterAddNew") as SqlCommand;
                db.AddOutParameter(dbCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, 20);
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterEdit") as SqlCommand;
                db.AddInParameter(dbCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            }
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "Inputby", SqlDbType.VarChar, InputBy.Trim());
            db.AddInParameter(dbCommand, "Approvedby", SqlDbType.VarChar, ApprovedBy.Trim());
            db.AddInParameter(dbCommand, "Reversedby", SqlDbType.VarChar, ReversedBy.Trim());
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, false);
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, false);
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            return dbCommand;
        }
        #endregion

        #region Get Batch SpreadSheet Master
        public bool GetBatchSpreadSheetMaster(string strTableName, string strColumnName, DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                oCommand = db.GetStoredProcCommand("GenSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                oCommand = db.GetStoredProcCommand("GenSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                oCommand = db.GetStoredProcCommand("GenSelectReversed") as SqlCommand;
            }
            db.AddInParameter(oCommand, "Recordvalue", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "ColumnName", SqlDbType.VarChar, strColumnName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                BatchSpreadSheetMasterId = Convert.ToInt64(thisRow[0]["BatchSpreadSheetMasterId"]);
                InputBy = thisRow[0]["InputBy"].ToString();
                ApprovedBy = thisRow[0]["ApprovedBy"].ToString();
                ReversedBy = thisRow[0]["ReversedBy"].ToString();
                Posted = Convert.ToBoolean(thisRow[0]["Posted"]);
                Reversed = Convert.ToBoolean(thisRow[0]["Reversed"]);
                EffectiveDate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["EffectiveDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Next Transaction Number For A Batch Owner Number
        public int GetNextTransNo()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectLastBatchNo") as SqlCommand;
            db.AddInParameter(oCommand, "BatchSpreadSheetMaster", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            var varLastNumber = db.ExecuteScalar(oCommand);
            return varLastNumber != null && varLastNumber.ToString().Trim() != ""
                ? int.Parse(varLastNumber.ToString()) + 1 : 1;
        }
        #endregion

        

        #region Get Latest Transaction 
        public DataSet GetLatestTransaction(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectLatestUnPosted") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Batch Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Batch Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, EffectiveDate);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Batch Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Date Of A Batch Owner
        public DateTime GetDateOfBatchSpreadSheetMaster()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectDate", BatchSpreadSheetMasterId) as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(dbCommand);
            if (oDs.Tables[0].Rows.Count == 0)
            {
                return DateTime.MinValue;
            }
            else
            {
                return DateTime.ParseExact(oDs.Tables[0].Rows[0]["ValDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format);
            }
        }
        #endregion

        #region Check Batch Owner Exist
        public bool ChkBatchSpreadSheetMasterExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (SaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterChkBatchSpreadSheetMasterExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "BatchSpreadSheetMasterId", SqlDbType.BigInt, BatchSpreadSheetMasterId);
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

        #region Update Post and Return A Command
        public SqlCommand UpDatePostCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, InputBy.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, Reversed);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, ReversedBy.Trim());
            return oCommand;
        }
        #endregion

        #region Get Total Credit For Batch Owner
        public decimal GetTotalCredit()
        {
            SaveType = "EDIT";


            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterReturnTotalCredit", BatchSpreadSheetMasterId) as SqlCommand;

            var varTotalAmount = db.ExecuteScalar(dbCommand);
            return varTotalAmount != null && varTotalAmount.ToString().Trim() != "" ? Convert.ToDecimal(varTotalAmount) : 0;

        }
        #endregion

        #region Get Total Debit For Batch Owner
        public decimal GetTotalDebit()
        {
            SaveType = "EDIT";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterReturnTotalDebit", BatchSpreadSheetMasterId) as SqlCommand;
            var varTotalAmount = db.ExecuteScalar(dbCommand);
            return varTotalAmount != null && varTotalAmount.ToString().Trim() != "" ? Convert.ToDecimal(varTotalAmount) : 0;

        }
        #endregion

        #region Check If Debit And Credit Transactions in Batch Number Is Equal
        public bool EqualDebitCredit()
        {
            bool blnStatus = false;
            if (GetTotalCredit() != GetTotalDebit())
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check If Batch Number Is Posted And Reversed
        public string ChkPosted()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterSelectChkPosted", BatchSpreadSheetMasterId) as SqlCommand;
            return (string)db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.BigInt, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Batchno", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Delete Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchSpreadSheetMasterDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, BatchSpreadSheetMasterId);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion


    }
}
