using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class TransactionType
    {
        #region Declarations
        private string strTranCode, strTransName, strTransType, strSingleProduct;
        private string strMainControlAcct, strProduct, strPettyCash, strContraControlAcct;
        private string strUserID;
        private string strSaveType;
        #endregion

        #region Properties
        public string TranCode
        {
            set { strTranCode = value; }
            get { return strTranCode; }
        }
        public string TransName
        {
            set { strTransName = value; }
            get { return strTransName; }
        }
        public string TransType
        {
            set { strTransType = value; }
            get { return strTransType; }
        }
        public string SingleProduct
        {
            set { strSingleProduct = value; }
            get { return strSingleProduct; }
        }
        public string MainControlAcct
        {
            set { strMainControlAcct = value; }
            get { return strMainControlAcct; }
        }
        public string Product
        {
            set { strProduct = value; }
            get { return strProduct; }
        }
        public string PettyCash
        {
            set { strPettyCash = value; }
            get { return strPettyCash; }
        }
        public string ContraControlAcct
        {
            set { strContraControlAcct = value; }
            get { return strContraControlAcct; }
        }
        public string UserID
        {
            set { strUserID = value; }
            get { return strUserID; }
        }
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion

        #region Save
        public DataGeneral.SaveStatus Save()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            if (!ChkTransNoExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NotExist;
                return enSaveStatus;
            }
            if (ChkNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("TransactionTypeAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("TransactionTypeEdit") as SqlCommand;
            }

            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTranCode.Trim());
            db.AddInParameter(dbCommand, "TransName", SqlDbType.VarChar, strTransName.Trim());
            //db.AddInParameter(dbCommand, "TransType", SqlDbType.VarChar, strTransType.Trim());
            //db.AddInParameter(dbCommand, "SingleProduct", SqlDbType.VarChar, strSingleProduct.Trim());
            db.AddInParameter(dbCommand, "MainControlAcct", SqlDbType.VarChar, strMainControlAcct.Trim());
            //db.AddInParameter(dbCommand, "Product", SqlDbType.VarChar, strProduct.Trim());
            //db.AddInParameter(dbCommand, "PettyCash", SqlDbType.VarChar, strPettyCash.Trim());
            db.AddInParameter(dbCommand, "ContraControlAcct", SqlDbType.VarChar, strContraControlAcct.Trim());

            db.AddInParameter(dbCommand, "UserID", SqlDbType.VarChar,GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;

        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;
            if (strSaveType == "EDIT")
            {
                DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
                SqlCommand oCommand = null;
                oCommand = db.GetStoredProcCommand("TransactionTypeChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTranCode.Trim());
                DataSet oDs = db.ExecuteDataSet(oCommand);
                if (oDs.Tables[0].Rows.Count > 0)
                {
                    blnStatus = true;
                }
                else
                {
                    blnStatus = false;
                }
            }
            else if (strSaveType == "ADDS")
            {
                blnStatus = true;
            }

            return blnStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("TransactionTypeChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTranCode.Trim());
            db.AddInParameter(oCommand, "TransName", SqlDbType.VarChar, strTransName.Trim());
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

        #region Get Transaction Type
        public bool GetTransactionType()
        {
            IFormatProvider format = new CultureInfo("en-GB");
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("TransactionTypeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTranCode.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTranCode = thisRow[0]["TransNo"].ToString();
                strTransName = thisRow[0]["TransName"].ToString();
                //strTransType = thisRow[0]["TransType"].ToString();
                //strSingleProduct = thisRow[0]["SingleProduct"].ToString();
                strMainControlAcct = thisRow[0]["MainControlAcct"].ToString();
                //strProduct = thisRow[0]["Product"].ToString();
                //strPettyCash = thisRow[0]["PettyCash"].ToString();
                strContraControlAcct = thisRow[0]["ContraControlAcct"].ToString();

                strUserID = thisRow[0]["UserID"].ToString();
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
            SqlCommand dbCommand = db.GetStoredProcCommand("TransactionTypeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Delete
        public DataGeneral.SaveStatus Delete()
        {
            DataGeneral.SaveStatus enSaveStatus = DataGeneral.SaveStatus.Nothing;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("TransactionTypeDelete") as SqlCommand;
            db.AddInParameter(oCommand, "TranCode", SqlDbType.VarChar, strTranCode.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion
    }
}
