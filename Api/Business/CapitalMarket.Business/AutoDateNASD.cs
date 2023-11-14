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
    public class AutoDateNASD
    {
        IFormatProvider format = new CultureInfo("en-GB");

        #region Declaration
        private DateTime datiAutoDate;
        private string strUserId;
        #endregion

        #region Properties
        public DateTime iAutoDate
        {
            set { datiAutoDate = value; }
            get { return datiAutoDate; }
        }
        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }
        #endregion


        #region Add New AutoDateNASD
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AutoDateNASDAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDateNASD", SqlDbType.DateTime, datiAutoDate);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Auto Date
        public bool GetAutoDateNASD()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AutoDateNASDSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "AutoDateNASD", SqlDbType.DateTime, datiAutoDate);

            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                //datiAutoDate = DateTime.ParseExact(thisRow[0]["iAutoDate"].ToString().Trim().Substring(0, 10), "dd/MM/yyyy", format);
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get Auto Date and Return A DataSet
        public DataSet GetAutoDateNASDReturnDataSet()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AutoDateNASDSelectByAutoDateNASD") as SqlCommand;
            db.AddInParameter(dbCommand, "AutoDateNASD", SqlDbType.DateTime, datiAutoDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Add New AutoDateNASD And Return SqlCommand
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AutoDateNASDAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDateNASD", SqlDbType.DateTime, datiAutoDate);
            return oCommand;
        }
        #endregion

        #region Delete An AutoDateNASD And Return SqlCommand
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AutoDateNASDDelete") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDateNASD", SqlDbType.DateTime, datiAutoDate);
            return oCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AutoDateNASDSelectAll") as SqlCommand;
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion
    }
}

