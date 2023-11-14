using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class AcctGL
    {

        #region Declaration
        private DateTime datEffectiveDate;
        private string strAccountId, strMasterId, strBranch;
        private decimal decCredit, decDebit;
        private string strDebcred, strDesciption, strTransType, strDescription2;
        private string strSysRef, strRef01, strRef02;
        private string strAcctRef, strAcctRefSecond;
        private string strReverse, strJnumber, strChqno;
        private string strFeeType, strRecAcct, strRecAcctMaster;
        private DataGeneral.GLInstrumentType enumInstrumentType;
        private Int64 lngNextNo;
        private string strOverrideEOM;
        private DateTime datTxnDate;
        private string strPostToOtherBranch;
        private string strClearingDayForTradingTransaction = "N";
        private string strCustomerName;
        private bool blnByPassYearAndMonthPeriodCheck;
        public string CannotFindDataBaseServer { get; set; }
        private bool StatutoryDebitAllowPosting = false;
        #endregion

        #region Properties
        public DateTime EffectiveDate
        {
            set { datEffectiveDate = value; }
            get { return datEffectiveDate; }
        }
        public string AccountID
        {
            set { strAccountId = value; }
            get { return strAccountId; }
        }
        public string MasterID
        {
            set { strMasterId = value; }
            get { return strMasterId; }
        }

        public decimal Credit
        {
            set { decCredit = value; }
            get { return decCredit; }
        }
        public decimal Debit
        {
            set { decDebit = value; }
            get { return decDebit; }
        }
        public string Debcred
        {
            set { strDebcred = value; }
            get { return strDebcred; }
        }
        public string Desciption
        {
            set { strDesciption = value; }
            get { return strDesciption; }
        }
        public string TransType
        {
            set { strTransType = value; }
            get { return strTransType; }
        }
        public string Description2
        {
            set { strDescription2 = value; }
            get { return strDescription2; }
        }

        public string SysRef
        {
            set { strSysRef = value; }
            get { return strSysRef; }
        }
        public string Ref01
        {
            set { strRef01 = value; }
            get { return strRef01; }
        }
        public string Ref02
        {
            set { strRef02 = value; }
            get { return strRef02; }
        }
        public string AcctRef
        {
            set { strAcctRef = value; }
            get { return strAcctRef; }
        }
        public string AcctRefSecond
        {
            set { strAcctRefSecond = value; }
            get { return strAcctRefSecond; }
        }
        
        public string Reverse
        {
            set { strReverse = value; }
            get { return strReverse; }
        }
        public string Jnumber
        {
            set { strJnumber = value; }
            get { return strJnumber; }
        }
        public string Chqno
        {
            set { strChqno = value; }
            get { return strChqno; }
        }
        public DataGeneral.GLInstrumentType InstrumentType
        {
            set { enumInstrumentType = value; }
            get { return enumInstrumentType; }
        }
        public string FeeType
        {
            set { strFeeType = value; }
            get { return strFeeType; }
        }
        public string RecAcct
        {
            set { strRecAcct = value; }
            get { return strRecAcct; }
        }
        public string RecAcctMaster
        {
            set { strRecAcctMaster = value; }
            get { return strRecAcctMaster; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
        public string OverrideEOM
        {
            set { strOverrideEOM = value; }
            get { return strOverrideEOM; }
        }
        public Int64 NextNo
        {
            set { lngNextNo = value; }
            get { return lngNextNo; }
        }
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }
        public string PostToOtherBranch
        {
            set { strPostToOtherBranch = value; }
            get { return strPostToOtherBranch; }
        }
        public string ClearingDayForTradingTransaction
        {
            set { strClearingDayForTradingTransaction = value; }
            get { return strClearingDayForTradingTransaction; }
        }
        public string CustomerName
        {
            set { strCustomerName = value; }
            get { return strCustomerName; }
        }
        public bool ByPassYearAndMonthPeriodCheck
        {
            set { blnByPassYearAndMonthPeriodCheck = value; }
            get { return blnByPassYearAndMonthPeriodCheck; }
        }

        public StringBuilder KYCErrorMessage = new StringBuilder(500);
        #endregion

        private DateTime GetTodayDate()
        {
            IFormatProvider format = new CultureInfo("en-GB", true);
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GetTodayDate") as SqlCommand;
            return DateTime.ParseExact(db.ExecuteScalar(oCommand).ToString(), "dd/MM/yyyy", format);
        }

        #region Date Concatatination
        private DateTime LastDayConcat(string strDateResult)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            string strResult = "";
            strResult = GeneralFunc.GetMonthLastDay(DateTime.ParseExact(strDateResult.Substring(0, 10), "dd/MM/yyyy", format).Month, DateTime.ParseExact(strDateResult.Substring(0, 10), "dd/MM/yyyy", format).Year) + "/" +
            DateTime.ParseExact(strDateResult.Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                DateTime.ParseExact(strDateResult.Substring(0, 10), "dd/MM/yyyy", format).Year.ToString();
            return DateTime.ParseExact(strResult, "dd/MM/yyyy", format);

        }
        #endregion

        //Start Of The Three GL Posting 

        #region Add A GL But Return The Command Object
        public SqlCommand AddCommand()
        {
            Account oAccount = new Account();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            ChecksBeforePosting("N");
            if ((decCredit < 0) && (decDebit == 0))
            {
                decDebit = Math.Abs(decCredit);
                decCredit = 0;
                strDebcred = "D";
            }
            if ((decDebit < 0) && (decCredit == 0))
            {
                decCredit = Math.Abs(decDebit);
                decDebit = 0;
                strDebcred = "C";
            }

            #region Check Customer Account Balance For Debit
            if (decDebit != 0 && strAccountId.Trim() != "")
            {
                string strConsolidatedAccount = "";
                bool blnUserOverdrawnAcctRight = false;
                string strRealProduct;

                GLParam oGLParam = new GLParam();
                oGLParam.Type = "CONSOLIDBALFORCUSTPOST";
                strConsolidatedAccount = oGLParam.CheckParameter();
                blnUserOverdrawnAcctRight = GetUserOverdrawnAccountRight(GeneralFunc.UserName);
                if (!blnUserOverdrawnAcctRight)
                {
                    if (!StatutoryDebitAllowPosting)
                    {
                        strRealProduct = strConsolidatedAccount.Trim() == "YES" ? "ALL" : strAcctRef.Trim();
                        if (!ChkCustomerAccountIsFunded(strRealProduct, strAccountId.Trim(), decDebit))
                        {
                            throw new Exception("Cannot Post The Debit Of " + decDebit.ToString("n") + " To Customer Account: " + strAccountId.Trim() + " Does Not Have Enough Fund");
                        }
                    }
                }
            }
            #endregion

            #region Check Account Is On Lien And A Debit Transaction
            if (strAccountId.Trim() != "" && decDebit != 0)
            {
                if (GetPlaceAccountOnLienByCustomer(strAccountId.Trim()))
                {
                    throw new Exception("Cannot Post Debit To This Account! Customer Account Is Placed On Lien");
                }
            }
            #endregion

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand;
            if ((decCredit != 0) || (decDebit != 0))
            {
                oCommand = db.GetStoredProcCommand("GLAdd") as SqlCommand;
            }
            else
            {
                if (strChqno != "PZR")
                {
                    oCommand = db.GetStoredProcCommand("GLAddNothing") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("GLAdd") as SqlCommand;
                }
            }
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "AccountID", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(oCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
            db.AddInParameter(oCommand, "Credit", SqlDbType.Decimal, decCredit);
            db.AddInParameter(oCommand, "Debit", SqlDbType.Decimal, decDebit);
            db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, strDesciption.Trim());
            db.AddInParameter(oCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
            db.AddInParameter(oCommand, "Debcred", SqlDbType.VarChar, strDebcred.Trim());
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, strRef01.Trim());
            db.AddInParameter(oCommand, "Ref02", SqlDbType.VarChar, strRef02.Trim());
            db.AddInParameter(oCommand, "AcctRef", SqlDbType.VarChar, strAcctRef.Trim());
            //Put Trim Later
            db.AddInParameter(oCommand, "AcctRefSecond", SqlDbType.VarChar, strAcctRefSecond);
            db.AddInParameter(oCommand, "Reverse", SqlDbType.VarChar, strReverse.Trim());
            db.AddInParameter(oCommand, "Jnumber", SqlDbType.VarChar, strJnumber.Trim());
            db.AddInParameter(oCommand, "Chqno", SqlDbType.VarChar, strChqno.Trim());
            db.AddInParameter(oCommand, "InstrumentType", SqlDbType.VarChar, enumInstrumentType.ToString().Trim());
            db.AddInParameter(oCommand, "FeeType", SqlDbType.VarChar, strFeeType.Trim());
            db.AddInParameter(oCommand, "RecAcctSub", SqlDbType.VarChar, strRecAcct.Trim());
            db.AddInParameter(oCommand, "RecAcctMas", SqlDbType.VarChar, strRecAcctMaster.Trim());
            if (strAccountId.Trim() != "")
            {
                db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, oAccount.GetCustomerBranch(strAccountId));
            }
            else
            {
                db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, oAccount.GetAccountBranch(strMasterId));
            }
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            if (enumInstrumentType == DataGeneral.GLInstrumentType.Q)
            {
                GeneralFunc oGeneralFunc = new GeneralFunc();
                GLParam oGLParam = new GLParam();
                if (strClearingDayForTradingTransaction == "Y")
                {
                    db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.TradingClearingDay, Holiday.GetAllReturnList()));
                }
                else
                {
                    db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.BankClearingDay, Holiday.GetAllReturnList()));
                }
            }
            else
            {
                db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, datEffectiveDate);
            }
            return oCommand;
        }
        #endregion

        #region Post Transaction
        public bool Post(string TransType)
        {
            if ((decCredit < 0) || (decDebit < 0))
            {
                throw new Exception("Cannot Post Negative To Customer Or GL Account");
            }
            Account oAccount = new Account();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            string strJnumberNext;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    ChecksBeforePosting("Y");
                    SqlCommand oCommandJnumber = db.GetStoredProcCommand("JnumberAdd") as SqlCommand;
                    db.AddOutParameter(oCommandJnumber, "Jnumber", SqlDbType.BigInt, 8);
                    db.ExecuteNonQuery(oCommandJnumber, transaction);
                    strJnumberNext = db.GetParameterValue(oCommandJnumber, "Jnumber").ToString();

                    SqlCommand oCommand;
                    if ((decCredit != 0) || (decDebit != 0))
                    {
                        oCommand = db.GetStoredProcCommand("GLAdd") as SqlCommand;
                    }
                    else
                    {
                        oCommand = db.GetStoredProcCommand("GLAddNothing") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffectiveDate);
                    db.AddInParameter(oCommand, "AccountID", SqlDbType.VarChar, strAccountId.Trim());
                    db.AddInParameter(oCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
                    db.AddInParameter(oCommand, "Credit", SqlDbType.Money,decCredit);
                    db.AddInParameter(oCommand, "Debit", SqlDbType.Money, 0);
                    db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, GetRealDescriptionTop(TransType.Trim(), strAccountId.Trim()));
                    db.AddInParameter(oCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
                    db.AddInParameter(oCommand, "Debcred", SqlDbType.VarChar, "C");
                    db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
                    db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, strRef01.Trim());
                    db.AddInParameter(oCommand, "Ref02", SqlDbType.VarChar, strRef02.Trim());
                    db.AddInParameter(oCommand, "AcctRef", SqlDbType.VarChar, strAcctRef.Trim());
                    db.AddInParameter(oCommand, "AcctRefSecond", SqlDbType.VarChar, strAcctRefSecond.Trim());
                    db.AddInParameter(oCommand, "Reverse", SqlDbType.VarChar, strReverse.Trim());
                    db.AddInParameter(oCommand, "Jnumber", SqlDbType.VarChar, strJnumberNext.Trim());
                    db.AddInParameter(oCommand, "Chqno", SqlDbType.VarChar, strChqno.Trim());
                    db.AddInParameter(oCommand, "InstrumentType", SqlDbType.VarChar, enumInstrumentType.ToString().Trim());
                    db.AddInParameter(oCommand, "FeeType", SqlDbType.VarChar, strFeeType.Trim());
                    db.AddInParameter(oCommand, "RecAcctSub", SqlDbType.VarChar, strRecAcct.Trim());
                    db.AddInParameter(oCommand, "RecAcctMas", SqlDbType.VarChar, strRecAcctMaster.Trim());
                    if (strAccountId.Trim() != "")
                    {
                        db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, oAccount.GetCustomerBranch(strAccountId));
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, oAccount.GetAccountBranch(strMasterId));
                    }
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    if (enumInstrumentType == DataGeneral.GLInstrumentType.Q)
                    {
                        GeneralFunc oGeneralFunc = new GeneralFunc();
                        GLParam oGLParam = new GLParam();
                        if (strClearingDayForTradingTransaction == "Y")
                        {
                            db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.TradingClearingDay, Holiday.GetAllReturnList()));
                        }
                        else
                        {
                            db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.BankClearingDay, Holiday.GetAllReturnList()));
                        }
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, datEffectiveDate);
                    }

                    #region Check Customer Account Balance For Debit
                    if (decCredit != 0 && strRecAcct.Trim() != "")
                    {
                        string strConsolidatedAccount = "";
                        bool blnUserOverdrawnAcctRight = false;
                        string strRealProduct;

                        GLParam oGLParam = new GLParam();
                        oGLParam.Type = "CONSOLIDBALFORCUSTPOST";
                        strConsolidatedAccount = oGLParam.CheckParameter();
                        blnUserOverdrawnAcctRight = GetUserOverdrawnAccountRight(GeneralFunc.UserName);
                        if (!blnUserOverdrawnAcctRight)
                        {
                            strRealProduct = strConsolidatedAccount.Trim() == "YES" ? "ALL" : strAcctRefSecond.Trim();
                            if (!ChkCustomerAccountIsFunded(strRealProduct, strRecAcct.Trim(), decCredit))
                            {
                                throw new Exception("Cannot Post The Debit Of " + decCredit.ToString("n") + " To Customer Account: " + strRecAcct.Trim() + " Does Not Have Enough Fund");
                            }
                        }
                    }
                    #endregion

                    #region Check Account Is On Lien And A Debit Transaction
                    if (strRecAcct.Trim() != "" && decCredit != 0)
                    {
                        if (GetPlaceAccountOnLienByCustomer(strRecAcct.Trim()))
                        {
                            throw new Exception("Cannot Post Debit To This Account! Customer Account Is Placed On Lien");
                        }
                    }
                    #endregion

                    SqlCommand oCommandDebit = db.GetStoredProcCommand("GLAdd") as SqlCommand;
                    db.AddInParameter(oCommandDebit, "EffDate", SqlDbType.DateTime, datEffectiveDate);
                    db.AddInParameter(oCommandDebit, "AccountID", SqlDbType.VarChar, strRecAcct.Trim());
                    db.AddInParameter(oCommandDebit, "MasterID", SqlDbType.VarChar, strRecAcctMaster.Trim());
                    db.AddInParameter(oCommandDebit, "Credit", SqlDbType.Money, 0);
                    db.AddInParameter(oCommandDebit, "Debit", SqlDbType.Money,decCredit);
                    db.AddInParameter(oCommandDebit, "Description", SqlDbType.VarChar, GetRealDescriptionBottom(TransType, strRecAcct));
                    db.AddInParameter(oCommandDebit, "TransType", SqlDbType.VarChar, strTransType.Trim());
                    db.AddInParameter(oCommandDebit, "Debcred", SqlDbType.VarChar, "D");
                    db.AddInParameter(oCommandDebit, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
                    db.AddInParameter(oCommandDebit, "Ref01", SqlDbType.VarChar, strRef01.Trim());
                    db.AddInParameter(oCommandDebit, "Ref02", SqlDbType.VarChar, strRef02.Trim());
                    db.AddInParameter(oCommandDebit, "AcctRef", SqlDbType.VarChar, strAcctRefSecond.Trim());
                    db.AddInParameter(oCommandDebit, "AcctRefSecond", SqlDbType.VarChar, strAcctRef.Trim());
                    db.AddInParameter(oCommandDebit, "Reverse", SqlDbType.VarChar, strReverse.Trim());
                    db.AddInParameter(oCommandDebit, "Jnumber", SqlDbType.VarChar, strJnumberNext.Trim());
                    db.AddInParameter(oCommandDebit, "Chqno", SqlDbType.VarChar, strChqno.Trim());
                    db.AddInParameter(oCommandDebit, "InstrumentType", SqlDbType.VarChar, enumInstrumentType.ToString().Trim());
                    db.AddInParameter(oCommandDebit, "FeeType", SqlDbType.VarChar, strFeeType.Trim());
                    db.AddInParameter(oCommandDebit, "RecAcctSub", SqlDbType.VarChar, strAccountId.Trim());
                    db.AddInParameter(oCommandDebit, "RecAcctMas", SqlDbType.VarChar, strMasterId.Trim());
                    if (strRecAcct.Trim() != "")
                    {
                        db.AddInParameter(oCommandDebit, "Branch", SqlDbType.VarChar, oAccount.GetCustomerBranch(strRecAcct));
                    }
                    else
                    {
                        db.AddInParameter(oCommandDebit, "Branch", SqlDbType.VarChar, oAccount.GetAccountBranch(strRecAcctMaster));
                    }
                    db.AddInParameter(oCommandDebit, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    if (enumInstrumentType == DataGeneral.GLInstrumentType.Q)
                    {
                        GeneralFunc oGeneralFunc = new GeneralFunc();
                        GLParam oGLParam = new GLParam();
                        if (strClearingDayForTradingTransaction == "Y")
                        {
                            db.AddInParameter(oCommandDebit, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.TradingClearingDay, Holiday.GetAllReturnList()));
                        }
                        else
                        {
                            db.AddInParameter(oCommandDebit, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.BankClearingDay, Holiday.GetAllReturnList()));
                        }
                    }
                    else
                    {
                        db.AddInParameter(oCommandDebit, "ChqDate", SqlDbType.DateTime, datEffectiveDate);
                    }

                    bool blnDoNotChargeStampDuty = false;
                    bool blnChargeVATFee = false;
                    bool blnVATIsInclusive = false;
                    Deposit oDeposit = new Deposit();
                    CNote oCNote = new CNote();
                    DNote oDNote = new DNote();
                    SqlCommand oCommandTransType = new SqlCommand();
                    if (TransType == "DEPOSIT")
                    {
                        blnDoNotChargeStampDuty = oDeposit.GetDoNotChargeBankStampDuty(strRef01.Trim());
                        oCommandTransType = db.GetStoredProcCommand("DepositUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "PAYMENT")
                    {
                        oCommandTransType = db.GetStoredProcCommand("PaymentUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "ACCTOBAL")
                    {
                        oCommandTransType = db.GetStoredProcCommand("AcctOBalanceUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "CRNOTE")
                    {
                        blnDoNotChargeStampDuty = oCNote.GetDoNotChargeBankStampDuty(strRef01.Trim());
                        oCommandTransType = db.GetStoredProcCommand("CrNoteUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "CUSTOBAL")
                    {
                        oCommandTransType = db.GetStoredProcCommand("CustOBalUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "DBNOTE")
                    {
                        oDNote.GetChargeVAT(strRef01.Trim());
                        blnChargeVATFee = oDNote.ChargeVAT;
                        blnVATIsInclusive = oDNote.VATIsInclusive;
                        oCommandTransType = db.GetStoredProcCommand("DbNoteUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "CUSTTRANSFER")
                    {
                        oCommandTransType = db.GetStoredProcCommand("CustTransferUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "GLINVOICE")
                    {
                        oCommandTransType = db.GetStoredProcCommand("GLInvoiceUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "GLRECEIPT")
                    {
                        oCommandTransType = db.GetStoredProcCommand("GLReceiptUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "OFFERRETURNS")
                    {
                        oCommandTransType = db.GetStoredProcCommand("PublicReturnsUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "SELFBAL")
                    {
                        oCommandTransType = db.GetStoredProcCommand("SelfBalUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "ACCT_PETTYVOUCHER")
                    {
                        oCommandTransType = db.GetStoredProcCommand("PettyVoucherUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "ACCTPETTYREPLENISH")
                    {
                        oCommandTransType = db.GetStoredProcCommand("PettyReplenishUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "ACCTPAYABLE")
                    {
                        
                    }
                    else if (TransType == "ACCTRECEIVABLE")
                    {
                        oCommandTransType = db.GetStoredProcCommand("AcctReceivableUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "ACCTPAYABLEPAYMENT")
                    {
                        oCommandTransType = db.GetStoredProcCommand("AcctPayablePaymentUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());

                        AcctPayable oAcctPayable = new AcctPayable();
                        oAcctPayable.TransNo = strAcctRefSecond;
                        SqlCommand dbCommandCompletePay = oAcctPayable.EditCompletePayCommand();
                        db.ExecuteNonQuery(dbCommandCompletePay, transaction);

                    }
                    else if (TransType == "ADMINPAYMENTVOUCHER")
                    {
                        oCommandTransType = db.GetStoredProcCommand("AdminPaymentVoucherUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "PREPAYMENT")
                    {
                        oCommandTransType = db.GetStoredProcCommand("PrepaymentUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "PREPAYMENTEXPENSED")
                    {
                        oCommandTransType = db.GetStoredProcCommand("PrepaymentExpensedUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "ACCRUAL")
                    {
                        oCommandTransType = db.GetStoredProcCommand("AccrualUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "ACCRUALPAYMENT")
                    {
                        oCommandTransType = db.GetStoredProcCommand("AccrualPaymentUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "STAFFLOANACCTPOSTING")
                    {
                        oCommandTransType = db.GetStoredProcCommand("StaffLoanAcctPostingUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef02.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "INTERBRANCHTRANSFER")
                    {
                        oCommandTransType = db.GetStoredProcCommand("InterBranchTransferUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "INTERCOMPANYPOSTING")
                    {
                        oCommandTransType = db.GetStoredProcCommand("InterCompanyPostingUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    else if (TransType == "BONDAPPLICATION")
                    {
                        oCommandTransType = db.GetStoredProcCommand("BondApplicationUpdatePost") as SqlCommand;
                        db.AddInParameter(oCommandTransType, "Code", SqlDbType.VarChar, strRef01.Trim());
                        db.AddInParameter(oCommandTransType, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    }
                    // Credit the first account.
                    db.ExecuteNonQuery(oCommand, transaction);
                    // Debit the second account.
                    db.ExecuteNonQuery(oCommandDebit, transaction);

                    //db.ExecuteNonQuery(oCommandJnumber, transaction);
                    db.ExecuteNonQuery(oCommandTransType, transaction);
                   
                    if (TransType == "PREPAYMENTEXPENSED")
                    {
                        Prepayment oPrepayment = new Prepayment(); oPrepayment.TransNo = strRef02.Trim();
                        PrepaymentSchedule oPrepaymentSchedule = new PrepaymentSchedule(); oPrepaymentSchedule.PrepaymentNumber = long.Parse(strRef02.Trim());
                        PrepaymentExpensed oPrepaymentExpensed = new PrepaymentExpensed(); oPrepaymentExpensed.PrepaymentNumber = long.Parse(strRef02.Trim());
                        if (oPrepaymentSchedule.GetNumberOfSchedule() == oPrepaymentExpensed.GetNumberOfExpensedByPrepaymentPosted())
                        {
                            SqlCommand dbCommandPaymentComplete = oPrepayment.EditCompletePaymentCommand();
                            db.ExecuteNonQuery(dbCommandPaymentComplete, transaction);
                        }
                    }


                    PostCBNStampDuty(ref db, ref transaction, TransType, strJnumberNext, blnDoNotChargeStampDuty);
                    PostVAT(ref db, ref transaction, TransType, strJnumberNext, blnChargeVATFee, blnVATIsInclusive);
                    // Commit the transaction.
                    transaction.Commit();

                    blnStatus = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Add A GL But Return The Command Object FIX
        public SqlCommand AddCommandFIX(string strUserId)
        {
            Account oAccount = new Account();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            ChecksBeforePosting("N");
            if ((decCredit < 0) && (decDebit == 0))
            {
                decDebit = Math.Abs(decCredit);
                decCredit = 0;
                strDebcred = "D";
            }
            if ((decDebit < 0) && (decCredit == 0))
            {
                decCredit = Math.Abs(decDebit);
                decDebit = 0;
                strDebcred = "C";
            }

            #region Check Customer Account Balance For Debit
            if (decDebit != 0 && strAccountId.Trim() != "")
            {
                string strConsolidatedAccount = "";
                bool blnUserOverdrawnAcctRight = false;
                string strRealProduct;

                GLParam oGLParam = new GLParam();
                oGLParam.Type = "CONSOLIDBALFORCUSTPOST";
                strConsolidatedAccount = oGLParam.CheckParameter();
                blnUserOverdrawnAcctRight = GetUserOverdrawnAccountRight(GeneralFunc.UserName);
                if (!blnUserOverdrawnAcctRight)
                {
                    strRealProduct = strConsolidatedAccount.Trim() == "YES" ? "ALL" : strAcctRef.Trim();
                    if (!ChkCustomerAccountIsFunded(strRealProduct, strAccountId.Trim(), decDebit))
                    {
                        throw new Exception("Cannot Post The Debit Of " + decDebit.ToString("n") + " To Customer Account: " + strAccountId.Trim() + " Does Not Have Enough Fund");
                    }
                }
            }
            #endregion

            #region Check Account Is On Lien And A Debit Transaction
            if (strAccountId.Trim() != "" && decDebit != 0)
            {
                if (GetPlaceAccountOnLienByCustomer(strAccountId.Trim()))
                {
                    throw new Exception("Cannot Post Debit To This Account! Customer Account Is Placed On Lien");
                }
            }
            #endregion

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand;
            if ((decCredit != 0) || (decDebit != 0))
            {
                oCommand = db.GetStoredProcCommand("GLAdd") as SqlCommand;
            }
            else
            {
                oCommand = db.GetStoredProcCommand("GLAddNothing") as SqlCommand;
            }
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "AccountID", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(oCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
            db.AddInParameter(oCommand, "Credit", SqlDbType.Decimal, decCredit);
            db.AddInParameter(oCommand, "Debit", SqlDbType.Decimal, decDebit);
            db.AddInParameter(oCommand, "Description", SqlDbType.VarChar, strDesciption.Trim());
            db.AddInParameter(oCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
            db.AddInParameter(oCommand, "Debcred", SqlDbType.VarChar, strDebcred.Trim());
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(oCommand, "Ref01", SqlDbType.VarChar, strRef01.Trim());
            db.AddInParameter(oCommand, "Ref02", SqlDbType.VarChar, strRef02.Trim());
            db.AddInParameter(oCommand, "AcctRef", SqlDbType.VarChar, strAcctRef.Trim());
            //Put Trim Later
            db.AddInParameter(oCommand, "AcctRefSecond", SqlDbType.VarChar, strAcctRefSecond);
            db.AddInParameter(oCommand, "Reverse", SqlDbType.VarChar, strReverse.Trim());
            db.AddInParameter(oCommand, "Jnumber", SqlDbType.VarChar, strJnumber.Trim());
            db.AddInParameter(oCommand, "Chqno", SqlDbType.VarChar, strChqno.Trim());
            db.AddInParameter(oCommand, "InstrumentType", SqlDbType.VarChar, enumInstrumentType.ToString().Trim());
            db.AddInParameter(oCommand, "FeeType", SqlDbType.VarChar, strFeeType.Trim());
            db.AddInParameter(oCommand, "RecAcctSub", SqlDbType.VarChar, strRecAcct.Trim());
            db.AddInParameter(oCommand, "RecAcctMas", SqlDbType.VarChar, strRecAcctMaster.Trim());
            if (strAccountId.Trim() != "")
            {
                db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, oAccount.GetCustomerBranch(strAccountId));
            }
            else
            {
                db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, oAccount.GetAccountBranch(strMasterId));
            }
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            if (enumInstrumentType == DataGeneral.GLInstrumentType.Q)
            {
                GeneralFunc oGeneralFunc = new GeneralFunc();
                GLParam oGLParam = new GLParam();
                if (strClearingDayForTradingTransaction == "Y")
                {
                    db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.TradingClearingDay, Holiday.GetAllReturnList()));
                }
                else
                {
                    db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, oGeneralFunc.AddBusinessDay(datEffectiveDate, oGLParam.BankClearingDay, Holiday.GetAllReturnList()));
                }
            }
            else
            {
                db.AddInParameter(oCommand, "ChqDate", SqlDbType.DateTime, datEffectiveDate);
            }
            return oCommand;
        }
        #endregion

        //End Of The Three GL Posting

        #region Post CBN Stamp Duty
        public void PostCBNStampDuty(ref SqlDatabase db, ref SqlTransaction transaction,string strTransactionType,string strNextJournalNumber,bool blnDoNotChargeStamp)
        {
            if (!blnDoNotChargeStamp)
            {
                if (strTransactionType.Trim() == "DEPOSIT" || strTransactionType.Trim() == "CRNOTE")
                {
                    string strFirstMasterAccountToPost, strFirstCustomerAccountToPost, strFirstProductCodeToPost;
                    decimal decCBNStampDutyAmount, decAmountDeposited, decCBNStampDutyMinimumAmount;
                    string strIncomeAcctForBankStampDuty = "";
                    string strBankName;
                    SqlCommand dbCommandCustomerStampDuty;
                    SqlCommand dbCommandBankStampDuty;
                    Account oAccount = new Account();
                    GLParam oGLParam = new GLParam();
                    oGLParam.Type = "CHARGECBNSTAMPDUTY";
                    if (oGLParam.CheckParameter().Trim() == "YES")
                    {

                        if (oAccount.ChkBankAccount(strRecAcctMaster))
                        {
                            decCBNStampDutyAmount = oGLParam.CBNStampDutyAmount;
                            decCBNStampDutyMinimumAmount = oGLParam.CBNStampDutyMinimumAmtToCharge;
                            decAmountDeposited = decCredit;
                            
                            if (decAmountDeposited >= decCBNStampDutyMinimumAmount)
                            {
                                GLParam oGLParamPostBankStampDutyToIncomeAccount = new GLParam();
                                oGLParamPostBankStampDutyToIncomeAccount.Type = "POSTCBNSTAMPDUTYTOINCOMEACCT";
                                string strPostBankStampDutyToIncomeAcct = oGLParamPostBankStampDutyToIncomeAccount.CheckParameter().Trim();
                                if (strPostBankStampDutyToIncomeAcct == "YES")
                                {
                                    strIncomeAcctForBankStampDuty = oGLParam.IncomeAcctForBankStampDuty;
                                }
                                //Customer Stamp Duty Charge
                                decCredit = 0;
                                decDebit = decCBNStampDutyAmount;
                                oAccount.AccountId = strRecAcctMaster;
                                strBankName = oAccount.GetAccountNameWithoutBranch();
                                strDesciption = "CBN Stamp Duty For =N= " + decAmountDeposited.ToString("N") + " Dep. @ " + strBankName.Trim() + " On " + datEffectiveDate.ToString("dd/MM/yyyy");
                                strDebcred = "D";
                                strJnumber = strNextJournalNumber;
                                enumInstrumentType = DataGeneral.GLInstrumentType.C;
                                strFeeType = "CBNSDCUST";
                                if (strPostBankStampDutyToIncomeAcct == "YES")
                                {
                                    strRecAcctMaster = strIncomeAcctForBankStampDuty;
                                    strRecAcct = "";
                                    strAcctRefSecond = "";
                                }
                                StatutoryDebitAllowPosting = true;
                                dbCommandCustomerStampDuty = AddCommand();
                                db.ExecuteNonQuery(dbCommandCustomerStampDuty, transaction);
                                StatutoryDebitAllowPosting = false;

                                strFirstMasterAccountToPost = strMasterId;
                                strFirstCustomerAccountToPost = strAccountId;
                                strFirstProductCodeToPost = strAcctRef;

                                //Bank Stamp Duty Charge
                                decCredit = decCBNStampDutyAmount;
                                decDebit = 0;
                                strMasterId = strRecAcctMaster;
                                strAccountId = strRecAcct;
                                strAcctRef = strAcctRefSecond;
                                strDesciption = strDesciption + " For " + strCustomerName.Trim();
                                strDebcred = "C";
                                strJnumber = strNextJournalNumber;
                                strRecAcctMaster = strFirstMasterAccountToPost;
                                strRecAcct = strFirstCustomerAccountToPost;
                                strAcctRefSecond = strFirstProductCodeToPost;
                                enumInstrumentType = DataGeneral.GLInstrumentType.C;
                                strFeeType = "CBNSDBANK";
                                dbCommandBankStampDuty = AddCommand();
                                db.ExecuteNonQuery(dbCommandBankStampDuty, transaction);
                            }
                        }
                    }
                }
                else if (strTransactionType.Trim() == "PAYMENT" || strTransactionType.Trim() == "DBNOTE")
                {
                    string strFirstMasterAccountToPost, strFirstCustomerAccountToPost, strFirstProductCodeToPost;
                    string strDescriptionForCustomer;
                    decimal decCBNStampDutyAmount, decAmountDeposited, decCBNStampDutyMinimumAmount;
                    string strIncomeAcctForBankStampDuty = "";
                    string strBankName;
                    SqlCommand dbCommandCustomerStampDuty;
                    SqlCommand dbCommandBankStampDuty;
                    Account oAccount = new Account();
                    GLParam oGLParam = new GLParam();
                    oGLParam.Type = "CHARGECBNSTAMPDUTYFORPAYMENT";
                    if (oGLParam.CheckParameter().Trim() == "YES")
                    {
                        if (oAccount.ChkBankAccount(strMasterId))
                        {

                            decCBNStampDutyAmount = oGLParam.CBNStampDutyAmount;
                            decCBNStampDutyMinimumAmount = oGLParam.CBNStampDutyMinimumAmtToCharge;
                            decAmountDeposited = decCredit;
                            
                            if (decAmountDeposited >= decCBNStampDutyMinimumAmount)
                            {
                                GLParam oGLParamPostBankStampDutyToIncomeAccount = new GLParam();
                                oGLParamPostBankStampDutyToIncomeAccount.Type = "POSTCBNSTAMPDUTYTOINCOMEACCT";
                                string strPostBankStampDutyToIncomeAcct = oGLParamPostBankStampDutyToIncomeAccount.CheckParameter().Trim();
                                if (strPostBankStampDutyToIncomeAcct == "YES")
                                {
                                    strIncomeAcctForBankStampDuty = oGLParam.IncomeAcctForBankStampDuty;
                                }
                                //Bank Stamp Duty Charge
                                decCredit = decCBNStampDutyAmount;
                                decDebit = 0;
                                oAccount.AccountId = strMasterId;
                                strBankName = oAccount.GetAccountNameWithoutBranch();
                                strDescriptionForCustomer = "CBN Stamp Duty For =N= " + decAmountDeposited.ToString("N") + " Dep. @ " + strBankName.Trim() + " On " + datEffectiveDate.ToString("dd/MM/yyyy");
                                strDesciption = strDescriptionForCustomer + " For " + strCustomerName.Trim();
                                if (strPostBankStampDutyToIncomeAcct == "YES")
                                {
                                    strMasterId = strIncomeAcctForBankStampDuty;
                                    strAccountId = "";
                                    strAcctRef = "";
                                }
                                strDebcred = "C";
                                strJnumber = strNextJournalNumber;
                                enumInstrumentType = DataGeneral.GLInstrumentType.C;
                                strFeeType = "CBNSPBANK";
                                dbCommandBankStampDuty = AddCommand();
                                db.ExecuteNonQuery(dbCommandBankStampDuty, transaction);

                                //Customer Stamp Duty Charge
                                strFirstMasterAccountToPost = strMasterId;
                                strFirstCustomerAccountToPost = strAccountId;
                                strFirstProductCodeToPost = strAcctRef;

                                
                                decCredit = 0;
                                decDebit = decCBNStampDutyAmount;
                                strMasterId = strRecAcctMaster;
                                strAccountId = strRecAcct;
                                strAcctRef = strAcctRefSecond;
                                strDesciption = strDescriptionForCustomer;
                                strDebcred = "D";
                                strJnumber = strNextJournalNumber;
                                strRecAcctMaster = strFirstMasterAccountToPost;
                                strRecAcct = strFirstCustomerAccountToPost;
                                strAcctRefSecond = strFirstProductCodeToPost;
                                enumInstrumentType = DataGeneral.GLInstrumentType.C;
                                strFeeType = "CBNSPCUST";
                                dbCommandCustomerStampDuty = AddCommand();
                                db.ExecuteNonQuery(dbCommandCustomerStampDuty, transaction);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Post VAT
        public void PostVAT(ref SqlDatabase db, ref SqlTransaction transaction, string strTransactionType, string strNextJournalNumber, bool blnChargeVAT, bool blnVATInclusive)
        {
            if (strTransactionType.Trim() == "DBNOTE" && blnChargeVAT)
            {
                float fltVATPercentage;
                string strVATAccount;
                decimal decAmountPaid,decVATAmount;
                SqlCommand dbCommandIncomeAccount;
                SqlCommand dbCommandVATAccount;
                SqlCommand dbCommandCustomerAccount;
                GLParam oGLParam = new GLParam();
                Account oAccount = new Account();
                string strIncomeACName;
                oGLParam.GetGLParam();
                oAccount.AccountId = strMasterId;
                strIncomeACName = oAccount.GetAccountNameWithoutBranch();
                fltVATPercentage = oGLParam.VAT;
                strVATAccount = oGLParam.VATAcct;
                decAmountPaid = decCredit;
                decVATAmount = (decCredit * decimal.Parse(fltVATPercentage.ToString()))/100;

                if(blnVATInclusive)
                {
                    //Income Account
                    decCredit = 0;
                    decDebit = decVATAmount;
                    strDesciption = "VAT Charged On =N= " + decAmountPaid.ToString("N") + " Paid ";
                    strDebcred = "D";
                    strJnumber = strNextJournalNumber;
                    strRecAcctMaster = strVATAccount;
                    strRecAcct = "";
                    strAcctRefSecond = "";
                    enumInstrumentType = DataGeneral.GLInstrumentType.C;
                    strFeeType = "VATDINCOME";
                    dbCommandIncomeAccount = AddCommand();
                    db.ExecuteNonQuery(dbCommandIncomeAccount, transaction);

                    //VAT Account
                    decCredit = decVATAmount;
                    decDebit = 0;
                    strRecAcctMaster = strMasterId;
                    strRecAcct = strAccountId;
                    strAcctRefSecond = strAcctRef;
                    strMasterId = strVATAccount;
                    strAccountId = "";
                    strAcctRef = "";
                    strDesciption = "VAT Charged On =N= " + decAmountPaid.ToString("N") + " For " + strDescription2.Trim() + " On " + datEffectiveDate.ToString("dd/MM/yyyy") + " A/C " + strIncomeACName;
                    strDebcred = "C";
                    strJnumber = strNextJournalNumber;
                    enumInstrumentType = DataGeneral.GLInstrumentType.C;
                    strFeeType = "VATCVATAC";
                    dbCommandVATAccount = AddCommand();
                    db.ExecuteNonQuery(dbCommandVATAccount, transaction);
                }
                else
                {
                    //Customer Account
                    decCredit = 0;
                    decDebit = decVATAmount;
                    strMasterId = strRecAcctMaster;
                    strAccountId = strRecAcct;
                    strAcctRef = strAcctRefSecond;
                    strDesciption = "VAT Charged On =N= " + decAmountPaid.ToString("N") + " For " + strDescription2.Trim() + " On " + datEffectiveDate.ToString("dd/MM/yyyy");
                    strDebcred = "D";
                    strJnumber = strNextJournalNumber;
                    strRecAcctMaster = strVATAccount;
                    strRecAcct = "";
                    strAcctRefSecond = "";
                    enumInstrumentType = DataGeneral.GLInstrumentType.C;
                    strFeeType = "VATDCUST";
                    dbCommandCustomerAccount = AddCommand();
                    db.ExecuteNonQuery(dbCommandCustomerAccount, transaction);

                    //VAT Account
                    decCredit = decVATAmount;
                    decDebit = 0;
                    strRecAcctMaster = strMasterId;
                    strRecAcct = strAccountId;
                    strAcctRefSecond = strAcctRef;
                    strMasterId = strVATAccount;
                    strAccountId = "";
                    strAcctRef = "";
                    strDesciption = "VAT Charged On =N= " + decAmountPaid.ToString("N") + " For " + strDescription2.Trim() + " On " + datEffectiveDate.ToString("dd/MM/yyyy") + " A/C " + strIncomeACName;
                    strDebcred = "C";
                    strJnumber = strNextJournalNumber;
                    enumInstrumentType = DataGeneral.GLInstrumentType.C;
                    strFeeType = "VATCVATACC";
                    dbCommandVATAccount = AddCommand();
                    db.ExecuteNonQuery(dbCommandVATAccount, transaction);
                }
            }
        }
        #endregion

        #region Checks Before Posting
        public void ChecksBeforePosting(string strDoublePost)
        {
            #region Declaration
            string strChkEndOfMonth = "";
            string strUserPostDifferentBranch = "";
            string strConsolidatedAccount = "";

            IFormatProvider format = new CultureInfo("en-GB");
            #endregion

            #region Pparam Table Setup
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CHKMONTHPOST";
            strChkEndOfMonth = oGLParam.CheckParameter();

            oGLParam.Type = "USERPOSTTODIFFERENTBRANCH";
            strUserPostDifferentBranch = oGLParam.CheckParameter();

            oGLParam.Type = "CONSOLIDBALFORCUSTPOST";
            strConsolidatedAccount = oGLParam.CheckParameter();

            #endregion

            if (strPostToOtherBranch != null && strPostToOtherBranch.Trim() == "Y")
            {
                strUserPostDifferentBranch = "YES";
            }

            //#region Check KYC Compliance
            //if (!CheckKYCComplete(strAccountId))
            //{
            //    throw new Exception("Cannot Post! Incomplete KYC Documentation For A/C No. " + strAccountId.Trim() + Environment.NewLine + KYCErrorMessage.ToString());
            //}

            //if (strRecAcct.Trim() != "")
            //{
            //    if (!CheckKYCComplete(strRecAcct))
            //    {
            //        throw new Exception("Cannot Post! Incomplete KYC Documentation For A/C No. " + strRecAcct.Trim() + Environment.NewLine + KYCErrorMessage.ToString());
            //    }
            //}
            //#endregion

            #region Check Account Is Activated
            ProductAcct oProductAcct = new ProductAcct();
            oProductAcct.ProductCode = strAcctRef;
            oProductAcct.CustAID = strAccountId;
            if (oProductAcct.AcctDeactivation.Trim() == "Y")
            {
                throw new Exception("Cannot Post! Customer Account Deactivated");
            }
            if (strAcctRefSecond != null && strAcctRefSecond.Trim() != "")
            {
                oProductAcct.ProductCode = strAcctRefSecond;
                oProductAcct.CustAID = strRecAcct;
                if (oProductAcct.AcctDeactivation.Trim() == "Y")
                {
                    throw new Exception("Cannot Post! Customer Account Deactivated");
                }
            }
            #endregion

            #region For Current Date Check
            Company oCompany = new Company();
            if (datEffectiveDate > GetTodayDate())
            {
                throw new Exception("Cannot Post Transaction In The Future");
            }
            #endregion

            #region For EOM Check
            if (strChkEndOfMonth.Trim() == "YES")
            {
                DateTime datReturnMonthDate = oCompany.GetEOMRunDate();
                if (datReturnMonthDate == DateTime.MinValue)
                {
                    if (oCompany.ErrMessageToReturn != null && oCompany.ErrMessageToReturn.Trim() != "")
                    {
                        throw new Exception(oCompany.ErrMessageToReturn.Trim());
                    }
                    else
                    {
                        throw new Exception("Cannot Post Transaction End Of Month Date Is Empty");
                    }
                }
                if (blnByPassYearAndMonthPeriodCheck)
                { }
                else
                {
                    if (datEffectiveDate > LastDayConcat(datReturnMonthDate.ToString()))
                    {
                        throw new Exception("Cannot Post Transaction For Future Month(s) Of Current End Of Month");
                    }
                }
                string strFirstDay = "01" + "/" + DateTime.ParseExact(datReturnMonthDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                            DateTime.ParseExact(datReturnMonthDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Year.ToString();

                if (datEffectiveDate < DateTime.ParseExact(strFirstDay.ToString(), "dd/MM/yyyy", format))
                {
                    throw new Exception("Cannot Post Transaction For Previous Month(s) Of Current End Of Month");
                }

                if (oCompany.CheckMonthIsClosed())
                {
                    throw new Exception("Cannot Post Transaction.Month Is Closed,please Run Start Of Month");
                }

                //if (strOverrideEOM != null && strOverrideEOM.Trim() == "Y")
                //{ }
            }
            #endregion

            #region For EOY Check
            if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
            {
                if (blnByPassYearAndMonthPeriodCheck)
                { }
                else
                {
                    if (!(datEffectiveDate >= oCompany.StartYear && datEffectiveDate <= oCompany.EndYear.AddMonths(oCompany.EndYearDeadlineExtension)))
                    {
                        throw new Exception("Cannot Post Transaction.Effective Date Not In Current Financial Year Period");
                    }
                }
            }
            else
            {
                throw new Exception("Cannot Post Transaction.Financial Year Period Table Not Setup Properly");
            }

            if (oCompany.CheckYearIsClosed())
            {
                throw new Exception("Cannot Post Transaction.Year Is Closed,please Run Start Of Year");
            }
            #endregion

            #region Check If Password Is Expired
            if (datEffectiveDate >= GeneralFunc.ExpireDate)
            {
                throw new Exception("Cannot Post User Password Has Expired, Please Contact Your Software Provider");
            }
            #endregion


            string strAccountBranch = "";
            string strCustomerBranch = "";
            string strAccountBranchSecond = "";
            string strCustomerBranchSecond = "";

            #region Check First Account Exist
            Account oAccount = new Account();
            oAccount.AccountId = strMasterId;
            oAccount.CustomerNo = strAccountId;
            oAccount.Product = strAcctRef;
            oAccount.Branch = oAccount.GetAccountBranch(strMasterId);
            strAccountBranch = oAccount.Branch;
            if (!oAccount.ChkAccountExist())
            {
                if (strAccountId.Trim() == "")
                {
                    throw new Exception("Account Code: " + strMasterId.Trim() + "  Does Not Exist");
                }
                else
                {
                    string strCustomerRefNumber = strAcctRef != null ? strAcctRef.Trim() : "";
                    string errMessage = "Account Code: " + strMasterId.Trim() + " Or Customer Account: " + strCustomerRefNumber + "/" + strAccountId.Trim() + " Does Not Exist";
                    throw new Exception(errMessage);
                }
            }
            if (strAccountId.Trim() != "")
            {
                string strCustomerBranchReturn = "";
                strCustomerBranchReturn = oAccount.GetCustomerBranch(strAccountId).Trim();
                strCustomerBranch = strCustomerBranchReturn;
                if (strCustomerBranchReturn == "")
                {
                    throw new Exception("Branch Code For Customer Account: " + strAccountId.Trim() + " Does Not Exist");
                }

                //Check User Branch And Customer Branch
                if (strUserPostDifferentBranch.Trim() == "NO")
                {
                    if (GeneralFunc.UserBranchNumber.Trim() != strCustomerBranchReturn)
                    {
                        throw new Exception("Cannot Post To Customer Account: " + strAccountId.Trim() + " User And Customer Have Different Branch Account");
                    }
                }
            }
            else
            {
                //Check User Branch And Account Branch
                if (strUserPostDifferentBranch.Trim() == "NO")
                {
                    if (GeneralFunc.UserBranchNumber.Trim() != oAccount.Branch.Trim())
                    {
                        throw new Exception("Cannot Post To GL Account: " + strMasterId.Trim() + " User And Account Have Different Branch Account");
                    }
                }
            }
            #endregion

            #region Check Second Account Exist
            oAccount.AccountId = strRecAcctMaster;
            oAccount.CustomerNo = strRecAcct;
            oAccount.Product = strAcctRefSecond;
            oAccount.Branch = oAccount.GetAccountBranch(strRecAcctMaster);
            strAccountBranchSecond = oAccount.Branch;
            if (!oAccount.ChkAccountExist())
            {
                if (strRecAcct.Trim() == "")
                {
                    throw new Exception("Account Code: " + strRecAcctMaster.Trim() + " Does Not Exist");
                }
                else
                {
                    throw new Exception("Account Code: " + strRecAcctMaster.Trim() + " Or Customer Account: " + strAcctRefSecond != null && strAcctRefSecond.Trim() != "" ? strAcctRefSecond.Trim() + "/" : "" + strRecAcct.Trim() + " Does Not Exist");
                }
            }

            if (strRecAcct.Trim() != "")
            {
                string strCustomerBranchReturnSecondAccount = "";
                strCustomerBranchReturnSecondAccount = oAccount.GetCustomerBranch(strRecAcct).Trim();
                strCustomerBranchSecond = strCustomerBranchReturnSecondAccount;
                if (strCustomerBranchReturnSecondAccount == "")
                {
                    throw new Exception("Branch Code For Customer Account: " + strRecAcct.Trim() + " Does Not Exist");
                }

                #region Check User Branch And Second Customer Branch
                if (strUserPostDifferentBranch.Trim() == "NO")
                {
                    if (GeneralFunc.UserBranchNumber.Trim() != strCustomerBranchReturnSecondAccount)
                    {
                        throw new Exception("Cannot Post To Customer Account: " + strRecAcct.Trim() + " User And Customer Have Different Branch Account");
                    }
                }
                #endregion
            }
            else
            {
                #region Check User Branch And Second Account Branch
                if (strUserPostDifferentBranch.Trim() == "NO")
                {
                    if (GeneralFunc.UserBranchNumber.Trim() != oAccount.Branch.Trim())
                    {
                        throw new Exception("Cannot Post To GL Account: " + strRecAcctMaster.Trim() + " User And Account Have Different Branch Account");
                    }
                }
                #endregion
            }
            #endregion

            #region Check Account And Contra Account The Same Branch
            if (strUserPostDifferentBranch.Trim() == "NO")
            {
                if (strCustomerBranch.Trim() != "" && strCustomerBranchSecond.Trim() != "")
                {
                    if (strCustomerBranch.Trim() != strCustomerBranchSecond.Trim())
                    {
                        throw new Exception("Cannot Post, Customer Account: " + strAccountId.Trim() + " And Contra Customer Account: " + strRecAcct.Trim() + " Not The Same Branch");
                    }
                }
                else if (strCustomerBranch.Trim() != "" && strCustomerBranchSecond.Trim() == "")
                {
                    if (strCustomerBranch.Trim() != strAccountBranchSecond.Trim())
                    {
                        throw new Exception("Cannot Post, Customer Account: " + strAccountId.Trim() + " And Contra GL Account: " + strRecAcctMaster.Trim() + " Not The Same Branch");
                    }
                }
                else if (strCustomerBranch.Trim() == "" && strCustomerBranchSecond.Trim() != "")
                {
                    if (strAccountBranch.Trim() != strCustomerBranchSecond.Trim())
                    {
                        throw new Exception("Cannot Post, GL Account: " + strMasterId.Trim() + " And Contra Customer Account: " + strRecAcct.Trim() + " Not The Same Branch");
                    }
                }
                else
                {
                    if (strAccountBranch.Trim() != strAccountBranchSecond.Trim())
                    {
                        throw new Exception("Cannot Post, GL Account: " + strMasterId.Trim() + " And Contra GL Account: " + strRecAcctMaster.Trim() + " Not The Same Branch");
                    }
                }

            }
            #endregion


            #region For Internal Accounts
            if (strDoublePost.Trim() == "N")
            {
                if (strAccountId.Trim() == "")
                {
                    strAcctRef = "";
                }
                if (strRecAcct.Trim() == "")
                {
                    strAcctRefSecond = "";
                }
            }
            #endregion
        }
        #endregion

        #region Checks Before Posting EOD,EOM,EOY Check Only
        public void ChecksBeforePostingEODEOMEOYCheckOnly()
        {
            #region Declaration
            string strChkEndOfMonth = "";
            IFormatProvider format = new CultureInfo("en-GB");
            #endregion

            #region Pparam Table Setup
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CHKMONTHPOST";
            strChkEndOfMonth = oGLParam.CheckParameter();
            #endregion

            #region For Current Date Check
            Company oCompany = new Company();
            if (datEffectiveDate > GetTodayDate())
            {
                throw new Exception("Cannot Post Transaction In The Future");
            }
            #endregion

            #region For EOM Check
            if (strChkEndOfMonth.Trim() == "YES")
            {
                DateTime datReturnMonthDate = oCompany.GetEOMRunDate();
                if (datReturnMonthDate == DateTime.MinValue)
                {
                    if (oCompany.ErrMessageToReturn != null && oCompany.ErrMessageToReturn.Trim() != "")
                    {
                        throw new Exception(oCompany.ErrMessageToReturn.Trim());
                    }
                    else
                    {
                        throw new Exception("Cannot Post Transaction End Of Month Date Is Empty");
                    }
                }
                if (blnByPassYearAndMonthPeriodCheck)
                { }
                else
                {
                    if (datEffectiveDate > LastDayConcat(datReturnMonthDate.ToString()))
                    {
                        throw new Exception("Cannot Post Transaction For Future Month(s) Of Current End Of Month");
                    }
                }
                string strFirstDay = "01" + "/" + DateTime.ParseExact(datReturnMonthDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Month.ToString().PadLeft(2, char.Parse("0")) + "/" +
                            DateTime.ParseExact(datReturnMonthDate.ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format).Year.ToString();

                if (datEffectiveDate < DateTime.ParseExact(strFirstDay.ToString(), "dd/MM/yyyy", format))
                {
                    throw new Exception("Cannot Post Transaction For Previous Month(s) Of Current End Of Month");
                }

                if (oCompany.CheckMonthIsClosed())
                {
                    throw new Exception("Cannot Post Transaction.Month Is Closed,please Run Start Of Month");
                }

                //if (strOverrideEOM != null && strOverrideEOM.Trim() == "Y")
                //{ }
            }
            #endregion

            #region For EOY Check
            if (oCompany.GetEOYRunDate(GeneralFunc.CompanyNumber))
            {
                if (blnByPassYearAndMonthPeriodCheck)
                { }
                else
                {
                    if (!(datEffectiveDate >= oCompany.StartYear && datEffectiveDate <= oCompany.EndYear.AddMonths(oCompany.EndYearDeadlineExtension)))
                    {
                        throw new Exception("Cannot Post Transaction.Effective Date Not In Current Financial Year Period");
                    }
                }
            }
            else
            {
                throw new Exception("Cannot Post Transaction.Financial Year Period Table Not Setup Properly");
            }

            if (oCompany.CheckYearIsClosed())
            {
                throw new Exception("Cannot Post Transaction.Year Is Closed,please Run Start Of Year");
            }
            #endregion

            #region Check If Password Is Expired
            if (datEffectiveDate >= GeneralFunc.ExpireDate)
            {
                throw new Exception("Cannot Post User Password Has Expired, Please Contact Your Software Provider");
            }
            #endregion
        }
        #endregion

        #region KYC Management

        #region Check KYC Complete
        public bool CheckKYCComplete(string strCustomerNumber)
        {
            bool blnKYCComplete = false;
            CannotFindDataBaseServer = "";

            #region Set Dormant Account
            Company oCompany = new Company();
            oCompany.GetCompany(1);
            ProductAcct oProductAcct = new ProductAcct();
            DateTime datLastDateForCustomer;
            if (oCompany.DormantValidityDay != 0)
            {
                foreach (DataRow oRowView in oProductAcct.GetAllProductByCustomer(strCustomerNumber).Tables[0].Rows)
                {
                    datLastDateForCustomer = GetLastTransactionDateForCustomerAccount(oRowView["ProductCode"].ToString().Trim(), strCustomerNumber);
                    if (datLastDateForCustomer != DateTime.MinValue && GeneralFunc.GetTodayDate().Subtract(datLastDateForCustomer).TotalDays >= oCompany.DormantValidityDay)
                    {
                        oProductAcct.SetProductAccountDormant(oRowView["ProductCode"].ToString().Trim(), strCustomerNumber);
                    }
                }
            }
            #endregion


            KYCErrorMessage.Append("");

            int intClientType = 0; byte[] imgSignature = null; byte[] imgPhoto = null;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustomerNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                intClientType = thisRow[0]["ClientType"] != null && thisRow[0]["ClientType"].ToString().Trim() != "" ? Convert.ToInt16(thisRow[0]["ClientType"]) : 0;
                if (thisRow[0]["Photo"] != System.DBNull.Value)
                {
                    imgPhoto = (byte[])thisRow[0]["Photo"];
                }
                else
                {
                    imgPhoto = null;
                }
                if (thisRow[0]["Signature"] != System.DBNull.Value)
                {
                    imgSignature = (byte[])thisRow[0]["Signature"];
                }
                else
                {
                    imgSignature = null;
                }
            }

            KYCExemption oKYCExemption = new KYCExemption();
            oKYCExemption.CustAID = strCustomerNumber;

            GLParam oGLParam = new GLParam();
            oGLParam.Type = "DONOTSHOWPICANDSIGNKYC";
            string strDoNotShowPicAndSignKYC = oGLParam.CheckParameter();

            if (strDoNotShowPicAndSignKYC != "YES")
            {
                oKYCExemption.KYCDocTypeId = 999999999998;
                if (!oKYCExemption.CheckKYCExemptionExistByCustomerIdAndKYCDocTypeId() && (imgPhoto == null))
                {
                    KYCErrorMessage.Append("Customer PASSPORT PHOTO Is Missing" + Environment.NewLine);
                }

                oKYCExemption.KYCDocTypeId = 999999999999;
                if (!oKYCExemption.CheckKYCExemptionExistByCustomerIdAndKYCDocTypeId() && (imgSignature == null))
                {
                    KYCErrorMessage.Append("Customer SIGNATURE Is Missing" + Environment.NewLine);
                }
            }

            KYCDocTypeForCustomerType oKYCDocTypeForCustomerType = new KYCDocTypeForCustomerType();
            oKYCDocTypeForCustomerType.CustomerTypeId = intClientType;
            CustomerDocumentTracking oCustomerDocumentTracking = new CustomerDocumentTracking();
            foreach (DataRow oRow in oKYCDocTypeForCustomerType.GetAllByCustomerTypeIdNotOptional().Tables[0].Rows)
            {
                try
                {
                    oKYCExemption.KYCDocTypeId = Convert.ToInt64(oRow["KYCDocTypeId"]);
                    if (!oKYCExemption.CheckKYCExemptionExistByCustomerIdAndKYCDocTypeId())
                    {
                        oCustomerDocumentTracking.KYCDocTypeId = Convert.ToInt64(oRow["KYCDocTypeId"]);
                        oCustomerDocumentTracking.CustomerId = strCustomerNumber;
                        if (!oCustomerDocumentTracking.ChkKYCDocTypeIdAndCustomerIdExist())
                        {
                            KYCErrorMessage.Append("Customer " + Convert.ToString(oRow["KYCDocTypeName"]) + " Is Missing" + Environment.NewLine);
                        }
                        else
                        {
                            oCustomerDocumentTracking.GetCustomerDocumentByKYCDocTypeIdAndCustomerId();
                            if (oCustomerDocumentTracking.ExpiryDate != DateTime.MinValue && oCustomerDocumentTracking.ExpiryDate <= GeneralFunc.GetTodayDate())
                            {
                                KYCErrorMessage.Append("Customer " + Convert.ToString(oRow["KYCDocTypeName"]) + " Date Is Expired" + Environment.NewLine);
                            }
                        }
                    }
                }
                catch (SqlException SqlEx)
                {
                    CannotFindDataBaseServer = SqlEx.Message;
                    blnKYCComplete = true;
                    return blnKYCComplete;
                }
            }

            KYCCompulsoryCustExemption oKYCCompulsoryCustExemption = new KYCCompulsoryCustExemption();
            CustomerFieldData.KYCCompulsoryCustomer oKYCCompulsoryCustomer = new CustomerFieldData.KYCCompulsoryCustomer();
            oKYCCompulsoryCustExemption.CustAID = strCustomerNumber;
            foreach (DataRow oRow in oKYCCompulsoryCustomer.GetAll().Tables[0].Rows)
            {
                oKYCCompulsoryCustExemption.CustomerFieldId = Convert.ToString(oRow["CustomerFieldId"]);
                if (!oKYCCompulsoryCustExemption.CheckKYCCompulsoryCustExemptionExistByCustomerIdAndCustomerFieldId())
                {
                    var varReturnValue = GetFieldInKYCCompulsory(strCustomerNumber, Convert.ToString(oRow["CustomerFieldId"]), Convert.ToString(oRow["TableName"]));
                    // 1 Means String
                    if (Convert.ToInt32(oRow["CustomerFieldDataType"]) == 1)
                    {
                        if (varReturnValue == null || varReturnValue.ToString().Trim() == "")
                        {
                            KYCErrorMessage.Append("Customer " + Convert.ToString(oRow["CustomerFieldName"]).ToUpper() + " Is Missing" + Environment.NewLine);
                        }
                    }
                    // 2 Means Int
                    else if (Convert.ToInt32(oRow["CustomerFieldDataType"]) == 2)
                    {
                        if (varReturnValue == null || varReturnValue.ToString().Trim() == "" || Convert.ToInt32(varReturnValue) == 0)
                        {
                            KYCErrorMessage.Append("Customer " + Convert.ToString(oRow["CustomerFieldName"]).ToUpper() + " Is Missing" + Environment.NewLine);
                        }
                    }
                    // 3 Means bool
                    else if (Convert.ToInt32(oRow["CustomerFieldDataType"]) == 3)
                    {
                        if (varReturnValue == null || varReturnValue.ToString().Trim() == "" || Convert.ToBoolean(varReturnValue) == false)
                        {
                            KYCErrorMessage.Append("Customer " + Convert.ToString(oRow["CustomerFieldName"]).ToUpper() + " Is Missing" + Environment.NewLine);
                        }
                    }
                    // 3 Means DateTime
                    else if (Convert.ToInt32(oRow["CustomerFieldDataType"]) == 4)
                    {
                        if (varReturnValue == null || varReturnValue.ToString().Trim() == "" || Convert.ToDateTime(varReturnValue) == SqlDateTime.Null)
                        {
                            KYCErrorMessage.Append("Customer " + Convert.ToString(oRow["CustomerFieldName"]).ToUpper() + " Is Missing" + Environment.NewLine);
                        }
                    }
                }
            }

            if (KYCErrorMessage.ToString().Trim() == "")
            {
                blnKYCComplete = true;
            }
            else
            {
                KYCErrorMessage.Insert(0, "Missing Customer KYC Documents" + Environment.NewLine);
            }

            return blnKYCComplete;
        }
        #endregion

        #region Get Field In KYC Compulsory
        public object GetFieldInKYCCompulsory(string strCustomerNo, string strColumnName, string strTableName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerSelectFieldInKYCCompulsory") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.VarChar, strCustomerNo.Trim());
            db.AddInParameter(dbCommand, "ColumnName", SqlDbType.NVarChar, strColumnName.Trim());
            db.AddInParameter(dbCommand, "TableName", SqlDbType.NVarChar, strTableName.Trim());
            return db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Get Access To Account Incomplete KYC
        public bool GetAccessAcctIncompleteKYC(string strUserName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("UserProfileSelectAccessAcctIncompleteKYC") as SqlCommand;
            db.AddInParameter(dbCommand, "UserName", SqlDbType.VarChar, strUserName.Trim());
            var varReturn = db.ExecuteScalar(dbCommand);
            return varReturn != null && varReturn.ToString().Trim() != "" ? Convert.ToBoolean(varReturn) : false;
        }
        #endregion

        #endregion


        #region Get Operational Activities
        public SqlCommand GetOperationalActivities(DateTime datDateFrom, DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectOperationalActivities") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            db.AddOutParameter(dbCommand, "NumberOfCustomerAcountOpened", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfDormantAccount", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfDormantAccountReactivated", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfStockPurchased", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfStockSold", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "AmountOfStockPurchased", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfStockSold", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfCommissionEarned", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "NumberOfRegistrarTransactions", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfFGNSavingsBond", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfCustomerPayment", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfCustomerDeposits", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfCustomerTransaction", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "NumberOfGLTransaction", SqlDbType.BigInt, 18);
            db.AddOutParameter(dbCommand, "AmountOfCustomerPayment", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfCustomerDeposit", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfCustomerTransaction", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfGLTransaction", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfStockPurchasedByProp", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfStockSoldByProp", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfStockPurchasedByCustomer", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfStockSoldByCustomer", SqlDbType.Money, 18);
            db.AddOutParameter(dbCommand, "AmountOfCapitalNetGainByProp", SqlDbType.Money, 18);
            return dbCommand;
        }
        #endregion

        #region Get Receipt And Purchase Balance
        public decimal GetReceiptPurchaseBalance(string strCustomerNumber, DateTime datEffectiveDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectReceiptPurchaseBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNumber", SqlDbType.VarChar, strCustomerNumber);
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? Convert.ToDecimal(varResult) : 0;
        }
        #endregion

        //Reconciliation

        #region Check Credit Value Exist
        public bool CheckCreditExist(DateTime dtDateFrom, DateTime dtDateTo)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLCheckCreditExist") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
            db.AddInParameter(dbCommand, "Credit", SqlDbType.Money, decCredit);
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, dtDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, dtDateTo);
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDesciption);
            db.AddOutParameter(dbCommand, "NextNo", SqlDbType.BigInt, 10);
            DataSet oDs = db.ExecuteDataSet(dbCommand);
            lngNextNo = long.Parse(db.GetParameterValue(dbCommand, "NextNo").ToString());
            if (lngNextNo > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check Debit Value Exist
        public bool CheckDebitExist(DateTime dtDateFrom, DateTime dtDateTo)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLCheckDebitExist") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
            db.AddInParameter(dbCommand, "Debit", SqlDbType.Money, decDebit);
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, dtDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, dtDateTo);
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDesciption);
            db.AddOutParameter(dbCommand, "NextNo", SqlDbType.BigInt, 10);
            DataSet oDs = db.ExecuteDataSet(dbCommand);
            lngNextNo = long.Parse(db.GetParameterValue(dbCommand, "NextNo").ToString());
            if (lngNextNo > 0)
            {
                blnStatus = true;
            }            
            return blnStatus;
        }
        #endregion

        #region Get All UnMatched By ReconBankStmtTranGL
        public DataSet GetAllUnMatchedByReconBankStmtTranGL(long intReconStmtTranNumber, DateTime datStartDate, DateTime datEndDate,string strAccountId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("GLSelectAllUnMatchedByReconBankStmtTran") as SqlCommand;
            db.AddInParameter(dbCommand, "ReconBankStmtTransaction", SqlDbType.BigInt, intReconStmtTranNumber);
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Initialize Reconcile Return Command
        public SqlCommand InitializeReconcileCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLInitializeReconcile") as SqlCommand;
            return dbCommand;
        }
        #endregion

        #region Update Reconcile
        public SqlCommand UpdateReconcile()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLUpdateReconcile") as SqlCommand;
            db.AddInParameter(dbCommand, "NextNo", SqlDbType.BigInt, lngNextNo);
            return dbCommand;
        }
        #endregion

        #region Initialize Exclude Transaction
        public void InitializeExcludeTransaction()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLInitializeExcludeTransaction") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Update Exclude Transaction
        public void UpdateExcludeTransaction()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLUpdateExcludeTransaction") as SqlCommand;
            db.AddInParameter(dbCommand, "NextNo", SqlDbType.BigInt, lngNextNo);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Reset Exclude Transaction
        public void ResetExcludeTransaction()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLResetExcludeTransaction") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        // End Reconcile

        #region Get All Profit Loss Balance
        public DataSet GetAllProfitLossBalance(DateTime dtDateFrom, DateTime dtDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllProfitLossBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, dtDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, dtDateTo);
            DataSet oDs = db.ExecuteDataSet(dbCommand);
            return oDs;
        }
        #endregion

        #region Get Next Receipt Number
        public int GetNextReceiptNo()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ReceiptSelect") as SqlCommand;
            if (db.ExecuteScalar(oCommand) == null || db.ExecuteScalar(oCommand).ToString() == "")
            {
                return 1;
            }
            else
            {
                return (int.Parse(db.ExecuteScalar(oCommand).ToString()) + 1);
            }

        }
        #endregion

        #region Get Next PV Number
        public int GetNextPVNo()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PVSelect") as SqlCommand;
            if (db.ExecuteScalar(oCommand) == null || db.ExecuteScalar(oCommand).ToString() == "")
            {
                return 1;
            }
            else
            {
                return (int.Parse(db.ExecuteScalar(oCommand).ToString()) + 1);
            }

        }
        #endregion

        #region The Real Description For Post Method For Top
        private string GetRealDescriptionTop(string TransType, string AcctNo)
        {
            string strRealDesc = "";
            if (TransType == "CUSTOBAL")
            {
                if (AcctNo.Trim() != "")
                {
                    strRealDesc = strDesciption.Trim();
                }
                else
                {
                    strRealDesc = strDescription2.Trim();
                }
            }
            else if (TransType == "ACCTOBAL")
            {
                if (AcctNo.Trim() != "")
                {
                    strRealDesc = strDesciption.Trim();
                }
                else
                {
                    strRealDesc = strDescription2.Trim();
                }

            }
            else if (TransType == "PAYMENT")
            {
                strRealDesc = strDescription2.Trim();
            }
            else if (TransType == "DBNOTE")
            {
                strRealDesc = strDescription2.Trim();
            }
            else if (TransType == "INVAPP")
            {
                strRealDesc = strDescription2.Trim();
            }
            else if (TransType == "OFFERRETURNS")
            {
                strRealDesc = strDescription2.Trim();
            }
            else
            {
                strRealDesc = strDesciption.Trim();
            }
            return strRealDesc;
        }
        #endregion

        #region The Real Description For Post Method For Bottom
        private string GetRealDescriptionBottom(string TransType, string AcctNo)
        {
            string strRealDesc = "";
            if (TransType == "CUSTOBAL")
            {
                if (AcctNo.Trim() != "")
                {
                    strRealDesc = strDesciption.Trim();
                }
                else
                {
                    strRealDesc = strDescription2.Trim();
                }
            }
            else if (TransType == "ACCTOBAL")
            {
                if (AcctNo.Trim() != "")
                {
                    strRealDesc = strDesciption.Trim();
                }
                else
                {
                    strRealDesc = strDescription2.Trim();
                }
            }
            else if (TransType == "DEPOSIT")
            {
                strRealDesc = strDescription2.Trim();
            }
            else if (TransType == "CUSTTRANSFER")
            {
                strRealDesc = strDescription2.Trim();
            }
            else if (TransType == "CRNOTE")
            {
                strRealDesc = strDescription2.Trim();
            }
            else if (TransType == "STAFFLOANACCTPOSTING")
            {
                strRealDesc = strDescription2.Trim();
            }
            else
            {
                strRealDesc = strDesciption.Trim();
            }
            return strRealDesc;
        }
        #endregion

        #region Add A GL Balance Only But Return The Command Object
        public SqlCommand AddBalanceOnlyCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAddBalanceOnly") as SqlCommand;
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
            db.AddInParameter(dbCommand, "Credit", SqlDbType.Decimal, decCredit);
            db.AddInParameter(dbCommand, "Debit", SqlDbType.Decimal, decDebit);
            db.AddInParameter(dbCommand, "Debcred", SqlDbType.VarChar, strDebcred.Trim());
            db.AddInParameter(dbCommand, "TxnDate", SqlDbType.VarChar, datTxnDate);

            return dbCommand;
        }
        #endregion

        #region Get GL Transactions Searched By SysRef
        public DataSet GetGLBySysRef()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectBySysRef") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Last Transaction Date For Customer Account
        public DateTime GetLastTransactionDateForCustomerAccount(string strProductNumber,string strCustomerNumber)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectLastTransactionDateForCustomerAccount") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductNumber.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strCustomerNumber.Trim());
            var varLastDate  = db.ExecuteScalar(dbCommand);
            return varLastDate != null && varLastDate.ToString().Trim() != "" ? DateTime.ParseExact(varLastDate.ToString().Trim().Substring(0,10),"dd/MM/yyyy",format) : DateTime.MinValue;
        }
        #endregion

        #region Delete GL Transactions By SysRef
        public bool DeleteGLBySysRef()
        {
            ChecksBeforePostingEODEOMEOYCheckOnly();
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLDeleteBySysRef") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Delete GL Transactions By SysRef Return Command
        public SqlCommand DeleteGLBySysRefCommand()
        {
            ChecksBeforePostingEODEOMEOYCheckOnly();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLDeleteBySysRef") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            return dbCommand;
        }
        #endregion

        #region Delete GL Transactions FIX Return Command
        public SqlCommand DeleteGLFIXCommand(DateTime datPostDate)
        {
            ChecksBeforePostingEODEOMEOYCheckOnly();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLDeleteGLFIX") as SqlCommand;
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datPostDate);
            return dbCommand;
        }
        #endregion

        #region Get GL Transactions Searched By SysRef Bank Margin Trade Bank Auto Reversal
        public DataSet GetGLBySysRefSpecialBank()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectBySysRefSpecialBank") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            db.AddInParameter(dbCommand, "BankMarginCode", SqlDbType.VarChar, strRef02.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get GL Transactions Searched By SysRef Show Non Reverse Transaction
        public DataSet GetGLBySysRefNonRev()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectBySysRefShowNonRev") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Capital Gain Amount By SysRef Non Reverse Transaction
        public decimal GetCapGainAmtBySysRefNonRev()
        {
            decimal decToReturn = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectCapitalGainBySysRefNonRev") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if (oDS.Tables[0].Rows.Count == 1)
            {
                decToReturn = decimal.Parse(oDS.Tables[0].Rows[0]["Credit"].ToString()) 
                                - decimal.Parse(oDS.Tables[0].Rows[0]["Debit"].ToString());
            }
            return decToReturn;
        }
        #endregion
        

        #region Update GL Trans For A SysRef For Reverse Status And Return SqlCommand
        public SqlCommand UpdateGLBySysRefReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GLUpdateGLBySysRefForReversal") as SqlCommand;
            db.AddInParameter(oCommand, "SysRef", SqlDbType.VarChar, strSysRef.Trim());
            return oCommand;
        }
        #endregion

        #region Delete GL Trading Bank Transaction Upload Selected By Date And Return SqlCommand
        public SqlCommand DeleteTradBankUploadByDateCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GLDeleteTradBankUploadByDate") as SqlCommand;
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            return oCommand;
        }
        #endregion

        #region Delete Bank Margin GL Trading Bank Transaction Upload Selected By Date And Return SqlCommand
        public SqlCommand DeleteTradBankUploadByDateBankCommand(string BankMarginCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GLDeleteTradBankUploadByDateBank") as SqlCommand;
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "BankMarginCode", SqlDbType.VarChar, BankMarginCode.Trim());

            return oCommand;
        }
        #endregion

        #region Delete Bank Margin GL Trading Bank Transaction Upload Selected By Date No Margin Code And Return SqlCommand
        public SqlCommand DeleteTradBankUploadByDateBankCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GLDeleteTradBankUploadByDateBankNoMarginCode") as SqlCommand;
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            return oCommand;
        }
        #endregion

        #region Delete NASD Trading Bank Transaction Upload Selected By Date And Return SqlCommand
        public SqlCommand DeleteTradBankUploadByDateNASDCommand(string NASDCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GLDeleteTradBankUploadByDateNASD") as SqlCommand;
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "NASDCode", SqlDbType.VarChar, NASDCode.Trim());

            return oCommand;
        }
        #endregion

        #region Delete Custodian GL Trading Bank Transaction Upload Selected By Date And Return SqlCommand
        public SqlCommand DeleteTradBankUploadByDateCustodianCommand(string CustodianCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("GLDeleteTradBankUploadByDateCustodian") as SqlCommand;
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar, CustodianCode.Trim());

            return oCommand;
        }
        #endregion

        #region Get Total GL Amount Posting For Disk Upload for a Date
        public Decimal GetTotalAmountPostingByDateForUpload(DateTime PostDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLTotalAmountPostByDateDiskPost", PostDate) as SqlCommand;
            return (decimal)db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Get Total GL Amount Posting For Disk Upload of Bank Margin for a Date
        public Decimal GetTotalAmountPostingByDateForUploadBank(DateTime PostDate, string BankMarginCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLTotalAmountPostByDateDiskPostBank", PostDate, BankMarginCode) as SqlCommand;
            return (decimal)db.ExecuteScalar(dbCommand);
        }
        #endregion

        #region Get Account Balance By Investment Account With Date
        public Decimal GetAccountBalanceByInvestmentAccountDate(string strInvestmentType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByInvestmentAccountDate", strInvestmentType.Trim(), datEffectiveDate) as SqlCommand;
            var varAccountBalance = db.ExecuteScalar(dbCommand);
            return varAccountBalance != null && varAccountBalance.ToString() != "" ? Convert.ToDecimal(varAccountBalance) : 0;
        }
        #endregion

        #region Get Account Balance From GL No Date Range
        public Decimal GetAccountBalanceByCustomer()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByCustomer",strAcctRef.Trim(), strAccountId.Trim()) as SqlCommand;
            
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Last Twenty Transaction
        public DataSet GetLastTwentyTransaction()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectLastTwentyTransByCustomer", strAcctRef.Trim(), strAccountId.Trim()) as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Account Balance Credit From GL No Date Range
        public Decimal GetAccountBalanceCreditByCustomer()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceCreditByCustomer",strAcctRef.Trim(), strAccountId.Trim()) as SqlCommand;
            
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Credit No Sale From GL No Date Range
        public Decimal GetAccountBalanceCreditNoSaleByCustomer()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceCreditNoSaleByCustomer", strAcctRef.Trim(), strAccountId.Trim()) as SqlCommand;

            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion
        
        #region Get Account Balance Credit,Debit No Sale And Purchase From GL No Date Range
        public Decimal GetAccountBalanceCreditDebitNoSalePurchaseByCustomer()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceCreditDebitNoSalePurchaseByCustomer", strAcctRef.Trim(), strAccountId.Trim()) as SqlCommand;

            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion
        
        #region Get Account Balance From GL With Date Range
        public Decimal GetAccountBalanceByCustomerDate()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByCustomerDate", strAcctRef.Trim(), strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance From GL With Date Range Exclude Staff Loan
        public Decimal GetAccountBalanceByCustomerDateExcludeStaffLoan()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByCustomerDateExcludeStaffLoan", strAcctRef.Trim(), strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Available Balance From GL With Date Range
        public Decimal GetAccountAvailableBalanceByCustomerDate()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountAvailableBalanceByCustomerDate", strAcctRef.Trim(), strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Available Balance From GL With Date Range Exclude Staff Loan
        public Decimal GetAccountAvailableBalanceByCustomerDateExcludeStaffLoan()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountAvailableBalanceByCustomerDateExcludeStaffLoan", strAcctRef.Trim(), strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        

        #region Get Account Balance Of Customer With Previous Date Range Exclude Staff Loan
        public Decimal GetAccountBalanceByCustomerPreviousDateExcludeStaffLoan()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByCustomerPreviousDateExcludeStaffLoan", strAcctRef.Trim(), strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            var varCustomerAccountBalance = db.ExecuteScalar(dbCommand);
            if (varCustomerAccountBalance != null && varCustomerAccountBalance.ToString().Trim() != "")
            {
                return (decimal)varCustomerAccountBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Of Staff Loan With Previous Date Range
        public Decimal GetAccountBalanceByStaffLoanPreviousDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByStaffLoanPreviousDate") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Of Customer With Previous Date Range
        public Decimal GetAccountBalanceByCustomerPreviousDate()
        {
            if (strAcctRef.Trim() == "")
            {
                strAcctRef = "ALL";
            }
            if (strAccountId.Trim() == "")
            {
                strAccountId = "ALL";
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByCustomerPreviousDate", strAcctRef.Trim(), strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Of Agent Customer With Previous Date Range
        public Decimal GetAccountBalanceByAgentCustomerPreviousDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByAgentCustomerPreviousDate", strAcctRef.Trim(), strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            var varAgentOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varAgentOpeningBalance != null && varAgentOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varAgentOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion
        
        #region Get Account Balance Of GL With Previous Date Range Per Branch
        public Decimal GetAccountBalanceByGLPreviousDatePerBranch(string strTransactionBranch)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLPreviousDatePerBranch", strTransactionBranch.Trim(), datEffectiveDate) as SqlCommand;
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion


        #region Get Account Balance Of GL With Previous Date Range Per Branch No TRADBANK Or An AccountId
        public Decimal GetAccountBalanceByGLPreviousDatePerBranchNoTradBankOrAnAccountId(string strTransactionBranch,string strAccountNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLPreviousDatePerBranchNoTradBankOrAnAccount", strTransactionBranch.Trim(), strAccountNumber, datEffectiveDate) as SqlCommand;
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Of GL With Previous Date Range
        public Decimal GetAccountBalanceByGLPreviousDate()
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "READINGEXCLUDEGLTRAN";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            if (oGLParam.CheckParameter().Trim() == "YES")
            {
                dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLPreviousDateExcludeGLTrans", strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLPreviousDate", strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            }
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Of GL With Previous Date Range Negative
        public Decimal GetAccountBalanceByGLPreviousDateNegative()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLPreviousDateNegative", strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Of GL With Single Date
        public Decimal GetAccountBalanceByGLDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLDate", strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            var varAccountBalance = db.ExecuteScalar(dbCommand);
            return varAccountBalance != null && varAccountBalance.ToString().Trim() != "" ? decimal.Parse(varAccountBalance.ToString()) : 0;
        }
        #endregion

        #region Get Account Balance Of GL With Date Range
        public Decimal GetAccountBalanceByGLDateRange(DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLDateRange", strAccountId.Trim(), datEffectiveDate,datEndDate) as SqlCommand;
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Income Account Balance Of GL With Date Range
        public Decimal GetIncomeAccountBalanceByGLDateRange(DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLIncomeAccountBalanceByGLDateRange",datEffectiveDate,datEndDate) as SqlCommand;
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion
        
        #region Get Account Balance Of GL With Date Range Debit Account
        public Decimal GetAccountBalanceByGLDateDebit()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByGLDateDebit", strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            var varAcountBalance = db.ExecuteScalar(dbCommand);
            return varAcountBalance != null && varAcountBalance.ToString().Trim() != "" ? decimal.Parse(varAcountBalance.ToString()) : 0;
        }
        #endregion

        #region Get Account Balance From GL With Date Range No Product
        public Decimal GetAccountBalanceByCustomerDateNoProduct()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceByCustomerDateNoProduct", strAccountId.Trim(), datEffectiveDate) as SqlCommand;
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Last Effective Date For An Account Transaction
        public DateTime GetLastEffDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLLastEffDateSelectByAccount",  strAcctRef.Trim(),strAccountId.Trim()) as SqlCommand;
            var varLastEffDate = db.ExecuteScalar(dbCommand);
            if (varLastEffDate != null && varLastEffDate.ToString().Trim() != "")
            {
                return (DateTime)varLastEffDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        #endregion

        #region Get Last Transaction Date For An Account Transaction
        public DateTime GetLastTransDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLLastTransDateSelectByAccount", strAcctRef.Trim(), strAccountId.Trim()) as SqlCommand;
            var varLastTransDate = db.ExecuteScalar(dbCommand);
            if (varLastTransDate != null && varLastTransDate.ToString().Trim() != "")
            {
                return (DateTime)varLastTransDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        #endregion

        #region Check If Transaction Exist For An Account
        public bool CheckTransExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLChkTransExist") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterId", SqlDbType.VarChar, strMasterId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 0)
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

        #region Check If Transaction Exist For A Customer Account
        public bool CheckTransExistCustomer()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLChkTransExistCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterID", SqlDbType.VarChar, strMasterId.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check If Transaction Exist For A Customer Account No Master Account
        public bool CheckTransExistCustomerNoMasterAccount()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLChkTransExistCustomerNoMaster") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check If Transaction Sums Up To Zero For An Account
        public bool ChkForZeroBalanceForAccount()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLChkZeroBalance", strAccountId.Trim(), strMasterId.Trim()) as SqlCommand;
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance == null)
            {
                blnStatus = false;
            }
            else if ((decimal)varGLAccountOpeningBalance == 0)
            {
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get All GL Transactions
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectByAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Transactions By Non Upload Online
        public DataSet GetAllByNonUploadOnline()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectByNonUploadOnline") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Old GL Trad Bank Transactions Given Auto Date Number
        public DataSet GetTradTransGivenAllotDate(string strAutoDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedbOld") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectBySysRefOld") as SqlCommand;
            db.AddInParameter(dbCommand, "SysRef", SqlDbType.VarChar, "TRB" + "-" + DateTime.ParseExact(strAutoDate.Substring(0, 10).Trim(), "dd/MM/yyyy", format).Year.ToString() + DateTime.ParseExact(strAutoDate.Substring(0, 10).Trim(), "dd/MM/yyyy", format).Month.ToString() + DateTime.ParseExact(strAutoDate.Substring(0, 10).Trim(), "dd/MM/yyyy", format).Day.ToString());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get GL Account Cleared Balance For The Previous Of The Particular Date.
        public decimal GetGLClearedBal(string strParAcct, string strAcct, DateTime datOpenDate)
        {
            decimal decOpenBal = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectByAcctGetClearedBal") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterID", SqlDbType.VarChar, strParAcct.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAcct.Trim());
            db.AddInParameter(dbCommand, "OpenDate", SqlDbType.DateTime, datOpenDate);
            db.AddOutParameter(dbCommand, "NetAmount", SqlDbType.Money, 8);

            db.ExecuteNonQuery(dbCommand);
            decOpenBal = decimal.Parse(db.GetParameterValue(dbCommand, "NetAmount").ToString());
            return decOpenBal;
        }
        #endregion

        #region Get GL Account Un-Cleared Balance For The Previous Of The Particular Date.
        public decimal GetGLUnClearedBal(string strParAcct, string strAcct, DateTime datOpenDate)
        {
            decimal decOpenBal = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectByAcctGetUnClearedBal") as SqlCommand;
            db.AddInParameter(dbCommand, "CustRef", SqlDbType.VarChar, strAcct.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strParAcct.Trim());
            db.AddInParameter(dbCommand, "OpenDate", SqlDbType.DateTime, datOpenDate);
            db.AddOutParameter(dbCommand, "NetAmount", SqlDbType.Money, 8);

            db.ExecuteNonQuery(dbCommand);
            decOpenBal = decimal.Parse(db.GetParameterValue(dbCommand, "NetAmount").ToString());
            return decOpenBal;
        }
        #endregion

        #region Get Net Current Asset
        public decimal GetNetCurrentAsset(DateTime datDateFrom, DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLCurrentAssetNet") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Net Current Asset Single
        public decimal GetNetCurrentAssetSingle(DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLCurrentAssetNetSingle") as SqlCommand;
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Net Current Liabilities
        public decimal GetNetCurrentLib(DateTime datDateFrom, DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLCurrentLibNet") as SqlCommand;
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Net Current Liabilities Single
        public decimal GetNetCurrentLibSingle(DateTime datDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLCurrentLibNetSingle") as SqlCommand;
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            var varGLAccountOpeningBalance = db.ExecuteScalar(dbCommand);
            if (varGLAccountOpeningBalance != null && varGLAccountOpeningBalance.ToString().Trim() != "")
            {
                return (decimal)varGLAccountOpeningBalance;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get All Credit Postings For Date Range
        public decimal GetAllCreditByDateRange(string strAccountNumber, DateTime datStartDate, DateTime datEndDate)
        {
            decimal decCreditAmt = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllCreditDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            db.AddOutParameter(dbCommand, "NetAmount", SqlDbType.Money, 8);

            db.ExecuteNonQuery(dbCommand);
            decCreditAmt = decimal.Parse(db.GetParameterValue(dbCommand, "NetAmount").ToString());
            return decCreditAmt;
        }
        #endregion

        #region Get All Credit Postings For Date Range
        public decimal GetAllCreditByDateRangeWithBankChargeAccountAsContraAC(string strAccountNumber, DateTime datStartDate, DateTime datEndDate)
        {
            decimal decCreditAmt = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllCreditDateRangeWithBankChargeAccountAsContraAC") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            db.AddOutParameter(dbCommand, "NetAmount", SqlDbType.Money, 8);

            db.ExecuteNonQuery(dbCommand);
            decCreditAmt = decimal.Parse(db.GetParameterValue(dbCommand, "NetAmount").ToString());
            return decCreditAmt;
        }
        #endregion

        #region Get Total Balance For All Account Date Range
        public decimal GetTotalBalanceForAllAccountByDateRange(DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectTotalBalanceForAllAccountDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            db.AddOutParameter(dbCommand, "NetAmount", SqlDbType.Money, 8);

            db.ExecuteNonQuery(dbCommand);
            var decTotalBalance  = db.GetParameterValue(dbCommand, "NetAmount");
            return decTotalBalance != null && decTotalBalance.ToString().Trim() != "" ? decimal.Parse(decTotalBalance.ToString()) : 0;
        }
        #endregion

        #region Get Number Of All Transactions For Date Range
        public long GetTransNumberByDateRange(string strAccountNumber, DateTime datStartDate, DateTime datEndDate)
        {
            long intTransNumber = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectTransNumberDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            db.AddOutParameter(dbCommand, "NetNumber", SqlDbType.BigInt, 8);

            db.ExecuteNonQuery(dbCommand);
            intTransNumber = long.Parse(db.GetParameterValue(dbCommand, "NetNumber").ToString());
            return intTransNumber;
        }
        #endregion

        #region Get Unbalanced Transactions
        public DataSet GetUnbalancedTrans()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectUnbalancedTrans") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check Customer Transaction Exist For Date
        public bool ChkCustomerTxnExistForDate()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectCustomerTxnAndDate") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctRef", SqlDbType.VarChar, strAcctRef.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datTxnDate);
            DataSet oDs = db.ExecuteDataSet(dbCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get All By Customer For Date Range Not Payment WorkSheet
        public DataSet GetAllByCustomerDateRange(string strProductCode, string strAccountNumber,
                                                DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllCustomerDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get All By Customer For Date Range With Opening Balance
        public DataSet GetAllByCustomerDateRangeWithOpeningBalance(string strProductCode, string strAccountNumber,
                                                DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllCustomerDateRangeWithOpeningBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get All By Sub Dealer For Date Range With Opening Balance
        public DataSet GetAllBySubDealerDateRangeWithOpeningBalance(string strProductCode, string strAccountNumber,
                                                DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllSubDealerDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get All By Merge Customer For Date Range With Opening Balance
        public DataSet GetAllByMergeCustomerDateRangeWithOpeningBalance(string strProductCode, string strAccountNumber,
                                                DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllMergeCustomerDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Update Investment Credit
        public void UpdateInvestmentCredit(string strProductCode,string strAccountNumber,DataSet oDsSale)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandCreditOther = db.GetStoredProcCommand("GLUpdateInvestmentCreditOther") as SqlCommand;
            db.AddInParameter(dbCommandCreditOther, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommandCreditOther, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.ExecuteNonQuery(dbCommandCreditOther);

            
            SqlCommand dbCommandCredit;
            foreach (DataRow oRow in oDsSale.Tables[0].Rows)
            {
                dbCommandCredit = db.GetStoredProcCommand("GLUpdateInvestmentCredit") as SqlCommand;
                db.AddInParameter(dbCommandCredit, "RealAmount", SqlDbType.VarChar, decimal.Parse(oRow["TotalAmt"].ToString()));
                db.AddInParameter(dbCommandCredit, "SysRef", SqlDbType.VarChar, "BSS-" + oRow["Txn#"].ToString().Trim());
                db.ExecuteNonQuery(dbCommandCredit);
            }

        }
        #endregion

        #region Get All Transact Type
        public DataSet GetAllTransactType()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllTransactType") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get FXAsset By Date For Monthly Run
        public DataSet GetFXAssetByDateForMonthlyRun(string strSystemRefNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectAllFXAssetByDateForMonthlyRun") as SqlCommand;
            db.AddInParameter(dbCommand, "SystemRefNumber", SqlDbType.VarChar, strSystemRefNumber);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Update GL Account Return Command
        public SqlCommand UpdateGLAccountCommand(string strMasterAccount, string strSubAccount)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLUpdateGLAccount") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterAccount", SqlDbType.VarChar, strMasterAccount.Trim());
            db.AddInParameter(dbCommand, "SubAccount", SqlDbType.VarChar, strSubAccount.Trim());
            return dbCommand;
        }
        #endregion

        #region Check Non Allotment Transaction Exist
        public bool ChkNonAllotmentTransactionExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectNonAllotmentTransaction") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctRef", SqlDbType.VarChar, strAcctRef.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            DataSet oDs = db.ExecuteDataSet(dbCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Account Balance Customer Credit Date Range
        public decimal GetAccountBalanceCustomerCreditDateRange(string strProductCode, string strAccountNumber,
                                                DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceCreditByCustomerDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varTotalCredit = db.ExecuteScalar(dbCommand);
            return varTotalCredit != null && varTotalCredit.ToString().Trim() != "" ? decimal.Parse(varTotalCredit.ToString()) : 0;
            
        }
        #endregion

        #region Get Account Balance Customer Debit Date Range
        public decimal GetAccountBalanceCustomerDebitDateRange(string strProductCode, string strAccountNumber,
                                                DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAccountBalanceDebitByCustomerDateRange") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "AccountID", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varTotalDebit = db.ExecuteScalar(dbCommand);
            return varTotalDebit != null && varTotalDebit.ToString().Trim() != "" ? decimal.Parse(varTotalDebit.ToString()) : 0;

        }
        #endregion

        #region Staff Loan

        #region Get All Staff Loan Repayment
        public Decimal GetAllStaffLoanRepayment(long lngStaffLoanId, DateTime datPayRollLoanDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLStaffLoanAllRepayment") as SqlCommand;
            db.AddInParameter(dbCommand, "StaffLoanId", SqlDbType.BigInt, lngStaffLoanId);
            db.AddInParameter(dbCommand, "LoanDate", SqlDbType.DateTime, datPayRollLoanDate);
            var varLoanRepaymentAmount = db.ExecuteScalar(dbCommand);
            return varLoanRepaymentAmount != null && varLoanRepaymentAmount.ToString().Trim() != "" ? decimal.Parse(varLoanRepaymentAmount.ToString()) : 0;
        }
        #endregion

        #region Get Staff Loan Balance
        public Decimal GetStaffLoanBalance(long lngStaffLoanId, decimal decTotalLoanAmount, DateTime datPayRollLoanDate)
        {
            return Math.Round((Math.Round(decTotalLoanAmount, 2) - Math.Round(GetAllStaffLoanRepayment(lngStaffLoanId, datPayRollLoanDate), 2)), 2);
        }
        #endregion

        #endregion

        #region Fix Asset

        #region Get Fix Asset Accumulated Depreciation
        public Decimal GetFixAssetAccumulatedDepreciation(long lngAssetItemId, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLFixAssetAccumulatedDepreciation") as SqlCommand;
            db.AddInParameter(dbCommand, "AssetItemId", SqlDbType.BigInt, lngAssetItemId);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varDepreciationAmount = db.ExecuteScalar(dbCommand);
            return varDepreciationAmount != null && varDepreciationAmount.ToString().Trim() != "" ? decimal.Parse(varDepreciationAmount.ToString()) : 0;
        }
        #endregion

        #region Get Fix Asset Period Depreciation
        public Decimal GetFixAssetPeriodDepreciation(long lngAssetItemId, DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLFixAssetPeriodDepreciation") as SqlCommand;
            db.AddInParameter(dbCommand, "AssetItemId", SqlDbType.BigInt, lngAssetItemId);
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varDepreciationAmount = db.ExecuteScalar(dbCommand);
            return varDepreciationAmount != null && varDepreciationAmount.ToString().Trim() != "" ? decimal.Parse(varDepreciationAmount.ToString()) : 0;
        }
        #endregion

        #region Get Fix Asset All Depreciation
        public Decimal GetFixAssetAllDepreciation(long lngAssetItemId, DateTime datRunDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLFixAssetAllDepreciation") as SqlCommand;
            db.AddInParameter(dbCommand, "AssetItemId", SqlDbType.BigInt, lngAssetItemId);
            db.AddInParameter(dbCommand, "RunDate", SqlDbType.DateTime, datRunDate);
            var varDepreciationAmount = db.ExecuteScalar(dbCommand);
            return varDepreciationAmount != null && varDepreciationAmount.ToString().Trim() != "" ? decimal.Parse(varDepreciationAmount.ToString()) : 0;
        }
        #endregion

        #region Get Fix Asset All NetBookValue
        public Decimal GetFixAssetAllNetBookValue(long lngAssetItemId, DateTime datRunDate)
        {
            FX_AssetItem oFX_AssetItem = new FX_AssetItem();
            decimal decItemRealAmount = oFX_AssetItem.GetRealAmount(lngAssetItemId.ToString());
            decimal decItemAmount = oFX_AssetItem.GetAmount(lngAssetItemId.ToString());
            return Math.Round((Math.Round(decItemRealAmount == 0 ? decItemAmount : decItemRealAmount, 2) - Math.Round(GetFixAssetAllDepreciation(lngAssetItemId, datRunDate), 2)), 2);
        }
        #endregion

        #region Get Fix Asset Accumulated NetBookValue
        public Decimal GetFixAssetAccumulatedNetBookValue(long lngAssetItemId,DateTime datEndDate)
        {
            FX_AssetItem oFX_AssetItem = new FX_AssetItem();
            decimal decItemRealAmount = oFX_AssetItem.GetRealAmount(lngAssetItemId.ToString());
            decimal decItemAmount = oFX_AssetItem.GetAmount(lngAssetItemId.ToString());
            return Math.Round((Math.Round(decItemRealAmount == 0 ? decItemAmount : decItemRealAmount, 2) - Math.Round(GetFixAssetAccumulatedDepreciation(lngAssetItemId, datEndDate), 2)), 2);
        }
        #endregion

        #endregion

        #region FIX Tradebook
        #region Get All FIX GL Distinct Date
        public DataSet GetAllFixDistinctDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("GLSelectAllFixDistinctDatePosted") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FIX GL Not In Allotment
        public DataSet GetFIXGLNotInAllotment(DateTime datTransDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectFIXGLNotInAllotment") as SqlCommand;
            db.AddInParameter(dbCommand, "TransDate", SqlDbType.DateTime, datTransDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FIX GL Not In Portfolio
        public DataSet GetFIXGLNotInPortfolio(DateTime datTransDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectFIXGLNotInPortfolio") as SqlCommand;
            db.AddInParameter(dbCommand, "TransDate", SqlDbType.DateTime, datTransDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
        #endregion

        #region Get Account With Merge Customer Total Debit And Credit Balancce
        public DataSet GetAccountMergeCustBalance(DateTime datFromDate, DateTime datToDate, string strBroughtForwardBal)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLSelectByMergeCustomerBalalnce") as SqlCommand;
            db.AddInParameter(dbCommand, "FromDate", SqlDbType.DateTime, datFromDate);
            db.AddInParameter(dbCommand, "ToDate", SqlDbType.DateTime, datToDate);
            db.AddInParameter(dbCommand, "BroughtForwardBalance", SqlDbType.Char, strBroughtForwardBal);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get User Overdrawn Account Right
        public bool GetUserOverdrawnAccountRight(string strUserNameToCheck)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("UserProfileSelectOverdrawnacct") as SqlCommand;
            db.AddInParameter(dbCommand, "UserName", SqlDbType.Char, strUserNameToCheck);
            var varOverdrawnAcct = db.ExecuteScalar(dbCommand);
            return varOverdrawnAcct != null && varOverdrawnAcct.ToString().Trim() != "" ? bool.Parse(varOverdrawnAcct.ToString()) : false;
        }
        #endregion

        #region Check Customer Account Is Funded
        public bool ChkCustomerAccountIsFunded(string strCustProduct, string strCustomerNumber, decimal decAmountToCheck)
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

        #region Get Place Account On Lien By Customer
        public bool GetPlaceAccountOnLienByCustomer(string strCustomerNumber)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PlaceAccountOnLienSelectByCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNo", SqlDbType.VarChar, strCustomerNumber);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Transaction Alert
        public DataSet TransactionAlert(string strTransactionType, DateTime datTransactionDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLAlert") as SqlCommand;
            db.AddInParameter(dbCommand, "TransactionType", SqlDbType.VarChar, strTransactionType.Trim());
            db.AddInParameter(dbCommand, "TransactionDate", SqlDbType.DateTime, datTransactionDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        public class AccountWithAmount
        {
            public string AccountId { get; set; }
            public decimal Amount { get; set; }
        }

    }
}
