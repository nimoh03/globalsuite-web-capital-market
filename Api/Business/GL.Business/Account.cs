using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class Account
    {
        #region Declaration
        private string strAccountId, strAccountName, strParentId, strSaveType, strBranch, strTransNo, strUserId, strAccountType;
        private int intAccountLevel;
        private string strIsParent, strInternalAccount,strCustomerNo,strProduct,strBankAccount,strPettyCashAccount;
        private string strSavedPartial;
        private string strPreviousYearCreditDebitAnnual, strParentAcctToGetFirstLevel = "";
        private Int64 intIncomeStateAnnual, intSOCFAnnual, intSOCIEAnnual, intSOFPAnnual;
        private Int64 intOldSOFPAnnualItem;
        private bool blnEOYPeriod;
        
        #endregion

        #region Properties
        public string AccountId
        {
            set { strAccountId = value; }
            get { return strAccountId; }
        }
        public string AccountName
        {
            set { strAccountName = value; }
            get { return strAccountName; }
        }

        public string ParentId
        {
            set { strParentId = value; }
            get { return strParentId; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public int AccountLevel
        {
            set { intAccountLevel = value; }
            get { return intAccountLevel; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }

        public string AccountType
        {
            set { strAccountType = value; }
            get { return strAccountType; }
        }

        public string IsParent
        {
            set { strIsParent = value; }
            get { return strIsParent; }
        }

        public string InternalAccount
        {
            set { strInternalAccount = value; }
            get { return strInternalAccount; }
        }

        public string CustomerNo
        {
            set { strCustomerNo = value; }
            get { return strCustomerNo; }
        }

        public string Product
        {
            set { strProduct = value; }
            get { return strProduct; }
        }

        public string BankAccount
        {
            set { strBankAccount = value; }
            get { return strBankAccount; }
        }

        public string PettyCashAccount
        {
            set { strPettyCashAccount = value; }
            get { return strPettyCashAccount; }
        }

        public string SavedPartial
        {
            set { strSavedPartial = value; }
            get { return strSavedPartial; }
        }

        public Int64 IncomeStateAnnual
        {
            set { intIncomeStateAnnual = value; }
            get { return intIncomeStateAnnual; }
        }
       
        public Int64 SOCFAnnual
        {
            set { intSOCFAnnual = value; }
            get { return intSOCFAnnual; }
        }

        public Int64 SOCIEAnnual
        {
            set { intSOCIEAnnual = value; }
            get { return intSOCIEAnnual; }
        }

        public Int64 SOFPAnnual
        {
            set { intSOFPAnnual = value; }
            get { return intSOFPAnnual; }
        }

        public Int64 OldSOFPAnnualItem
        {
            set { intOldSOFPAnnualItem = value; }
            get { return intOldSOFPAnnualItem; }
        }

        public string PreviousYearCreditDebitAnnual
        {
            set { strPreviousYearCreditDebitAnnual = value; }
            get { return strPreviousYearCreditDebitAnnual; }
        }

        public bool ExcludeInIFRSReporting {set ;get; }

        public bool EOYPeriod
        {
            set { blnEOYPeriod = value; }
            get { return blnEOYPeriod; }
        }

        public string BankChargeAccount { get; set; }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            string strFormerAccountNo = GetAccountId();
            strSavedPartial = "N";
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            //if (ChkAccountIsCustomerAcct(strParentId))
            //{
            //    enSaveStatus = DataGeneral.SaveStatus.IsCustomerAccount;
            //    return enSaveStatus;
            //}
            AcctGL oGLParent = new AcctGL();
            oGLParent.MasterID = strParentId;
            if (oGLParent.CheckTransExist() && !ChkAccountIsCustomerAcct(strParentId))
            {
                enSaveStatus = DataGeneral.SaveStatus.DuplicateRef;
                return enSaveStatus;
            }
            if (ChkNameExist())
            {
                if (strSaveType == "ADDS")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                }
                else if (strSaveType == "EDIT")
                {
                    enSaveStatus = DataGeneral.SaveStatus.NameExistEdit;
                }
                return enSaveStatus;
            }
            if (ChkAccountIdExist())
            {
                if (strSaveType == "ADDS")
                {
                    enSaveStatus = DataGeneral.SaveStatus.AccountIdExistAdd;
                }
                else if (strSaveType == "EDIT")
                {
                    enSaveStatus = DataGeneral.SaveStatus.AccountIdExistEdit;
                }
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("AccountAddNew") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                AcctGL oGL = new AcctGL();
                oGL.MasterID = strFormerAccountNo;
                if (!ChkAccountIsParentAcct(strFormerAccountNo) && (!oGL.CheckTransExist()))
                {
                    oCommand = db.GetStoredProcCommand("AccountEdit") as SqlCommand;
                }
                else
                {
                    oCommand = db.GetStoredProcCommand("AccountEditExtended") as SqlCommand;
                    strSavedPartial = "Y";
                }
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            }
            db.AddInParameter(oCommand, "IsParent", SqlDbType.Char, strIsParent.Trim());
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim().ToUpper());
            db.AddInParameter(oCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim().ToUpper());
            db.AddInParameter(oCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim().ToUpper());
            db.AddInParameter(oCommand, "AccountLevel", SqlDbType.Int, intAccountLevel);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "AccountType", SqlDbType.VarChar, strAccountType.Trim());
            db.AddInParameter(oCommand, "InternalAccount", SqlDbType.Char, strInternalAccount.Trim());
            db.AddInParameter(oCommand, "BankAccount", SqlDbType.Char, strBankAccount.Trim());
            db.AddInParameter(oCommand, "PettyCashAccount", SqlDbType.Char, strPettyCashAccount.Trim());
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(oCommand, "IncomeStateAnnual", SqlDbType.BigInt, intIncomeStateAnnual);
            db.AddInParameter(oCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
            db.AddInParameter(oCommand, "SOCIEAnnual", SqlDbType.BigInt, intSOCIEAnnual);
            db.AddInParameter(oCommand, "SOFPAnnual", SqlDbType.BigInt, intSOFPAnnual);
            db.AddInParameter(oCommand, "PreviousYearCreditDebitAnnual", SqlDbType.Char, strPreviousYearCreditDebitAnnual.Trim());
            db.AddInParameter(oCommand, "ExcludeInIFRSReporting", SqlDbType.Bit, ExcludeInIFRSReporting);
            db.AddInParameter(oCommand, "BankChargeAccount", SqlDbType.Char, BankChargeAccount.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enDeleteAccountStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enDeleteAccountStatus = DataGeneral.SaveStatus.NotExist;
                return enDeleteAccountStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            AcctGL oGL = new AcctGL();
            oGL.MasterID = strAccountId;
            Product oProduct = new Product();
            
            if (!ChkAccountIsParentAcct(strAccountId) && (!oGL.CheckTransExist()))
            {
                oCommand = db.GetStoredProcCommand("AccountDelete") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                db.AddInParameter(oCommand, "IsParent", SqlDbType.Char, "");
                db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim().ToUpper());
                db.AddInParameter(oCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim().ToUpper());
                db.AddInParameter(oCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim().ToUpper());
                db.AddInParameter(oCommand, "AccountLevel", SqlDbType.Int, intAccountLevel);
                db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                db.AddInParameter(oCommand, "AccountType", SqlDbType.VarChar, strAccountType.Trim());
                db.AddInParameter(oCommand, "InternalAccount", SqlDbType.Char, strInternalAccount.Trim());
                db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
                db.ExecuteNonQuery(oCommand);
                enDeleteAccountStatus = DataGeneral.SaveStatus.Saved;
            }
            else if (ChkAccountIsCustomerAcct(strAccountId) && (!oGL.CheckTransExist()) && oProduct.GetAllByAccount(strAccountId).Tables[0].Rows.Count == 0)
            {
                oCommand = db.GetStoredProcCommand("AccountDelete") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                db.AddInParameter(oCommand, "IsParent", SqlDbType.Char, "");
                db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim().ToUpper());
                db.AddInParameter(oCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim().ToUpper());
                db.AddInParameter(oCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim().ToUpper());
                db.AddInParameter(oCommand, "AccountLevel", SqlDbType.Int, intAccountLevel);
                db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                db.AddInParameter(oCommand, "AccountType", SqlDbType.VarChar, strAccountType.Trim());
                db.AddInParameter(oCommand, "InternalAccount", SqlDbType.Char, strInternalAccount.Trim());
                db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
                db.ExecuteNonQuery(oCommand);
                enDeleteAccountStatus = DataGeneral.SaveStatus.Saved;
            }
            else
            {
                enDeleteAccountStatus = DataGeneral.SaveStatus.NotSaved;
            }

            return enDeleteAccountStatus;
        }
        #endregion

        #region Get All Account Level By Account Level
        public DataSet GetAllAcctLevel()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctLevelSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Account Level By Account Level Except Zero
        public DataSet GetAllAcctLevelExceptZero()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctLevelSelectAllExceptZero") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Account Type Order By Print Pos
        public DataSet GetAllAcctType()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctTypeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Account
        public bool GetAccount(string strAccountBranch)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSelect") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strAccountBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strAccountId = thisRow[0]["AccountId"].ToString();
                strAccountName = thisRow[0]["Description"].ToString();
                strAccountType = thisRow[0]["AcctType"].ToString();
                intAccountLevel = int.Parse(thisRow[0]["AccountLevel"].ToString());
                strParentId = thisRow[0]["ParentId"].ToString();
                strInternalAccount = thisRow[0]["InternalAccount"].ToString();
                strTransNo = thisRow[0]["Transno"].ToString();
                strBankAccount = thisRow[0]["Bank"].ToString();
                strPettyCashAccount = thisRow[0]["PettyCash"].ToString();
                strBranch = thisRow[0]["BranchId"].ToString();
                strUserId = thisRow[0]["UserID"].ToString();
                intIncomeStateAnnual = thisRow[0]["IncomeStateAnnual"] == null || thisRow[0]["IncomeStateAnnual"].ToString().Trim() == ""
                                     ? 0 : long.Parse(thisRow[0]["IncomeStateAnnual"].ToString());
                intSOCFAnnual = thisRow[0]["SOCFAnnual"] == null || thisRow[0]["SOCFAnnual"].ToString().Trim() == ""
                     ? 0 : long.Parse(thisRow[0]["SOCFAnnual"].ToString());
                intSOCIEAnnual = thisRow[0]["SOCIEAnnual"] == null || thisRow[0]["SOCIEAnnual"].ToString().Trim() == ""
                    ? 0 : long.Parse(thisRow[0]["SOCIEAnnual"].ToString());
                intSOFPAnnual = thisRow[0]["SOFPAnnual"] == null || thisRow[0]["SOFPAnnual"].ToString().Trim() == ""
                    ? 0 : long.Parse(thisRow[0]["SOFPAnnual"].ToString());
                strPreviousYearCreditDebitAnnual = thisRow[0]["PreviousYearCreditDebitAnnual"] == null ? "" : thisRow[0]["PreviousYearCreditDebitAnnual"].ToString();
                ExcludeInIFRSReporting = thisRow[0]["ExcludeInIFRSReporting"] == null || thisRow[0]["ExcludeInIFRSReporting"].ToString().Trim() == ""
                    ? false : bool.Parse(thisRow[0]["ExcludeInIFRSReporting"].ToString());

                BankChargeAccount = thisRow[0]["BankChargeAccount"] != null
                    ? thisRow[0]["BankChargeAccount"].ToString() : "";

                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Account Without Branch
        public bool GetAccountWithoutBranch()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSelectWithoutBranch") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strAccountId = thisRow[0]["AccountId"].ToString();
                strAccountName = thisRow[0]["Description"].ToString();
                strAccountType = thisRow[0]["AcctType"].ToString();
                intAccountLevel = int.Parse(thisRow[0]["AccountLevel"].ToString());
                strParentId = thisRow[0]["ParentId"].ToString();
                strInternalAccount = thisRow[0]["InternalAccount"].ToString();
                strTransNo = thisRow[0]["Transno"].ToString();
                strBankAccount = thisRow[0]["Bank"].ToString();
                strPettyCashAccount = thisRow[0]["PettyCash"].ToString();
                strBranch = thisRow[0]["BranchId"].ToString();
                strUserId = thisRow[0]["UserID"].ToString();
                intIncomeStateAnnual = thisRow[0]["IncomeStateAnnual"] == null || thisRow[0]["IncomeStateAnnual"].ToString().Trim() == ""
                                     ? 0 : long.Parse(thisRow[0]["IncomeStateAnnual"].ToString());
                intSOCFAnnual = thisRow[0]["SOCFAnnual"] == null || thisRow[0]["SOCFAnnual"].ToString().Trim() == ""
                     ? 0 : long.Parse(thisRow[0]["SOCFAnnual"].ToString());
                intSOCIEAnnual = thisRow[0]["SOCIEAnnual"] == null || thisRow[0]["SOCIEAnnual"].ToString().Trim() == ""
                    ? 0 : long.Parse(thisRow[0]["SOCIEAnnual"].ToString());
                intSOFPAnnual = thisRow[0]["SOFPAnnual"] == null || thisRow[0]["SOFPAnnual"].ToString().Trim() == ""
                    ? 0 : long.Parse(thisRow[0]["SOFPAnnual"].ToString());
                strPreviousYearCreditDebitAnnual = thisRow[0]["PreviousYearCreditDebitAnnual"] == null ? "" : thisRow[0]["PreviousYearCreditDebitAnnual"].ToString();
                BankChargeAccount = thisRow[0]["BankChargeAccount"] != null
                   ? thisRow[0]["BankChargeAccount"].ToString() : "";
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get Account Name
        public string GetAccountName(string strAccountBranch)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountName") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strAccountBranch.Trim());
            var varAccountName = db.ExecuteScalar(dbCommand);
            return varAccountName != null ? varAccountName.ToString() : "";
        }
        #endregion

        
        

        #region Get Account Name Without Branch
        public string GetAccountNameWithoutBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountNameWithoutBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            var varAccountName = db.ExecuteScalar(dbCommand);
            return varAccountName != null ? varAccountName.ToString() : "";
        }
        #endregion

        #region Get Account Type
        public string GetAccountType(string strAccountNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountType") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            var varAccounType = db.ExecuteScalar(dbCommand);
            return varAccounType != null ? varAccounType.ToString() : "";
        }
        #endregion

        #region Get Account Id
        public string GetAccountId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountId") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            var varAccountId = db.ExecuteScalar(dbCommand);
            if (varAccountId != null)
            {
                return (string)varAccountId;
            }
            else
            {
                return "";
            }

        }
        #endregion

        #region Get Account Parent Id
        public string GetAccountParentId(string strAccountNumber, string strAccountBranch)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectParentId") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strAccountBranch.Trim());
            var varParentId = db.ExecuteScalar(dbCommand);
            return varParentId != null ? varParentId.ToString() : "";
        }
        #endregion

        #region Get Account Parent Id With List
        public string GetAccountParentId(List<AccountNumber> oAccountNumbers,string strAccountNumber, string strAccountBranch)
        {
            if (oAccountNumbers != null)
            {
                string strParentNumber =
                (from s in oAccountNumbers
                 where s.AccountId.Trim() == strAccountNumber.Trim() && s.BranchId.Trim() == strAccountBranch.Trim()
                 select s.ParentId).SingleOrDefault();
                return strParentNumber;
            }
            else
            {
                return "";
            }

        }
        #endregion

        #region Get Account Level
        public string GetAccountLevel(string strAccountNumber, string strAccountBranch)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountLevel") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strAccountBranch.Trim());
            if (db.ExecuteScalar(dbCommand) == null)
            {
                return "0";
            }
            else
            {
                return db.ExecuteScalar(dbCommand).ToString();
            }

        }
        #endregion

        #region Get Account Level With List
        public string GetAccountLevel(List<AccountNumber> oAccountNumbers,string strAccountNumber, string strAccountBranch)
        {
           
            if (oAccountNumbers != null)
            {
                int intAccountLevel =
                (from s in oAccountNumbers
                 where s.AccountId.Trim() == strAccountNumber.Trim() && s.BranchId.Trim() == strAccountBranch.Trim()
                 select s.AccountLevel).SingleOrDefault();
                return intAccountLevel.ToString();
            }
            else
            {
                return "0";
            }

        }
        #endregion

        #region Get Account Branch
        public string GetAccountBranch(string strAccountNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            var varAccountBranch = db.ExecuteScalar(dbCommand);
            return varAccountBranch != null ? varAccountBranch.ToString() : "";
        }
        #endregion

        #region Get Customer Branch
        public string GetCustomerBranch(string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerSelectBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNumber", SqlDbType.VarChar, strCustomerNumber.Trim());
            var varCustomerBranch = db.ExecuteScalar(dbCommand);
            if (varCustomerBranch != null)
            {
                return varCustomerBranch.ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Get Employee Branch
        public string GetEmployeeBranch(string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("EmployeeSelectBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "EmployeeNumber", SqlDbType.VarChar, strCustomerNumber.Trim());
            var varCustomerBranch = db.ExecuteScalar(dbCommand);
            if (varCustomerBranch != null)
            {
                return varCustomerBranch.ToString();
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Get Account Name Parent Only
        public string GetAccountNameParentOnly()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByAccountIdParentOnlyReturnName") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountLevel", SqlDbType.Int, intAccountLevel);
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            if (db.ExecuteScalar(dbCommand) == null)
            {
                return "";
            }
            else
            {
                return (string)db.ExecuteScalar(dbCommand);
            }

        }
        #endregion

        #region Get Account Name Child Only For A Given Parent Id
        public string GetAccountNameChildOnlyByParentId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByAccountIdChildOnlyByParentIdReturnName") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim());
            if (db.ExecuteScalar(dbCommand) == null)
            {
                return "";
            }
            else
            {
                return (string)db.ExecuteScalar(dbCommand);
            }

        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Account Number
        public DataSet GetAllAccountNumber()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountNumber") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All - COA Search
        public DataSet GetAllByCOASearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllByCOASearch") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountLevel", SqlDbType.Int, intAccountLevel);
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            db.AddInParameter(dbCommand, "InternalAccount", SqlDbType.VarChar, strInternalAccount.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Internal Child Account
        public DataSet GetAllInternalChildAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllInternalChildAcct") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Internal Child Account Without Branch
        public DataSet GetAllInternalChildAccountWithoutBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllInternalChildAcctWithoutBranch") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child Account By Branch
        public DataSet GetAllChildAccountByBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllChildAcctByBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child Account
        public DataSet GetAllChildAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllChildAcct") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child Account With Control Account By Branch
        public DataSet GetAllChildWithControlAccountByBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllChildWithControlAcctByBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Child Account With Control Account
        public DataSet GetAllChildWithControlAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllChildWithControlAcct") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Parent Account No Control Account By Branch
        public DataSet GetAllParentNoControlAccountByBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllParentNoControlAcctByBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Parent Account With No Control Account
        public DataSet GetAllParentNoControlAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAllParentNoControlAcct") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get By Level
        public DataSet GetByLevel()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByLevel") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountLevel", SqlDbType.Int, intAccountLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        
        #region Get By Level Parent Internal Account
        public DataSet GetInternalParentGivenLevel()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByLevelInternalParent") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctLevel", SqlDbType.Int, intAccountLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get By Level And Branch Parent Internal Account
        public DataSet GetInternalParentGivenLevelBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByLevelBranchInternalParent") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "AcctLevel", SqlDbType.Int, intAccountLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get By Level And Branch Parent Internal Account - Search
        public DataSet GetInternalParentGivenLevelBranchSearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByLevelBranchInternalParentSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "AcctLevel", SqlDbType.Int, intAccountLevel);
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get By Level Parent Internal Account - Search
        public DataSet GetInternalParentGivenLevelSearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByLevelInternalParentSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctLevel", SqlDbType.Int, intAccountLevel);
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account Given Branch - Search
        public DataSet GetInternalAcctGivenBranchSearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectInternalAcctGivenBranchSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account - Search
        public DataSet GetInternalAcctSearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectInternalAcctSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account By Level
        public DataSet GetInternalAccountGivenLevel()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByLevelInternalAcct") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctLevel", SqlDbType.Int, intAccountLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account By Level And Branch
        public DataSet GetInternalAccountGivenLevelBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByLevelBranchInternalAcct") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctLevel", SqlDbType.Int, intAccountLevel);
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
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
                SqlCommand oCommand = db.GetStoredProcCommand("AccountChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
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

        #region Check AccountId Exist
        public bool ChkAccountIdExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkAccountIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(oCommand, "InternalAccount", SqlDbType.Char, strInternalAccount.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
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

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            db.AddInParameter(oCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(oCommand, "InternalAccount", SqlDbType.Char, strInternalAccount.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
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

        #region Check If Account Exist
        public bool ChkAccountExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountChkAccountExist") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (ChkAccountIsCustomerAcct(strAccountId))
                {
                    ProductAcct oProductAcct = new ProductAcct();
                    oProductAcct.ProductCode = strProduct;
                    oProductAcct.CustAID = strCustomerNo;
                    if (oProductAcct.ChkCustomerAcctExist())
                    {
                        blnStatus = true;
                    }
                }
                else
                {
                    blnStatus = true;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check Account Is Customer Account
        public bool ChkAccountIsCustomerAcct(string strAccountNumber)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkAccountIsCustomer") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check Account Is A Parent Account
        public bool ChkAccountIsParentAcct(string strAccountNumber)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkAccountIsParent") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Get All Customer Parent Accounts 
        public DataSet GetAllParentCustAcct()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand  dbCommand = db.GetStoredProcCommand("AccountSelectAllParentCustAcct") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account Given Parent and Branch
        public DataSet GetInternalAcctGivenParentBranch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectInternalAcctGivenParentBranch") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar,strBranch.Trim());
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar,strParentId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account Given Parent
        public DataSet GetInternalAcctGivenParent()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectInternalAcctGivenParent") as SqlCommand;
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar,strParentId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        

        #region Get Internal Account Given Branch Without Management Item
        public DataSet GetInternalAcctGivenBranchWithoutManagementItem(string strIFRSReportType, string strIFRSPeriod)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectInternalAcctGivenBranchWithoutManagementItem") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar,strBranch.Trim());
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim());
            db.AddInParameter(dbCommand, "IFRSReportType", SqlDbType.VarChar, strIFRSReportType.Trim());
            db.AddInParameter(dbCommand, "IFRSPeriod", SqlDbType.VarChar, strIFRSPeriod.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account Given Parent and Branch - Search
        public DataSet GetInternalAcctGivenParentBranchSearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectInternalAcctGivenParentBranchSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "BranchId", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Internal Account Given Parent - Search
        public DataSet GetInternalAcctGivenParentSearch()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectInternalAcctGivenParentSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim());
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountName", SqlDbType.VarChar, strAccountName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check If Account Exist Given Account Level
        public bool ChkAccountExistGivenLevel()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountChkMasterAccountExistGivenLevel") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "AccountLevel", SqlDbType.Int, intAccountLevel);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
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

        #region Check If Account Exist Given Parent Account
        public bool ChkAccountExistGivenParent()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountChkAccountExistGivenParent") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(dbCommand, "ParentId", SqlDbType.VarChar, strParentId.Trim().ToUpper());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
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

        #region Get Account First Level Parent Id
        public string GetAccountFirstLevelParentId(string strAccountNumber,string strAccountBranch)
        {
            string strLevelAcct = "";
            strParentAcctToGetFirstLevel = GetAccountParentId(strAccountNumber, strAccountBranch);
            strLevelAcct = GetAccountLevel(GetAccountParentId(strAccountNumber, strAccountBranch),strAccountBranch);
            if (int.Parse(strLevelAcct) != 1 && strLevelAcct.Trim() != "" && strParentAcctToGetFirstLevel.Trim() != "")
            {
                GetAccountFirstLevelParentId(strParentAcctToGetFirstLevel, strAccountBranch);
            }
            return strParentAcctToGetFirstLevel;
        }
        #endregion

        #region Check If Bank Account
        public bool ChkBankAccount(string strAccountNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectBank") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            var varBank = db.ExecuteScalar(dbCommand);
            return varBank != null && varBank.ToString().Trim() != "" && varBank.ToString().Trim() == "Y" ? true : false;
        }
        #endregion

        #region Get All Master Range
        public DataSet GetAllMasterRange(string strAccountNumberFrom, string strAccountNumberTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectMasterRange") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountIdFrom", SqlDbType.VarChar, strAccountNumberFrom.Trim());
            db.AddInParameter(dbCommand, "AccountIdTo", SqlDbType.VarChar, strAccountNumberTo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Account Range
        public DataSet GetAllAccountRange(string strAccountNumberFrom, string strAccountNumberTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountRange") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountIdFrom", SqlDbType.VarChar, strAccountNumberFrom.Trim());
            db.AddInParameter(dbCommand, "AccountIdTo", SqlDbType.VarChar, strAccountNumberTo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Total Income
        public decimal GetTotalIncome(DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByTotalIncome") as SqlCommand;
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;
        }
        #endregion

        #region Get Total Expense
        public decimal GetTotalExpense(DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByTotalExpense") as SqlCommand;
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;
        }
        #endregion
        #region IFRS

        #region Income

        #region Get Total Account Balances Given Income Statement
        public decimal GetTotalAccountBalancesGivenIncomeState(DateTime datEffDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByIncomeStateAnnualReturnBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "IncomeStateAnnual", SqlDbType.BigInt, intIncomeStateAnnual);
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;
        }
        #endregion

        #region Get Total Account Balances Given Income Statement Period
        public decimal GetTotalAccountBalancesGivenIncomeStatePeriod(DateTime datDateFrom, DateTime datDateTo)
        {
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectByIncomeStateAnnualReturnBalancePeriod") as SqlCommand;
            db.AddInParameter(dbCommand, "IncomeStateAnnual", SqlDbType.BigInt, intIncomeStateAnnual);
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;
        }
        #endregion

        #region Check Other Account Type Set To Income IFRS
        public bool ChkOtherAcctTypeSetToIncomeIFRS()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkOtherAcctTypeSetToIncomeAnnual") as SqlCommand;
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #endregion

        #region Cash Flow

        #region Get Total Account Balances Given SOCF 
        public decimal GetTotalAccountBalancesGivenSOCF(DateTime datStartDate, DateTime datEndDate, bool blnSelectedIfPreviousYear)
        {
            AcctGL oAcctGL = new AcctGL();
            decimal decTotalAmount = 0;
            Company oCompany = new Company();
            IFRSAnnual oIFRSAnnual =  new IFRSAnnual();

            oIFRSAnnual.ReportType = "SOCF";
            bool blnChkControlAccountCreditBal = oIFRSAnnual.ChkControlAccountCreditBal(intSOCFAnnual);
            string strCreditOrDebitForComputing = oIFRSAnnual.GetCreditOrDebitForComputing(intSOCFAnnual);
            bool blnChkPeriodDepreciationDiff = oIFRSAnnual.ChkPeriodDepreciationDiff(intSOCFAnnual);
            

            oCompany.GetCompany(GeneralFunc.CompanyNumber);

            
            if (blnChkControlAccountCreditBal)
            {
                Account oAccountCreditBal = new Account();
                IFRSAnnual oIFRSAnnualForAcctDebitBal = new IFRSAnnual();
                oAccountCreditBal.ChangeBackAllItemParentSOCF();
                oIFRSAnnualForAcctDebitBal.ReportType = "SOCF";
                long lngAccountDebitBaltem = oIFRSAnnualForAcctDebitBal.GetAccountDebitBaltemGivenControlAcctCreditBal(intSOCFAnnual);
                oAccountCreditBal.SOCFAnnual = lngAccountDebitBaltem;
                oAcctGL.EffectiveDate = datEndDate;
                foreach (DataRow oRowAccount in oAccountCreditBal.GetAllAccountGivenSOCF().Tables[0].Rows)
                {
                    oAcctGL.AccountID = oRowAccount["AccountId"].ToString();
                    if (oAcctGL.GetAccountBalanceByGLDate() > 0)
                    {
                        oAccountCreditBal.ChangeItemParentSOCF(oRowAccount["AccountId"].ToString(), intSOCFAnnual);
                    }
                }
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnBalanceWithoutIncomeExpense") as SqlCommand;
            db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varResult = db.ExecuteScalar(dbCommand);
            decTotalAmount = varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;

            dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnBalanceIncomeExpenseOnly") as SqlCommand;
            db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            varResult = db.ExecuteScalar(dbCommand);
            decTotalAmount =  decTotalAmount  +  (varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0);

            //PreviousYear means comparing begining year balance with current previous balance below code gives you begining year balance
            //While PreviousYearOnly gives begining balance only and I have changed the date to begining year in the IFRSAnnual class
            //Therefore PreviousYearOnly should not run in the below code only PreviousYear
            
            if (blnSelectedIfPreviousYear)
            {
                if (blnChkControlAccountCreditBal)
                {
                    Account oAccountCreditBal = new Account();
                    IFRSAnnual oIFRSAnnualForAcctDebitBal = new IFRSAnnual();
                    oAccountCreditBal.ChangeBackAllItemParentSOCF();
                    oIFRSAnnualForAcctDebitBal.ReportType = "SOCF";
                    long lngAccountDebitBaltem = oIFRSAnnualForAcctDebitBal.GetAccountDebitBaltemGivenControlAcctCreditBal(intSOCFAnnual);
                    oAccountCreditBal.SOCFAnnual = lngAccountDebitBaltem;
                    oAcctGL.EffectiveDate = oCompany.StartYear;
                    foreach (DataRow oRowAccount in oAccountCreditBal.GetAllAccountGivenSOCF().Tables[0].Rows)
                    {
                        oAcctGL.AccountID = oRowAccount["AccountId"].ToString();
                        if (oAcctGL.GetAccountBalanceByGLDate() > 0)
                        {
                            oAccountCreditBal.ChangeItemParentSOCF(oRowAccount["AccountId"].ToString(), intSOCFAnnual);
                        }
                    }
                }

                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnBalanceWithoutIncomeExpense") as SqlCommand;
                db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, oCompany.StartYear);
                varResult = db.ExecuteScalar(dbCommand);
                decTotalAmount = decTotalAmount - (varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0);
                

                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnBalanceIncomeExpenseOnly") as SqlCommand;
                db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
                db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, oCompany.StartYear);
                varResult = db.ExecuteScalar(dbCommand);
                decTotalAmount = decTotalAmount - (varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0);

                if (strCreditOrDebitForComputing.Trim() == "D")
                {
                    decTotalAmount = decTotalAmount * -1;
                }
            }

            if (blnChkPeriodDepreciationDiff)
            {
                long lngControlAccountCreditBalItem = oIFRSAnnual.GetControlAccountCreditBalItem(intSOCFAnnual);
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnBalanceWithoutIncomeExpense") as SqlCommand;
                db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, lngControlAccountCreditBalItem);
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
                varResult = db.ExecuteScalar(dbCommand);
                decTotalAmount = decTotalAmount - (varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0);

                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnBalanceIncomeExpenseOnly") as SqlCommand;
                db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, lngControlAccountCreditBalItem);
                db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
                db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
                varResult = db.ExecuteScalar(dbCommand);
                decTotalAmount = decTotalAmount - (varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0);
            }
            return decTotalAmount;
        }
        #endregion

        #region Get Total Profit And Loss Given SOCF
        public decimal GetTotalProfitAndLossGivenSOCF(DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFReturnProfitAndLoss") as SqlCommand;
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;
        }
        #endregion

        #region Get Total Customer Account Debit Balances Given SOCF
        public decimal GetTotalCustomerAccountDebitBalancesGivenSOCF(DateTime datEffDate, bool blnMergeCustBalance, bool blnSelectedIfPreviousYear)
        {
            decimal decTotalAmount = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            if (!blnMergeCustBalance)
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerDebitBalance") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerDebitBalanceMerge") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            var varResult = db.ExecuteScalar(dbCommand);
            decTotalAmount = varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;

            if (blnSelectedIfPreviousYear)
            {
                if (!blnMergeCustBalance)
                {
                    dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerDebitBalance") as SqlCommand;
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerDebitBalanceMerge") as SqlCommand;
                }
                db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
                db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate.AddYears(-1));
                varResult = db.ExecuteScalar(dbCommand);
                decTotalAmount = decTotalAmount - (varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0);
            }
            return decTotalAmount;

        }
        #endregion

        #region Get Total Customer Account Credit Balances Given SOCF
        public decimal GetTotalCustomerAccountCreditBalancesGivenSOCF(DateTime datEffDate, bool blnMergeCustBalance, bool blnSelectedIfPreviousYear)
        {
            decimal decTotalAmount = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            if (!blnMergeCustBalance)
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerCreditBalance") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerCreditBalanceMerge") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            var varResult = db.ExecuteScalar(dbCommand);
            decTotalAmount = varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;

            if (blnSelectedIfPreviousYear)
            {
                if (!blnMergeCustBalance)
                {
                    dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerCreditBalance") as SqlCommand;
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnualReturnCustomerCreditBalanceMerge") as SqlCommand;
                }
                db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
                db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate.AddYears(-1));
                varResult = db.ExecuteScalar(dbCommand);
                decTotalAmount = decTotalAmount - (varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0);
            }
            return decTotalAmount;

        }
        #endregion


        #endregion

        #region Change In Equity

        #region Get Total Account Balances Given SOCIE 
        public decimal GetTotalAccountBalancesGivenSOCIE(DateTime datStartDate, DateTime datEndDate, bool blnAsAtFlag)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectBySOCIEAnnualReturnBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "SOCIEAnnual", SqlDbType.BigInt, intSOCIEAnnual);
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            db.AddInParameter(dbCommand, "AsAt", SqlDbType.Bit, blnAsAtFlag);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;

        }
        #endregion

        #endregion

        #region Financial Position

        #region Get Total Account Balances Given SOFP
        public decimal GetTotalAccountBalancesGivenSOFP(DateTime datEffDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectBySOFPAnnualReturnBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "SOFPAnnual", SqlDbType.BigInt, intSOFPAnnual);
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;
        }
        #endregion


        #region Get Total Customer Account Debit Balances Given SOFP
        public decimal GetTotalCustomerAccountDebitBalancesGivenSOFP(DateTime datEffDate, bool blnMergeCustBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            if (!blnMergeCustBalance)
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOFPAnnualReturnCustomerDebitBalance") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOFPAnnualReturnCustomerDebitBalanceMerge") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "SOFPAnnual", SqlDbType.BigInt, intSOFPAnnual);
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;

        }
        #endregion

        #region Get Total Customer Account Credit Balances Given SOFP
        public decimal GetTotalCustomerAccountCreditBalancesGivenSOFP(DateTime datEffDate, bool blnMergeCustBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            if (!blnMergeCustBalance)
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOFPAnnualReturnCustomerCreditBalance") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectBySOFPAnnualReturnCustomerCreditBalanceMerge") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "SOFPAnnual", SqlDbType.BigInt, intSOFPAnnual);
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null && varResult.ToString().Trim() != "" ? (decimal)varResult : 0;

        }
        #endregion

        #endregion

        #region Check Item Has Account Code
        public bool ChkIFRSItemHasAccountCode(Int64 intItemNumber, string strIFRSType)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkIFRSItemHasAccountCode") as SqlCommand;
            db.AddInParameter(oCommand, "IFRSType", SqlDbType.VarChar, strIFRSType.Trim());
            db.AddInParameter(oCommand, "IFRSItemNumber", SqlDbType.BigInt, intItemNumber);
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

        
        #region Get All Total Account Balances For IFRSAnnual
        public DataSet GetAllTotalAccountBalancesForIFRSAnnual(DateTime datEffDate, string strReportTypeParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strReportTypeParam.Trim() == "INCOME")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForIncomeStateAnnual") as SqlCommand;
            }
            else if (strReportTypeParam.Trim() == "SOCF")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForSOCFAnnual") as SqlCommand;
            }
            else if (strReportTypeParam.Trim() == "SOCIE")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForSOCIEAnnual") as SqlCommand;
            }
            else if (strReportTypeParam.Trim() == "SOFP")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForSOFPAnnual") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportTypeParam.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion
        #region Get All Total Account Balances For IFRSAnnual Period
        public DataSet GetAllTotalAccountBalancesForIFRSAnnualPeriod(DateTime datDateFrom, DateTime datDateTo, string strReportTypeParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strReportTypeParam.Trim() == "INCOME")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForIncomeStateAnnualPeriod") as SqlCommand;
            }
            else if (strReportTypeParam.Trim() == "SOCF")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForSOCFAnnualPeriod") as SqlCommand;
            }
            else if (strReportTypeParam.Trim() == "SOCIE")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForSOCIEAnnualPeriod") as SqlCommand;
            }
            else if (strReportTypeParam.Trim() == "SOFP")
            {
                dbCommand = db.GetStoredProcCommand("AccountSelectAllAccountBalanceForSOFPAnnualPeriod") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "DateFrom", SqlDbType.DateTime, datDateFrom);
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, datDateTo);
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportTypeParam.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Save All Account Amount
        public void SaveAllAccountAmount(DateTime datStartDate, DateTime datPreviousDateResult,DateTime datPreviousDateResultSecond, string strReportType,string strReportPeriod,DateTime datEndDate)
        {
            IFormatProvider format = new CultureInfo("en-GB");
            ZeroriseIFRSAccountAmount();

            Company oCompany = new Company();
            oCompany.GetCompany(GeneralFunc.CompanyNumber);
            //DateTime datBeginDateForPreviousYear = DateTime.ParseExact(oCompany.StartYear.Day.ToString().PadLeft(2, char.Parse("0")) + "/" + oCompany.StartYear.Month.ToString().PadLeft(2, char.Parse("0")) + "/" + datPreviousDateResult.Year.ToString(), "dd/MM/yyyy", format);

            DateTime datBeginDateForStartYear = datPreviousDateResult.AddDays(1);
            DateTime datBeginDateForPreviousYear = datPreviousDateResultSecond.AddDays(1);
            //Start Of Date From For Income StatementOnly
            //DateTime datBeginDateForStartYearForIncomeStatement = DateTime.ParseExact(oCompany.StartYear.Day.ToString().PadLeft(2, char.Parse("0")) + "/" + oCompany.StartYear.Month.ToString().PadLeft(2, char.Parse("0")) + "/" + datStartDate.Year.ToString(), "dd/MM/yyyy", format);

            if (strReportType.Trim() == "INCOME")
            {
                if (strReportPeriod.Trim() == "PERIOD")
                {
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnualPeriod(datStartDate, Convert.ToDateTime(datEndDate), strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount1(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                }
                else if (strReportPeriod.Trim() == "QUARTER")
                {
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datStartDate, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount3(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datPreviousDateResult, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount4(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                    
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnualPeriod(datPreviousDateResult.AddDays(1), datStartDate, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount1(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnualPeriod(datPreviousDateResultSecond.AddDays(1), datPreviousDateResult, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount2(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                }
                else
                {
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datStartDate, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount1(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datPreviousDateResult, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount2(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnualPeriod(datPreviousDateResult.AddDays(1), datStartDate, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount3(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnualPeriod(datPreviousDateResultSecond.AddDays(1), datPreviousDateResult, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount4(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                }
            }
            else
            {
                foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datStartDate, strReportType).Tables[0].Rows)
                {
                    SaveToIFRSAmount1(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                }

                foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datPreviousDateResult, strReportType).Tables[0].Rows)
                {
                    SaveToIFRSAmount2(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                }
                if (strReportType.Trim() == "SOFP")
                {
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datPreviousDateResultSecond, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmount3(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                }
                if (strReportType.Trim() == "SOCF")
                {
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datBeginDateForStartYear, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmountStart1(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                    foreach (DataRow oRowAccount in GetAllTotalAccountBalancesForIFRSAnnual(datBeginDateForPreviousYear, strReportType).Tables[0].Rows)
                    {
                        SaveToIFRSAmountStart2(oRowAccount["AccountNumber"].ToString().Trim(), decimal.Parse(oRowAccount["TotalBalance"].ToString()));
                    }
                }
            }
        }
        #endregion

        #region Save To IFRS Amount1
        public void SaveToIFRSAmount1(string strAccountNumber, decimal decTotalBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSaveIFRSTotalAmount1") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(oCommand, "TotalAmount", SqlDbType.Money, decTotalBalance);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Save To IFRS Amount2
        public void SaveToIFRSAmount2(string strAccountNumber, decimal decTotalBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSaveIFRSTotalAmount2") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(oCommand, "TotalAmount2", SqlDbType.Money, decTotalBalance);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Save To IFRS Amount3
        public void SaveToIFRSAmount3(string strAccountNumber, decimal decTotalBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSaveIFRSTotalAmount3") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(oCommand, "TotalAmount3", SqlDbType.Money, decTotalBalance);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Save To IFRS Amount4
        public void SaveToIFRSAmount4(string strAccountNumber, decimal decTotalBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSaveIFRSTotalAmount4") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(oCommand, "TotalAmount4", SqlDbType.Money, decTotalBalance);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Save To IFRS AmountStart1
        public void SaveToIFRSAmountStart1(string strAccountNumber, decimal decTotalBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSaveIFRSTotalAmountStart1") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(oCommand, "TotalAmount", SqlDbType.Money, decTotalBalance);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Save To IFRS AmountStart2
        public void SaveToIFRSAmountStart2(string strAccountNumber, decimal decTotalBalance)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSaveIFRSTotalAmountStart2") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountNumber.Trim());
            db.AddInParameter(oCommand, "TotalAmount2", SqlDbType.Money, decTotalBalance);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Zerorise Amount
        public void ZeroriseIFRSAccountAmount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountZeroriseIFRSAmount") as SqlCommand;
            db.ExecuteNonQuery(oCommand);
        }
        #endregion


        #region Check IFRS SOFP Investment Exist
        public bool ChkIFRSSOFPInvestmentExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkIFRSSOFPAnnualInvestmentExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "IFRSInvestmentItemNo", SqlDbType.BigInt, intSOFPAnnual);
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
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

        #region Check IFRS SOFP Retained Earning Exist
        public bool ChkIFRSSOFPRetainedEarningExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountChkIFRSSOFPAnnualRetainedEarningExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "IFRSRetainedEarningItemNo", SqlDbType.BigInt, intSOFPAnnual);
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
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

        #region Reset All IFRS Item
        public void ResetAllIFRSItem(string strParamReportType)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountResetIFRSItem") as SqlCommand;
            db.AddInParameter(oCommand, "ReportType", SqlDbType.VarChar, strParamReportType.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Save IFRS Item
        public void SaveIFRSItem(string strItemReportType, long lngItemValue)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountSaveIFRSItem") as SqlCommand;
            db.AddInParameter(oCommand, "AccountId", SqlDbType.VarChar, strAccountId.Trim());
            db.AddInParameter(oCommand, "ItemReportType", SqlDbType.VarChar, strItemReportType.Trim());
            db.AddInParameter(oCommand, "ItemValue", SqlDbType.BigInt, lngItemValue);
            db.ExecuteNonQuery(oCommand);

        }
        #endregion

        #region Get Account Not Attached For IFRS Item
        public DataSet GetAccountNotAttachedForIFRSItem(string strReportTypeParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountNotAttachedForIFRSItem") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportTypeParam.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get Account With Different Account Type For IFRS Item
        public DataSet GetAccountWithDiffAccountTypeForIFRSItem(Int64 intIFRSItem, string strDebitCredit)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountNoWithDiffAccountTypeIFRSItem") as SqlCommand;
            db.AddInParameter(dbCommand, "IFRSItem", SqlDbType.BigInt, intIFRSItem);
            db.AddInParameter(dbCommand, "DebitCredit", SqlDbType.Char, strDebitCredit.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get Account With Wrong IFRS Item Number
        public DataSet GetAccountWithWrongIFRSItemNumber(string strReportTypeParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountWithWrongIFRSItem") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportTypeParam.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get Parent Account In GL
        public DataSet GetParentAccountInGL(string strReportTypeParam, DateTime datEffectiveDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectParentAccountInGL") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportTypeParam.Trim());
            db.AddInParameter(dbCommand, "EffectiveDate", SqlDbType.DateTime, datEffectiveDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get Account Not Attached For IFRS Item
        public DataSet GetAccountProfitLossNotAttachedForIFRSItem(string strReportTypeParam)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectAccountProfitLossNotAttachedForIFRSItem") as SqlCommand;
            db.AddInParameter(dbCommand, "ReportType", SqlDbType.VarChar, strReportTypeParam.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion


        #region Change Item Parent SOFP
        public void ChangeItemParentSOFP(string strAccountNumber, long lngNewItemParentNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSOFPAnnualChangeItemParent") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strAccountNumber);
            db.AddInParameter(dbCommand, "NewItemParentNumber", SqlDbType.BigInt, lngNewItemParentNumber);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Change Item Parent SOCF
        public void ChangeItemParentSOCF(string strAccountNumber, long lngNewItemParentNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSOCFAnnualChangeItemParent") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strAccountNumber);
            db.AddInParameter(dbCommand, "NewItemParentNumber", SqlDbType.BigInt, lngNewItemParentNumber);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Change Back All Item Parent SOFP
        public void ChangeBackAllItemParentSOFP()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSOFPAnnualChangeBackAllItemParent") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Change Back All Item Parent SOCF
        public void ChangeBackAllItemParentSOCF()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSOCFAnnualChangeBackAllItemParent") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Get All Account Given SOFP 
        public DataSet GetAllAccountGivenSOFP()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectBySOFPAnnual") as SqlCommand;
            db.AddInParameter(dbCommand, "SOFPAnnual", SqlDbType.BigInt, intSOFPAnnual);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        

        #region Get All Account Given SOCF
        public DataSet GetAllAccountGivenSOCF()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectBySOCFAnnual") as SqlCommand;
            db.AddInParameter(dbCommand, "SOCFAnnual", SqlDbType.BigInt, intSOCFAnnual);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get Old SOFP Item
        public string GetOldSOFPItem(string strAccountNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountSelectOldSOFPAnnualItem") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountId", SqlDbType.VarChar, strAccountNumber);
            var varResult = db.ExecuteScalar(dbCommand);
            return varResult != null ? varResult.ToString() : "";
        }
        #endregion

        #endregion

        public class AccountNumber
        {
            public string AccountId { get; set; }
            public string ParentId { get; set; }
            public string BranchId { get; set; }
            public int AccountLevel { get; set; }
        }

        public class AccountNumberWithDetail
        {
            public string AccountNumber { get; set; }
            public string AccountDetail { get; set; }


            #region Convert DataSet To List
            public List<AccountNumberWithDetail> ConvertDataSetToList(DataSet oDataSet)
            {
                List<AccountNumberWithDetail> lstAccountNumberWithDetail = oDataSet.Tables[0].AsEnumerable().Select(
                                oRow => new AccountNumberWithDetail
                                {
                                    AccountNumber = oRow["AccountId"].ToString(),
                                    AccountDetail = oRow["AccountDetail"].ToString(),
                                }).ToList();
                return lstAccountNumberWithDetail;
            }
            #endregion
        }
    }
}
