using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class StopLoss
    {
        #region Declaration
        private string strTransNo;
        private DateTime datEffDate;
        private string strStockCode;
        private string strCustomer;
        private string strTransType;
        private decimal decLowPrice;
        private decimal decHighPrice;
        private string strUserId;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        public string Stockcode
        {
            set { strStockCode = value; }
            get { return strStockCode; }
        }
        public string Customer
        {
            set { strCustomer = value; }
            get { return strCustomer; }
        }
        public string TransType
        {
            set { strTransType = value; }
            get { return strTransType; }
        }
        public decimal LowPrice
        {
            set { decLowPrice = value; }
            get { return decLowPrice; }
        }
        public decimal HighPrice
        {
            set { decHighPrice = value; }
            get { return decHighPrice; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
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
            if (!ChkTransNoExist(DataGeneral.PostStatus.UnPosted))
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("StopLossAddNew") as SqlCommand;

            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("StopLossEdit") as SqlCommand;
            }
            db.AddInParameter(oCommand, "Transno", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(oCommand, "StockCode", SqlDbType.VarChar, strStockCode.Trim());
            db.AddInParameter(oCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            db.AddInParameter(oCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
            db.AddInParameter(oCommand, "LowPrice", SqlDbType.Decimal, decLowPrice);
            db.AddInParameter(oCommand, "HighPrice", SqlDbType.Decimal, decHighPrice);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim().ToUpper());
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion


        #region Get Stop Loss
        public bool GetStopLoss()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("StopLossSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                datEffDate = DateTime.Parse(thisRow[0]["EffDate"].ToString());
                strStockCode = thisRow[0]["StockCode"].ToString();
                strCustomer = thisRow[0]["Customer"].ToString();
                strTransType = thisRow[0]["TransType"].ToString();
                decLowPrice = decimal.Parse(thisRow[0]["LowPrice"].ToString());
                decHighPrice = decimal.Parse(thisRow[0]["HighPrice"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
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
                    oCommand = db.GetStoredProcCommand("StopLossChkTransNoExistUnPosted") as SqlCommand;
                }
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

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("StopLossDelete") as SqlCommand;
            db.AddInParameter(oCommand, "Code", SqlDbType.VarChar, strTransNo.Trim());


            using (SqlConnection connection = db.CreateConnection() as SqlConnection)
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                    IFormatProvider format = new CultureInfo("en-GB");

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

        #region Get All
        public DataSet GetAll(DataGeneral.PostStatus TransStatus)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
          
                dbCommand = db.GetStoredProcCommand("StopLossSelectAll") as SqlCommand;
            
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

    }
}
