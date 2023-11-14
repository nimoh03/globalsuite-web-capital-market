using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using GL.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class PriceDateMarkToMarket
    {
        #region Declaration
        private DateTime datAutoDate;
        private string strNSEorNASD;
        #endregion

        #region Properties
        public DateTime AutoDate
        {
            set { datAutoDate = value; }
            get { return datAutoDate; }
        }

        public string NSEorNASD
        {
            set { strNSEorNASD = value; }
            get { return strNSEorNASD; }
        }
        #endregion

        #region Add New AutoDate And Return SqlCommand
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PriceDateMarkToMarketAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            db.AddInParameter(oCommand, "NSEorNASD", SqlDbType.VarChar, strNSEorNASD);
            return oCommand;
        }
        #endregion

        #region Get Price Date
        public bool GetAutoDate(string stringNSEorNASD)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PriceDateMarkToMarketSelectByAutoDate") as SqlCommand;
            db.AddInParameter(dbCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, stringNSEorNASD);
            try
            {
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                DataTable thisTable = oDS.Tables[0];
                DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 1)
                {
                    datAutoDate = DateTime.Parse(thisRow[0]["AutoDate"].ToString());
                    strNSEorNASD = thisRow[0]["strNSEorNASD"].ToString();
                    blnStatus = true;
                }
            }
            catch
            {
                return blnStatus;
            }

            return blnStatus;
        }
        #endregion

        #region Delete An AutoDate And Return SqlCommand
        public SqlCommand DeleteCommand(string stringNSEorNASD)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PriceDateMarkToMarketDelete") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            db.AddInParameter(oCommand, "NSEorNASD", SqlDbType.VarChar, stringNSEorNASD);
            return oCommand;
        }
        #endregion

        #region Check Next Date Exist
        public bool CheckNextDateExist(DateTime datDateToCheck, DateTime datLastDate, string stringNSEorNASD)
        {
            bool blnStatus = false;
            GeneralFunc oGeneralFunc = new GeneralFunc();
            datAutoDate = oGeneralFunc.MinusBusinessDayForPriceList(datDateToCheck, 1, Holiday.GetAllReturnList(), datLastDate);
            blnStatus = GetAutoDate(stringNSEorNASD);
            return blnStatus;
        }
        #endregion

        #region Get Last Date
        public DateTime GetLastDate(DateTime datCurrentDate, string stringNSEorNASD)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PriceDateMarkToMarketSelectLastDateBySpecifiedDate") as SqlCommand;
            db.AddInParameter(dbCommand, "CurrentDate", SqlDbType.DateTime, datCurrentDate);
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, stringNSEorNASD);
            var varResultDate = db.ExecuteScalar(dbCommand);
            if (varResultDate != null && varResultDate.ToString().Trim() != "")
            {
                return DateTime.Parse(varResultDate.ToString());
            }
            else
            {
                return datCurrentDate;
            }
        }
        #endregion

        #region Get All
        public DataSet GetAll(string stringNSEorNASD)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PriceDateMarkToMarketSelectAll") as SqlCommand;
            db.AddInParameter(dbCommand, "NSEorNASD", SqlDbType.VarChar, stringNSEorNASD);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion


    }
}

