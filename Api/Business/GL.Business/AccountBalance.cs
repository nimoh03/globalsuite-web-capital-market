using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class AccountBalance
    {
        #region Declaration
        private string strAcctRef,strCustomerId, strAccountId;
        private decimal decCumYrToDtBal;
        private DateTime datFinEndDate;
        #endregion

        #region Properties
        public string AcctRef
        {
            set { strAcctRef = value; }
            get { return strAcctRef; }
        }
        public string CustomerId
        {
            set { strCustomerId = value; }
            get { return strCustomerId; }
        }
        public string AccountId
        {
            set { strAccountId = value; }
            get { return strAccountId; }
        }
        public decimal CumYrToDtBal
        {
            set { decCumYrToDtBal = value; }
            get { return decCumYrToDtBal; }
        }
        public DateTime FinEndDate
        {
            set { datFinEndDate = value; }
            get { return datFinEndDate; }
        }
        #endregion


        #region Add All GL Account Balance And Return The Command Object
        public SqlCommand AddAllGLAccountBalance(DateTime dtDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctBalanceAddAllGLAccountBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, dtDateTo);
            return dbCommand;
        }
        #endregion

        #region Add All Customer Account Balance And Return The Command Object
        public SqlCommand AddAllCustomerAccountBalance(DateTime dtDateTo)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AcctBalanceAddAllCustomerAccountBalance") as SqlCommand;
            db.AddInParameter(dbCommand, "DateTo", SqlDbType.DateTime, dtDateTo);
            return dbCommand;
        }
        #endregion

        #region Delete Account Balance And Return The Command Object
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AcctBalanceDelete") as SqlCommand;
            return oCommand;
        }
        #endregion

    }

}
