using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;
using BaseUtility.Business;

namespace CapitalMarket.Business
{
    public class PortfolioUploadDate
    {
        IFormatProvider format = new CultureInfo("en-GB");

        #region Declaration
        private DateTime datUploadDate;
        #endregion

        #region Properties
        public DateTime UploadDate
        {
            set { datUploadDate = value; }
            get { return datUploadDate; }
        }
        
        #endregion

        #region Add New UploadDate
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioUploadDateAdd") as SqlCommand;
            db.AddInParameter(oCommand, "UploadDate", SqlDbType.DateTime, datUploadDate);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Portfolio Upload Date
        public bool GetPortfolioUploadDate()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioUploadDateSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "UploadDate", SqlDbType.DateTime, datUploadDate);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                //datiPortfolioUploadDate = DateTime.ParseExact(thisRow[0]["iPortfolioUploadDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get Portfolio Upload Date and Return A DataSet
        public DataSet GetPortfolioUploadDateReturnDataSet()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioUploadDateSelectByUploadDate") as SqlCommand;
            db.AddInParameter(dbCommand, "UploadDate", SqlDbType.DateTime, datUploadDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Add New Portfolio Upload Date And Return SqlCommand
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioUploadDateAdd") as SqlCommand;
            db.AddInParameter(oCommand, "UploadDate", SqlDbType.DateTime, datUploadDate);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion

        #region Delete An Portfolio Upload Date And Return SqlCommand
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("PortfolioUploadDateDelete") as SqlCommand;
            db.AddInParameter(oCommand, "UploadDate", SqlDbType.DateTime, datUploadDate);
            return oCommand;
        }
        #endregion

        #region Get All 
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioUploadDateSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Get FIX Date In Acct_GL,Stkb_Sold,Stkb_Portfolio Not In FIX PortfolioUploadDate
        public DataSet GetFIXTransactionNotInFIXPortfolioUploadDate()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("PortfolioUploadDateSelectFIXTransNotInPortfolioUploadDate") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion
    }
}
