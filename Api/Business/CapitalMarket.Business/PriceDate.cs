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
    public class PriceDate
    {
        #region Declaration
        private DateTime datAutoDate;
        #endregion

        #region Properties
        public DateTime AutoDate
        {
            set { datAutoDate = value; }
            get { return datAutoDate; }
        }

        #endregion



        #region Add New Price Date
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PriceDateAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            try
            {
                db.ExecuteNonQuery(oCommand);
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

        #region Add New AutoDate And Return SqlCommand
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PriceDateAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            return oCommand;
        }
        #endregion

        #region Get Price Date
        public bool GetAutoDate()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PriceDateSelectByAutoDate") as SqlCommand;
            db.AddInParameter(dbCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            try
            {
                DataSet oDS = db.ExecuteDataSet(dbCommand);
                DataTable thisTable = oDS.Tables[0];
                DataRow[] thisRow = thisTable.Select();
                if (thisRow.Length == 1)
                {
                    datAutoDate = DateTime.Parse(thisRow[0]["AutoDate"].ToString());
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
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PriceDateDelete") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datAutoDate);
            return oCommand;
        }
        #endregion

        #region Check Next Date Exist
        public bool CheckNextDateExist(DateTime datDateToCheck,DateTime datLastDate)
        {
            bool blnStatus = false;
            GeneralFunc oGeneralFunc = new GeneralFunc();
            datAutoDate = oGeneralFunc.MinusBusinessDayForPriceList(datDateToCheck, 1, Holiday.GetAllReturnList(), datLastDate);
            blnStatus = GetAutoDate();
            return blnStatus;
        }
        #endregion

        #region Get Last Date
        public DateTime GetLastDate(DateTime datCurrentDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PriceDateSelectLastDateBySpecifiedDate") as SqlCommand;
            db.AddInParameter(dbCommand, "CurrentDate", SqlDbType.DateTime, datCurrentDate);
            var varResultDate = db.ExecuteScalar(dbCommand);
            if(varResultDate != null && varResultDate.ToString().Trim() != "")
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
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PriceDateSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        
    }
}
