using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Deposit
    {
        #region Declarations
        private string strCode, strTransNoRev;
        private string strAcctMasBank, strAcctSubBank, strTransDesc, strRef,strRecNo;
        private string strChequeNo, strPayDesc, strCustno, strAmtword;
        private string strApproved;
        private string strSaveType,strTransType;
        private decimal decAmount;
        private DateTime datRNDate, datETime, datEDate;
        private bool blnPosted,blnReversed, blnBankDepSlip;
        private DataGeneral.GLInstrumentType enumInstrumentType;
        private int intAcctRef;
        private bool blnDoNotChargeBankStampDuty;
        private DateTime datRNDateTo, datEDateTo;
        private string strExistingCodeNumber;
        #endregion

        #region Properties
        public string Code
        {
            set { strCode = value; }
            get { return strCode; }
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
        public string TransDesc
        {
            set { strTransDesc = value; }
            get { return strTransDesc; }
        }
        public string Ref
        {
            set { strRef = value; }
            get { return strRef; }
        }

        public string RecNo
        {
            set { strRecNo = value; }
            get { return strRecNo; }
        }
        public string ChqueNo
        {
            set { strChequeNo = value; }
            get { return strChequeNo; }
        }
        public string PayDesc
        {
            set { strPayDesc = value; }
            get { return strPayDesc; }
        }
        public string Custno
        {
            set { strCustno = value; }
            get { return strCustno; }
        }
        public string Amtword
        {
            set { strAmtword = value; }
            get { return strAmtword; }
        }
        public string TransType
        {
            set { strTransType = value; }
            get { return strTransType; }
        }

        public int AcctRef
        {
            set { intAcctRef = value; }
            get { return intAcctRef; }
        }
        public string Approved
        {
            set { strApproved = value; }
            get { return strApproved; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }


        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public DateTime RNDate
        {
            set { datRNDate = value; }
            get { return datRNDate; }
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
        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public DataGeneral.GLInstrumentType InstrumentType
        {
            set { enumInstrumentType = value; }
            get { return enumInstrumentType; }
        }
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }
        public bool BankDepSlip
        {
            set { blnBankDepSlip = value; }
            get { return blnBankDepSlip; }
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
        public string ExistingCodeNumber
        {
            set { strExistingCodeNumber = value; }
            get { return strExistingCodeNumber; }
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (strRef != "")
            {
                if (ChkRefExist())
                {
                    enSaveStatus = DataGeneral.SaveStatus.DuplicateRef;
                    return enSaveStatus;
                }
            }
            if (strChequeNo != "")
            {
                if (ChkChequeNoExist())
                {
                    enSaveStatus = DataGeneral.SaveStatus.HeadOfficeExistEdit;
                    return enSaveStatus;
                }
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;

            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand oCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        string strRnumberNext;
                        SqlCommand oCommandRnumber = db.GetStoredProcCommand("ReceiptAdd") as SqlCommand;
                        db.AddOutParameter(oCommandRnumber, "Rnumber", SqlDbType.BigInt, 8);
                        db.AddInParameter(oCommandRnumber, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                        db.ExecuteNonQuery(oCommandRnumber, transaction);
                        strRnumberNext = db.GetParameterValue(oCommandRnumber, "Rnumber").ToString();

                        oCommand = db.GetStoredProcCommand("DepositAddNew") as SqlCommand;
                        db.AddInParameter(oCommand, "RecNo", SqlDbType.VarChar, strRnumberNext.Trim());
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "DEPOSIT");
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("DepositEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
                    if (datRNDate != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "RNDate", SqlDbType.DateTime, datRNDate);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "RNDate", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    
                    db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, SqlDateTime.Null);
                    
                    db.AddInParameter(oCommand, "AcctMasBank", SqlDbType.VarChar, strAcctMasBank.Trim());
                    db.AddInParameter(oCommand, "AcctSubBank", SqlDbType.VarChar, strAcctSubBank.Trim());
                    db.AddInParameter(oCommand, "TransDesc", SqlDbType.VarChar, strTransDesc.Trim());
                    db.AddInParameter(oCommand, "Ref", SqlDbType.VarChar, strRef.Trim());
                    db.AddInParameter(oCommand, "Custno", SqlDbType.VarChar, strCustno.Trim());
                    db.AddInParameter(oCommand, "ChequeNo", SqlDbType.VarChar, strChequeNo.Trim());
                    db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
                    db.AddInParameter(oCommand, "Amtword", SqlDbType.VarChar, strAmtword.Trim().ToUpper());
                    db.AddInParameter(oCommand, "InstrumentType", SqlDbType.VarChar, enumInstrumentType.ToString().Trim());
                    db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(oCommand, "DoNotChargeBankStampDuty", SqlDbType.Bit, blnDoNotChargeBankStampDuty);
                    db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "AcctRef", SqlDbType.Int, intAcctRef);
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar,GeneralFunc.UserBranchNumber.Trim() );
                    db.ExecuteNonQuery(oCommand, transaction);
                    if (strTransNoRev.Trim() != "")
                    {
                        SqlCommand dbCommandDeleteReversal = DeleteReversalCommand();
                        db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                    }
                    transaction.Commit();
                    enSaveStatus = DataGeneral.SaveStatus.Saved;
                    return enSaveStatus;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                
            }

        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("DepositChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
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

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAll") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("DepositSelectLatestUnPosted") as SqlCommand;
            }
            
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer Deposit Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datEDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datEDateTo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer Deposit Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datRNDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datRNDateTo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer Deposit Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check That Reference Number Exist
        public bool ChkRefExist()
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DepositSelectByRefExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.NVarChar, strCode.Trim());
            db.AddInParameter(oCommand, "Ref", SqlDbType.NVarChar, strRef.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            db.AddOutParameter(oCommand, "TransNoExit", SqlDbType.NVarChar, 20);
            db.AddInParameter(oCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                var varResult = db.GetParameterValue(oCommand, "TransNoExit");
                strExistingCodeNumber = varResult != null ? varResult.ToString() : "";
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Cheque Number Exist
        public bool ChkChequeNoExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DepositSelectByChequeExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strCode.Trim());
            db.AddInParameter(oCommand, "ChequeNo", SqlDbType.VarChar, strChequeNo.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            
            return blnStatus;
        }
        #endregion

        #region Get
        public bool GetDeposit(DataGeneral.PostStatus TranStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TranStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectUnPost") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelectReversed") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("DepositSelect") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "ReceiptNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCode = thisRow[0]["ReceiptNo"].ToString().Trim();
                strAcctMasBank = thisRow[0]["AcctMasBank"].ToString();
                strAcctSubBank = thisRow[0]["AcctSubBank"].ToString();
                strTransDesc = thisRow[0]["TranDesc"].ToString().Trim();
                strRef = thisRow[0]["Ref"].ToString().Trim();
                strCustno = thisRow[0]["Custno"].ToString().Trim();
                strChequeNo = thisRow[0]["ChqueNo"].ToString().Trim();
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString().Trim());
                if (thisRow[0]["InstrumentType"] != null && thisRow[0]["InstrumentType"].ToString().Trim() != "")
                {
                    enumInstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), thisRow[0]["InstrumentType"].ToString().Trim(), false);
                }
                else
                {
                    enumInstrumentType = DataGeneral.GLInstrumentType.C;
                }
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString().Trim());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString().Trim());
                strRecNo = thisRow[0]["RecNo"].ToString().Trim();
                blnDoNotChargeBankStampDuty = thisRow[0]["DoNotChargeBankStampDuty"] != null && thisRow[0]["DoNotChargeBankStampDuty"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DoNotChargeBankStampDuty"].ToString().Trim()) : false;
                datRNDate = DateTime.ParseExact(thisRow[0]["RNDate"].ToString().Trim().Substring(0,10), "dd/MM/yyyy", format);
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
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DepositUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReceiptUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DepositDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("DepositDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Get Do Not Charge Bank StampDuty
        public bool GetDoNotChargeBankStampDuty(string strTransactionNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("DepositSelectDoNotChargeBankStampDuty") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransactionNumber.Trim());
            var varResult =  db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? bool.Parse(varResult.ToString().Trim()) : false;
        }
        #endregion    

    }
}
