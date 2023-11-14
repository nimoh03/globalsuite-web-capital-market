using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class ProductAcctStatus
    {
        #region Declarations
        private string strTransNo, strProduct, strCustomer, strDeactivateReason,strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string Product
        {
            set { strProduct = value; }
            get { return strProduct; }
        }
        public string Customer
        {
            set { strCustomer = value; }
            get { return strCustomer; }
        }
        public string DeactivateReason
        {
            set { strDeactivateReason = value; }
            get { return strDeactivateReason; }
        }

       
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save
        public SqlCommand Save()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("ProductAcctStatusAddNew") as SqlCommand;
            }
            else if (strSaveType == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("ProductAcctStatusEdit") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "Customer", SqlDbType.VarChar, strCustomer.Trim());
            db.AddInParameter(dbCommand, "DeactivateReason", SqlDbType.VarChar, strDeactivateReason.Trim());
            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return dbCommand;
        }
        #endregion

        #region Get Product Account Status
        public bool GetProductAcctStatus()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctStatusSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strProduct = thisRow[0]["Product"].ToString();
                strCustomer = thisRow[0]["Customer"].ToString();
                strDeactivateReason = thisRow[0]["DeactivateReason"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("ProductAcctStatusSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        
    }
}
