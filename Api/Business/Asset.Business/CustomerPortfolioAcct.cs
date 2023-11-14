using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using BaseUtility.Business;

namespace Asset.Business
{
    public class CustomerPortfolioAcct
    {
        #region Declarations
        private string strTransNo, strPrimayPortProduct, strSubPortProduct;
        private string strCustAID, strAccountCode,strTenor,strAmtInvested;
        private string strRateOrInterest,strPortObjective,strRiskType,strSaveType;
        private DateTime datStartDate,datCreationDate;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string PrimayPortProduct
        {
            set { strPrimayPortProduct = value; }
            get { return strPrimayPortProduct; }
        }
        public string SubPortProduct
        {
            set { strSubPortProduct = value; }
            get { return strSubPortProduct; }
        }
        public string CustAID
        {
            set { strCustAID = value; }
            get { return strCustAID; }
        }
        public string AccountCode
        {
            set { strAccountCode = value; }
            get { return strAccountCode; }
        }
        public DateTime StartDate
        {
            set { datStartDate = value; }
            get { return datStartDate; }
        }
        public string Tenor
        {
            set { strTenor = value; }
            get { return strTenor; }
        }
        public string AmtInvested
        {
            set { strAmtInvested = value; }
            get { return strAmtInvested; }
        }
        public string RateOrInterest
        {
            set { strRateOrInterest = value; }
            get { return strRateOrInterest; }
        }
        public string PortObjective
        {
            set { strPortObjective = value; }
            get { return strPortObjective; }
        }
        public string RiskType
        {
            set { strRiskType = value; }
            get { return strRiskType; }
        }
        public DateTime CreationDate
        {
            set { datCreationDate = value; }
            get { return datCreationDate; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerPortfolioAcctSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Check Trans No Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("CustomerPortfolioAcctChkTransNoExist") as SqlCommand;
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
                oCommand = db.GetStoredProcCommand("CustomerPortfolioAcctAdd") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("CustomerPortfolioAcctEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "PortfolioProductCode", SqlDbType.VarChar, strPrimayPortProduct.Trim());
            db.AddInParameter(oCommand, "CustomerNumber", SqlDbType.VarChar, strCustAID.Trim());
            db.AddInParameter(oCommand, "ReferenceNumber", SqlDbType.VarChar, "1");
            db.AddInParameter(oCommand, "AmountInvested", SqlDbType.Money, Decimal.Parse(strAmtInvested.Trim()));
            db.AddInParameter(oCommand, "Tenor", SqlDbType.Int, int.Parse(strTenor.Trim()));
            db.AddInParameter(oCommand, "InvestmentStartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(oCommand, "InvestmentRate", SqlDbType.Real, float.Parse(strRateOrInterest.Trim()));
            db.AddInParameter(oCommand, "PortfolioObjective", SqlDbType.VarChar, strPortObjective.Trim());
            db.AddInParameter(oCommand, "RiskType", SqlDbType.VarChar, strRiskType.Trim());
            db.AddInParameter(oCommand, "CreationDate", SqlDbType.DateTime, datCreationDate);
            db.AddInParameter(oCommand, "AccountOnHold", SqlDbType.Bit, false);
            db.AddInParameter(oCommand, "Posted", SqlDbType.Bit, false);
            db.AddInParameter(oCommand, "Reversed", SqlDbType.Bit, false);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
