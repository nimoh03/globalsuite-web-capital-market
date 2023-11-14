using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class CustomerName
    {
        #region Declarations
        private string strCustID, strSurname, strFirstname;
        private string strOthername;
        private byte[] imgSignature;
        private byte[] imgPhoto;
        private int intClientType;
        private string strClientTypeName;
        #endregion

        #region Properties
        public byte[] Photo
        {
            set { imgPhoto = value; }
            get { return imgPhoto; }
        }

        public byte[] Signature
        {
            set { imgSignature = value; }
            get { return imgSignature; }
        }

        public string CustID
        {
            set { strCustID = value; }
            get { return strCustID; }
        }
        public string Surname
        {
            set { strSurname = value; }
            get { return strSurname; }
        }
        public string Firstname
        {
            set { strFirstname = value; }
            get { return strFirstname; }
        }
        public string Othername
        {
            set { strOthername = value; }
            get { return strOthername; }
        }
        
        public string ClientTypeName
        {
            set { strClientTypeName = value; }
            get { return strClientTypeName; }
        }
        
        public int ClientType
        {
            set { intClientType = value; }
            get { return intClientType; }
        }

        #endregion

        #region Save
        public void Save()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerNameAdd") as SqlCommand;
            db.AddInParameter(oCommand, "CustID", SqlDbType.VarChar, strCustID.Trim());
            db.AddInParameter(oCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(oCommand, "Othername", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(oCommand, "Firstname", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(oCommand, "Photo", SqlDbType.Image, imgPhoto);
            db.AddInParameter(oCommand, "Signature", SqlDbType.Image, imgSignature);
            db.AddInParameter(oCommand, "ClientType", SqlDbType.Int, intClientType);
            db.AddInParameter(oCommand, "ClientTypeName", SqlDbType.VarChar, strClientTypeName.Trim());
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Delete
        public void Delete()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingDeleteCustomerName") as SqlCommand;
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Customer Select All For Customer Name
        public DataSet CustomerSelectForCustomerName()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerSelectForCustomerName") as SqlCommand;
            return db.ExecuteDataSet(oCommand);
        }
        #endregion

        #region Customer Select All For Customer Name With Out Image
        public DataSet CustomerSelectForCustomerNameWithOutImage()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerSelectForCustomerNameWithOutImage") as SqlCommand;
            return db.ExecuteDataSet(oCommand);
        }
        #endregion

        
    }
}
