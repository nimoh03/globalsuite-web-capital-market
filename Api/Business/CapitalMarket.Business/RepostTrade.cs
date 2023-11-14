using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
namespace CapitalMarket.Business
{
    public class RepostTrade
    {
        IFormatProvider format = new CultureInfo("en-GB");

        #region Declaration
        private long lngTransNo;
        private DateTime datTradeDate;
        private string strUserId;
        #endregion

        #region Properties
        public long TransNo
        {
            set { lngTransNo = value; }
            get { return lngTransNo; }
        }
        public DateTime TradeDate
        {
            set { datTradeDate = value; }
            get { return datTradeDate; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        #endregion

        #region Add Repost Trade And Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("RepostTradeAdd") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            db.AddInParameter(oCommand, "TradeDate", SqlDbType.DateTime,datTradeDate);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserId.Trim());
            return oCommand;
        }
        #endregion

        #region Get All 
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("RepostTradeSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion
    }
}
