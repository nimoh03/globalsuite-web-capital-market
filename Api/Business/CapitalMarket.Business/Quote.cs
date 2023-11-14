using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using System.Linq;
using GL.Business;

namespace CapitalMarket.Business
{

    public class Quote
    {
        #region Declaration
        private string strStockcode;
        private DateTime datQDate;
        private decimal decRPrice, decRClose, decPLow, decPHigh, decPClose, decPOpen, decPChange, decHPFirst, decHPSec;
        private double decPValue;
        private Int32 intPTrades, intHPThird;
        private Int64 intPVolume;

        #endregion

        #region Properties
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
        public decimal RClose
        {
            set { decRClose = value; }
            get { return decRClose; }
        }
        public decimal PLow
        {
            set { decPLow = value; }
            get { return decPLow; }
        }
        public decimal PHigh
        {
            set { decPHigh = value; }
            get { return decPHigh; }
        }
        public decimal PClose
        {
            set { decPClose = value; }
            get { return decPClose; }
        }
        public decimal POpen
        {
            set { decPOpen = value; }
            get { return decPOpen; }
        }
        public decimal PChange
        {
            set { decPChange = value; }
            get { return decPChange; }
        }
        public double PValue
        {
            set { decPValue = value; }
            get { return decPValue; }
        }
        public decimal HPFirst
        {
            set { decHPFirst = value; }
            get { return decHPFirst; }
        }
        public decimal HPSec
        {
            set { decHPSec = value; }
            get { return decHPSec; }
        }
        public Int32 PTrades
        {
            set { intPTrades = value; }
            get { return intPTrades; }
        }
        public Int32 HPThird
        {
            set { intHPThird = value; }
            get { return intHPThird; }
        }
        public Int64 PVolume
        {
            set { intPVolume = value; }
            get { return intPVolume; }
        }
        #endregion


        #region Add New Quotes
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode);
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            db.AddInParameter(dbCommand, "RPrice", SqlDbType.Decimal, decRPrice);
            db.AddInParameter(dbCommand, "RClose", SqlDbType.Decimal, decRClose);
            db.AddInParameter(dbCommand, "PVolume", SqlDbType.BigInt, intPVolume);
            db.AddInParameter(dbCommand, "PLow", SqlDbType.Decimal, decPLow);
            db.AddInParameter(dbCommand, "PHigh", SqlDbType.Decimal, decPHigh);
            db.AddInParameter(dbCommand, "PClose", SqlDbType.Decimal, decPClose);
            db.AddInParameter(dbCommand, "POpen", SqlDbType.Decimal, decPOpen);
            db.AddInParameter(dbCommand, "PChange", SqlDbType.Float, decPChange);
            db.AddInParameter(dbCommand, "PTrades", SqlDbType.BigInt, intPTrades);
            db.AddInParameter(dbCommand, "PValue", SqlDbType.Decimal, decPValue);
            db.AddInParameter(dbCommand, "HPFirst", SqlDbType.Decimal, decHPFirst);
            db.AddInParameter(dbCommand, "HPSec", SqlDbType.Decimal, decHPSec);
            db.AddInParameter(dbCommand, "HPThird", SqlDbType.Int, intHPThird);
            try
            {
                db.ExecuteNonQuery(dbCommand);
                blnStatus = true;
                return blnStatus;
            }
            catch (Exception e)
            {
                string you = e.Message;
                blnStatus = false;
                return blnStatus;
            }
        }
        #endregion


        #region Delete For Date Return Command
        public SqlCommand DeleteForDateCommand(DateTime datSpecifiedDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesDeleteByDate") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datSpecifiedDate);
            return dbCommand;
        }
        #endregion

        #region Get Price For A Particluar Date - Given Date Parameter
        public List<StockPrice> GetPriceForADate(DateTime datSpecifiedDate)
        {
            string strPriceSource = "";
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "GETJOBPRICEFROMFIX";           
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            DatabaseProviderFactory factoryTradebook = new DatabaseProviderFactory(); SqlDatabase dbTradeBook = factoryTradebook.Create("GlobalSuiteTBookdb") as SqlDatabase;
            SqlCommand dbCommand;
            strPriceSource = oGLParam.CheckParameter();
            if (strPriceSource.Trim() == "YES")
            {
                dbCommand = dbTradeBook.GetStoredProcCommand("QuotesSelectByDate") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("QuotesSelectByDate") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datSpecifiedDate);
            if (datSpecifiedDate != DateTime.MinValue)
            {
                if (strPriceSource.Trim() == "YES")
                {
                    return dbTradeBook.ExecuteDataSet(dbCommand).Tables[0].AsEnumerable().Select(dataRow => new StockPrice { SecurityName = dataRow.Field<string>("SecurityID"), ReferencePrice = dataRow.Field<decimal?>("RefPrice"), RClose = dataRow.Field<decimal?>("ClosePrice") }).ToList();
                }
                else
                {
                    return db.ExecuteDataSet(dbCommand).Tables[0].AsEnumerable().Select(dataRow => new StockPrice { SecurityName = dataRow.Field<string>("StockCode"), ReferencePrice = dataRow.Field<decimal?>("RPrice"), RClose = dataRow.Field<decimal?>("RClose") }).ToList();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Get NGX Equity Price For A Particluar Date - Given Date Parameter
        public List<FileQuote> GetNGXEquityPriceForADate(DateTime datSpecifiedDate)
        {
            string strPriceSource = "";
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "GETJOBPRICEFROMFIX";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            DatabaseProviderFactory factoryTradebook = new DatabaseProviderFactory(); SqlDatabase dbTradeBook = factoryTradebook.Create("GlobalSuiteTBookdb") as SqlDatabase;
            SqlCommand dbCommand;
            strPriceSource = oGLParam.CheckParameter();
            if (strPriceSource.Trim() == "YES")
            {
                dbCommand = dbTradeBook.GetStoredProcCommand("QuotesSelectByDateNGXEquity") as SqlCommand;
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("QuotesSelectByDateNGXEquity") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datSpecifiedDate);
            if (datSpecifiedDate != DateTime.MinValue)
            {
                if (strPriceSource.Trim() == "YES")
                {
                    return dbTradeBook.ExecuteDataSet(dbCommand).Tables[0].AsEnumerable().Select(dataRow => new FileQuote { SecurityName = dataRow.Field<string>("SecurityID"), ReferencePrice = dataRow.Field<decimal?>("RefPrice"), ClosePrice = dataRow.Field<decimal?>("ClosePrice"),OpenPrice  = dataRow.Field<decimal?>("OpenPrice"), BidPrice = dataRow.Field<decimal?>("BidPrice"), OfferPrice = dataRow.Field<decimal?>("OfferPrice") }).ToList();
                }
                else
                {
                    return db.ExecuteDataSet(dbCommand).Tables[0].AsEnumerable().Select(dataRow => new FileQuote { SecurityName = dataRow.Field<string>("StockCode"), ReferencePrice = dataRow.Field<decimal?>("RPrice"), ClosePrice = dataRow.Field<decimal?>("RClose"), OpenPrice = dataRow.Field<decimal?>("POpen"), BidPrice = dataRow.Field<decimal?>("POpen"), OfferPrice = dataRow.Field<decimal?>("POpen") }).ToList();
                }
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
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "GETREFPRICE";
            string strRefPrice = oGLParam.CheckParameter();
            if (oStockPrices != null)
            {
                decimal? query = 0;
                if (strRefPrice.Trim() == "YES")
                {
                    query =
                    (from s in oStockPrices
                     where s.SecurityName.Trim() == strSecurityName.Trim()
                     select s.ReferencePrice).SingleOrDefault();
                }
                else
                {
                    query =
                    (from s in oStockPrices
                     where s.SecurityName.Trim() == strSecurityName.Trim()
                     select s.RClose).SingleOrDefault();
                }
                return Convert.ToDecimal(query);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Close Price For A Particluar Stock
        public decimal GetClosePriceForStock(List<StockPrice> oStockPrices, string strSecurityName)
        {
            if (oStockPrices != null)
            {
                decimal? query =
                (from s in oStockPrices
                 where s.SecurityName.Trim() == strSecurityName.Trim()
                 select s.RClose).SingleOrDefault();
                return Convert.ToDecimal(query);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Last Quote Date
        public DateTime GetLastDate()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "GETJOBPRICEFROMFIX";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            DatabaseProviderFactory factoryTradebook = new DatabaseProviderFactory(); SqlDatabase dbTradeBook = factoryTradebook.Create("GlobalSuiteTBookdb") as SqlDatabase;
            SqlCommand dbCommand;

            if (oGLParam.CheckParameter().Trim() == "YES")
            {

                dbCommand = dbTradeBook.GetStoredProcCommand("QuotesMaxDate") as SqlCommand;
                var varLastQuoteDate = dbTradeBook.ExecuteScalar(dbCommand);
                if (varLastQuoteDate == null || varLastQuoteDate == System.DBNull.Value)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return DateTime.ParseExact(varLastQuoteDate.ToString().Substring(0, 10), "dd/MM/yyyy", format);
                }
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("QuotesMaxDate") as SqlCommand;
                var varLastQuoteDate = db.ExecuteScalar(dbCommand);
                if (varLastQuoteDate == null || varLastQuoteDate == System.DBNull.Value)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return DateTime.ParseExact(varLastQuoteDate.ToString().Substring(0, 10), "dd/MM/yyyy", format);
                }
            }

        }
        #endregion

        #region Get Last Quote Date For A Particular Stock
        public DateTime GetLastDateForAStock()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "GETJOBPRICEFROMFIX";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            DatabaseProviderFactory factoryTradebook = new DatabaseProviderFactory(); SqlDatabase dbTradeBook = factoryTradebook.Create("GlobalSuiteTBookdb") as SqlDatabase;
            SqlCommand dbCommand;
            
            if (oGLParam.CheckParameter().Trim() == "YES")
            {
                if (strStockcode != null && strStockcode.Trim() != "")
                {
                    dbCommand = dbTradeBook.GetStoredProcCommand("QuotesMaxDateSelectByStock", strStockcode.Trim()) as SqlCommand;
                    var varLastQuoteDate = dbTradeBook.ExecuteScalar(dbCommand);
                    if (varLastQuoteDate == null || varLastQuoteDate == System.DBNull.Value)
                    {
                        return DateTime.MinValue;
                    }
                    else
                    {
                        return DateTime.ParseExact(varLastQuoteDate.ToString().Substring(0, 10), "dd/MM/yyyy", format);
                    }
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            else
            {
                dbCommand = db.GetStoredProcCommand("QuotesMaxDateSelectByStock", strStockcode.Trim()) as SqlCommand;
                var varLastQuoteDate = db.ExecuteScalar(dbCommand);
                if (varLastQuoteDate == null || varLastQuoteDate == System.DBNull.Value)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return DateTime.ParseExact(varLastQuoteDate.ToString().Substring(0, 10), "dd/MM/yyyy", format);
                }
            }
            
           
        }
        #endregion


        #region Get Last Price For A Particular Stock
        public decimal GetLastPriceForAStock()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMaxDatePriceSelectByStock", strStockcode.Trim()) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) == System.DBNull.Value || db.ExecuteScalar(dbCommand).ToString().Trim() == "")
            {
                return 0;
            }
            else
            {
                return decimal.Parse((string)db.ExecuteScalar(dbCommand));
            }
        }
        #endregion

        #region Add New Quotes And Return Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode);
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            db.AddInParameter(dbCommand, "RPrice", SqlDbType.Decimal, decRPrice);
            db.AddInParameter(dbCommand, "RClose", SqlDbType.Decimal, decRClose);
            db.AddInParameter(dbCommand, "PVolume", SqlDbType.BigInt, intPVolume);
            db.AddInParameter(dbCommand, "PLow", SqlDbType.Decimal, decPLow);
            db.AddInParameter(dbCommand, "PHigh", SqlDbType.Decimal, decPHigh);
            db.AddInParameter(dbCommand, "PClose", SqlDbType.Decimal, decPClose);
            db.AddInParameter(dbCommand, "POpen", SqlDbType.Decimal, decPOpen);
            db.AddInParameter(dbCommand, "PChange", SqlDbType.Float, decPChange);
            db.AddInParameter(dbCommand, "PTrades", SqlDbType.Int, intPTrades);
            db.AddInParameter(dbCommand, "PValue", SqlDbType.Decimal, decPValue);
            db.AddInParameter(dbCommand, "HPFirst", SqlDbType.Decimal, decHPFirst);
            db.AddInParameter(dbCommand, "HPSec", SqlDbType.Decimal, decHPSec);
            db.AddInParameter(dbCommand, "HPThird", SqlDbType.Int, intHPThird);
            return dbCommand;

        }
        #endregion

        #region Get All Stock Price For A Date Order By Symbol
        public DataSet GetPricesForADateOrderBySymbol()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesSelectByDateOrderBySymbol") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Stock Price For A Date Order By Symbol Price Only
        public DataSet GetPricesForADateOrderBySymbolPriceOnly()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesSelectByDateOrderBySymbolPriceOnly") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All Stock Price For A Date Order By Sector
        public DataSet GetPricesForADateOrderBySector()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesSelectByDateOrderBySector") as SqlCommand;
            db.AddInParameter(dbCommand, "QDate", SqlDbType.DateTime, datQDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get High And Low For 52 Week
        public void GetHighAndLowFor52Week(DateTime datStartDate, DateTime datEndDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesSelectByStockHighAndLowFor52Week") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strStockcode.Trim());
            db.AddInParameter(dbCommand, "StartDate", SqlDbType.DateTime, datStartDate);
            db.AddInParameter(dbCommand, "EndDate", SqlDbType.DateTime, datEndDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                if (thisRow[0]["HighPrice"] != null && thisRow[0]["HighPrice"].ToString().Trim() != "")
                {
                    PHigh = decimal.Parse(thisRow[0]["HighPrice"].ToString());
                }
                else
                {
                    PHigh = 0;
                }
                if (thisRow[0]["LowPrice"] != null && thisRow[0]["LowPrice"].ToString().Trim() != "")
                {
                    PLow = decimal.Parse(thisRow[0]["LowPrice"].ToString());
                }
                else
                {
                    PLow = 0;
                }
            }
            else
            {
                PHigh = 0;
                PLow = 0;
            }
        }
        #endregion

        


        #region Get Price For Last Quote Date For A Particular Stock For A Date
        public decimal GetLastPriceForAStockAndDate(DateTime datSpecifiedDate)
        {
            string strPriceSource = "";
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            GLParam oGLParam = new GLParam();
            oGLParam.Type = "GETJOBPRICEFROMFIX";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            DatabaseProviderFactory factoryTradebook = new DatabaseProviderFactory(); SqlDatabase dbTradeBook = factoryTradebook.Create("GlobalSuiteTBookdb") as SqlDatabase;
            SqlCommand dbCommandMarkToMarketPrice = db.GetStoredProcCommand("QuotesMaxDateSelectByStockAndDate", strStockcode.Trim(), datSpecifiedDate) as SqlCommand;
            SqlCommand dbCommand;
            strPriceSource = oGLParam.CheckParameter();
            if (strPriceSource.Trim() == "YES")
            {
                dbCommand = dbTradeBook.GetStoredProcCommand("QuotesMaxDateSelectByStockAndDate", strStockcode.Trim(), datSpecifiedDate) as SqlCommand;
                var varMaxDate = dbTradeBook.ExecuteScalar(dbCommand);
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
            else
            {
                dbCommand = db.GetStoredProcCommand("QuotesMaxDateSelectByStockAndDate", strStockcode.Trim(), datSpecifiedDate) as SqlCommand;
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
        }
        #endregion

        #region Set Last Date Of Price List
        public void SetLastDateOfPriceListAndUpdateStockTable(DateTime datDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesSelectLastDatePriceListUpdateStockTable") as SqlCommand;
            db.AddInParameter(dbCommand, "ToDate", SqlDbType.DateTime, datDate);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Set Last Date Of Price List
        public DateTime SetLastDateOfPriceList(DateTime datDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesSelectLastDatePriceList") as SqlCommand;
            db.AddInParameter(dbCommand, "ToDate", SqlDbType.DateTime, datDate);
            db.AddOutParameter(dbCommand, "LastPriceDate", SqlDbType.DateTime, 20);
            db.ExecuteNonQuery(dbCommand);
            
            return DateTime.ParseExact(db.GetParameterValue(dbCommand, "LastPriceDate").ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
        }
        #endregion

        #region Set Last Date Of Price List For Opening Price
        public DateTime SetLastDateOfPriceListForOpeningPrice(DateTime datDate)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            IFormatProvider format = new CultureInfo("en-GB");
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesSelectLastDatePriceListOpeningPrice") as SqlCommand;
            db.AddInParameter(dbCommand, "ToDate", SqlDbType.DateTime, datDate);
            db.AddOutParameter(dbCommand, "LastPriceDate", SqlDbType.DateTime, 20);
            db.ExecuteNonQuery(dbCommand);
            return DateTime.ParseExact(db.GetParameterValue(dbCommand, "LastPriceDate").ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
        }
        #endregion

        #region Delete Missing Quote
        public void DeleteMissingQuote()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMissingDelete") as SqlCommand;
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Save Missing Quote
        public void SaveMissingQuote(string strMissingStock, decimal decMissingOpeningPrice, decimal decMissingClosingPrice)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("QuotesMissingSave") as SqlCommand;
            db.AddInParameter(dbCommand, "StockCode", SqlDbType.VarChar, strMissingStock);
            db.AddInParameter(dbCommand, "MissingOpeningPrice", SqlDbType.Money, decMissingOpeningPrice);
            db.AddInParameter(dbCommand, "MissingClosingPrice", SqlDbType.Money, decMissingClosingPrice);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        public class FileQuote
        {
            public string  SecurityName { get; set; }
            public decimal? ClosePrice { get; set; }
            public decimal? OpenPrice { get; set; }
            public decimal? HighPrice { get; set; }
            public decimal? LowPrice { get; set; }
            public decimal? ReferencePrice { get; set; }
            public decimal? PriceChange { get; set; }
            public long Volume { get; set; }
            public double Value { get; set; }

            public decimal? BidPrice { get; set; }
            public decimal? OfferPrice { get; set; }

            public long? BidDepth { get; set; }
            public long? OfferDepth { get; set; }
        }

        public class FileQuoteGainLoss
        {
            public string SecurityName { get; set; }
            public decimal? OpenPr { get; set; }
            public decimal? CurrPr { get; set; }
            public decimal? Chg { get; set; }
            public decimal? PerChg { get; set; }

        }

        public class FileQuotePrice
        {
            public string SecurityName { get; set; }
            public decimal? Price { get; set; }
        }

        public class FileQuoteBidOffer
        {
            public string SecurityName { get; set; }
            public decimal? CurrPr { get; set; }
            public decimal? BidPr { get; set; }
            public decimal? OffPr { get; set; }
            public decimal? BidDf { get; set; }
            public decimal? OffDf { get; set; }
        }

        public class StockPrice
        {
            public string  SecurityName { get; set; }
            public decimal? ReferencePrice { get; set; }
            public decimal? RClose { get; set; }
        }
    }
}
