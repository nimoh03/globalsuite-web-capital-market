using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class BrokerMissing
    {

        #region Declaration
        private string strSecCode;
        private string strUserID;
        #endregion

        #region Properties
        public string SecCode
        {
            set { strSecCode = value; }
            get { return strSecCode; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        #endregion


        #region Add
        public bool Add()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BrokerMissingAdd") as SqlCommand;
            db.AddInParameter(oCommand, "SecCode", SqlDbType.VarChar, strSecCode.ToUpper());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, strUserID);
            db.ExecuteNonQuery(oCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("BrokerMissingSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
