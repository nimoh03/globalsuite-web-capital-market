using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class CustMissing
    {
        #region Declaration
        private string strTransNo, strSurname, strUserID, strCsCsAcct;
        private DateTime datValDate;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string Surname
        {
            set { strSurname = value; }
            get { return strSurname; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public string CsCsAcct
        {
            set { strCsCsAcct = value; }
            get { return strCsCsAcct; }
        }
        public DateTime ValDate
        {
            set { datValDate = value; }
            get { return datValDate; }
        }

        #endregion

        #region Add New Cust Missing Record - Used In Transaction Disk Upload
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustMissingAddNew") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, strUserID);
            db.AddInParameter(dbCommand, "CsCsAcct", SqlDbType.VarChar, strCsCsAcct);
            db.AddInParameter(dbCommand, "ValDate", SqlDbType.DateTime, datValDate);
            db.AddInParameter(dbCommand, "TableName", SqlDbType.VarChar, "CUSTMISSING");
                db.ExecuteNonQuery(dbCommand);
                blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Delete All CustMissing Record
        public bool DeleteAll()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustMissingDeleteAll") as SqlCommand;
                db.ExecuteNonQuery(dbCommand);
                blnStatus = true;
            return blnStatus;

        }
        #endregion

        #region Get All CustMissing Record
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustMissingSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
