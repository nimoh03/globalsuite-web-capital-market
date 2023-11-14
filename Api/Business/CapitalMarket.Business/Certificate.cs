using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class Certificate
    {
        public void UpdateVerifiedCertificate(string[] acctList)
        {
            SqlDatabase db = DatabaseFactory.CreateDatabase("GlobalStockDB") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("SaveTradeLogCert") as SqlCommand;

 
            db.AddInParameter(dbCommand, "@ACCOUNTNO", SqlDbType.VarChar, acctList[0]);
            db.AddInParameter(dbCommand, "@ACCOUNTNAME", SqlDbType.VarChar, acctList[1]);
            db.AddInParameter(dbCommand, "@SHARESYMBOL", SqlDbType.VarChar, acctList[2]);
            db.AddInParameter(dbCommand, "@CERTIFICATENO", SqlDbType.VarChar, acctList[3]);
            db.AddInParameter(dbCommand, "@QTY", SqlDbType.VarChar, acctList[4]);
            db.AddInParameter(dbCommand, "@DEPOSITDATE", SqlDbType.VarChar, acctList[5]);

            db.ExecuteNonQuery(dbCommand);

        }
        public DataTable getMissingAccount()
        {
            SqlDatabase db = DatabaseFactory.CreateDatabase("GlobalStockDB") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("getMissingCust_Account") as SqlCommand;
            DataSet ds = db.ExecuteDataSet(dbCommand);
            return ds.Tables[0];

        }
    }
}
