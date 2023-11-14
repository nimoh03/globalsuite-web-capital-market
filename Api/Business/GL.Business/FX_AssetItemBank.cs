using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class FX_AssetItemBank
    {
        #region Declarations
        private string strTransNo;
        private string strPayBankAcct;
        private decimal decPayAmount;
        private int intAssetItem;
        private DateTime datEffDate;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public decimal PayAmount
        {
            set { decPayAmount = value; }
            get { return decPayAmount; }
        }
        public int AssetItem
        {
            set { intAssetItem = value; }
            get { return intAssetItem; }
        }
        public string PayBankAcct
        {
            set { strPayBankAcct = value; }
            get { return strPayBankAcct; }
        }
        public DateTime EffDate
        {
            set { datEffDate = value; }
            get { return datEffDate; }
        }
        
        #endregion

        #region Add Command
        public SqlCommand AddCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            dbCommand = db.GetStoredProcCommand("FX_AssetItemBankAdd") as SqlCommand;
            db.AddInParameter(dbCommand, "AssetItem", SqlDbType.Int,intAssetItem);
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "EffDate", SqlDbType.DateTime, datEffDate);
            db.AddInParameter(dbCommand, "PayAmount", SqlDbType.Decimal, decPayAmount);
            db.AddInParameter(dbCommand, "PayBankAcct", SqlDbType.VarChar, strPayBankAcct.Trim());
            return dbCommand;
        }
        #endregion


        #region Get All 
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("FX_AssetItemBankSelectAllAssetItem") as SqlCommand;
            db.AddInParameter(dbCommand, "AssetItem", SqlDbType.Int, intAssetItem);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete And Return Command
        public SqlCommand DeleteCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("FX_AssetItemBankDeleteByAssetItem") as SqlCommand;
            db.AddInParameter(oCommand, "AssetItem", SqlDbType.Int, intAssetItem);
            return oCommand;
        }
        #endregion
    }
}
