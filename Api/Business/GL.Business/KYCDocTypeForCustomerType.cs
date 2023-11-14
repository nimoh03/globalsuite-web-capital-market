using System;
using System.Data;
using System.Data.SqlClient;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class KYCDocTypeForCustomerType
    {
        #region Declaration
        private string strTransNo;
        private Int64 intKYCDocTypeId;
        private Int32 intCustomerTypeId;
        private bool blnIsOptional;
        private string strSaveType;

        #endregion

        #region Properties
        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public Int64 KYCDocTypeId
        {
            set { intKYCDocTypeId = value; }
            get { return intKYCDocTypeId; }
        }
        public Int32 CustomerTypeId
        {
            set { intCustomerTypeId = value; }
            get { return intCustomerTypeId; }
        }
        public bool IsOptional
        {
            set { blnIsOptional = value; }
            get { return blnIsOptional; }
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
            Int64 intReturnNumber = 0;
            SqlCommand dbCommand = null;
            if (!ChkTransNoExist())
            {
                if (strSaveType == "EDIT")
                {
                    throw new Exception("Cannot Edit Document Type For Customer Type. KYC Document Does Exist For This Customer Type");
                }
                else if (strSaveType == "ADDS")
                {
                    throw new Exception("Cannot Add New Document Type For Customer Type. KYC Document Exist For This Customer Type");
                }
            }
            


            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeEdit") as SqlCommand;
            }
            db.AddOutParameter(dbCommand, "TransNo", SqlDbType.VarChar, 8);
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            db.AddInParameter(dbCommand, "IsOptional", SqlDbType.Bit, blnIsOptional);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
            intReturnNumber = Convert.ToInt64(db.GetParameterValue(dbCommand, "TransNo"));
        
            return intReturnNumber;
        }
        #endregion

        #region Save Image Return Command
        public void SaveImageCommand()
        {
            SqlCommand dbCommand = null;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            if (strSaveType.Trim() == "ADDS")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeAddNew") as SqlCommand;
            }
            else if (strSaveType.Trim() == "EDIT")
            {
                dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeEdit") as SqlCommand;
            }
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            db.AddInParameter(dbCommand, "IsOptional", SqlDbType.Bit, blnIsOptional);
            db.AddInParameter(dbCommand, "UserId", SqlDbType.VarChar, GeneralFunc.UserName.Trim());
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Remove Customer Type Return Command
        public void RemoveCustomerTypeIdCommand()
        {
            CustomerDocumentTracking oCustomerDocumentTracking = new CustomerDocumentTracking();
            if (oCustomerDocumentTracking.ChkKYCDocTypeIdAndCustomerTypeIdExist(intCustomerTypeId))
            {
                throw new Exception("Cannot Remove Document Type For Customer Type. KYC Document Exist For This Customer Type");
            }
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeRemoveCustomerTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            db.ExecuteNonQuery(dbCommand);
        }
        #endregion

        #region Remove Customer Type Image Return Command
        public void RemoveCustomerTypeIdImageCommand()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeRemoveCustomerTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            db.ExecuteNonQuery(dbCommand);

        }
        #endregion

        #region Check TransNo Exist
        public bool ChkTransNoExist()
        {
            bool blnStatus = false;

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = null;
            oCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelectByKYCDOcTypeIdAndCustomerTypeId") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            DataSet oDs = db.ExecuteDataSet(oCommand);
            if (oDs.Tables[0].Rows.Count == 1 && strSaveType == "EDIT")
            {
                blnStatus = true;
            }
            else if (oDs.Tables[0].Rows.Count == 0 && strSaveType == "ADDS")
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
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelectAll") as SqlCommand;
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By KYCDocTypeId
        public DataSet GetAllByKYCDocTypeId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelectAllByKYCDocTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Customer Type Id
        public DataSet GetAllByCustomerTypeId()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelectAllByCustomerTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get All By Customer Type Id Not Optional
        public DataSet GetAllByCustomerTypeIdNotOptional()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelectAllByCustomerTypeIdNotOptional") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get KYCDocTypeForCustomerType
        public bool GetKYCDocTypeForCustomerType()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intKYCDocTypeId = Convert.ToInt64(thisRow[0]["KYCDocTypeId"]);
                intCustomerTypeId = Convert.ToInt32(thisRow[0]["CustomerTypeId"]);
                blnIsOptional = thisRow[0]["IsOptional"] != null && thisRow[0]["IsOptional"].ToString().Trim() != ""
                    ? Convert.ToBoolean(thisRow[0]["IsOptional"]) : false;
                blnStatus = true;
            }
            else
            {
                blnStatus = false;
            }
            return blnStatus;
        }
        #endregion

        #region Get KYCDocTypeForCustomerType By KYCDOcTypeId And Customer Type Id
        public bool GetKYCDocTypeForCustomerTypeByKYCDOcTypeIdAndCustomerTypeId()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("KYCDocTypeForCustomerTypeSelectByKYCDOcTypeIdAndCustomerTypeId") as SqlCommand;
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int, intCustomerTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strTransNo = thisRow[0]["TransNo"].ToString();
                intKYCDocTypeId = Convert.ToInt64(thisRow[0]["KYCDocTypeId"]);
                intCustomerTypeId = Convert.ToInt32(thisRow[0]["CustomerTypeId"]);
                blnIsOptional = thisRow[0]["IsOptional"] != null && thisRow[0]["IsOptional"].ToString().Trim() != ""
                    ? Convert.ToBoolean(thisRow[0]["IsOptional"]) : false;
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
