using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class KYCDocType
    {
        #region Declaration
        private string strTransNo;
        private string strKYCDocTypeName;
        private string strDocumentType;
        private string strSaveType;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string KYCDocTypeName
        {
            set { strKYCDocTypeName = value; }
            get { return strKYCDocTypeName; }
        }
        public string DocumentType
        {
            set { strDocumentType = value; }
            get { return strDocumentType; }
        }
        
        public string SaveType
        {
            set { strSaveType = value; }
            get { return strSaveType; }
        }
        #endregion


        #region Save Return Command
        public Int64 SaveCommand()
        {
            if (!ChkTransNoExist())
            {
                throw new Exception("Cannot Save KYC Document Type Code Does Not Exist");
            }
            if (ChkNameExist())
            {
                throw new Exception("Cannot Save KYC Document Type Name Exist");
            }
            
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeAddNew") as SqlCommand;
                db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 8);
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeEdit") as SqlCommand;
                db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            }
            db.AddInParameter(dbCommand, "KYCDocTypeName", SqlDbType.VarChar, strKYCDocTypeName.Trim().ToUpper());
            db.AddInParameter(dbCommand, "DocumentType", SqlDbType.VarChar, strDocumentType.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
            return Convert.ToInt64(db.GetParameterValue(dbCommand, "TransNo").ToString());
        }
        #endregion

        #region Save Image Return Command
        public void SaveImageCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = null;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeEdit") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "KYCDocTypeName", SqlDbType.VarChar, strKYCDocTypeName.Trim().ToUpper());
            db.AddInParameter(dbCommand, "DocumentType", SqlDbType.VarChar, strDocumentType.Trim());
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);

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
                oCommand = db.GetStoredProcCommand("KYCDocTypeChkTransNoExist") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
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
            SqlCommand oCommand = db.GetStoredProcCommand("KYCDocTypeChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.Char, strTransNo.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar, strKYCDocTypeName.Trim().ToUpper());
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

        #region Get All
        public DataSet GetAll()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get KYCDocType
        public bool GetKYCDocType()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                strKYCDocTypeName = thisRow[0]["KYCDocTypeName"].ToString();
                strDocumentType = thisRow[0]["DocumentType"] != null && thisRow[0]["DocumentType"].ToString() != "" ? thisRow[0]["DocumentType"].ToString() : "";
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion


        #region Get All KYCDocTypeForCustomerType By Customer Type Id Return List
        public List<KYCDocType> GetAllKYCDocTypeForCustomerTypeByCustomerTypeIdReturnList(Int32 intCustomerType)
        {
            List<KYCDocType> lstKYCDocType = new List<KYCDocType>();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelectAllByCustomerTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerType);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            foreach (DataRow oRow in oDS.Tables[0].Rows)
            {
                KYCDocType oKYCDocType = new KYCDocType();
                oKYCDocType.strTransNo = oRow["KYCDocTypeId"].ToString();
                oKYCDocType.KYCDocTypeName = oRow["KYCDocTypeName"].ToString();
                lstKYCDocType.Add(oKYCDocType);
            }

            KYCDocType oKYCDocTypePhoto = new KYCDocType();
            oKYCDocTypePhoto.strTransNo = "999999999998";
            oKYCDocTypePhoto.KYCDocTypeName = "PASSPORT PHOTO";
            lstKYCDocType.Add(oKYCDocTypePhoto);

            KYCDocType oKYCDocTypeSignature = new KYCDocType();
            oKYCDocTypeSignature.strTransNo = "999999999999";
            oKYCDocTypeSignature.KYCDocTypeName = "SIGNATURE";
            lstKYCDocType.Add(oKYCDocTypeSignature);
            return lstKYCDocType;
        }
        #endregion
    }
}
