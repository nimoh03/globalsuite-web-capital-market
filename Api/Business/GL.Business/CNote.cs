using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class CNote
    {
        #region Declaration
        private string strCreditNo, strTransNoRev, strAcctMasBank, strAcctSubBank;
        private DateTime datRNDate;
        private string strTrandesc;
        private decimal decAmount;
        private string strRef, strPayDesc, strCustNo, strAmtWord;
        private bool blnPosted;
        private string strOfficer;
        private DateTime datETime, datEDate;
        private bool blnReversed;
        private string strSaveType, strRecNo;
        private bool blnDoNotChargeBankStampDuty;
        private DateTime datRNDateTo, datEDateTo;

        #endregion

        #region Properties
        public string CreditNo
        {
            set { strCreditNo = value; }
            get { return strCreditNo; }
        }
        public string TransNoRev
        {
            set { strTransNoRev = value; }
            get { return strTransNoRev; }
        }
        public string AcctMasBank
        {
            set { strAcctMasBank = value; }
            get { return strAcctMasBank; }
        }
        public string AcctSubBank
        {
            set { strAcctSubBank = value; }
            get { return strAcctSubBank; }
        }
        public DateTime RNDate
        {
            set { datRNDate = value; }
            get { return datRNDate; }
        }
        public string Trandesc
        {
            set { strTrandesc = value; }
            get { return strTrandesc; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public string Ref
        {
            set { strRef = value; }
            get { return strRef; }
        }
        public string PayDesc
        {
            set { strPayDesc = value; }
            get { return strPayDesc; }
        }
        public string CustNo
        {
            set { strCustNo = value; }
            get { return strCustNo; }
        }
        public string AmtWord
        {
            set { strAmtWord = value; }
            get { return strAmtWord; }
        }
        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public string Officer
        {
            set { strOfficer = value; }
            get { return strOfficer; }
        }
        public DateTime ETime
        {
            set { datETime = value; }
            get { return datETime; }
        }
        public DateTime EDate
        {
            set { datEDate = value; }
            get { return datEDate; }
        }
       
        
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }

        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        
        public string RecNo
        {
            set { strRecNo = value; }
            get { return strRecNo; }
        }
        public bool DoNotChargeBankStampDuty
        {
            set { blnDoNotChargeBankStampDuty = value; }
            get { return blnDoNotChargeBankStampDuty; }
        }
        public DateTime RNDateTo
        {
            set { datRNDateTo = value; }
            get { return datRNDateTo; }
        }
        public DateTime EDateTo
        {
            set { datEDateTo = value; }
            get { return datEDateTo; }
        }

        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            var enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (var connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand dbCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("CrNoteAddNew") as SqlCommand;
                        db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "CRNOTE");
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("CrNoteEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "CreditNo", SqlDbType.VarChar, strCreditNo.Trim());
                    db.AddInParameter(dbCommand, "AcctMasBank", SqlDbType.VarChar, strAcctMasBank.Trim());
                    db.AddInParameter(dbCommand, "AcctSubBank", SqlDbType.VarChar, strAcctSubBank.Trim());
                    db.AddInParameter(dbCommand, "Trandesc", SqlDbType.VarChar, strTrandesc.Trim());
                    db.AddInParameter(dbCommand, "Amount", SqlDbType.Decimal, decAmount);
                    db.AddInParameter(dbCommand, "Ref", SqlDbType.VarChar, strRef.Trim());
                    db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
                    db.AddInParameter(dbCommand, "AmtWord", SqlDbType.VarChar, strAmtWord.Trim());
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(dbCommand, "RNDate", SqlDbType.DateTime, datRNDate);
                    db.AddInParameter(dbCommand, "DoNotChargeBankStampDuty", SqlDbType.Bit, blnDoNotChargeBankStampDuty);
                    db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (strTransNoRev.Trim() != "")
                    {
                        var dbCommandDeleteReversal = DeleteReversalCommand();
                        db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                    }
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    throw new Exception(err.Message);
                }
            }
            return enSaveStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist(DataGeneral.PostStatus ePostStatus)
        {
            var blnStatus = false;
            if (strSaveType == "EDIT")
            {
                var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("CrNoteChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCreditNo.Trim());
                var oDs = db.ExecuteDataSet(oCommand);
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

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            var oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            var oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Credit Note Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            var oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datEDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datEDateTo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            var oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer Deposit Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            var oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datRNDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datRNDateTo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            var oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer Deposit Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            var oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            var oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Credit Note 
        public bool GetCrNote(DataGeneral.PostStatus ePostStatus)
        {
            var oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            var blnStatus = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            var dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (ePostStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectUnPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectPosted") as SqlCommand;
            }
            else if (ePostStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("CrNoteSelectReversed") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "CreditNo", SqlDbType.VarChar, strCreditNo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            var oDS = db.ExecuteDataSet(dbCommand);
            var thisTable = oDS.Tables[0];
            var thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCreditNo = thisRow[0]["CreditNo"].ToString();
                strAcctMasBank = thisRow[0]["AcctMasBank"].ToString();
                strAcctSubBank = thisRow[0]["AcctSubBank"].ToString();
                strTrandesc = thisRow[0]["Trandesc"].ToString();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString());
                strRef = thisRow[0]["Ref"].ToString();
                strPayDesc = thisRow[0]["PayDesc"].ToString();
                strCustNo = thisRow[0]["CustNo"].ToString();
                strAmtWord = thisRow[0]["AmtWord"].ToString();
                blnDoNotChargeBankStampDuty = thisRow[0]["DoNotChargeBankStampDuty"] != null && thisRow[0]["DoNotChargeBankStampDuty"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DoNotChargeBankStampDuty"].ToString().Trim()) : false;
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                datRNDate = DateTime.ParseExact(DateTime.Parse(thisRow[0]["RNDate"].ToString()).ToString("d", dtfi), "dd/MM/yyyy", format);
                strOfficer = thisRow[0]["Officer"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Update Post and Return A Command
        public SqlCommand UpDatePostCommand()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("CrNoteUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCreditNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Update Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("CrNoteUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strCreditNo.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            var blnStatus = false;
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("CrNoteDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCreditNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var oCommand = db.GetStoredProcCommand("CrNoteDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Get Do Not Charge Bank StampDuty
        public bool GetDoNotChargeBankStampDuty(string strTransactionNumber)
        {
            var factory = new DatabaseProviderFactory(); var db = factory.Create("GlobalSuitedb") as SqlDatabase;
            var dbCommand = db.GetStoredProcCommand("CrNoteSelectDoNotChargeBankStampDuty") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransactionNumber.Trim());
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? bool.Parse(varResult.ToString().Trim()) : false;
        }
        #endregion   
    }
}
