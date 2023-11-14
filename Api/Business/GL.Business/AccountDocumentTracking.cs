using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class AccountDocumentTracking
    {
        #region Declarations
        private string strAccountTransactionType;
        private string strTransactionId;
        private byte[] imgDocumentPhoto;
        #endregion

        #region Properties
        public string AccountTransactionType
        {
            set { strAccountTransactionType = value; }
            get { return strAccountTransactionType; }
        }
        public string TransactionId
        {
            set { strTransactionId = value; }
            get { return strTransactionId; }
        }
        public byte[] DocumentPhoto
        {
            set { imgDocumentPhoto = value; }
            get { return imgDocumentPhoto; }
        }
        

        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("AccountDocumentTrackingDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "AccountTransactionType", SqlDbType.VarChar, strAccountTransactionType.Trim());
            db.AddInParameter(dbCommand, "TransactionId", SqlDbType.VarChar, strTransactionId.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Save
        public void Save()
        {
           
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("AccountDocumentTrackingAdd") as SqlCommand;
            db.AddInParameter(oCommand, "AccountTransactionType", SqlDbType.VarChar, strAccountTransactionType.Trim());
            db.AddInParameter(oCommand, "TransactionId", SqlDbType.VarChar, strTransactionId.Trim());
            db.AddInParameter(oCommand, "DocumentPhoto", SqlDbType.Image, imgDocumentPhoto);
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

    }
}
