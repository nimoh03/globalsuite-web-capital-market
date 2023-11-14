using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class BondOffer
    {
        #region Declaration
        private long lngTransNo;
        private DateTime datEffDate;
        private int intTenureYear;
        private decimal decInterest,decUnitPrice, decMinimumSubAmount, decMaximumSubAmount;
        private string strBondName, strIssuer, strInterestPaymentCycle,strRedemption, strBondAccount;
        private DateTime datOpenDate, datCloseDate,datSettlementDate;
        private string strSaveType;
        #endregion

        #region Properties
        public long TransNo
        {
            set { lngTransNo = value; }
            get { return lngTransNo; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public string BondName
        {
            set { strBondName = value; }
            get { return strBondName; }
        }
        public int TenureYear
        {
            set { intTenureYear = value; }
            get { return intTenureYear; }
        }
        public decimal Interest
        {
            set { decInterest = value; }
            get { return decInterest; }
        }


        public decimal UnitPrice
        { 
            set { decUnitPrice = value; }
            get { return decUnitPrice; }
        }
        public decimal MinimumSubAmount
        {
            set { decMinimumSubAmount = value; }
            get { return decMinimumSubAmount; }
        }
        public decimal MaximumSubAmount
        {
            set { decMaximumSubAmount = value; }
            get { return decMaximumSubAmount; }
        }
        public DateTime OpenDate
        {
            set { datOpenDate = value; }
            get { return datOpenDate; }
        }

        public DateTime CloseDate
        {
            set { datCloseDate = value; }
            get { return datCloseDate; }
        }
        public DateTime SettlementDate
        {
            set { datSettlementDate = value; }
            get { return datSettlementDate; }
        }
        public string Issuer
        {
            set { strIssuer = value; }
            get { return strIssuer; }
        }
        public string InterestPaymentCycle
        {
            set { strInterestPaymentCycle = value; }
            get { return strInterestPaymentCycle; }
        }
        public string Redemption
        {
            set { strRedemption = value; }
            get { return strRedemption; }
        }
        public string BondAccount
        {
            set { strBondAccount = value; }
            get { return strBondAccount; }
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
            if (!ChkTransNoExist())
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
                        dbCommand = db.GetStoredProcCommand("BondOfferAddNew") as SqlCommand;
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("BondOfferEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
                    db.AddInParameter(dbCommand, "BondName", SqlDbType.VarChar, strBondName);
                    db.AddInParameter(dbCommand, "TenureYear", SqlDbType.Int, intTenureYear);
                    db.AddInParameter(dbCommand, "Interest", SqlDbType.Decimal, decInterest);
                    db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
                    db.AddInParameter(dbCommand, "MinimumSubAmount", SqlDbType.Decimal, decMinimumSubAmount);
                    db.AddInParameter(dbCommand, "MaximumSubAmount", SqlDbType.Decimal, decMaximumSubAmount);
                    db.AddInParameter(dbCommand, "OpenDate", SqlDbType.DateTime, datOpenDate);
                    db.AddInParameter(dbCommand, "CloseDate", SqlDbType.DateTime, datCloseDate);
                    db.AddInParameter(dbCommand, "SettlementDate", SqlDbType.DateTime, datSettlementDate);
                    db.AddInParameter(dbCommand, "Issuer", SqlDbType.VarChar, strIssuer);
                    db.AddInParameter(dbCommand, "InterestPaymentCycle", SqlDbType.VarChar, strInterestPaymentCycle);
                    db.AddInParameter(dbCommand, "Redemption", SqlDbType.VarChar, strRedemption);
                    db.AddInParameter(dbCommand, "BondAccount", SqlDbType.VarChar, strBondAccount);
                    db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
                    db.ExecuteNonQuery(dbCommand, transaction);
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
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = db.GetStoredProcCommand("BondOfferChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
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

        #region Get Bond Offer
        public bool GetBondOffer()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BondOfferSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                lngTransNo = long.Parse(thisRow[0]["TransNo"].ToString());
                strBondName = thisRow[0]["BondName"].ToString();
                intTenureYear = int.Parse(thisRow[0]["TenureYear"].ToString());
                decInterest = decimal.Parse(thisRow[0]["Interest"].ToString());
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                decMinimumSubAmount = decimal.Parse(thisRow[0]["MinimumSubAmount"].ToString());
                decMaximumSubAmount = decimal.Parse(thisRow[0]["MaximumSubAmount"].ToString());
                datOpenDate = DateTime.Parse(thisRow[0]["OpenDate"].ToString());
                datCloseDate = DateTime.Parse(thisRow[0]["CloseDate"].ToString());
                datSettlementDate = DateTime.Parse(thisRow[0]["SettlementDate"].ToString());
                strIssuer = thisRow[0]["Issuer"].ToString();
                strInterestPaymentCycle = thisRow[0]["InterestPaymentCycle"] != null ? thisRow[0]["InterestPaymentCycle"].ToString() : "";
                strRedemption = thisRow[0]["Redemption"].ToString();
                strBondAccount = thisRow[0]["BondAccount"].ToString();
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
            SqlCommand oCommand = db.GetStoredProcCommand("BondOfferDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BondOfferSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


    }
}
