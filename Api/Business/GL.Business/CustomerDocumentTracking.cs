using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BaseUtility.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace GL.Business
{
    public class CustomerDocumentTracking
    {
        #region Declarations
        private string strTransNo;
        private string strCustomerId;
        private Int64 intKYCDocTypeId;
        private byte[] imgDocumentPhoto;
        private string strDocumentFileName;
        private string strIdentificationType, strIdentificationNo;
        private DateTime datExpiryDate;
        private string strSaveType;
        IFormatProvider format = new CultureInfo("en-GB");
        public string IssuingAuthority { get; set; }
        #endregion

        #region Properties

        public string TransNo
        {
            set { strTransNo = value; }
            get { return strTransNo; }
        }
        public string CustomerId
        {
            set { strCustomerId = value; }
            get { return strCustomerId; }
        }
        public byte[] DocumentPhoto
        {
            set { imgDocumentPhoto = value; }
            get { return imgDocumentPhoto; }
        }
        public string DocumentFileName
        {
            set { strDocumentFileName = value; }
            get { return strDocumentFileName; }
        }
        public Int64 KYCDocTypeId
        {
            set { intKYCDocTypeId = value; }
            get { return intKYCDocTypeId; }
        }
        public string IdentificationType
        {
            set { strIdentificationType = value; }
            get { return strIdentificationType; }
        }
        public string IdentificationNo
        {
            set { strIdentificationNo = value; }
            get { return strIdentificationNo; }
        }
        
        public DateTime ExpiryDate
        {
            set { datExpiryDate = value; }
            get { return datExpiryDate; }
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
            if (ChkNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.NameExistAdd;
                return enSaveStatus;
            }

            if (ChkDocumentTypeNameExist())
            {
                enSaveStatus = DataGeneral.SaveStatus.HeadOfficeExistAdd;
                return enSaveStatus;
            }

            

            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = null;
            if (strSaveType == "ADDS")
            {
                oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingAdd") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, "");
            }
            else if (strSaveType == "EDIT")
            {
                oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingEdit") as SqlCommand;
                db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo);
            }
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(oCommand, "DocumentPhoto", SqlDbType.Image, imgDocumentPhoto);
            db.AddInParameter(oCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "IdentificationType", SqlDbType.VarChar, strIdentificationType.Trim());
            db.AddInParameter(oCommand, "IdentificationNo", SqlDbType.VarChar, strIdentificationNo.Trim());
            if (datExpiryDate != DateTime.MinValue)
            {
                db.AddInParameter(oCommand, "ExpiryDate", SqlDbType.DateTime, datExpiryDate);
            }
            else
            {
                db.AddInParameter(oCommand, "ExpiryDate", SqlDbType.DateTime, null);
            }
            db.AddInParameter(oCommand, "IssuingAuthority", SqlDbType.VarChar, IssuingAuthority.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar,GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
            enSaveStatus = DataGeneral.SaveStatus.Saved;
            return enSaveStatus;
        }
        #endregion

        #region Delete
        public bool Delete()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;

            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Check Name Exist
        public bool ChkNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingChkNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(oCommand, "Name", SqlDbType.VarChar,strDocumentFileName.Trim());
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType);
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

        #region Check Document Type Name Exist
        public bool ChkDocumentTypeNameExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingChkDocumentTypeNameExist") as SqlCommand;
            db.AddInParameter(oCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "SaveType", SqlDbType.VarChar, strSaveType);
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


        


        



        #region Get Customer Document By Customer
        public DataSet GetCustomerDocumentByCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelectByCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Get Customer Document By Customer List
        public DataSet GetCustomerDocumentByCustomerList(int CustomerTypeId)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelectByCustomerList") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(dbCommand, "CustomerTypeId", SqlDbType.Int,CustomerTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;

        }
        #endregion

        #region Get Customer Document Image By FileName and Customer
        public byte[] GetCustomerDocumentImageByFileNameandCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelectImageByFileNameCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(dbCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
            if (db.ExecuteScalar(dbCommand) != null && db.ExecuteScalar(dbCommand).ToString().Trim() != "")
            {
                return (byte[])(db.ExecuteScalar(dbCommand));
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region Get Customer Document Type By FileName and Customer
        public string GetCustomerDocumentTypeByFileNameandCustomer()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelectTypeByFileNameCustomer") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(dbCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
            var strDocumentTypeReturn = db.ExecuteScalar(dbCommand);
            return strDocumentTypeReturn != null ? strDocumentTypeReturn.ToString() : "";
        }
        #endregion

        #region Get Customer Document
        public bool GetCustomerDocument()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelect") as SqlCommand;
            db.AddInParameter(dbCommand, "TransNo", SqlDbType.VarChar, strTransNo.Trim());
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustomerId = thisRow[0]["CustomerId"].ToString();
                intKYCDocTypeId = Convert.ToInt64(thisRow[0]["DocumentType"]);
                strDocumentFileName = thisRow[0]["DocumentFileName"] != null ? thisRow[0]["DocumentFileName"].ToString() : "";
                imgDocumentPhoto = thisRow[0]["DocumentPhoto"] != null ? (byte[])thisRow[0]["DocumentPhoto"] : null;
                strIdentificationType = thisRow[0]["IdentificationType"] != null ? thisRow[0]["IdentificationType"].ToString() : "";
                strIdentificationNo = thisRow[0]["IdentificationNo"] != null ? thisRow[0]["IdentificationNo"].ToString() : "";
                datExpiryDate = thisRow[0]["ExpiryDate"] != null && thisRow[0]["ExpiryDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["ExpiryDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                IssuingAuthority = thisRow[0]["IssuingAuthority"] != null ? thisRow[0]["IssuingAuthority"].ToString() : "";
                blnStatus = true;
            }
            
            return blnStatus;
           
        }
        #endregion

        

        #region Delete Document Same Database
        public bool DeleteDocumentSameDatabase()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentDelete") as SqlCommand;
            db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.ExecuteNonQuery(dbCommand);
            blnStatus = true;
            return blnStatus;
        }
        #endregion

        #region Save Document Same Database
        public void SaveDocumentSameDatabase()
        {
           
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuitedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentAdd") as SqlCommand;
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, strCustomerId.Trim());
            db.AddInParameter(oCommand, "DocumentPhoto", SqlDbType.Image, imgDocumentPhoto);
            db.AddInParameter(oCommand, "DocumentFileName", SqlDbType.VarChar, strDocumentFileName.Trim());
            db.AddInParameter(oCommand, "UserID", SqlDbType.VarChar, GeneralFunc.UserName);
            db.ExecuteNonQuery(oCommand);
        }
        #endregion

        #region Get All Expired Means Of Id
        public DataSet GetAllExpiredMeansOfId(DateTime datCurrentDate)
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelectExpiredMeansOfId") as SqlCommand;
            db.AddInParameter(dbCommand, "DateToCheck", SqlDbType.DateTime, datCurrentDate);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

        #region Update Customer Name
        public void UpdateCustomerName(string strCustomerNo,bool blnAddPhotoAndSignature)
        {
            AppSettingsReader oAppSettingsReader = new AppSettingsReader();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            DatabaseProviderFactory factoryGlobalSuite = new DatabaseProviderFactory(); SqlDatabase dbGlobalSuite = factoryGlobalSuite.Create("GlobalSuitedb") as SqlDatabase;

            GLParam oGLParam = new GLParam();
            oGLParam.Type = "IMAGEDATAINDIFFLOCATION";
            if (oGLParam.CheckParameter().Trim() != "YES")
            {
                SqlCommand dbCommand;
                if (blnAddPhotoAndSignature)
                {
                    dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingUpdateCustomerName") as SqlCommand;
                }
                else
                {
                    dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingUpdateCustomerNameWithOutPhotoSignature") as SqlCommand;
                }
                db.AddInParameter(dbCommand, "CustomerId", SqlDbType.VarChar, strCustomerNo.Trim());
                db.AddInParameter(dbCommand, "DatabaseName", SqlDbType.VarChar, oAppSettingsReader.GetValue("DatabaseName", typeof(string)).ToString());
                db.ExecuteNonQuery(dbCommand);
            }
            else
            {
                SqlCommand dbCommandDeleteCustomerName = db.GetStoredProcCommand("CustomerDocumentTrackingDeleteCustomerName") as SqlCommand;
                db.ExecuteNonQuery(dbCommandDeleteCustomerName);
                SqlCommand dbCommandCustomer;
                if (blnAddPhotoAndSignature)
                {
                    dbCommandCustomer = dbGlobalSuite.GetStoredProcCommand("CustomerSelectWithCustomerType") as SqlCommand;
                }
                else
                {
                    dbCommandCustomer = dbGlobalSuite.GetStoredProcCommand("CustomerSelectWithCustomerTypeWithOutPhotoSignature") as SqlCommand;
                }
                dbGlobalSuite.AddInParameter(dbCommandCustomer, "CustomerId", SqlDbType.VarChar, strCustomerNo.Trim());
                DataTable oDTblCustomerByClientType = dbGlobalSuite.ExecuteDataSet(dbCommandCustomer).Tables[0];

                using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        foreach (DataColumn col in oDTblCustomerByClientType.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "CustomerName";
                        bulkCopy.WriteToServer(oDTblCustomerByClientType);
                    }
                }

            }
        }
        #endregion

        #region Update Customer Name All
        public void UpdateCustomerNameAll(bool blnAddPhotoAndSignature)
        {
            CustomerName oCustomerName = new CustomerName();
            oCustomerName.Delete();

            AppSettingsReader oAppSettingsReader = new AppSettingsReader();
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase; 
            DatabaseProviderFactory factoryGlobalSuite = new DatabaseProviderFactory(); SqlDatabase dbGlobalSuite = factoryGlobalSuite.Create("GlobalSuitedb") as SqlDatabase;

            GLParam oGLParam = new GLParam();
            oGLParam.Type = "IMAGEDATAINDIFFLOCATION";
            if (oGLParam.CheckParameter().Trim() != "NO")
            {
                DataTable oDTblCustomerByClientTypeDiffLocation;
                if (blnAddPhotoAndSignature)
                {
                    oDTblCustomerByClientTypeDiffLocation = oCustomerName.CustomerSelectForCustomerName().Tables[0];
                }
                else
                {
                    oDTblCustomerByClientTypeDiffLocation = oCustomerName.CustomerSelectForCustomerNameWithOutImage().Tables[0];
                }
                using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        foreach (DataColumn col in oDTblCustomerByClientTypeDiffLocation.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "CustomerName";
                        bulkCopy.WriteToServer(oDTblCustomerByClientTypeDiffLocation);
                    }
                }
            }
            else
            {
                SqlCommand dbCommandCustomer;
                if (blnAddPhotoAndSignature)
                {
                    dbCommandCustomer = dbGlobalSuite.GetStoredProcCommand("CustomerSelectAllWithCustomerType") as SqlCommand;
                }
                else
                {
                    dbCommandCustomer = dbGlobalSuite.GetStoredProcCommand("CustomerSelectAllWithCustomerTypeWithOutPhotoSignature") as SqlCommand;
                }
                DataTable oDTblCustomerByClientType = dbGlobalSuite.ExecuteDataSet(dbCommandCustomer).Tables[0];


                using (SqlConnection connection = db.CreateConnection() as SqlConnection)
                {
                    connection.Open();
                    using (var bulkCopy = new SqlBulkCopy(connection))
                    {
                        foreach (DataColumn col in oDTblCustomerByClientType.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "CustomerName";
                        bulkCopy.WriteToServer(oDTblCustomerByClientType);
                    }
                }

            }
        }
        #endregion

        #region Check KYCDocTypeId And Customer Type Id Exist
        public bool ChkKYCDocTypeIdAndCustomerTypeIdExist(Int32 CustomerTypeId)
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingChkKYCDocTypeIdAndCustomerTypeIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "CustomerTypeId", SqlDbType.Int, CustomerTypeId);
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

        #region Get Customer Document  ByKYCDocTypeId And CustomerId
        public bool GetCustomerDocumentByKYCDocTypeIdAndCustomerId()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelectByKYCDocTypeIdAndCustomerId") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId);
            DataSet oDS = db.ExecuteDataSet(oCommand);
            DataTable thisTable = oDS.Tables[0];
            DataRow[] thisRow = thisTable.Select();
            if (thisRow.Length == 1)
            {
                strCustomerId = thisRow[0]["CustomerId"].ToString();
                intKYCDocTypeId = Convert.ToInt64(thisRow[0]["DocumentType"]);
                strDocumentFileName = thisRow[0]["DocumentFileName"] != null ? thisRow[0]["DocumentFileName"].ToString() : "";
                imgDocumentPhoto = thisRow[0]["DocumentPhoto"] != null ? (byte[])thisRow[0]["DocumentPhoto"] : null;
                strIdentificationType = thisRow[0]["IdentificationType"] != null ? thisRow[0]["IdentificationType"].ToString() : "";
                strIdentificationNo = thisRow[0]["IdentificationNo"] != null ? thisRow[0]["IdentificationNo"].ToString() : "";
                datExpiryDate = thisRow[0]["ExpiryDate"] != null && thisRow[0]["ExpiryDate"].ToString().Trim() != "" ? DateTime.ParseExact(thisRow[0]["ExpiryDate"].ToString().Substring(0, 10), "dd/MM/yyyy", format) : DateTime.MinValue;
                IssuingAuthority = thisRow[0]["IssuingAuthority"] != null ? thisRow[0]["IssuingAuthority"].ToString() : "";
                blnStatus = true;
            }

            return blnStatus;

        }
        #endregion

        #region Check KYCDocTypeId And Customer Id Exist
        public bool ChkKYCDocTypeIdAndCustomerIdExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingChkKYCDocTypeIdAndCustomerIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            db.AddInParameter(oCommand, "CustomerId", SqlDbType.VarChar, CustomerId);
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

        #region Check KYCDocTypeId Exist
        public bool ChkKYCDocTypeIdExist()
        {
            bool blnStatus = false;
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand oCommand = db.GetStoredProcCommand("CustomerDocumentTrackingChkKYCDocTypeIdExist") as SqlCommand;
            db.AddInParameter(oCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
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


        #region Get All By KYC Document Type
        public DataSet GetAllByKYCDocType()
        {
            DatabaseProviderFactory factory = new DatabaseProviderFactory(); SqlDatabase db = factory.Create("GlobalSuiteImagedb") as SqlDatabase;
            SqlCommand dbCommand = db.GetStoredProcCommand("CustomerDocumentTrackingSelectAllByKYCDocType") as SqlCommand;
            db.AddInParameter(dbCommand, "KYCDocTypeId", SqlDbType.BigInt, intKYCDocTypeId);
            DataSet oDS = db.ExecuteDataSet(dbCommand);
            return oDS;
        }
        #endregion

    }
}
