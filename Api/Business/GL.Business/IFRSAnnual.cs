using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class IFRSAnnual
    {
        #region Declaration
        private Int64 intTransNo;
        private string strItemName;
        private Int64 intItemParent, intCustomerDebitBalanceItem, intControlAccountCreditBalItem;
        private int intItemLevel, intReportPosition, intReportPosition2, intReportPosition3, intReportPosition4, intReportPosition5;
        private int intReportPosition6, intReportPosition7, intReportPosition8, intReportPosition9;
        private decimal decTotalAmount, decTotalAmount2, decTotalAmount3, decTotalAmount4;
        private string strIsParent, strSaveType;
        private string strCreditDebitBalance, strCreditDebitBalanceForComputing;
        private bool blnShowItemName, blnShowTotalValue, blnShowTotalValueName;
        private bool blnRetainedEarning, blnCustomerDebitBalance, blnCustomerCreditBalance, blnInvestmentAccount, blnProfitLoss, blnPreviousYear;
        private string strReportType;
        private bool blnMergeCustomerBalances, blnAccountDebitBalOnly, blnControlAccountCreditBal;
        private bool blnEOYPeriod;
        
        #endregion

        #region Properties
        public Int64 TransNo
        {
            set { intTransNo = value; }
            get { return intTransNo; }
        }
        public string ItemName
        {
            set { strItemName = value; }
            get { return strItemName; }
        }
        public Int64 ItemParent
        {
            set { intItemParent = value; }
            get { return intItemParent; }
        }
        public Int64 CustomerDebitBalanceItem
        {
            set { intCustomerDebitBalanceItem = value; }
            get { return intCustomerDebitBalanceItem; }
        }
        public Int64 ControlAccountCreditBalItem
        {
            set { intControlAccountCreditBalItem = value; }
            get { return intControlAccountCreditBalItem; }
        }
        public int ItemLevel
        {
            set { intItemLevel = value; }
            get { return intItemLevel; }
        }
        public int ReportPosition
        {
            set { intReportPosition = value; }
            get { return intReportPosition; }
        }
        public int ReportPosition2
        {
            set { intReportPosition2 = value; }
            get { return intReportPosition2; }
        }
        public int ReportPosition3
        {
            set { intReportPosition3 = value; }
            get { return intReportPosition3; }
        }
        public int ReportPosition4
        {
            set { intReportPosition4 = value; }
            get { return intReportPosition4; }
        }
        public int ReportPosition5
        {
            set { intReportPosition5 = value; }
            get { return intReportPosition5; }
        }
        public int ReportPosition6
        {
            set { intReportPosition6 = value; }
            get { return intReportPosition6; }
        }
        public int ReportPosition7
        {
            set { intReportPosition7 = value; }
            get { return intReportPosition7; }
        }
        public int ReportPosition8
        {
            set { intReportPosition8 = value; }
            get { return intReportPosition8; }
        }
        public int ReportPosition9
        {
            set { intReportPosition9 = value; }
            get { return intReportPosition9; }
        }
        public decimal TotalAmount
        {
            set { decTotalAmount = value; }
            get { return decTotalAmount; }
        }
        public decimal TotalAmount2
        {
            set { decTotalAmount2 = value; }
            get { return decTotalAmount2; }
        }
        public decimal TotalAmount3
        {
            set { decTotalAmount3 = value; }
            get { return decTotalAmount3; }
        }
        public decimal TotalAmount4
        {
            set { decTotalAmount4 = value; }
            get { return decTotalAmount4; }
        }
        public string IsParent
        {
            set { strIsParent = value; }
            get { return strIsParent; }
        }

        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }

        public string CreditDebitBalance
        {
            set { strCreditDebitBalance = value; }
            get { return strCreditDebitBalance; }
        }

        public string CreditDebitBalanceForComputing
        {
            set { strCreditDebitBalanceForComputing = value; }
            get { return strCreditDebitBalanceForComputing; }
        }

        public bool ShowItemName
        {
            set { blnShowItemName = value; }
            get { return blnShowItemName; }
        }

        public bool ShowTotalValue
        {
            set { blnShowTotalValue = value; }
            get { return blnShowTotalValue; }
        }

        public bool ShowTotalValueName
        {
            set { blnShowTotalValueName = value; }
            get { return blnShowTotalValueName; }
        }

        public bool RetainedEarning
        {
            set { blnRetainedEarning = value; }
            get { return blnRetainedEarning; }
        }

        public bool CustomerDebitBalance
        {
            set { blnCustomerDebitBalance = value; }
            get { return blnCustomerDebitBalance; }
        }

        public bool CustomerCreditBalance
        {
            set { blnCustomerCreditBalance = value; }
            get { return blnCustomerCreditBalance; }
        }

        public bool InvestmentAccount
        {
            set { blnInvestmentAccount = value; }
            get { return blnInvestmentAccount; }
        }
        public string ReportType
        {
            set { strReportType = value; }
            get { return strReportType; }
        }
        public bool MergeCustomerBalances
        {
            set { blnMergeCustomerBalances = value; }
            get { return blnMergeCustomerBalances; }
        }

        public bool ProfitLoss
        {
            set { blnProfitLoss = value; }
            get { return blnProfitLoss; }
        }
        public bool PreviousYear
        {
            set { blnPreviousYear = value; }
            get { return blnPreviousYear; }
        }

        public bool AccountDebitBalOnly
        {
            set { blnAccountDebitBalOnly = value; }
            get { return blnAccountDebitBalOnly; }
        }
        public bool ControlAccountCreditBal
        {
            set { blnControlAccountCreditBal = value; }
            get { return blnControlAccountCreditBal; }
        }

        public bool EOYPeriod
        {
            set { blnEOYPeriod = value; }
            get { return blnEOYPeriod; }
        }

       
        
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("IFRSAnnualAddNew") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("IFRSAnnualEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "ItemName", SqlDbType.VarChar, strItemName.Trim().ToUpper());
            db.AddInParameter(oCommand, "ItemParent", SqlDbType.BigInt, intItemParent);
            db.AddInParameter(oCommand, "ItemLevel", SqlDbType.Int, intItemLevel);
            db.AddInParameter(oCommand, "ReportPosition", SqlDbType.Int, intReportPosition);
            db.AddInParameter(oCommand, "ReportPosition2", SqlDbType.Int, intReportPosition2);
            db.AddInParameter(oCommand, "ReportPosition3", SqlDbType.Int, intReportPosition3);
            db.AddInParameter(oCommand, "ReportPosition4", SqlDbType.Int, intReportPosition4);
            db.AddInParameter(oCommand, "ReportPosition5", SqlDbType.Int, intReportPosition5);
            db.AddInParameter(oCommand, "ReportPosition6", SqlDbType.Int, intReportPosition6);
            db.AddInParameter(oCommand, "ReportPosition7", SqlDbType.Int, intReportPosition7);
            db.AddInParameter(oCommand, "ReportPosition8", SqlDbType.Int, intReportPosition8);
            db.AddInParameter(oCommand, "ReportPosition9", SqlDbType.Int, intReportPosition9);
            db.AddInParameter(oCommand, "TotalAmount", SqlDbType.Money, decTotalAmount);
            db.AddInParameter(oCommand, "TotalAmount2", SqlDbType.Money, decTotalAmount2);
            db.AddInParameter(oCommand, "TotalAmount3", SqlDbType.Money, decTotalAmount3);
            db.AddInParameter(oCommand, "TotalAmount4", SqlDbType.Money, decTotalAmount4);
            db.AddInParameter(oCommand, "CreditDebitBalance", SqlDbType.VarChar, strCreditDebitBalance.Trim());
            db.AddInParameter(oCommand, "CreditDebitBalanceForComputing", SqlDbType.VarChar, strCreditDebitBalanceForComputing.Trim());
            db.AddInParameter(oCommand, "ShowItemName", SqlDbType.Bit, blnShowItemName);
            db.AddInParameter(oCommand, "ShowTotalValue", SqlDbType.Bit, blnShowTotalValue);
            db.AddInParameter(oCommand, "ShowTotalValueName", SqlDbType.Bit, blnShowTotalValueName);
            db.AddInParameter(oCommand, "RetainedEarning", SqlDbType.Bit, blnRetainedEarning);
            db.AddInParameter(oCommand, "CustomerDebitBalance", SqlDbType.Bit, blnCustomerDebitBalance);
            db.AddInParameter(oCommand, "CustomerCreditBalance", SqlDbType.Bit, blnCustomerCreditBalance);
            db.AddInParameter(oCommand, "InvestmentAccount", SqlDbType.Bit, blnInvestmentAccount);
            db.AddInParameter(oCommand, "ProfitLoss", SqlDbType.Bit, blnProfitLoss);
            db.AddInParameter(oCommand, "PreviousYear", SqlDbType.Bit, blnPreviousYear);
            db.AddInParameter(oCommand, "CustomerDebitBalanceItem", SqlDbType.BigInt, intCustomerDebitBalanceItem);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enDeleteItemStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            if (ChkItemHasChildren(intTransNo))
            {
                enDeleteItemStatus = DataGeneral.SaveStatus.AccountIdExistAdd;
                return enDeleteItemStatus;
            }
            Account oAccount = new Account();
            if (oAccount.ChkIFRSItemHasAccountCode(intTransNo, "ANNUAL"))
            {
                enDeleteItemStatus = DataGeneral.SaveStatus.AccountIdExistEdit;
                return enDeleteItemStatus;
            }
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("IFRSAnnualDelete") as SqlCommand;
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.ExecuteNonQuery(oCommand);
            enDeleteItemStatus = DataGeneral.SaveStatus.Saved;
            return enDeleteItemStatus;
        }
        #endregion

        #region Get IFRSAnnual
        public bool GetIFRSAnnual()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSAnnualSelect") as SqlCommand;
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strItemName = thisRow[0]["ItemName"].ToString();
                intItemLevel = int.Parse(thisRow[0]["ItemLevel"].ToString());
                intItemParent = Int64.Parse(thisRow[0]["ItemParent"].ToString());
                strIsParent = thisRow[0]["IsParent"].ToString();
                intReportPosition = int.Parse(thisRow[0]["ReportPosition"] != null && thisRow[0]["ReportPosition"].ToString().Trim() != "" ? thisRow[0]["ReportPosition"].ToString() : "0");
                intReportPosition2 = int.Parse(thisRow[0]["ReportPosition2"] != null && thisRow[0]["ReportPosition2"].ToString().Trim() != "" ? thisRow[0]["ReportPosition2"].ToString() : "0");
                intReportPosition3 = int.Parse(thisRow[0]["ReportPosition3"] != null && thisRow[0]["ReportPosition3"].ToString().Trim() != "" ? thisRow[0]["ReportPosition3"].ToString() : "0");
                intReportPosition4 = int.Parse(thisRow[0]["ReportPosition4"] != null && thisRow[0]["ReportPosition4"].ToString().Trim() != "" ? thisRow[0]["ReportPosition4"].ToString() : "0");
                intReportPosition5 = int.Parse(thisRow[0]["ReportPosition5"] != null && thisRow[0]["ReportPosition5"].ToString().Trim() != "" ? thisRow[0]["ReportPosition5"].ToString() : "0");
                intReportPosition6 = int.Parse(thisRow[0]["ReportPosition6"] != null && thisRow[0]["ReportPosition6"].ToString().Trim() != "" ? thisRow[0]["ReportPosition6"].ToString() : "0");
                intReportPosition7 = int.Parse(thisRow[0]["ReportPosition7"] != null && thisRow[0]["ReportPosition7"].ToString().Trim() != "" ? thisRow[0]["ReportPosition7"].ToString() : "0");
                intReportPosition8 = int.Parse(thisRow[0]["ReportPosition8"] != null && thisRow[0]["ReportPosition8"].ToString().Trim() != "" ? thisRow[0]["ReportPosition8"].ToString() : "0");
                intReportPosition9 = int.Parse(thisRow[0]["ReportPosition9"] != null && thisRow[0]["ReportPosition9"].ToString().Trim() != "" ? thisRow[0]["ReportPosition9"].ToString() : "0");
                decTotalAmount = thisRow[0]["TotalAmount"] != null && thisRow[0]["TotalAmount"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount"].ToString()) : 0;
                decTotalAmount2 = thisRow[0]["TotalAmount2"] != null && thisRow[0]["TotalAmount2"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount2"].ToString()) : 0;
                decTotalAmount3 = thisRow[0]["TotalAmount3"] != null && thisRow[0]["TotalAmount3"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount3"].ToString()) : 0;
                decTotalAmount4 = thisRow[0]["TotalAmount4"] != null && thisRow[0]["TotalAmount4"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount4"].ToString()) : 0;
                strCreditDebitBalance = thisRow[0]["CreditDebitBalance"].ToString();
                strCreditDebitBalanceForComputing = thisRow[0]["CreditDebitBalanceForComputing"] != null ? thisRow[0]["CreditDebitBalanceForComputing"].ToString() : "";
                blnShowItemName = thisRow[0]["ShowItemName"] != null && thisRow[0]["ShowItemName"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ShowItemName"].ToString()) : false;
                blnShowTotalValue = thisRow[0]["ShowTotalValue"] != null && thisRow[0]["ShowTotalValue"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ShowTotalValue"].ToString()) : false;
                blnShowTotalValueName = thisRow[0]["ShowTotalValueName"] != null && thisRow[0]["ShowTotalValueName"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ShowTotalValueName"].ToString()) : false;
                blnRetainedEarning = thisRow[0]["RetainedEarning"] != null && thisRow[0]["RetainedEarning"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["RetainedEarning"].ToString()) : false;
                blnCustomerDebitBalance = thisRow[0]["CustomerDebitBalance"] != null && thisRow[0]["CustomerDebitBalance"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["CustomerDebitBalance"].ToString()) : false;
                blnCustomerCreditBalance = thisRow[0]["CustomerCreditBalance"] != null && thisRow[0]["CustomerCreditBalance"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["CustomerCreditBalance"].ToString()) : false;
                blnInvestmentAccount = thisRow[0]["InvestmentAccount"] != null && thisRow[0]["InvestmentAccount"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["InvestmentAccount"].ToString()) : false;
                intCustomerDebitBalanceItem = thisRow[0]["CustomerDebitBalanceItem"] != null && thisRow[0]["CustomerDebitBalanceItem"].ToString().Trim() != "" ? Int64.Parse(thisRow[0]["CustomerDebitBalanceItem"].ToString()) : 0;
                intControlAccountCreditBalItem = thisRow[0]["ControlAccountCreditBalItem"] != null && thisRow[0]["ControlAccountCreditBalItem"].ToString().Trim() != "" ? Int64.Parse(thisRow[0]["ControlAccountCreditBalItem"].ToString()) : 0;
                blnProfitLoss = thisRow[0]["ProfitLoss"] != null && thisRow[0]["ProfitLoss"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ProfitLoss"].ToString()) : false;
                blnPreviousYear = thisRow[0]["PreviousYear"] != null && thisRow[0]["PreviousYear"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["PreviousYear"].ToString()) : false;
                blnAccountDebitBalOnly = thisRow[0]["AccountDebitBalOnly"] != null && thisRow[0]["AccountDebitBalOnly"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["AccountDebitBalOnly"].ToString()) : false;
                blnControlAccountCreditBal = thisRow[0]["ControlAccountCreditBal"] != null && thisRow[0]["ControlAccountCreditBal"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ControlAccountCreditBal"].ToString()) : false;
                strReportType = thisRow[0]["ReportType"] != null ? thisRow[0]["ReportType"].ToString() : "";
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Get IFRSAnnual Customer Debit Bal
        public bool GetIFRSAnnualCustomerDebitBal()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSAnnualSelectCustomerDebitBal") as SqlCommand;
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strItemName = thisRow[0]["ItemName"].ToString();
                intItemLevel = int.Parse(thisRow[0]["ItemLevel"].ToString());
                intItemParent = Int64.Parse(thisRow[0]["ItemParent"].ToString());
                strIsParent = thisRow[0]["IsParent"].ToString();
                intReportPosition = int.Parse(thisRow[0]["ReportPosition"] != null && thisRow[0]["ReportPosition"].ToString().Trim() != "" ? thisRow[0]["ReportPosition"].ToString() : "0");
                intReportPosition2 = int.Parse(thisRow[0]["ReportPosition2"] != null && thisRow[0]["ReportPosition2"].ToString().Trim() != "" ? thisRow[0]["ReportPosition2"].ToString() : "0");
                intReportPosition3 = int.Parse(thisRow[0]["ReportPosition3"] != null && thisRow[0]["ReportPosition3"].ToString().Trim() != "" ? thisRow[0]["ReportPosition3"].ToString() : "0");
                intReportPosition4 = int.Parse(thisRow[0]["ReportPosition4"] != null && thisRow[0]["ReportPosition4"].ToString().Trim() != "" ? thisRow[0]["ReportPosition4"].ToString() : "0");
                intReportPosition5 = int.Parse(thisRow[0]["ReportPosition5"] != null && thisRow[0]["ReportPosition5"].ToString().Trim() != "" ? thisRow[0]["ReportPosition5"].ToString() : "0");
                intReportPosition6 = int.Parse(thisRow[0]["ReportPosition6"] != null && thisRow[0]["ReportPosition6"].ToString().Trim() != "" ? thisRow[0]["ReportPosition6"].ToString() : "0");
                intReportPosition7 = int.Parse(thisRow[0]["ReportPosition7"] != null && thisRow[0]["ReportPosition7"].ToString().Trim() != "" ? thisRow[0]["ReportPosition7"].ToString() : "0");
                intReportPosition8 = int.Parse(thisRow[0]["ReportPosition8"] != null && thisRow[0]["ReportPosition8"].ToString().Trim() != "" ? thisRow[0]["ReportPosition8"].ToString() : "0");
                intReportPosition9 = int.Parse(thisRow[0]["ReportPosition9"] != null && thisRow[0]["ReportPosition9"].ToString().Trim() != "" ? thisRow[0]["ReportPosition9"].ToString() : "0");
                decTotalAmount = thisRow[0]["TotalAmount"] != null && thisRow[0]["TotalAmount"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount"].ToString()) : 0;
                decTotalAmount2 = thisRow[0]["TotalAmount2"] != null && thisRow[0]["TotalAmount2"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount2"].ToString()) : 0;
                decTotalAmount3 = thisRow[0]["TotalAmount3"] != null && thisRow[0]["TotalAmount3"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount3"].ToString()) : 0;
                decTotalAmount4 = thisRow[0]["TotalAmount4"] != null && thisRow[0]["TotalAmount4"].ToString().Trim() != "" ? decimal.Parse(thisRow[0]["TotalAmount4"].ToString()) : 0;
                strCreditDebitBalance = thisRow[0]["CreditDebitBalance"].ToString();
                strCreditDebitBalanceForComputing = thisRow[0]["CreditDebitBalanceForComputing"] != null ? thisRow[0]["CreditDebitBalanceForComputing"].ToString() : "";
                blnShowItemName = thisRow[0]["ShowItemName"] != null && thisRow[0]["ShowItemName"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ShowItemName"].ToString()) : false;
                blnShowTotalValue = thisRow[0]["ShowTotalValue"] != null && thisRow[0]["ShowTotalValue"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ShowTotalValue"].ToString()) : false;
                blnShowTotalValueName = thisRow[0]["ShowTotalValueName"] != null && thisRow[0]["ShowTotalValueName"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ShowTotalValueName"].ToString()) : false;
                blnRetainedEarning = thisRow[0]["RetainedEarning"] != null && thisRow[0]["RetainedEarning"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["RetainedEarning"].ToString()) : false;
                blnCustomerDebitBalance = thisRow[0]["CustomerDebitBalance"] != null && thisRow[0]["CustomerDebitBalance"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["CustomerDebitBalance"].ToString()) : false;
                blnCustomerCreditBalance = thisRow[0]["CustomerCreditBalance"] != null && thisRow[0]["CustomerCreditBalance"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["CustomerCreditBalance"].ToString()) : false;
                blnInvestmentAccount = thisRow[0]["InvestmentAccount"] != null && thisRow[0]["InvestmentAccount"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["InvestmentAccount"].ToString()) : false;
                intCustomerDebitBalanceItem = thisRow[0]["CustomerDebitBalanceItem"] != null && thisRow[0]["CustomerDebitBalanceItem"].ToString().Trim() != "" ? Int64.Parse(thisRow[0]["CustomerDebitBalanceItem"].ToString()) : 0;
                intControlAccountCreditBalItem = thisRow[0]["ControlAccountCreditBalItem"] != null && thisRow[0]["ControlAccountCreditBalItem"].ToString().Trim() != "" ? Int64.Parse(thisRow[0]["ControlAccountCreditBalItem"].ToString()) : 0;
                blnProfitLoss = thisRow[0]["ProfitLoss"] != null && thisRow[0]["ProfitLoss"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ProfitLoss"].ToString()) : false;
                blnPreviousYear = thisRow[0]["PreviousYear"] != null && thisRow[0]["PreviousYear"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["PreviousYear"].ToString()) : false;
                blnAccountDebitBalOnly = thisRow[0]["AccountDebitBalOnly"] != null && thisRow[0]["AccountDebitBalOnly"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["AccountDebitBalOnly"].ToString()) : false;
                blnControlAccountCreditBal = thisRow[0]["ControlAccountCreditBal"] != null && thisRow[0]["ControlAccountCreditBal"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["ControlAccountCreditBal"].ToString()) : false;
                strReportType = thisRow[0]["ReportType"] != null ? thisRow[0]["ReportType"].ToString() : "";
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion


        #region Get Item Name
        public string GetItemName(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectItemName") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null ? (string)varResult : "";

        }
        #endregion

        #region Get Item Credit Or Debit
        public string GetItemCreditOrDebit(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectItemCreditOrDebit") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null ? (string)varResult : "";
        }
        #endregion

        #region Check Retained Earning
        public bool CheckRetainedEarning(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectRetainedEarning") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != ""  ? bool.Parse(varResult.ToString()) : false;
        }
        #endregion

        #region Get Credit Or Debit For Computing
        public string GetCreditOrDebitForComputing(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectCreditOrDebitForComputing") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null ? (string)varResult : "";
        }
        #endregion

        #region Get Get Control Account Credit Balance Item
        public long GetControlAccountCreditBalItem(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectControlAccountCreditBalItem") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? long.Parse(varResult.ToString()) : 0;
        }
        #endregion

        

        #region Get  Item Parent Id
        public string GetItemParentId(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectParentId") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null ? (string)varResult : "";
        }
        #endregion

        #region Get Item Level
        public string GetItemLevel(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectItemLevel") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null ? (string)varResult : "0";
        }
        #endregion

        #region Get If Previous Year Only
        public bool GetIfPreviousYearOnly(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectPreviousYearOnly") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (bool)varResult : false;
        }
        #endregion

        #region Check Control Account Credit Bal
        public bool ChkControlAccountCreditBal(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectControlAccountCreditBal") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (bool)varResult : false;
        }
        #endregion

        #region Check Period Depreciation Difference
        public bool ChkPeriodDepreciationDiff(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectPeriodDepreciationDiff") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (bool)varResult : false;
        }
        #endregion

        #region Get Account Debit Ba ltem Given Control Acct Credit Bal
        public long GetAccountDebitBaltemGivenControlAcctCreditBal(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAcctDebitBalItemGivenAcctCreditBal") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (long)varResult : 0;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAll") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer Debit Bal
        public DataSet GetAllCustomerDebitBal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAllCustomerDebitBal") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer Credit Balance For A Debit Balance
        public DataSet GetAllCustomerCreditBalForADebitBal(string strTypeOfSave, long intTransNoForCreditBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAllCustomerCreditBalForADebitBal") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(dbCommand, "TransNoForCreditBalance", SqlDbType.BigInt, intTransNoForCreditBalance);
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TypeOfSave", SqlDbType.VarChar, strTypeOfSave.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child
        public DataSet GetAllChild()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAllChild") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child ControlAccountCreditBal
        public DataSet GetAllChildControlAccountCreditBal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAllChildControlAccountCreditBal") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child For An Item
        public DataSet GetAllChildForItem(long intItemNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAllChildForItem") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child Without Customer Debit Bal
        public DataSet GetAllChildWithoutCustomerDebitBal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAllChildWithoutCustomerDebitBal") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Parent
        public DataSet GetAllParent()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectAllParent") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get By Level
        public DataSet GetByLevel()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectByLevel") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "ItemLevel", SqlDbType.Int, intItemLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Item By Level
        public DataSet GetItemGivenLevel()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectByLevel") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "ItemLevel", SqlDbType.Int, intItemLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("IFRSAnnualChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnCustomerCreditBalance = oDs.Tables[0].Rows[0]["CustomerCreditBalance"] != null && oDs.Tables[0].Rows[0]["CustomerCreditBalance"].ToString().Trim() != "" ? bool.Parse(oDs.Tables[0].Rows[0]["CustomerCreditBalance"].ToString()) : false;
                    blnStatus = true;
                }
                else
                {
                    blnCustomerCreditBalance = false;
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

        #region Check Item Is A Parent Item
        public bool ChkItemIsParentAcct(Int64 intItemNumber)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSAnnualChkItemIsParent") as SqlCommand;
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
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

        #region Check Item Has Children
        public bool ChkItemHasChildren(Int64 intItemNumber)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSAnnualChkItemHasChildren") as SqlCommand;
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intItemNumber);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            
            return blnStatus;
        }
        #endregion

        #region Check Position Exist Given Parent Item
        public bool ChkPositionExistGivenParent()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualChkPositionExistGivenParent") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(dbCommand, "ItemParent", SqlDbType.BigInt, intItemParent);
            db.AddInParameter(dbCommand, "ItemLevel", SqlDbType.Int, intItemLevel);
            db.AddInParameter(dbCommand, "ReportPosition", SqlDbType.Int, intReportPosition);
            db.AddInParameter(dbCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
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

        #region Save All Amount
        public void SaveAllAmount(DateTime datStartDate, DateTime datPreviousDate, DateTime datPreviousDateSecond, string strReportPeriod, DateTime datEndDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            ZeroriseAmount();
            Company oCompany = new Company();
            oCompany.GetCompany(GeneralFunc.CompanyNumber);
            DateTime datBeginDateForStartYear = datPreviousDate.AddDays(1);
            DateTime datBeginDateForPreviousYear;
            datBeginDateForPreviousYear = datPreviousDateSecond.AddDays(1);
            
            //Start Of Date From For Income Statement Only
            //DateTime datBeginDateForStartYearForIncomeStatement = DateTime.ParseExact(oCompany.StartYear.Day.ToString().PadLeft(2, char.Parse("0")) + "/" + oCompany.StartYear.Month.ToString().PadLeft(2, char.Parse("0")) + "/" + datStartDate.Year.ToString(), "dd/MM/yyyy", format);


            Account oAccount = new Account();
            if (strReportType.Trim() == "INCOME")
            {
                foreach (DataRow oRow in GetAllChild().Tables[0].Rows)
                {
                    intTransNo = long.Parse(oRow["TransNo"].ToString());
                    oAccount.IncomeStateAnnual = long.Parse(oRow["TransNo"].ToString());
                    if (strReportPeriod.Trim() == "PERIOD")
                    {
                       
                        decTotalAmount = oAccount.GetTotalAccountBalancesGivenIncomeStatePeriod(datStartDate, Convert.ToDateTime(datEndDate));
                        decTotalAmount2 = 0;
                        decTotalAmount3 = 0;
                        decTotalAmount4 = 0;
                    }
                    else if (strReportPeriod.Trim() == "QUARTER")
                    {
                        decTotalAmount3 = oAccount.GetTotalAccountBalancesGivenIncomeState(datStartDate);
                        decTotalAmount4 = oAccount.GetTotalAccountBalancesGivenIncomeState(datPreviousDate);
                        decTotalAmount = oAccount.GetTotalAccountBalancesGivenIncomeStatePeriod
                            (datPreviousDate.AddDays(1), datStartDate);
                        decTotalAmount2 = oAccount.GetTotalAccountBalancesGivenIncomeStatePeriod
                            (datPreviousDateSecond.AddDays(1), datPreviousDate);
                    }
                    else
                    {
                        decTotalAmount = oAccount.GetTotalAccountBalancesGivenIncomeState(datStartDate);
                        decTotalAmount2 = oAccount.GetTotalAccountBalancesGivenIncomeState(datPreviousDate);
                        decTotalAmount3 = oAccount.GetTotalAccountBalancesGivenIncomeStatePeriod
                            (datPreviousDate.AddDays(1),datStartDate);
                        decTotalAmount4 = oAccount.GetTotalAccountBalancesGivenIncomeStatePeriod
                            (datPreviousDateSecond.AddDays(1), datPreviousDate);
                    }
                    
                    SaveEachAmount();
                }
            }
            else if (strReportType.Trim() == "SOCF")
            {
                bool blnIfPreviousYear = false;
                oAccount.ChangeBackAllItemParentSOCF();
                AcctGL oAcctGL = new AcctGL();
                //DateTime datBeginDateForStartYear = DateTime.ParseExact(oCompany.StartYear.Day.ToString().PadLeft(2, char.Parse("0")) + "/" + oCompany.StartYear.Month.ToString().PadLeft(2, char.Parse("0")) + "/" + datStartDate.Year.ToString(), "dd/MM/yyyy", format);
                //DateTime datBeginDateForPreviousYear = DateTime.ParseExact(oCompany.StartYear.Day.ToString().PadLeft(2, char.Parse("0")) + "/" + oCompany.StartYear.Month.ToString().PadLeft(2, char.Parse("0")) + "/" + datPreviousDate.Year.ToString(), "dd/MM/yyyy", format);
                foreach (DataRow oRow in GetAllChild().Tables[0].Rows)
                {
                    blnIfPreviousYear = bool.Parse(oRow["PreviousYear"] != null && oRow["PreviousYear"].ToString().Trim() != "" ? oRow["PreviousYear"].ToString() : "false");
                    intTransNo = long.Parse(oRow["TransNo"].ToString());
                    oAccount.SOCFAnnual = long.Parse(oRow["TransNo"].ToString());
                    if (!(bool.Parse(oRow["ProfitLoss"] != null && oRow["ProfitLoss"].ToString().Trim() != "" ? oRow["ProfitLoss"].ToString() : "false")
                        || bool.Parse(oRow["CustomerDebitBalance"] != null && oRow["CustomerDebitBalance"].ToString().Trim() != "" ? oRow["CustomerDebitBalance"].ToString() : "false") || bool.Parse(oRow["CustomerCreditBalance"] != null && oRow["CustomerCreditBalance"].ToString().Trim() != "" ? oRow["CustomerCreditBalance"].ToString() : "false") ))
                    {
                        if (bool.Parse(oRow["AccountDebitBalOnly"] != null && oRow["AccountDebitBalOnly"].ToString().Trim() != "" ? oRow["AccountDebitBalOnly"].ToString() : "false"))
                        {
                            if (oRow["ControlAccountCreditBalItem"] != null && oRow["ControlAccountCreditBalItem"].ToString().Trim() != "")
                            {
                                oAccount.ChangeBackAllItemParentSOCF();
                                if (bool.Parse(oRow["PreviousYearOnly"] != null && oRow["PreviousYearOnly"].ToString().Trim() != "" ? oRow["PreviousYearOnly"].ToString() : "false"))
                                {
                                    oAcctGL.EffectiveDate = datBeginDateForStartYear; 
                                }
                                else
                                {
                                    oAcctGL.EffectiveDate = datStartDate;
                                }
                                foreach (DataRow oRowAccount in oAccount.GetAllAccountGivenSOCF().Tables[0].Rows)
                                {
                                    oAcctGL.AccountID = oRowAccount["AccountId"].ToString();
                                    if (oAcctGL.GetAccountBalanceByGLDate() > 0)
                                    {
                                        oAccount.ChangeItemParentSOCF(oRowAccount["AccountId"].ToString(), long.Parse(oRow["ControlAccountCreditBalItem"].ToString()));
                                    }
                                }
                            }
                        }
                        if (bool.Parse(oRow["PreviousYearOnly"] != null && oRow["PreviousYearOnly"].ToString().Trim() != "" ? oRow["PreviousYearOnly"].ToString() : "false"))
                        {
                            decTotalAmount = oAccount.GetTotalAccountBalancesGivenSOCF(datBeginDateForStartYear, datBeginDateForStartYear, blnIfPreviousYear);
                            decTotalAmount2 = oAccount.GetTotalAccountBalancesGivenSOCF(datBeginDateForPreviousYear, datBeginDateForStartYear.AddYears(-1), blnIfPreviousYear);
                        }
                        else
                        {
                            decTotalAmount = oAccount.GetTotalAccountBalancesGivenSOCF(datBeginDateForStartYear, datStartDate, blnIfPreviousYear);
                            decTotalAmount2 = oAccount.GetTotalAccountBalancesGivenSOCF(datBeginDateForPreviousYear, datPreviousDate, blnIfPreviousYear);
                        }
                        decTotalAmount3 = 0;
                        decTotalAmount4 = 0;
                    }
                    else if (bool.Parse(oRow["ProfitLoss"] != null && oRow["ProfitLoss"].ToString().Trim() != "" ? oRow["ProfitLoss"].ToString() : "false"))
                    {
                        decTotalAmount = oAccount.GetTotalProfitAndLossGivenSOCF(datBeginDateForStartYear,datStartDate);
                        decTotalAmount2 = oAccount.GetTotalProfitAndLossGivenSOCF(datBeginDateForPreviousYear,datPreviousDate);
                        decTotalAmount3 = 0;
                        decTotalAmount4 = 0;
                    }
                    else if (bool.Parse(oRow["CustomerCreditBalance"] != null && oRow["CustomerCreditBalance"].ToString().Trim() != "" ? oRow["CustomerCreditBalance"].ToString() : "false"))
                    {
                        decTotalAmount = oAccount.GetTotalCustomerAccountCreditBalancesGivenSOCF(datStartDate, blnMergeCustomerBalances, blnIfPreviousYear);
                        decTotalAmount2 = oAccount.GetTotalCustomerAccountCreditBalancesGivenSOCF(datPreviousDate, blnMergeCustomerBalances, blnIfPreviousYear);
                        decTotalAmount3 = 0;
                        decTotalAmount4 = 0;
                    }
                    else if (bool.Parse(oRow["CustomerDebitBalance"] != null && oRow["CustomerDebitBalance"].ToString().Trim() != "" ? oRow["CustomerDebitBalance"].ToString() : "false"))
                    { }
                    else
                    {
                       
                    }

                    if (!bool.Parse(oRow["CustomerDebitBalance"] != null && oRow["CustomerDebitBalance"].ToString().Trim() != "" ? oRow["CustomerDebitBalance"].ToString() : "false"))
                    {
                        SaveEachAmount();
                    }


                    if (bool.Parse(oRow["CustomerCreditBalance"] != null && oRow["CustomerCreditBalance"].ToString().Trim() != "" ? oRow["CustomerCreditBalance"].ToString() : "false"))
                    {
                        intTransNo = long.Parse(oRow["CustomerDebitBalanceItem"].ToString());
                        decTotalAmount = oAccount.GetTotalCustomerAccountDebitBalancesGivenSOCF(datStartDate, blnMergeCustomerBalances, blnIfPreviousYear);
                        decTotalAmount2 = oAccount.GetTotalCustomerAccountDebitBalancesGivenSOCF(datPreviousDate, blnMergeCustomerBalances, blnIfPreviousYear);
                        decTotalAmount3 = 0;
                        decTotalAmount4 = 0;
                        SaveEachAmount();
                    }
                }

                //Commenting this means that at no time must the Control Debit Account Be Before The Account Debit Account
                //foreach (DataRow oRow in GetAllChildControlAccountCreditBal().Tables[0].Rows)
                //{
                //    blnIfPreviousYear = bool.Parse(oRow["PreviousYear"] != null && oRow["PreviousYear"].ToString().Trim() != "" ? oRow["PreviousYear"].ToString() : "false");
                //    intTransNo = long.Parse(oRow["TransNo"].ToString());
                //    oAccount.SOCFAnnual = long.Parse(oRow["TransNo"].ToString());
                //    decTotalAmount = oAccount.GetTotalAccountBalancesGivenSOCFAnnual(datBeginDateForStartYear, datStartDate, blnIfPreviousYear);
                //    decTotalAmount2 = oAccount.GetTotalAccountBalancesGivenSOCFAnnual(datBeginDateForPreviousYear, datPreviousDate, blnIfPreviousYear);
                //    decTotalAmount3 = 0;
                //    decTotalAmount4 = 0;
                //    SaveEachAmount();
                //}
            }
            else if (strReportType.Trim() == "SOFP")
            {
                oAccount.ChangeBackAllItemParentSOFP();
                AcctGL oAcctGL = new AcctGL();
                foreach (DataRow oRow in GetAllChild().Tables[0].Rows)
                {
                    intTransNo = long.Parse(oRow["TransNo"].ToString());
                    oAccount.SOFPAnnual = long.Parse(oRow["TransNo"].ToString());
                   
                    if (!(bool.Parse(oRow["CustomerDebitBalance"] != null && oRow["CustomerDebitBalance"].ToString().Trim() != "" ? oRow["CustomerDebitBalance"].ToString() : "false") || bool.Parse(oRow["CustomerCreditBalance"] != null && oRow["CustomerCreditBalance"].ToString().Trim() != "" ? oRow["CustomerCreditBalance"].ToString() : "false")))
                    {
                        if (bool.Parse(oRow["AccountDebitBalOnly"] != null && oRow["AccountDebitBalOnly"].ToString().Trim() != "" ? oRow["AccountDebitBalOnly"].ToString() : "false"))
                        {
                            if (oRow["ControlAccountCreditBalItem"] != null && oRow["ControlAccountCreditBalItem"].ToString().Trim() != "")
                            {
                                oAcctGL.EffectiveDate = datStartDate;
                                foreach (DataRow oRowAccount in oAccount.GetAllAccountGivenSOFP().Tables[0].Rows)
                                {
                                    oAcctGL.AccountID = oRowAccount["AccountId"].ToString();
                                    if (oAcctGL.GetAccountBalanceByGLDate() > 0)
                                    {
                                        oAccount.ChangeItemParentSOFP(oRowAccount["AccountId"].ToString(), long.Parse(oRow["ControlAccountCreditBalItem"].ToString()));
                                    }
                                }
                            }
                        }
                        decTotalAmount = oAccount.GetTotalAccountBalancesGivenSOFP(datStartDate);
                        decTotalAmount2 = oAccount.GetTotalAccountBalancesGivenSOFP(datPreviousDate);
                        decTotalAmount3 = oAccount.GetTotalAccountBalancesGivenSOFP(datPreviousDateSecond);
                        decTotalAmount4 = 0;
                    }
                    else
                    {
                        if (bool.Parse(oRow["CustomerCreditBalance"] != null && oRow["CustomerCreditBalance"].ToString().Trim() != "" ? oRow["CustomerCreditBalance"].ToString() : "false"))
                        {
                            decTotalAmount = oAccount.GetTotalCustomerAccountCreditBalancesGivenSOFP(datStartDate, blnMergeCustomerBalances);
                            decTotalAmount2 = oAccount.GetTotalCustomerAccountCreditBalancesGivenSOFP(datPreviousDate, blnMergeCustomerBalances);
                            decTotalAmount3 = oAccount.GetTotalCustomerAccountCreditBalancesGivenSOFP(datPreviousDateSecond, blnMergeCustomerBalances);
                            decTotalAmount4 = 0;
                        }
                        else if (bool.Parse(oRow["CustomerDebitBalance"] != null && oRow["CustomerDebitBalance"].ToString().Trim() != "" ? oRow["CustomerDebitBalance"].ToString() : "false"))
                        { }
                        else
                        {
                            
                        }
                    }
                    if (!bool.Parse(oRow["CustomerDebitBalance"] != null && oRow["CustomerDebitBalance"].ToString().Trim() != "" ? oRow["CustomerDebitBalance"].ToString() : "false"))
                    {
                        SaveEachAmount();
                    }

                    if (bool.Parse(oRow["CustomerCreditBalance"] != null && oRow["CustomerCreditBalance"].ToString().Trim() != "" ? oRow["CustomerCreditBalance"].ToString() : "false"))
                    {
                        intTransNo = long.Parse(oRow["CustomerDebitBalanceItem"].ToString());

                        decTotalAmount = oAccount.GetTotalCustomerAccountDebitBalancesGivenSOFP(datStartDate, blnMergeCustomerBalances);
                        decTotalAmount2 = oAccount.GetTotalCustomerAccountDebitBalancesGivenSOFP(datPreviousDate, blnMergeCustomerBalances);
                        decTotalAmount3 = oAccount.GetTotalCustomerAccountDebitBalancesGivenSOFP(datPreviousDateSecond, blnMergeCustomerBalances);
                        decTotalAmount4 = 0;
                        SaveEachAmount();
                    }
                }

                //For Control Credit Balance Account Before Debit Balance Account
                foreach (DataRow oRow in GetAllChildControlAccountCreditBal().Tables[0].Rows)
                {
                    intTransNo = long.Parse(oRow["TransNo"].ToString());
                    oAccount.SOFPAnnual = long.Parse(oRow["TransNo"].ToString());
                    decTotalAmount = oAccount.GetTotalAccountBalancesGivenSOFP(datStartDate);
                    decTotalAmount2 = oAccount.GetTotalAccountBalancesGivenSOFP(datPreviousDate);
                    decTotalAmount3 = oAccount.GetTotalAccountBalancesGivenSOFP(datPreviousDateSecond);
                    decTotalAmount4 = 0;
                    SaveEachAmount();
                }
            }
        }
        #endregion

        

        #region Save Each Amount
        public void SaveEachAmount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSAnnualSaveTotalAmount") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, intTransNo);
            db.AddInParameter(oCommand, "TotalAmount", SqlDbType.Money, decTotalAmount);
            db.AddInParameter(oCommand, "TotalAmount2", SqlDbType.Money, decTotalAmount2);
            db.AddInParameter(oCommand, "TotalAmount3", SqlDbType.Money, decTotalAmount3);
            db.AddInParameter(oCommand, "TotalAmount4", SqlDbType.Money, decTotalAmount4);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Zerorise Amount
        public void ZeroriseAmount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("IFRSAnnualZeroriseAmount") as SqlCommand;
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get Item Name Given Report Position 2
        public string GetItemNameGivenReportPosition2()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("IFRSAnnualSelectItemNameGivenReportPosition2") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportType.Trim());
            db.AddInParameter(dbCommand, "ReportPosition2", SqlDbType.Int, intReportPosition2);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null ? (string)varResult : "";
        }
        #endregion
        
    }
}
