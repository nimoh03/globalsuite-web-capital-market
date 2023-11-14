using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlTypes;
using CustomerManagement.Business;
using System.Globalization;
using Admin.Business;
using BaseUtility.Business;
using GL.Business;

namespace CapitalMarket.Business
{
    public class JobOrder
    {
        #region Declarations
        private string strCode, strTransNoRev;
        private string strCustNo;
        private DateTime datEffectiveDate, datDateLimit, datTxnDate;
        private string strStockCode;
        private int intUnits, intAmtProc, intBalance, intAmtCanBuy, intTxnType;
        private decimal decPrice, decCustBalance, decUnitPrice, decAmount, decCustCreditLimit,decPriceLimit, decOutStandAmount;
        private string strInstructions;
        private string strCustNo_CD;
        private string strJB_ID, strPort_ID;
        private string strMAcct, strAgAcct, strBroker, strSaveType, strInputFrom;
        private string strPosted, strReversed, strOutMessage,strKYCMissingMessage;
        private string strTransNoReturn,strSubmitMedium,strProduct;
        private DateTime datEffectiveDateTo, datTxnDateTo;
        private bool blnDroppedOrder;
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
        public string CustNo
        {
            set { strCustNo = value; }
            get { return strCustNo; }
        }

        public DateTime EffectiveDate
        {
            set { datEffectiveDate = value; }
            get { return datEffectiveDate; }
        }
        public DateTime DateLimit
        {
            set { datDateLimit = value; }
            get { return datDateLimit; }
        }
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }

        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }

        public int Units
        {
            set { intUnits = value; }
            get { return intUnits; }
        }
        public int AmtProc
        {
            set { intAmtProc = value; }
            get { return intAmtProc; }
        }
        public int Balance
        {
            set { intBalance = value; }
            get { return intBalance; }
        }
        public int AmtCanBuy
        {
            set { intAmtCanBuy = value; }
            get { return intAmtCanBuy; }
        }

        public decimal Price
        {
            set { decPrice = value; }
            get { return decPrice; }
        }
        public decimal CustBalance
        {
            set { decCustBalance = value; }
            get { return decCustBalance; }
        }
        public decimal UnitPrice
        {
            set { decUnitPrice = value; }
            get { return decUnitPrice; }
        }
        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }
        public decimal PriceLimit
        {
            set { decPriceLimit = value; }
            get { return decPriceLimit; }
        }
        public decimal OutStandAmount
        {
            set { decOutStandAmount = value; }
            get { return decOutStandAmount; }
        }
        public decimal CustCreditLimit
        {
            set { decCustCreditLimit = value; }
            get { return decCustCreditLimit; }
        }
        
        public string Instructions
        {
            set { strInstructions = value; }
            get { return strInstructions; }
        }
        public int TxnType
        {
            set { intTxnType = value; }
            get { return intTxnType; }
        }
        public string CustNo_CD
        {
            set { strCustNo_CD = value; }
            get { return strCustNo_CD; }
        }


        public string JB_ID
        {
            set { strJB_ID = value; }
            get { return strJB_ID; }
        }
        public string Port_ID
        {
            set { strPort_ID = value; }
            get { return strPort_ID; }
        }
        public string MAcct
        {
            set { strMAcct = value; }
            get { return strMAcct; }
        }
        public string AgAcct
        {
            set { strAgAcct = value; }
            get { return strAgAcct; }
        }
        public string Broker
        {
            set { strBroker = value; }
            get { return strBroker; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public string InputFrom
        {
            set { strInputFrom = value; }
            get { return strInputFrom; }
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
       
        public string OutMessage
        {
            set { strOutMessage = value; }
            get { return strOutMessage; }
        }

        public string KYCMissingMessage
        {
            set { strKYCMissingMessage = value; }
            get { return strKYCMissingMessage; }
        }
        
        public string TransNoReturn
        {
            set { strTransNoReturn = value; }
            get { return strTransNoReturn; }
        }

        public DateTime EffectiveDateTo
        {
            set { datEffectiveDateTo = value; }
            get { return datEffectiveDateTo; }
        }
       
        public DateTime TxnDateTo
        {
            set { datTxnDateTo = value; }
            get { return datTxnDateTo; }
        }

        public bool DroppedOrder
        {
            set { blnDroppedOrder = value; }
            get { return blnDroppedOrder; }
        }

        public string SubmitMedium
        {
            set { strSubmitMedium = value; }
            get { return strSubmitMedium; }
        }

        public string Product
        {
            set { strProduct = value; }
            get { return strProduct; }
        }
        #endregion

        public JobOrder()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        #region Enum
        public enum SaveStatus
        {
            Nothing, NotExist, Saved, PurchaseEmptyTotalAmount,
            PurchaseNotEnoughFund, SaleNotEnoughStock,
            SaleCannotModifyCertJobOrder, TransactionSavedExist, TransactionPostedExist,
            KYCMissingDocument, ProductStockBrokingOrNASD, StockNotForNSEProduct, StockNotForNASDProduct

        }
        #endregion


        #region Save
        public SaveStatus Save()
        {
            SaveStatus enSaveStatus = SaveStatus.Nothing;

            #region Check If User Has Saved This Order Before And If It Even Exist
            GLParam oGLParamTranExist = new GLParam();
            oGLParamTranExist.Type = "CHKJOBORDEREXIST";
            if (oGLParamTranExist.CheckParameter() == "YES")
            {
                if (ChkTransactionSavedExist())
                {
                    enSaveStatus = SaveStatus.TransactionSavedExist;
                    return enSaveStatus;
                }
                if (ChkTransactionPostedExist())
                {
                    enSaveStatus = SaveStatus.TransactionPostedExist;
                    return enSaveStatus;
                }
            }
            if (!ChkTransNoExist())
            {
                enSaveStatus = SaveStatus.NotExist;
                return enSaveStatus;
            }
            #endregion

            #region Check Customer Have Enough Balance In Account
            GLParam oGLParamJobOrderForDateOnly = new GLParam();
            oGLParamJobOrderForDateOnly.Type = "JOBORDERFORDATEONLY";
            string strJobOrderForDateOnly = oGLParamJobOrderForDateOnly.CheckParameter();

            GLParam oGLParamExcludePendingJobOrder = new GLParam();
            oGLParamExcludePendingJobOrder.Type = "EXCLUDEPENDJOBORDER";
            string strExcludePendingJobOrder = oGLParamExcludePendingJobOrder.CheckParameter();

            

            UserProfile oUserProfile = new UserProfile();
            GrantOD oGrantOD = new GrantOD();
            oGrantOD.Customer = strCustNo;
            oGrantOD.Product = strProduct.Trim();
            oUserProfile.UserName = GeneralFunc.UserName.Trim();
            oUserProfile.GetUserProfile();
            if (!oUserProfile.JobNoInSufficientBal && !oGrantOD.GetUnlimited())
            {
                if (intTxnType == 0)
                {
                    if (decAmount <= 0 || (decUnitPrice <= 0 && decPriceLimit <= 0))
                    {
                        enSaveStatus = SaveStatus.PurchaseEmptyTotalAmount;
                        return enSaveStatus;
                    }
                    if (strExcludePendingJobOrder.Trim() == "YES")
                    {
                        decOutStandAmount = 0;
                    }
                    if ((decCustBalance + oGrantOD.GetOverdraftAmount()) < (decAmount + decOutStandAmount))
                    {
                        enSaveStatus = SaveStatus.PurchaseNotEnoughFund;
                        return enSaveStatus;
                    }
                }
            }
            #endregion

            #region Check That Customer Have Enough Stock In Portfolio
            GLParam oGLParamCheckStockJob = new GLParam();
            oGLParamCheckStockJob.Type = "CHKSTOCKJOB";
            string strCheckStockJob = oGLParamCheckStockJob.CheckParameter();
            if (intTxnType == 1 || intTxnType == 2)
            {
                Portfolio oPortfolio = new Portfolio();
                if (intTxnType == 1)
                {
                    oPortfolio.CustomerAcct = strCustNo;
                }
                else
                {
                    oPortfolio.CustomerAcct = strCustNo_CD;
                }
                oPortfolio.StockCode = strStockCode;
                oPortfolio.Units = intUnits;
                oPortfolio.PurchaseDate = datEffectiveDate;
                
                DataTable dtAllotEmpty = null;
                if (oPortfolio.ChkQuantityStockNotEnough(dtAllotEmpty) && (strCheckStockJob.Trim() == "YES"))
                {
                    Verification oVerification = new Verification();
                    if (intTxnType == 1)
                    {
                        oVerification.CustNo = strCustNo;
                    }
                    else
                    {
                        oVerification.CustNo = strCustNo_CD;
                    }
                    oVerification.Stockcode = strStockCode;
                    strOutMessage = "";
                    strOutMessage = "Stock Quantity Not Enough:  Job "
                        + intUnits.ToString().Trim() + " Of " +
                        StockCode.Trim() + " But Has Only " +
                        oPortfolio.GetNetHolding().ToString().Trim() + " In Portfolio";
                    DataSet oDsUnVerifyCert = oVerification.GetUnPostedGivenCustStock();
                    if (oDsUnVerifyCert.Tables[0].Rows.Count >= 1)
                    {
                        strOutMessage = strOutMessage + "\r\n" + "\t" +
                               "-----Please Verify This UnVerified Certificates-------";
                        strOutMessage = strOutMessage + "\r\n" + "\t" +
                                " PLEASE MAKE SURE RETURNED DATE IS SAME OR EARLIER THAN DATE OF TRANSACTION POSTING";

                        foreach (DataRow oRowUnVerifyCert in oDsUnVerifyCert.Tables[0].Rows)
                        {
                            strOutMessage = strOutMessage + "\r\n" + "\t" +
                                "TransNo: " + oRowUnVerifyCert["Transno"].ToString().Trim() +
                                " CertNo: " + oRowUnVerifyCert["Certno"].ToString().Trim() +
                                " Units: " + oRowUnVerifyCert["Units"].ToString().Trim() +
                                " StockCode: " + oRowUnVerifyCert["StockCode"].ToString().Trim() +
                                " Date Lodged: " + oRowUnVerifyCert["EffDate"].ToString().Trim().Substring(0, 10);
                        }
                    }
                    enSaveStatus = SaveStatus.SaleNotEnoughStock;
                    return enSaveStatus;
                }
                if (ChkJobCertForSale())
                {
                    enSaveStatus = SaveStatus.SaleCannotModifyCertJobOrder;
                    return enSaveStatus;
                }
            }
            #endregion

            if (!ChkKYCDocumentIsComplete(strCustNo.Trim()))
            {
                enSaveStatus = SaveStatus.KYCMissingDocument;
                return enSaveStatus;
            }

            StkParam oStkParam = new StkParam();
            if ((strProduct.Trim() != oStkParam.Product.Trim()) && (strProduct.Trim() != oStkParam.ProductNASDAccount.Trim()))
            {
                enSaveStatus = SaveStatus.ProductStockBrokingOrNASD;
                return enSaveStatus;
            }

            Stock oStock = new Stock();
            oStock.SecCode = strStockCode;
            DataGeneral.StockInstrumentType oStockType = oStock.GetInstrumentTypeUsingStockCode();
            if ((strProduct.Trim() == oStkParam.Product.Trim()) && oStockType != DataGeneral.StockInstrumentType.QUOTEDEQUITY
                && oStockType != DataGeneral.StockInstrumentType.BOND)
            {
                enSaveStatus = SaveStatus.StockNotForNSEProduct;
                return enSaveStatus;
            }

            if ((strProduct.Trim() == oStkParam.ProductNASDAccount.Trim()) && oStock.GetInstrumentTypeUsingStockCode() != DataGeneral.StockInstrumentType.NASD)
            {
                enSaveStatus = SaveStatus.StockNotForNASDProduct;
                return enSaveStatus;
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
                        oCommand = db.GetStoredProcCommand("JobbingAddNew") as SqlCommand;
                        db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "JOBBOOK");

                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("JobbingEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
                    db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
                    if (datDateLimit != DateTime.MinValue)
                    {
                        db.AddInParameter(oCommand, "DateLimit", SqlDbType.DateTime, datDateLimit);

                    }
                    else
                    {
                        db.AddInParameter(oCommand, "DateLimit", SqlDbType.DateTime, SqlDateTime.Null);
                    }
                    db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
                    db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                    db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
                    db.AddInParameter(oCommand, "Price", SqlDbType.Money, decUnitPrice);
                    db.AddInParameter(oCommand, "PriceLimit", SqlDbType.Money, decPriceLimit);
                    db.AddInParameter(oCommand, "Instructions", SqlDbType.NText, strInstructions.Trim());
                    db.AddInParameter(oCommand, "TxnType", SqlDbType.SmallInt, intTxnType);
                    db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "InputFrom", SqlDbType.VarChar, strInputFrom.Trim());
                    db.AddInParameter(oCommand, "AmtProc", SqlDbType.Int, intAmtProc);
                    db.AddInParameter(oCommand, "Balance", SqlDbType.Int, intBalance);
                    db.AddInParameter(oCommand, "CustBalance", SqlDbType.Money, decCustBalance);
                    db.AddInParameter(oCommand, "Broker", SqlDbType.VarChar, strBroker.Trim());
                    db.AddInParameter(oCommand, "CustNo_CD", SqlDbType.VarChar, strCustNo_CD.Trim());
                    db.AddInParameter(oCommand, "JB_ID", SqlDbType.BigInt, Int64.Parse(strJB_ID.Trim()));
                    db.AddInParameter(oCommand, "Posted", SqlDbType.VarChar, strPosted.Trim());
                    db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
                    db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
                    db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
                    db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
                    db.AddInParameter(oCommand, "SubmitMedium", SqlDbType.VarChar,strSubmitMedium.Trim());
                    db.ExecuteNonQuery(oCommand, transaction);
                    if (strTransNoRev.Trim() != "")
                    {
                        SqlCommand dbCommandDeleteReversal = DeleteReversalCommand();
                        db.ExecuteNonQuery(dbCommandDeleteReversal, transaction);
                    }
                    transaction.Commit();
                    enSaveStatus = SaveStatus.Saved;
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

        #region Add Return A Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            if (datDateLimit != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateLimit", SqlDbType.DateTime, datDateLimit);

            }
            else
            {
                db.AddInParameter(oCommand, "DateLimit", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(oCommand, "CustNo", SqlDbType.Char, strCustNo.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
            db.AddInParameter(oCommand, "Price", SqlDbType.Money, decUnitPrice);
            db.AddInParameter(oCommand, "PriceLimit", SqlDbType.Money, decPriceLimit);
            db.AddInParameter(oCommand, "Instructions", SqlDbType.NText, strInstructions.Trim());
            db.AddInParameter(oCommand, "TxnType", SqlDbType.SmallInt, intTxnType);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "InputFrom", SqlDbType.VarChar, strInputFrom.Trim());
            db.AddInParameter(oCommand, "AmtProc", SqlDbType.Int, intAmtProc);
            db.AddInParameter(oCommand, "Balance", SqlDbType.Int, intBalance);
            db.AddInParameter(oCommand, "CustBalance", SqlDbType.Money, decCustBalance);
            db.AddInParameter(oCommand, "Broker", SqlDbType.VarChar, strBroker.Trim());
            db.AddInParameter(oCommand, "CustNo_CD", SqlDbType.VarChar, strCustNo_CD.Trim());
            db.AddInParameter(oCommand, "JB_ID", SqlDbType.BigInt, Int64.Parse(strJB_ID.Trim()));
            db.AddInParameter(oCommand, "Posted", SqlDbType.VarChar, strPosted.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(oCommand, "SubmitMedium", SqlDbType.VarChar, strSubmitMedium.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "JOBBOOK");

            return oCommand;
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
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            if (datEffectiveDate != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffectiveDate);
            }
            else
            {
                db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(dbCommand, "Customer", SqlDbType.Char, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Units", SqlDbType.Int, intUnits);
            db.AddInParameter(dbCommand, "TxnType", SqlDbType.SmallInt, intTxnType);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, "ALL");
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
                dbCommand = db.GetStoredProcCommand("JobbingSelectLatestUnPosted") as SqlCommand;
            }
            
            
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Job Order Transactions By Entry Date 
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datTxnDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datTxnDateTo);
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Job Order Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            } 
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datEffectiveDateTo);
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Job Order Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, string strTxnFrom, string strTxnTo)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            StkParam oStkParam = new StkParam();
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
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product);
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Job Order 
        public bool GetJobOrder(DataGeneral.PostStatus TransStatus)
        {
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHVIEWONLY";

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.All)
            {
                dbCommand = db.GetStoredProcCommand("JobbingSelectAllExcludeReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "YesOrNo", SqlDbType.VarChar, oGLParam.CheckParameter().Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCode = thisRow[0]["TransNo"].ToString();
                if (thisRow[0]["EffectiveDate"].ToString() == "" || thisRow[0]["EffectiveDate"].ToString() == null)
                {
                    datEffectiveDate = DateTime.MinValue;
                }
                else
                {
                    datEffectiveDate = DateTime.Parse(thisRow[0]["EffectiveDate"].ToString());
                }
                if (thisRow[0]["DateLimit"].ToString() == "" || thisRow[0]["DateLimit"].ToString() == null)
                {
                    datDateLimit = DateTime.MinValue;
                }
                else
                {
                    datDateLimit = DateTime.Parse(thisRow[0]["DateLimit"].ToString());
                }
                strCustNo = thisRow[0]["CustNo"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                intUnits = int.Parse(thisRow[0]["Units"].ToString());
                decUnitPrice = decimal.Parse(thisRow[0]["Price"].ToString());
                decPriceLimit = decimal.Parse(thisRow[0]["PriceLimit"].ToString());
                strInstructions = thisRow[0]["Instructions"].ToString();
                intTxnType = int.Parse(thisRow[0]["TxnType"].ToString());
                intAmtProc = int.Parse(thisRow[0]["AmtProc"].ToString());
                intBalance = int.Parse(thisRow[0]["Balance"].ToString());
                decCustBalance = decimal.Parse(thisRow[0]["CustBalance"].ToString());
                strBroker = thisRow[0]["Broker"].ToString();
                strCustNo_CD = thisRow[0]["CustNo_CD"].ToString();
                strPosted = thisRow[0]["Posted"].ToString();
                strReversed = thisRow[0]["Reversed"].ToString();
                if (thisRow[0]["TxnDate"].ToString() == "" || thisRow[0]["TxnDate"].ToString() == null)
                {
                    datTxnDate = DateTime.MinValue;
                }
                else
                {
                    datTxnDate = DateTime.Parse(thisRow[0]["TxnDate"].ToString());
                }
                strSubmitMedium = thisRow[0]["SubmitMedium"] != null ? thisRow[0]["SubmitMedium"].ToString() : "";
                strProduct = thisRow[0]["Product"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("JobbingChkTransNoExist") as SqlCommand;
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

        #region Get All Job Order Posted and Balance Not Zero
        public DataSet GetAllBalNotZero()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectAllPostedBalNotZero") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        //-----------------------------Jobbing Report-----------------------

        #region Process Jobbing Report
        public void ProcessJobReport()
        {
            DeleteJobReport();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlBulkCopy sbc = new SqlBulkCopy(connection);
                sbc.DestinationTableName = "Stkb_JobbingBookTxnReport";
                sbc.WriteToServer(GetAllBalNotZero().Tables[0]);
                sbc.Close();
            }
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "JOBAUTOCROSS";
            if (oGLParam.CheckParameter().Trim() == "YES") 
            {
                EmptyCrossCust();
                ProcessCrossDeal();
                ProcessUnEqualCrossDeal();
            }
        }
        #endregion

        #region Delete Job Order Report Table
        public void DeleteJobReport()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingReportDeleteAll") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Empty Cross Customer In Job Order Report Table
        public void EmptyCrossCust()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingReportEmptyCrossCust") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Process Cross Deals
        public void ProcessCrossDeal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandSell = null;
            SqlCommand dbCommandBuy = db.GetStoredProcCommand("JobbingReportSelectAllBuy") as SqlCommand;
            DataSet oDSBuy = db.ExecuteDataSet(dbCommandBuy);
            foreach (DataRow oRowBuy in oDSBuy.Tables[0].Rows)
            {
                dbCommandSell = db.GetStoredProcCommand("JobbingReportSelectSellForBuy") as SqlCommand;
                db.AddInParameter(dbCommandSell, "StockCode", SqlDbType.VarChar, oRowBuy["StockCode"].ToString());
                db.AddInParameter(dbCommandSell, "Units", SqlDbType.BigInt, int.Parse(oRowBuy["Balance"].ToString()));
                db.AddInParameter(dbCommandSell, "PriceLimit", SqlDbType.Decimal, decimal.Parse(oRowBuy["PriceLimit"].ToString()));
                DataSet oDSSell = db.ExecuteDataSet(dbCommandSell);
                DataTable thisTableSell = oDSSell.Tables[0];
                DataRow[] thisRowSell = thisTableSell.Select();
                if (thisRowSell.Length >= 1)
                {
                    strCode = oRowBuy["TransNo"].ToString();
                    strCustNo_CD = thisRowSell[0]["CustNo"].ToString();
                    UpdateCrossCustNoWithTxnType();
                    strCode = thisRowSell[0]["TransNo"].ToString();
                    DeleteJobReportTransNo();
                }
            }
        }
        #endregion

        #region Process UnEqual Cross Deals
        public void ProcessUnEqualCrossDeal()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandSell = null;
            SqlCommand dbCommandBuy = db.GetStoredProcCommand("JobbingReportSelectAllBuy") as SqlCommand;
            DataSet oDSBuy = db.ExecuteDataSet(dbCommandBuy);
            foreach (DataRow oRowBuy in oDSBuy.Tables[0].Rows)
            {
                dbCommandSell = db.GetStoredProcCommand("JobbingReportSelectUnEqualSellForBuy") as SqlCommand;
                db.AddInParameter(dbCommandSell, "StockCode", SqlDbType.VarChar, oRowBuy["StockCode"].ToString());
                db.AddInParameter(dbCommandSell, "PriceLimit", SqlDbType.Decimal, decimal.Parse(oRowBuy["PriceLimit"].ToString()));
                DataSet oDSSell = db.ExecuteDataSet(dbCommandSell);
                foreach (DataRow oRowSell in oDSSell.Tables[0].Rows)
                {
                    if (int.Parse(oRowBuy["Balance"].ToString()) >= int.Parse(oRowSell["Balance"].ToString()))
                    {
                        strCode = oRowBuy["TransNo"].ToString();
                        datEffectiveDate = DateTime.ParseExact(oRowBuy["EffectiveDate"].ToString().Substring(0,10), "dd/MM/yyyy", format);
                        strCustNo = oRowBuy["CustNo"].ToString();
                        strStockCode = oRowBuy["StockCode"].ToString();
                        intUnits = int.Parse(oRowSell["Balance"].ToString());
                        decUnitPrice = Decimal.Parse(oRowBuy["Price"].ToString());
                        decPriceLimit = Decimal.Parse(oRowBuy["PriceLimit"].ToString());
                        if (datDateLimit != DateTime.MinValue)
                        {
                            datDateLimit = DateTime.ParseExact(oRowBuy["DateLimit"].ToString().Substring(0, 10), "dd/MM/yyyy", format);


                        }
                        else
                        {
                            datDateLimit = DateTime.MinValue;
                        }
                        strInstructions = oRowBuy["Instructions"].ToString() + " Selling " + oRowSell["Instructions"].ToString();
                        intAmtProc = int.Parse(oRowSell["AmtProc"].ToString());
                        intBalance = int.Parse(oRowSell["Balance"].ToString());
                        decCustBalance = 0;
                        strBroker = "";
                        strCustNo_CD = oRowSell["CustNo"].ToString();
                        intTxnType = 2;
                        
                        strPosted = oRowBuy["Posted"].ToString();
                        strReversed = oRowBuy["Reversed"].ToString();
                        strJB_ID = "0";
                        strInputFrom = "";
                        SaveJobbingReport();

                        strCode = oRowBuy["TransNo"].ToString();
                        intUnits = int.Parse(oRowSell["Balance"].ToString());
                        UpdateUnit();
                        strCode = oRowSell["TransNo"].ToString();
                        DeleteJobReportTransNo();
                    }
                }
            }
            ProcessUnEqualSellCrossDeal();
            DeleteZeroCrossDeal();
        }
        #endregion

        #region Process UnEqual Sell Cross Deals
        public void ProcessUnEqualSellCrossDeal()
        {
            BargainTrans oBargainTrans = new BargainTrans();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandBuy = null;
            SqlCommand dbCommandSell = db.GetStoredProcCommand("JobbingReportSelectAllSell") as SqlCommand;
            DataSet oDSSell = db.ExecuteDataSet(dbCommandSell);
            foreach (DataRow oRowSell in oDSSell.Tables[0].Rows)
            {
                dbCommandBuy = db.GetStoredProcCommand("JobbingReportSelectUnEqualBuyForSell") as SqlCommand;
                db.AddInParameter(dbCommandBuy, "StockCode", SqlDbType.VarChar, oRowSell["StockCode"].ToString());
                db.AddInParameter(dbCommandBuy, "PriceLimit", SqlDbType.Decimal, decimal.Parse(oRowSell["PriceLimit"].ToString()));
                DataSet oDSBuy = db.ExecuteDataSet(dbCommandBuy);
                foreach (DataRow oRowBuy in oDSBuy.Tables[0].Rows)
                {
                    if (int.Parse(oRowSell["Balance"].ToString()) > int.Parse(oRowBuy["Balance"].ToString()))
                    {
                        strCode = oRowBuy["TransNo"].ToString();
                        strCustNo_CD = oRowSell["CustNo"].ToString();
                        UpdateCrossCustNoWithTxnType();
                        strCode = oRowSell["TransNo"].ToString();
                        intUnits = int.Parse(oRowBuy["Balance"].ToString());
                        UpdateUnit();
                    }
                }
            }
        }
        #endregion

        #region Update Cross Customer No  With Txn Type
        public bool UpdateCrossCustNoWithTxnType()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingReportUpdateCrossCustNo") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "CrossCustNo", SqlDbType.VarChar, strCustNo_CD.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;

            return blnStatus;

        }
        #endregion

        #region Delete A Job Order Report Transaction
        public bool DeleteJobReportTransNo()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingReportDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Update Unit
        public bool UpdateUnit()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingReportUpdateUnit") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.BigInt, intUnits);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;

            return blnStatus;

        }
        #endregion

        #region Delete JobOrder Zero Balance
        public void DeleteZeroCrossDeal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommandBuy = db.GetStoredProcCommand("JobbingReportDeleteZeroUnitCrossDeal") as SqlCommand;
            db.ExecuteNonQuery(dbCommandBuy);

        }
        #endregion


        #region Save To Jobbing Report
        public void SaveJobbingReport()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("JobbingReportAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "JOBBOOKREPORT");
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            if (datDateLimit != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "DateLimit", SqlDbType.DateTime, datDateLimit);

            }
            else
            {
                db.AddInParameter(oCommand, "DateLimit", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(oCommand, "CustNo", SqlDbType.Char, strCustNo.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
            db.AddInParameter(oCommand, "Price", SqlDbType.Money, decUnitPrice);
            db.AddInParameter(oCommand, "PriceLimit", SqlDbType.Money, decPriceLimit);
            db.AddInParameter(oCommand, "Instructions", SqlDbType.NText, strInstructions.Trim());
            db.AddInParameter(oCommand, "TxnType", SqlDbType.SmallInt, intTxnType);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "InputFrom", SqlDbType.VarChar, strInputFrom.Trim());
            db.AddInParameter(oCommand, "AmtProc", SqlDbType.Int, intAmtProc);
            db.AddInParameter(oCommand, "Balance", SqlDbType.Int, intBalance);
            db.AddInParameter(oCommand, "CustBalance", SqlDbType.Money, decCustBalance);
            db.AddInParameter(oCommand, "Broker", SqlDbType.VarChar, strBroker.Trim());
            db.AddInParameter(oCommand, "CustNo_CD", SqlDbType.VarChar, strCustNo_CD.Trim());
            db.AddInParameter(oCommand, "JB_ID", SqlDbType.BigInt, Int64.Parse(strJB_ID.Trim()));
            db.AddInParameter(oCommand, "Posted", SqlDbType.VarChar, strPosted.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.VarChar, strReversed.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            db.AddInParameter(oCommand, "Product", SqlDbType.VarChar, oStkParam.Product.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        //------------------------End Of Jobbing Report-------------------------------

        #region Get Jobbing Transactions Effected By An Allotment Number
        public DataSet GetJobbingAllotmentEffect(string strAllotmentNo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingByAllotment") as SqlCommand;
            db.AddInParameter(dbCommand, "Allotment", SqlDbType.VarChar, strAllotmentNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
        
        #region Get Buy Jobbing Not Yet Executed Or Completed For A Customer and Stock Code
        public DataSet GetUnProcBuyGivenCustStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectUnProcByCustStockBuy") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Sell Jobbing Not Yet Executed Or Completed For A Customer and Stock Code
        public DataSet GetUnProcSellGivenCustStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectUnProcByCustStockSell") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Cross Jobbing Not Yet Executed Or Completed For A Customer and Stock Code
        public DataSet GetUnProcCrossGivenCustStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectUnProcByCustStockCross") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "CustNo_CD", SqlDbType.VarChar, strCustNo_CD.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Buy Jobbing Updated For A Customer and Stock Code
        public DataSet GetUpdatedBuyGivenCustStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectUpdatedByCustStockBuy") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Sell Jobbing Updated For A Customer and Stock Code
        public DataSet GetUpdatedSellGivenCustStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectUpdatedByCustStockSell") as SqlCommand;
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All UnProcessed Job Order Order By Jobbing No
        public DataSet GetAllUnProcessed()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByUnProcessed") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
        #region Get All UnProcessed Job Order By Date
        public DataSet GetAllUnProcessedByDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByUnProcessedWithDate") as SqlCommand;
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
        

        #region Update Amount Processed In Jobbing Book Addition
        public void UpdateProcessed(string strJobTransNo, int intUnitProc)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingUpdateProcessedAdd") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.BigInt, long.Parse(strJobTransNo));
            db.AddInParameter(oCommand, "Amount", SqlDbType.Int, intUnitProc);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Update Balance In Jobbing Book Minus
        public void UpdateBalance(string strJobTransNo,string strAllotmentNo, int intUnitProc)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingUpdateBalanceMinus") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.BigInt, long.Parse(strJobTransNo));
            db.AddInParameter(oCommand, "AllotmentNo", SqlDbType.VarChar, strAllotmentNo.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Int, intUnitProc);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Update Balance In Jobbing Book Zerorize
        public void UpdateBalance(string strJobTransNo,string strAllotmentNo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingUpdateBalanceZero") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.BigInt, long.Parse(strJobTransNo));
            db.AddInParameter(oCommand, "AllotmentNo", SqlDbType.VarChar, strAllotmentNo.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion
        
        #region Update Amount Processed In Jobbing Book Addition
        public SqlCommand UpdateProcessedCommand(string strJobTransNo, int intUnitProc)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingUpdateProcessedAdd") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar,strJobTransNo.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Int, intUnitProc);
            return oCommand;
        }
        #endregion

        #region Update Balance In Jobbing Book Minus
        public SqlCommand UpdateBalanceCommand(string strJobTransNo,string strAllotmentNo, int intUnitProc)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingUpdateBalanceMinus") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strJobTransNo.Trim());
            db.AddInParameter(oCommand, "AllotmentNo", SqlDbType.VarChar, strAllotmentNo.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Int, intUnitProc);
            return oCommand;
        }
        #endregion

        #region Update Balance In Jobbing Book Zerorize
        public SqlCommand UpdateBalanceCommand(string strJobTransNo, string strAllotmentNo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingUpdateBalanceZero") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strJobTransNo.Trim());
            db.AddInParameter(oCommand, "AllotmentNo", SqlDbType.VarChar, strAllotmentNo.Trim());
            return oCommand;
        }
        #endregion

        #region Update Balance Addition and Processed Minus In Jobbing Book 
        public SqlCommand UpdateBalanceProcessedCommand(string strJobTransNo, string strAllotmentNo, int intUnitProc)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingUpdateBalanceAddProcessedMinus") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strJobTransNo.Trim());
            db.AddInParameter(oCommand, "AllotmentNo", SqlDbType.VarChar, strAllotmentNo.Trim());
            db.AddInParameter(oCommand, "Amount", SqlDbType.Int, intUnitProc);

            return oCommand;
        }
        #endregion

        #region Get Jobbing Executed Completed Or Partially For A Customer,Stock,Txn Type Less or Equal To A Date
        public DataSet GetProcGivenCustStockTxnType()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectProcessedByCustStockTransType") as SqlCommand;
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "TxnType", SqlDbType.Int, intTxnType);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Post Job Order
        public bool Post()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingPost") as SqlCommand;
            db.AddInParameter(oCommand, "@Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "@EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "@Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            db.AddInParameter(oCommand, "@UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Post Job Order And Return A Command
        public SqlCommand PostCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingPost") as SqlCommand;
            db.AddInParameter(oCommand, "@Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "@EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "@Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            db.AddInParameter(oCommand, "@UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Reverse Posted Jobbing Order
        public void ReverseTrans()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingReverseTrans") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "@UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Reverse Posted Jobbing Order Return Command
        public SqlCommand ReverseTransCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingReverseTrans") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "@UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());

            return oCommand;
        }
        #endregion

        #region Check That Job Order For Cetificate Has Been Processed
        public bool ChkJobCertProcessed()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByCertTransIsProcessed") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strJB_ID.Trim());
            db.AddOutParameter(dbCommand, "IsProcessed", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "IsProcessed").ToString().Trim() == "1")
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

        #region Check That Job Order Transaction Has Been Processed
        public bool ChkJobOrderProcessed()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByJobOrderTransIsProcessed") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strCode.Trim());
            db.AddOutParameter(dbCommand, "IsProcessed", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "IsProcessed").ToString().Trim() == "1")
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

        #region Check That Job Order Is Cetificate For Sale
        public bool ChkJobCertForSale()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByCertForSale") as SqlCommand;
            db.AddInParameter(dbCommand, "Transno", SqlDbType.VarChar, strCode.Trim());
            db.AddOutParameter(dbCommand, "IsCert", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "IsCert").ToString().Trim() == "1")
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

        #region Check That Trade Exist In Jobbing Book - Buy
        public bool ChkTradeExistBuy(string strTradeCSCSNumber, string strTradeStock)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByCSCNoStockCodeBuy") as SqlCommand;
            db.AddInParameter(dbCommand, "CSCSNO", SqlDbType.VarChar, strTradeCSCSNumber.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strTradeStock.Trim());
            if (db.ExecuteDataSet(dbCommand).Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Trade Exist In Jobbing Book - Sell
        public bool ChkTradeExistSell(string strTradeCSCSNumber, string strTradeStock)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByCSCNoStockCodeSell") as SqlCommand;
            db.AddInParameter(dbCommand, "CSCSNO", SqlDbType.VarChar, strTradeCSCSNumber.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strTradeStock.Trim());
            if (db.ExecuteDataSet(dbCommand).Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Trade Exist In Jobbing Book With Equal Quantity - Buy
        public bool ChkTradeExistEqualBuy(string strTradeCSCSNumber, string strTradeStock, int intTradeQty)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByCSCNoStockCodeUnitBuy") as SqlCommand;
            db.AddInParameter(dbCommand, "CSCSNO", SqlDbType.VarChar, strTradeCSCSNumber.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strTradeStock.Trim());
            db.AddInParameter(dbCommand, "Units", SqlDbType.Int, intTradeQty);
            if (db.ExecuteDataSet(dbCommand).Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Trade Exist In Jobbing Book With Equal Quantity - Sell
        public bool ChkTradeExistEqualSell(string strTradeCSCSNumber, string strTradeStock, int intTradeQty)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectByCSCNoStockCodeUnitSell") as SqlCommand;
            db.AddInParameter(dbCommand, "CSCSNO", SqlDbType.VarChar, strTradeCSCSNumber.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strTradeStock.Trim());
            db.AddInParameter(dbCommand, "Units", SqlDbType.Int, intTradeQty);
            if (db.ExecuteDataSet(dbCommand).Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Delete Given Cert Number
        public SqlCommand DeleteCertNumberCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingDeleteCertNumber") as SqlCommand;
            db.AddInParameter(oCommand, "JB_ID", SqlDbType.VarChar, strJB_ID.Trim());
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "Units", SqlDbType.Int, intUnits);
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, GeneralFunc.UserBranchNumber.Trim());
            return oCommand;
        }
        #endregion

        #region Get TransNo Given Certificate Verification Number
        public long GetTransNoGivenJBID()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingSelectByJBIDReturnCode") as SqlCommand;
            db.AddInParameter(oCommand, "JB_ID", SqlDbType.BigInt, long.Parse(strJB_ID.Trim()));
            if (db.ExecuteScalar(oCommand) == null)
            {
                return 0;
            }
            else
            {
                return (long)db.ExecuteScalar(oCommand);
            }
        }
        #endregion

        #region Save Print Date
        public void SaveCustBalDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CompanyUpdateCustBalDate") as SqlCommand;
            db.AddInParameter(oCommand, "@CustBalDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(oCommand, "@CustBalDateFrom", SqlDbType.DateTime, datEffectiveDate);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get Customer Outstanding Order Amount
        public decimal GetOutOrderAmount()
        {
            decimal decAmountResult = 0;
            //SaveQuotePriceForCustomer();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingSumCustomerOutOrderAmount") as SqlCommand;
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            if (db.ExecuteScalar(oCommand) != null && db.ExecuteScalar(oCommand).ToString() != "")
            {
                decAmountResult = decimal.Parse(db.ExecuteScalar(oCommand).ToString());
            }
            return decAmountResult;
        }
        #endregion

        #region Check Transaction Exist For Saved Transactions
        public bool ChkTransactionSavedExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingChkTransactionExistSaved") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "TxnType", SqlDbType.Int, intTxnType);
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "CustNo_CD", SqlDbType.VarChar, strCustNo_CD.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                strTransNoReturn = oDs.Tables[0].Rows[0]["TransNo"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Transaction Exist For Posted Transactions
        public bool ChkTransactionPostedExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingChkTransactionExistPosted") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "TxnType", SqlDbType.Int, intTxnType);
            db.AddInParameter(oCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "CustNo_CD", SqlDbType.VarChar, strCustNo_CD.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                strTransNoReturn = oDs.Tables[0].Rows[0]["TransNo"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Save Quote Price In Job Order Table For Customer
        public bool SaveQuotePriceForCustomer()
        {
            bool blnStatus = false;
            string strQuotesEmpty;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommandChkQuotesEmpty = db.GetStoredProcCommand("QuotesIsEmpty") as SqlCommand;
            db.AddOutParameter(oCommandChkQuotesEmpty, "IsEmpty", SqlDbType.Char, 1);
            db.ExecuteNonQuery(oCommandChkQuotesEmpty);
            strQuotesEmpty = db.GetParameterValue(oCommandChkQuotesEmpty, "IsEmpty").ToString();
            if (strQuotesEmpty.Trim() == "N")
            {
                SqlCommand dbCommandLastPrice = db.GetStoredProcCommand("QuotesMaxDate") as SqlCommand;
                DateTime datLastDate = DateTime.ParseExact((string)db.ExecuteScalar(dbCommandLastPrice), "dd/MM/yyyy", format);


                SqlCommand dbCommand = db.GetStoredProcCommand("JobbingUpdateCustomerWithLastPrice") as SqlCommand;
                db.AddInParameter(dbCommand, "CustNo", SqlDbType.Char, strCustNo.Trim());
                db.AddInParameter(dbCommand, "QuoteDate", SqlDbType.DateTime, datLastDate);

                try
                {
                    db.ExecuteNonQuery(dbCommand);
                    blnStatus = true;
                    return blnStatus;
                }
                catch (Exception e)
                {
                    string you = e.Message;
                    blnStatus = false;
                    return blnStatus;
                }
            }
            else
            {
                blnStatus = false;
                return blnStatus;
            }
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Get Job Order Due For Drop
        public DataSet GetOrderDueForDrop(int intDaysToDrop)
        {
            GeneralFunc oGeneralFunc = new GeneralFunc();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectDueForDrop") as SqlCommand;
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime,oGeneralFunc.AddBusinessDay(GeneralFunc.GetTodayDate(),-(intDaysToDrop),Holiday.GetAllReturnList()));
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Drop Job Order
        public void DropJobOrder(DataSet oDsJobOrdersToDrop)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand oCommand = null;
                    foreach (DataRow oRow in oDsJobOrdersToDrop.Tables[0].Rows)
                    {
                        oCommand = db.GetStoredProcCommand("JobbingDrop") as SqlCommand;
                        db.AddInParameter(oCommand, "@Code", SqlDbType.VarChar, oRow["TransNo"].ToString().Trim());
                        db.ExecuteNonQuery(oCommand);
                    }
                }
                catch(Exception err)
                {
                    throw new Exception(err.Message);
                }
            }
        }
        #endregion

        #region Get All Posted Given TransNo Date CustNo Stock
        public DataSet GetAllPostedGivenTransNoDateCustNoStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectAllPostedByCustStockTransType") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar,strCode.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "EffectiveDateTo", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Buy Posted Given TransNo Date CustNo Stock
        public DataSet GetBuyPostedGivenTransNoDateCustNoStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectBuyPostedByCustStockTransType") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "EffectiveDateTo", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Sell Posted Given TransNo Date CustNo Stock
        public DataSet GetSellPostedGivenTransNoDateCustNoStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectSellPostedByCustStockTransType") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "EffectiveDateTo", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Cross Posted Given TransNo Date CustNo Stock
        public DataSet GetCrossPostedGivenTransNoDateCustNoStock()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("JobbingSelectCrossPostedByCustStockTransType") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "EffectiveDateTo", SqlDbType.DateTime, datEffectiveDate);
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustNo.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Drop Order Return Command
        public SqlCommand DropOrderCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("JobbingDropOrder") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.AddInParameter(oCommand, "@UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());

            return oCommand;
        }
        #endregion

        #region Check KYC Documents
        public bool ChKKYCDocuments(string strCustomerNumber)
        {
            bool blnStatus = false;
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CHECKKYC";
            if (oGLParam.CheckParameter() == "YES")
            {
                Customer oCustomer = new Customer();
                oCustomer.CustAID = strCustomerNumber;
                oCustomer.GetCustomer();

                //CustomerDocumentTracking oCustomerDocumentTracking = new CustomerDocumentTracking();
                //oCustomerDocumentTracking.CustomerId = strCustomerNumber;
                //oCustomerDocumentTracking.DocumentFileName = "";
                //oCustomerDocumentTracking.GetCustomerDocumentByFileNameandCustomer();
                //if (oCustomerDocumentTracking.ExpiryDate < EffectiveDate)
                //{
                //    strKYCMissingMessage = "Cannot Job Order.Customer KYC Identification Date Has Expired";
                //    return blnStatus;
                //}
                DataSet oDS = oGLParam.GetMissingKYCDocuments(strCustomerNumber, oCustomer.ClientType);
                if (oDS.Tables[0].Rows.Count > 0)
                {
                    strKYCMissingMessage = "";
                    strKYCMissingMessage = "Cannot Job Order.Customer KYC Documents Incomplete";
                    strKYCMissingMessage = strKYCMissingMessage + "\r\n" +
                    "--------Missing KYC Documents---------";

                    foreach (DataRow oRowKYC in oDS.Tables[0].Rows)
                    {
                        strKYCMissingMessage = strKYCMissingMessage + "\r\n" + "\t" +
                            oRowKYC["Description"].ToString().Trim();
                    }
                }
                else
                {
                    blnStatus = true;
                }
            }
            else
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Check KYC Document Is Complete
        public bool ChkKYCDocumentIsComplete(string strCustomerNumber)
        {
            bool blnStatus = false;
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "CHECKKYC";
            if (oGLParam.CheckParameter() == "YES")
            {
                Customer oCustomer = new Customer();
                oCustomer.CustAID = strCustomerNumber;
                oCustomer.GetCustomer();
                //CustomerDocumentTracking oCustomerDocumentTracking = new CustomerDocumentTracking();
                //oCustomerDocumentTracking.CustomerId = strCustomerNumber;
                ////Do Later You look trough all document s for datea
                //oCustomerDocumentTracking.DocumentFileName = "";
                //oCustomerDocumentTracking.GetCustomerDocumentByFileNameandCustomer();
                //if (oCustomerDocumentTracking.ExpiryDate < datEffectiveDate)
                //{
                //    strKYCMissingMessage = "";
                //    strKYCMissingMessage = "Cannot Job Order.Customer KYC Identification Document Has Expired";
                //    return blnStatus;
                //}
                //if (oCustomerDocumentTracking.UtilityBillDate.AddMonths(6) < datEffectiveDate)
                //{
                //    strKYCMissingMessage = "";
                //    strKYCMissingMessage = "Cannot Job Order.Customer KYC Utility Bill Document Transaction Date Is Over Six Months";
                //    return blnStatus;
                //}
                if (oCustomer.Photo == null || oCustomer.Signature == null || oCustomer.Photo.Length == 0 || oCustomer.Signature.Length == 0)
                {
                    strKYCMissingMessage = "";
                    strKYCMissingMessage = "Cannot Job Order.Customer Picture Or Signature Is Missing";
                    return blnStatus;
                }
                
                DataSet oDS = oGLParam.GetMissingKYCDocuments(strCustomerNumber, oCustomer.ClientType);
                if (oDS.Tables[0].Rows.Count > 0)
                {
                    strKYCMissingMessage = "";
                    strKYCMissingMessage = "Cannot Job Order.Customer KYC Documents Incomplete";
                    strKYCMissingMessage = strKYCMissingMessage + "\r\n" + 
                    "--------Missing KYC Documents---------";

                    foreach (DataRow oRowKYC in oDS.Tables[0].Rows)
                    {
                        strKYCMissingMessage = strKYCMissingMessage + "\r\n" + "\t" +
                            oRowKYC["Description"].ToString().Trim();
                    }
                }
                else
                {
                    blnStatus = true;
                }
            }
            else
            {
                blnStatus = true;
            }
            
            return blnStatus;
        }
        #endregion

        
    }
}

