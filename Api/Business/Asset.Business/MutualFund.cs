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

namespace Asset.Business
{
    public class MutualFund
    {
        #region Declaration
        private long lngTransNo;
        private DateTime datEffDate;
        private decimal decUnitPrice, decMinimumSubAmount, decMaximumSubAmount;
        private string strMutualFundName, strIssuer, strMutualFundAccount,strMutualFundType,strProductCode;
        private bool blnTradingOnNSE;
        private DateTime datOpenDate, datCloseDate, datSettlementDate;
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
        public string MutualFundName
        {
            set { strMutualFundName = value; }
            get { return strMutualFundName; }
        }
        public string MutualFundType
        {
            set { strMutualFundType = value; }
            get { return strMutualFundType; }
        }
        public bool TradingOnNSE
        {
            set { blnTradingOnNSE = value; }
            get { return blnTradingOnNSE; }
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
        public string MutualFundAccount
        {
            set { strMutualFundAccount = value; }
            get { return strMutualFundAccount; }
        }
        public string ProductCode
        {
            set { strProductCode = value; }
            get { return strProductCode; }
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
                        dbCommand = db.GetStoredProcCommand("MutualFundAddNew") as SqlCommand;
                    }
                    else if (strSaveType.Trim() == "EDIT")
                    {
                        dbCommand = db.GetStoredProcCommand("MutualFundEdit") as SqlCommand;
                    }
                    db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
                    db.AddInParameter(dbCommand, "MutualFundName", SqlDbType.VarChar, strMutualFundName);
                    db.AddInParameter(dbCommand, "UnitPrice", SqlDbType.Decimal, decUnitPrice);
                    db.AddInParameter(dbCommand, "MinimumSubAmount", SqlDbType.Decimal, decMinimumSubAmount);
                    db.AddInParameter(dbCommand, "MaximumSubAmount", SqlDbType.Decimal, decMaximumSubAmount);
                    db.AddInParameter(dbCommand, "OpenDate", SqlDbType.DateTime, datOpenDate);
                    db.AddInParameter(dbCommand, "CloseDate", SqlDbType.DateTime, datCloseDate);
                    db.AddInParameter(dbCommand, "SettlementDate", SqlDbType.DateTime, datSettlementDate);
                    db.AddInParameter(dbCommand, "Issuer", SqlDbType.VarChar, strIssuer);
                    db.AddInParameter(dbCommand, "MutualFundAccount", SqlDbType.VarChar, strMutualFundAccount);
                    db.AddInParameter(dbCommand, "MutualFundType", SqlDbType.VarChar, strMutualFundType.Trim());
                    db.AddInParameter(dbCommand, "TradingOnNSE", SqlDbType.Bit, blnTradingOnNSE);
                    db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, strProductCode.Trim());
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
                SqlCommand oCommand = db.GetStoredProcCommand("MutualFundChkTransNoExist") as SqlCommand;
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

        #region Get Mutual Fund
        public bool GetMutualFund()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");

            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MutualFundSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                lngTransNo = long.Parse(thisRow[0]["TransNo"].ToString());
                strMutualFundName = thisRow[0]["MutualFundName"].ToString();
                decUnitPrice = decimal.Parse(thisRow[0]["UnitPrice"].ToString());
                decMinimumSubAmount = decimal.Parse(thisRow[0]["MinimumSubAmount"].ToString());
                decMaximumSubAmount = decimal.Parse(thisRow[0]["MaximumSubAmount"].ToString());
                datOpenDate = DateTime.Parse(thisRow[0]["OpenDate"].ToString());
                datCloseDate = DateTime.Parse(thisRow[0]["CloseDate"].ToString());
                datSettlementDate = DateTime.Parse(thisRow[0]["SettlementDate"].ToString());
                strIssuer = thisRow[0]["Issuer"].ToString();
                strMutualFundAccount = thisRow[0]["MutualFundAccount"].ToString();
                strMutualFundType = thisRow[0]["MutualFundType"].ToString();
                blnTradingOnNSE = bool.Parse(thisRow[0]["TradingOnNSE"].ToString());
                strProductCode = thisRow[0]["ProductCode"].ToString();
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
            SqlCommand oCommand = db.GetStoredProcCommand("MutualFundDelete") as SqlCommand;
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
            SqlCommand dbCommand = db.GetStoredProcCommand("MutualFundSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


    }
}
