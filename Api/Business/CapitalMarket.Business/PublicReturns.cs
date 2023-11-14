using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;
using GL.Business;

namespace CapitalMarket.Business
{
    public class PublicReturns
    {
        #region Declaration
        private string strTxn, strTransNoRev, strOfferCode;
        private DateTime datiDate, datiDateTo;
        private string strStockCode, strCustAid, strMasAcct, strSubAcct, strSurnameApplicant;
        private string strOtherNameApplicant, strTitleApplicant, strSurnameApplicant2;
        private string strOtherNameApplicant2, strTitleApplicant2, strApplicantAddress1;
        private string strApplicantAddress2, strApplicantAddress3;
        private Int32 intApplyUnits;
        private decimal decUnitPrice, decAmountDue, decAmountPaid;
        private string strApliType;
        private DateTime datApliDate;
        private string strRcno;
        private bool blnPosted, blnReversed;
        private decimal decRealAmtPaid;
        private string strCustStatus,strApproved, strSaveType;
        private Int32 intAcceptUnits, intAddUnits;
        private string strPAOfferBank, strAOfferBank,strBranch;
        #endregion

        #region Properties
        public string Txn
        {
            set { strTxn = value; }
            get { return strTxn; }
        }
        public string TransNoRev
        {
            set { strTransNoRev = value; }
            get { return strTransNoRev; }
        }
        public string OfferCode
        {
            set { strOfferCode = value; }
            get { return strOfferCode; }
        }
        public DateTime iDate
        {
            set { datiDate = value; }
            get { return datiDate; }
        }
        public DateTime iDateTo
        {
            set { datiDateTo = value; }
            get { return datiDateTo; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public string CustAid
        {
            set { strCustAid = value; }
            get { return strCustAid; }
        }
        public string MasAcct
        {
            set { strMasAcct = value; }
            get { return strMasAcct; }
        }
        public string SubAcct
        {
            set { strSubAcct = value; }
            get { return strSubAcct; }
        }
        public string SurnameApplicant
        {
            set { strSurnameApplicant = value; }
            get { return strSurnameApplicant; }
        }
        public string OtherNameApplicant
        {
            set { strOtherNameApplicant = value; }
            get { return strOtherNameApplicant; }
        }
        public string TitleApplicant
        {
            set { strTitleApplicant = value; }
            get { return strTitleApplicant; }
        }
        public string SurnameApplicant2
        {
            set { strSurnameApplicant2 = value; }
            get { return strSurnameApplicant2; }
        }
        public string OtherNameApplicant2
        {
            set { strOtherNameApplicant2 = value; }
            get { return strOtherNameApplicant2; }
        }
        public string TitleApplicant2
        {
            set { strTitleApplicant2 = value; }
            get { return strTitleApplicant2; }
        }
        public string ApplicantAddress1
        {
            set { strApplicantAddress1 = value; }
            get { return strApplicantAddress1; }
        }
        public string ApplicantAddress2
        {
            set { strApplicantAddress2 = value; }
            get { return strApplicantAddress2; }
        }
        public string ApplicantAddress3
        {
            set { strApplicantAddress3 = value; }
            get { return strApplicantAddress3; }
        }
        public Int32 ApplyUnits
        {
            set { intApplyUnits = value; }
            get { return intApplyUnits; }
        }
        public decimal UnitPrice
        {
            set { decUnitPrice = value; }
            get { return decUnitPrice; }
        }
        public decimal AmountDue
        {
            set { decAmountDue = value; }
            get { return decAmountDue; }
        }
        public decimal AmountPaid
        {
            set { decAmountPaid = value; }
            get { return decAmountPaid; }
        }
        public string ApliType
        {
            set { strApliType = value; }
            get { return strApliType; }
        }
        public DateTime ApliDate
        {
            set { datApliDate = value; }
            get { return datApliDate; }
        }
        public string Rcno
        {
            set { strRcno = value; }
            get { return strRcno; }
        }
       
        
        public bool Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public bool Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }
        public decimal RealAmtPaid
        {
            set { decRealAmtPaid = value; }
            get { return decRealAmtPaid; }
        }
        public string CustStatus
        {
            set { strCustStatus = value; }
            get { return strCustStatus; }
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
        public Int32 AcceptUnits
        {
            set { intAcceptUnits = value; }
            get { return intAcceptUnits; }
        }
        public Int32 AddUnits
        {
            set { intAddUnits = value; }
            get { return intAddUnits; }
        }
        public string PAOfferBank
        {
            set { strPAOfferBank = value; }
            get { return strPAOfferBank; }
        }
        public string AOfferBank
        {
            set { strAOfferBank = value; }
            get { return strAOfferBank; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
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
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand dbCommand = null;
                    if (strSaveType == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("PublicReturnsAddNew") as SqlCommand;
                        db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "OFFERRETURNS");
                    }
                    else if (strSaveType == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("PublicReturnsEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
                    db.AddInParameter(dbCommand, "OfferCode", SqlDbType.VarChar, strOfferCode.Trim());
                    db.AddInParameter(dbCommand, "iDate", SqlDbType.DateTime, datiDate);
                    db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                    db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAid.Trim());
                    db.AddInParameter(dbCommand, "MasAcct", SqlDbType.VarChar, strMasAcct.Trim());
                    db.AddInParameter(dbCommand, "SubAcct", SqlDbType.VarChar, strSubAcct.Trim());
                    db.AddInParameter(dbCommand, "SurnameApplicant", SqlDbType.VarChar, strSurnameApplicant.Trim());
                    db.AddInParameter(dbCommand, "OtherNameApplicant", SqlDbType.VarChar, strOtherNameApplicant.Trim());
                    db.AddInParameter(dbCommand, "TitleApplicant", SqlDbType.VarChar, strTitleApplicant.Trim());
                    db.AddInParameter(dbCommand, "SurnameApplicant2", SqlDbType.VarChar, strSurnameApplicant2.Trim());
                    db.AddInParameter(dbCommand, "OtherNameApplicant2", SqlDbType.VarChar, strOtherNameApplicant2.Trim());
                    db.AddInParameter(dbCommand, "TitleApplicant2", SqlDbType.VarChar, strTitleApplicant2);
                    db.AddInParameter(dbCommand, "ApplicantAddress1", SqlDbType.VarChar, strApplicantAddress1);
                    db.AddInParameter(dbCommand, "ApplicantAddress2", SqlDbType.VarChar, strApplicantAddress2);
                    db.AddInParameter(dbCommand, "ApplicantAddress3", SqlDbType.VarChar, strApplicantAddress3);
                    db.AddInParameter(dbCommand, "ApplyUnits", SqlDbType.Int, intApplyUnits);
                    db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
                    db.AddInParameter(dbCommand, "AmountDue", SqlDbType.Decimal, decAmountDue);
                    db.AddInParameter(dbCommand, "AmountPaid", SqlDbType.Decimal, decAmountPaid);
                    db.AddInParameter(dbCommand, "Rcno", SqlDbType.VarChar, strRcno);
                    db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(dbCommand, "RealAmtPaid", SqlDbType.Decimal, decRealAmtPaid);
                    db.AddInParameter(dbCommand, "CustStatus", SqlDbType.VarChar, strCustStatus.Trim());
                    db.AddInParameter(dbCommand, "ApliType", SqlDbType.VarChar, strApliType.Trim());
                    db.AddInParameter(dbCommand, "AcceptUnits", SqlDbType.Int, intAcceptUnits);
                    db.AddInParameter(dbCommand, "AddUnits", SqlDbType.Int, intAddUnits);
                    db.AddInParameter(dbCommand, "PAOfferBank", SqlDbType.VarChar, strPAOfferBank.Trim());
                    db.AddInParameter(dbCommand, "AOfferBank", SqlDbType.VarChar, strAOfferBank.Trim());
                    db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
                    if (strTransNoRev.Trim() != "")
                    {
                        SqlCommand dbCommandDeleteReversal = DeleteReversalCommand();
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
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    oCommand = db.GetStoredProcCommand("PublicReturnsChkTransNoExistUnPosted") as SqlCommand;
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTxn.Trim());
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

        #region Get Public Returns 
        public bool GetPublicReturns(DataGeneral.PostStatus ePostStatus)
        {
            bool blnStatus = false;
            //if (strSaveType == "EDIT")
            //{
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                IFormatProvider format = new CultureInfo("en-GB");
                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                dtfi.ShortDatePattern = "dd/MM/yyyy";
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = null;
                if (ePostStatus == DataGeneral.PostStatus.UnPosted)
                {
                    dbCommand = db.GetStoredProcCommand("PublicReturnsSelectUnPosted") as SqlCommand;
                }
                else if (ePostStatus == DataGeneral.PostStatus.Posted)
                {
                    dbCommand = db.GetStoredProcCommand("PublicReturnsSelectPosted") as SqlCommand;
                }
                else if (ePostStatus == DataGeneral.PostStatus.Reversed)
                {
                    dbCommand = db.GetStoredProcCommand("PublicReturnsSelectReversed") as SqlCommand;
                }
                db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn);
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                DataTable thisTable = oDS.Tables[0];
                DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 1)
                {
                    strTxn = thisRow[0]["Txn#"].ToString();
                    strOfferCode = thisRow[0]["OfferCode"].ToString();
                    datiDate = DateTime.Parse(thisRow[0]["Date"].ToString());
                    strStockCode = thisRow[0]["StockCode"].ToString();
                    strCustAid = thisRow[0]["CustAid"].ToString();
                    strMasAcct = thisRow[0]["MasAcct"].ToString();
                    strSubAcct = thisRow[0]["SubAcct"].ToString();
                    strSurnameApplicant = thisRow[0]["SurnameApplicant"].ToString();
                    strOtherNameApplicant = thisRow[0]["OtherNameApplicant"].ToString();
                    strTitleApplicant = thisRow[0]["TitleApplicant"].ToString();
                    strSurnameApplicant2 = thisRow[0]["SurnameApplicant2"].ToString();
                    strOtherNameApplicant2 = thisRow[0]["OtherNameApplicant2"].ToString();
                    strTitleApplicant2 = thisRow[0]["TitleApplicant2"].ToString();

                    strApplicantAddress1 = thisRow[0]["ApplicantAddress1"].ToString();
                    strApplicantAddress2 = thisRow[0]["ApplicantAddress2"].ToString();
                    strApplicantAddress3 = thisRow[0]["ApplicantAddress3"].ToString();

                    intApplyUnits = int.Parse(thisRow[0]["ApplyUnits"].ToString());
                    decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                    decAmountDue = decimal.Parse(thisRow[0]["AmountDue"].ToString());
                    decAmountPaid = decimal.Parse(thisRow[0]["AmountPaid"].ToString());

                    strRcno = thisRow[0]["Rcno"].ToString();
                    blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                    blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                    strCustStatus = thisRow[0]["CustStatus"].ToString();
                    if (thisRow[0]["ApliType"] != null)
                    {
                        strApliType = thisRow[0]["ApliType"].ToString();
                    }
                    strApproved = thisRow[0]["Approved"].ToString();
                    if (thisRow[0]["AcceptUnits"] != null &&
                        thisRow[0]["AcceptUnits"].ToString().Trim() != "")
                    {
                        intAcceptUnits = int.Parse(thisRow[0]["AcceptUnits"].ToString());
                    }
                    if (thisRow[0]["AddUnits"] != null &&
                        thisRow[0]["AddUnits"].ToString().Trim() != "")
                    {
                        intAddUnits = int.Parse(thisRow[0]["AddUnits"].ToString());
                    }
                    strAOfferBank = thisRow[0]["PAOfferBank"].ToString();
                    strAOfferBank = thisRow[0]["AOfferBank"].ToString();
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            //}
            //else if (strSaveType == "ADDS")
            //{
            //    blnStatus = true;
            //}
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllUnPostedAsc") as SqlCommand;
            }
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Public Returns Transactions By Transaction Date
        public DataSet GetAllGivenTxnDate(DataGeneral.PostStatus TransStatus,DateTime datTxnDateFrom,DateTime datTxnDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenTxnDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenTxnDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenTxnDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenTxnDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnDateFrom", SqlDbType.DateTime, datTxnDateFrom);
            db.AddInParameter(dbCommand, "TxnDateTo", SqlDbType.DateTime, datTxnDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Public Returns Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("PublicReturnsSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime,datiDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datiDateTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        
        #region Get All Public Returns Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PublicReturnsUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

      

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("DepositDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTxn.Trim());


            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {

                    db.ExecuteNonQuery(oCommand, transaction);
                    transaction.Commit();
                    blnStatus = true;

                }
                catch (Exception e)
                {
                    string your = e.Message;
                    transaction.Rollback();
                    blnStatus = false;
                    return blnStatus;

                }
                connection.Close();
            }
            return blnStatus;
        }
        #endregion

        #region Get Total Number Of Returns For A Particular Offer
        public decimal GetTotalOfferApp()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PublicReturnsSelectByOfferCodeReturnSumUnits") as SqlCommand;
            db.AddInParameter(dbCommand, "OfferCode", SqlDbType.VarChar, strOfferCode.Trim());
            db.AddOutParameter(dbCommand, "TotalUnitsApplied", SqlDbType.Money, 8);
            db.ExecuteNonQuery(dbCommand);
            return decimal.Parse(db.GetParameterValue(dbCommand, "TotalUnitsApplied").ToString());
        }
        #endregion

        #region Get Total Number Of Returns For A Particular Offer Real
        public long GetTotalOfferAppReal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PublicReturnsSelectByOfferCodeReturnSumUnitsReal") as SqlCommand;
            db.AddInParameter(dbCommand, "OfferCode", SqlDbType.VarChar, strOfferCode.Trim());
            db.AddOutParameter(dbCommand, "TotalUnitsApplied", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(dbCommand);
            return long.Parse(db.GetParameterValue(dbCommand, "TotalUnitsApplied").ToString());
        }
        #endregion

        #region Check That Offer Application Exist For A Offer Code
        public bool ChkPostedAppExistForOfferCode()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PublicReturnsSelectByPostedAndOfferCode") as SqlCommand;
            db.AddInParameter(dbCommand, "OfferCode", SqlDbType.VarChar, strOfferCode.Trim());
            db.AddOutParameter(dbCommand, "ApplicationExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "ApplicationExist").ToString().Trim() == "0")
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

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PublicReturnsDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Check Customer Account Is Funded
        public bool ChkCustomerAccountIsFunded(string strCustProduct, string strCustomerNumber, decimal decAmountToCheck)
        {
            bool blnStatus = false;
            AcctGL oAcctGL = new AcctGL();
            oAcctGL.AcctRef = strCustProduct;
            oAcctGL.AccountID = strCustomerNumber;
            if (oAcctGL.GetAccountBalanceByCustomer() >= decAmountToCheck)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
    }
}
