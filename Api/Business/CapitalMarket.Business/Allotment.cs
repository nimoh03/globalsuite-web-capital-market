using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using System.Data.SqlTypes;
using Admin.Business;
using BaseUtility.Business;
using CustomerManagement.Business;
using GL.Business;

/// <summary>
/// Summary description for Allotment
/// </summary>
namespace CapitalMarket.Business
{
    public class Allotment
    {
        #region Declaration
        private string strTxn, strTransNoRev, strCustAID, strStockCode;
        private string strOldAllotNo;
        private DateTime datDateAlloted, datDateAllotedTo;
        private Int64 intQty;
        private int intNumberOfTrans;
        private decimal decUnitPrice;
        private string strCommissionType;
        private decimal decSecFee, decStampDuty, decCommission, decVAT, decNSEFee, decCSCSFee, decSMSAlertCSCS, decTotalAmount;
        private string strCrossType;
        private Boolean blnPosted, blnReversed;
        private string strUserId;
        private DateTime datTxnDate, datTxnDateTo, datTxnTime;
        private char charCdeal;
        private char charAutopost;
        private string strMarginCode, strCustodianCode;
        private string strTicketNO, strSoldBy, strBoughtBy, strCDSellTrans;
        private string strOtherCust;
        private char charBuy_sold_Ind;
        private char charPrintFlag;
        private decimal decConsideration;
        private decimal decSecVat, decNseVat, decCscsVat, decSMSAlertCSCSVAT, decDeductComm;
        private string strSaveType;
        public string ProductCode { get; set; }
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
        public string CustAID
        {
            set { strCustAID = value; }
            get { return strCustAID; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public string OldAllotNo
        {
            set { strOldAllotNo = value; }
            get { return strOldAllotNo; }
        }

        public DateTime DateAlloted
        {
            set { datDateAlloted = value; }
            get { return datDateAlloted; }
        }
        public DateTime DateAllotedTo
        {
            set { datDateAllotedTo = value; }
            get { return datDateAllotedTo; }
        }
        public Int64 Qty
        {
            set { intQty = value; }
            get { return intQty; }
        }
        public int NumberOfTrans
        {
            set { intNumberOfTrans = value; }
            get { return intNumberOfTrans; }
        }
        public decimal UnitPrice
        {
            set { decUnitPrice = value; }
            get { return decUnitPrice; }
        }

        public string CommissionType
        {
            set { strCommissionType = value; }
            get { return strCommissionType; }
        }

        public decimal Consideration
        {
            set { decConsideration = value; }
            get { return decConsideration; }
        }
        public decimal SecFee
        {
            set { decSecFee = value; }
            get { return decSecFee; }
        }
        public decimal StampDuty
        {
            set { decStampDuty = value; }
            get { return decStampDuty; }
        }
        public decimal Commission
        {
            set { decCommission = value; }
            get { return decCommission; }
        }
        public decimal VAT
        {
            set { decVAT = value; }
            get { return decVAT; }
        }
        public decimal NSEFee
        {
            set { decNSEFee = value; }
            get { return decNSEFee; }
        }
        public decimal CSCSFee
        {
            set { decCSCSFee = value; }
            get { return decCSCSFee; }
        }
        public decimal SMSAlertCSCS
        {
            set { decSMSAlertCSCS = value; }
            get { return decSMSAlertCSCS; }
        }
        public decimal TotalAmount
        {
            set { decTotalAmount = value; }
            get { return decTotalAmount; }
        }

        public string CrossType
        {
            set { strCrossType = value; }
            get { return strCrossType; }
        }
        public Boolean Posted
        {
            set { blnPosted = value; }
            get { return blnPosted; }
        }
        public Boolean Reversed
        {
            set { blnReversed = value; }
            get { return blnReversed; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
        }
        public DateTime TxnDateTo
        {
            set { datTxnDateTo = value; }
            get { return datTxnDateTo; }
        }
        public DateTime TxnTime
        {
            set { datTxnTime = value; }
            get { return datTxnTime; }
        }

        public char Cdeal
        {
            set { charCdeal = value; }
            get { return charCdeal; }
        }
        public char Autopost
        {
            set { charAutopost = value; }
            get { return charAutopost; }
        }
        public string MarginCode
        {
            set { strMarginCode = value; }
            get { return strMarginCode; }
        }
        public string CustodianCode
        {
            set { strCustodianCode = value; }
            get { return strCustodianCode; }
        }
        public string TicketNO
        {
            set { strTicketNO = value; }
            get { return strTicketNO; }
        }
        public string SoldBy
        {
            set { strSoldBy = value; }
            get { return strSoldBy; }
        }
        public string BoughtBy
        {
            set { strBoughtBy = value; }
            get { return strBoughtBy; }
        }
        public string CDSellTrans
        {
            set { strCDSellTrans = value; }
            get { return strCDSellTrans; }
        }
        public string OtherCust
        {
            set { strOtherCust = value; }
            get { return strOtherCust; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public char Buy_sold_Ind
        {
            set { charBuy_sold_Ind = value; }
            get { return charBuy_sold_Ind; }
        }
        public char PrintFlag
        {
            set { charPrintFlag = value; }
            get { return charPrintFlag; }
        }
        public decimal SecVat
        {
            set { decSecVat = value; }
            get { return decSecVat; }
        }
        public decimal NseVat
        {
            set { decNseVat = value; }
            get { return decNseVat; }
        }
        public decimal CscsVat
        {
            set { decCscsVat = value; }
            get { return decCscsVat; }
        }
        public decimal SMSAlertCSCSVAT
        {
            set { decSMSAlertCSCSVAT = value; }
            get { return decSMSAlertCSCSVAT; }
        }
        public decimal DeductComm
        {
            set { decDeductComm = value; }
            get { return decDeductComm; }
        }
        #endregion

        public Allotment()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            StkParam oStkParam = new StkParam();
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
                    if (strSaveType.Trim() == "ADDS")
                    {
                        dbCommand = db.GetStoredProcCommand("AllotmentAddNew") as SqlCommand;
                        db.AddOutParameter(dbCommand, "Txn", SqlDbType.VarChar, 8);
                        db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "SOLD");
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("AllotmentEdit") as SqlCommand;
                        db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
                    }
                    db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,
                        ProductCode != null && ProductCode.Trim() != "" ? ProductCode : oStkParam.Product);
                    db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
                    db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAID.Trim());
                    db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
                    db.AddInParameter(dbCommand, "Qty", SqlDbType.BigInt, intQty);
                    db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
                    db.AddInParameter(dbCommand, "CommissionType", SqlDbType.VarChar, strCommissionType.Trim());
                    db.AddInParameter(dbCommand, "Consideration", SqlDbType.Decimal, decConsideration);
                    db.AddInParameter(dbCommand, "SecFee", SqlDbType.Decimal, decSecFee);
                    db.AddInParameter(dbCommand, "StampDuty", SqlDbType.Decimal, decStampDuty);
                    db.AddInParameter(dbCommand, "Commission", SqlDbType.Decimal, decCommission);
                    db.AddInParameter(dbCommand, "VAT", SqlDbType.Decimal, decVAT);
                    db.AddInParameter(dbCommand, "NSEFee", SqlDbType.Decimal, decNSEFee);
                    db.AddInParameter(dbCommand, "CSCSFee", SqlDbType.Decimal, decCSCSFee);
                    db.AddInParameter(dbCommand, "TotalAmt", SqlDbType.Decimal, decTotalAmount);
                    db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar, strCrossType.Trim());
                    db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
                    db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
                    db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(dbCommand, "Cdeal", SqlDbType.Char, charCdeal);
                    db.AddInParameter(dbCommand, "AutoPost", SqlDbType.Char, charAutopost);
                    db.AddInParameter(dbCommand, "TicketNo", SqlDbType.VarChar, strTicketNO.Trim());
                    db.AddInParameter(dbCommand, "SoldBy", SqlDbType.VarChar, strSoldBy.Trim());
                    db.AddInParameter(dbCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
                    db.AddInParameter(dbCommand, "Buy_Sold_Ind", SqlDbType.Char, charBuy_sold_Ind);
                    db.AddInParameter(dbCommand, "CDSellTrans", SqlDbType.VarChar, strCDSellTrans.Trim());
                    db.AddInParameter(dbCommand, "OtherCust", SqlDbType.VarChar, strOtherCust.Trim());
                    db.AddInParameter(dbCommand, "SecVat", SqlDbType.Decimal, decSecVat);
                    db.AddInParameter(dbCommand, "NseVat", SqlDbType.Decimal, decNseVat);
                    db.AddInParameter(dbCommand, "CscsVat", SqlDbType.Decimal, decCscsVat);
                    db.AddInParameter(dbCommand, "MarginCode", SqlDbType.VarChar, strMarginCode.Trim());
                    db.AddInParameter(dbCommand, "PrintFlag", SqlDbType.Char, charPrintFlag);
                    db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode);
                    db.AddInParameter(dbCommand, "SMSAlertCSCS", SqlDbType.Money, decSMSAlertCSCS);
                    db.AddInParameter(dbCommand, "NumberOfTrans", SqlDbType.Int, intNumberOfTrans);
                    db.AddInParameter(dbCommand, "SMSAlertCSCSVAT", SqlDbType.Decimal, decSMSAlertCSCSVAT);
                    Customer oCustomer = new Customer();
                    oCustomer.CustAID = strCustAID;
                    db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, oCustomer.GetBranchId());
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

        #region Add Allotment And Return The Command
        public SqlCommand AddCommand()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,
                        ProductCode != null && ProductCode.Trim() != "" ? ProductCode : oStkParam.Product);
            db.AddOutParameter(dbCommand, "Txn", SqlDbType.VarChar, 8);
            db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Qty", SqlDbType.BigInt, intQty);
            db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
            db.AddInParameter(dbCommand, "CommissionType", SqlDbType.VarChar, strCommissionType.Trim());
            db.AddInParameter(dbCommand, "Consideration", SqlDbType.Decimal, decConsideration);
            db.AddInParameter(dbCommand, "SecFee", SqlDbType.Decimal, decSecFee);
            db.AddInParameter(dbCommand, "StampDuty", SqlDbType.Decimal, decStampDuty);
            db.AddInParameter(dbCommand, "Commission", SqlDbType.Decimal, decCommission);
            db.AddInParameter(dbCommand, "VAT", SqlDbType.Decimal, decVAT);
            db.AddInParameter(dbCommand, "NSEFee", SqlDbType.Decimal, decNSEFee);
            db.AddInParameter(dbCommand, "CSCSFee", SqlDbType.Decimal, decCSCSFee);
            db.AddInParameter(dbCommand, "TotalAmt", SqlDbType.Decimal, decTotalAmount);
            db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar, strCrossType.Trim());
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(dbCommand, "Cdeal", SqlDbType.Char, charCdeal);
            db.AddInParameter(dbCommand, "AutoPost", SqlDbType.Char, charAutopost);
            db.AddInParameter(dbCommand, "TicketNo", SqlDbType.VarChar, strTicketNO.Trim());
            db.AddInParameter(dbCommand, "SoldBy", SqlDbType.VarChar, strSoldBy.Trim());
            db.AddInParameter(dbCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
            db.AddInParameter(dbCommand, "Buy_Sold_Ind", SqlDbType.Char, charBuy_sold_Ind);
            db.AddInParameter(dbCommand, "CDSellTrans", SqlDbType.VarChar, strCDSellTrans.Trim());
            db.AddInParameter(dbCommand, "OtherCust", SqlDbType.VarChar, strOtherCust.Trim());
            db.AddInParameter(dbCommand, "SecVat", SqlDbType.Decimal, decSecVat);
            db.AddInParameter(dbCommand, "NseVat", SqlDbType.Decimal, decNseVat);
            db.AddInParameter(dbCommand, "CscsVat", SqlDbType.Decimal, decCscsVat);
            db.AddInParameter(dbCommand, "MarginCode", SqlDbType.VarChar, strMarginCode.Trim());
            db.AddInParameter(dbCommand, "PrintFlag", SqlDbType.Char, charPrintFlag);
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode);
            db.AddInParameter(dbCommand, "SMSAlertCSCS", SqlDbType.Money, decSMSAlertCSCS);
            db.AddInParameter(dbCommand, "NumberOfTrans", SqlDbType.Int, intNumberOfTrans);
            db.AddInParameter(dbCommand, "SMSAlertCSCSVAT", SqlDbType.Decimal, decSMSAlertCSCSVAT);
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "SOLD");
            Customer oCustomer = new Customer();
            oCustomer.CustAID = strCustAID;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, oCustomer.GetBranchId());
            return dbCommand;
        }
        #endregion

        #region Add Allotment And Return The Command FIX
        public SqlCommand AddCommandFIX(string strUserId)
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,
                        ProductCode != null && ProductCode.Trim() != "" ? ProductCode : oStkParam.Product);
            db.AddOutParameter(dbCommand, "Txn", SqlDbType.VarChar, 8);
            db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Qty", SqlDbType.BigInt, intQty);
            db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
            db.AddInParameter(dbCommand, "CommissionType", SqlDbType.VarChar, strCommissionType.Trim());
            db.AddInParameter(dbCommand, "Consideration", SqlDbType.Decimal, decConsideration);
            db.AddInParameter(dbCommand, "SecFee", SqlDbType.Decimal, decSecFee);
            db.AddInParameter(dbCommand, "StampDuty", SqlDbType.Decimal, decStampDuty);
            db.AddInParameter(dbCommand, "Commission", SqlDbType.Decimal, decCommission);
            db.AddInParameter(dbCommand, "VAT", SqlDbType.Decimal, decVAT);
            db.AddInParameter(dbCommand, "NSEFee", SqlDbType.Decimal, decNSEFee);
            db.AddInParameter(dbCommand, "CSCSFee", SqlDbType.Decimal, decCSCSFee);
            db.AddInParameter(dbCommand, "TotalAmt", SqlDbType.Decimal, decTotalAmount);
            db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar, strCrossType.Trim());
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, strUserId.Trim());
            db.AddInParameter(dbCommand, "Cdeal", SqlDbType.Char, charCdeal);
            db.AddInParameter(dbCommand, "AutoPost", SqlDbType.Char, charAutopost);
            db.AddInParameter(dbCommand, "TicketNo", SqlDbType.VarChar, strTicketNO.Trim());
            db.AddInParameter(dbCommand, "SoldBy", SqlDbType.VarChar, strSoldBy.Trim());
            db.AddInParameter(dbCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
            db.AddInParameter(dbCommand, "Buy_Sold_Ind", SqlDbType.Char, charBuy_sold_Ind);
            db.AddInParameter(dbCommand, "CDSellTrans", SqlDbType.VarChar, strCDSellTrans.Trim());
            db.AddInParameter(dbCommand, "OtherCust", SqlDbType.VarChar, strOtherCust.Trim());
            db.AddInParameter(dbCommand, "SecVat", SqlDbType.Decimal, decSecVat);
            db.AddInParameter(dbCommand, "NseVat", SqlDbType.Decimal, decNseVat);
            db.AddInParameter(dbCommand, "CscsVat", SqlDbType.Decimal, decCscsVat);
            db.AddInParameter(dbCommand, "MarginCode", SqlDbType.VarChar, strMarginCode.Trim());
            db.AddInParameter(dbCommand, "PrintFlag", SqlDbType.Char, charPrintFlag);
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode);
            db.AddInParameter(dbCommand, "SMSAlertCSCS", SqlDbType.Money, decSMSAlertCSCS);
            db.AddInParameter(dbCommand, "NumberOfTrans", SqlDbType.Int, intNumberOfTrans);
            db.AddInParameter(dbCommand, "SMSAlertCSCSVAT", SqlDbType.Decimal, decSMSAlertCSCSVAT);
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "SOLD");
            Customer oCustomer = new Customer();
            oCustomer.CustAID = strCustAID;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, oCustomer.GetBranchId());
            return dbCommand;
        }
        #endregion

        #region Edit Allotment And Return The Command
        public SqlCommand EditCommand()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentEdit") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,
                        ProductCode != null && ProductCode.Trim() != "" ? ProductCode : oStkParam.Product);
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Qty", SqlDbType.BigInt, intQty);
            db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
            db.AddInParameter(dbCommand, "CommissionType", SqlDbType.VarChar, strCommissionType.Trim());
            db.AddInParameter(dbCommand, "Consideration", SqlDbType.Decimal, decConsideration);
            db.AddInParameter(dbCommand, "SecFee", SqlDbType.Decimal, decSecFee);
            db.AddInParameter(dbCommand, "StampDuty", SqlDbType.Decimal, decStampDuty);
            db.AddInParameter(dbCommand, "Commission", SqlDbType.Decimal, decCommission);
            db.AddInParameter(dbCommand, "VAT", SqlDbType.Decimal, decVAT);
            db.AddInParameter(dbCommand, "NSEFee", SqlDbType.Decimal, decNSEFee);
            db.AddInParameter(dbCommand, "CSCSFee", SqlDbType.Decimal, decCSCSFee);
            db.AddInParameter(dbCommand, "TotalAmt", SqlDbType.Decimal, decTotalAmount);
            db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar, strCrossType.Trim());
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(dbCommand, "Cdeal", SqlDbType.Char, charCdeal);
            db.AddInParameter(dbCommand, "AutoPost", SqlDbType.Char, charAutopost);
            db.AddInParameter(dbCommand, "TicketNo", SqlDbType.VarChar, strTicketNO.Trim());
            db.AddInParameter(dbCommand, "SoldBy", SqlDbType.VarChar, strSoldBy.Trim());
            db.AddInParameter(dbCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
            db.AddInParameter(dbCommand, "Buy_Sold_Ind", SqlDbType.Char, charBuy_sold_Ind);
            db.AddInParameter(dbCommand, "CDSellTrans", SqlDbType.VarChar, strCDSellTrans.Trim());
            db.AddInParameter(dbCommand, "OtherCust", SqlDbType.VarChar, strOtherCust.Trim());
            db.AddInParameter(dbCommand, "SecVat", SqlDbType.Decimal, decSecVat);
            db.AddInParameter(dbCommand, "NseVat", SqlDbType.Decimal, decNseVat);
            db.AddInParameter(dbCommand, "CscsVat", SqlDbType.Decimal, decCscsVat);
            db.AddInParameter(dbCommand, "MarginCode", SqlDbType.VarChar, strMarginCode.Trim());
            db.AddInParameter(dbCommand, "PrintFlag", SqlDbType.Char, charPrintFlag);
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode);
            db.AddInParameter(dbCommand, "SMSAlertCSCS", SqlDbType.Decimal, decSMSAlertCSCS);
            db.AddInParameter(dbCommand, "NumberOfTrans", SqlDbType.Int, intNumberOfTrans);
            db.AddInParameter(dbCommand, "SMSAlertCSCSVAT", SqlDbType.Decimal, decSMSAlertCSCSVAT);
            Customer oCustomer = new Customer();
            oCustomer.CustAID = strCustAID;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, oCustomer.GetBranchId());
            return dbCommand;
        }
        #endregion

        #region Add Allotment And Return The Command
        public SqlCommand AddManualCommand()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentAddNewManual") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,
                        ProductCode != null && ProductCode.Trim() != "" ? ProductCode : oStkParam.Product);
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Qty", SqlDbType.BigInt, intQty);
            db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
            db.AddInParameter(dbCommand, "CommissionType", SqlDbType.VarChar, strCommissionType.Trim());
            db.AddInParameter(dbCommand, "Consideration", SqlDbType.Decimal, decConsideration);
            db.AddInParameter(dbCommand, "SecFee", SqlDbType.Decimal, decSecFee);
            db.AddInParameter(dbCommand, "StampDuty", SqlDbType.Decimal, decStampDuty);
            db.AddInParameter(dbCommand, "Commission", SqlDbType.Decimal, decCommission);
            db.AddInParameter(dbCommand, "VAT", SqlDbType.Decimal, decVAT);
            db.AddInParameter(dbCommand, "NSEFee", SqlDbType.Decimal, decNSEFee);
            db.AddInParameter(dbCommand, "CSCSFee", SqlDbType.Decimal, decCSCSFee);
            db.AddInParameter(dbCommand, "TotalAmt", SqlDbType.Decimal, decTotalAmount);
            db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar, strCrossType.Trim());
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, strUserId.Trim());
            db.AddInParameter(dbCommand, "Cdeal", SqlDbType.Char, charCdeal);
            db.AddInParameter(dbCommand, "AutoPost", SqlDbType.Char, charAutopost);
            db.AddInParameter(dbCommand, "TicketNo", SqlDbType.VarChar, strTicketNO.Trim());
            db.AddInParameter(dbCommand, "SoldBy", SqlDbType.VarChar, strSoldBy.Trim());
            db.AddInParameter(dbCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
            db.AddInParameter(dbCommand, "Buy_Sold_Ind", SqlDbType.Char, charBuy_sold_Ind);
            db.AddInParameter(dbCommand, "CDSellTrans", SqlDbType.VarChar, strCDSellTrans.Trim());
            db.AddInParameter(dbCommand, "OtherCust", SqlDbType.VarChar, strOtherCust.Trim());
            db.AddInParameter(dbCommand, "SecVat", SqlDbType.Decimal, decSecVat);
            db.AddInParameter(dbCommand, "NseVat", SqlDbType.Decimal, decNseVat);
            db.AddInParameter(dbCommand, "CscsVat", SqlDbType.Decimal, decCscsVat);
            db.AddInParameter(dbCommand, "MarginCode", SqlDbType.VarChar, strMarginCode.Trim());
            db.AddInParameter(dbCommand, "PrintFlag", SqlDbType.Char, charPrintFlag);
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode);
            db.AddInParameter(dbCommand, "SMSAlertCSCS", SqlDbType.Money, decSMSAlertCSCS);
            db.AddInParameter(dbCommand, "NumberOfTrans", SqlDbType.Int, intNumberOfTrans);
            db.AddInParameter(dbCommand, "SMSAlertCSCSVAT", SqlDbType.Decimal, decSMSAlertCSCSVAT);
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "SOLD");
            Customer oCustomer = new Customer();
            oCustomer.CustAID = strCustAID;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, oCustomer.GetBranchId());
            return dbCommand;
        }
        #endregion

        #region Save Cross Deals
        public DataGeneral.SaveStatus SaveCross(Allotment oAllotBuy, Allotment oAllotSale)
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            strTxn = oAllotBuy.strTxn;
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            string strAllotmentNo = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand dbCommandAllotBuy = null;
                    SqlCommand dbCommandAllotSale = null;
                    if (strSaveType.Trim() == "ADDS")
                    {
                        dbCommandAllotBuy = oAllotBuy.AddCommand();
                        db.ExecuteNonQuery(dbCommandAllotBuy, transaction);
                        strAllotmentNo = db.GetParameterValue(dbCommandAllotBuy, "Txn").ToString().Trim();
                        oAllotSale.CDSellTrans = strAllotmentNo;
                        dbCommandAllotSale = oAllotSale.AddCommand();
                        db.ExecuteNonQuery(dbCommandAllotSale, transaction);
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommandAllotBuy = oAllotBuy.EditCommand();
                        db.ExecuteNonQuery(dbCommandAllotBuy, transaction);
                        oAllotSale.CDSellTrans = oAllotBuy.Txn;
                        dbCommandAllotSale = oAllotSale.EditCommand();
                        db.ExecuteNonQuery(dbCommandAllotSale, transaction);
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
                    oCommand = db.GetStoredProcCommand("AllotmentChkTransNoExistUnPosted") as SqlCommand;
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

        #region Get All Allotment
        public DataSet GetAll(DataGeneral.PostStatus TransStatus, DataGeneral.AllotmentType AllotType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strTxn.Trim());
            if (datDateAlloted != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datDateAlloted);
            }
            else
            {
                db.AddInParameter(dbCommand, "Effdate", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Unit", SqlDbType.BigInt, intQty);
            db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
            db.AddInParameter(dbCommand, "TicketNo", SqlDbType.VarChar, strTicketNO.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Entry Date
        public DataSet GetAllGivenEntryDate(DataGeneral.PostStatus TransStatus, DataGeneral.AllotmentType AllotType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenEntryDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEntryDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenEntryDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEntryDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEntryDatePostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EntryDateFrom", SqlDbType.DateTime, datTxnDate);
            db.AddInParameter(dbCommand, "EntryDateTo", SqlDbType.DateTime, datTxnDateTo);
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Effective Date
        public DataSet GetAllGivenEffDate(DataGeneral.PostStatus TransStatus, DataGeneral.AllotmentType AllotType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenEffDateUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEffDatePosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEffDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenEffDateUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDatePostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenEffDatePostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Effective Date All
        public DataSet GetAllGivenEffDateAll(DataGeneral.PostStatus TransStatus, DataGeneral.AllotmentType AllotType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDateUnPostedAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDateUnPostedAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDatePostedAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDatePostedAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDateUnPostedAscAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDateUnPostedAscAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenEffDatePostedAscAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDatePostedAscAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenEffDateUnPostedAll") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenEffDatePostedAll") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Transaction No
        public DataSet GetAllGivenTxnNo(DataGeneral.PostStatus TransStatus, DataGeneral.AllotmentType AllotType, string strTxnFrom, string strTxnTo, string strCustomer)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenTxnNoUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenTxnNoPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPostedAsc && AllotType == DataGeneral.AllotmentType.All)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenTxnNoUnPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllBuyGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenTxnNoPostedAsc") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.PostedAsc && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectAllCrossGivenTxnNoPostedAsc") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TxnFrom", SqlDbType.VarChar, strTxnFrom.Trim());
            db.AddInParameter(dbCommand, "TxnTo", SqlDbType.VarChar, strTxnTo.Trim());
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get All Allotment Sale By Effective Date For Payment Requisition
        public DataSet GetAllPostedSaleForPayment()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSellGivenEffDatePostedForPayment") as SqlCommand;
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Customer
        public DataSet GetAllGivenCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenCustomerAsc") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Customer Posted
        public DataSet GetAllGivenCustomerPosted()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenCustomerAscPosted") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Customer Posted Buy
        public DataSet GetAllGivenCustomerPostedBuy()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenCustomerAscPostedBuy") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Customer Posted Sell
        public DataSet GetAllGivenCustomerPostedSell()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentSelectAllGivenCustomerAscPostedSell") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Posted Sale Allotment Transactions By Customer No Date
        public DataSet GetAllPostedSellGivenCustomerNoDate(string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentSelectAllPostedSellGivenCustomerNoDate") as SqlCommand;
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomerNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Allotment Transactions By Customer
        public DataSet GetAllInvestmentCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("GLSelectAllInvestmentCustomerAsc") as SqlCommand;
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Fix Allotment Distinct Date  
        public DataSet GetAllFixDistinctDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentSelectAllFixDistinctDatePosted") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Delete All Investment Portfolio
        public SqlCommand DeleteInvestmentPortfolioCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("PortfolioDeleteAllInvestmentCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            return dbCommand;
        }
        #endregion


        #region Get Allotment 
        public bool GetAllotment(DataGeneral.PostStatus TransStatus, DataGeneral.AllotmentType AllotType)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBuyUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectSellUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectCrossUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.UnPosted && AllotType == DataGeneral.AllotmentType.CrossSell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectCrossSellUnPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBuyPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectSellPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectCrossPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Posted && AllotType == DataGeneral.AllotmentType.CrossSell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectCrossSellPosted") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed && AllotType == DataGeneral.AllotmentType.Buy)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBuyReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed && AllotType == DataGeneral.AllotmentType.Sell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectSellReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed && AllotType == DataGeneral.AllotmentType.Cross)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectCrossReversed") as SqlCommand;
            }
            else if (TransStatus == DataGeneral.PostStatus.Reversed && AllotType == DataGeneral.AllotmentType.CrossSell)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectCrossSellReversed") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn);
            StkParam oStkParam = new StkParam();
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ProductCode = thisRow[0]["ProductCode"] != null && thisRow[0]["ProductCode"].ToString().Trim() != "" ? thisRow[0]["ProductCode"].ToString() : oStkParam.Product;
                strTxn = thisRow[0]["Txn#"].ToString();
                strCustAID = thisRow[0]["CustAID"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                datDateAlloted = DateTime.ParseExact(thisRow[0]["DateAlloted"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                intQty = long.Parse(thisRow[0]["Qty"].ToString());
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                strCommissionType = thisRow[0]["CommissionType"].ToString();
                decConsideration = decimal.Parse(thisRow[0]["Consideration"].ToString());
                decSecFee = decimal.Parse(thisRow[0]["SecFee"].ToString());
                decStampDuty = decimal.Parse(thisRow[0]["StampDuty"].ToString());
                decCommission = decimal.Parse(thisRow[0]["Commission"].ToString());
                decVAT = decimal.Parse(thisRow[0]["VAT"].ToString());
                decNSEFee = decimal.Parse(thisRow[0]["NSEFee"].ToString());
                decCSCSFee = decimal.Parse(thisRow[0]["CSCSFee"].ToString());
                decSMSAlertCSCS = thisRow[0]["SMSAlertCSCS"] != null &&
                             thisRow[0]["SMSAlertCSCS"].ToString().Trim() != "" ?
                             decimal.Parse(thisRow[0]["SMSAlertCSCS"].ToString()) : 0;
                decTotalAmount = decimal.Parse(thisRow[0]["TotalAmt"].ToString());
                intNumberOfTrans = thisRow[0]["NumberOfTrans"] != null &&
                             thisRow[0]["NumberOfTrans"].ToString().Trim() != "" ?
                             int.Parse(thisRow[0]["NumberOfTrans"].ToString()) : 0;
                strCrossType = thisRow[0]["CrossType"].ToString();
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                strUserId = thisRow[0]["UserID"].ToString();

                charCdeal = char.Parse(thisRow[0]["Cdeal"].ToString());
                charAutopost = char.Parse(thisRow[0]["Autopost"].ToString());
                charBuy_sold_Ind = char.Parse(thisRow[0]["Buy_sold_Ind"].ToString());
                strTicketNO = thisRow[0]["TicketNO"].ToString();
                strSoldBy = thisRow[0]["SoldBy"].ToString();
                strBoughtBy = thisRow[0]["BoughtBy"].ToString();
                decCscsVat = thisRow[0]["CscsVat"] != null &&
                             thisRow[0]["CscsVat"].ToString().Trim() != "" ?
                             decimal.Parse(thisRow[0]["CscsVat"].ToString()) : 0;
                decSMSAlertCSCSVAT = thisRow[0]["SMSAlertCSCSVAT"] != null &&
                                  thisRow[0]["SMSAlertCSCSVAT"].ToString().Trim() != "" ?
                                  decimal.Parse(thisRow[0]["SMSAlertCSCSVAT"].ToString()) : 0;

                decNseVat = thisRow[0]["NseVat"] != null &&
                            thisRow[0]["NseVat"].ToString().Trim() != "" ?
                            decimal.Parse(thisRow[0]["NseVat"].ToString()) : 0;
                decSecVat = thisRow[0]["SecVat"] != null &&
                            thisRow[0]["SecVat"].ToString().Trim() != "" ?
                           decimal.Parse(thisRow[0]["SecVat"].ToString()) : 0;

                if (thisRow[0]["PrintFlag"] != null && thisRow[0]["PrintFlag"].ToString().Trim() != "")
                {
                    charPrintFlag = char.Parse(thisRow[0]["PrintFlag"].ToString());
                }
                else
                {
                    charPrintFlag = char.MinValue;
                }
                strOtherCust = thisRow[0]["OtherCust"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get Allotment Any
        public bool GetAllotmentAny()
        {
            StkParam oStkParam = new StkParam();
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                ProductCode = thisRow[0]["ProductCode"] != null && thisRow[0]["ProductCode"].ToString().Trim() != "" ? thisRow[0]["ProductCode"].ToString() : oStkParam.Product;
                strTxn = thisRow[0]["Txn#"].ToString();
                strCustAID = thisRow[0]["CustAID"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                datDateAlloted = DateTime.ParseExact(thisRow[0]["DateAlloted"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                intQty = long.Parse(thisRow[0]["Qty"].ToString());
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                strCommissionType = thisRow[0]["CommissionType"].ToString();
                decConsideration = decimal.Parse(thisRow[0]["Consideration"].ToString());
                decSecFee = decimal.Parse(thisRow[0]["SecFee"].ToString());
                decStampDuty = decimal.Parse(thisRow[0]["StampDuty"].ToString());
                decCommission = decimal.Parse(thisRow[0]["Commission"].ToString());
                decVAT = decimal.Parse(thisRow[0]["VAT"].ToString());
                decNSEFee = decimal.Parse(thisRow[0]["NSEFee"].ToString());
                decCSCSFee = decimal.Parse(thisRow[0]["CSCSFee"].ToString());
                decSMSAlertCSCS = thisRow[0]["SMSAlertCSCS"] != null &&
                             thisRow[0]["SMSAlertCSCS"].ToString().Trim() != "" ?
                             decimal.Parse(thisRow[0]["SMSAlertCSCS"].ToString()) : 0;
                decTotalAmount = decimal.Parse(thisRow[0]["TotalAmt"].ToString());
                strCrossType = thisRow[0]["CrossType"].ToString();
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                strUserId = thisRow[0]["UserID"].ToString();

                charCdeal = char.Parse(thisRow[0]["Cdeal"].ToString());
                charAutopost = char.Parse(thisRow[0]["Autopost"].ToString());
                charBuy_sold_Ind = char.Parse(thisRow[0]["Buy_sold_Ind"].ToString());
                strTicketNO = thisRow[0]["TicketNO"].ToString();
                strSoldBy = thisRow[0]["SoldBy"].ToString();
                strBoughtBy = thisRow[0]["BoughtBy"].ToString();
                decCscsVat = thisRow[0]["CscsVat"] != null &&
                             thisRow[0]["CscsVat"].ToString().Trim() != "" ?
                             decimal.Parse(thisRow[0]["CscsVat"].ToString()) : 0;
                decSMSAlertCSCSVAT = thisRow[0]["SMSAlertCSCSVAT"] != null &&
                             thisRow[0]["SMSAlertCSCSVAT"].ToString().Trim() != "" ?
                             decimal.Parse(thisRow[0]["SMSAlertCSCSVAT"].ToString()) : 0;
                decNseVat = thisRow[0]["NseVat"] != null &&
                            thisRow[0]["NseVat"].ToString().Trim() != "" ?
                            decimal.Parse(thisRow[0]["NseVat"].ToString()) : 0;
                decSecVat = thisRow[0]["SecVat"] != null &&
                            thisRow[0]["SecVat"].ToString().Trim() != "" ?
                            decimal.Parse(thisRow[0]["SecVat"].ToString()) : 0;

                if (thisRow[0]["PrintFlag"] != null && thisRow[0]["PrintFlag"].ToString().Trim() != "")
                {
                    charPrintFlag = char.Parse(thisRow[0]["PrintFlag"].ToString());
                }
                else
                {
                    charPrintFlag = char.MinValue;
                }
                strOtherCust = thisRow[0]["OtherCust"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Check Agent Commission Exist
        public bool ChkAgentCommExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("AllotmentCheckAgentCommExist") as SqlCommand;
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(dbCommand, "Buy_sold_Ind", SqlDbType.Char, charBuy_sold_Ind);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if(oDS.Tables[0].Rows.Count == 0)
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Compute Purchase or Sale's VAT
        public decimal ComputeVAT(decimal allotConsideration, decimal allotComm, string allotTransType, decimal VatRate, decimal MinVatAmount, decimal CommissionRate,DateTime datDirectCashDate)
        {
            string strBranchBuySellCharge = "";
            string strChargeVATForCommPref = "";
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHBUYSELLCHARGESDIFFERENT";
            strBranchBuySellCharge = oGLParam.CheckParameter();

            oGLParam.Type = "CHARGEVATCOMMPREF";
            strChargeVATForCommPref = oGLParam.CheckParameter();

            Customer oCustomer = new Customer();
            Branch oBranch = new Branch();
            oCustomer.CustAID = strCustAID;
            string strCustomerBranch = oCustomer.GetBranchId();

            CustomerExtraInformation oCustomerExtraInformation = new CustomerExtraInformation();
            oCustomerExtraInformation.CustAID = strCustAID;
            oCustomerExtraInformation.WedAnniversaryDate = datDirectCashDate;
            bool blnDirectCash = oCustomerExtraInformation.DirectCashSettlement;

            if (strBranchBuySellCharge.Trim() == "YES" && strCustomerBranch.Trim() != "" && strCustomerBranch.Trim() != oBranch.DefaultBranch)
            {
                BranchBuySellCharge oBranchBuySellCharge = new BranchBuySellCharge();
                oBranchBuySellCharge.GetBranchBuySellCharges();
                if (allotTransType.Trim() == "B")
                {
                    if (allotComm * (oBranchBuySellCharge.CommVATBuy / 100) > MinVatAmount)
                    {
                        return (allotComm * (oBranchBuySellCharge.CommVATBuy / 100));
                    }
                    else
                    {
                        return MinVatAmount;
                    }
                }
                else
                {
                    if (allotComm * (oBranchBuySellCharge.CommVATSell / 100) > MinVatAmount)
                    {
                        return (allotComm * (oBranchBuySellCharge.CommVATSell / 100));
                    }
                    else
                    {
                        return MinVatAmount;
                    }
                }
            }
            else
            {
                if (strChargeVATForCommPref.Trim() == "YES" || blnDirectCash)
                {
                    if ((allotConsideration * (CommissionRate / 100)) * (VatRate / 100) > MinVatAmount)
                    {
                        return ((allotConsideration * (CommissionRate / 100)) * (VatRate / 100));
                    }
                    else
                    {
                        return MinVatAmount;
                    }
                }
                else
                {
                    if (allotComm * (VatRate / 100) > MinVatAmount)
                    {
                        return (allotComm * (VatRate / 100));
                    }
                    else
                    {
                        return MinVatAmount;
                    }
                }
            }
        }
        #endregion

        #region Compute Purchase or Sale's Commission
        public decimal ComputeComm(decimal allotConsideration, string allotTransType, decimal CommRate, decimal MinCommAmount)
        {
            string strBranchBuySellCharge = "";
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "BRANCHBUYSELLCHARGESDIFFERENT";
            strBranchBuySellCharge = oGLParam.CheckParameter();

            decimal decResultRate = 0;
            decimal decResult = 0;
            decimal decResultComm = 0;
            StkParam oStkParam = new StkParam();
            ProductAcct oProductAcct = new ProductAcct();
            Customer oCustomer = new Customer();
            oProductAcct.CustAID = strCustAID;
            oProductAcct.ProductCode = oStkParam.Product;
            oCustomer.CustAID = strCustAID;
            string strCustomerBranch = oCustomer.GetBranchId();
            Branch oBranch = new Branch();
            oBranch.TransNo = strCustomerBranch;
            bool blnIsJointHeadOffice = oBranch.GetIsJointHeadOffice();

            if (oProductAcct.GetCustomerByCustId())
            {

                if (oProductAcct.GetBoxLoadStatus())
                {
                    decResult = 0;
                }
                else
                {
                    if (allotTransType.Trim() == "B")
                    {
                        if (oProductAcct.Comm == null)
                        {
                            if (oProductAcct.Agent.Trim() == "" || oProductAcct.Agent == null)
                            {
                                if (strBranchBuySellCharge.Trim() == "YES" && strCustomerBranch.Trim() != "" && strCustomerBranch.Trim() != oBranch.DefaultBranch && (!blnIsJointHeadOffice))
                                {
                                    BranchBuySellCharge oBranchBuySellCharge = new BranchBuySellCharge();
                                    oBranchBuySellCharge.GetBranchBuySellCharges();
                                    decResultRate = oBranchBuySellCharge.CommissionBuy;
                                }
                                else
                                {
                                    decResultRate = CommRate;
                                }
                                if (oStkParam.CommType.Trim() == "G")
                                {
                                    decResultComm = ComputeGradComm(allotConsideration);
                                }
                                else
                                {
                                    decResultComm = allotConsideration * (decResultRate / 100);
                                }
                                if (decResultComm > MinCommAmount)
                                {
                                    decResult = decResultComm;
                                }
                                else
                                {
                                    decResult = MinCommAmount;
                                }
                            }
                            else
                            {
                                ProductAcct oCustAgent = new ProductAcct();
                                oCustAgent.CustAID = oProductAcct.Agent.Trim();
                                oCustAgent.ProductCode = oStkParam.Product;
                                if (oCustAgent.GetCustomerByCustId())
                                {
                                    if ((oCustAgent.Comm == null))
                                    {
                                        if (strBranchBuySellCharge.Trim() == "YES" && strCustomerBranch.Trim() != "" && strCustomerBranch.Trim() != oBranch.DefaultBranch && (!blnIsJointHeadOffice))
                                        {
                                            BranchBuySellCharge oBranchBuySellChargeAgent = new BranchBuySellCharge();
                                            oBranchBuySellChargeAgent.GetBranchBuySellCharges();
                                            decResultRate = oBranchBuySellChargeAgent.CommissionBuy;
                                        }
                                        else
                                        {
                                            decResultRate = CommRate;
                                        }
                                        if (oStkParam.CommType.Trim() == "G")
                                        {
                                            decResultComm = ComputeGradComm(allotConsideration);
                                        }
                                        else
                                        {
                                            decResultComm = allotConsideration * (decResultRate / 100);
                                        }
                                        if (decResultComm > MinCommAmount)
                                        {
                                            decResult = decResultComm;
                                        }
                                        else
                                        {
                                            decResult = MinCommAmount;
                                        }
                                    }
                                    else
                                    {
                                        if (oCustAgent.Comm == 0)
                                        {
                                            decResult = 0;
                                        }
                                        else
                                        {
                                            decResultRate = decimal.Parse(oCustAgent.Comm.ToString());
                                            if (allotConsideration * (decResultRate / 100) > MinCommAmount)
                                            {
                                                decResult = (allotConsideration * (decResultRate / 100));
                                            }
                                            else
                                            {
                                                decResult = MinCommAmount;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Customer's Agent Account Does Not Exist");
                                }
                            }
                        }
                        else
                        {
                            if (oProductAcct.Comm == 0)
                            {
                                decResult = 0;
                            }
                            else
                            {
                                decResultRate = decimal.Parse(oProductAcct.Comm.ToString());
                                if (allotConsideration * (decResultRate / 100) > MinCommAmount)
                                {
                                    decResult = (allotConsideration * (decResultRate / 100));
                                }
                                else
                                {
                                    decResult = MinCommAmount;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (oProductAcct.SellCommission == null)
                        {
                            if (oProductAcct.Agent.Trim() == "" || oProductAcct.Agent == null)
                            {
                                if (strBranchBuySellCharge.Trim() == "YES" && strCustomerBranch.Trim() != "" && strCustomerBranch.Trim() != oBranch.DefaultBranch && (!blnIsJointHeadOffice))
                                {
                                    BranchBuySellCharge oBranchBuySellChargeSell = new BranchBuySellCharge();
                                    oBranchBuySellChargeSell.GetBranchBuySellCharges();
                                    decResultRate = oBranchBuySellChargeSell.CommissionSell;
                                }
                                else
                                {
                                    decResultRate = CommRate;
                                }
                                if (oStkParam.CommType.Trim() == "G")
                                {
                                    decResultComm = ComputeGradComm(allotConsideration);

                                }
                                else
                                {
                                    decResultComm = allotConsideration * (decResultRate / 100);
                                }
                                if (decResultComm > MinCommAmount)
                                {
                                    decResult = decResultComm;
                                }
                                else
                                {
                                    decResult = MinCommAmount;
                                }

                            }
                            else
                            {
                                ProductAcct oCustAgent = new ProductAcct();
                                oCustAgent.CustAID = oProductAcct.Agent.Trim();
                                oCustAgent.ProductCode = oStkParam.Product;

                                if (oCustAgent.GetCustomerByCustId())
                                {
                                    if (oCustAgent.SellCommission == null)
                                    {
                                        if (strBranchBuySellCharge.Trim() == "YES" && strCustomerBranch.Trim() != "" && strCustomerBranch.Trim() != oBranch.DefaultBranch && (!blnIsJointHeadOffice))
                                        {
                                            BranchBuySellCharge oBranchBuySellChargeSellAgent = new BranchBuySellCharge();
                                            oBranchBuySellChargeSellAgent.GetBranchBuySellCharges();
                                            decResultRate = oBranchBuySellChargeSellAgent.CommissionSell;
                                        }
                                        else
                                        {
                                            decResultRate = CommRate;
                                        }
                                        if (oStkParam.CommType.Trim() == "G")
                                        {
                                            decResultComm = ComputeGradComm(allotConsideration);

                                        }
                                        else
                                        {
                                            decResultComm = allotConsideration * (decResultRate / 100);
                                        }
                                        if (decResultComm > MinCommAmount)
                                        {
                                            decResult = decResultComm;
                                        }
                                        else
                                        {
                                            decResult = MinCommAmount;
                                        }
                                    }
                                    else
                                    {
                                        if (oCustAgent.SellCommission == 0)
                                        {
                                            decResult = 0;
                                        }
                                        else
                                        {
                                            decResultRate = decimal.Parse(oCustAgent.SellCommission.ToString());
                                            if (allotConsideration * (decResultRate / 100) > MinCommAmount)
                                            {
                                                decResult = (allotConsideration * (decResultRate / 100));
                                            }
                                            else
                                            {
                                                decResult = MinCommAmount;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Customer's Agent Account Does Not Exist");
                                }
                            }
                        }
                        else
                        {
                            if (oProductAcct.SellCommission == 0)
                            {
                                decResult = 0;
                            }
                            else
                            {
                                decResultRate = decimal.Parse(oProductAcct.SellCommission.ToString());
                                if (allotConsideration * (decResultRate / 100) > MinCommAmount)
                                {
                                    decResult = (allotConsideration * (decResultRate / 100));
                                }
                                else
                                {
                                    decResult = MinCommAmount;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Customer Does Not Exist");
            }
            return decResult;
        }
        #endregion

        #region Compute Graduated Commission
        public decimal ComputeGradComm(decimal decGradConsideration)
        {
            decimal decGradResult = 0;
            GradComm oGradComm = new GradComm();
            DataSet oDsGrad = oGradComm.GetAll();
            if (oDsGrad.Tables[0].Rows.Count <= 0)
            {
                throw new Exception("Cannot Calculate Graduated Commisson, Graduated Table Is Empty");
            }

            int intRecNo = oGradComm.GetGradCommMaxTransNo();
            foreach (DataRow oGradRow in oDsGrad.Tables[0].Rows)
            {
                if (decGradConsideration == decimal.Parse(oGradRow["Amount"].ToString()))
                {
                    decGradResult = decGradResult + ((decimal.Parse(oGradRow["Amount"].ToString()) * decimal.Parse(oGradRow["Perc"].ToString())) / 100);
                    break;
                }
                else if (decGradConsideration > decimal.Parse(oGradRow["Amount"].ToString()))
                {
                    if (int.Parse(oGradRow["TransNo"].ToString()) == intRecNo)
                    {
                        decGradResult = decGradResult + ((decGradConsideration * decimal.Parse(oGradRow["Perc"].ToString())) / 100);
                        break;
                    }
                    else
                    {
                        decGradResult = decGradResult + ((decimal.Parse(oGradRow["Amount"].ToString()) * decimal.Parse(oGradRow["Perc"].ToString())) / 100);
                        decGradConsideration = decGradConsideration - decimal.Parse(oGradRow["Amount"].ToString());
                    }
                }
                else if (decGradConsideration < decimal.Parse(oGradRow["Amount"].ToString()))
                {
                    decGradResult = decGradResult + ((decGradConsideration * decimal.Parse(oGradRow["Perc"].ToString())) / 100);
                    break;
                }
            }
            return decGradResult;
        }
        #endregion

        #region Get All Diskette Allotment Posting for a Date
        public DataSet GetAllotmentByDateForUpload(DateTime PostDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByDateDiskPost") as SqlCommand;
            db.AddInParameter(dbCommand, "PostDate", SqlDbType.DateTime, PostDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Diskette Allotment Posting for a Date All
        public DataSet GetAllotmentByDateForUploadAll(DateTime PostDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByDateDiskPostAll") as SqlCommand;
            db.AddInParameter(dbCommand, "PostDate", SqlDbType.DateTime, PostDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Diskette Allotment Posting for a Date
        public List<Allotment> GetAllotmentByDateForUploadAndCustomerNo(DateTime PostDate, List<string> ListCustomerNo)
        {
            List<Allotment> oListOfAllotment = new List<Allotment>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            foreach (string strCustomerNumber in ListCustomerNo)
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectByDateDiskPostAndCustomerNo") as SqlCommand;
                db.AddInParameter(dbCommand, "PostDate", SqlDbType.DateTime, PostDate);
                db.AddInParameter(dbCommand, "CustAID", SqlDbType.DateTime, strCustomerNumber.Trim());
                var varReturn = db.ExecuteScalar(dbCommand);
                strTxn = varReturn != null && varReturn.ToString().Trim() != "" ? varReturn.ToString() : "";
                GetAllotmentAny();
                oListOfAllotment.Add(this);
            }
            return oListOfAllotment;
        }
        #endregion

        #region Get Net Purchase And Sale Diskette Allotment Posting for a Date And Branch
        public decimal GetNetPurchaseSaleAllotmentByDateAndBranchForUpload(DateTime PostDate, string strBranch)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectTotalPurchaseByDateBranchDiskPosted") as SqlCommand;
            db.AddInParameter(dbCommand, "PostDate", SqlDbType.DateTime, PostDate);
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch);
            var decTotalPurchase = db.ExecuteScalar(dbCommand);

            dbCommand = db.GetStoredProcCommand("AllotmentSelectTotalSaleByDateBranchDiskPosted") as SqlCommand;
            db.AddInParameter(dbCommand, "PostDate", SqlDbType.DateTime, PostDate);
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch);
            var decTotalSale = db.ExecuteScalar(dbCommand);

            return (decTotalSale != null && decTotalSale.ToString().Trim() != "" ? decimal.Parse(decTotalSale.ToString()) : 0) - (decTotalPurchase != null && decTotalPurchase.ToString().Trim() != "" ? decimal.Parse(decTotalPurchase.ToString()) : 0);
        }
        #endregion

        #region Get All Bank Margin Diskette Allotment Posting for a Date
        public DataSet GetAllotmentByDateForUploadBank(DateTime PostDate, string BankMarginCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByDateDiskPostBank") as SqlCommand;
            db.AddInParameter(dbCommand, "PostDate", SqlDbType.DateTime, PostDate);
            db.AddInParameter(dbCommand, "BankMarginCode", SqlDbType.VarChar, BankMarginCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Custodian Margin Diskette Allotment Posting for a Date
        public DataSet GetAllotmentByDateForUploadCustodian(DateTime PostDate, string CustodianCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByDateDiskPostCustodian") as SqlCommand;
            db.AddInParameter(dbCommand, "PostDate", SqlDbType.DateTime, PostDate);
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, CustodianCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internet Based Transaction
        public DataSet GetInternetBasedTransaction(string strStockVar, DateTime datFromDate, DateTime datToDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentInternetBasedTransaction") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockVar);
            db.AddInParameter(dbCommand, "FromDate", SqlDbType.DateTime, datFromDate);
            db.AddInParameter(dbCommand, "ToDate", SqlDbType.DateTime, datToDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Sale By Date Alloted Group By Customer
        public DataSet GetAllSaleByDateAllotedGroupByCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSaleGivenEffDateGroupByCustomerId") as SqlCommand;

            db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "DateAllotedTo", SqlDbType.DateTime, datDateAllotedTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Sale By Date Alloted Group By Customer One Date
        public DataSet GetAllSaleByDateAllotedGroupByCustomerOneDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectAllSaleGivenEffDateGroupByCustomerIdOneDate") as SqlCommand;
            db.AddInParameter(dbCommand, "DateAllotedTo", SqlDbType.DateTime, datDateAllotedTo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Update Receipt Purchase Balance
        public void UpdateReceiptPurchaseBalance(decimal decReceiptPurchaseBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentUpdateReceiptPurchaseBalance") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "ReceiptPurchaseBalance", SqlDbType.Money, decReceiptPurchaseBalance);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Delete Purchase and Sale Transaction Upload Selected By TransNo And Return SqlCommand
        public SqlCommand DeleteBuySellUploadByTransNoCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentDeleteBuySellUploadByTransNo") as SqlCommand;
            db.AddInParameter(oCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Purchase and Sale Transaction Upload FIX And Return SqlCommand
        public SqlCommand DeleteBuySellUploadFIXCommand(DateTime datPostDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentDeleteBuySellUploadFIX") as SqlCommand;
            db.AddInParameter(oCommand, "DateAlloted", SqlDbType.DateTime, datPostDate);
            return oCommand;
        }
        #endregion



        #region Delete Bank Margin Purchase and Sale Transaction Upload Selected By TransNo And Return SqlCommand
        public SqlCommand DeleteBuySellUploadByTransNoBankCommand(string BankMarginCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentDeleteBuySellUploadByTransNoBank") as SqlCommand;
            db.AddInParameter(oCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "BankMarginCode", SqlDbType.VarChar, BankMarginCode.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Custodian Purchase and Sale Transaction Upload Selected By TransNo And Return SqlCommand
        public SqlCommand DeleteBuySellUploadByTransNoCustodianCommand(string CustodianCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentDeleteBuySellUploadByTransNoCustodian") as SqlCommand;
            db.AddInParameter(oCommand, "Txn", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar, CustodianCode.Trim());
            return oCommand;
        }
        #endregion


        #region Get Txn Sell Side Of Cross Deal
        public string GetTxnSellSide(string strTxnBuySide)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectTxnSellSide", strTxnBuySide.Trim()) as SqlCommand;
            var varAllotNo = db.ExecuteScalar(dbCommand);
            return varAllotNo.ToString();
        }
        #endregion

        #region Get Total Amount For Purchase Given Consideration
        public decimal GetTotalAmountForPurchase()
        {
            StkParam oStkbPGenTable = new StkParam();
            if (oStkbPGenTable.GetStkbPGenTable())
            {

                decimal decSec, decStamp, decComm, decNse, decCommVAT, decSecVAT, decCsCs, decSMSAlert, decNseVAT, decCsCsVAT, decSMSAlertVAT;
                if (decConsideration * (oStkbPGenTable.Bsec / 100) > oStkbPGenTable.MinSecB)
                {
                    decSec = (decConsideration * (oStkbPGenTable.Bsec / 100));
                    decSec = decimal.Parse(decSec.ToString("n"));
                }
                else
                {
                    decSec = decimal.Parse(oStkbPGenTable.MinSecB.ToString("n"));
                }

                if (decConsideration * (oStkbPGenTable.Bstamp / 100) > oStkbPGenTable.MinStampB)
                {
                    decStamp = (decConsideration * (oStkbPGenTable.Bstamp / 100));
                    decStamp = decimal.Parse(decStamp.ToString("n"));
                }
                else
                {
                    decStamp = decimal.Parse(oStkbPGenTable.MinStampB.ToString("n"));
                }
                decComm = (ComputeComm(decConsideration, "B", oStkbPGenTable.Bcncomm, oStkbPGenTable.MinCommB));
                decComm = decimal.Parse(decComm.ToString("n"));
                decCommVAT = (ComputeVAT(decConsideration, decComm, "B", oStkbPGenTable.Bvat, oStkbPGenTable.MinVatB, oStkbPGenTable.Bcncomm,datDateAlloted));
                decCommVAT = decimal.Parse(decCommVAT.ToString("n"));

                if (decConsideration * (oStkbPGenTable.Bnse / 100) > oStkbPGenTable.MinNSceB)
                {
                    decNse = (decConsideration * (oStkbPGenTable.Bnse / 100));
                    decNse = decimal.Parse(decNse.ToString("n"));
                }
                else
                {
                    decNse = decimal.Parse(oStkbPGenTable.MinNSceB.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.BCsCs / 100) > oStkbPGenTable.MinCscsB)
                {
                    decCsCs = (decConsideration * (oStkbPGenTable.BCsCs / 100));
                    decCsCs = decimal.Parse(decCsCs.ToString("n"));
                }
                else
                {
                    decCsCs = decimal.Parse(oStkbPGenTable.MinCscsB.ToString("n"));
                }
                decSMSAlert = oStkbPGenTable.SMSAlertCSCSB;
                decSMSAlert = decimal.Parse(decSMSAlert.ToString("n"));

                if (decConsideration * (oStkbPGenTable.BsecVat / 100) > oStkbPGenTable.MinSecVatB)
                {
                    decSecVAT = (decSec * (oStkbPGenTable.BsecVat / 100));
                    decSecVAT = decimal.Parse(decSecVAT.ToString("n"));
                }
                else
                {
                    decSecVAT = decimal.Parse(oStkbPGenTable.MinSecVatB.ToString());
                }
                if (decConsideration * (oStkbPGenTable.BnseVat / 100) > oStkbPGenTable.MinNseVatB)
                {
                    decNseVAT = (decNse * (oStkbPGenTable.BnseVat / 100));
                    decNseVAT = decimal.Parse(decNseVAT.ToString("n"));
                }
                else
                {
                    decNseVAT = decimal.Parse(oStkbPGenTable.MinNseVatB.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.BcscsVat / 100) > oStkbPGenTable.MinCscsVATB)
                {
                    decCsCsVAT = (decCsCs * (oStkbPGenTable.BcscsVat / 100));
                    decCsCsVAT = decimal.Parse(decCsCsVAT.ToString("n"));
                }
                else
                {
                    decCsCsVAT = decimal.Parse(oStkbPGenTable.MinCscsVATB.ToString("n"));
                }
                decSMSAlertVAT = (decSMSAlert * (oStkbPGenTable.SMSAlertCSCSVATB / 100));
                decSMSAlertVAT = decimal.Parse(decSMSAlertVAT.ToString("n"));

                decTotalAmount = (decConsideration + decSec + decStamp
                   + decComm + decCommVAT + decCsCs + decSMSAlert + decNse
                   + decSecVAT + decNseVAT + decCsCsVAT + decSMSAlertVAT);
                decTotalAmount = decimal.Parse(decTotalAmount.ToString("n"));
            }
            else
            {
                decTotalAmount = 0;
            }
            return decTotalAmount;
        }
        #endregion

        #region Get Total Amount For Sale Given Consideration
        public decimal GetTotalAmountForSale()
        {
            StkParam oStkbPGenTable = new StkParam();
            if (oStkbPGenTable.GetStkbPGenTable())
            {

                decimal decSec, decStamp, decComm, decNse, decCommVAT, decSecVAT, decCsCs, decSMSAlert, decNseVAT, decCsCsVAT, decSMSAlertVAT;
                if (decConsideration * (oStkbPGenTable.Ssec / 100) > oStkbPGenTable.MinSecS)
                {
                    decSec = (decConsideration * (oStkbPGenTable.Ssec / 100));
                    decSec = decimal.Parse(decSec.ToString("n"));
                }
                else
                {
                    decSec = decimal.Parse(oStkbPGenTable.MinSecS.ToString("n"));
                }

                if (decConsideration * (oStkbPGenTable.Sstamp / 100) > oStkbPGenTable.MinStampS)
                {
                    decStamp = (decConsideration * (oStkbPGenTable.Sstamp / 100));
                    decStamp = decimal.Parse(decStamp.ToString("n"));
                }
                else
                {
                    decStamp = decimal.Parse(oStkbPGenTable.MinStampS.ToString("n"));
                }
                decComm = (ComputeComm(decConsideration, "S", oStkbPGenTable.Scncomm, oStkbPGenTable.MinCommS));
                decComm = decimal.Parse(decComm.ToString("n"));
                decCommVAT = (ComputeVAT(decConsideration, decComm, "S", oStkbPGenTable.Svat, oStkbPGenTable.MinVatS, oStkbPGenTable.Scncomm,datDateAlloted));
                decCommVAT = decimal.Parse(decCommVAT.ToString("n"));

                if (decConsideration * (oStkbPGenTable.Snse / 100) > oStkbPGenTable.MinSceS)
                {
                    decNse = (decConsideration * (oStkbPGenTable.Snse / 100));
                    decNse = decimal.Parse(decNse.ToString("n"));
                }
                else
                {
                    decNse = decimal.Parse(oStkbPGenTable.MinSceS.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.Scscs / 100) > oStkbPGenTable.MinCscsS)
                {
                    decCsCs = (decConsideration * (oStkbPGenTable.Scscs / 100));
                    decCsCs = decimal.Parse(decCsCs.ToString("n"));
                }
                else
                {
                    decCsCs = decimal.Parse(oStkbPGenTable.MinCscsS.ToString("n"));
                }
                decSMSAlert = oStkbPGenTable.SMSAlertCSCSS;
                decSMSAlert = decimal.Parse(decSMSAlert.ToString("n"));

                if (decConsideration * (oStkbPGenTable.SsecVat / 100) > oStkbPGenTable.MinSecVatS)
                {
                    decSecVAT = (decSec * (oStkbPGenTable.SsecVat / 100));
                    decSecVAT = decimal.Parse(decSecVAT.ToString("n"));
                }
                else
                {
                    decSecVAT = decimal.Parse(oStkbPGenTable.MinSecVatS.ToString());
                }
                if (decConsideration * (oStkbPGenTable.SnseVat / 100) > oStkbPGenTable.MinNseVatS)
                {
                    decNseVAT = (decNse * (oStkbPGenTable.SnseVat / 100));
                    decNseVAT = decimal.Parse(decNseVAT.ToString("n"));
                }
                else
                {
                    decNseVAT = decimal.Parse(oStkbPGenTable.MinNseVatS.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.ScscsVat / 100) > oStkbPGenTable.MinCscsVatS)
                {
                    decCsCsVAT = (decCsCs * (oStkbPGenTable.ScscsVat / 100));
                    decCsCsVAT = decimal.Parse(decCsCsVAT.ToString("n"));
                }
                else
                {
                    decCsCsVAT = decimal.Parse(oStkbPGenTable.MinCscsVatS.ToString("n"));
                }
                decSMSAlertVAT = (decSMSAlert * (oStkbPGenTable.SMSAlertCSCSVATS / 100));
                decSMSAlertVAT = decimal.Parse(decSMSAlertVAT.ToString("n"));

                decTotalAmount = (decConsideration - (decSec + decStamp
                   + decComm + decCommVAT + decCsCs + decSMSAlert + decNse
                   + decSecVAT + decNseVAT + decCsCsVAT + decSMSAlertVAT));
                decTotalAmount = decimal.Parse(decTotalAmount.ToString("n"));




            }
            else
            {
                decTotalAmount = 0;
            }
            return decTotalAmount;
        }
        #endregion

        #region Get Total Amount For Purchase Given Consideration Markup Price Included
        public decimal GetTotalAmountForPurchaseMarkupPrice()
        {
            StkParam oStkbPGenTable = new StkParam();
            if (charCdeal == 'N')
            {
                decConsideration = intQty * (decUnitPrice + (decUnitPrice * (oStkbPGenTable.GetOrderPriceMarkup() / 100)));
            }
            else
            {
                decConsideration = intQty * decUnitPrice;
            }
            if (oStkbPGenTable.GetStkbPGenTable())
            {
                decimal decSec, decStamp, decComm, decNse, decCommVAT, decSecVAT, decCsCs, decSMSAlert, decNseVAT, decCsCsVAT, decSMSAlertVAT;
                if (decConsideration * (oStkbPGenTable.Bsec / 100) > oStkbPGenTable.MinSecB)
                {
                    decSec = (decConsideration * (oStkbPGenTable.Bsec / 100));
                    decSec = decimal.Parse(decSec.ToString("n"));
                }
                else
                {
                    decSec = decimal.Parse(oStkbPGenTable.MinSecB.ToString("n"));
                }

                if (decConsideration * (oStkbPGenTable.Bstamp / 100) > oStkbPGenTable.MinStampB)
                {
                    decStamp = (decConsideration * (oStkbPGenTable.Bstamp / 100));
                    decStamp = decimal.Parse(decStamp.ToString("n"));
                }
                else
                {
                    decStamp = decimal.Parse(oStkbPGenTable.MinStampB.ToString("n"));
                }
                decComm = (ComputeComm(decConsideration, "B", oStkbPGenTable.Bcncomm, oStkbPGenTable.MinCommB));
                decComm = decimal.Parse(decComm.ToString("n"));
                decCommVAT = (ComputeVAT(decConsideration, decComm, "B", oStkbPGenTable.Bvat, oStkbPGenTable.MinVatB, oStkbPGenTable.Bcncomm,datDateAlloted));
                decCommVAT = decimal.Parse(decCommVAT.ToString("n"));

                if (decConsideration * (oStkbPGenTable.Bnse / 100) > oStkbPGenTable.MinNSceB)
                {
                    decNse = (decConsideration * (oStkbPGenTable.Bnse / 100));
                    decNse = decimal.Parse(decNse.ToString("n"));
                }
                else
                {
                    decNse = decimal.Parse(oStkbPGenTable.MinNSceB.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.BCsCs / 100) > oStkbPGenTable.MinCscsB)
                {
                    decCsCs = (decConsideration * (oStkbPGenTable.BCsCs / 100));
                    decCsCs = decimal.Parse(decCsCs.ToString("n"));
                }
                else
                {
                    decCsCs = decimal.Parse(oStkbPGenTable.MinCscsB.ToString("n"));
                }
                decSMSAlert = oStkbPGenTable.SMSAlertCSCSB;
                decSMSAlert = decimal.Parse(decSMSAlert.ToString("n"));

                if (decConsideration * (oStkbPGenTable.BsecVat / 100) > oStkbPGenTable.MinSecVatB)
                {
                    decSecVAT = (decSec * (oStkbPGenTable.BsecVat / 100));
                    decSecVAT = decimal.Parse(decSecVAT.ToString("n"));
                }
                else
                {
                    decSecVAT = decimal.Parse(oStkbPGenTable.MinSecVatB.ToString());
                }
                if (decConsideration * (oStkbPGenTable.BnseVat / 100) > oStkbPGenTable.MinNseVatB)
                {
                    decNseVAT = (decNse * (oStkbPGenTable.BnseVat / 100));
                    decNseVAT = decimal.Parse(decNseVAT.ToString("n"));
                }
                else
                {
                    decNseVAT = decimal.Parse(oStkbPGenTable.MinNseVatB.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.BcscsVat / 100) > oStkbPGenTable.MinCscsVATB)
                {
                    decCsCsVAT = (decCsCs * (oStkbPGenTable.BcscsVat / 100));
                    decCsCsVAT = decimal.Parse(decCsCsVAT.ToString("n"));
                }
                else
                {
                    decCsCsVAT = decimal.Parse(oStkbPGenTable.MinCscsVATB.ToString("n"));
                }
                decSMSAlertVAT = (decSMSAlert * (oStkbPGenTable.SMSAlertCSCSVATB / 100));
                decSMSAlertVAT = decimal.Parse(decSMSAlertVAT.ToString("n"));

                decTotalAmount = (decConsideration + decSec + decStamp
                   + decComm + decCommVAT + decCsCs + decSMSAlert + decNse
                   + decSecVAT + decNseVAT + decCsCsVAT + decSMSAlertVAT);
                decTotalAmount = decimal.Parse(decTotalAmount.ToString("n"));

            }
            else
            {
                decTotalAmount = 0;
            }
            return decTotalAmount;
        }
        #endregion

        #region Get Fee Amount For Purchase Given Consideration
        public decimal GetFeeAmountForPurchase()
        {
            StkParam oStkbPGenTable = new StkParam();
            if (oStkbPGenTable.GetStkbPGenTable())
            {

                decimal decSec, decStamp, decComm, decNse, decCommVAT, decSecVAT, decCsCs, decSMSAlert, decNseVAT, decCsCsVAT, decSMSAlertVAT;
                if (decConsideration * (oStkbPGenTable.Bsec / 100) > oStkbPGenTable.MinSecB)
                {
                    decSec = (decConsideration * (oStkbPGenTable.Bsec / 100));
                    decSec = decimal.Parse(decSec.ToString("n"));
                }
                else
                {
                    decSec = decimal.Parse(oStkbPGenTable.MinSecB.ToString("n"));
                }

                if (decConsideration * (oStkbPGenTable.Bstamp / 100) > oStkbPGenTable.MinStampB)
                {
                    decStamp = (decConsideration * (oStkbPGenTable.Bstamp / 100));
                    decStamp = decimal.Parse(decStamp.ToString("n"));
                }
                else
                {
                    decStamp = decimal.Parse(oStkbPGenTable.MinStampB.ToString("n"));
                }
                decComm = (ComputeComm(decConsideration, "B", oStkbPGenTable.Bcncomm, oStkbPGenTable.MinCommB));
                decComm = decimal.Parse(decComm.ToString("n"));
                decCommVAT = (ComputeVAT(decConsideration, decComm, "B", oStkbPGenTable.Bvat, oStkbPGenTable.MinVatB, oStkbPGenTable.Bcncomm,datDateAlloted));
                decCommVAT = decimal.Parse(decCommVAT.ToString("n"));

                if (decConsideration * (oStkbPGenTable.Bnse / 100) > oStkbPGenTable.MinNSceB)
                {
                    decNse = (decConsideration * (oStkbPGenTable.Bnse / 100));
                    decNse = decimal.Parse(decNse.ToString("n"));
                }
                else
                {
                    decNse = decimal.Parse(oStkbPGenTable.MinNSceB.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.BCsCs / 100) > oStkbPGenTable.MinCscsB)
                {
                    decCsCs = (decConsideration * (oStkbPGenTable.BCsCs / 100));
                    decCsCs = decimal.Parse(decCsCs.ToString("n"));
                }
                else
                {
                    decCsCs = decimal.Parse(oStkbPGenTable.MinCscsB.ToString("n"));
                }
                decSMSAlert = oStkbPGenTable.SMSAlertCSCSB;
                decSMSAlert = decimal.Parse(decSMSAlert.ToString("n"));

                if (decConsideration * (oStkbPGenTable.BsecVat / 100) > oStkbPGenTable.MinSecVatB)
                {
                    decSecVAT = (decSec * (oStkbPGenTable.BsecVat / 100));
                    decSecVAT = decimal.Parse(decSecVAT.ToString("n"));
                }
                else
                {
                    decSecVAT = decimal.Parse(oStkbPGenTable.MinSecVatB.ToString());
                }
                if (decConsideration * (oStkbPGenTable.BnseVat / 100) > oStkbPGenTable.MinNseVatB)
                {
                    decNseVAT = (decNse * (oStkbPGenTable.BnseVat / 100));
                    decNseVAT = decimal.Parse(decNseVAT.ToString("n"));
                }
                else
                {
                    decNseVAT = decimal.Parse(oStkbPGenTable.MinNseVatB.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.BcscsVat / 100) > oStkbPGenTable.MinCscsVATB)
                {
                    decCsCsVAT = (decCsCs * (oStkbPGenTable.BcscsVat / 100));
                    decCsCsVAT = decimal.Parse(decCsCsVAT.ToString("n"));
                }
                else
                {
                    decCsCsVAT = decimal.Parse(oStkbPGenTable.MinCscsVATB.ToString("n"));
                }
                decSMSAlertVAT = (decSMSAlert * (oStkbPGenTable.SMSAlertCSCSVATB / 100));
                decSMSAlertVAT = decimal.Parse(decSMSAlertVAT.ToString("n"));

                decTotalAmount = (decSec + decStamp
                   + decComm + decCommVAT + decCsCs + decSMSAlert + decNse
                   + decSecVAT + decNseVAT + decCsCsVAT + decSMSAlertVAT);
                decTotalAmount = decimal.Parse(decTotalAmount.ToString("n"));




            }
            else
            {
                decTotalAmount = 0;
            }
            return decTotalAmount;
        }
        #endregion

        #region Get Fee Amount For Sale Given Consideration
        public decimal GetFeeAmountForSale()
        {
            StkParam oStkbPGenTable = new StkParam();
            if (oStkbPGenTable.GetStkbPGenTable())
            {

                decimal decSec, decStamp, decComm, decNse, decCommVAT, decSecVAT, decCsCs, decSMSAlert, decNseVAT, decCsCsVAT, decSMSAlertVAT;
                if (decConsideration * (oStkbPGenTable.Ssec / 100) > oStkbPGenTable.MinSecS)
                {
                    decSec = (decConsideration * (oStkbPGenTable.Ssec / 100));
                    decSec = decimal.Parse(decSec.ToString("n"));
                }
                else
                {
                    decSec = decimal.Parse(oStkbPGenTable.MinSecS.ToString("n"));
                }

                if (decConsideration * (oStkbPGenTable.Sstamp / 100) > oStkbPGenTable.MinStampS)
                {
                    decStamp = (decConsideration * (oStkbPGenTable.Sstamp / 100));
                    decStamp = decimal.Parse(decStamp.ToString("n"));
                }
                else
                {
                    decStamp = decimal.Parse(oStkbPGenTable.MinStampS.ToString("n"));
                }
                decComm = (ComputeComm(decConsideration, "S", oStkbPGenTable.Scncomm, oStkbPGenTable.MinCommS));
                decComm = decimal.Parse(decComm.ToString("n"));
                decCommVAT = (ComputeVAT(decConsideration, decComm, "S", oStkbPGenTable.Svat, oStkbPGenTable.MinVatS, oStkbPGenTable.Scncomm,datDateAlloted));
                decCommVAT = decimal.Parse(decCommVAT.ToString("n"));

                if (decConsideration * (oStkbPGenTable.Snse / 100) > oStkbPGenTable.MinSceS)
                {
                    decNse = (decConsideration * (oStkbPGenTable.Snse / 100));
                    decNse = decimal.Parse(decNse.ToString("n"));
                }
                else
                {
                    decNse = decimal.Parse(oStkbPGenTable.MinSceS.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.Scscs / 100) > oStkbPGenTable.MinCscsS)
                {
                    decCsCs = (decConsideration * (oStkbPGenTable.Scscs / 100));
                    decCsCs = decimal.Parse(decCsCs.ToString("n"));
                }
                else
                {
                    decCsCs = decimal.Parse(oStkbPGenTable.MinCscsS.ToString("n"));
                }
                decSMSAlert = oStkbPGenTable.SMSAlertCSCSS;
                decSMSAlert = decimal.Parse(decSMSAlert.ToString("n"));

                if (decConsideration * (oStkbPGenTable.SsecVat / 100) > oStkbPGenTable.MinSecVatS)
                {
                    decSecVAT = (decSec * (oStkbPGenTable.SsecVat / 100));
                    decSecVAT = decimal.Parse(decSecVAT.ToString("n"));
                }
                else
                {
                    decSecVAT = decimal.Parse(oStkbPGenTable.MinSecVatS.ToString());
                }
                if (decConsideration * (oStkbPGenTable.SnseVat / 100) > oStkbPGenTable.MinNseVatS)
                {
                    decNseVAT = (decNse * (oStkbPGenTable.SnseVat / 100));
                    decNseVAT = decimal.Parse(decNseVAT.ToString("n"));
                }
                else
                {
                    decNseVAT = decimal.Parse(oStkbPGenTable.MinNseVatS.ToString("n"));
                }
                if (decConsideration * (oStkbPGenTable.ScscsVat / 100) > oStkbPGenTable.MinCscsVatS)
                {
                    decCsCsVAT = (decCsCs * (oStkbPGenTable.ScscsVat / 100));
                    decCsCsVAT = decimal.Parse(decCsCsVAT.ToString("n"));
                }
                else
                {
                    decCsCsVAT = decimal.Parse(oStkbPGenTable.MinCscsVatS.ToString("n"));
                }
                decSMSAlertVAT = (decSMSAlert * (oStkbPGenTable.SMSAlertCSCSVATS / 100));
                decSMSAlertVAT = decimal.Parse(decSMSAlertVAT.ToString("n"));

                decTotalAmount = (decSec + decStamp
                   + decComm + decCommVAT + decCsCs + decSMSAlert + decNse
                   + decSecVAT + decNseVAT + decCsCsVAT + decSMSAlertVAT);
                decTotalAmount = decimal.Parse(decTotalAmount.ToString("n"));




            }
            else
            {
                decTotalAmount = 0;
            }
            return decTotalAmount;
        }
        #endregion

        #region Get Purchase Quantity For A Given Total Amount
        public long GetPurchaseQuantityForTotalAmount()
        {
            long lngQuantityAmount = 0;
            if (decTotalAmount != 0 && decUnitPrice != 0)
            {
                StkParam oStkbPGenTable = new StkParam();
                if (oStkbPGenTable.GetStkbPGenTable())
                {
                    decimal decTotalPer;
                    decimal decSec, decStamp, decComm, decNse, decCsCs, decSMSAlert;
                    decimal decRealCommVAT, decRealSecVAT, decRealNseVAT, decRealCsCsVAT, decRealSMSAlertVAT;
                    decSec = oStkbPGenTable.Bsec;
                    decStamp = oStkbPGenTable.Bstamp;
                    decComm = oStkbPGenTable.Bcncomm;
                    decNse = oStkbPGenTable.Bnse;
                    decCsCs = oStkbPGenTable.BCsCs;
                    decSMSAlert = oStkbPGenTable.SMSAlertCSCSB;

                    decRealCommVAT = (oStkbPGenTable.Bvat / 100) * (decComm);
                    decRealSecVAT = (oStkbPGenTable.BsecVat / 100) * (decSec);
                    decRealNseVAT = (oStkbPGenTable.BnseVat / 100) * (decNse);
                    decRealCsCsVAT = (oStkbPGenTable.BcscsVat / 100) * (decCsCs);
                    decRealSMSAlertVAT = (oStkbPGenTable.SMSAlertCSCSVATB / 100) * (decSMSAlert);

                    decTotalPer = (100 + (decSec + decStamp + decComm + decCsCs + decNse
                       + decRealCommVAT + decRealSecVAT + decRealNseVAT + decRealCsCsVAT));

                    decimal decActualConsideration = (100 * decTotalAmount) / (Math.Round(decTotalPer, 2)) + (decSMSAlert + decRealSMSAlertVAT);
                    lngQuantityAmount = Convert.ToInt64(decActualConsideration / decUnitPrice);
                }
            }
            return lngQuantityAmount;
        }
        #endregion

        #region Get Sale Quantity For A Given Total Amount
        public long GetSaleQuantityForTotalAmount()
        {
            long lngQuantityAmount = 0;
            if (decTotalAmount != 0 && decUnitPrice != 0)
            {
                StkParam oStkbPGenTable = new StkParam();
                if (oStkbPGenTable.GetStkbPGenTable())
                {
                    decimal decTotalPer;
                    decimal decSec, decStamp, decComm, decNse, decCsCs, decSMSAlert;
                    decimal decRealCommVAT, decRealSecVAT, decRealNseVAT, decRealCsCsVAT, decRealSMSAlertVAT;
                    decSec = oStkbPGenTable.Ssec;
                    decStamp = oStkbPGenTable.Sstamp;
                    decComm = oStkbPGenTable.Scncomm;
                    decNse = oStkbPGenTable.Snse;
                    decCsCs = oStkbPGenTable.Scscs;
                    decSMSAlert = oStkbPGenTable.SMSAlertCSCSS;

                    decRealCommVAT = (oStkbPGenTable.Svat / 100) * (decComm);
                    decRealSecVAT = (oStkbPGenTable.SsecVat / 100) * (decSec);
                    decRealNseVAT = (oStkbPGenTable.SnseVat / 100) * (decNse);
                    decRealCsCsVAT = (oStkbPGenTable.ScscsVat / 100) * (decCsCs);
                    decRealSMSAlertVAT = (oStkbPGenTable.SMSAlertCSCSVATS / 100) * (decSMSAlert);

                    decTotalPer = (100 - (decSec + decStamp + decComm + decCsCs + decNse
                       + decRealCommVAT + decRealSecVAT + decRealNseVAT + decRealCsCsVAT));

                    decimal decActualConsideration = (100 * decTotalAmount) / (Math.Round(decTotalPer, 2)) - (decSMSAlert + decRealSMSAlertVAT);
                    lngQuantityAmount = Convert.ToInt64(decActualConsideration / decUnitPrice);
                }
            }
            return lngQuantityAmount;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion


        #region Delete Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            return oCommand;
        }
        #endregion

        #region Update Post and Reversal and Return A Command
        public SqlCommand UpDatePostRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentUpdatePostRev") as SqlCommand;

            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserId.Trim());
            return oCommand;
        }
        #endregion

        #region Update Only Reversal and Return A Command
        public SqlCommand UpDateRevCommand()
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentUpdateRev") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserId.Trim());
            return oCommand;
        }
        #endregion

        #region Get Minimum Trans No For A Disk Upload
        public string GetMinTransForDiskUpload()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentGetMinByDateAllotedForDiskUpload") as SqlCommand;
            if (datDateAlloted != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            }
            else
            {
                db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddOutParameter(dbCommand, "MinValue", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(dbCommand);
            return db.GetParameterValue(dbCommand, "MinValue").ToString();
        }
        #endregion

        #region Get Maximum Trans No For A Disk Upload
        public string GetMaxTransForDiskUpload()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentGetMaxByDateAllotedForDiskUpload") as SqlCommand;
            if (datDateAlloted != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            }
            else
            {
                db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, SqlDateTime.Null);
            }
            db.AddOutParameter(dbCommand, "MaxValue", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(dbCommand);
            return db.GetParameterValue(dbCommand, "MaxValue").ToString();
        }
        #endregion

        #region Get Total Rate For Purchase
        public decimal GetTotalRateForPurchase()
        {
            StkParam oStkbPGenTable = new StkParam();
            if (oStkbPGenTable.GetStkbPGenTable())
            {
                return oStkbPGenTable.Bsec + oStkbPGenTable.Bstamp + oStkbPGenTable.Bcncomm +
               (oStkbPGenTable.Bcncomm * (oStkbPGenTable.Bvat / 100)) + oStkbPGenTable.Bnse + oStkbPGenTable.BCsCs +
                (oStkbPGenTable.Bsec * (oStkbPGenTable.BsecVat / 100)) + (oStkbPGenTable.Bnse * (oStkbPGenTable.BnseVat / 100)) + (oStkbPGenTable.BCsCs * (oStkbPGenTable.BcscsVat / 100));
            }

            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Total Rate For Sale
        public decimal GetTotalRateForSale()
        {
            StkParam oStkbPGenTable = new StkParam();
            if (oStkbPGenTable.GetStkbPGenTable())
            {
                return oStkbPGenTable.Ssec + oStkbPGenTable.Sstamp + oStkbPGenTable.Scncomm +
                (oStkbPGenTable.Scncomm * (oStkbPGenTable.Svat / 100)) + oStkbPGenTable.Snse + oStkbPGenTable.Scscs +
                (oStkbPGenTable.Ssec * (oStkbPGenTable.SsecVat / 100)) + (oStkbPGenTable.Snse * (oStkbPGenTable.SnseVat / 100)) + (oStkbPGenTable.Scscs * (oStkbPGenTable.ScscsVat / 100));
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Delete Reversal and Return A Command
        public SqlCommand DeleteReversalCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentDeleteReversal") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNoRev.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.ToUpper());
            return oCommand;
        }
        #endregion

        #region Get All Transactions By Non Upload Online
        public DataSet GetAllByNonUploadOnline()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByNonUploadOnline") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get Top Total Amount
        public decimal GetTopTotalAmount(int intTopNumber, string strReportFieldType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strReportFieldType.Trim() == "TOTALAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopTotalAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "TOTALUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopTotalUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPurchaseAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPurchaseUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopSaleAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopSaleUnitPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TopNumber", SqlDbType.Int, intTopNumber);
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                return decimal.Parse(db.ExecuteScalar(dbCommand).ToString());
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Bottom Total Amount
        public decimal GetBottomTotalAmount(int intTopNumber, string strReportFieldType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strReportFieldType.Trim() == "TOTALAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomTotalAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "TOTALUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomTotalUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPurchaseAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPurchaseUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomSaleAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomSaleUnitPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TopNumber", SqlDbType.Int, intTopNumber);
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                return decimal.Parse(db.ExecuteScalar(dbCommand).ToString());
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Top Percent Total Amount
        public decimal GetTopPercentTotalAmount(int intTopNumber, string strReportFieldType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strReportFieldType.Trim() == "TOTALAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPercentTotalAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "TOTALUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPercentTotalUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPercentPurchaseAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPercentPurchaseUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPercentSaleAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectTopPercentSaleUnitPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TopNumber", SqlDbType.Int, intTopNumber);
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                return decimal.Parse(db.ExecuteScalar(dbCommand).ToString());
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Bottom Perecnt Total Amount
        public decimal GetBottomPercentTotalAmount(int intTopNumber, string strReportFieldType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strReportFieldType.Trim() == "TOTALAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPercentTotalAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "TOTALUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPercentTotalUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPercentPurchaseAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "PURCHASEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPercentPurchaseUnitPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEAMOUNT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPercentSaleAmountPosted") as SqlCommand;
            }
            else if (strReportFieldType.Trim() == "SALEUNIT")
            {
                dbCommand = db.GetStoredProcCommand("AllotmentSelectBottomPercentSaleUnitPosted") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TopNumber", SqlDbType.Int, intTopNumber);
            db.AddInParameter(dbCommand, "EffDateFrom", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "EffDateTo", SqlDbType.DateTime, datDateAllotedTo);
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                return decimal.Parse(db.ExecuteScalar(dbCommand).ToString());
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Update DeductComm Return A Command
        public SqlCommand UpDateDeductCommCommand(decimal decDeductCommValue)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentUpdateDeductComm") as SqlCommand;
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTxn.Trim());
            db.AddInParameter(oCommand, "DeductComm", SqlDbType.Decimal, decDeductCommValue);
            return oCommand;
        }
        #endregion

        #region Update Customer Return Command
        public SqlCommand UpdateCustomerCommand(string strMasterCustomer, string strSubCustomer)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentUpdateCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterCustomer", SqlDbType.VarChar, strMasterCustomer.Trim());
            db.AddInParameter(dbCommand, "SubCustomer", SqlDbType.VarChar, strSubCustomer.Trim());
            return dbCommand;
        }
        #endregion

        #region Get FIX Buy and Sell
        public DataSet GetAllFIXBuySell(DateTime datPostDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentSelectFIXBuySell") as SqlCommand;
            db.AddInParameter(oCommand, "DateAlloted", SqlDbType.DateTime, datPostDate);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;

        }
        #endregion

        #region Get FIX Buy and Sell Group By BuySell,CsCsAccount,StockCode,Quantity,Price
        public DataSet GetAllFIXBuySellGroup(DateTime datPostDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AllotmentSelectFIXBuySellGroup") as SqlCommand;
            db.AddInParameter(oCommand, "DateAlloted", SqlDbType.DateTime, datPostDate);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;

        }
        #endregion

        #region Get FIX Allotment Not In GL
        public DataSet GetFIXAllotmentNotInGL(DateTime datTransDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectFIXAllotmentNotInGL") as SqlCommand;
            db.AddInParameter(dbCommand, "TransDate", SqlDbType.DateTime, datTransDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get FIX Allotment Not In Portfolio
        public DataSet GetFIXAllotmentNotInPortfolio(DateTime datTransDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectFIXAllotmentNotInPortfolio") as SqlCommand;
            db.AddInParameter(dbCommand, "TransDate", SqlDbType.DateTime, datTransDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get Group Date By Customer
        public DataSet GetGroupDateByCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectGroupDateByCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);
            db.AddInParameter(dbCommand, "DateAllotedTo", SqlDbType.DateTime, datDateAllotedTo);
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustAID.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Allotment By Date And Customer
        public DataSet GetAllotmentByDateAndCustomer(DateTime datAllotmentDate, string strCustomerId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByDateAndCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "AllotmentDate", SqlDbType.DateTime, datAllotmentDate);
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete All Allotment Sale
        public void DeleteAllAllotmentSale()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentDeleteAllAllotmentSale") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Save Allotment Sale
        public void SaveAllotmentSale()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSaleAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "TotalAmount", SqlDbType.Decimal, decTotalAmount);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        //--------- Transfer From Old Database

        #region Get All Allotment From Old GlobalStock Database Order By Trans No
        public DataSet GetAllOld()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedbOld") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByAllOrderByTransNoOld") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Allotment Type Given Old Transaction Number
        public string GetAllotType(string strTransNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedbOld") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AllotmentSelectByTransNumberReturnAllotType", strTransNumber.Trim()) as SqlCommand;
            return (string)db.ExecuteScalar(dbCommand);
        }
        #endregion

        //---------------end of Transfer

        public class TradeFileGroupByStockPrice
        {
            public string CustomerId { set; get; }
            public string StockCode{ set; get; }
            public DateTime EffectiveDate { set; get; }
            public char Buy_sold_Ind { set; get; }
            public Int64 TradeUnit { set; get; }
            public decimal TradePrice { set; get; }

            
        }
    }
}
