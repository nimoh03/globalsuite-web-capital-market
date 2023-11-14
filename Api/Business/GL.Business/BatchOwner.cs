using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class BatchOwner
    {
        #region Declaration
        private string strBatchno, strTransNoRev, strPostsdby, strApprovedby, strReversedby, strPosted, strReversed, strSaveType;
        private DateTime datValDate, datTxnDate, datTxnTime;
        private DateTime datValDateTo, datTxnDateTo;
        #endregion

        #region Properties
        public string Batchno
        {
            set { strBatchno = value; }
            get { return strBatchno; }
        }
        public string TransNoRev
        {
            set { strTransNoRev = value; }
            get { return strTransNoRev; }
        }
        public string Postsdby
        {
            set { strPostsdby = value; }
            get { return strPostsdby; }
        }
        public string Approvedby
        {
            set { strApprovedby = value; }
            get { return strApprovedby; }
        }
        public string Reversedby
        {
            set { strReversedby = value; }
            get { return strReversedby; }
        }
        public string Posted
        {
            set { strPosted = value; }
            get { return strPosted; }
        }
        public string Reversed
        {
            set { strReversed = value; }
            get { return strReversed; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public DateTime ValDate
        {
            set { datValDate = value; }
            get { return datValDate; }
        }
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }
        public DateTime TxnTime
        {
            set { datTxnTime = value; }
            get { return datTxnTime; }
        }
        public DateTime ValDateTo
        {
            set { datValDateTo = value; }
            get { return datValDateTo; }
        }
        public DateTime TxnDateTo
        {
            set { datTxnDateTo = value; }
            get { return datTxnDateTo; }
        }
        #endregion

        #region Add Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchOwnerAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "BATCHOWNER");
            db.AddOutParameter(dbCommand, "BatchNo", SqlDbType.VarChar,20);
            db.AddInParameter(dbCommand, "ValDate", SqlDbType.DateTime, datValDate);
            db.AddInParameter(dbCommand, "Postsdby", SqlDbType.VarChar, GeneralFunc.UserName);
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Char, 'N');
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Char, 'N');
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            return dbCommand;
        }
        #endregion

        #region Get Batch Owner
        public bool GetBatchowner(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "BatchNo", SqlDbType.VarChar, strBatchno.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strBatchno = thisRow[0]["Batchno"].ToString();
                strPostsdby = thisRow[0]["Postedby"].ToString();
                strApprovedby = thisRow[0]["Approvedby"].ToString();
                strReversedby = thisRow[0]["Reversedby"].ToString();
                strPosted = thisRow[0]["Posted"].ToString();
                strReversed = thisRow[0]["Reversed"].ToString();
                datValDate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["ValDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
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
            SqlCommand oCommand = db.GetStoredProcCommand("BatchOwnerSelectLastBatchNo") as SqlCommand;
            db.AddInParameter(oCommand, "BatchOwner", SqlDbType.VarChar, strBatchno.Trim());
            var varLastNumber = db.ExecuteScalar(oCommand);
            return varLastNumber != null && varLastNumber.ToString().Trim() != ""
                ? int.Parse(varLastNumber.ToString()) + 1 : 1;
        }
        #endregion

        #region Get All 
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
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
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectLatestUnPosted") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datTxnDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datTxnDateTo);
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
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datValDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datValDateTo);
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
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("BatchOwnerSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
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
        public DateTime GetDateOfBatchOwner()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchOwnerSelectDate", strBatchno.Trim()) as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(dbCommand);
            if (oDs.Tables[0].Rows.Count == 0)
            {
                return DateTime.MinValue;
            }
            else
            {
                return DateTime.ParseExact(oDs.Tables[0].Rows[0]["ValDate"].ToString().Substring(0,10), "dd/MM/yyyy", format);
            }
        }
        #endregion

        #region Check Batch Owner Exist
        public bool ChkBatchOwnerExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("BatchOwnerChkBatchOwnerExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "BatchNo", SqlDbType.VarChar, strBatchno.Trim());
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            else if (strSaveType == "ADDS")
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
            SqlCommand oCommand = db.GetStoredProcCommand("BatchOwnerUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strBatchno.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strPostsdby.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchOwnerUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strBatchno.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strReversedby.Trim());
            return oCommand;
        }
        #endregion

        #region Get Total Credit For Batch Owner
        public decimal GetTotalCredit()
        {
            strSaveType = "EDIT";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchOwnerReturnTotalCredit", strBatchno.Trim()) as SqlCommand;
            var varTotalAmount = db.ExecuteScalar(dbCommand);
            return varTotalAmount != null && varTotalAmount.ToString().Trim() != "" ? Convert.ToDecimal(varTotalAmount) : 0;
        }
        #endregion

        #region Get Total Debit For Batch Owner
        public decimal GetTotalDebit()
        {
            strSaveType = "EDIT";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchOwnerReturnTotalDebit", strBatchno.Trim()) as SqlCommand;
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
            SqlCommand dbCommand = db.GetStoredProcCommand("BatchOwnerSelectChkPosted", strBatchno.Trim()) as SqlCommand;
            return (string)db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchOwnerDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strBatchno.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("BatchOwnerDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Batchno", SqlDbType.VarChar,strBatchno.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Delete Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BatchOwnerDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strBatchno.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        
    }
}
