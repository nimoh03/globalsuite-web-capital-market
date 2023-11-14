using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class CustMargin
    {
        #region Declarations
        private string strTransNo, strSurname,	strFirstname, strOthername, strClientType, strCustAID;
        private string strStockCode, strQty, strUnitPrice, strCSCSTrans, strStockType, strCommissionType;
        private string strGraduatedCommission, strConsideration, strSecFee, strStampDuty, strCommission;
        private string strVAT, strNSEFee, strCSCSFee, strTotalAmt, strPM_ID, strCrossType;
        private string strCsCsRegNumber, strCscsAcctNumber, strCDeal, strCapGain, strAutoPost;
        private string strDR_ID, strDir_Sell, strJB_ID, strSCNPaid, strSCNAmt, strTicketNo, strSoldBy;
        private string strBoughtBy, strBuy_Sold_Ind, strCDSellTrans, strCDSellSurname, strCDSellFirstname;
        private string strCDSellOthername, strOtherCust, strPrintFlag, strChangePrice, strBranName, strSing_Mult_Ind;
        private string strAgentName, strStkRem, strUpdateInd, strBranch, strTransBranch, strSecVat;
        private string strNseVat, strCscsVat, strExcAgent;
        private bool blnPosted, blnReversed;
        private decimal decAgentComm;
        private DateTime datDateAlloted;
        private string strTxnDate, strTxnTime, strUserID;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string Surname
        {
            set { strSurname = value; }
            get { return strSurname; }
        }
        public string Firstname
        {
            set { strFirstname = value; }
            get { return strFirstname; }
        }
        public string Othername
        {
            set { strOthername = value; }
            get { return strOthername; }
        }
        public string ClientType
        {
            set { strClientType = value; }
            get { return strClientType; }
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
        public string Qty
        {
            set { strQty = value; }
            get { return strQty; }
        }
        public string UnitPrice
        {
            set { strUnitPrice = value; }
            get { return strUnitPrice; }
        }
        public string CSCSTrans
        {
            set { strCSCSTrans = value; }
            get { return strCSCSTrans; }
        }
        public string StockType
        {
            set { strStockType = value; }
            get { return strStockType; }
        }
        public string CommissionType
        {
            set { strCommissionType = value; }
            get { return strCommissionType; }
        }
        public string GraduatedCommission
        {
            set { strGraduatedCommission = value; }
            get { return strGraduatedCommission; }
        }
        public string Consideration
        {
            set { strConsideration = value; }
            get { return strConsideration; }
        }
        public string SecFee
        {
            set { strSecFee = value; }
            get { return strSecFee; }
        }
        public string StampDuty
        {
            set { strStampDuty = value; }
            get { return strStampDuty; }
        }
        public string Commission
        {
            set { strCommission = value; }
            get { return strCommission; }
        }
        public string VAT
        {
            set { strVAT = value; }
            get { return strVAT; }
        }
        public string NSEFee
        {
            set { strNSEFee = value; }
            get { return strNSEFee; }
        }
        public string CSCSFee
        {
            set { strCSCSFee = value; }
            get { return strCSCSFee; }
        }
        public string TotalAmt
        {
            set { strTotalAmt = value; }
            get { return strTotalAmt; }
        }
        public string PM_ID
        {
            set { strPM_ID = value; }
            get { return strPM_ID; }
        }
        public string CrossType
        {
            set { strCrossType = value; }
            get { return strCrossType; }
        }
        public string CsCsRegNumber
        {
            set { strCsCsRegNumber = value; }
            get { return strCsCsRegNumber; }
        }
        public string CscsAcctNumber
        {
            set { strCscsAcctNumber = value; }
            get { return strCscsAcctNumber; }
        }
        public string CDeal
        {
            set { strCDeal = value; }
            get { return strCDeal; }
        }
        public string CapGain
        {
            set { strCapGain = value; }
            get { return strCapGain; }
        }
        public string AutoPost
        {
            set { strAutoPost = value; }
            get { return strAutoPost; }
        }
        public string DR_ID
        {
            set { strDR_ID = value; }
            get { return strDR_ID; }
        }
        public string Dir_Sell
        {
            set { strDir_Sell = value; }
            get { return strDir_Sell; }
        }
        public string JB_ID
        {
            set { strJB_ID = value; }
            get { return strJB_ID; }
        }
        public string SCNPaid
        {
            set { strSCNPaid = value; }
            get { return strSCNPaid; }
        }
        public string SCNAmt
        {
            set { strSCNAmt = value; }
            get { return strSCNAmt; }
        }
        public string TicketNo
        {
            set { strTicketNo = value; }
            get { return strTicketNo; }
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
        public string Buy_Sold_Ind
        {
            set { strBuy_Sold_Ind = value; }
            get { return strBuy_Sold_Ind; }
        }
        public string CDSellTrans
        {
            set { strCDSellTrans = value; }
            get { return strCDSellTrans; }
        }
        public string CDSellSurname
        {
            set { strCDSellSurname = value; }
            get { return strCDSellSurname; }
        }
        public string CDSellFirstname
        {
            set { strCDSellFirstname = value; }
            get { return strCDSellFirstname; }
        }
        public string CDSellOthername
        {
            set { strCDSellOthername = value; }
            get { return strCDSellOthername; }
        }
        public string OtherCust
        {
            set { strOtherCust = value; }
            get { return strOtherCust; }
        }
        public string PrintFlag
        {
            set { strPrintFlag = value; }
            get { return strPrintFlag; }
        }
        public string ChangePrice
        {
            set { strChangePrice = value; }
            get { return strChangePrice; }
        }
        public string BranName
        {
            set { strBranName = value; }
            get { return strBranName; }
        }
        public string Sing_Mult_Ind
        {
            set { strSing_Mult_Ind = value; }
            get { return strSing_Mult_Ind; }
        }
        public string AgentName
        {
            set { strAgentName = value; }
            get { return strAgentName; }
        }
        public string StkRem
        {
            set { strStkRem = value; }
            get { return strStkRem; }
        }
        public string UpdateInd
        {
            set { strUpdateInd = value; }
            get { return strUpdateInd; }
        }
        public string Branch
        {
            set { strBranch = value; }
            get { return strBranch; }
        }
        public string TransBranch
        {
            set { strTransBranch = value; }
            get { return strTransBranch; }
        }
        public string SecVat
        {
            set { strSecVat = value; }
            get { return strSecVat; }
        }
        public string NseVat
        {
            set { strNseVat = value; }
            get { return strNseVat; }
        }
        public string CscsVat
        {
            set { strCscsVat = value; }
            get { return strCscsVat; }
        }
        public string ExcAgent
        {
            set { strExcAgent = value; }
            get { return strExcAgent; }
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
        public decimal AgentComm
        {
            set { decAgentComm = value; }
            get { return decAgentComm; }
        }
        public DateTime DateAlloted
        {
            set { datDateAlloted = value; }
            get { return datDateAlloted; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public string TxnDate
        {
            set { strTxnDate = value; }
            get { return strTxnDate; }
        }
        public string TxnTime
        {
            set { strTxnTime = value; }
            get { return strTxnTime; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            //if (!ChkTransNoExist())
            //{
            //    enSaveStatus = DataGeneral.SaveStatus.NotExist;
            //    return enSaveStatus;
            //}
            //if (ChkNameExist())
            //{
            //    enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
            //    return enSaveStatus;
            //}

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("Stkb_CustMarginAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("Stkb_CustMarginEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.ToUpper().Trim());
            db.AddInParameter(dbCommand, "Firstname", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(dbCommand, "Othername", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(dbCommand, "ClientType", SqlDbType.VarChar, strClientType.Trim());
            db.AddInParameter(dbCommand, "CustAID", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(dbCommand, "Qty", SqlDbType.VarChar, strQty.Trim());
            db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.VarChar, strUnitPrice.Trim());
            db.AddInParameter(dbCommand, "CSCSTrans", SqlDbType.VarChar, strCSCSTrans.Trim());
            db.AddInParameter(dbCommand, "StockType", SqlDbType.VarChar, strStockType.Trim());
            db.AddInParameter(dbCommand, "CommissionType", SqlDbType.VarChar, strCommissionType.Trim());
            db.AddInParameter(dbCommand, "GraduatedCommission", SqlDbType.VarChar, strGraduatedCommission.Trim());
            db.AddInParameter(dbCommand, "Consideration", SqlDbType.VarChar, strConsideration.Trim());
            db.AddInParameter(dbCommand, "SecFee", SqlDbType.VarChar, strSecFee.Trim());
            db.AddInParameter(dbCommand, "StampDuty", SqlDbType.VarChar, strStampDuty.Trim());
            db.AddInParameter(dbCommand, "Commission", SqlDbType.VarChar, strCommission.Trim());
            db.AddInParameter(dbCommand, "VAT", SqlDbType.VarChar, strVAT.Trim());
            db.AddInParameter(dbCommand, "NSEFee", SqlDbType.VarChar, strNSEFee.Trim());
            db.AddInParameter(dbCommand, "CSCSFee", SqlDbType.VarChar, strCSCSFee.Trim());
            db.AddInParameter(dbCommand, "TotalAmt", SqlDbType.VarChar, strTotalAmt.Trim());
            db.AddInParameter(dbCommand, "PM_ID", SqlDbType.VarChar, strPM_ID.Trim());
            db.AddInParameter(dbCommand, "CrossType", SqlDbType.VarChar, strCrossType.Trim());
            db.AddInParameter(dbCommand, "CsCsRegNumber", SqlDbType.VarChar, strCsCsRegNumber.Trim());
            db.AddInParameter(dbCommand, "CscsAcctNumber", SqlDbType.VarChar, strCscsAcctNumber.Trim());
            db.AddInParameter(dbCommand, "CDeal", SqlDbType.VarChar, strCDeal.Trim());
            db.AddInParameter(dbCommand, "CapGain", SqlDbType.VarChar, strCapGain.Trim());
            db.AddInParameter(dbCommand, "AutoPost", SqlDbType.VarChar, strAutoPost.Trim());
            db.AddInParameter(dbCommand, "DR_ID", SqlDbType.VarChar, strDR_ID.Trim());
            db.AddInParameter(dbCommand, "Dir_Sell", SqlDbType.VarChar, strDir_Sell.Trim());
            db.AddInParameter(dbCommand, "JB_ID", SqlDbType.VarChar, strJB_ID.Trim());
            db.AddInParameter(dbCommand, "SCNPaid", SqlDbType.VarChar, strSCNPaid.Trim());
            db.AddInParameter(dbCommand, "SCNAmt", SqlDbType.VarChar, strSCNAmt.Trim());
            db.AddInParameter(dbCommand, "TicketNo", SqlDbType.VarChar, strTicketNo.Trim());
            db.AddInParameter(dbCommand, "SoldBy", SqlDbType.VarChar, strSoldBy.Trim());
            db.AddInParameter(dbCommand, "BoughtBy", SqlDbType.VarChar, strBoughtBy.Trim());
            db.AddInParameter(dbCommand, "Buy_Sold_Ind", SqlDbType.VarChar, strBuy_Sold_Ind.Trim());
            db.AddInParameter(dbCommand, "CDSellTrans", SqlDbType.VarChar, strCDSellTrans.Trim());
            db.AddInParameter(dbCommand, "CDSellSurname", SqlDbType.VarChar, strCDSellSurname.Trim());
            db.AddInParameter(dbCommand, "CDSellFirstname", SqlDbType.VarChar, strCDSellFirstname.Trim());
            db.AddInParameter(dbCommand, "CDSellOthername", SqlDbType.VarChar, strCDSellOthername.Trim());
            db.AddInParameter(dbCommand, "OtherCust", SqlDbType.VarChar, strOtherCust.Trim());
            db.AddInParameter(dbCommand, "PrintFlag", SqlDbType.VarChar, strPrintFlag.Trim());
            db.AddInParameter(dbCommand, "ChangePrice", SqlDbType.VarChar, strChangePrice.Trim());
            db.AddInParameter(dbCommand, "BranName", SqlDbType.VarChar, strBranName.Trim());
            db.AddInParameter(dbCommand, "Sing_Mult_Ind", SqlDbType.VarChar, strSing_Mult_Ind.Trim());
            db.AddInParameter(dbCommand, "AgentName", SqlDbType.VarChar, strAgentName.Trim());
            db.AddInParameter(dbCommand, "StkRem", SqlDbType.VarChar, strStkRem.Trim());
            db.AddInParameter(dbCommand, "UpdateInd", SqlDbType.VarChar, strUpdateInd.Trim());
            db.AddInParameter(dbCommand, "Branch", SqlDbType.VarChar, strBranch.Trim());
            db.AddInParameter(dbCommand, "TransBranch", SqlDbType.VarChar, strTransBranch.Trim());
            db.AddInParameter(dbCommand, "SecVat", SqlDbType.VarChar, strSecVat.Trim());
            db.AddInParameter(dbCommand, "NseVat", SqlDbType.VarChar, strNseVat.Trim());
            db.AddInParameter(dbCommand, "CscsVat", SqlDbType.VarChar, strCscsVat.Trim());
            db.AddInParameter(dbCommand, "ExcAgent", SqlDbType.VarChar, strExcAgent.Trim());
            db.AddInParameter(dbCommand, "Posted", SqlDbType.Bit, blnPosted);
            db.AddInParameter(dbCommand, "Reversed", SqlDbType.Bit, blnReversed);
            db.AddInParameter(dbCommand, "AgentComm", SqlDbType.Decimal, decAgentComm);
            if (datDateAlloted != DateTime.MinValue)
            {
                db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, datDateAlloted);

            }
            else
            {
                db.AddInParameter(dbCommand, "DateAlloted", SqlDbType.DateTime, SqlDateTime.Null);

            }
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("Stkb_CustMarginChkTransNoExist") as SqlCommand;
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

        #region Get CustMargin
        public bool GetCustMargin()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("Stkb_CustMarginSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strSurname = thisRow[0]["Surname"].ToString();
                strFirstname = thisRow[0]["Firstname"].ToString();
                strOthername = thisRow[0]["Othername"].ToString();
                strClientType = thisRow[0]["ClientType"].ToString();
                strCustAID = thisRow[0]["CustAID"].ToString();
                strStockCode = thisRow[0]["StockCode"].ToString();
                strQty = thisRow[0]["Qty"].ToString();
                strUnitPrice = thisRow[0]["UnitPrice"].ToString();
                strCSCSTrans = thisRow[0]["CSCSTrans"].ToString();
                strStockType = thisRow[0]["StockType"].ToString();
                strCommissionType = thisRow[0]["CommissionType"].ToString();
                strGraduatedCommission = thisRow[0]["GraduatedCommission"].ToString();
                strConsideration = thisRow[0]["Consideration"].ToString();
                strSecFee = thisRow[0]["SecFee"].ToString();
                strStampDuty = thisRow[0]["StampDuty"].ToString();
                strCommission = thisRow[0]["Commission"].ToString();
                strVAT = thisRow[0]["VAT"].ToString();
                strNSEFee = thisRow[0]["NSEFee"].ToString();
                strCSCSFee = thisRow[0]["CSCSFee"].ToString();
                strTotalAmt = thisRow[0]["TotalAmt"].ToString();
                strPM_ID = thisRow[0]["PM_ID"].ToString();
                strCrossType = thisRow[0]["CrossType"].ToString();
                strCsCsRegNumber = thisRow[0]["CsCsRegNumber"].ToString();
                strCscsAcctNumber = thisRow[0]["CscsAcctNumber"].ToString();
                strCDeal = thisRow[0]["CDeal"].ToString();
                strCapGain = thisRow[0]["CapGain"].ToString();
                strAutoPost = thisRow[0]["AutoPost"].ToString();
                strDR_ID = thisRow[0]["DR_ID"].ToString();
                strDir_Sell = thisRow[0]["Dir_Sell"].ToString();
                strJB_ID = thisRow[0]["JB_ID"].ToString();
                strSCNPaid = thisRow[0]["SCNPaid"].ToString();
                strSCNAmt = thisRow[0]["SCNAmt"].ToString();
                strTicketNo = thisRow[0]["TicketNo"].ToString();
                strSoldBy = thisRow[0]["SoldBy"].ToString();
                strBoughtBy = thisRow[0]["BoughtBy"].ToString();
                strBuy_Sold_Ind = thisRow[0]["Buy_Sold_Ind"].ToString();
                strCDSellTrans = thisRow[0]["CDSellTrans"].ToString();
                strCDSellSurname = thisRow[0]["CDSellSurname"].ToString();
                strCDSellFirstname = thisRow[0]["CDSellFirstname"].ToString();
                strCDSellOthername = thisRow[0]["CDSellOthername"].ToString();
                strOtherCust = thisRow[0]["OtherCust"].ToString();
                strPrintFlag = thisRow[0]["PrintFlag"].ToString();
                strChangePrice = thisRow[0]["ChangePrice"].ToString();
                strBranName = thisRow[0]["BranName"].ToString();
                strSing_Mult_Ind = thisRow[0]["Sing_Mult_Ind"].ToString();
                strAgentName = thisRow[0]["AgentName"].ToString();
                strStkRem = thisRow[0]["StkRem"].ToString();
                strUpdateInd = thisRow[0]["UpdateInd"].ToString();
                strBranch = thisRow[0]["Branch"].ToString();
                strTransBranch = thisRow[0]["TransBranch"].ToString();
                strSecVat = thisRow[0]["SecVat"].ToString();
                strNseVat = thisRow[0]["NseVat"].ToString();
                strCscsVat = thisRow[0]["CscsVat"].ToString();
                strExcAgent = thisRow[0]["ExcAgent"].ToString();
                blnPosted = bool.Parse(thisRow[0]["Posted"].ToString());
                blnReversed = bool.Parse(thisRow[0]["Reversed"].ToString());
                decAgentComm = decimal.Parse(thisRow[0]["AgentComm"].ToString());
                if (thisRow[0]["DateAlloted"].ToString() == "" || thisRow[0]["DateAlloted"].ToString() == null)
                {
                    datDateAlloted = DateTime.MinValue;
                }
                else
                {
                    datDateAlloted = DateTime.ParseExact(thisRow[0]["DateAlloted"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                }
                strUserID = thisRow[0]["UserID"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("Stkb_CustMarginSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("Stkb_CustMarginDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
