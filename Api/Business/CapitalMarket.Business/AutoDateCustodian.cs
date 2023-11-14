using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Globalization;

namespace CapitalMarket.Business
{
    public class AutoDateCustodian
    {
        IFormatProvider format = new CultureInfo("en-GB");

        #region Declaration
        private DateTime datiAutoDate;
        private string strUserId;
        private string strCustodianCode;
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
        public string CustodianCode
        {
            set { strCustodianCode = value; }
            get { return strCustodianCode; }
        }
        #endregion


        #region Add New AutoDate
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AutoDateCustodianAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datiAutoDate);
            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode.Trim());
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Get Auto Date
        public bool GetAutoDate()
        {
            bool blnStatus = true;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AutoDateCustodianSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "AutoDate", SqlDbType.DateTime, datiAutoDate);
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode.Trim());
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
        public DataSet GetAutoDateReturnDataSet()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AutoDateCustodianSelectByAutoDate") as SqlCommand;
            db.AddInParameter(dbCommand, "AutoDate", SqlDbType.DateTime, datiAutoDate);
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion

        #region Add New AutoDate And Return SqlCommand
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AutoDateCustodianAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datiAutoDate);
            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode.Trim());
            return oCommand;
        }
        #endregion

        #region Delete An AutoDate And Return SqlCommand
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AutoDateCustodianDelete") as SqlCommand;
            db.AddInParameter(oCommand, "AutoDate", SqlDbType.DateTime, datiAutoDate);
            db.AddInParameter(oCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode.Trim());
            return oCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AutoDateCustodianSelectAll") as SqlCommand;
            db.AddInParameter(dbCommand, "CustodianCode", SqlDbType.VarChar, strCustodianCode.Trim());
            return db.ExecuteDataSet(dbCommand);
        }
        #endregion
    }
}
