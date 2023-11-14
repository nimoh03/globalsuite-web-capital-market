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

    public class QuoteMarkToMarket
    {
        #region Declaration
        private long lngTransNo;
        private string strStockcode;
        private DateTime datQDate;
        private decimal decRPrice;
        private string strNSEorNASD;
        private string strSaveType;
        #endregion

        #region Properties
        public long TransNo
        {
            set { lngTransNo = value; }
            get { return lngTransNo; }
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

        public decimal RPrice
        {
            set { decRPrice = value; }
            get { return decRPrice; }
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

        #region Delete For Date Return Command
        public SqlCommand DeleteForDateCommand(DateTime datSpecifiedDate,string strRetNSEorNASD)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketDeleteByDate") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datSpecifiedDate);
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, strRetNSEorNASD);
            return dbCommand;
        }
        #endregion

        #region Get Price For A Particluar Date - Given Date Parameter
        public List<StockPrice> GetPriceForADate(DateTime datSpecifiedDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand;
            dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketSelectByDate") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datSpecifiedDate);
            if (datSpecifiedDate != DateTime.MinValue)
            {
                return db.ExecuteDataSet(dbCommand).Tables[0].AsEnumerable().Select(dataRow => new StockPrice { SecurityName = dataRow.Field<string>("StockCode"), ReferencePrice = dataRow.Field<decimal>("RPrice") }).ToList();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Get Price For A Particluar Stock
        public decimal GetPriceForStock(List<StockPrice> oStockPrices, string strSecurityName)
        {
            if (oStockPrices != null)
            {
                decimal query =
                (from s in oStockPrices
                 where s.SecurityName.Trim() == strSecurityName.Trim()
                 select s.ReferencePrice).SingleOrDefault();
                return query;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Last Quote Date Specified
        public DateTime GetLastDate(DateTime datSpecifiedDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketMaxDate") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datSpecifiedDate);
            var varLastDate = db.ExecuteScalar(dbCommand);
            return varLastDate != null && varLastDate.ToString().Trim() != "" ?  DateTime.ParseExact(varLastDate.ToString().Substring(0,10), "dd/MM/yyyy", format) : DateTime.MinValue;
        }
        #endregion



        #region Add New Quotes And Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode);
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            db.AddInParameter(dbCommand, "RPrice", SqlDbType.Decimal, decRPrice);
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, strNSEorNASD);
            return dbCommand;
        }
        #endregion

        #region Save Quotes
        public bool Save()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketAdd") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketEdit") as SqlCommand;
                db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            }
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode);
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            db.AddInParameter(dbCommand, "RPrice", SqlDbType.Decimal, decRPrice);
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, strNSEorNASD);
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Quote Mark To Market
        public bool GetQuoteMarkToMarket()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strStockcode = thisRow[0]["Stockcode"].ToString();
                datQDate = DateTime.ParseExact(thisRow[0]["QDate"].ToString(),"dd/MM/yyyy",format);
                decRPrice = decimal.Parse(thisRow[0]["RPrice"].ToString());
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Quote Mark To Market Already Exist For Existing Record
        public bool QuoteMarkToMarketNameExist()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketSelectByStockDateExist") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode);
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.BigInt, lngTransNo);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
            {
                blnStatus = true;
            }
            return blnStatus;
        }
        #endregion

        #region Check That Quote Mark To Market Already Exist For New Record
        public bool QuoteMarkToMarketNameExistNoTransNo()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketSelectByStockDateExistNoTransNo") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode);
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            db.AddOutParameter(dbCommand, "NameExist", SqlDbType.VarChar, 1);
            db.ExecuteNonQuery(dbCommand);
            if (db.GetParameterValue(dbCommand, "NameExist").ToString().Trim() == "0")
            {
                blnStatus = false;
            }
            else
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
            SqlCommand oCommand = db.GetStoredProcCommand("QuotesMarkToMarketDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TrasNo", SqlDbType.BigInt, lngTransNo);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;


            return blnStatus;
        }
        #endregion


        #region Get All Stock Price For A Date Order By Symbol
        public DataSet GetPricesForADateOrderBySymbol()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketSelectByDateOrderBySymbol") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Stock Price For A Date Order By Symbol Price Only
        public DataSet GetPricesForADateOrderBySymbolPriceOnly()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketSelectByDateOrderBySymbolPriceOnly") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Stock Price For A Date Order By Sector
        public DataSet GetPricesForADateOrderBySector()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketSelectByDateOrderBySector") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Price For Last Quote Date For A Particular Stock For A Date
        public decimal GetLastPriceForAStockAndDate(DateTime datSpecifiedDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketMaxDateSelectByStockAndDate", strStockcode.Trim(), datSpecifiedDate) as SqlCommand;
            var varMaxDate = db.ExecuteScalar(dbCommand);
            if (varMaxDate == null || varMaxDate.ToString().Trim() == "")
            {
                return 0;
            }
            else
            {
                datQDate = DateTime.ParseExact(varMaxDate.ToString().Substring(0, 10), "dd/MM/yyyy", format);
                //GetPriceForADate(datQDate);
                return GetPriceForStock(GetPriceForADate(datQDate), strStockcode.Trim());
            }
        }
        #endregion

        #region Get Price For Last Quote Date For A Particular Stock For A Date Exclude Current Date
        public decimal GetLastPriceForAStockAndDateExcludeCurrentDate(DateTime datSpecifiedDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMarkToMarketMaxDateSelectByStockAndDateExcludeCurrentDate", strStockcode.Trim(), datSpecifiedDate) as SqlCommand;
            var varMaxDate = db.ExecuteScalar(dbCommand);
            if (varMaxDate == null || varMaxDate.ToString().Trim() == "")
            {
                return 0;
            }
            else
            {
                datQDate = DateTime.ParseExact(varMaxDate.ToString().Substring(0, 10), "dd/MM/yyyy", format);
                //GetPriceForADate(datQDate);
                return GetPriceForStock(GetPriceForADate(datQDate), strStockcode.Trim());
            }
        }
        #endregion

        public class FileQuote
        {
            public string SecurityName { get; set; }
            public decimal ReferencePrice { get; set; }
        }

        public class StockPrice
        {
            public string SecurityName { get; set; }
            public decimal ReferencePrice { get; set; }
        }
    }
}

