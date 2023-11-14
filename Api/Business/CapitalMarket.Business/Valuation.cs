using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlTypes;
using System.Globalization;
using BaseUtility.Business;

/// <summary>
/// Summary description for Valuation
/// </summary>
/// 
namespace CapitalMarket.Business
{
    public class Valuation
    {
        #region Declaration
        private string strTxn;
        private DateTime datiDate;
        private string strCustAid, strStockCode;
        private Int32 intQty;
        private decimal decUnitPrice;
        private string strCommissionType;
        private double dobGraduatedCommission;
        private decimal decConsideration, decFees, decStampDuty, decCommission;
        private decimal decNSEFee, decSECFee, decCscsFee, decVat, decTotalAmt;
        private string strTransType, strSurname, strFirstName, strOtherName;
        private string strCscsRegNo, strCscsAcctNo, strCDeal, strCustAidSec;
        private decimal decFeesSec, decStampDutySec, decCommissionSec, decNseFeeSec;
        private decimal decSecFeeSec, decCscsFeeSec, decVatSec, decTotalAmtSec;
        private string strSurnameSec, strFinalNameSec, strOtherNameSec;
        private string strCsCSRegNo, strCdeal;
        private string strCscsRegNoSec;
        private string strCscsAcctNoReg, strClientType, strClienttypeSec;
        private Int32 intPM_ID;
        private bool blnBuy;
        private string strUserID;
        private DateTime datTxnDate;
        private decimal decSecVat, decNseVat, decCscsVat;
        private string strCertNo, strPABank, strABank;
        private Int16 intPosted;
        private Int32 intProcAmt;
        private string strSaveType;
        private Int32 intBalance;
        #endregion

        #region Properties
        public string Txn
        {
            set { strTxn = value; }
            get { return strTxn; }
        }
        public DateTime iDate
        {
            set { datiDate = value; }
            get { return datiDate; }
        }
        public string CustAid
        {
            set { strCustAid = value; }
            get { return strCustAid; }
        }
        public string StockCode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public Int32 Qty
        {
            set { intQty = value; }
            get { return intQty; }
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
        public double GraduatedCommission
        {
            set { dobGraduatedCommission = value; }
            get { return dobGraduatedCommission; }
        }
        public decimal Consideration
        {
            set { decConsideration = value; }
            get { return decConsideration; }
        }
        public decimal Fees
        {
            set { decFees = value; }
            get { return decFees; }
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
        public decimal NSEFee
        {
            set { decNSEFee = value; }
            get { return decNSEFee; }
        }
        public decimal SECFee
        {
            set { decSECFee = value; }
            get { return decSECFee; }
        }
        public decimal CscsFee
        {
            set { decCscsFee = value; }
            get { return decCscsFee; }
        }
        public decimal Vat
        {
            set { decVat = value; }
            get { return decVat; }
        }
        public decimal TotalAmt
        {
            set { decTotalAmt = value; }
            get { return decTotalAmt; }
        }
        public string TransType
        {
            set { strTransType = value; }
            get { return strTransType; }
        }
        public string Surname
        {
            set { strSurname = value; }
            get { return strSurname; }
        }
        public string FirstName
        {
            set { strFirstName = value; }
            get { return strFirstName; }
        }
        public string OtherName
        {
            set { strOtherName = value; }
            get { return strOtherName; }
        }
        public string CscsRegNo
        {
            set { strCscsRegNo = value; }
            get { return strCscsRegNo; }
        }
        public string CscsAcctNo
        {
            set { strCscsAcctNo = value; }
            get { return strCscsAcctNo; }
        }
        public string CDeal
        {
            set { strCDeal = value; }
            get { return strCDeal; }
        }
        public string CustAidSec
        {
            set { strCustAidSec = value; }
            get { return strCustAidSec; }
        }
        public decimal FeesSec
        {
            set { decFeesSec = value; }
            get { return decFeesSec; }
        }
        public decimal StampDutySec
        {
            set { decStampDutySec = value; }
            get { return decStampDutySec; }
        }
        public decimal CommissionSec
        {
            set { decCommissionSec = value; }
            get { return decCommissionSec; }
        }
        public decimal NseFeeSec
        {
            set { decNseFeeSec = value; }
            get { return decNseFeeSec; }
        }
        public decimal SecFeeSec
        {
            set { decSecFeeSec = value; }
            get { return decSecFeeSec; }
        }
        public decimal CscsFeeSec
        {
            set { decCscsFeeSec = value; }
            get { return decCscsFeeSec; }
        }
        public decimal VatSec
        {
            set { decVatSec = value; }
            get { return decVatSec; }
        }
        public decimal TotalAmtSec
        {
            set { decTotalAmtSec = value; }
            get { return decTotalAmtSec; }
        }
        public string SurnameSec
        {
            set { strSurnameSec = value; }
            get { return strSurnameSec; }
        }
        public string FinalNameSec
        {
            set { strFinalNameSec = value; }
            get { return strFinalNameSec; }
        }
        public string OtherNameSec
        {
            set { strOtherNameSec = value; }
            get { return strOtherNameSec; }
        }
        public string CsCSRegNo
        {
            set { strCsCSRegNo = value; }
            get { return strCsCSRegNo; }
        }

        public string Cdeal
        {
            set { strCdeal = value; }
            get { return strCdeal; }
        }
        public string CscsRegNoSec
        {
            set { strCscsRegNoSec = value; }
            get { return strCscsRegNoSec; }
        }
        public string CscsAcctNoReg
        {
            set { strCscsAcctNoReg = value; }
            get { return strCscsAcctNoReg; }
        }
        public string ClientType
        {
            set { strClientType = value; }
            get { return strClientType; }
        }
        public string ClienttypeSec
        {
            set { strClienttypeSec = value; }
            get { return strClienttypeSec; }
        }
        public Int32 PM_ID
        {
            set { intPM_ID = value; }
            get { return intPM_ID; }
        }
        public bool Buy
        {
            set { blnBuy = value; }
            get { return blnBuy; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public DateTime TxnDate
        {
            set { datTxnDate = value; }
            get { return datTxnDate; }
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
        public string CertNo
        {
            set { strCertNo = value; }
            get { return strCertNo; }
        }
        public string PABank
        {
            set { strPABank = value; }
            get { return strPABank; }
        }
        public string ABank
        {
            set { strABank = value; }
            get { return strABank; }
        }
        public Int16 Posted
        {
            set { intPosted = value; }
            get { return intPosted; }
        }
        public Int32 ProcAmt
        {
            set { intProcAmt = value; }
            get { return intProcAmt; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        public Int32 Balance
        {
            set { intBalance = value; }
            get { return intBalance; }
        }
        #endregion

        public Valuation()
        {
            //
            // TODO: Add constructor logic here
            //
        }



        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("ValuationAddNew") as SqlCommand;
                db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "VALUATION");
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("ValuationEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "Txn", SqlDbType.VarChar, strTxn);
            db.AddInParameter(oCommand, "DateAlloted", SqlDbType.DateTime, datiDate);
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAid);
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode);
            db.AddInParameter(oCommand, "Qty", SqlDbType.Int, intQty);
            db.AddInParameter(oCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
            db.AddInParameter(oCommand, "CommissionType", SqlDbType.VarChar, strCommissionType);
            db.AddInParameter(oCommand, "Consideration", SqlDbType.Decimal, decConsideration);
            db.AddInParameter(oCommand, "SECFee", SqlDbType.Decimal, decSECFee);
            db.AddInParameter(oCommand, "StampDuty", SqlDbType.Decimal, decStampDuty);
            db.AddInParameter(oCommand, "Commission", SqlDbType.Decimal, decCommission);
            db.AddInParameter(oCommand, "VAT", SqlDbType.Decimal, decVat);
            db.AddInParameter(oCommand, "NSEFee", SqlDbType.Decimal, decNSEFee);
            db.AddInParameter(oCommand, "CSCSFee", SqlDbType.Decimal, decCscsFee);
            db.AddInParameter(oCommand, "TotalAmt", SqlDbType.Decimal, decTotalAmt);
            db.AddInParameter(oCommand, "PM_ID", SqlDbType.VarChar, intPM_ID.ToString());            
            db.AddInParameter(oCommand, "Buy_Sold_Ind", SqlDbType.VarChar, strTransType);
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserID);
            db.AddInParameter(oCommand, "CDeal", SqlDbType.VarChar, strCdeal);
            db.AddInParameter(oCommand, "SecVat", SqlDbType.Decimal, decSecVat);
            db.AddInParameter(oCommand, "NseVat", SqlDbType.Decimal, decNseVat);
            db.AddInParameter(oCommand, "CscsVat", SqlDbType.Decimal, decCscsVat);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion



        #region Get Valuation
        public bool GetValuation()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd/MM/yyyy";

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ValuationSelect") as SqlCommand;
            
            db.AddInParameter(dbCommand, "Txn", SqlDbType.VarChar, strTxn);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTxn = thisRow[0]["Txn#"].ToString();
                strCustAid = thisRow[0]["CustAID"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                
                datiDate = DateTime.ParseExact(thisRow[0]["Date"].ToString().Trim().Substring(0,10), "dd/MM/yyyy", format);
                
                intQty = int.Parse(thisRow[0]["Qty"].ToString());
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                strCommissionType = thisRow[0]["CommissionType"].ToString();
                decConsideration = decimal.Parse(thisRow[0]["Consideration"].ToString());
                decSECFee = decimal.Parse(thisRow[0]["SecFee"].ToString());
                decStampDuty = decimal.Parse(thisRow[0]["StampDuty"].ToString());
                decCommission = decimal.Parse(thisRow[0]["Commission"].ToString());
                decVat = decimal.Parse(thisRow[0]["VAT"].ToString());
                decNSEFee = decimal.Parse(thisRow[0]["NSEFee"].ToString());
                decCscsFee = decimal.Parse(thisRow[0]["CSCSFee"].ToString());
                decTotalAmt = decimal.Parse(thisRow[0]["TotalAmt"].ToString());
                intPM_ID = int.Parse(thisRow[0]["PM_ID"].ToString());
                strUserID = thisRow[0]["UserID"].ToString();

                strCdeal = thisRow[0]["Cdeal"].ToString();
                decCscsVat = decimal.Parse(thisRow[0]["CscsVat"].ToString());
                decNseVat = decimal.Parse(thisRow[0]["NseVat"].ToString());
                decSecVat = decimal.Parse(thisRow[0]["SecVat"].ToString());
                strTransType = thisRow[0]["TransType"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get All Valuation Order By Officer No
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ValuationSelectAll") as SqlCommand;
            
            DataSet oDS = db.ExecuteDataSet(oCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ValuationDelete") as SqlCommand;
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

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("ValuationChkTransNoExist") as SqlCommand;
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
    }

}
