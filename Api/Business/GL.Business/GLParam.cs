using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class GLParam
    {
        #region Declarations
        private string strCustomerProduct;
        private string strVendorProduct;
        private string strPettyCashAcct;
        private string strPayableAcct;
        private string strFXAssetControlAcct;
        private string strSalesAcct;
        private string strSalesAcctIncome;
        private string strPurchaseAcct;
        private string strPurchaseAcctIncome;
        private string strGLOpenAcct;
        private string strCustOpenAcct;
        private string strCOTAcct;
        private string strVATAcct;
        private string strIncomeAcctForBankStampDuty;
        private string strSMSChargeAcct;
        private string strSMSAlertIncomeAcct;
        private string strReserveAcct;
        private float fltVAT;
        private decimal decCOT;
        private decimal decSMSAlert;
        private decimal decSMSAlertCustomer;
        private int intBankClearingDay;
        private int intTradingClearingDay;
        private string strType;
        private string strCode;
        private string strPayableChargeVAT;
        private string strReceivableChargeVAT;
        private string strPayablePayChargeVAT;//The Bug Indicator
        private DateTime datLastOnlinePostDate;
        private int intNoOfTimeFileGenerated;
        private decimal decCBNStampDutyAmount;
        private decimal decCBNStampDutyMinimumAmtToCharge;
        private string strRevaluationreserveAcct;
        IFormatProvider format = new CultureInfo("en-GB");
        #endregion

        #region Properties
        public string CustomerProduct
        {
            set { strCustomerProduct = value; }

            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectCustomerProduct") as SqlCommand;
                strCustomerProduct = (string)db.ExecuteScalar(dbCommand).ToString();
                return strCustomerProduct;

            }
        }

        public string VendorProduct
        {
            set { strVendorProduct = value; }

            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectVendorProduct") as SqlCommand;
                strVendorProduct = (string)db.ExecuteScalar(dbCommand).ToString();
                return strVendorProduct;

            }
        }

        public string PayablePayChargeVAT
        {
            set { strPayablePayChargeVAT = value; }

            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectPayablePayChargeVAT") as SqlCommand;
                var varPayablePayChargeVAT = db.ExecuteScalar(dbCommand);
                return varPayablePayChargeVAT != null ? varPayablePayChargeVAT.ToString() : "";
            }
        }

        public string PettyCashAcct
        {
            set { strPettyCashAcct = value; }
            get { return strPettyCashAcct; }
        }
        public string PayableAcct
        {
            set { strPayableAcct = value; }
            get { return strPayableAcct; }
        }
        public string FXAssetControlAcct
        {
            set { strFXAssetControlAcct = value; }
            get { return strFXAssetControlAcct; }
        }
        public string SalesAcct
        {
            set { strSalesAcct = value; }
            get { return strSalesAcct; }
        }
        public string SalesAcctIncome
        {
            set { strSalesAcctIncome = value; }
            get { return strSalesAcctIncome; }
        }
        public string PurchaseAcct
        {
            set { strPurchaseAcct = value; }
            get { return strPurchaseAcct; }
        }
        public string PurchaseAcctIncome
        {
            set { strPurchaseAcctIncome = value; }
            get { return strPurchaseAcctIncome; }
        }
        public string COTAcct
        {
            set { strCOTAcct = value; }
            get { return strCOTAcct; }
        }
        public string VATAcct
        {
            set { strVATAcct = value; }
            get { return strVATAcct; }
        }
        public string SMSChargeAcct
        {
            set { strSMSChargeAcct = value; }
            get { return strSMSChargeAcct; }
        }
        public string SMSAlertIncomeAcct
        {
            set { strSMSAlertIncomeAcct = value; }
            get { return strSMSAlertIncomeAcct; }
        }
        public int BankClearingDay
        {
            set { intBankClearingDay = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectBankClearingDay") as SqlCommand;
                var varBankClearingDay = db.ExecuteScalar(dbCommand);
                return varBankClearingDay != null && varBankClearingDay.ToString().Trim() != "" ? int.Parse(varBankClearingDay.ToString()) : 0;
            }
        }
        public int TradingClearingDay
        {
            set { intTradingClearingDay = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectTradingClearingDay") as SqlCommand;
                var varTradingClearingDay = db.ExecuteScalar(dbCommand);
                return varTradingClearingDay != null && varTradingClearingDay.ToString().Trim() != "" ? int.Parse(varTradingClearingDay.ToString()) : 0;
            }
        }
        public string ReserveAcct
        {
            set { strReserveAcct = value; }
            get { return strReserveAcct; }
        }
        public string GLOpenAcct
        {
            set { strGLOpenAcct = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectGLOpenAcct") as SqlCommand;
                if (db.ExecuteScalar(dbCommand) != null)
                {
                    strGLOpenAcct = (string)db.ExecuteScalar(dbCommand).ToString();
                }
                else
                {
                    strGLOpenAcct = "";
                }
                return strGLOpenAcct;
            }
        }

        public string CustOpenAcct
        {
            set { strCustOpenAcct = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectCustOpenAcct") as SqlCommand;
                if (db.ExecuteScalar(dbCommand) != null)
                {
                    strCustOpenAcct = (string)db.ExecuteScalar(dbCommand).ToString();
                }
                else
                {
                    strCustOpenAcct = "";
                }
                return strCustOpenAcct;
            }
        }

        public float VAT
        {
            set { fltVAT = value; }
            get { return fltVAT; }
        }
        public decimal COT
        {
            set { decCOT = value; }
            get { return decCOT; }
        }
        public decimal SMSAlert
        {
            set { decSMSAlert = value; }
            get { return decSMSAlert; }
        }
        public decimal SMSAlertCustomer
        {
            set { decSMSAlertCustomer = value; }
            get { return decSMSAlertCustomer; }
        }
        public string Type
        {
            set { strType = value; }
            get { return strType; }
        }

        public string Code
        {
            set { strCode = value; }
            get { return strCode; }
        }

        public string PayableChargeVAT
        {
            set { strPayableChargeVAT = value; }
            get { return strPayableChargeVAT; }
        }

        public string ReceivableChargeVAT
        {
            set { strReceivableChargeVAT = value; }
            get { return strReceivableChargeVAT; }
        }

        public DateTime LastOnlinePostDate
        {
            set { datLastOnlinePostDate = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectLastOnlinePostDate") as SqlCommand;
                if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
                {
                    datLastOnlinePostDate = DateTime.ParseExact(db.ExecuteScalar(dbCommand).ToString().Substring(0,10),"dd/MM/yyyy",format);
                }
                else
                {

                    datLastOnlinePostDate = GeneralFunc.GetTodayDate();
                }
                return datLastOnlinePostDate;
            }
        }

        public int NoOfTimeFileGenerated
        {
            set { intNoOfTimeFileGenerated = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectNoOfTimeFileGenerated") as SqlCommand;
                if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
                {
                    intNoOfTimeFileGenerated = int.Parse(db.ExecuteScalar(dbCommand).ToString());
                }
                else
                {

                    intNoOfTimeFileGenerated = 0;
                }
                return intNoOfTimeFileGenerated;
            }
        }

        public decimal CBNStampDutyAmount
        {
            set { decCBNStampDutyAmount = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectCBNStampDutyAmount") as SqlCommand;
                var varCBNStampDutyAmount = db.ExecuteScalar(dbCommand);
                return varCBNStampDutyAmount != null && varCBNStampDutyAmount.ToString().Trim() != "" ? decimal.Parse(varCBNStampDutyAmount.ToString()) : 0;
            }
        }

        public decimal CBNStampDutyMinimumAmtToCharge
        {
            set { decCBNStampDutyMinimumAmtToCharge = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectCBNStampDutyMinimumAmtToCharge") as SqlCommand;
                var varCBNStampDutyMinimumAmtToCharge = db.ExecuteScalar(dbCommand);
                return varCBNStampDutyMinimumAmtToCharge != null && varCBNStampDutyMinimumAmtToCharge.ToString().Trim() != "" ? decimal.Parse(varCBNStampDutyMinimumAmtToCharge.ToString()) : 0;
            }
        }

        public string IncomeAcctForBankStampDuty
        {
            set { strIncomeAcctForBankStampDuty = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectIncomeAcctForBankStampDuty") as SqlCommand;
                var varIncomeAcctForBankStampDuty = db.ExecuteScalar(dbCommand);
                return varIncomeAcctForBankStampDuty != null ? varIncomeAcctForBankStampDuty.ToString() : "";
            }
        }

        public string RevaluationreserveAcct
        {
            set { strRevaluationreserveAcct = value; }
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelectRevaluationreserveAcct") as SqlCommand;
                var varRevaluationreserveAcct = db.ExecuteScalar(dbCommand);
                return varRevaluationreserveAcct != null ? varRevaluationreserveAcct.ToString() : "";
            }
        }

        #endregion

        #region Add
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (!ChkRecordExist())
            {
                oCommand = db.GetStoredProcCommand("GLParamAdd") as SqlCommand;
            }
            else
            {
                oCommand = db.GetStoredProcCommand("GLParamEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "CustomerProduct", SqlDbType.VarChar, strCustomerProduct);
            db.AddInParameter(oCommand, "VendorProduct", SqlDbType.VarChar, strVendorProduct);
            db.AddInParameter(oCommand, "PettyCashAcct", SqlDbType.VarChar, strPettyCashAcct);
            db.AddInParameter(oCommand, "PayableAcct", SqlDbType.VarChar, strPayableAcct);
            db.AddInParameter(oCommand, "FXAssetControlAcct", SqlDbType.VarChar, strFXAssetControlAcct);
            db.AddInParameter(oCommand, "SalesAcct", SqlDbType.VarChar, strSalesAcct);
            db.AddInParameter(oCommand, "SalesAcctIncome", SqlDbType.VarChar, strSalesAcctIncome);
            db.AddInParameter(oCommand, "PurchaseAcct", SqlDbType.VarChar, strPurchaseAcct);
            db.AddInParameter(oCommand, "PurchaseAcctIncome", SqlDbType.VarChar, strPurchaseAcctIncome);
            db.AddInParameter(oCommand, "GLOpenAcct", SqlDbType.VarChar, strGLOpenAcct);
            db.AddInParameter(oCommand, "COTAcct", SqlDbType.VarChar, strCOTAcct);
            db.AddInParameter(oCommand, "VATAcct", SqlDbType.VarChar, strVATAcct);
            db.AddInParameter(oCommand, "SMSChargeAcct", SqlDbType.VarChar, strSMSChargeAcct);
            db.AddInParameter(oCommand, "SMSAlertIncomeAcct", SqlDbType.VarChar, strSMSAlertIncomeAcct);
            db.AddInParameter(oCommand, "ReserveAcct", SqlDbType.VarChar, strReserveAcct);
            db.AddInParameter(oCommand, "CustOpenAcct", SqlDbType.VarChar, strCustOpenAcct);
            db.AddInParameter(oCommand, "VAT", SqlDbType.Float, fltVAT);
            db.AddInParameter(oCommand, "COT", SqlDbType.Decimal, decCOT);
            db.AddInParameter(oCommand, "SMSAlert", SqlDbType.Decimal, decSMSAlert);
            db.AddInParameter(oCommand, "SMSAlertCustomer", SqlDbType.Decimal, decSMSAlertCustomer);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "PayableChargeVAT", SqlDbType.VarChar, strPayableChargeVAT);
            db.AddInParameter(oCommand, "ReceivableChargeVAT", SqlDbType.VarChar, strReceivableChargeVAT);
            db.AddInParameter(oCommand, "BankClearingDay", SqlDbType.Int, intBankClearingDay);
            db.AddInParameter(oCommand, "TradingClearingDay", SqlDbType.Int, intTradingClearingDay);
            db.ExecuteNonQuery(oCommand);

            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get GL Parameter
        public bool GetGLParam()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("GLParamSelect") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustomerProduct = thisRow[0]["CustomerProduct"].ToString();
                strVendorProduct = thisRow[0]["VendorProduct"].ToString();
                strPettyCashAcct = thisRow[0]["PettyCashAcct"].ToString();
                strPayableAcct = thisRow[0]["PayableAcct"].ToString();
                strFXAssetControlAcct = thisRow[0]["FXAssetControlAcct"].ToString();
                strSalesAcct = thisRow[0]["SalesAcct"].ToString();
                strSalesAcctIncome = thisRow[0]["SalesAcctIncome"].ToString();
                strPurchaseAcct = thisRow[0]["PurchaseAcct"].ToString();
                strPurchaseAcctIncome = thisRow[0]["PurchaseAcctIncome"].ToString();
                strGLOpenAcct = thisRow[0]["GLOpenAcct"].ToString();
                strCOTAcct = thisRow[0]["COTAcct"].ToString();
                strVATAcct = thisRow[0]["VATAcct"].ToString();
                strSMSChargeAcct = thisRow[0]["SMSChargeAcct"].ToString();
                strSMSAlertIncomeAcct = thisRow[0]["SMSAlertIncomeAcct"].ToString();
                strReserveAcct = thisRow[0]["ReserveAcct"].ToString();
                strCustOpenAcct = thisRow[0]["CustOpenAcct"].ToString();
                if (thisRow[0]["VAT"] != null && thisRow[0]["VAT"].ToString() != "")
                {
                    fltVAT = float.Parse(thisRow[0]["VAT"].ToString());
                }
                else
                {
                    fltVAT = 0;
                }
                if (thisRow[0]["COT"] != null && thisRow[0]["COT"].ToString() != "")
                {
                    decCOT = decimal.Parse(thisRow[0]["COT"].ToString());
                }
                else
                {
                    decCOT = 0;
                }
                if (thisRow[0]["SMSAlert"] != null && thisRow[0]["SMSAlert"].ToString() != "")
                {
                    decSMSAlert = decimal.Parse(thisRow[0]["SMSAlert"].ToString());
                }
                else
                {
                    decSMSAlert = 0;
                }
                if (thisRow[0]["SMSAlertCustomer"] != null && thisRow[0]["SMSAlertCustomer"].ToString() != "")
                {
                    decSMSAlertCustomer = decimal.Parse(thisRow[0]["SMSAlertCustomer"].ToString());
                }
                else
                {
                    decSMSAlertCustomer = 0;
                }
                strPayableChargeVAT = thisRow[0]["PayableChargeVAT"].ToString();
                strReceivableChargeVAT = thisRow[0]["ReceivableChargeVAT"].ToString();
                intBankClearingDay = thisRow[0]["BankClearingDay"].ToString().Trim() != "" ? int.Parse(thisRow[0]["BankClearingDay"].ToString()) : 0;
                intTradingClearingDay = thisRow[0]["TradingClearingDay"].ToString().Trim() != "" ? int.Parse(thisRow[0]["TradingClearingDay"].ToString()) : 0;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Record Exist
        public bool ChkRecordExist()
        {
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("GLParamChkRecordExist") as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
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

        #region Flag Expire Status
        public void FlagExpireStatus()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("GLParamEditPayablePayChargeVAT") as SqlCommand;
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Other Parameter Tables

        #region Get Check Paramater
        public string CheckParameter()
        {
            string strParamValue;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("pParamsSelectByType") as SqlCommand;
            db.AddInParameter(oCommand, "Type", SqlDbType.VarChar, strType.Trim());
            strParamValue = (string) db.ExecuteScalar(oCommand);
            if (strParamValue != null)
            {
                return strParamValue;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Get All PParam
        public DataSet GetAllPParam()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("pParamsSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Update Pparam
        public void UpdatePparam()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("pParamsUpdate") as SqlCommand;
            db.AddInParameter(oCommand, "Type", SqlDbType.VarChar, strType.Trim());
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get All
        public DataSet GetAll(string strTableName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("cParamsSelectAll") as SqlCommand;
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Order By Code
        public DataSet GetAllOrderByCode(string strTableName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("cParamsSelectAllOrderByCode") as SqlCommand;
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get cParam
        public string GetcParam(string strTableName,string strCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("cParamsSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.AddInParameter(dbCommand, "Code", SqlDbType.VarChar, strCode.Trim());
            var cParamDescription = db.ExecuteScalar(dbCommand);
            return cParamDescription != null ? cParamDescription.ToString() : "";
        }
        #endregion

        #region Get Missing KYC Documents
        public DataSet GetMissingKYCDocuments(string strCustomerNumber, int intCustomerType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("cParamsChkKYCMissing") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNumber", SqlDbType.VarChar, strCustomerNumber.Trim());
            db.AddInParameter(dbCommand, "ClientType", SqlDbType.Int, intCustomerType);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        

        #region Update rParam Open Close Date
        public void UpdaterParamOpenCloseDate(DateTime datOpenDate, DateTime datCloseDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("rParamUpdateOpenCloseDate") as SqlCommand;
            db.AddInParameter(dbCommand, "OpenDate", SqlDbType.DateTime, datOpenDate);
            db.AddInParameter(dbCommand, "CloseDate", SqlDbType.DateTime, datCloseDate);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Delete Account Customer Range
        public void DeleteAccountCustomerRange()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctCustRangeDelete") as SqlCommand;
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Save Account Customer Range
        public void SaveAccountCustomerRange(string strCustomerNo,decimal decBalanceAmount)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctCustRangeAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "CustID", SqlDbType.VarChar, strCustomerNo.Trim());
            db.AddInParameter(oCommand, "OBalance", SqlDbType.Money, decBalanceAmount);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Check Account Deactivation Exist In Acct Cust Range
        public bool ChkAccountDeactivationExistInAcctCustRange(string strProductCodeRange)
        {
            bool blnStastus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctCustRangeSelectDeactivateAccount") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCodeRange.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            if (oDS.Tables[0].Rows.Count >= 1)
            {
                blnStastus = true;
            }
            return blnStastus;
        }
        #endregion

        public class PParam
        {
            public string TypeName { get; set; }
            public string CodeValue { get; set; }
        }

        #endregion

        #region Get Next Scan Image Number
        public Int64 GetNextScanImageNumber()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ScanImageNumberAdd") as SqlCommand;
            db.AddOutParameter(dbCommand, "ScanImageNumber", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(dbCommand);
            var varReturnNumber = db.GetParameterValue(dbCommand, "ScanImageNumber");
            return varReturnNumber != null && varReturnNumber.ToString().Trim() != ""
                ? Int64.Parse(varReturnNumber.ToString()) : 1;
        }
        #endregion

        #region Convert To List
        public IEnumerable<GLParam> ConvertToList(DataTable dataTable)
        {
            var oLists = new List<GLParam>();
            foreach (DataRow oRow in dataTable.Rows)
            {
                var oList = new GLParam
                {
                    COTAcct = Convert.ToString(oRow["Code"]),
                    VATAcct = Convert.ToString(oRow["Description"])
                };
                oLists.Add(oList);
            }
            return oLists;
        }
        #endregion

    }
}
