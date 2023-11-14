using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace CapitalMarket.Business
{
    public class MergeCustomer
    {
        #region Declaration
        private string strTransNo, strAcctID;
        private string strSubID;
        private string strUserId,strSaveType;
        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string AcctID
        {
            set { strAcctID = value; }
            get { return strAcctID; }
        }
        public string SubID
        {
            set { strSubID = value; }
            get { return strSubID; }
        }

        public string UserId
        {
            set { strUserId = value; }
            get { return strUserId; }
        }

        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion


        #region Add New Merge Master Account
        public SqlCommand AddMasterCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeMasterAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "AcctID", SqlDbType.VarChar, strAcctID.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "MERGE");
            return oCommand;
        }
        #endregion

        #region Edit Merge Master Account
        public SqlCommand EditMasterCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeMasterEdit") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "AcctID", SqlDbType.VarChar, strAcctID.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            return oCommand;
        }
        #endregion


        #region Add New Merge Sub Account
        public SqlCommand AddSubCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubAddNew") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "AcctID", SqlDbType.VarChar, strAcctID.Trim());
            db.AddInParameter(oCommand, "SubID", SqlDbType.VarChar, strSubID.Trim());
            db.AddInParameter(oCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.AddInParameter(oCommand, "TableName", SqlDbType.VarChar, "MERGESUB");
            return oCommand;
        }
        #endregion

        #region Delete Merge Master Account By Master Account
        public SqlCommand DeleteMasterCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeMasterDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Merge Sub Account By Master Account
        public SqlCommand DeleteSubByMasterCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubDeleteByMaster") as SqlCommand;
            db.AddInParameter(oCommand, "AcctID", SqlDbType.VarChar, strAcctID.Trim());
            return oCommand;
        }
        #endregion

        #region Delete Single Merge Sub Account By Master Account
        public SqlCommand DeleteSingleSubByMasterCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubSingleDeleteByMaster") as SqlCommand;
            db.AddInParameter(oCommand, "AcctID", SqlDbType.VarChar, strAcctID.Trim());
            db.AddInParameter(oCommand, "SubID", SqlDbType.VarChar, strSubID.Trim());
            return oCommand;
        }
        #endregion

        #region Get All 
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All With Customer Details
        public DataSet GetAllWithCustDetails()
        {
            StkParam oStkParam = new StkParam();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterSelectAllWithCustDetails") as SqlCommand;
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar, oStkParam.Product.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All With Customer Details - Search
        public DataSet GetAllWithCustDetailsBySurnameSearch(string strSurname,string strOthername,string strFirstname,string strProduct,string strAllName)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterSelectAllWithCustDetailsBySurnameSearch") as SqlCommand;
            db.AddInParameter(dbCommand, "Surname", SqlDbType.VarChar, strSurname.Trim());
            db.AddInParameter(dbCommand, "CustNo", SqlDbType.VarChar, strSubID.Trim());
            db.AddInParameter(dbCommand, "OtherName", SqlDbType.VarChar, strOthername.Trim());
            db.AddInParameter(dbCommand, "FirstName", SqlDbType.VarChar, strFirstname.Trim());
            db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar,strProduct.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubChkCustomerNumberExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "SubID", SqlDbType.VarChar, strSubID.Trim());
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
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strAcctID = thisRow[0]["AcctID"].ToString();
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
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeMasterSelectByCustomerNo") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctID", SqlDbType.VarChar, strAcctID.Trim());
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
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeSubSelectByCustomerNo") as SqlCommand;
            db.AddInParameter(dbCommand, "AcctID", SqlDbType.VarChar, strAcctID.Trim());
            db.AddInParameter(dbCommand, "ProductCode", SqlDbType.VarChar,oStkParam.Product.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Master Customer 
        public string GetMasterCustomer()
        {
            string strResult = "";
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeSubSelectMaster") as SqlCommand;
            db.AddInParameter(dbCommand, "SubID", SqlDbType.VarChar, strSubID.Trim());
            if (db.ExecuteScalar(dbCommand) != null)
            {
                strResult = db.ExecuteScalar(dbCommand).ToString();
            }
            return strResult;
        }
        #endregion
        

        #region Get Account Balance Of Merge Customer With Previous Date Range
        public Decimal GetAccountBalanceByMergeCustomerPreviousDate(string strAcctRef, DateTime datEffectiveDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeSubAccountBalanceByMasterCustomerPreviousDate", strAcctRef.Trim(), strAcctID.Trim(), datEffectiveDate) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Get Account Balance Of Merge Customer 
        public Decimal GetAccountBalanceByMergeCustomer(string strAcctRef)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeSubAccountBalanceByMasterCustomer", strAcctRef.Trim(), strAcctID.Trim()) as SqlCommand;
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString() != "")
            {
                return (decimal)db.ExecuteScalar(dbCommand);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Check That Customer No Exist At All
        public bool CustomerNumberExistAtAll()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeSubChkCustomerNumberExistAtAll") as SqlCommand;
            db.AddInParameter(oCommand, "SubID", SqlDbType.VarChar, strSubID.Trim());
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

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("MergeMasterChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar,strAcctID.Trim());
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

        #region Get All Transactions By Non Upload Online
        public DataSet GetAllByNonUploadOnline()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("MergeSubSelectByNonUploadOnline") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion
    }
}
