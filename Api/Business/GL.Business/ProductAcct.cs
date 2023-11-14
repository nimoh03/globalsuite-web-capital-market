using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ProductAcct
    {
        #region Declarations
        private string strTransNo,strProductCode, strCustAId;
        private string strAgent;
        private string strCsCsReg, strCsCsAcct;
        private string strNASDCsCsReg, strNASDCsCsAcct;
        private decimal decAgentComm;
        private string strUserID;
        private float? fltComm, fltSellCommission;
        private string strSaveType;
        private string strAgentInd,strBoxLoad;
        private string strProductCodeAgent;
        private string strProductCodeInvestment;
        private string strProductCodeNASD;
        private string strBranch, strAcctDeactivation;
        private string strAccessCode;
        private int intCustodian;
        private bool blnIsCustodian;
        private bool blnIsNASD, blnProcessCrossDeal,blnCustomerDirectCashSettlement, blnCustomerDirectCashSettlementNASD,blnDoNotChargeStampDuty;
        private string strExcludeAccountInReport;
        private DateTime datDCSSetupDate, datDCSSetupDateNASD;
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string ProductCode
        {
            set { strProductCode = value; }
            get { return strProductCode; }
        }
        public float? Comm
        {
            set { fltComm = value; }
            get { return fltComm; }
        }
        public float? SellCommission
        {
            set { fltSellCommission = value; }
            get { return fltSellCommission; }
        }
        public decimal AgentComm
        {
            set { decAgentComm = value; }
            get { return decAgentComm; }
        }
        public string CustAID
        {
            set { strCustAId = value; }
            get { return strCustAId; }
        }
        public string CsCsAcct
        {
            set { strCsCsAcct = value; }
            get { return strCsCsAcct; }
        }
        public string CsCsReg
        {
            set { strCsCsReg = value; }
            get { return strCsCsReg; }
        }
        public string NASDCsCsAcct
        {
            set { strNASDCsCsAcct = value; }
            get { return strNASDCsCsAcct; }
        }
        public string NASDCsCsReg
        {
            set { strNASDCsCsReg = value; }
            get { return strNASDCsCsReg; }
        }
        public string Agent
        {
            set { strAgent = value; }
            get { return strAgent; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        public string AgentInd
        {
            set { strAgentInd = value; }
            get { return strAgentInd; }
        }
        public string BoxLoad
        {
            set { strBoxLoad = value; }
            get { return strBoxLoad; }
        }
       
        public string ProductCodeAgent
        {
            set { strProductCodeAgent = value; }
            get { return strProductCodeAgent; }
        }
        public string ProductCodeInvestment
        {
            set { strProductCodeInvestment = value; }
            get { return strProductCodeInvestment; }
        }
        public string ProductCodeNASD
        {
            set { strProductCodeNASD = value; }
            get { return strProductCodeNASD; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }

        public string AcctDeactivation
        {
            get
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAcctDeactivation") as SqlCommand;
                db.AddInParameter(dbCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
                db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
                var varAccountDeactivate = db.ExecuteScalar(dbCommand);
                strAcctDeactivation = varAccountDeactivate != null && varAccountDeactivate.ToString().Trim() != "" ? varAccountDeactivate.ToString() : "";
                return strAcctDeactivation;
            }
        }

        public string AccessCode
        {
            set { strAccessCode = value; }
            get { return strAccessCode; }
        }
        public int Custodian
        {
            set { intCustodian = value; }
            get { return intCustodian; }
        }
        public bool IsCustodian
        {
            set { blnIsCustodian = value; }
            get { return blnIsCustodian; }
        }
        public bool IsNASD
        {
            set { blnIsNASD = value; }
            get { return blnIsNASD; }
        }
        public bool ProcessCrossDeal
        {
            set { blnProcessCrossDeal = value; }
            get { return blnProcessCrossDeal; }
        }
        public bool CustomerDirectCashSettlement
        {
            set { blnCustomerDirectCashSettlement = value; }
            get { return blnCustomerDirectCashSettlement; }
        }

        public bool CustomerDirectCashSettlementNASD
        {
            set { blnCustomerDirectCashSettlementNASD = value; }
            get { return blnCustomerDirectCashSettlementNASD; }
        }
        public bool DoNotChargeStampDuty
        {
            set { blnDoNotChargeStampDuty = value; }
            get { return blnDoNotChargeStampDuty; }
        }

        public string ExcludeAccountInReport
        {
            set { strExcludeAccountInReport = value; }
            get { return strExcludeAccountInReport; }
        }
        public DateTime DCSSetupDate
        {
            set { datDCSSetupDate = value; }
            get { return datDCSSetupDate; }
        }
        public DateTime DCSSetupDateNASD
        {
            set { datDCSSetupDateNASD = value; }
            get { return datDCSSetupDateNASD; }
        }
        #endregion

        #region Enum
        public enum SaveStatus
        {
            Nothing, NotExist, AccountIdExistAdd, Saved, NotSaved, CsCsAcctExistAdd,
            CsCsAcctExistEdit, CsCsRegExistAdd, CsCsRegExistEdit, NASDCsCsAcctExistAdd,
            NASDCsCsAcctExistEdit, NASDCsCsRegExistAdd, NASDCsCsRegExistEdit,
            BoxLoadUnCheck,AgentUnCheck, AttachedToAgent,AgentCommIncluded,
            BoxLoadExist, AgentExist, AttachedToAgentExist
        }
        public enum ProductType
        {
            Nothing, StockBroking
        }
        
        #endregion

        #region Save
        public SaveStatus Save(ProductType SaveProduct)
        {
            SaveStatus enSaveStatus = SaveStatus.Nothing;
            if (strCustAId.Trim() == "" || strProductCode.Trim() == "")
            {
                throw new Exception("Customer Number Or Product Code Cannot Be Blank");
            }
            if (!ChkTransNoExist())
            {
                enSaveStatus = SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (strSaveType == "ADDS")
            {
                if (ChkCustomerAcctExist())
                {
                    enSaveStatus = SaveStatus.AccountIdExistAdd;
                    return enSaveStatus;
                }
            }
            if (SaveProduct == ProductType.StockBroking)
            {
                if (strBoxLoad == "Y")
                {
                    if (strAgent != null && strAgent.Trim() != "")
                    {
                        enSaveStatus = SaveStatus.AttachedToAgent;
                        return enSaveStatus;
                    }
                    if (decAgentComm != 0)
                    {
                        enSaveStatus = SaveStatus.AgentCommIncluded;
                        return enSaveStatus;
                    }
                }
                if (strAgentInd == "N")
                {
                    if (ChkAgentAcctGLStatus())
                    {
                        enSaveStatus = SaveStatus.AgentUnCheck;
                        return enSaveStatus;
                    }
                }
                if (strBoxLoad == "N")
                {
                    if (ChkBoxLoadGLStatus())
                    {
                        enSaveStatus = SaveStatus.BoxLoadUnCheck;
                        return enSaveStatus;
                    }
                }
                if (strAgentInd == "Y")
                {
                    if (ChkInvestmentExist())
                    {
                        enSaveStatus = SaveStatus.BoxLoadExist;
                        return enSaveStatus;
                    }
                }
                if (strBoxLoad == "Y")
                {
                    if (ChkAgentExist())
                    {
                        enSaveStatus = SaveStatus.AgentExist;
                        return enSaveStatus;
                    }
                }
                if (strBoxLoad == "Y")
                {
                    if (ChkAgentAttachedExist())
                    {
                        enSaveStatus = SaveStatus.AttachedToAgentExist;
                        return enSaveStatus;
                    }
                }
                if (ChkCsCsAcctExist())
                {
                    if (strSaveType == "ADDS")
                    {
                        enSaveStatus = SaveStatus.CsCsAcctExistAdd;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        enSaveStatus = SaveStatus.CsCsAcctExistEdit;
                    }
                    return enSaveStatus;
                }
                if (ChkNASDCsCsAcctExist())
                {
                    if (strSaveType == "ADDS")
                    {
                        enSaveStatus = SaveStatus.NASDCsCsAcctExistAdd;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        enSaveStatus = SaveStatus.NASDCsCsAcctExistEdit;
                    }
                    return enSaveStatus;
                }
                //if (ChkCsCsRegExist())
                //{
                //    if (strSaveType == "ADDS")
                //    {
                //        enSaveStatus = SaveStatus.CsCsRegExistAdd;
                //    }
                //    else if (strSaveType == "EDIT")
                //    {
                //        enSaveStatus = SaveStatus.CsCsRegExistEdit;
                //    }
                //    return enSaveStatus;
                //}
            }

            GeneralFunc oGeneralFunc = new GeneralFunc();
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
                        oCommand = db.GetStoredProcCommand("ProductAcctAdd") as SqlCommand;
                    }
                    else if (strSaveType == "EDIT")
                    {
                        oCommand = db.GetStoredProcCommand("ProductAcctEdit") as SqlCommand;
                    }
                    db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                    db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
                    db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
                    db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
                    db.AddInParameter(oCommand, "ProcessCrossDeal", SqlDbType.Bit, blnProcessCrossDeal);
                    if (SaveProduct == ProductType.StockBroking)
                    {
                        db.AddInParameter(oCommand, "CsCsAcct#", SqlDbType.VarChar, strCsCsAcct.Trim());
                        db.AddInParameter(oCommand, "CsCsReg#", SqlDbType.VarChar, strCsCsReg.Trim());
                        db.AddInParameter(oCommand, "NASDCsCsAcct", SqlDbType.VarChar, strNASDCsCsAcct.Trim());
                        db.AddInParameter(oCommand, "NASDCsCsReg", SqlDbType.VarChar, strNASDCsCsReg.Trim());
                        db.AddInParameter(oCommand, "BoxLoad", SqlDbType.Char, strBoxLoad.Trim());
                        db.AddInParameter(oCommand, "Agent", SqlDbType.VarChar, strAgent.Trim());
                        db.AddInParameter(oCommand, "AgentComm", SqlDbType.Decimal, decAgentComm);
                        db.AddInParameter(oCommand, "Comm", SqlDbType.Real, fltComm);
                        db.AddInParameter(oCommand, "SellComm", SqlDbType.Real, fltSellCommission);
                        db.AddInParameter(oCommand, "AgentInd", SqlDbType.Char, strAgentInd.Trim());
                        db.AddInParameter(oCommand, "ProductType", SqlDbType.VarChar, "S");
                        db.AddInParameter(oCommand, "AccessCode", SqlDbType.VarChar, strAccessCode.Trim());
                        db.AddInParameter(oCommand, "Custodian", SqlDbType.Int, intCustodian);
                        db.AddInParameter(oCommand, "IsCustodian", SqlDbType.Bit, blnIsCustodian);
                        db.AddInParameter(oCommand, "IsNASD", SqlDbType.Bit, blnIsNASD);
                    }
                    else
                    {
                        db.AddInParameter(oCommand, "CsCsAcct#", SqlDbType.VarChar, "");
                        db.AddInParameter(oCommand, "CsCsReg#", SqlDbType.VarChar, "");
                        db.AddInParameter(oCommand, "NASDCsCsAcct", SqlDbType.VarChar, "");
                        db.AddInParameter(oCommand, "NASDCsCsReg", SqlDbType.VarChar, "");
                        db.AddInParameter(oCommand, "BoxLoad", SqlDbType.Char, "N");
                        db.AddInParameter(oCommand, "Agent", SqlDbType.VarChar, "");
                        db.AddInParameter(oCommand, "AgentComm", SqlDbType.Decimal, 0);
                        db.AddInParameter(oCommand, "Comm", SqlDbType.Real, 0);
                        db.AddInParameter(oCommand, "SellComm", SqlDbType.Real, 0);
                        db.AddInParameter(oCommand, "AgentInd", SqlDbType.Char, "N");
                        db.AddInParameter(oCommand, "ProductType", SqlDbType.VarChar, "");
                        db.AddInParameter(oCommand, "AccessCode", SqlDbType.VarChar, "");
                        db.AddInParameter(oCommand, "Custodian", SqlDbType.Int, 0);
                        db.AddInParameter(oCommand, "IsCustodian", SqlDbType.Bit, false);
                        db.AddInParameter(oCommand, "IsNASD", SqlDbType.Bit, false);
                    }
                    db.AddInParameter(oCommand, "TableType", SqlDbType.VarChar, "C");
                    db.ExecuteNonQuery(oCommand, transaction);
                    SqlCommand dbCommandCustomerDSC = oGeneralFunc.CustomerDirectCashSettlementAndNoStampDutyUpdateCommand
                        (strCustAId.Trim(), blnCustomerDirectCashSettlement, blnCustomerDirectCashSettlementNASD,
                        blnDoNotChargeStampDuty,datDCSSetupDate,datDCSSetupDateNASD);
                    db.ExecuteNonQuery(dbCommandCustomerDSC, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            enSaveStatus = SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Save and Return Command
        public SqlCommand SaveCommand(ProductType SaveProduct)
        {
            if (strCustAId.Trim() == "" || strProductCode.Trim() == "")
            {
                throw new Exception("Customer Number Or Product Code Cannot Be Blank");
            }
            if (!ChkTransNoExist())
            {
                throw new Exception("Product Code Does Not Exist");
            }
            if (strSaveType == "ADDS")
            {
                if (ChkCustomerAcctExist())
                {
                    throw new Exception("Account Already Exit For This Product");
                }
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand  oCommand = db.GetStoredProcCommand("ProductAcctAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch);
            
            if (SaveProduct == ProductType.StockBroking)
            {
                db.AddInParameter(oCommand, "CsCsAcct#", SqlDbType.VarChar, strCsCsAcct.Trim());
                db.AddInParameter(oCommand, "CsCsReg#", SqlDbType.VarChar, strCsCsReg.Trim());
                db.AddInParameter(oCommand, "NASDCsCsAcct", SqlDbType.VarChar, strNASDCsCsAcct.Trim());
                db.AddInParameter(oCommand, "NASDCsCsReg", SqlDbType.VarChar, strNASDCsCsReg.Trim());
                db.AddInParameter(oCommand, "BoxLoad", SqlDbType.Char, strBoxLoad.Trim());
                db.AddInParameter(oCommand, "Agent", SqlDbType.VarChar, strAgent.Trim());
                db.AddInParameter(oCommand, "AgentComm", SqlDbType.Decimal, decAgentComm);
                db.AddInParameter(oCommand, "Comm", SqlDbType.Real, fltComm);
                db.AddInParameter(oCommand, "SellComm", SqlDbType.Real, fltSellCommission);
                db.AddInParameter(oCommand, "AgentInd", SqlDbType.Char, strAgentInd.Trim());
                db.AddInParameter(oCommand, "ProductType", SqlDbType.VarChar, "S");
                db.AddInParameter(oCommand, "AccessCode", SqlDbType.VarChar, strAccessCode.Trim());
                db.AddInParameter(oCommand, "Custodian", SqlDbType.Int, intCustodian);
                db.AddInParameter(oCommand, "IsCustodian", SqlDbType.Bit, blnIsCustodian);
                db.AddInParameter(oCommand, "IsNASD", SqlDbType.Bit, blnIsNASD);
                db.AddInParameter(oCommand, "ProcessCrossDeal", SqlDbType.Bit, blnProcessCrossDeal);
            }
            else
            {
                db.AddInParameter(oCommand, "CsCsAcct#", SqlDbType.VarChar, "");
                db.AddInParameter(oCommand, "CsCsReg#", SqlDbType.VarChar, "");
                db.AddInParameter(oCommand, "NASDCsCsAcct", SqlDbType.VarChar, "");
                db.AddInParameter(oCommand, "NASDCsCsReg", SqlDbType.VarChar, "");
                db.AddInParameter(oCommand, "BoxLoad", SqlDbType.Char, "N");
                db.AddInParameter(oCommand, "Agent", SqlDbType.VarChar, "");
                db.AddInParameter(oCommand, "AgentComm", SqlDbType.Decimal, 0);
                db.AddInParameter(oCommand, "Comm", SqlDbType.Real, 0);
                db.AddInParameter(oCommand, "SellComm", SqlDbType.Real, 0);
                db.AddInParameter(oCommand, "AgentInd", SqlDbType.Char, "N");
                db.AddInParameter(oCommand, "ProductType", SqlDbType.VarChar, "");
                db.AddInParameter(oCommand, "AccessCode", SqlDbType.VarChar, "");
                db.AddInParameter(oCommand, "Custodian", SqlDbType.Int, 0);
                db.AddInParameter(oCommand, "IsCustodian", SqlDbType.Bit, false);
                db.AddInParameter(oCommand, "IsNASD", SqlDbType.Bit, false);
                db.AddInParameter(oCommand, "ProcessCrossDeal", SqlDbType.Bit, false);
            }
            db.AddInParameter(oCommand, "TableType", SqlDbType.VarChar, "C");
            return oCommand;
        }
        #endregion

        #region Save Other Business and Return Command
        public SqlCommand SaveOtherBusinessCommand()
        {
            if (strCustAId.Trim() == "" || strProductCode.Trim() == "")
            {
                throw new Exception("Reference Number Or Account Id Cannot Be Blank");
            }
            if (!ChkTransNoExist())
            {
                throw new Exception("Product Code Does Not Exist");
            }
            if (strSaveType == "ADDS")
            {
                if (ChkCustomerAcctExist())
                {
                    throw new Exception("Reference Already Exit For This Account");
                }
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctOtherBusinessAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "Branch", SqlDbType.VarChar, strBranch);
            return oCommand;
        }
        #endregion



        #region Update CSCS Account and Return Command
        public SqlCommand UpDateCsCsAcctCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand  oCommand = db.GetStoredProcCommand("ProductAcctUpdateCSCSAcct") as SqlCommand;
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
            db.AddInParameter(oCommand, "CsCsAcct#", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(oCommand, "CsCsReg#", SqlDbType.VarChar, strCsCsReg.Trim());
            return oCommand;
        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkTransNoExist") as SqlCommand;
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

        #region Check CsCs Account Exist
        public bool ChkCsCsAcctExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkCsCsAcctExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "CsCsAcct", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (strCsCsAcct.Trim() == "" || strCsCsAcct == null)
            {
                blnStatus = false;
            }
            else
            {
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check NASD CsCs Account Exist
        public bool ChkNASDCsCsAcctExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctNASDChkCsCsAcctExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "NASDCsCsAcct", SqlDbType.VarChar, strNASDCsCsAcct.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (strNASDCsCsAcct.Trim() == "" || strNASDCsCsAcct == null)
            {
                blnStatus = false;
            }
            else
            {
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check CsCs Reg Exist
        public bool ChkCsCsRegExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkCsCsRegExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (strCsCsReg.Trim() == "" || strCsCsReg == null)
            {
                blnStatus = false;
            }
            else
            {
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check Access Code Exist
        public bool ChkAccessCodeExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkAccessCodeExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "AccessCode", SqlDbType.VarChar, strAccessCode.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check Investment Exist
        public bool ChkInvestmentExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                ProductAcct oChkCust = new ProductAcct();
                oChkCust.ProductCode = strProductCodeInvestment;
                oChkCust.CustAID = CustAID;
                if (oChkCust.ChkCustomerAcctExist())
                {
                    blnStatus = true;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check Agent Exist
        public bool ChkAgentExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                ProductAcct oChkCust = new ProductAcct();
                oChkCust.ProductCode = strProductCodeAgent;
                oChkCust.CustAID = CustAID;
                if (oChkCust.ChkCustomerAcctExist())
                {
                    blnStatus = true;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check Agent Attached Exist
        public bool ChkAgentAttachedExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelect") as SqlCommand;
                db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                DataTable thisTable = oDS.Tables[0];
                DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 1)
                {
                    if (thisRow[0]["Agent"] != null && thisRow[0]["Agent"].ToString().Trim() != "")
                    {
                        blnStatus = true;
                    }
                    if (thisRow[0]["AgentComm"] != null && thisRow[0]["AgentComm"].ToString().Trim() != "0" && thisRow[0]["AgentComm"].ToString().Trim() != "")
                    {
                        blnStatus = true;
                    }
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check Agent Customer Exist
        public bool ChkAgentCustomerExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctChkAgentCustomerExist") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "CustId", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["Agent"] != null && thisRow[0]["Agent"].ToString().Trim() != "")
                {
                    blnStatus = true;
                }
            }
            return blnStatus;
        }
        #endregion

        #region Check Agent Account Status
        public bool ChkAgentAcctGLStatus()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                Product oProduct = new Product();
                AcctGL oGL = new AcctGL();
                oProduct.TransNo = strProductCodeAgent;
                oGL.MasterID = oProduct.GetProductGLAcct();
                oGL.AccountID = strCustAId;
                if (oGL.CheckTransExistCustomer())
                {
                    blnStatus = true;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check Box Load Account Status
        public bool ChkBoxLoadGLStatus()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                Product oProduct = new Product();
                AcctGL oGL = new AcctGL();
                oProduct.TransNo = strProductCodeInvestment;
                oGL.MasterID = oProduct.GetProductGLAcct();
                oGL.AccountID = strCustAId;
                if (oGL.CheckTransExistCustomer())
                {
                    blnStatus = true;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion


        #region Get All Dormant Account
        public DataSet GetAllDormantAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllDormantAccount") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        //Only For Stock Market If StockBroking has a Product Class Will Remove The Two Going Forward

        #region Get All By Module
        public DataSet GetAllByModule(string strModuleName, string strExcludeProduct)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByModule") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            db.AddInParameter(dbCommand, "ExcludeProduct", SqlDbType.VarChar, strExcludeProduct.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Module - Search
        public DataSet GetAllByModuleSearch(string strModuleName, string strExcludeProduct, string strCustSurname, string strCustFirstname, string strCustOthername, string strCustAllName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByModuleSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            db.AddInParameter(dbCommand, "ExcludeProduct", SqlDbType.VarChar, strExcludeProduct.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Module Not Created
        public DataSet GetAllByModuleNotCreated(string strModuleName, string strExcludeProduct, string strProductNotCreated)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByModuleNotCreated") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            db.AddInParameter(dbCommand, "ExcludeProduct", SqlDbType.VarChar, strExcludeProduct.Trim());
            db.AddInParameter(dbCommand, "ProductNotCreated", SqlDbType.VarChar, strProductNotCreated.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Module - Search
        public DataSet GetAllByModuleNotCreatedSearch(string strModuleName, string strExcludeProduct, string strCustSurname, string strCustFirstname, string strCustOthername,string strProductNotCreated,string strCustAllName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByModuleNotCreatedSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            db.AddInParameter(dbCommand, "ExcludeProduct", SqlDbType.VarChar, strExcludeProduct.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "ProductNotCreated", SqlDbType.VarChar, strProductNotCreated.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Get All By Module And Product Class
        public DataSet GetAllByModuleClass(string strModuleName, string strProductClass)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByModuleClass") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            db.AddInParameter(dbCommand, "ProductClass", SqlDbType.VarChar, strProductClass.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Treasury Product
        public DataSet GetAllByTreasuryProduct()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByTreasuryProduct") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Module And Product Class - Search
        public DataSet GetAllByModuleClassSearch(string strModuleName, string strProductClass, string strCustSurname, string strCustFirstname, string strCustOthername, string strCustAllName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByModuleClassSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "ModuleName", SqlDbType.VarChar, strModuleName.Trim());
            db.AddInParameter(dbCommand, "ProductClass", SqlDbType.VarChar, strProductClass.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Treasury Product Search
        public DataSet GetAllByTreasuryProductSearch(string strCustSurname, string strCustFirstname, string strCustOthername, string strCustAllName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByTreasuryProductSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Product
        public DataSet GetAllByProduct(string strProduct,string strBranchCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null; 
            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProduct") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranchCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Set Product Account Dormant
        public void SetProductAccountDormant(string strProduct, string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ProductAcctSetAccountDormant") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustomerNumber.Trim());
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Reactivate Product Account Dormant
        public void ReactivateProductAccountDormant(string strProduct, string strCustomerNumber,DateTime datReactivateDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ProductAcctReactivateAccountDormant") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustomerNumber.Trim());
            db.AddInParameter(dbCommand, "ReactivateDate", SqlDbType.VarChar, datReactivateDate);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Get All Product By Customer
        public DataSet GetAllProductByCustomer(string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllProductByCustomerId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustomerNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Product Employee
        public DataSet GetAllByProductEmployee(string strProduct, string strBranchCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProductEmployee") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranchCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Product Desc
        public DataSet GetAllByProductDesc(string strProduct)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProductDesc") as SqlCommand;

            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Customer By Range By Product
        public DataSet GetAllCustomerRangeByProduct(string strCustomerTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllCustomerRangeByProduct") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,strProductCode.Trim());
            db.AddInParameter(dbCommand, "CustAIDFrom", SqlDbType.VarChar,strCustAId.Trim());
            db.AddInParameter(dbCommand, "CustAIDTo", SqlDbType.VarChar, strCustomerTo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Product With GL Transaction
        public DataSet GetAllByProductWithGLTransaction(string strProduct)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProductWithGLTransaction") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Agent
        public DataSet GetAllByAgent(string strAgentParam)
        {
      
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByAgent") as SqlCommand;

            db.AddInParameter(dbCommand, "Agent", SqlDbType.VarChar, strAgentParam.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Agent Few Column
        public DataSet GetAllByAgentFewColumn(string strAgentParam)
        {

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByAgentFewColumn") as SqlCommand;
            db.AddInParameter(dbCommand, "Agent", SqlDbType.VarChar, strAgentParam.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Deactivated Account By Product
        public DataSet GetAllDeactiveByProduct(string strProduct)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            
            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllDeactiveByProduct") as SqlCommand;
            
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Activated Account By Product
        public DataSet GetAllActiveByProduct(string strProduct)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllActiveByProduct") as SqlCommand;

            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Product Desc
        public string GetFirstCustForAllByProduct(string strProduct)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("ProductAcctSelectFirstCustomerByProduct") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            var varCustomerAcct = db.ExecuteScalar(dbCommand);
            return varCustomerAcct != null ? varCustomerAcct.ToString() : "";
        }
        #endregion

        #region Check Customer Account Exist
        public bool ChkCustomerAcctExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkCustAcctExist") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
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

        #region Check Process Cross Exist
        public bool ChkProcessCrossDeal()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkProcessCrossDeal") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            var blnReturnProcessCrossDeal = db.ExecuteScalar(oCommand);
            return blnReturnProcessCrossDeal != null && blnReturnProcessCrossDeal.ToString().Trim() != "" ? bool.Parse(blnReturnProcessCrossDeal.ToString()) : false;
        }
        #endregion

        #region Check Customer Account Exist All Product
        public bool ChkCustomerAcctExistAllProduct()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkCustAcctExistAllProduct") as SqlCommand;
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
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

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get
        public bool GetProductAcct()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustAId = thisRow[0]["CustAID"].ToString();
                strProductCode = thisRow[0]["ProductCode"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                strCsCsAcct = thisRow[0]["CsCsAcct#"].ToString();
                strCsCsReg = thisRow[0]["CsCsReg#"].ToString();
                strNASDCsCsAcct = thisRow[0]["NASDCsCsAcct"].ToString();
                strNASDCsCsReg = thisRow[0]["NASDCsCsReg"].ToString();
                
                if (thisRow[0]["Comm"].ToString() == "" || thisRow[0]["Comm"] == null)
                {
                    fltComm = null;
                }
                else
                {
                    fltComm = float.Parse(thisRow[0]["Comm"].ToString());
                }
                if (thisRow[0]["SellCommission"].ToString() == "" || thisRow[0]["SellCommission"] == null)
                {
                    fltSellCommission = null;
                }
                else
                {
                    fltSellCommission = float.Parse(thisRow[0]["SellCommission"].ToString());
                }
                if (thisRow[0]["AgentComm"].ToString() == "" || thisRow[0]["AgentComm"] == null)
                {
                    decAgentComm = 0;
                }
                else
                {
                    decAgentComm = decimal.Parse(thisRow[0]["AgentComm"].ToString());
                }
                strAgent = thisRow[0]["Agent"] != null ? thisRow[0]["Agent"].ToString() : "";
                strBoxLoad = thisRow[0]["BoxLoad"] != null ? thisRow[0]["BoxLoad"].ToString() : "";
                strAccessCode = thisRow[0]["AccessCode"] != null ? thisRow[0]["AccessCode"].ToString() : "";
                intCustodian = thisRow[0]["Custodian"] != null && thisRow[0]["Custodian"].ToString().Trim() != "" ? int.Parse(thisRow[0]["Custodian"].ToString()) : 0;
                blnIsCustodian = thisRow[0]["IsCustodian"] != null && thisRow[0]["IsCustodian"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["IsCustodian"].ToString()) : false;
                blnIsNASD = thisRow[0]["IsNASD"] != null && thisRow[0]["IsNASD"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["IsNASD"].ToString()) : false;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Direct Cash Settlement And Do Not Charge Stamp Duty From Customer Extra Info
        public void GetDirectCashAndNoStampDuty(string strCustomerNumber)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerExtraInformationSelectDirectCashAndNoStampDuty") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerNumber", SqlDbType.VarChar, strCustomerNumber.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                blnCustomerDirectCashSettlement = thisRow[0]["DirectCashSettlement"] != null && thisRow[0]["DirectCashSettlement"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DirectCashSettlement"].ToString()) : false;
                blnCustomerDirectCashSettlementNASD = thisRow[0]["DirectCashSettlementNASD"] != null && thisRow[0]["DirectCashSettlementNASD"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DirectCashSettlementNASD"].ToString()) : false;
                blnDoNotChargeStampDuty = thisRow[0]["DoNotChargeStampDuty"] != null && thisRow[0]["DoNotChargeStampDuty"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["DoNotChargeStampDuty"].ToString()) : false;
                datDCSSetupDate = thisRow[0]["DCSSetupDate"] != null && thisRow[0]["DCSSetupDate"].ToString().Trim() != ""  ? DateTime.Parse(thisRow[0]["DCSSetupDate"].ToString()) : DateTime.MinValue;
                datDCSSetupDateNASD = thisRow[0]["DCSSetupDateNASD"] != null && thisRow[0]["DCSSetupDateNASD"].ToString().Trim() != ""  ? DateTime.Parse(thisRow[0]["DCSSetupDateNASD"].ToString()) : DateTime.MinValue;
            }
            else
            {
                blnCustomerDirectCashSettlement = false;
                blnCustomerDirectCashSettlementNASD = false;
                blnDoNotChargeStampDuty = false;
                datDCSSetupDate = DateTime.MinValue;
                datDCSSetupDateNASD = DateTime.MinValue;
            }
        }
        #endregion

        #region Get Customer By Customer No
        public bool GetCustomerByCustId()
        {
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectByCustIdProductCode") as SqlCommand;
            db.AddInParameter(dbCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strCsCsAcct = thisRow[0]["CsCsAcct#"].ToString();
                strCsCsReg = thisRow[0]["CsCsReg#"].ToString();
                strNASDCsCsAcct = thisRow[0]["NASDCsCsAcct"].ToString();
                strNASDCsCsReg = thisRow[0]["NASDCsCsReg"].ToString();
                if (thisRow[0]["Comm"].ToString() == "" || thisRow[0]["Comm"] == null)
                {
                    fltComm = null;
                }
                else
                {
                    fltComm = float.Parse(thisRow[0]["Comm"].ToString());
                }
                if (thisRow[0]["SellCommission"].ToString() == "" || thisRow[0]["SellCommission"] == null)
                {
                    fltSellCommission = null;
                }
                else
                {
                    fltSellCommission = float.Parse(thisRow[0]["SellCommission"].ToString());
                }
                if (thisRow[0]["AgentComm"].ToString() == "" || thisRow[0]["AgentComm"] == null)
                {
                    decAgentComm = 0;
                }
                else
                {
                    decAgentComm = decimal.Parse(thisRow[0]["AgentComm"].ToString());
                }
                strAgent = thisRow[0]["Agent"] != null ? thisRow[0]["Agent"].ToString() : "";
                strBoxLoad = thisRow[0]["BoxLoad"] != null ? thisRow[0]["BoxLoad"].ToString() : "";
                strAccessCode = thisRow[0]["AccessCode"] != null ? thisRow[0]["AccessCode"].ToString() : "";
                intCustodian = int.Parse(thisRow[0]["Custodian"] != null && thisRow[0]["Custodian"].ToString().Trim() != "" ? thisRow[0]["Custodian"].ToString() : "0");
                blnIsCustodian = bool.Parse(thisRow[0]["IsCustodian"] != null && thisRow[0]["IsCustodian"].ToString().Trim() != "" ? thisRow[0]["IsCustodian"].ToString() : "false");
                blnIsNASD = thisRow[0]["IsNASD"] != null && thisRow[0]["IsNASD"].ToString().Trim() != "" ? bool.Parse(thisRow[0]["IsNASD"].ToString()) : false;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get All By Product - Search
        public DataSet GetAllByProductSearch(string strProduct, string strCustSurname, string strCustFirstname, string strCustOthername,string strOnlyNameFirstLetter,string strCustAllName)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOnlyNameFirstLetter.Trim() == "N")
            {
                dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProductSearch") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProductSearchOnlyNameFirstLetter") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Product - Search - Employee
        public DataSet GetAllByProductSearchEmployee(string strProduct, string strCustSurname, string strCustFirstname, string strCustOthername, string strOnlyNameFirstLetter, string strCustAllName)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strOnlyNameFirstLetter.Trim() == "N")
            {
                dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProductSearchEmployee") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllByProductSearchOnlyNameFirstLetterEmployee") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Deactivated Account By Product - Search
        public DataSet GetAllDeactiveByProductSearch(string strProduct, string strCustSurname, string strCustFirstname, string strCustOthername,string strCustAllName)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllDeactiveByProductSearch") as SqlCommand;
            
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Activated Account By Product - Search
        public DataSet GetAllActiveByProductSearch(string strProduct, string strCustSurname, string strCustFirstname, string strCustOthername,string strCustAllName)
        {
            Product oProduct = new Product();
            oProduct.TransNo = strProduct;
            if (!oProduct.GetProduct())
            {
                throw new Exception("Product Does Not Exist");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;

            dbCommand = db.GetStoredProcCommand("ProductAcctSelectAllActiveByProductSearch") as SqlCommand;

            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strCustSurname.Trim());
            db.AddInParameter(dbCommand, "CsCsAccount", SqlDbType.VarChar, strCsCsAcct.Trim());
            db.AddInParameter(dbCommand, "CsCsReg", SqlDbType.VarChar, strCsCsReg.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strCustOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strCustFirstname.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strCustAllName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Deactivate Account And Return Command
        public SqlCommand DeactivateAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctDeactivateAccount") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "ExcludeAccountInReport", SqlDbType.Char, strExcludeAccountInReport.Trim());
            return oCommand;
        }
        #endregion

        #region Check Exclude Account In Report
        public string ChkExcludeAccountInReport()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctChkExcludeAccountInReport") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            var varExcludeInAccount = db.ExecuteScalar(oCommand);
            return varExcludeInAccount != null ? varExcludeInAccount.ToString() : "" ;
        }
        #endregion
        
        #region Activate Account
        public void ActivateAccount()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctActivateAccount") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get All Transactions By Non Upload Online
        public DataSet GetAllByNonUploadOnline()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectByNonUploadOnline") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete By Product Code And Customer Id Return Command
        public SqlCommand DeleteByProductIdAndCustomerIdCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctDeleteByProductIdAndCustomerId") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            return oCommand;
        }
        #endregion

        #region Check Other ProductCode Exist
        public bool ChkOtherProductCodeExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("ProductAcctSelectOtherProductCode") as SqlCommand;
            db.AddInParameter(oCommand, "ProductCode", SqlDbType.Char, strProductCode.Trim());
            db.AddInParameter(oCommand, "CustAID", SqlDbType.VarChar, strCustAId.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion
        
        
        //Capital Market Details

        #region Get CSCS Numbers
        public bool GetCSCSNo()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctGetCSCSNo") as SqlCommand;
            db.AddInParameter(dbCommand, "CustId", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCsCsAcct = thisRow[0]["CsCsAcct#"] != null ? thisRow[0]["CsCsAcct#"].ToString() : "";
                strCsCsReg = thisRow[0]["CsCsReg#"] != null ? thisRow[0]["CsCsReg#"].ToString() : "";
                strNASDCsCsAcct = thisRow[0]["NASDCsCsAcct"] != null ? thisRow[0]["NASDCsCsAcct"].ToString() : "";
                strNASDCsCsReg = thisRow[0]["NASDCsCsReg"] != null ? thisRow[0]["NASDCsCsReg"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Customer No Given The CsCs Account Number
        public string GetCustNoGivenCsCsAcct()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectByCsCsAcctReturnCustAID", strCsCsAcct.Trim(), strProductCode.Trim()) as SqlCommand;
            var varCustomerNumber = db.ExecuteScalar(dbCommand);
            return varCustomerNumber != null ? varCustomerNumber.ToString() : "";
        }
        #endregion

        #region Get Customer No Given The CsCs Account Number For Margin
        public string GetCustNoGivenCsCsAcctForMargin(string strCSCSAccountForMargin)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectByCsCsAcctForMarginReturnCustAID", strCSCSAccountForMargin.Trim(), strProductCode.Trim()) as SqlCommand;
            var varReturnCustomerNumber = db.ExecuteScalar(dbCommand);
            return varReturnCustomerNumber != null ? varReturnCustomerNumber.ToString().Trim() : "";
        }
        #endregion

        #region Get CsCs Account Given The CsCs Account Number For Margin
        public string GetCsCsAccountGivenCsCsAcctForMargin(string strCSCSAccountForMargin)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectByCsCsAcctForMarginReturnCsCsAccount", strCSCSAccountForMargin.Trim(), strProductCode.Trim()) as SqlCommand;
            var varReturnCustomerNumber = db.ExecuteScalar(dbCommand);
            return varReturnCustomerNumber != null ? varReturnCustomerNumber.ToString().Trim() : "";
        }
        #endregion

        #region Get Customer No Given The NASD Account Number
        public string GetCustNoGivenNASDAcct()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectByNASDAcctReturnCustAID", strNASDCsCsAcct.Trim(), strProductCode.Trim()) as SqlCommand;
            var varCustomerNumber = db.ExecuteScalar(dbCommand);
            return varCustomerNumber != null ? varCustomerNumber.ToString() : "";
        }
        #endregion

        #region Get Customer No Given The Access Code
        public string GetCustNoGivenAccessCode()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctSelectByAccessCodeReturnCustAID", strAccessCode.Trim(), strProductCode.Trim()) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null)
            {
                return (string)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Check If Agent Account Has Customers Attached To It
        public bool ChkAgentHasCustomerAttached(string strAgentId,string strProduct)
        {
            bool blnStatus = true;
            if (strAgentId.Trim().Length > 0)
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctChkAgentCustAttached") as SqlCommand;
                db.AddInParameter(dbCommand, "AgentId", SqlDbType.VarChar, strAgentId.Trim());
                db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
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
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;

        }
        #endregion

        #region Check If Customer Has Agent Attached To It
        public bool ChkCustomerHasAgentAttached(string strCustomerId,string strProduct)
        {
            bool blnStatus = true;
            if (strCustomerId.Trim().Length > 0)
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctChkCustHasAgentAttached") as SqlCommand;
                db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustomerId.Trim());
                db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProduct.Trim());
                if (db.ExecuteScalar(dbCommand) == null || db.ExecuteScalar(dbCommand).ToString().Trim() == "")
                {
                    blnStatus = false;
                }
                else
                {
                    blnStatus = true;
                }
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Customer Agent
        public bool GetCustomerAgent()
        {
            bool blnStatus = false;
            string strReturnValue = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctGetAgent", strCustAId.Trim(),strProductCode.Trim()) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) == null || db.ExecuteScalar(dbCommand) == DBNull.Value)
            {
                blnStatus = false;
            }
            else
            {
                strReturnValue = (string)db.ExecuteScalar(dbCommand);
                if (strReturnValue == "")
                {
                    blnStatus = false;
                }
                else
                {
                    blnStatus = true;
                    strAgent = strReturnValue.Trim();
                }
            }
            return blnStatus;
        }
        #endregion

        #region Get Customer Agent Commission
        public bool GetAgentCommission()
        {
            bool blnStatus = false;
            decimal decReturnValue = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctAgentCommission", strCustAId.Trim(),strProductCode.Trim()) as SqlCommand;

            if (db.ExecuteScalar(dbCommand) == DBNull.Value || db.ExecuteScalar(dbCommand) == null)
            {
                blnStatus = false;
            }
            else
            {
                decReturnValue = (decimal)db.ExecuteScalar(dbCommand);
                if (decReturnValue == 0)
                {
                    blnStatus = false;
                }
                else
                {
                    blnStatus = true;
                    decAgentComm = decReturnValue;
                }

            }
            return blnStatus;
        }
        #endregion

        #region Get Box Load Status
        public bool GetBoxLoadStatus()
        {
            string strReturnValue = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctGetBoxLoadStatus", strCustAId.Trim(), strProductCode.Trim()) as SqlCommand;
            var varBoxLoad = db.ExecuteScalar(dbCommand);
            strReturnValue = varBoxLoad != null && varBoxLoad.ToString().Trim() != "" ? varBoxLoad.ToString() : "N";
            return strReturnValue.Trim() == "Y" ? true : false;
        }
        #endregion

        #region Account WithOut CSCS AC Selected By Contact Date - CSCS Format
        public DataSet GetAccountWithOutCSCSACByDateCSCSFormat(DateTime datOpeningDate, DateTime datClosingDate,string strMemCode,string strStockProductCode)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctStockAcctWithOutCSCSACByDateCSCSFormat") as SqlCommand;
            db.AddInParameter(dbCommand, "ODate", SqlDbType.DateTime, datOpeningDate);
            db.AddInParameter(dbCommand, "CDate", SqlDbType.DateTime, datClosingDate);
            db.AddInParameter(dbCommand, "MemberCode", SqlDbType.VarChar, strMemCode.Trim());
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strStockProductCode.Trim());

            DataSet ds = db.ExecuteDataSet(dbCommand);
            return ds;
        }
        #endregion

        #region Remove Agent Return Command
        public SqlCommand RemoveAgentCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctRemoveAgent") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Transfer Agent Return Command
        public SqlCommand TransferAgentCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctTransferAgent") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
            db.AddInParameter(dbCommand, "CustAId", SqlDbType.VarChar, strCustAId.Trim());
            db.AddInParameter(dbCommand, "Agent", SqlDbType.VarChar, strAgent.Trim());
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        //End Capital Market Details


    }
}
