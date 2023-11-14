using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using System.Linq;


namespace CapitalMarket.Business
{

    public class MarkToMarketDetail
    {
        #region Declaration
        private long lngTransNo;
        private string strCustomerId;
        private string strStockcode;
        private DateTime datQDate;
        private decimal decAmount;
        private string strGainOrLoss;
        private string strDescription;
        private string strNSEorNASD;
        private string strSaveType;
        #endregion

        #region Properties
        public long TransNo
        {
            set { lngTransNo = value; }
            get { return lngTransNo; }
        }
        public string CustomerId
        {
            set { strCustomerId = value; }
            get { return strCustomerId; }
        }
        public string Stockcode
        {
            set { strStockcode = value; }
            get { return strStockcode; }
        }
        public DateTime QDate
        {
            set { datQDate = value; }
            get { return datQDate; }
        }

        public decimal Amount
        {
            set { decAmount = value; }
            get { return decAmount; }
        }

        public string GainOrLoss
        {
            set { strGainOrLoss = value; }
            get { return strGainOrLoss; }
        }

        public string Description
        {
            set { strDescription = value; }
            get { return strDescription; }
        }
        public string NSEorNASD
        {
            set { strNSEorNASD = value; }
            get { return strNSEorNASD; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Add New Quotes And Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MarkToMarketDetailAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            db.AddInParameter(dbCommand, "Amount", SqlDbType.Money, decAmount);
            db.AddInParameter(dbCommand, "GainOrLoss", SqlDbType.Char, strGainOrLoss.Trim());
            db.AddInParameter(dbCommand, "Description", SqlDbType.VarChar, strDescription.Trim());
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, strNSEorNASD.Trim());
            return dbCommand;
        }
        #endregion

        #region Delete For Date Return Command
        public SqlCommand DeleteForDateCommand(DateTime datSpecifiedDate, string strRetNSEorNASD)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MarkToMarketDetailDeleteByDate") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datSpecifiedDate);
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, strRetNSEorNASD);
            return dbCommand;
        }
        #endregion
    }
}


