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
    public class Payment
    {

        #region Declarations
        private string strCode, strTransNoRev;
        private string strAcctMasBank, strAcctSubBank, strTransDesc, strRef, strPVNo;
        private string strChequeNo, strPayDesc, strCustno, strAmtword;
        private string strApproved;
        private string strSaveType, strTransType;
        private decimal decAmount;
        private DateTime datRNDate, datETime, datEDate;
        private bool blnPosted,blnReversed, blnBankDepSlip;
        private DataGeneral.GLInstrumentType enumInstrumentType;
        private int intAcctRef;
        private DateTime datRNDateTo, datEDateTo;
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

        public string PVNo
        {
            set { strPVNo = value; }
            get { return strPVNo; }
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
                        SqlCommand oCommandRnumber = db.GetStoredProcCommand("PVAdd") as SqlCommand;
                        db.AddOutParameter(oCommandRnumber, "Pnumber", SqlDbType.BigInt, 8);
                        db.ExecuteNonQuery(oCommandRnumber, transaction);
                        strRnumberNext = db.GetParameterValue(oCommandRnumber, "Pnumber").ToString();

                        oCommand = db.GetStoredProcCommand("PaymentAddNew") as SqlCommand;
                        db.AddInParameter(oCommand, "RecNo", SqlDbType.VarChar, strRnumberNext.Trim());
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "PAYMENT");
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("PaymentEdit") as SqlCommand;
                        db.AddInParameter(oCommand, "RecNo", SqlDbType.VarChar, strPVNo.Trim());
                    }
                    db.AddInParameter(oCommand, "PaymentNo", SqlDbType.VarChar, strCode.Trim());
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
                    db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
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
                    oCommand = db.GetStoredProcCommand("PaymentChkTransNoExistUnPosted") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAll") as SqlCommand;
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
                dbCommand = db.GetStoredProcCommand("PaymentSelectLatestUnPosted") as SqlCommand;
            }
            
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Payment Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datEDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datEDateTo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Payment Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datRNDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datRNDateTo);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Payment Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PaymentSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
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
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentSelectByRefExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strCode.Trim());
            db.AddInParameter(oCommand, "Ref", SqlDbType.VarChar, strRef.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
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
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentSelectByChequeNoExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strCode.Trim());
            db.AddInParameter(oCommand, "ChequeNo", SqlDbType.VarChar,strChequeNo.Trim());
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
        public bool GetPayment(DataGeneral.PostStatus TranStatus)
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

                dbCommand = db.GetStoredProcCommand("PaymentSelectUnPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Posted)
            {

                dbCommand = db.GetStoredProcCommand("PaymentSelectPosted") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.Reversed)
            {

                dbCommand = db.GetStoredProcCommand("PaymentSelectReversed") as SqlCommand;
            }
            else if (TranStatus == DataGeneral.PostStatus.All)
            {

                dbCommand = db.GetStoredProcCommand("PaymentSelect") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "PaymentNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCode = thisRow[0]["PaymentNo"].ToString().Trim();
                strAcctMasBank = thisRow[0]["AcctMasBank"].ToString().Trim();
                strAcctSubBank = thisRow[0]["AcctSubBank"].ToString().Trim();
                strTransDesc = thisRow[0]["TranDesc"].ToString().Trim();
                strRef = thisRow[0]["Ref"] != null ? thisRow[0]["Ref"].ToString().Trim() : "";
                strCustno = thisRow[0]["Custno"].ToString().Trim();
                strChequeNo = thisRow[0]["ChqueNo"] != null ? thisRow[0]["ChqueNo"].ToString().Trim() : "";
                decAmount = decimal.Parse(thisRow[0]["Amount"].ToString().Trim());
                enumInstrumentType = (DataGeneral.GLInstrumentType)Enum.Parse(typeof(DataGeneral.GLInstrumentType), thisRow[0]["InstrumentType"].ToString().Trim(), false); 
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString().Trim());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString().Trim());
                strPVNo = thisRow[0]["PVNo"].ToString().Trim();
                datRNDate = DateTime.ParseExact(thisRow[0]["RNDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
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
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentUpdatePost") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar,strCode.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentUpdateRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar,strCode.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentDelete") as SqlCommand;
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
            SqlCommand oCommand = db.GetStoredProcCommand("PaymentDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Check Customer Account Is Funded
        public bool ChkCustomerAccountIsFunded(string strCustProduct,string strCustomerNumber,decimal decAmountToCheck)
        {
            bool blnStatus = false;
            Account oAccount = new Account();
            Product oProduct = new Product();
            oProduct.TransNo = strCustProduct;
            AcctGL oAcctGL = new AcctGL();
            oAcctGL.AcctRef = strCustProduct;
            oAcctGL.AccountID = strCustomerNumber;
            if (strCustProduct.Trim() != "ALL" && oAccount.GetAccountType(oProduct.GetProductGLAcct()) == "A100")
            {
                blnStatus = true;
            }
            else
            {
                if (oAcctGL.GetAccountBalanceByCustomer() >= decAmountToCheck)
                {
                    blnStatus = true;
                }
            }
            return blnStatus;
        }
        #endregion
    }
}
