using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace BaseUtility.Business
{
    public class Bucket
    {

        #region Get Last Bucket Number
        private long GetLastBucketNo(string strTableName)
        {
            long lngLastNo = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("LastRecordSaved") as SqlCommand;
            db.AddOutParameter(oCommand, "NextNo", SqlDbType.BigInt, 8);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);
            lngLastNo = long.Parse(db.GetParameterValue(oCommand, "NextNo").ToString());

            return lngLastNo;
        }
        #endregion

        #region Get Last Bucket Number Second
        private long GetLastBucketNoSecond(string strTableName)
        {
            long lngLastNo = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("LastRecordSavedSecond") as SqlCommand;
            db.AddOutParameter(oCommand, "NextNo", SqlDbType.BigInt, 8);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);
            lngLastNo = long.Parse(db.GetParameterValue(oCommand, "NextNo").ToString());

            return lngLastNo;
        }
        #endregion

        #region Get Last Bucket Number For Tables That Does Not Use Identity
        private long GetLastBucketNoNonIdentity(string strTableName)
        {
            long lngLastNo = 1;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BucketSelectByTableName") as SqlCommand;
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName);
            db.AddOutParameter(oCommand, "NextNo", SqlDbType.BigInt, 8);
            db.ExecuteNonQuery(oCommand);
            lngLastNo = long.Parse(db.GetParameterValue(oCommand, "NextNo").ToString());
            return lngLastNo;
        }
        #endregion

        #region Get Next Bucket Number
        public string GetNextBucketNo(string strTableName)
        {
            long lngNextNo;
            lngNextNo = GetLastBucketNo(strTableName) + 1;
            return lngNextNo.ToString();
        }
        #endregion

        #region Get Next Bucket Number For Tables Not using Identity
        public string GetNextBucketNoNonIdentity(string strTableName)
        {
            long lngNextNo;
            lngNextNo = GetLastBucketNoNonIdentity(strTableName) + 1;
            return lngNextNo.ToString();
        }
        #endregion

        #region Get Next Bucket Number Second
        public string GetNextBucketNoSecond(string strTableName)
        {
            long lngNextNo;
            lngNextNo = GetLastBucketNoSecond(strTableName) + 1;
            return lngNextNo.ToString();
        }
        #endregion

        #region Add To Bucket Number And Return Command
        public SqlCommand AddToBucketNoCommand(string strTableName,string strUserName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("BucketAddToBucketNo") as SqlCommand;
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, strUserName.Trim());
            return oCommand;
        }
        #endregion


        #region Get Last Bucket Number Image
        private long GetLastBucketNoImage(string strTableName)
        {
            long lngLastNo = 0;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("LastRecordSaved") as SqlCommand;
            db.AddOutParameter(oCommand, "NextNo", SqlDbType.BigInt, 8);
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, strTableName.Trim());
            db.ExecuteNonQuery(oCommand);
            lngLastNo = long.Parse(db.GetParameterValue(oCommand, "NextNo").ToString());

            return lngLastNo;
        }
        #endregion

        #region Get Next Bucket Number Image
        public string GetNextBucketNoImage(string strTableName)
        {
            long lngNextNo;
            lngNextNo = GetLastBucketNoImage(strTableName) + 1;
            return lngNextNo.ToString();
        }
        #endregion
    }
}
