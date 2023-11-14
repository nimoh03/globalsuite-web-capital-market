using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class MergeCustomerDelete
    {
        #region Declaration
        private string strTransNo, strMasterAccount, strSubAccount, strCustomerName;
        private string strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string MasterAccount
        {
            set { strMasterAccount = value; }
            get { return strMasterAccount; }
        }
        public string SubAccount
        {
            set { strSubAccount = value; }
            get { return strSubAccount; }
        }
        public string CustomerName
        {
            set { strCustomerName = value; }
            get { return strCustomerName; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion


        #region Add Merge Master Account
        public SqlCommand AddMasterCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeMasterDeleteAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "MasterAccount", SqlDbType.VarChar, strMasterAccount.Trim());
            return oCommand;
        }
        #endregion


        #region Add New Merge Sub Account
        public SqlCommand AddSubCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubDeleteAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "MasterAccount", SqlDbType.VarChar, strMasterAccount.Trim());
            db.AddInParameter(oCommand, "SubAccount", SqlDbType.VarChar, strSubAccount.Trim());
            db.AddInParameter(oCommand, "CustomerName", SqlDbType.VarChar, strCustomerName.Trim());
            return oCommand;
        }
        #endregion

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterDeleteSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All With Customer Details
        public DataSet GetAllWithCustDetails()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterDeleteSelectAllWithCustDetails") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All With Customer Details - Search
        public DataSet GetAllWithCustDetailsBySurnameSearch(string strSurname, string strOthername, string strFirstname, string strProduct, string strAllName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterDeleteSelectAllWithCustDetailsBySurnameSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(dbCommand, "SubAccount", SqlDbType.VarChar, strSubAccount.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            db.AddInParameter(dbCommand, "AllName", SqlDbType.VarChar, strAllName.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion


        #region Check That Customer No Already Exist
        public bool CustomerNumberExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubDeleteChkCustomerNumberExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "SubAccount", SqlDbType.VarChar, strSubAccount.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get Merge Customer
        public bool GetMergeCustomer()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterDeleteSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strSubAccount = thisRow[0]["SubAccount"].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get Merge Customer By Customer No
        public bool GetMergeCustomerByCustNo()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterDeleteSelectByCustomerNo") as SqlCommand;
            db.AddInParameter(dbCommand, "SubAccount", SqlDbType.VarChar, strSubAccount.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo "].ToString();
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }

            return blnStatus;
        }
        #endregion

        #region Get Sub Customer By Customer No
        public DataSet GetSubCustomer()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeSubDeleteSelectByCustomerNo") as SqlCommand;
            db.AddInParameter(dbCommand, "SubAccount", SqlDbType.VarChar, strSubAccount.Trim());
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Master Customer
        public string GetMasterCustomer()
        {
            string strResult = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeSubDeleteSelectMaster") as SqlCommand;
            db.AddInParameter(dbCommand, "MasterAccount", SqlDbType.VarChar, strMasterAccount.Trim());
            if (db.ExecuteScalar(dbCommand) != null)
            {
                strResult = db.ExecuteScalar(dbCommand).ToString();
            }
            return strResult;
        }
        #endregion

        #region Check That Customer No Exist At All
        public bool CustomerNumberExistAtAll()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubDeleteChkCustomerNumberExistAtAll") as SqlCommand;
            db.AddInParameter(oCommand, "SubAccount", SqlDbType.VarChar, strSubAccount.Trim());
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count > 0)
            {
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion
    }
}
